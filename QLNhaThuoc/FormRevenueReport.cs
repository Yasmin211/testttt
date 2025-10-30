using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace QLNhaThuoc
{
    public partial class FormRevenueReport : Form
    {
        // ================== Models ==================
        public class SaleItem
        {
            public string Drug { get; set; } = "";
            public int Qty { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Amount => Qty * UnitPrice;
        }

        public class SaleOrder
        {
            public DateTime Ngay { get; set; }
            public string SoHD { get; set; } = "";
            public string Customer { get; set; } = "";
            public List<SaleItem> Items { get; set; } = new();
            public int TongSL => Items.Sum(i => i.Qty);
            public int SoMatHang => Items.Count;
            public decimal DoanhThu => Items.Sum(i => i.Amount);
        }

        // ================== UI controls ==================
        readonly DateTimePicker dtFrom = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };
        readonly DateTimePicker dtTo = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };
        readonly ComboBox cboGran = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        readonly NumericUpDown numTop = new() { Minimum = 3, Maximum = 20, Value = 5, Width = 60 };
        readonly Button btnFilter = new() { Text = "Lọc dữ liệu" };
        readonly Button btnReset = new() { Text = "Xóa lọc" };
        readonly Button btnExport = new() { Text = "Xuất CSV" };
        readonly Button btnPrint = new() { Text = "In báo cáo" };

        readonly Label lblTotalValue = new();

        readonly Chart chRevenueLine = new();
        readonly Chart chTopDrugs = new();
        readonly Chart chTopCustomers = new();
        readonly DataGridView dgv = new()
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false
        };

        BindingList<SaleOrder> _all = new();
        List<SaleOrder> _view = new();
        PrintDocument printDoc = new();

        private void InitializeComponent() { }

        public FormRevenueReport()
        {
            InitializeComponent();
            Text = "Báo cáo doanh thu (SQL)";
            Width = 1280; Height = 800; StartPosition = FormStartPosition.CenterScreen;

            BuildLayout();
            WireEvents();

            dtFrom.Value = DateTime.Today.AddMonths(-3);
            dtTo.Value = DateTime.Today;
            cboGran.Items.AddRange(new object[] { "Ngày", "Tháng", "Năm" });
            cboGran.SelectedIndex = 0;

            LoadFromDatabase();
            ApplyFilter();
        }

        // ================== LOAD SQL DATA ==================
        void LoadFromDatabase()
        {
            try
            {
                string cs = ConfigurationManager.ConnectionStrings["Db"].ConnectionString;
                using SqlConnection con = new(cs);
                con.Open();

                string sql = @"
                SELECT 
                    hd.MaHoaDon, hd.NgayLap, kh.TenKH,
                    ct.MaThuoc, t.TenThuoc, ct.SoLuong, ct.DonGia, ct.ThanhTien
                FROM HoaDon hd
                JOIN KhachHang kh ON hd.MaKH = kh.MaKH
                JOIN ChiTietHoaDon ct ON hd.MaHoaDon = ct.MaHoaDon
                JOIN Thuoc t ON ct.MaThuoc = t.MaThuoc
                WHERE hd.NgayLap IS NOT NULL
                ORDER BY hd.NgayLap DESC;";

                using SqlCommand cmd = new(sql, con);
                using SqlDataReader rd = cmd.ExecuteReader();

                _all.Clear();
                var dict = new Dictionary<string, SaleOrder>();

                while (rd.Read())
                {
                    string maHD = rd["MaHoaDon"].ToString() ?? "";
                    if (!dict.ContainsKey(maHD))
                    {
                        dict[maHD] = new SaleOrder
                        {
                            SoHD = maHD,
                            Ngay = rd.GetDateTime(rd.GetOrdinal("NgayLap")),
                            Customer = rd["TenKH"].ToString() ?? "",
                            Items = new List<SaleItem>()
                        };
                    }

                    dict[maHD].Items.Add(new SaleItem
                    {
                        Drug = rd["TenThuoc"].ToString() ?? "",
                        Qty = Convert.ToInt32(rd["SoLuong"]),
                        UnitPrice = Convert.ToDecimal(rd["DonGia"])
                    });
                }

                foreach (var o in dict.Values)
                    _all.Add(o);
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi tải dữ liệu doanh thu: " + ex.Message,
                    "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ================== UI BUILD ==================
        void BuildLayout()
        {
            SuspendLayout();
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, Padding = new Padding(8) };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 300));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(root);

            // KPI
            var kpiPanel = new Panel { Dock = DockStyle.Top, Height = 70, Padding = new Padding(8) };
            var kpiLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
            kpiLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            kpiLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            var lblTitle = new Label { Text = "Tổng doanh thu", Font = new Font("Segoe UI", 10, FontStyle.Bold), Dock = DockStyle.Left };
            lblTotalValue.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblTotalValue.Dock = DockStyle.Right;
            lblTotalValue.Text = "—";

            kpiLayout.Controls.Add(lblTitle, 0, 0);
            kpiLayout.Controls.Add(lblTotalValue, 1, 0);
            kpiPanel.Controls.Add(kpiLayout);
            root.Controls.Add(kpiPanel, 0, 0);

            // Charts
            var charts = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
            charts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            charts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            charts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

            SetupChart(chRevenueLine, "Doanh thu theo thời gian", SeriesChartType.Line);
            SetupChart(chTopDrugs, "Top thuốc bán chạy", SeriesChartType.Column);
            SetupChart(chTopCustomers, "Top khách hàng", SeriesChartType.Column);

            charts.Controls.Add(chRevenueLine, 0, 0);
            charts.Controls.Add(chTopDrugs, 1, 0);
            charts.Controls.Add(chTopCustomers, 2, 0);
            root.Controls.Add(charts, 0, 1);

            // Filters
            var filters = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 7, RowCount = 2, AutoSize = true };
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            AddFilter(filters, "Từ ngày", dtFrom, 0, 0);
            AddFilter(filters, "Đến ngày", dtTo, 2, 0);
            AddFilter(filters, "Theo", cboGran, 4, 0);
            AddFilter(filters, "Top (n)", numTop, 0, 1);

            var btns = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            btns.Controls.AddRange(new Control[] { btnPrint, btnExport, btnReset, btnFilter });
            filters.SetRowSpan(btns, 2);
            filters.Controls.Add(btns, 6, 0);

            root.Controls.Add(filters, 0, 2);

            // Grid
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            root.Controls.Add(dgv, 0, 3);
            ResumeLayout();
        }

        static void AddFilter(TableLayoutPanel host, string label, Control ctrl, int col, int row)
        {
            var lbl = new Label { Text = label, Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight, Width = 100 };
            ctrl.Width = 150;
            host.Controls.Add(lbl, col, row);
            host.Controls.Add(ctrl, col + 1, row);
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
            c.Series.Add(s);
        }

        // ================== FILTERING ==================
        void WireEvents()
        {
            btnFilter.Click += (_, __) => ApplyFilter();
            btnReset.Click += (_, __) => { ResetFilter(); ApplyFilter(); };
            btnExport.Click += (_, __) => ExportCsv();
            btnPrint.Click += (_, __) => PrintPreview();
        }

        void ResetFilter()
        {
            dtFrom.Value = _all.Min(x => x.Ngay).Date;
            dtTo.Value = _all.Max(x => x.Ngay).Date;
            cboGran.SelectedIndex = 0;
            numTop.Value = 5;
        }

        void ApplyFilter()
        {
            DateTime from = dtFrom.Value.Date;
            DateTime to = dtTo.Value.Date.AddDays(1);
            _view = _all.Where(x => x.Ngay >= from && x.Ngay < to).ToList();

            var flat = _view.Select(x => new
            {
                x.Ngay,
                x.SoHD,
                KhachHang = x.Customer,
                SoMatHang = x.SoMatHang,
                TongSL = x.TongSL,
                DoanhThu = x.DoanhThu
            }).OrderByDescending(x => x.Ngay).ToList();

            dgv.DataSource = flat;
            dgv.Columns["Ngay"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgv.Columns["DoanhThu"].DefaultCellStyle.Format = "N0";

            lblTotalValue.Text = $"{_view.Sum(x => x.DoanhThu):N0} ₫";
            UpdateCharts();
        }

        void UpdateCharts()
        {
            string gran = cboGran.SelectedItem?.ToString() ?? "Ngày";
            IEnumerable<(string Key, decimal Sum)> series;
            if (gran == "Năm")
                series = _view.GroupBy(o => o.Ngay.ToString("yyyy")).Select(g => (g.Key, g.Sum(x => x.DoanhThu))).OrderBy(x => x.Key);
            else if (gran == "Tháng")
                series = _view.GroupBy(o => o.Ngay.ToString("yyyy-MM")).Select(g => (g.Key, g.Sum(x => x.DoanhThu))).OrderBy(x => x.Key);
            else
                series = _view.GroupBy(o => o.Ngay.ToString("dd/MM")).Select(g => (g.Key, g.Sum(x => x.DoanhThu)));

            chRevenueLine.Series["S"].Points.Clear();
            foreach (var p in series) chRevenueLine.Series["S"].Points.AddXY(p.Key, (double)p.Sum);

            int topN = (int)numTop.Value;
            var topDrugs = _view.SelectMany(o => o.Items)
                .GroupBy(i => i.Drug)
                .Select(g => new { Drug = g.Key, Sum = g.Sum(x => x.Amount) })
                .OrderByDescending(x => x.Sum).Take(topN).ToList();

            chTopDrugs.Series["S"].Points.Clear();
            foreach (var d in topDrugs) chTopDrugs.Series["S"].Points.AddXY(d.Drug, (double)d.Sum);

            var topCus = _view.GroupBy(o => o.Customer)
                .Select(g => new { Cus = g.Key, Sum = g.Sum(x => x.DoanhThu) })
                .OrderByDescending(x => x.Sum).Take(topN).ToList();

            chTopCustomers.Series["S"].Points.Clear();
            foreach (var c in topCus) chTopCustomers.Series["S"].Points.AddXY(c.Cus, (double)c.Sum);
        }

        // ================== EXPORT ==================
        void ExportCsv()
        {
            if (_view.Count == 0) { MessageBox.Show("Không có dữ liệu."); return; }
            using var sfd = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = $"bao_cao_doanh_thu_{DateTime.Now:yyyyMMdd_HHmm}.csv" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using var sw = new StreamWriter(sfd.FileName);
                sw.WriteLine("Ngay,SoHD,KhachHang,SoMatHang,TongSL,DoanhThu");
                foreach (var o in _view)
                    sw.WriteLine($"{o.Ngay:yyyy-MM-dd},{o.SoHD},{o.Customer},{o.SoMatHang},{o.TongSL},{o.DoanhThu}");
                MessageBox.Show("✅ Xuất CSV thành công!");
            }
        }

        // ================== PRINT ==================
        void PrintPreview()
        {
            if (!_view.Any()) { MessageBox.Show("Không có dữ liệu để in."); return; }
            printDoc = new PrintDocument();
            printDoc.DocumentName = "Báo cáo doanh thu";
            printDoc.PrintPage += PrintDoc_PrintPage;
            using var prev = new PrintPreviewDialog { Document = printDoc, Width = 1000, Height = 700 };
            prev.ShowDialog();
        }

        int _printIndex = 0;
        void PrintDoc_PrintPage(object? sender, PrintPageEventArgs e)
        {
            int left = e.MarginBounds.Left, y = e.MarginBounds.Top;
            var title = new Font("Segoe UI", 12, FontStyle.Bold);
            var normal = new Font("Segoe UI", 9);

            e.Graphics.DrawString("BÁO CÁO DOANH THU", title, Brushes.Black, left, y);
            y += 24;
            e.Graphics.DrawString($"Từ {dtFrom.Value:dd/MM/yyyy} đến {dtTo.Value:dd/MM/yyyy}", normal, Brushes.Black, left, y);
            y += 20;
            e.Graphics.DrawString($"Tổng doanh thu: {_view.Sum(x => x.DoanhThu):N0} ₫", normal, Brushes.Black, left, y);
            y += 24;
            e.Graphics.DrawString("Ngày | Hóa đơn | Khách hàng | Doanh thu", normal, Brushes.Black, left, y);
            y += 16;

            var flat = _view.OrderByDescending(x => x.Ngay).ToList();
            while (_printIndex < flat.Count)
            {
                var r = flat[_printIndex];
                string line = $"{r.Ngay:dd/MM/yyyy} | {r.SoHD} | {r.Customer} | {r.DoanhThu:N0} ₫";
                e.Graphics.DrawString(line, normal, Brushes.Black, left, y);
                y += 18;

                if (y > e.MarginBounds.Bottom - 20)
                {
                    e.HasMorePages = true; return;
                }
                _printIndex++;
            }
            _printIndex = 0;
            e.HasMorePages = false;
        }
    }
}
