using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        // ================== Fake data (seeder) ==================
        static class SaleSeeder
        {
            static readonly Random rnd = new Random(106);
            static readonly string[] Drugs = {
                "Paracetamol 500mg","Ibuprofen 400mg","Vitamin C 1000mg","Omeprazol 20mg",
                "Amoxicillin 500mg","Cefalexin 500mg","Clorpheniramine 4mg","Azithromycin 250mg",
                "Panadol Extra","Efferalgan 500mg","Salonpas Patch","ORS Gói","Tums Antacid",
                "Cetirizine 10mg","Meloxicam 7.5mg","Magie B6","Betadine 10%"
            };
            static readonly string[] Customers = {
                "Nguyễn Văn A","Trần Thị B","Phạm Minh C","Cửa hàng Minh Phúc",
                "Hiệu thuốc An Khang - CN01","Nguyễn Thị D","Pharmacity CN07","Shop Medigo Q3",
                "Lê Quốc E","Đỗ Thảo F"
            };

            public static BindingList<SaleOrder> Generate(DateTime start, DateTime end, int count = 600)
            {
                var list = new BindingList<SaleOrder>();
                int days = Math.Max(1, (int)(end - start).TotalDays);

                for (int i = 0; i < count; i++)
                {
                    int dayOffset = (rnd.NextDouble() < 0.6) ? rnd.Next(days / 2, days) : rnd.Next(0, days / 2);
                    var d = start.AddDays(dayOffset);

                    var soHD = $"HD{d:yyMMdd}-{rnd.Next(1000, 9999)}";
                    var cus = Customers[rnd.Next(Customers.Length)];

                    int itemCount = rnd.Next(1, 6);
                    var items = new List<SaleItem>();
                    for (int j = 0; j < itemCount; j++)
                    {
                        string drug = Drugs[rnd.Next(Drugs.Length)];
                        int qty = rnd.Next(1, 12);
                        decimal price = drug.Contains("Vitamin") || drug.Contains("ORS") || drug.Contains("Salonpas")
                                        ? rnd.Next(10000, 90000)
                                        : rnd.Next(25000, 250000);
                        items.Add(new SaleItem { Drug = drug, Qty = qty, UnitPrice = price });
                    }

                    list.Add(new SaleOrder
                    {
                        Ngay = d,
                        SoHD = soHD,
                        Customer = cus,
                        Items = items
                    });
                }
                return list;
            }
        }

        // ================== UI controls ==================
        readonly DateTimePicker dtFrom = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };
        readonly DateTimePicker dtTo = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };
        readonly ComboBox cboGran = new() { DropDownStyle = ComboBoxStyle.DropDownList }; // Ngày / Tháng / Năm
        readonly NumericUpDown numTop = new() { Minimum = 3, Maximum = 20, Value = 5, Width = 60 };
        readonly Button btnFilter = new() { Text = "Lọc dữ liệu" };
        readonly Button btnReset = new() { Text = "Xóa lọc" };
        readonly Button btnExport = new() { Text = "Xuất CSV" };
        readonly Button btnPrint = new() { Text = "In báo cáo" };

        readonly Label lblTotalTitle = new();
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

        // ================== Data ==================
        BindingList<SaleOrder> _all = new();
        List<SaleOrder> _view = new();

        PrintDocument printDoc = new();

        // Minimal InitializeComponent in case designer is not present
        private void InitializeComponent() { }

        public FormRevenueReport()
        {
            InitializeComponent();
            Text = "Báo cáo doanh thu";
            Width = 1280; Height = 800; StartPosition = FormStartPosition.CenterScreen;

            BuildLayout();
            WireEvents();

            // seed demo
            dtFrom.Value = DateTime.Today.AddMonths(-3);
            dtTo.Value = DateTime.Today;
            cboGran.Items.AddRange(new object[] { "Ngày", "Tháng", "Năm" });
            cboGran.SelectedIndex = 0;

            _all = SaleSeeder.Generate(dtFrom.Value.AddMonths(-3), dtTo.Value.AddDays(1), 700);

            ApplyFilter();
        }

        // ================== Layout ==================
        void BuildLayout()
        {
            SuspendLayout();
            AutoScaleMode = AutoScaleMode.Dpi;
            DoubleBuffered = true;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(8),
                GrowStyle = TableLayoutPanelGrowStyle.FixedSize
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));          // KPI (fixed height)
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 300));     // Charts top row
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));          // Filters
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));      // Grid
            Controls.Add(root);

            // KPI total revenue - show title and large value separately
            var kpi = new Panel { Dock = DockStyle.Top, Height = 72, Padding = new Padding(8, 8, 8, 8) };
            var kpiInner = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
            kpiInner.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            kpiInner.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            lblTotalTitle.Text = "Tổng doanh thu";
            lblTotalTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTotalTitle.AutoSize = true;
            lblTotalTitle.Dock = DockStyle.Left;

            lblTotalValue.Text = "—";
            lblTotalValue.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTotalValue.AutoSize = true;
            lblTotalValue.Dock = DockStyle.Right;

            // Place title on left and value on right
            kpiInner.Controls.Add(lblTotalTitle, 0, 0);
            kpiInner.Controls.Add(lblTotalValue, 1, 0);
            kpi.Controls.Add(kpiInner);
            root.Controls.Add(kpi, 0, 0);

            // Charts row (3 charts)
            var charts = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Margin = new Padding(0, 0, 0, 8)
            };
            charts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            charts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            charts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            SetupChart(chRevenueLine, "Doanh thu theo thời gian", SeriesChartType.Line);
            SetupChart(chTopDrugs, "Top thuốc bán chạy", SeriesChartType.Column);
            SetupChart(chTopCustomers, "Top khách hàng doanh thu", SeriesChartType.Column);
            charts.Controls.Add(chRevenueLine, 0, 0);
            charts.Controls.Add(chTopDrugs, 1, 0);
            charts.Controls.Add(chTopCustomers, 2, 0);
            root.Controls.Add(charts, 0, 1);

            // Filters row (fixed columns)
            var filters = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 7,
                RowCount = 2,
                Margin = new Padding(0, 0, 0, 8),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100)); // label / input pair 1
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160)); // input
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100)); // label / input pair 2
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160)); // input
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100)); // label / input pair 3
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140)); // input
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));  // buttons area
            filters.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            filters.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Row 0: From | To | Theo | Buttons
            var lblFrom = new Label { Text = "Từ ngày", Anchor = AnchorStyles.Right, AutoSize = false, TextAlign = ContentAlignment.MiddleRight, Width = 100 };
            dtFrom.Width = 120; dtFrom.Margin = new Padding(6, 4, 12, 4);
            filters.Controls.Add(lblFrom, 0, 0);
            filters.Controls.Add(dtFrom, 1, 0);

            var lblTo = new Label { Text = "Đến ngày", Anchor = AnchorStyles.Right, AutoSize = false, TextAlign = ContentAlignment.MiddleRight, Width = 100 };
            dtTo.Width = 120; dtTo.Margin = new Padding(6, 4, 12, 4);
            filters.Controls.Add(lblTo, 2, 0);
            filters.Controls.Add(dtTo, 3, 0);

            var lblTheo = new Label { Text = "Theo", Anchor = AnchorStyles.Right, AutoSize = false, TextAlign = ContentAlignment.MiddleRight, Width = 100 };
            cboGran.Width = 120; cboGran.Margin = new Padding(6, 4, 12, 4);
            filters.Controls.Add(lblTheo, 4, 0);
            filters.Controls.Add(cboGran, 5, 0);

            // Buttons panel (right) spanning two rows
            var btns = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, WrapContents = false, AutoSize = true };
            btnFilter.Size = new Size(90, 30); btnReset.Size = new Size(90, 30); btnExport.Size = new Size(90, 30); btnPrint.Size = new Size(90, 30);
            btns.Controls.Add(btnPrint); btns.Controls.Add(btnExport); btns.Controls.Add(btnReset); btns.Controls.Add(btnFilter);
            filters.SetRowSpan(btns, 2);
            filters.Controls.Add(btns, 6, 0);

            // Row 1: Top(n)
            var lblTop = new Label { Text = "Top (n)", Anchor = AnchorStyles.Right, AutoSize = false, TextAlign = ContentAlignment.MiddleRight, Width = 100 };
            numTop.Width = 70; numTop.Margin = new Padding(6, 4, 12, 4);
            filters.Controls.Add(lblTop, 0, 1);
            filters.Controls.Add(numTop, 1, 1);

            root.Controls.Add(filters, 0, 2);

            // Grid
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 34;
            root.Controls.Add(dgv, 0, 3);

            ResumeLayout();
        }

        void StyleCard(Label lbl, string title, string valueText)
        {
            lbl.AutoSize = false;
            lbl.Dock = DockStyle.Fill;
            lbl.BorderStyle = BorderStyle.FixedSingle;
            lbl.Padding = new Padding(10);
            lbl.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            lbl.Text = $"{title}\n{valueText}";
            lbl.Height = 56;
        }

        static void SetupChart(Chart c, string title, SeriesChartType type)
        {
            c.Dock = DockStyle.Fill;
            c.ChartAreas.Clear();
            var ca = new ChartArea("ca");
            ca.AxisX.MajorGrid.Enabled = false;
            ca.AxisY.MajorGrid.LineColor = Color.Gainsboro;
            c.ChartAreas.Add(ca);
            c.Titles.Clear(); c.Titles.Add(title);
            c.Series.Clear();
            var s = new Series("S") { ChartType = type, XValueType = ChartValueType.String, YValueType = ChartValueType.Double };
            if (type == SeriesChartType.Line) s.BorderWidth = 3;
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

        // ================== Behavior ==================
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

            // Bảng chi tiết đơn hàng
            var flat = _view.Select(x => new
            {
                x.Ngay,
                x.SoHD,
                KhachHang = x.Customer,
                SoMatHang = x.SoMatHang,
                TongSL = x.TongSL,
                DoanhThu = x.DoanhThu
            })
            .OrderByDescending(x => x.Ngay).ThenByDescending(x => x.DoanhThu).ToList();

            dgv.DataSource = null;
            dgv.Columns.Clear();
            dgv.DataSource = flat;

            dgv.Columns[nameof(SaleOrder.Ngay)].HeaderText = "Ngày";
            dgv.Columns["SoHD"].HeaderText = "Số HĐ";
            dgv.Columns["KhachHang"].HeaderText = "Khách hàng";
            dgv.Columns["SoMatHang"].HeaderText = "Số mặt hàng";
            dgv.Columns["TongSL"].HeaderText = "Tổng SL";
            dgv.Columns["DoanhThu"].HeaderText = "Doanh thu (₫)";
            dgv.Columns[nameof(SaleOrder.Ngay)].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgv.Columns["DoanhThu"].DefaultCellStyle.Format = "N0";

            // KPI
            var total = _view.Sum(o => o.DoanhThu);
            lblTotalValue.Text = string.Format("{0:N0} ₫", total);

            // Charts
            UpdateCharts();
        }

        void UpdateCharts()
        {
            var gran = cboGran.SelectedItem?.ToString() ?? "Ngày";
            // 1) Line chart: Revenue by period
            IEnumerable<(string Key, decimal Sum)> series;
            if (gran == "Năm")
            {
                series = _view.GroupBy(o => o.Ngay.ToString("yyyy"))
                              .Select(g => (g.Key, g.Sum(x => x.DoanhThu)))
                              .OrderBy(x => x.Key);
            }
            else if (gran == "Tháng")
            {
                series = _view.GroupBy(o => o.Ngay.ToString("yyyy-MM"))
                              .Select(g => (g.Key, g.Sum(x => x.DoanhThu)))
                              .OrderBy(x => x.Key);
            }
            else
            {
                series = _view.GroupBy(o => o.Ngay.ToString("dd/MM"))
                              .Select(g => (g.Key, g.Sum(x => x.DoanhThu)))
                              .OrderBy(x => DateTime.ParseExact(x.Key, "dd/MM", CultureInfo.InvariantCulture));
            }

            chRevenueLine.Series["S"].Points.Clear();
            foreach (var p in series) chRevenueLine.Series["S"].Points.AddXY(p.Key, (double)p.Sum);

            // 2) Top drugs
            int topN = (int)numTop.Value;
            var topDrugs = _view.SelectMany(o => o.Items)
                                .GroupBy(i => i.Drug)
                                .Select(g => new { Drug = g.Key, Sum = g.Sum(x => x.Amount) })
                                .OrderByDescending(x => x.Sum)
                                .Take(topN).ToList();

            chTopDrugs.Series["S"].Points.Clear();
            foreach (var d in topDrugs) chTopDrugs.Series["S"].Points.AddXY(d.Drug, (double)d.Sum);
            chTopDrugs.ChartAreas[0].AxisX.Interval = 1;

            // 3) Top customers by revenue
            var topCus = _view.GroupBy(o => o.Customer)
                              .Select(g => new { Customer = g.Key, Sum = g.Sum(x => x.DoanhThu) })
                              .OrderByDescending(x => x.Sum)
                              .Take(topN).ToList();

            chTopCustomers.Series["S"].Points.Clear();
            foreach (var c in topCus) chTopCustomers.Series["S"].Points.AddXY(c.Customer, (double)c.Sum);
            chTopCustomers.ChartAreas[0].AxisX.Interval = 1;
        }

        void ExportCsv()
        {
            var data = dgv.DataSource as IEnumerable<object>;
            if (data == null || !data.Cast<object>().Any())
            {
                MessageBox.Show("Không có dữ liệu để xuất."); return;
            }
            using var sfd = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = $"bao_cao_doanh_thu_{DateTime.Now:yyyyMMdd_HHmm}.csv" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using var sw = new StreamWriter(sfd.FileName);
                sw.WriteLine("Ngay,SoHD,KhachHang,SoMatHang,TongSL,DoanhThu");
                foreach (dynamic x in data)
                {
                    sw.WriteLine($"{x.Ngay:yyyy-MM-dd},{x.SoHD},{x.KhachHang},{x.SoMatHang},{x.TongSL},{Convert.ToDecimal(x.DoanhThu).ToString(CultureInfo.InvariantCulture)}");
                }
                sw.Flush();
                MessageBox.Show("Xuất CSV thành công!");
            }
        }

        // ================== Print ==================
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

            // Header
            e.Graphics.DrawString("BÁO CÁO DOANH THU", title, Brushes.Black, left, y);
            y += 24;
            e.Graphics.DrawString($"Khoảng thời gian: {dtFrom.Value:dd/MM/yyyy} - {dtTo.Value:dd/MM/yyyy} | Theo: {cboGran.SelectedItem}", normal, Brushes.Black, left, y);
            y += 20;

            // Tổng
            var total = _view.Sum(o => o.DoanhThu);
            e.Graphics.DrawString($"Tổng doanh thu: {total:N0} ₫", new Font("Segoe UI", 10, FontStyle.Bold), Brushes.Black, left, y);
            y += 26;

            // Liệt kê tối đa 40 dòng đơn hàng
            e.Graphics.DrawString("Ngày | Số HĐ | Khách hàng | Doanh thu", normal, Brushes.Black, left, y);
            y += 16;
            e.Graphics.DrawLine(Pens.Black, left, y, e.MarginBounds.Right, y);
            y += 6;

            var flat = _view.OrderByDescending(o => o.Ngay).ThenByDescending(o => o.DoanhThu).ToList();
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
            _printIndex = 0; e.HasMorePages = false;
        }
    }
}
