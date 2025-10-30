using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace QLNhaThuoc
{
    public class FormImportReport : Form
    {
        // ===== MODEL =====
        public class ImportRecord
        {
            public DateTime Ngay { get; set; }
            public string MaPhieu { get; set; } = "";
            public string NhaCungCap { get; set; } = "";
            public int SoMatHang { get; set; }
            public int TongSL { get; set; }
            public decimal TongTien { get; set; }
        }

        // ===== UI =====
        readonly DateTimePicker dtFrom = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };
        readonly DateTimePicker dtTo = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };
        readonly ComboBox cboNCC = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        readonly NumericUpDown numMin = new() { Maximum = decimal.MaxValue, Increment = 10000, ThousandsSeparator = true };
        readonly NumericUpDown numMax = new() { Maximum = decimal.MaxValue, Increment = 10000, ThousandsSeparator = true };

        readonly Button btnFilter = new() { Text = "Lọc dữ liệu" };
        readonly Button btnReset = new() { Text = "Xóa lọc" };
        readonly Button btnExport = new() { Text = "Xuất CSV" };

        readonly Chart chartByNCC = new();
        readonly Chart chartByDate = new();
        readonly Chart chartPie = new();
        readonly DataGridView dgv = new()
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false
        };

        // ===== DATA =====
        BindingList<ImportRecord> _all = new();
        List<ImportRecord> _view = new();

        private void InitializeComponent() { }

        public FormImportReport()
        {
            InitializeComponent();
            Text = "Báo cáo nhập hàng";
            Width = 1200;
            Height = 760;
            StartPosition = FormStartPosition.CenterScreen;

            BuildLayout();
            WireEvents();

            dtFrom.Value = DateTime.Today.AddMonths(-2);
            dtTo.Value = DateTime.Today;

            LoadDataFromDatabase();
            LoadFilter();
            ApplyFilter();
        }

        // ==========================================================
        // 🔹 TẢI DỮ LIỆU TỪ SQL SERVER
        // ==========================================================
        void LoadDataFromDatabase()
        {
            try
            {
                string cs = ConfigurationManager.ConnectionStrings["Db"].ConnectionString;
                using SqlConnection con = new(cs);
                con.Open();

                string sql = @"
                    SELECT 
                        pn.NgayNhap AS Ngay,
                        pn.MaPhieuNhap AS MaPhieu,
                        ncc.TenNCC AS NhaCungCap,
                        COUNT(DISTINCT ctpn.MaThuoc) AS SoMatHang,
                        SUM(ctpn.SoLuong) AS TongSL,
                        SUM(ctpn.ThanhTien) AS TongTien
                    FROM PhieuNhap pn
                    JOIN ChiTietPhieuNhap ctpn ON pn.MaPhieuNhap = ctpn.MaPhieuNhap
                    JOIN NhaCungCap ncc ON pn.MaNCC = ncc.MaNCC
                    GROUP BY pn.NgayNhap, pn.MaPhieuNhap, ncc.TenNCC
                    ORDER BY pn.NgayNhap DESC;";

                using SqlCommand cmd = new(sql, con);
                using SqlDataReader rd = cmd.ExecuteReader();

                _all.Clear();
                while (rd.Read())
                {
                    _all.Add(new ImportRecord
                    {
                        Ngay = rd.GetDateTime(0),
                        MaPhieu = rd.GetString(1),
                        NhaCungCap = rd.GetString(2),
                        SoMatHang = rd.IsDBNull(3) ? 0 : rd.GetInt32(3),
                        TongSL = rd.IsDBNull(4) ? 0 : rd.GetInt32(4),
                        TongTien = rd.IsDBNull(5) ? 0 : rd.GetDecimal(5)
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi khi tải dữ liệu từ SQL: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================================================
        // 🔹 XÂY DỰNG GIAO DIỆN (LAYOUT)
        // ==========================================================
        void BuildLayout()
        {
            SuspendLayout();
            AutoScaleMode = AutoScaleMode.Dpi;
            DoubleBuffered = true;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(8),
                GrowStyle = TableLayoutPanelGrowStyle.FixedSize
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 320));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(root);

            // Biểu đồ trên
            var charts = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Margin = new Padding(0, 0, 0, 8)
            };
            charts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
            charts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            charts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));

            SetupChart(chartByNCC, "Tổng theo nhà cung cấp", SeriesChartType.Column);
            SetupChart(chartByDate, "Tổng theo ngày nhập", SeriesChartType.Line);
            SetupChart(chartPie, "Tỷ trọng nhà cung cấp", SeriesChartType.Pie);

            charts.Controls.Add(Wrap(chartByNCC), 0, 0);
            charts.Controls.Add(Wrap(chartByDate), 1, 0);
            charts.Controls.Add(Wrap(chartPie), 2, 0);
            root.Controls.Add(charts, 0, 0);

            // Bộ lọc
            var filters = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 6,
                RowCount = 2,
                Margin = new Padding(0, 0, 0, 8),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 320));

            filters.Controls.Add(new Label { Text = "Từ ngày", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 0);
            dtFrom.Width = 120;
            filters.Controls.Add(dtFrom, 1, 0);
            filters.Controls.Add(new Label { Text = "Đến ngày", Anchor = AnchorStyles.Left, AutoSize = true }, 2, 0);
            dtTo.Width = 120;
            filters.Controls.Add(dtTo, 3, 0);

            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                AutoSize = true
            };
            btnFilter.Size = new Size(90, 30);
            btnReset.Size = new Size(90, 30);
            btnExport.Size = new Size(90, 30);
            btnPanel.Controls.Add(btnExport);
            btnPanel.Controls.Add(btnReset);
            btnPanel.Controls.Add(btnFilter);
            filters.Controls.Add(btnPanel, 5, 0);

            filters.Controls.Add(new Label { Text = "Nhà cung cấp", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 1);
            filters.Controls.Add(cboNCC, 1, 1);
            filters.Controls.Add(new Label { Text = "Giá trị từ (₫)", Anchor = AnchorStyles.Left, AutoSize = true }, 2, 1);
            filters.Controls.Add(numMin, 3, 1);
            filters.Controls.Add(new Label { Text = "Đến (₫)", Anchor = AnchorStyles.Left, AutoSize = true }, 4, 1);
            filters.Controls.Add(numMax, 5, 1);

            root.Controls.Add(filters, 0, 1);
            root.Controls.Add(dgv, 0, 2);
            ResumeLayout();
        }

        Panel Wrap(Control c) => new Panel { Dock = DockStyle.Fill, Padding = new Padding(6), Controls = { c } };

        static void SetupChart(Chart c, string title, SeriesChartType type)
        {
            c.Dock = DockStyle.Fill;
            c.ChartAreas.Clear();
            var ca = new ChartArea("ca");
            ca.AxisX.MajorGrid.Enabled = false;
            ca.AxisY.MajorGrid.LineColor = Color.Gainsboro;
            c.ChartAreas.Add(ca);
            c.Titles.Clear();
            c.Titles.Add(title);
            c.Series.Clear();
            var s = new Series("S") { ChartType = type, XValueType = ChartValueType.String, YValueType = ChartValueType.Double };
            if (type == SeriesChartType.Line) s.BorderWidth = 3;
            if (type == SeriesChartType.Pie)
            {
                s["PieLabelStyle"] = "Outside";
                s["PieLineColor"] = "Gray";
            }
            c.Series.Add(s);
        }

        void WireEvents()
        {
            btnFilter.Click += (_, __) => ApplyFilter();
            btnReset.Click += (_, __) => { ResetFilter(); ApplyFilter(); };
            btnExport.Click += (_, __) => ExportCsv();
        }

        // ==========================================================
        // 🔹 BỘ LỌC VÀ HIỂN THỊ
        // ==========================================================
        void LoadFilter()
        {
            var list = _all.Select(x => x.NhaCungCap).Distinct().OrderBy(x => x).ToList();
            list.Insert(0, "(Tất cả)");
            cboNCC.DataSource = list;
        }

        void ResetFilter()
        {
            if (_all.Count == 0) return;
            dtFrom.Value = _all.Min(x => x.Ngay).Date;
            dtTo.Value = _all.Max(x => x.Ngay).Date;
            cboNCC.SelectedIndex = 0;
            numMin.Value = 0; numMax.Value = 0;
        }

        void ApplyFilter()
        {
            if (_all.Count == 0) return;

            DateTime from = dtFrom.Value.Date;
            DateTime to = dtTo.Value.Date.AddDays(1);
            string ncc = cboNCC.SelectedItem?.ToString() ?? "(Tất cả)";
            decimal min = numMin.Value;
            decimal max = numMax.Value;

            var q = _all.Where(x => x.Ngay >= from && x.Ngay < to);
            if (ncc != "(Tất cả)") q = q.Where(x => x.NhaCungCap == ncc);
            if (min > 0) q = q.Where(x => x.TongTien >= min);
            if (max > 0) q = q.Where(x => x.TongTien <= max);

            _view = q.OrderByDescending(x => x.Ngay).ThenByDescending(x => x.TongTien).ToList();

            dgv.DataSource = null;
            dgv.Columns.Clear();
            dgv.DataSource = _view;
            dgv.Columns[nameof(ImportRecord.Ngay)].HeaderText = "Ngày nhập";
            dgv.Columns[nameof(ImportRecord.MaPhieu)].HeaderText = "Mã phiếu nhập";
            dgv.Columns[nameof(ImportRecord.NhaCungCap)].HeaderText = "Nhà cung cấp";
            dgv.Columns[nameof(ImportRecord.SoMatHang)].HeaderText = "Số mặt hàng";
            dgv.Columns[nameof(ImportRecord.TongSL)].HeaderText = "Tổng SL";
            dgv.Columns[nameof(ImportRecord.TongTien)].HeaderText = "Tổng tiền (₫)";
            dgv.Columns[nameof(ImportRecord.Ngay)].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgv.Columns[nameof(ImportRecord.TongTien)].DefaultCellStyle.Format = "N0";

            UpdateCharts();
        }

        void UpdateCharts()
        {
            var byNcc = _view.GroupBy(x => x.NhaCungCap)
                             .Select(g => new { NCC = g.Key, Sum = g.Sum(x => x.TongTien) })
                             .OrderByDescending(x => x.Sum).ToList();
            chartByNCC.Series["S"].Points.Clear();
            foreach (var i in byNcc) chartByNCC.Series["S"].Points.AddXY(i.NCC, (double)i.Sum);
            if (chartByNCC.ChartAreas.Count > 0) chartByNCC.ChartAreas[0].AxisX.Interval = 1;

            var byDate = _view.GroupBy(x => x.Ngay.Date)
                              .Select(g => new { D = g.Key, Sum = g.Sum(x => x.TongTien) })
                              .OrderBy(x => x.D).ToList();
            chartByDate.Series["S"].Points.Clear();
            foreach (var i in byDate) chartByDate.Series["S"].Points.AddXY(i.D.ToString("dd/MM"), (double)i.Sum);

            chartPie.Series["S"].Points.Clear();
            foreach (var i in byNcc)
            {
                int idx = chartPie.Series["S"].Points.AddY((double)i.Sum);
                var pt = chartPie.Series["S"].Points[idx];
                pt.LegendText = i.NCC;
                pt.Label = string.Format("{0}\n{1:N0} ₫", i.NCC, i.Sum);
            }
        }

        void ExportCsv()
        {
            if (_view.Count == 0) { MessageBox.Show("Không có dữ liệu để xuất."); return; }
            using var sfd = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = $"bao_cao_nhap_{DateTime.Now:yyyyMMdd_HHmm}.csv" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using var sw = new StreamWriter(sfd.FileName);
                sw.WriteLine("Ngay,MaPhieu,NhaCungCap,SoMatHang,TongSL,TongTien");
                foreach (var x in _view)
                    sw.WriteLine($"{x.Ngay:yyyy-MM-dd},{x.MaPhieu},{x.NhaCungCap},{x.SoMatHang},{x.TongSL},{x.TongTien.ToString(CultureInfo.InvariantCulture)}");
                sw.Flush();
                MessageBox.Show("Xuất CSV thành công!");
            }
        }
    }
}
