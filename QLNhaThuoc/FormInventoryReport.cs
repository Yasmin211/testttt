using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace QLNhaThuoc
{
    public partial class FormInventoryReport : Form
    {
        // ===================== Models =====================
        public class Category
        {
            public string CatId { get; set; } = "";
            public string Name { get; set; } = "";
        }

        public class ProductStock
        {
            public string ProdId { get; set; } = "";
            public string Name { get; set; } = "";
            public string CatId { get; set; } = "";
            public string CatName { get; set; } = "";
            public int Qty { get; set; }             // Tồn thực tế
            public int MinStock { get; set; }             // Ngưỡng sắp hết hàng
            public DateTime Expiry { get; set; }             // Hạn dùng gần nhất
            public int DaysLeft => (int)Math.Ceiling((Expiry.Date - DateTime.Today).TotalDays);
            public bool IsExpiringSoon(int daysThreshold) => DaysLeft <= daysThreshold;
            public bool IsLowStock() => Qty <= MinStock;
        }

        // ===================== Fake data =====================
        static class InventorySeeder
        {
            static readonly Random rnd = new Random(77);
            static readonly (string id, string name)[] CATS = new[]
            {
                ("OTC", "Thuốc OTC"),
                ("KS",  "Kháng sinh"),
                ("DD",  "Dạ dày - Tiêu hoá"),
                ("VT",  "Vitamin - Khoáng"),
                ("GY",  "Giảm đau - Hạ sốt")
            };

            static readonly string[] OTC = { "Salonpas Patch", "ORS Gói", "Tiffy", "Decolgen", "Cough Syrup 60ml" };
            static readonly string[] KS = { "Amoxicillin 500mg", "Cefalexin 500mg", "Azithromycin 250mg", "Ciprofloxacin 500mg" };
            static readonly string[] DD = { "Omeprazol 20mg", "Esomeprazole 40mg", "Antacid Tums", "Smecta", "Domperidone 10mg" };
            static readonly string[] VT = { "Vitamin C 1000mg", "Magie B6", "D3K2", "Zinc 15mg", "Multivitamin ABC" };
            static readonly string[] GY = { "Paracetamol 500mg", "Ibuprofen 400mg", "Meloxicam 7.5mg", "Naproxen 250mg" };

            public static (List<Category> cats, BindingList<ProductStock> products) Generate(int countPerCat = 18)
            {
                var cats = CATS.Select(c => new Category { CatId = c.id, Name = c.name }).ToList();
                var list = new BindingList<ProductStock>();

                foreach (var c in cats)
                {
                    string[] pool = c.CatId switch
                    {
                        "OTC" => OTC,
                        "KS" => KS,
                        "DD" => DD,
                        "VT" => VT,
                        _ => GY
                    };

                    for (int i = 0; i < countPerCat; i++)
                    {
                        string name = pool[rnd.Next(pool.Length)];
                        int qty = rnd.Next(0, 250);                 // có thể 0 để test hết hàng
                        int min = rnd.Next(10, 60);
                        // 25% gần hết hạn (<= 30 ngày), 8% đã hết hạn
                        int days = rnd.NextDouble() < 0.08 ? -rnd.Next(1, 15)
                                  : (rnd.NextDouble() < 0.25 ? rnd.Next(0, 30) : rnd.Next(31, 540));
                        DateTime exp = DateTime.Today.AddDays(days);

                        list.Add(new ProductStock
                        {
                            ProdId = $"{c.CatId}-{i:000}",
                            Name = name,
                            CatId = c.CatId,
                            CatName = c.Name,
                            Qty = qty,
                            MinStock = min,
                            Expiry = exp
                        });
                    }
                }
                return (cats, list);
            }
        }

        // ===================== UI controls =====================
        readonly ComboBox cboCategory = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        readonly NumericUpDown numDaysThreshold = new() { Minimum = 1, Maximum = 365, Value = 30, Width = 60 };
        readonly Button btnFilter = new() { Text = "Lọc dữ liệu", AutoSize = true, Padding = new Padding(8, 4, 8, 4) };
        readonly Button btnReset = new() { Text = "Xóa lọc", AutoSize = true, Padding = new Padding(8, 4, 8, 4) };
        readonly Button btnPrint = new() { Text = "In báo cáo", AutoSize = true, Padding = new Padding(8, 4, 8, 4) };

        readonly DataGridView dgvCatSummary = new()
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false
        };

        readonly DataGridView dgvDetails = new()
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false
        };

        readonly DataGridView dgvExpiringSoon = new()
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false
        };

        readonly DataGridView dgvLowStock = new()
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false
        };

        // Icons
        readonly Bitmap iconWarn = SystemIcons.Warning.ToBitmap();
        readonly Bitmap iconStop = SystemIcons.Error.ToBitmap();

        // ===================== Data =====================
        List<Category> _cats = new();
        BindingList<ProductStock> _all = new();
        List<ProductStock> _view = new();
        int ExpiringDaysThreshold => (int)numDaysThreshold.Value;

        // Print
        PrintDocument printDoc = new();

        // Minimal InitializeComponent when designer not present
        private void InitializeComponent() { }

        public FormInventoryReport()
        {
            InitializeComponent();
            Text = "Báo cáo tồn kho";
            Width = 1300; Height = 820; StartPosition = FormStartPosition.CenterScreen;
            // Open maximized by default for better visibility
            WindowState = FormWindowState.Maximized;

            BuildLayout();
            WireEvents();

            // Seed
            (_cats, _all) = InventorySeeder.Generate(16);

            // Filters
            var catNames = _cats.Select(c => c.Name).OrderBy(x => x).ToList();
            catNames.Insert(0, "(Tất cả)");
            cboCategory.DataSource = catNames;

            ApplyFilter();
        }

        // ===================== Layout =====================
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
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // filters
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 55));   // top split (summary + details)
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 45));   // bottom tabs
            Controls.Add(root);

            // Filters
            var filters = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 12,
                RowCount = 1,
                Margin = new Padding(0, 0, 0, 8),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            for (int i = 0; i < 12; i++) filters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 12f));

            AddLabeled(filters, 0, "Danh mục", cboCategory);
            AddLabeled(filters, 4, "Sắp hết hạn ≤ (ngày)", numDaysThreshold);

            var btns = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0)
            };
            // Add some gap so buttons show fully
            btns.Controls.Add(btnPrint);
            btns.Controls.Add(btnReset);
            btns.Controls.Add(btnFilter);
            filters.SetColumnSpan(btns, 4);
            filters.Controls.Add(btns, 8, 0);

            root.Controls.Add(filters, 0, 0);

            // Top: split summary + detail
            var topSplit = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 420
            };
            // Summary grid
            dgvCatSummary.EnableHeadersVisualStyles = false;
            dgvCatSummary.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvCatSummary.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            topSplit.Panel1.Padding = new Padding(0, 0, 8, 0);
            topSplit.Panel1.Controls.Add(dgvCatSummary);

            // Details grid
            dgvDetails.EnableHeadersVisualStyles = false;
            dgvDetails.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvDetails.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            topSplit.Panel2.Controls.Add(dgvDetails);

            root.Controls.Add(topSplit, 0, 1);

            // Bottom: tabs (Expiring soon / Low stock)
            var tabs = new TabControl { Dock = DockStyle.Fill };
            var tp1 = new TabPage("Thuốc sắp hết hạn") { Padding = new Padding(6) };
            var tp2 = new TabPage("Thuốc sắp hết hàng") { Padding = new Padding(6) };
            tp1.Controls.Add(dgvExpiringSoon);
            tp2.Controls.Add(dgvLowStock);
            tabs.TabPages.Add(tp1);
            tabs.TabPages.Add(tp2);
            root.Controls.Add(tabs, 0, 2);

            ResumeLayout();
        }

        void AddLabeled(TableLayoutPanel host, int colStart, string text, Control input)
        {
            var lbl = new Label { Text = text, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 6, 6, 0) };
            input.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            input.Margin = new Padding(0, 0, 12, 6);

            host.SetColumnSpan(lbl, 2);
            host.SetColumnSpan(input, 2);
            host.Controls.Add(lbl, colStart, 0);
            host.Controls.Add(input, colStart + 2, 0);
        }

        // ===================== Behavior =====================
        void WireEvents()
        {
            btnFilter.Click += (_, __) => ApplyFilter();
            btnReset.Click += (_, __) => { cboCategory.SelectedIndex = 0; numDaysThreshold.Value = 30; ApplyFilter(); };
            btnPrint.Click += (_, __) => PrintPreview();

            // click chọn danh mục để lọc chi tiết
            dgvCatSummary.SelectionChanged += (_, __) =>
            {
                if (dgvCatSummary.CurrentRow?.DataBoundItem is CatSummaryRow r)
                    FilterDetailsByCategory(r.CatName);
            };
        }

        void ApplyFilter()
        {
            string cat = cboCategory.SelectedItem?.ToString() ?? "(Tất cả)";
            var q = _all.AsEnumerable();
            if (cat != "(Tất cả)") q = q.Where(p => p.CatName == cat);

            _view = q.OrderBy(p => p.CatName).ThenBy(p => p.Name).ToList();

            BindSummaryGrid();
            FilterDetailsByCategory(cat == "(Tất cả)" ? null : cat);
            BindExtraTables();
        }

        // ------- Summary table -------
        class CatSummaryRow
        {
            public string CatName { get; set; } = "";
            public int SoSanPham { get; set; }
            public int TongTon { get; set; }
            public int SapHetHan { get; set; }
            public int SapHetHang { get; set; }
        }

        void BindSummaryGrid()
        {
            int threshold = ExpiringDaysThreshold;

            var rows = _view.GroupBy(p => p.CatName)
                            .Select(g => new CatSummaryRow
                            {
                                CatName = g.Key,
                                SoSanPham = g.Count(),
                                TongTon = g.Sum(x => x.Qty),
                                SapHetHan = g.Count(x => x.IsExpiringSoon(threshold)),
                                SapHetHang = g.Count(x => x.IsLowStock())
                            })
                            .OrderBy(x => x.CatName)
                            .ToList();

            dgvCatSummary.DataSource = null;
            dgvCatSummary.Columns.Clear();
            dgvCatSummary.DataSource = rows;

            dgvCatSummary.Columns[nameof(CatSummaryRow.CatName)].HeaderText = "Danh mục";
            dgvCatSummary.Columns[nameof(CatSummaryRow.SoSanPham)].HeaderText = "Số SP";
            dgvCatSummary.Columns[nameof(CatSummaryRow.TongTon)].HeaderText = "Tổng tồn";
            dgvCatSummary.Columns[nameof(CatSummaryRow.SapHetHan)].HeaderText = "Sắp hết hạn";
            dgvCatSummary.Columns[nameof(CatSummaryRow.SapHetHang)].HeaderText = "Sắp hết hàng";
        }

        // ------- Details with warning icons -------
        void FilterDetailsByCategory(string? catName)
        {
            var data = string.IsNullOrEmpty(catName) ? _view : _view.Where(p => p.CatName == catName).ToList();

            dgvDetails.DataSource = null;
            dgvDetails.Columns.Clear();
            dgvDetails.AutoGenerateColumns = false;

            // Warning column
            var colIcon = new DataGridViewImageColumn
            {
                HeaderText = "",
                Width = 28,
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                ValueType = typeof(Image)
            };
            // Ensure nulls don't attempt formatting
            colIcon.DefaultCellStyle.NullValue = null;
            dgvDetails.Columns.Add(colIcon);

            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductStock.Name), HeaderText = "Thuốc" });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductStock.CatName), HeaderText = "Danh mục" });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductStock.Qty), HeaderText = "Tồn" });
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductStock.MinStock), HeaderText = "Ngưỡng" });
            var colExpiry = new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductStock.Expiry), HeaderText = "Hạn dùng" };
            colExpiry.DefaultCellStyle.Format = "dd/MM/yyyy";
            colExpiry.ValueType = typeof(DateTime);
            dgvDetails.Columns.Add(colExpiry);
            dgvDetails.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductStock.DaysLeft), HeaderText = "Còn (ngày)" });

            // Ensure single handler for formatting
            dgvDetails.CellFormatting -= DgvDetails_CellFormatting;
            dgvDetails.CellFormatting += DgvDetails_CellFormatting;

            // Handle DataError to suppress default dialog when cell formatting types mismatch
            dgvDetails.DataError -= DgvDetails_DataError;
            dgvDetails.DataError += DgvDetails_DataError;

            dgvDetails.DataSource = new BindingList<ProductStock>(data);
        }

        private void DgvDetails_DataError(object? sender, DataGridViewDataErrorEventArgs e)
        {
            // Prevent the default DataGridView error dialog from showing.
            // Swallow formatting exceptions that can occur when temporarily assigning image values.
            e.ThrowException = false;
        }

        private void DgvDetails_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var dgv = sender as DataGridView;
            if (dgv == null) return;

            // Icon column
            if (e.ColumnIndex == 0)
            {
                if (dgv.Rows[e.RowIndex].DataBoundItem is ProductStock item)
                {
                    if (item.IsLowStock()) e.Value = iconStop;
                    else if (item.IsExpiringSoon(ExpiringDaysThreshold)) e.Value = iconWarn;
                    else e.Value = null;
                    e.FormattingApplied = true;
                }
                return;
            }

            // Expiry formatted by DefaultCellStyle; nothing else needed here
        }

        void BindExtraTables()
        {
            int threshold = ExpiringDaysThreshold;

            // Expiring soon
            var exp = _view.Where(p => p.IsExpiringSoon(threshold))
                           .OrderBy(p => p.DaysLeft)
                           .Select(p => new
                           {
                               Thuoc = p.Name,
                               DanhMuc = p.CatName,
                               HanDung = p.Expiry.ToString("dd/MM/yyyy"),
                               ConNgay = p.DaysLeft
                           }).ToList();
            dgvExpiringSoon.DataSource = null;
            dgvExpiringSoon.Columns.Clear();
            dgvExpiringSoon.DataSource = exp;
            dgvExpiringSoon.Columns["Thuoc"].HeaderText = "Thuốc";
            dgvExpiringSoon.Columns["DanhMuc"].HeaderText = "Danh mục";
            dgvExpiringSoon.Columns["HanDung"].HeaderText = "Hạn dùng";
            dgvExpiringSoon.Columns["ConNgay"].HeaderText = "Còn (ngày)";

            // Low stock
            var low = _view.Where(p => p.IsLowStock())
                           .OrderBy(p => p.Qty)
                           .Select(p => new
                           {
                               Thuoc = p.Name,
                               DanhMuc = p.CatName,
                               Ton = p.Qty,
                               Nguong = p.MinStock
                           }).ToList();
            dgvLowStock.DataSource = null;
            dgvLowStock.Columns.Clear();
            dgvLowStock.DataSource = low;
            dgvLowStock.Columns["Thuoc"].HeaderText = "Thuốc";
            dgvLowStock.Columns["DanhMuc"].HeaderText = "Danh mục";
            dgvLowStock.Columns["Ton"].HeaderText = "Tồn";
            dgvLowStock.Columns["Nguong"].HeaderText = "Ngưỡng";
        }

        // ===================== Print =====================
        void PrintPreview()
        {
            if (!_view.Any())
            {
                MessageBox.Show("Không có dữ liệu để in."); return;
            }
            printDoc = new PrintDocument();
            printDoc.DocumentName = "Báo cáo tồn kho";
            printDoc.PrintPage += PrintDoc_PrintPage;
            using var prev = new PrintPreviewDialog { Document = printDoc, Width = 1000, Height = 700 };
            prev.ShowDialog();
        }

        int _printPhase = 0; // 0 summary, 1 expiring, 2 low stock
        int _rowIndex = 0;
        void PrintDoc_PrintPage(object? sender, PrintPageEventArgs e)
        {
            int left = e.MarginBounds.Left, y = e.MarginBounds.Top;
            var title = new Font("Segoe UI", 12, FontStyle.Bold);
            var normal = new Font("Segoe UI", 9);

            if (_printPhase == 0)
            {
                e.Graphics.DrawString("BÁO CÁO TỒN KHO - TỔNG HỢP DANH MỤC", title, Brushes.Black, left, y);
                y += 26;
                e.Graphics.DrawString($"Danh mục: {cboCategory.Text} | Cảnh báo hết hạn ≤ {ExpiringDaysThreshold} ngày", normal, Brushes.Black, left, y);
                y += 20;

                e.Graphics.DrawString("Danh mục | Số SP | Tổng tồn | Sắp hết hạn | Sắp hết hàng", normal, Brushes.Black, left, y);
                y += 16;
                e.Graphics.DrawLine(Pens.Black, left, y, e.MarginBounds.Right, y);
                y += 6;

                var rows = (dgvCatSummary.DataSource as IEnumerable<CatSummaryRow>)?.ToList() ?? new();
                while (_rowIndex < rows.Count)
                {
                    var r = rows[_rowIndex];
                    string line = $"{r.CatName} | {r.SoSanPham} | {r.TongTon} | {r.SapHetHan} | {r.SapHetHang}";
                    e.Graphics.DrawString(line, normal, Brushes.Black, left, y);
                    y += 18;
                    if (y > e.MarginBounds.Bottom - 20) { e.HasMorePages = true; return; }
                    _rowIndex++;
                }
                _rowIndex = 0; _printPhase = 1; e.HasMorePages = true; return;
            }

            if (_printPhase == 1)
            {
                e.Graphics.DrawString("BÁO CÁO - THUỐC SẮP HẾT HẠN", title, Brushes.Black, left, y);
                y += 26;
                var rows = (dgvExpiringSoon.DataSource as IEnumerable<dynamic>)?.ToList() ?? new();
                e.Graphics.DrawString("Thuốc | Danh mục | Hạn dùng | Còn (ngày)", normal, Brushes.Black, left, y);
                y += 16; e.Graphics.DrawLine(Pens.Black, left, y, e.MarginBounds.Right, y); y += 6;

                while (_rowIndex < rows.Count)
                {
                    dynamic r = rows[_rowIndex];
                    string line = $"{r.Thuoc} | {r.DanhMuc} | {r.HanDung} | {r.ConNgay}";
                    e.Graphics.DrawString(line, normal, Brushes.Black, left, y);
                    y += 18;
                    if (y > e.MarginBounds.Bottom - 20) { e.HasMorePages = true; return; }
                    _rowIndex++;
                }
                _rowIndex = 0; _printPhase = 2; e.HasMorePages = true; return;
            }

            if (_printPhase == 2)
            {
                e.Graphics.DrawString("BÁO CÁO - THUỐC SẮP HẾT HÀNG", title, Brushes.Black, left, y);
                y += 26;
                var rows = (dgvLowStock.DataSource as IEnumerable<dynamic>)?.ToList() ?? new();
                e.Graphics.DrawString("Thuốc | Danh mục | Tồn | Ngưỡng", normal, Brushes.Black, left, y);
                y += 16; e.Graphics.DrawLine(Pens.Black, left, y, e.MarginBounds.Right, y); y += 6;

                while (_rowIndex < rows.Count)
                {
                    dynamic r = rows[_rowIndex];
                    string line = $"{r.Thuoc} | {r.DanhMuc} | {r.Ton} | {r.Nguong}";
                    e.Graphics.DrawString(line, normal, Brushes.Black, left, y);
                    y += 18;
                    if (y > e.MarginBounds.Bottom - 20) { e.HasMorePages = true; return; }
                    _rowIndex++;
                }
                _rowIndex = 0; _printPhase = 0; e.HasMorePages = false; return;
            }
        }
    }
}
