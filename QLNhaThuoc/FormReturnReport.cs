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
    public partial class FormReturnReport : Form
    {
        // ===== Model =====
        public class ReturnRecord
        {
            public DateTime Ngay { get; set; }
            public string MaPhieu { get; set; } = "";
            public string NhaCungCap { get; set; } = "";
            public string LoaiThuoc { get; set; } = "";
            public string LyDo { get; set; } = "";
            public int SoMatHang { get; set; }
            public int TongSL { get; set; }
            public decimal TongTien { get; set; }
        }

        // ===== UI controls =====
        readonly DateTimePicker dtFrom = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };
        readonly DateTimePicker dtTo = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };
        readonly ComboBox cboNCC = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        readonly ComboBox cboLoai = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        readonly ComboBox cboLyDo = new() { DropDownStyle = ComboBoxStyle.DropDownList };
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

        BindingList<ReturnRecord> _all = new();
        List<ReturnRecord> _view = new();

        private void InitializeComponent() { }

        public FormReturnReport()
        {
            InitializeComponent();
            Text = "Báo cáo đổi trả hàng (SQL)";
            Width = 1200; Height = 760;
            StartPosition = FormStartPosition.CenterScreen;

            BuildLayout();
            WireEvents();

            dtFrom.Value = DateTime.Today.AddMonths(-3);
            dtTo.Value = DateTime.Today;

            LoadFromDatabase();
            LoadFilters();
            ApplyFilter();
        }

        // ================= LOAD SQL DATA =================
        void LoadFromDatabase()
        {
            try
            {
                string cs = ConfigurationManager.ConnectionStrings["Db"].ConnectionString;
                using SqlConnection con = new(cs);
                con.Open();

                string sql = @"
                SELECT 
                    p.MaPhieuTra,
                    ncc.TenNCC,
                    dm.TenDanhMuc,
                    p.LyDoTra,
                    p.NgayTra,
                    COUNT(DISTINCT ct.MaThuoc) AS SoMatHang,
                    SUM(ct.SoLuong) AS TongSL,
                    SUM(ct.ThanhTien) AS TongTien
                FROM PhieuTraHang p
                JOIN NhaCungCap ncc ON p.MaNCC = ncc.MaNCC
                JOIN ChiTietPhieuTraHang ct ON p.MaPhieuTra = ct.MaPhieuTra
                JOIN Thuoc t ON ct.MaThuoc = t.MaThuoc
                JOIN DanhMucThuoc dm ON t.MaDanhMuc = dm.MaDanhMuc
                GROUP BY p.MaPhieuTra, ncc.TenNCC, dm.TenDanhMuc, p.LyDoTra, p.NgayTra
                ORDER BY p.NgayTra DESC;";

                using SqlCommand cmd = new(sql, con);
                using SqlDataReader rd = cmd.ExecuteReader();

                _all.Clear();
                while (rd.Read())
                {
                    _all.Add(new ReturnRecord
                    {
                        MaPhieu = rd["MaPhieuTra"].ToString() ?? "",
                        NhaCungCap = rd["TenNCC"].ToString() ?? "",
                        LoaiThuoc = rd["TenDanhMuc"].ToString() ?? "",
                        LyDo = rd["LyDoTra"].ToString() ?? "",
                        Ngay = rd.GetDateTime(rd.GetOrdinal("NgayTra")),
                        SoMatHang = Convert.ToInt32(rd["SoMatHang"]),
                        TongSL = Convert.ToInt32(rd["TongSL"]),
                        TongTien = Convert.ToDecimal(rd["TongTien"])
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi tải dữ liệu đổi trả: " + ex.Message,
                    "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ================= LAYOUT =================
        void BuildLayout()
        {
            SuspendLayout();

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(8) };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 300));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(root);

            // Charts row
            var charts = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
            charts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            charts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
            charts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));

            SetupChart(chartByNCC, "Giá trị đổi trả theo NCC", SeriesChartType.Column);
            SetupChart(chartByDate, "Giá trị đổi trả theo ngày", SeriesChartType.Line);
            SetupChart(chartPie, "Tỷ trọng theo lý do", SeriesChartType.Pie);

            charts.Controls.Add(chartByNCC, 0, 0);
            charts.Controls.Add(chartByDate, 1, 0);
            charts.Controls.Add(chartPie, 2, 0);
            root.Controls.Add(charts, 0, 0);

            // Filters
            var filters = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 7, RowCount = 2, AutoSize = true };
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            AddFilter(filters, "Từ ngày", dtFrom, 0, 0);
            AddFilter(filters, "Đến ngày", dtTo, 2, 0);
            AddFilter(filters, "Nhà cung cấp", cboNCC, 4, 0);
            AddFilter(filters, "Loại thuốc", cboLoai, 0, 1);
            AddFilter(filters, "Lý do", cboLyDo, 2, 1);
            AddFilter(filters, "Giá trị từ (₫)", numMin, 4, 1);

            var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            btnPanel.Controls.AddRange(new Control[] { btnExport, btnReset, btnFilter });
            filters.SetRowSpan(btnPanel, 2);
            filters.Controls.Add(btnPanel, 6, 0);

            root.Controls.Add(filters, 0, 1);

            // Grid
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            root.Controls.Add(dgv, 0, 2);

            ResumeLayout();
        }

        static void AddFilter(TableLayoutPanel host, string label, Control control, int col, int row)
        {
            var lbl = new Label { Text = label, Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight, Width = 100 };
            control.Width = 150;
            host.Controls.Add(lbl, col, row);
            host.Controls.Add(control, col + 1, row);
        }

        static void SetupChart(Chart c, string title, SeriesChartType type)
        {
            c.Dock = DockStyle.Fill;
            c.ChartAreas.Clear();
            var ca = new ChartArea("ca");
            ca.AxisX.MajorGrid.Enabled = false;
            ca.AxisY.MajorGrid.LineColor = Color.Gainsboro;
            c.ChartAreas.Add(ca);
            c.Titles.Add(title);
            var s = new Series("S") { ChartType = type };
            if (type == SeriesChartType.Line) s.BorderWidth = 3;
            if (type == SeriesChartType.Pie)
            {
                s["PieLabelStyle"] = "Outside";
                s["PieLineColor"] = "Gray";
            }
            c.Series.Add(s);
        }

        // ================= FILTERS =================
        void WireEvents()
        {
            btnFilter.Click += (_, __) => ApplyFilter();
            btnReset.Click += (_, __) => { ResetFilter(); ApplyFilter(); };
            btnExport.Click += (_, __) => ExportCsv();
        }

        void LoadFilters()
        {
            var ncc = _all.Select(x => x.NhaCungCap).Distinct().OrderBy(x => x).ToList();
            ncc.Insert(0, "(Tất cả)");
            cboNCC.DataSource = ncc;

            var loai = _all.Select(x => x.LoaiThuoc).Distinct().OrderBy(x => x).ToList();
            loai.Insert(0, "(Tất cả)");
            cboLoai.DataSource = loai;

            var lydo = _all.Select(x => x.LyDo).Distinct().OrderBy(x => x).ToList();
            lydo.Insert(0, "(Tất cả)");
            cboLyDo.DataSource = lydo;
        }

        void ResetFilter()
        {
            dtFrom.Value = _all.Min(x => x.Ngay);
            dtTo.Value = _all.Max(x => x.Ngay);
            cboNCC.SelectedIndex = 0;
            cboLoai.SelectedIndex = 0;
            cboLyDo.SelectedIndex = 0;
            numMin.Value = 0;
            numMax.Value = 0;
        }

        // ================= APPLY FILTER =================
        void ApplyFilter()
        {
            DateTime from = dtFrom.Value.Date;
            DateTime to = dtTo.Value.Date.AddDays(1);
            string ncc = cboNCC.Text;
            string loai = cboLoai.Text;
            string lydo = cboLyDo.Text;
            decimal min = numMin.Value;
            decimal max = numMax.Value;

            var q = _all.Where(x => x.Ngay >= from && x.Ngay < to);
            if (ncc != "(Tất cả)") q = q.Where(x => x.NhaCungCap == ncc);
            if (loai != "(Tất cả)") q = q.Where(x => x.LoaiThuoc == loai);
            if (lydo != "(Tất cả)") q = q.Where(x => x.LyDo == lydo);
            if (min > 0) q = q.Where(x => x.TongTien >= min);
            if (max > 0) q = q.Where(x => x.TongTien <= max);

            _view = q.OrderByDescending(x => x.Ngay).ToList();

            dgv.DataSource = null;
            dgv.Columns.Clear();
            dgv.DataSource = _view;

            dgv.Columns[nameof(ReturnRecord.Ngay)].HeaderText = "Ngày trả";
            dgv.Columns[nameof(ReturnRecord.MaPhieu)].HeaderText = "Mã phiếu";
            dgv.Columns[nameof(ReturnRecord.NhaCungCap)].HeaderText = "Nhà cung cấp";
            dgv.Columns[nameof(ReturnRecord.LoaiThuoc)].HeaderText = "Loại thuốc";
            dgv.Columns[nameof(ReturnRecord.LyDo)].HeaderText = "Lý do";
            dgv.Columns[nameof(ReturnRecord.SoMatHang)].HeaderText = "Số mặt hàng";
            dgv.Columns[nameof(ReturnRecord.TongSL)].HeaderText = "Số lượng";
            dgv.Columns[nameof(ReturnRecord.TongTien)].HeaderText = "Tổng tiền (₫)";
            dgv.Columns[nameof(ReturnRecord.Ngay)].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgv.Columns[nameof(ReturnRecord.TongTien)].DefaultCellStyle.Format = "N0";

            UpdateCharts();
        }

        // ================= CHARTS =================
        void UpdateCharts()
        {
            var byNcc = _view.GroupBy(x => x.NhaCungCap)
                             .Select(g => new { NCC = g.Key, Sum = g.Sum(x => x.TongTien) })
                             .OrderByDescending(x => x.Sum).ToList();
            chartByNCC.Series["S"].Points.Clear();
            foreach (var i in byNcc) chartByNCC.Series["S"].Points.AddXY(i.NCC, (double)i.Sum);

            var byDate = _view.GroupBy(x => x.Ngay.Date)
                              .Select(g => new { D = g.Key, Sum = g.Sum(x => x.TongTien) })
                              .OrderBy(x => x.D).ToList();
            chartByDate.Series["S"].Points.Clear();
            foreach (var i in byDate) chartByDate.Series["S"].Points.AddXY(i.D.ToString("dd/MM"), (double)i.Sum);

            var byReason = _view.GroupBy(x => x.LyDo)
                                .Select(g => new { LyDo = g.Key, Sum = g.Sum(x => x.TongTien) })
                                .OrderByDescending(x => x.Sum).ToList();
            chartPie.Series["S"].Points.Clear();
            foreach (var i in byReason)
            {
                int idx = chartPie.Series["S"].Points.AddY((double)i.Sum);
                chartPie.Series["S"].Points[idx].LegendText = i.LyDo;
                chartPie.Series["S"].Points[idx].Label = $"{i.LyDo}\n{i.Sum:N0} ₫";
            }
        }

        // ================= EXPORT =================
        void ExportCsv()
        {
            if (_view.Count == 0) { MessageBox.Show("Không có dữ liệu để xuất."); return; }
            using var sfd = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = $"bao_cao_doi_tra_{DateTime.Now:yyyyMMdd_HHmm}.csv" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using var sw = new StreamWriter(sfd.FileName);
                sw.WriteLine("Ngay,MaPhieu,NhaCungCap,LoaiThuoc,LyDo,SoMatHang,TongSL,TongTien");
                foreach (var x in _view)
                    sw.WriteLine($"{x.Ngay:yyyy-MM-dd},{x.MaPhieu},{x.NhaCungCap},{x.LoaiThuoc},{x.LyDo},{x.SoMatHang},{x.TongSL},{x.TongTien}");
                MessageBox.Show("✅ Xuất CSV thành công!");
            }
        }
    }
}
