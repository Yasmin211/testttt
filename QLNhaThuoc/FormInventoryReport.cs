using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace QLNhaThuoc
{
    public partial class FormInventoryReport : Form
    {
        // ===================== Models =====================
        public class ProductStock
        {
            public string MaThuoc { get; set; } = "";
            public string TenThuoc { get; set; } = "";
            public string MaDanhMuc { get; set; } = "";
            public string TenDanhMuc { get; set; } = "";
            public int TonKho { get; set; }
            public int NguongToiThieu { get; set; } = 20; // quy định ngưỡng cảnh báo
            public DateTime HanSuDung { get; set; }

            public int ConLai => (int)Math.Ceiling((HanSuDung.Date - DateTime.Today).TotalDays);
            public bool SapHetHan(int nguongNgay) => ConLai <= nguongNgay;
            public bool SapHetHang() => TonKho <= NguongToiThieu;
        }

        // ===================== UI controls =====================
        readonly ComboBox cboDanhMuc = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        readonly NumericUpDown numNgay = new() { Minimum = 1, Maximum = 365, Value = 30, Width = 60 };
        readonly Button btnLoc = new() { Text = "Lọc dữ liệu", AutoSize = true };
        readonly Button btnXoa = new() { Text = "Xóa lọc", AutoSize = true };
        readonly Button btnIn = new() { Text = "In báo cáo", AutoSize = true };

        readonly DataGridView dgvTongHop = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        readonly DataGridView dgvChiTiet = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        readonly DataGridView dgvHetHan = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        readonly DataGridView dgvHetHang = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

        readonly Bitmap iconWarn = SystemIcons.Warning.ToBitmap();
        readonly Bitmap iconStop = SystemIcons.Error.ToBitmap();

        List<ProductStock> _all = new();
        List<ProductStock> _view = new();
        int NguongHetHan => (int)numNgay.Value;

        PrintDocument printDoc = new();

        private void InitializeComponent() { }

        public FormInventoryReport()
        {
            InitializeComponent();
            Text = "Báo cáo tồn kho (Dữ liệu thật)";
            Width = 1300; Height = 820;
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;

            BuildLayout();
            WireEvents();
            LoadDataFromDatabase();
            ApplyFilter();
        }

        // ======================================================
        // 🔹 LOAD DỮ LIỆU TỪ SQL
        // ======================================================
        void LoadDataFromDatabase()
        {
            try
            {
                string cs = ConfigurationManager.ConnectionStrings["Db"].ConnectionString;
                using SqlConnection con = new(cs);
                con.Open();

                string sql = @"
                    SELECT 
                        t.MaThuoc,
                        t.TenThuoc,
                        dm.MaDanhMuc,
                        dm.TenDanhMuc,
                        tk.TonKho,
                        tk.HanSuDung
                    FROM Thuoc t
                    JOIN DanhMucThuoc dm ON t.MaDanhMuc = dm.MaDanhMuc
                    JOIN TonKho tk ON t.MaThuoc = tk.MaThuoc
                    ORDER BY dm.TenDanhMuc, t.TenThuoc;";

                using SqlCommand cmd = new(sql, con);
                using SqlDataReader rd = cmd.ExecuteReader();

                _all.Clear();
                while (rd.Read())
                {
                    _all.Add(new ProductStock
                    {
                        MaThuoc = rd.GetString(0),
                        TenThuoc = rd.GetString(1),
                        MaDanhMuc = rd.GetString(2),
                        TenDanhMuc = rd.GetString(3),
                        TonKho = rd.IsDBNull(4) ? 0 : rd.GetInt32(4),
                        HanSuDung = rd.IsDBNull(5) ? DateTime.Today.AddMonths(6) : rd.GetDateTime(5)
                    });
                }

                var dm = _all.Select(x => x.TenDanhMuc).Distinct().OrderBy(x => x).ToList();
                dm.Insert(0, "(Tất cả)");
                cboDanhMuc.DataSource = dm;
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi tải dữ liệu từ SQL: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ======================================================
        // 🔹 LAYOUT
        // ======================================================
        void BuildLayout()
        {
            SuspendLayout();

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                Padding = new Padding(8)
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
            Controls.Add(root);

            // Bộ lọc
            var filters = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true
            };
            filters.Controls.Add(new Label { Text = "Danh mục:", AutoSize = true, Margin = new Padding(4, 8, 4, 0) });
            filters.Controls.Add(cboDanhMuc);
            filters.Controls.Add(new Label { Text = "Sắp hết hạn ≤ (ngày):", AutoSize = true, Margin = new Padding(12, 8, 4, 0) });
            filters.Controls.Add(numNgay);
            filters.Controls.Add(btnLoc);
            filters.Controls.Add(btnXoa);
            filters.Controls.Add(btnIn);
            root.Controls.Add(filters);

            // Khu vực trên: tổng hợp + chi tiết
            var topSplit = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Vertical, SplitterDistance = 400 };
            topSplit.Panel1.Controls.Add(dgvTongHop);
            topSplit.Panel2.Controls.Add(dgvChiTiet);
            root.Controls.Add(topSplit);

            // Tab dưới: sắp hết hạn / sắp hết hàng
            var tabs = new TabControl { Dock = DockStyle.Fill };
            var tp1 = new TabPage("Thuốc sắp hết hạn") { Padding = new Padding(6) };
            var tp2 = new TabPage("Thuốc sắp hết hàng") { Padding = new Padding(6) };
            tp1.Controls.Add(dgvHetHan);
            tp2.Controls.Add(dgvHetHang);
            tabs.TabPages.Add(tp1);
            tabs.TabPages.Add(tp2);
            root.Controls.Add(tabs);

            ResumeLayout();
        }

        // ======================================================
        // 🔹 SỰ KIỆN
        // ======================================================
        void WireEvents()
        {
            btnLoc.Click += (_, __) => ApplyFilter();
            btnXoa.Click += (_, __) =>
            {
                cboDanhMuc.SelectedIndex = 0;
                numNgay.Value = 30;
                ApplyFilter();
            };
            btnIn.Click += (_, __) => PrintPreview();

            dgvTongHop.SelectionChanged += (_, __) =>
            {
                if (dgvTongHop.CurrentRow?.DataBoundItem is CatSummaryRow row)
                    FilterDetails(row.TenDanhMuc);
            };
        }

        // ======================================================
        // 🔹 ÁP DỤNG LỌC DỮ LIỆU
        // ======================================================
        void ApplyFilter()
        {
            string dm = cboDanhMuc.SelectedItem?.ToString() ?? "(Tất cả)";
            var q = _all.AsEnumerable();
            if (dm != "(Tất cả)") q = q.Where(x => x.TenDanhMuc == dm);

            _view = q.OrderBy(x => x.TenDanhMuc).ThenBy(x => x.TenThuoc).ToList();

            BindSummary();
            FilterDetails(dm == "(Tất cả)" ? null : dm);
            BindExtraTables();
        }

        // ======================================================
        // 🔹 TỔNG HỢP DANH MỤC
        // ======================================================
        class CatSummaryRow
        {
            public string TenDanhMuc { get; set; } = "";
            public int SoThuoc { get; set; }
            public int TongTon { get; set; }
            public int SapHetHan { get; set; }
            public int SapHetHang { get; set; }
        }

        void BindSummary()
        {
            int threshold = NguongHetHan;

            var rows = _view.GroupBy(x => x.TenDanhMuc)
                            .Select(g => new CatSummaryRow
                            {
                                TenDanhMuc = g.Key,
                                SoThuoc = g.Count(),
                                TongTon = g.Sum(x => x.TonKho),
                                SapHetHan = g.Count(x => x.SapHetHan(threshold)),
                                SapHetHang = g.Count(x => x.SapHetHang())
                            }).OrderBy(x => x.TenDanhMuc).ToList();

            dgvTongHop.DataSource = null;
            dgvTongHop.Columns.Clear();
            dgvTongHop.DataSource = rows;
        }

        // ======================================================
        // 🔹 CHI TIẾT THUỐC
        // ======================================================
        void FilterDetails(string? dm)
        {
            var data = string.IsNullOrEmpty(dm) ? _view : _view.Where(x => x.TenDanhMuc == dm).ToList();

            dgvChiTiet.DataSource = null;
            dgvChiTiet.Columns.Clear();

            var iconCol = new DataGridViewImageColumn { HeaderText = "", Width = 28, ImageLayout = DataGridViewImageCellLayout.Zoom };
            dgvChiTiet.Columns.Add(iconCol);
            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductStock.TenThuoc), HeaderText = "Tên thuốc" });
            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductStock.TenDanhMuc), HeaderText = "Danh mục" });
            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductStock.TonKho), HeaderText = "Tồn" });
            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductStock.NguongToiThieu), HeaderText = "Ngưỡng" });
            var colHD = new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductStock.HanSuDung), HeaderText = "Hạn dùng" };
            colHD.DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvChiTiet.Columns.Add(colHD);
            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductStock.ConLai), HeaderText = "Còn (ngày)" });

            dgvChiTiet.DataSource = new BindingList<ProductStock>(data);
            dgvChiTiet.CellFormatting += DgvChiTiet_CellFormatting;
        }

        void DgvChiTiet_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex != 0) return;

            if (dgvChiTiet.Rows[e.RowIndex].DataBoundItem is ProductStock p)
            {
                if (p.SapHetHang()) e.Value = iconStop;
                else if (p.SapHetHan(NguongHetHan)) e.Value = iconWarn;
                else e.Value = null;
                e.FormattingApplied = true;
            }
        }

        // ======================================================
        // 🔹 BẢNG CẢNH BÁO
        // ======================================================
        void BindExtraTables()
        {
            int threshold = NguongHetHan;

            var hetHan = _view.Where(x => x.SapHetHan(threshold))
                              .OrderBy(x => x.ConLai)
                              .Select(x => new { x.TenThuoc, x.TenDanhMuc, x.HanSuDung, x.ConLai })
                              .ToList();
            dgvHetHan.DataSource = hetHan;

            var hetHang = _view.Where(x => x.SapHetHang())
                               .OrderBy(x => x.TonKho)
                               .Select(x => new { x.TenThuoc, x.TenDanhMuc, x.TonKho, x.NguongToiThieu })
                               .ToList();
            dgvHetHang.DataSource = hetHang;
        }

        // ======================================================
        // 🔹 IN BÁO CÁO
        // ======================================================
        void PrintPreview()
        {
            if (!_view.Any()) { MessageBox.Show("Không có dữ liệu để in."); return; }
            printDoc = new PrintDocument();
            printDoc.DocumentName = "Báo cáo tồn kho";
            printDoc.PrintPage += PrintDoc_PrintPage;
            using var prev = new PrintPreviewDialog { Document = printDoc, Width = 1000, Height = 700 };
            prev.ShowDialog();
        }

        int phase = 0, row = 0;
        void PrintDoc_PrintPage(object? sender, PrintPageEventArgs e)
        {
            int left = e.MarginBounds.Left, y = e.MarginBounds.Top;
            var title = new Font("Segoe UI", 12, FontStyle.Bold);
            var normal = new Font("Segoe UI", 9);

            if (phase == 0)
            {
                e.Graphics.DrawString("BÁO CÁO TỒN KHO", title, Brushes.Black, left, y);
                y += 30;
                foreach (var r in dgvTongHop.Rows.Cast<DataGridViewRow>())
                {
                    string line = string.Join(" | ", r.Cells.Cast<DataGridViewCell>().Select(c => c.Value?.ToString()));
                    e.Graphics.DrawString(line, normal, Brushes.Black, left, y);
                    y += 18;
                    if (y > e.MarginBounds.Bottom - 20) { e.HasMorePages = true; return; }
                }
                phase = 1; y = e.MarginBounds.Top; e.HasMorePages = true; return;
            }

            if (phase == 1)
            {
                e.Graphics.DrawString("THUỐC SẮP HẾT HẠN", title, Brushes.Black, left, y);
                y += 30;
                foreach (var r in dgvHetHan.Rows.Cast<DataGridViewRow>())
                {
                    string line = string.Join(" | ", r.Cells.Cast<DataGridViewCell>().Select(c => c.Value?.ToString()));
                    e.Graphics.DrawString(line, normal, Brushes.Black, left, y);
                    y += 18;
                    if (y > e.MarginBounds.Bottom - 20) { e.HasMorePages = true; return; }
                }
                phase = 2; y = e.MarginBounds.Top; e.HasMorePages = true; return;
            }

            if (phase == 2)
            {
                e.Graphics.DrawString("THUỐC SẮP HẾT HÀNG", title, Brushes.Black, left, y);
                y += 30;
                foreach (var r in dgvHetHang.Rows.Cast<DataGridViewRow>())
                {
                    string line = string.Join(" | ", r.Cells.Cast<DataGridViewCell>().Select(c => c.Value?.ToString()));
                    e.Graphics.DrawString(line, normal, Brushes.Black, left, y);
                    y += 18;
                    if (y > e.MarginBounds.Bottom - 20) { e.HasMorePages = true; return; }
                }
                phase = 0; e.HasMorePages = false; return;
            }
        }
    }
}
