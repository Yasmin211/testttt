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
            public string MaDanhMuc { get; set; } = "";
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
                                MaDanhMuc = g.First().MaDanhMuc,
                                TenDanhMuc = g.Key,
                                SoThuoc = g.Count(),
                                TongTon = g.Sum(x => x.TonKho),
                                SapHetHan = g.Count(x => x.SapHetHan(threshold)),
                                SapHetHang = g.Count(x => x.SapHetHang())
                            }).OrderBy(x => x.TenDanhMuc).ToList();

            dgvTongHop.DataSource = null;
            dgvTongHop.Columns.Clear();
            dgvTongHop.DataSource = rows;

            // Đổi tên cột sang tiếng Việt
            if (dgvTongHop.Columns["MaDanhMuc"] != null)
                dgvTongHop.Columns["MaDanhMuc"].HeaderText = "Mã danh mục";
            if (dgvTongHop.Columns["TenDanhMuc"] != null)
                dgvTongHop.Columns["TenDanhMuc"].HeaderText = "Tên danh mục";
            if (dgvTongHop.Columns["SoThuoc"] != null)
                dgvTongHop.Columns["SoThuoc"].HeaderText = "Số loại thuốc";
            if (dgvTongHop.Columns["TongTon"] != null)
                dgvTongHop.Columns["TongTon"].HeaderText = "Tổng tồn kho";
            if (dgvTongHop.Columns["SapHetHan"] != null)
                dgvTongHop.Columns["SapHetHan"].HeaderText = "Sắp hết hạn";
            if (dgvTongHop.Columns["SapHetHang"] != null)
                dgvTongHop.Columns["SapHetHang"].HeaderText = "Sắp hết hàng";
        }

        // ======================================================
        // 🔹 CHI TIẾT THUỐC
        // ======================================================
        void FilterDetails(string? dm)
        {
            var data = string.IsNullOrEmpty(dm) ? _view : _view.Where(x => x.TenDanhMuc == dm).ToList();

            // Xóa event handler cũ để tránh đăng ký nhiều lần
            dgvChiTiet.CellFormatting -= DgvChiTiet_CellFormatting;
            
            dgvChiTiet.DataSource = null;
            dgvChiTiet.Columns.Clear();

            var iconCol = new DataGridViewImageColumn 
            { 
          HeaderText = "", 
    Width = 28, 
         ImageLayout = DataGridViewImageCellLayout.Zoom,
           Name = "IconColumn"
         };
         dgvChiTiet.Columns.Add(iconCol);
            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductStock.MaThuoc), HeaderText = "Mã thuốc" });
            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductStock.TenThuoc), HeaderText = "Tên thuốc" });
          dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductStock.MaDanhMuc), HeaderText = "Mã danh mục" });
            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductStock.TenDanhMuc), HeaderText = "Tên danh mục" });
   dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductStock.TonKho), HeaderText = "Tồn kho" });
       dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductStock.NguongToiThieu), HeaderText = "Ngưỡng tối thiểu" });
   var colHD = new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductStock.HanSuDung), HeaderText = "Hạn sử dụng" };
       colHD.DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvChiTiet.Columns.Add(colHD);
      dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductStock.ConLai), HeaderText = "Còn lại (ngày)" });

            dgvChiTiet.DataSource = new BindingList<ProductStock>(data);
            
            // Đăng ký event handler sau khi bind data
            dgvChiTiet.CellFormatting += DgvChiTiet_CellFormatting;
     }

  void DgvChiTiet_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
 if (e.RowIndex < 0) return;
            if (e.ColumnIndex != 0) return;

       if (dgvChiTiet.Rows[e.RowIndex].DataBoundItem is ProductStock p)
         {
  if (p.SapHetHang()) 
      {
     e.Value = iconStop;
       e.FormattingApplied = true;
   }
      else if (p.SapHetHan(NguongHetHan)) 
  {
             e.Value = iconWarn;
    e.FormattingApplied = true;
              }
    else 
   {
          e.Value = null;
        e.FormattingApplied = false;
    }
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
       .Select(x => new { x.MaThuoc, x.TenThuoc, x.MaDanhMuc, x.TenDanhMuc, x.HanSuDung, x.ConLai })
.ToList();
   dgvHetHan.DataSource = hetHan;
            
          // Đổi tên cột sang tiếng Việt
            if (dgvHetHan.Columns["MaThuoc"] != null)
    dgvHetHan.Columns["MaThuoc"].HeaderText = "Mã thuốc";
    if (dgvHetHan.Columns["TenThuoc"] != null)
      dgvHetHan.Columns["TenThuoc"].HeaderText = "Tên thuốc";
    if (dgvHetHan.Columns["MaDanhMuc"] != null)
   dgvHetHan.Columns["MaDanhMuc"].HeaderText = "Mã danh mục";
    if (dgvHetHan.Columns["TenDanhMuc"] != null)
 dgvHetHan.Columns["TenDanhMuc"].HeaderText = "Tên danh mục";
if (dgvHetHan.Columns["HanSuDung"] != null)
    {
  dgvHetHan.Columns["HanSuDung"].HeaderText = "Hạn sử dụng";
   dgvHetHan.Columns["HanSuDung"].DefaultCellStyle.Format = "dd/MM/yyyy";
   }
       if (dgvHetHan.Columns["ConLai"] != null)
dgvHetHan.Columns["ConLai"].HeaderText = "Còn lại (ngày)";

  var hetHang = _view.Where(x => x.SapHetHang())
 .OrderBy(x => x.TonKho)
  .Select(x => new { x.MaThuoc, x.TenThuoc, x.MaDanhMuc, x.TenDanhMuc, x.TonKho, x.NguongToiThieu })
  .ToList();
      dgvHetHang.DataSource = hetHang;
    
   // Đổi tên cột sang tiếng Việt
   if (dgvHetHang.Columns["MaThuoc"] != null)
  dgvHetHang.Columns["MaThuoc"].HeaderText = "Mã thuốc";
     if (dgvHetHang.Columns["TenThuoc"] != null)
 dgvHetHang.Columns["TenThuoc"].HeaderText = "Tên thuốc";
  if (dgvHetHang.Columns["MaDanhMuc"] != null)
    dgvHetHang.Columns["MaDanhMuc"].HeaderText = "Mã danh mục";
  if (dgvHetHang.Columns["TenDanhMuc"] != null)
 dgvHetHang.Columns["TenDanhMuc"].HeaderText = "Tên danh mục";
 if (dgvHetHang.Columns["TonKho"] != null)
      dgvHetHang.Columns["TonKho"].HeaderText = "Tồn kho";
    if (dgvHetHang.Columns["NguongToiThieu"] != null)
   dgvHetHang.Columns["NguongToiThieu"].HeaderText = "Ngưỡng tối thiểu";
      }

        // ======================================================
        // 🔹 IN BÁO CÁO
        // ======================================================
        void PrintPreview()
    {
 if (!_view.Any()) { MessageBox.Show("Không có dữ liệu để in."); return; }
     
    // Reset trạng thái in
       printRowIndex = 0;
        
            printDoc = new PrintDocument();
          printDoc.DocumentName = "Báo cáo tồn kho";
       printDoc.DefaultPageSettings.Landscape = true; // In ngang để có nhiều cột
            printDoc.PrintPage += PrintDoc_PrintPage;
     
            using var prev = new PrintPreviewDialog 
 { 
    Document = printDoc, 
       Width = 1200, 
          Height = 800,
   WindowState = FormWindowState.Maximized
      };
        prev.ShowDialog();
        }

      int printRowIndex = 0;
        
        void PrintDoc_PrintPage(object? sender, PrintPageEventArgs e)
 {
            try
            {
      Graphics g = e.Graphics;
                int leftMargin = e.MarginBounds.Left;
                int topMargin = e.MarginBounds.Top;
  int pageWidth = e.MarginBounds.Width;
     int pageHeight = e.MarginBounds.Height;
     int y = topMargin;

 // Font định nghĩa
     Font titleFont = new Font("Times New Roman", 16, FontStyle.Bold);
     Font headerFont = new Font("Times New Roman", 11, FontStyle.Bold);
          Font normalFont = new Font("Times New Roman", 10);
   Font footerFont = new Font("Times New Roman", 10, FontStyle.Italic);

          // ========== TIÊU ĐỀ ==========
        string title = "BÁO CÁO HÀNG TỒN KHO TRONG THÁNG";
            SizeF titleSize = g.MeasureString(title, titleFont);
        g.DrawString(title, titleFont, Brushes.Black, leftMargin + (pageWidth - titleSize.Width) / 2, y);
           y += (int)titleSize.Height + 5;

      // Thời gian
          string dateRange = $"Từ ngày: {DateTime.Now:dd/MM/yyyy}... Đến ngày: {DateTime.Now:dd/MM/yyyy}...";
     SizeF dateSize = g.MeasureString(dateRange, normalFont);
            g.DrawString(dateRange, normalFont, Brushes.Black, leftMargin + (pageWidth - dateSize.Width) / 2, y);
  y += (int)dateSize.Height + 20;

// ========== BẢNG DỮ LIỆU ==========
           // Định nghĩa các cột
           int colSTT = 50;
      int colMaThuoc = 100;
     int colTenThuoc = 180;
         int colSoLuongXuat = 120;
       int colSoLuongNhap = 120;
           int colTonKho = 100;

        int totalWidth = colSTT + colMaThuoc + colTenThuoc + colSoLuongXuat + colSoLuongNhap + colTonKho;
    int startX = leftMargin + (pageWidth - totalWidth) / 2;

              // Vẽ header bảng
        Pen tablePen = new Pen(Color.Black, 1);
Brush headerBrush = Brushes.LightGray;
      
  int currentX = startX;
       
// Header row background
              g.FillRectangle(headerBrush, startX, y, totalWidth, 30);
                g.DrawRectangle(tablePen, startX, y, totalWidth, 30);

 // Header cells
   DrawTableCell(g, "STT", currentX, y, colSTT, 30, headerFont, tablePen);
  currentX += colSTT;
     
     DrawTableCell(g, "Mã thuốc", currentX, y, colMaThuoc, 30, headerFont, tablePen);
           currentX += colMaThuoc;
       
     DrawTableCell(g, "Tên thuốc", currentX, y, colTenThuoc, 30, headerFont, tablePen);
            currentX += colTenThuoc;
     
        DrawTableCell(g, "Số lượng xuất", currentX, y, colSoLuongXuat, 30, headerFont, tablePen);
  currentX += colSoLuongXuat;
       
      DrawTableCell(g, "Số lượng nhập", currentX, y, colSoLuongNhap, 30, headerFont, tablePen);
       currentX += colSoLuongNhap;
      
     DrawTableCell(g, "Tồn kho", currentX, y, colTonKho, 30, headerFont, tablePen);
        
  y += 30;

                // Vẽ dữ liệu
          int rowHeight = 25;
            int maxRowsPerPage = (pageHeight - (y - topMargin) - 100) / rowHeight;
      int rowsPrinted = 0;
                int tongTonKho = 0;

             while (printRowIndex < _view.Count && rowsPrinted < maxRowsPerPage)
         {
        var item = _view[printRowIndex];
  currentX = startX;

    // STT
   DrawTableCell(g, (printRowIndex + 1).ToString(), currentX, y, colSTT, rowHeight, normalFont, tablePen);
             currentX += colSTT;

        // Mã thuốc
        DrawTableCell(g, item.MaThuoc, currentX, y, colMaThuoc, rowHeight, normalFont, tablePen);
       currentX += colMaThuoc;

        // Tên thuốc
     DrawTableCell(g, item.TenThuoc, currentX, y, colTenThuoc, rowHeight, normalFont, tablePen);
         currentX += colTenThuoc;

         // Số lượng xuất (giả định = 0, có thể lấy từ DB)
         DrawTableCell(g, "0", currentX, y, colSoLuongXuat, rowHeight, normalFont, tablePen, StringAlignment.Far);
  currentX += colSoLuongXuat;

       // Số lượng nhập (giả định = 0, có thể lấy từ DB)
     DrawTableCell(g, "0", currentX, y, colSoLuongNhap, rowHeight, normalFont, tablePen, StringAlignment.Far);
    currentX += colSoLuongNhap;

   // Tồn kho
              DrawTableCell(g, item.TonKho.ToString(), currentX, y, colTonKho, rowHeight, normalFont, tablePen, StringAlignment.Far);

   tongTonKho += item.TonKho;
  y += rowHeight;
     printRowIndex++;
       rowsPrinted++;
     }

     // Nếu là trang cuối, vẽ tổng kết
          if (printRowIndex >= _view.Count)
  {
        y += 10;
     
         // Tổng tồn kho (viết bằng số)
       string tongSo = $"Tổng tồn kho (Viết bằng số): {tongTonKho:N0}";
 g.DrawString(tongSo, normalFont, Brushes.Black, startX, y);
          y += 25;

      // Tổng tồn kho (viết bằng chữ)
    string tongChu = $"Tổng tồn kho (Viết bằng chữ): {NumberToWords(tongTonKho)} hộp";
          g.DrawString(tongChu, normalFont, Brushes.Black, startX, y);
   y += 40;

    // Chữ ký
      int signatureY = y;
      string signDate = $"Ngày...Tháng...Năm...";
           g.DrawString(signDate, footerFont, Brushes.Black, startX + totalWidth - 200, signatureY);
      signatureY += 20;

    string signTitle = "Người lập phiếu";
              SizeF signSize = g.MeasureString(signTitle, headerFont);
           g.DrawString(signTitle, headerFont, Brushes.Black, startX + totalWidth - 200 + (200 - signSize.Width) / 2, signatureY);
       signatureY += 20;

  string signNote = "(Ký, họ tên)";
         SizeF noteSize = g.MeasureString(signNote, footerFont);
     g.DrawString(signNote, footerFont, Brushes.Black, startX + totalWidth - 200 + (200 - noteSize.Width) / 2, signatureY);

    e.HasMorePages = false;
           printRowIndex = 0; // Reset cho lần in tiếp theo
     }
  else
          {
             e.HasMorePages = true;
      }
            }
            catch (Exception ex)
            {
    MessageBox.Show("Lỗi in báo cáo: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
             e.HasMorePages = false;
  }
        }

  void DrawTableCell(Graphics g, string text, int x, int y, int width, int height, Font font, Pen pen, StringAlignment align = StringAlignment.Center)
{
            // Vẽ viền
       g.DrawRectangle(pen, x, y, width, height);

            // Vẽ text
         StringFormat sf = new StringFormat
       {
      Alignment = align,
    LineAlignment = StringAlignment.Center,
        Trimming = StringTrimming.EllipsisCharacter
            };

            Rectangle rect = new Rectangle(x + 3, y, width - 6, height);
            g.DrawString(text, font, Brushes.Black, rect, sf);
        }

        string NumberToWords(int number)
    {
       if (number == 0) return "Không";

          string[] ones = { "", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
         string[] teens = { "mười", "mười một", "mười hai", "mười ba", "mười bốn", "mười lăm", "mười sáu", "mười bảy", "mười tám", "mười chín" };
            string[] tens = { "", "", "hai mươi", "ba mươi", "bốn mươi", "năm mươi", "sáu mươi", "bảy mươi", "tám mươi", "chín mươi" };

            if (number < 0) return "âm " + NumberToWords(Math.Abs(number));
            if (number < 10) return ones[number];
         if (number < 20) return teens[number - 10];
         if (number < 100)
     {
    int ten = number / 10;
 int one = number % 10;
      if (one == 0) return tens[ten];
       if (one == 5 && ten > 1) return tens[ten] + " lăm";
          if (one == 1 && ten > 1) return tens[ten] + " mốt";
              return tens[ten] + " " + ones[one];
   }
       if (number < 1000)
            {
  int hundred = number / 100;
     int remainder = number % 100;
    if (remainder == 0) return ones[hundred] + " trăm";
       if (remainder < 10) return ones[hundred] + " trăm lẻ " + NumberToWords(remainder);
                return ones[hundred] + " trăm " + NumberToWords(remainder);
            }
if (number < 1000000)
    {
                int thousand = number / 1000;
       int remainder = number % 1000;
          if (remainder == 0) return NumberToWords(thousand) + " nghìn";
     if (remainder < 100) return NumberToWords(thousand) + " nghìn lẻ " + NumberToWords(remainder);
  return NumberToWords(thousand) + " nghìn " + NumberToWords(remainder);
            }

      int million = number / 1000000;
            int remainderMil = number % 1000000;
            if (remainderMil == 0) return NumberToWords(million) + " triệu";
  return NumberToWords(million) + " triệu " + NumberToWords(remainderMil);
      }
    }
}
