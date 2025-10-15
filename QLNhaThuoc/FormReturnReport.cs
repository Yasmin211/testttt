using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        // ===== Model cho báo cáo đổi trả =====
        public class ReturnRecord
        {
            public DateTime Ngay { get; set; }
            public string MaPhieu { get; set; } = "";
            public string NhaCungCap { get; set; } = "";
            public string LoaiThuoc { get; set; } = "";     // OTC, Kháng sinh, Dạ dày...
            public string LyDo { get; set; } = "";          // Hết HSD, Lỗi bao bì, Khác...
            public int SoMatHang { get; set; }
            public int TongSL { get; set; }
            public decimal TongTien { get; set; }
        }

        // ===== Seeder dữ liệu giả =====
        static class FakeReturnData
        {
            static readonly Random rnd = new Random(66);
            static readonly string[] NCCs = { "Dược Hậu Giang", "Pharmacity", "An Khang", "Medigo", "Vimedimex" };
            static readonly string[] Loai = { "OTC", "Kháng sinh", "Giảm đau", "Dạ dày", "Vitamin" };
            static readonly string[] LyDo = { "Hết HSD", "Lỗi bao bì", "Giao sai", "Hư hỏng", "Khác" };

            public static BindingList<ReturnRecord> Generate(DateTime start, DateTime end, int count = 220)
            {
                var list = new BindingList<ReturnRecord>();
                int days = Math.Max(1, (int)(end - start).TotalDays);

                for (int i = 0; i < count; i++)
                {
                    int off = (rnd.NextDouble() < 0.6) ? rnd.Next(days / 2, days) : rnd.Next(0, days / 2);
                    DateTime d = start.AddDays(off);

                    string ncc = NCCs[rnd.Next(NCCs.Length)];
                    string loai = Loai[rnd.Next(Loai.Length)];
                    string lydo = LyDo[rnd.Next(LyDo.Length)];

                    int soMH = rnd.Next(1, 5);
                    int tongSL = 0; decimal tong = 0;
                    for (int j = 0; j < soMH; j++)
                    {
                        int sl = rnd.Next(1, 30);
                        decimal dg = loai is "OTC" or "Vitamin" ? rnd.Next(10000, 90000) : rnd.Next(30000, 220000);
                        tongSL += sl;
                        tong += sl * dg * 0.9m; // giá trị hoàn/đổi thường thấp hơn nhập
                    }

                    list.Add(new ReturnRecord
                    {
                        Ngay = d,
                        MaPhieu = $"PR{d:yyMMdd}-{rnd.Next(100, 999)}",
                        NhaCungCap = ncc,
                        LoaiThuoc = loai,
                        LyDo = lydo,
                        SoMatHang = soMH,
                        TongSL = tongSL,
                        TongTien = Math.Round(tong, 0)
                    });
                }
                return list;
            }
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
        readonly Chart chartPie = new();  // mặc định pie theo Lý do (có thể đổi sang Loại)

        readonly DataGridView dgv = new()
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false
        };

        // ===== Data =====
        BindingList<ReturnRecord> _all = new();
        List<ReturnRecord> _view = new();

        // Minimal InitializeComponent when Designer is absent
        private void InitializeComponent() { }

        public FormReturnReport()
        {
            InitializeComponent();
            Text = "Báo cáo đổi trả hàng";
            Width = 1200; Height = 760; StartPosition = FormStartPosition.CenterScreen;

            BuildLayout();
            WireEvents();

            // Seed demo
            dtFrom.Value = DateTime.Today.AddMonths(-2);
            dtTo.Value = DateTime.Today;
            _all = FakeReturnData.Generate(dtFrom.Value.AddMonths(-2), dtTo.Value.AddDays(1));

            LoadFilters();
            ApplyFilter();
        }

        // ================= LAYOUT =================
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
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 300)); // Charts
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // Filters
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // Grid
            Controls.Add(root);

            // Charts row
            var charts = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Margin = new Padding(0, 0, 0, 8)
            };
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

            // Filters row (2 hàng, 7 cột) - improved layout with labels to the right of inputs and fixed sizes
            var filters = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 7,
                RowCount = 2,
                Margin = new Padding(0, 0, 0, 8),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100)); // label
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160)); // input
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100)); // label
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220)); // input
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100)); // label
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160)); // input
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));  // buttons area
            filters.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            filters.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Row 0
            var lblFrom = new Label { Text = "Từ ngày", Anchor = AnchorStyles.Right, AutoSize = false, TextAlign = ContentAlignment.MiddleRight, Width = 100 };
            dtFrom.Width = 140; dtFrom.Margin = new Padding(6, 4, 12, 4);
            filters.Controls.Add(lblFrom, 0, 0);
            filters.Controls.Add(dtFrom, 1, 0);

            var lblTo = new Label { Text = "Đến ngày", Anchor = AnchorStyles.Right, AutoSize = false, TextAlign = ContentAlignment.MiddleRight, Width = 100 };
            dtTo.Width = 140; dtTo.Margin = new Padding(6, 4, 12, 4);
            filters.Controls.Add(lblTo, 2, 0);
            filters.Controls.Add(dtTo, 3, 0);

            var lblNcc = new Label { Text = "Nhà cung cấp", Anchor = AnchorStyles.Right, AutoSize = false, TextAlign = ContentAlignment.MiddleRight, Width = 100 };
            cboNCC.Width = 160; cboNCC.Margin = new Padding(6, 4, 12, 4);
            filters.Controls.Add(lblNcc, 4, 0);
            filters.Controls.Add(cboNCC, 5, 0);

            // Row 1
            var lblLoai = new Label { Text = "Loại thuốc", Anchor = AnchorStyles.Right, AutoSize = false, TextAlign = ContentAlignment.MiddleRight, Width = 100 };
            cboLoai.Width = 160; cboLoai.Margin = new Padding(6, 4, 12, 4);
            filters.Controls.Add(lblLoai, 0, 1);
            filters.Controls.Add(cboLoai, 1, 1);

            var lblLyDo = new Label { Text = "Lý do đổi trả", Anchor = AnchorStyles.Right, AutoSize = false, TextAlign = ContentAlignment.MiddleRight, Width = 100 };
            cboLyDo.Width = 160; cboLyDo.Margin = new Padding(6, 4, 12, 4);
            filters.Controls.Add(lblLyDo, 2, 1);
            filters.Controls.Add(cboLyDo, 3, 1);

            var lblMin = new Label { Text = "Giá trị từ (₫)", Anchor = AnchorStyles.Right, AutoSize = false, TextAlign = ContentAlignment.MiddleRight, Width = 100 };
            numMin.Width = 120; numMin.Margin = new Padding(6, 4, 12, 4);
            filters.Controls.Add(lblMin, 4, 1);
            filters.Controls.Add(numMin, 5, 1);

            // Buttons panel (right-aligned) in column 6 spanning 2 rows
            var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, WrapContents = false, AutoSize = true };
            btnFilter.Size = new Size(90, 30); btnReset.Size = new Size(90, 30); btnExport.Size = new Size(90, 30);
            btnPanel.Controls.Add(btnExport); btnPanel.Controls.Add(btnReset); btnPanel.Controls.Add(btnFilter);
            filters.SetRowSpan(btnPanel, 2);
            filters.Controls.Add(btnPanel, 6, 0);

            root.Controls.Add(filters, 0, 1);

            // Grid row
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 34;
            root.Controls.Add(dgv, 0, 2);

            ResumeLayout();
        }

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

        static void AddLabeled(TableLayoutPanel host, int col, int row, string text, Control input)
        {
            var lbl = new Label { Text = text, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 6, 6, 0) };
            input.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            input.Margin = new Padding(0, 0, 12, 6);
            host.SetColumnSpan(lbl, 2);
            host.SetColumnSpan(input, 2);
            host.Controls.Add(lbl, col, row);
            host.Controls.Add(input, col + 2, row);
        }
        // ================= END LAYOUT =================

        void WireEvents()
        {
            btnFilter.Click += (_, __) => ApplyFilter();
            btnReset.Click += (_, __) => { ResetFilter(); ApplyFilter(); };
            btnExport.Click += (_, __) => ExportCsv();
        }

        void LoadFilters()
        {
            // NCC
            var ncc = _all.Select(x => x.NhaCungCap).Distinct().OrderBy(x => x).ToList();
            ncc.Insert(0, "(Tất cả)");
            cboNCC.DataSource = ncc;

            // Loại
            var loai = _all.Select(x => x.LoaiThuoc).Distinct().OrderBy(x => x).ToList();
            loai.Insert(0, "(Tất cả)");
            cboLoai.DataSource = loai;

            // Lý do
            var lydo = _all.Select(x => x.LyDo).Distinct().OrderBy(x => x).ToList();
            lydo.Insert(0, "(Tất cả)");
            cboLyDo.DataSource = lydo;
        }

        void ResetFilter()
        {
            dtFrom.Value = _all.Min(x => x.Ngay).Date;
            dtTo.Value = _all.Max(x => x.Ngay).Date;
            cboNCC.SelectedIndex = 0;
            cboLoai.SelectedIndex = 0;
            cboLyDo.SelectedIndex = 0;
            numMin.Value = 0; numMax.Value = 0;
        }

        void ApplyFilter()
        {
            DateTime from = dtFrom.Value.Date;
            DateTime to = dtTo.Value.Date.AddDays(1);
            string ncc = cboNCC.SelectedItem?.ToString() ?? "(Tất cả)";
            string loai = cboLoai.SelectedItem?.ToString() ?? "(Tất cả)";
            string lydo = cboLyDo.SelectedItem?.ToString() ?? "(Tất cả)";
            decimal min = numMin.Value;
            decimal max = numMax.Value;

            var q = _all.Where(x => x.Ngay >= from && x.Ngay < to);
            if (ncc != "(Tất cả)") q = q.Where(x => x.NhaCungCap == ncc);
            if (loai != "(Tất cả)") q = q.Where(x => x.LoaiThuoc == loai);
            if (lydo != "(Tất cả)") q = q.Where(x => x.LyDo == lydo);
            if (min > 0) q = q.Where(x => x.TongTien >= min);
            if (max > 0) q = q.Where(x => x.TongTien <= max);

            _view = q.OrderByDescending(x => x.Ngay).ThenByDescending(x => x.TongTien).ToList();

            // Bảng chi tiết
            dgv.DataSource = null;
            dgv.Columns.Clear();
            dgv.DataSource = _view;

            dgv.Columns[nameof(ReturnRecord.Ngay)].HeaderText = "Ngày";
            dgv.Columns[nameof(ReturnRecord.MaPhieu)].HeaderText = "Mã phiếu";
            dgv.Columns[nameof(ReturnRecord.NhaCungCap)].HeaderText = "Nhà cung cấp";
            dgv.Columns[nameof(ReturnRecord.LoaiThuoc)].HeaderText = "Loại thuốc";
            dgv.Columns[nameof(ReturnRecord.LyDo)].HeaderText = "Lý do";
            dgv.Columns[nameof(ReturnRecord.SoMatHang)].HeaderText = "Số mặt hàng";
            dgv.Columns[nameof(ReturnRecord.TongSL)].HeaderText = "Số lượng";
            dgv.Columns[nameof(ReturnRecord.TongTien)].HeaderText = "Tổng tiền (₫)";

            dgv.Columns[nameof(ReturnRecord.Ngay)].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgv.Columns[nameof(ReturnRecord.TongTien)].DefaultCellStyle.Format = "N0";

            // Biểu đồ
            UpdateCharts();
        }

        void UpdateCharts()
        {
            // 1) Theo NCC
            var byNcc = _view.GroupBy(x => x.NhaCungCap)
                             .Select(g => new { NCC = g.Key, Sum = g.Sum(x => x.TongTien) })
                             .OrderByDescending(x => x.Sum).ToList();
            chartByNCC.Series["S"].Points.Clear();
            foreach (var i in byNcc) chartByNCC.Series["S"].Points.AddXY(i.NCC, (double)i.Sum);
            chartByNCC.ChartAreas[0].AxisX.Interval = 1;

            // 2) Theo ngày
            var byDate = _view.GroupBy(x => x.Ngay.Date)
                              .Select(g => new { D = g.Key, Sum = g.Sum(x => x.TongTien) })
                              .OrderBy(x => x.D).ToList();
            chartByDate.Series["S"].Points.Clear();
            foreach (var i in byDate) chartByDate.Series["S"].Points.AddXY(i.D.ToString("dd/MM"), (double)i.Sum);

            // 3) Pie theo Lý do (có thể đổi sang LoạiThuoc nếu thích)
            var byReason = _view.GroupBy(x => x.LyDo)
                                .Select(g => new { LyDo = g.Key, Sum = g.Sum(x => x.TongTien) })
                                .OrderByDescending(x => x.Sum).ToList();
            chartPie.Series["S"].Points.Clear();
            foreach (var i in byReason)
            {
                int idx = chartPie.Series["S"].Points.AddY((double)i.Sum);
                var p = chartPie.Series["S"].Points[idx];
                p.LegendText = i.LyDo;
                p.Label = string.Format("{0}\n{1:N0} ₫", i.LyDo, i.Sum);
            }
        }

        void ExportCsv()
        {
            if (_view.Count == 0) { MessageBox.Show("Không có dữ liệu để xuất."); return; }
            using var sfd = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = $"bao_cao_doi_tra_{DateTime.Now:yyyyMMdd_HHmm}.csv" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using var sw = new StreamWriter(sfd.FileName);
                sw.WriteLine("Ngay,MaPhieu,NhaCungCap,LoaiThuoc,LyDo,SoMatHang,TongSL,TongTien");
                foreach (var x in _view)
                    sw.WriteLine($"{x.Ngay:yyyy-MM-dd},{x.MaPhieu},{x.NhaCungCap},{x.LoaiThuoc},{x.LyDo},{x.SoMatHang},{x.TongSL},{x.TongTien.ToString(CultureInfo.InvariantCulture)}");
                sw.Flush();
                MessageBox.Show("Xuất CSV thành công!");
            }
        }
    }
}
