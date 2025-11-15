using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace QLNhaThuoc
{
    public class FormWarehouse : Form
    {
        // ===== MODEL =====
        public class Thuoc
        {
            public string MaThuoc { get; set; } = "";
            public string TenThuoc { get; set; } = "";
            public string HoatChat { get; set; } = "";
            public string DongGoi { get; set; } = "";
            public string DonViTinh { get; set; } = "";
            public string MaLoSX { get; set; } = "";
            public DateTime HanSuDung { get; set; }
            public int TonKho { get; set; }
            public string NhaCungCap { get; set; } = "";
            public decimal GiaBan { get; set; }
        }

        // ===== DATA =====
        private readonly BindingList<Thuoc> _all = new();
        private readonly BindingList<Thuoc> _view = new();

        // ===== UI =====
        private readonly GroupBox gbFilter = new() { Text = "Bộ lọc", Width = 410, Dock = DockStyle.Left, Padding = new Padding(12) };
        private readonly DateTimePicker dtFrom = new() { Format = DateTimePickerFormat.Short };
        private readonly DateTimePicker dtTo = new() { Format = DateTimePickerFormat.Short };
        private readonly ComboBox cbNcc = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly CheckBox chkLow = new() { Text = "Dưới mức tồn kho thấp" };
        private readonly NumericUpDown nudStock = new() { Minimum = 0, Maximum = 1000000, Width = 120, ThousandsSeparator = true };
        private readonly Button btnApply = new() { Text = "Áp dụng", Width = 110, Height = 32 };
        private readonly Button btnClear = new() { Text = "Bỏ lọc", Width = 100, Height = 32 };

        private readonly Panel rightPanel = new() { Dock = DockStyle.Fill, Padding = new Padding(12) };
        private readonly Button btnAdd = new() { Text = "+ Thêm thuốc", Height = 34, Width = 140, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(47, 111, 237), ForeColor = Color.White };

        private readonly TextBox txtSearch = new() { PlaceholderText = "Nhập tên hoặc mã thuốc..." };
        private readonly Button btnSearch = new() { Text = "Tra cứu", Width = 90, Height = 28 };

        private readonly DataGridView dgv = new()
        {
            Dock = DockStyle.Fill,
            BackgroundColor = Color.White,
            AutoGenerateColumns = false,
            ReadOnly = true,
            AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersVisible = false,
            ColumnHeadersVisible = true,// ✅ Bật hiển thị header
            ColumnHeadersHeight = 40,     // ✅ Set chiều cao header
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        };

        public FormWarehouse()
        {
            Text = "Quản lý kho thuốc";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1280, 760);
            Font = new Font("Segoe UI", 9F);
            BackColor = Color.White;

            BuildLeft();
            BuildRight();
            BuildGrid();
            LoadDataFromDatabase();
            ApplyFilter();
        }

        // ========= LOAD DỮ LIỆU =========
        private void LoadDataFromDatabase()
        {
            _all.Clear();
            try
            {
                string cs = ConfigurationManager.ConnectionStrings["Db"].ConnectionString;
                using SqlConnection con = new(cs);
                con.Open();

                string sql = @"
                    SELECT 
                        t.MaThuoc,
                        t.TenThuoc,
                        t.HoatChat,
                        t.DongGoi,
                        d.TenDonViTinh AS DonViTinh,
                        k.MaLoSX,
                        k.HanSuDung,
                        k.TonKho,
                        n.TenNCC AS NhaCungCap,
                        t.GiaBan
                    FROM Thuoc t
                    LEFT JOIN TonKho k ON t.MaThuoc = k.MaThuoc
                    LEFT JOIN DonViTinh d ON t.MaDonViTinh = d.MaDonViTinh
                    LEFT JOIN NhaCungCap n ON t.MaNCC = n.MaNCC;";

                using SqlCommand cmd = new(sql, con);
                using SqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    _all.Add(new Thuoc
                    {
                        MaThuoc = rd["MaThuoc"].ToString(),
                        TenThuoc = rd["TenThuoc"].ToString(),
                        HoatChat = rd["HoatChat"].ToString(),
                        DongGoi = rd["DongGoi"].ToString(),
                        DonViTinh = rd["DonViTinh"].ToString(),
                        MaLoSX = rd["MaLoSX"].ToString(),
                        HanSuDung = rd["HanSuDung"] == DBNull.Value ? DateTime.Today : Convert.ToDateTime(rd["HanSuDung"]),
                        TonKho = rd["TonKho"] == DBNull.Value ? 0 : Convert.ToInt32(rd["TonKho"]),
                        NhaCungCap = rd["NhaCungCap"].ToString(),
                        GiaBan = rd["GiaBan"] == DBNull.Value ? 0 : Convert.ToDecimal(rd["GiaBan"])
                    });
                }

                cbNcc.Items.Clear();
                cbNcc.Items.Add("(Tất cả)");
                foreach (var ncc in _all.Select(x => x.NhaCungCap).Distinct().OrderBy(x => x))
                    cbNcc.Items.Add(ncc);
                cbNcc.SelectedIndex = 0;

                dtFrom.Value = DateTime.Today.AddMonths(-1);
                dtTo.Value = DateTime.Today.AddYears(3);
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi tải dữ liệu kho: " + ex.Message,
                    "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ========= UI =========
        private void BuildLeft()
        {
            Controls.Add(rightPanel);
            Controls.Add(gbFilter);
            var tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, AutoSize = true };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            void Row(string label, Control c)
            {
                tbl.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 6, 0, 6) });
                c.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                c.Margin = new Padding(0, 3, 0, 3);
                tbl.Controls.Add(c);
            }
            Row("HSD từ:", dtFrom);
            Row("đến:", dtTo);
            Row("Nhà CC:", cbNcc);
            tbl.Controls.Add(new Label()); tbl.Controls.Add(chkLow);
            Row("Tồn kho ≤", nudStock);
            var pnlBtn = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true };
            pnlBtn.Controls.AddRange(new Control[] { btnApply, btnClear });
            tbl.Controls.Add(new Label()); tbl.Controls.Add(pnlBtn);
            gbFilter.Controls.Add(tbl);
            btnApply.Click += (_, __) => ApplyFilter();
            btnClear.Click += (_, __) => { ClearFilter(); ApplyFilter(); };
        }

        private void BuildRight()
        {
            var topBar = new Panel { Dock = DockStyle.Top, Height = 46 };
            topBar.Controls.AddRange(new Control[] { btnAdd });
            void LayoutTopButtons()
            {
            btnAdd.Location = new Point(topBar.Width - btnAdd.Width, 6);
            }
            topBar.Resize += (_, __) => LayoutTopButtons();
            LayoutTopButtons();

            var searchBar = new Panel { Dock = DockStyle.Top, Height = 42 };
            searchBar.Controls.Add(txtSearch);
            searchBar.Controls.Add(btnSearch);
            txtSearch.Location = new Point(0, 8);
            txtSearch.Width = searchBar.Width - 100;
            btnSearch.Location = new Point(searchBar.Width - 90, 7);
            searchBar.Resize += (_, __) =>
            {
                txtSearch.Width = searchBar.Width - 100;
                btnSearch.Location = new Point(searchBar.Width - 90, 7);
            };

            // ✅ QUAN TRỌNG: Add theo thứ tự ngược khi dùng Dock
            rightPanel.Controls.Add(dgv);   // Fill - Add TRƯỚC
            rightPanel.Controls.Add(searchBar);   // Top - Add sau
            rightPanel.Controls.Add(topBar);    // Top - Add cuối

            btnAdd.Click += (_, __) => AddThuoc();
            btnSearch.Click += (_, __) => ApplyFilter();
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) ApplyFilter(); };
        }

        private void BuildGrid()
        {
            dgv.Columns.Clear();
            dgv.Columns.Add(MkText(nameof(Thuoc.MaThuoc), "Mã thuốc", 100));
            dgv.Columns.Add(MkText(nameof(Thuoc.TenThuoc), "Tên thuốc", 200));
            dgv.Columns.Add(MkText(nameof(Thuoc.HoatChat), "Hoạt chất", 150));
            dgv.Columns.Add(MkText(nameof(Thuoc.DongGoi), "Đóng gói", 150));
            dgv.Columns.Add(MkText(nameof(Thuoc.DonViTinh), "Đơn vị", 80));
            dgv.Columns.Add(MkText(nameof(Thuoc.MaLoSX), "Lô SX", 80));
            dgv.Columns.Add(MkDate(nameof(Thuoc.HanSuDung), "Hạn sử dụng", 120));
            dgv.Columns.Add(MkInt(nameof(Thuoc.TonKho), "Tồn kho", 90));
            dgv.Columns.Add(MkText(nameof(Thuoc.NhaCungCap), "Nhà cung cấp", 160));
            dgv.Columns.Add(MkMoney(nameof(Thuoc.GiaBan), "Giá bán", 100));
            dgv.DataSource = _view;
        }

        private static DataGridViewTextBoxColumn MkText(string prop, string header, int w)
            => new() { DataPropertyName = prop, HeaderText = header, Width = w, SortMode = DataGridViewColumnSortMode.Automatic };
        private static DataGridViewTextBoxColumn MkInt(string prop, string header, int w)
        { var c = MkText(prop, header, w); c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; return c; }
        private static DataGridViewTextBoxColumn MkMoney(string prop, string header, int w)
        { var c = MkText(prop, header, w); c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; c.DefaultCellStyle.Format = "#,0"; return c; }
        private static DataGridViewTextBoxColumn MkDate(string prop, string header, int w)
        { var c = MkText(prop, header, w); c.DefaultCellStyle.Format = "dd/MM/yyyy"; return c; }

        // ========= FILTER =========
        private void ApplyFilter()
        {
            string kw = txtSearch.Text.Trim().ToLowerInvariant();
            string ncc = cbNcc.SelectedIndex <= 0 ? "" : cbNcc.SelectedItem.ToString();
            var from = dtFrom.Value.Date;
            var to = dtTo.Value.Date;
            int stockMax = (int)nudStock.Value;

            IEnumerable<Thuoc> q = _all;
            if (from <= to) q = q.Where(x => x.HanSuDung >= from && x.HanSuDung <= to);
            if (!string.IsNullOrEmpty(ncc)) q = q.Where(x => x.NhaCungCap == ncc);
            if (chkLow.Checked) q = q.Where(x => x.TonKho < 10);
            if (stockMax > 0) q = q.Where(x => x.TonKho <= stockMax);
            if (!string.IsNullOrEmpty(kw))
                q = q.Where(x => x.TenThuoc.ToLower().Contains(kw) ||
                                 x.MaThuoc.ToLower().Contains(kw) ||
                                 x.HoatChat.ToLower().Contains(kw));
            _view.Clear();
            foreach (var it in q) _view.Add(it);
        }

        private void ClearFilter()
        {
            dtFrom.Value = DateTime.Today.AddMonths(-1);
            dtTo.Value = DateTime.Today.AddYears(3);
            cbNcc.SelectedIndex = 0;
            chkLow.Checked = false;
            nudStock.Value = 0;
            txtSearch.Text = "";
        }

        // ========= POPUP THÊM THUỐC =========
        private void AddThuoc()
        {
            var f = new FormThuocPro();
            if (f.ShowDialog() == DialogResult.OK)
            {
                 // Form đã tự lưu vào database, chỉ cần refresh lại
                LoadDataFromDatabase();
                ApplyFilter();
 }
        }
    }

    // ========== POPUP FORM THÊM THUỐC ==========
    public class FormThuocPro : Form
    {
        public FormWarehouse.Thuoc Value { get; private set; } = new();
        
        // UI Controls
        TextBox txtMa = new(), txtTen = new(), txtHoat = new(), txtDongGoi = new(), txtLo = new();
     NumericUpDown nudGia = new() { Maximum = 100000000, ThousandsSeparator = true, TextAlign = HorizontalAlignment.Right };
        NumericUpDown nudTon = new() { Maximum = 1000000, ThousandsSeparator = true };
   DateTimePicker dtHSD = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };
   ComboBox cbDonVi = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        ComboBox cbNCC = new() { DropDownStyle = ComboBoxStyle.DropDownList };

        // Dictionaries to map tên -> mã
        Dictionary<string, string> donViDict = new();
      Dictionary<string, string> nccDict = new();

   public FormThuocPro()
        {
            Text = "Thêm thuốc mới";
    StartPosition = FormStartPosition.CenterParent;
Size = new Size(550, 600);
   FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 9.5F);
    BackColor = Color.White;

    LoadComboData();
       BuildUI();
        }

        private void LoadComboData()
        {
            try
        {
    string cs = ConfigurationManager.ConnectionStrings["Db"].ConnectionString;
          using SqlConnection con = new(cs);
              con.Open();

    // Load Đơn vị tính
   string sql1 = "SELECT MaDonViTinh, TenDonViTinh FROM DonViTinh ORDER BY TenDonViTinh";
   using SqlCommand cmd1 = new(sql1, con);
                using SqlDataReader rd1 = cmd1.ExecuteReader();
       cbDonVi.Items.Clear();
     donViDict.Clear();
                while (rd1.Read())
         {
     string ma = rd1["MaDonViTinh"].ToString();
    string ten = rd1["TenDonViTinh"].ToString();
     cbDonVi.Items.Add(ten);
        donViDict[ten] = ma;
      }
 rd1.Close();

                // Load Nhà cung cấp
                string sql2 = "SELECT MaNCC, TenNCC FROM NhaCungCap ORDER BY TenNCC";
      using SqlCommand cmd2 = new(sql2, con);
using SqlDataReader rd2 = cmd2.ExecuteReader();
        cbNCC.Items.Clear();
            nccDict.Clear();
         while (rd2.Read())
                {
 string ma = rd2["MaNCC"].ToString();
         string ten = rd2["TenNCC"].ToString();
           cbNCC.Items.Add(ten);
         nccDict[ten] = ma;
          }

     if (cbDonVi.Items.Count > 0) cbDonVi.SelectedIndex = 0;
    if (cbNCC.Items.Count > 0) cbNCC.SelectedIndex = 0;
  }
 catch (Exception ex)
            {
  MessageBox.Show("❌ Lỗi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
       }
    }

        private void BuildUI()
 {
  var mainPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(25, 20, 25, 20) };
      
  var table = new TableLayoutPanel 
     { 
          Dock = DockStyle.Fill, 
         ColumnCount = 2,
                AutoSize = true,
           CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };
         table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        void Row(string label, Control c)
            {
            var lbl = new Label 
          { 
       Text = label, 
          AutoSize = true, 
    Anchor = AnchorStyles.Left,
          Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
   ForeColor = Color.FromArgb(64, 64, 64),
         Margin = new Padding(0, 10, 0, 0)
          };
         c.Anchor = AnchorStyles.Left | AnchorStyles.Right;
       c.Margin = new Padding(0, 8, 0, 0);
     c.Font = new Font("Segoe UI", 9.5F);
      if (c is TextBox || c is ComboBox)
      {
        c.Height = 32;
          }
          table.Controls.Add(lbl);
                table.Controls.Add(c);
     }

            dtHSD.Value = DateTime.Today;
     
         Row("Mã thuốc:", txtMa);
       Row("Tên thuốc:", txtTen);
   Row("Hoạt chất:", txtHoat);
            Row("Đóng gói:", txtDongGoi);
   Row("Đơn vị tính:", cbDonVi);
  Row("Nhà cung cấp:", cbNCC);
        Row("Giá bán (VNĐ):", nudGia);
            Row("Lô SX:", txtLo);
 Row("Hạn sử dụng:", dtHSD);
            Row("Tồn kho:", nudTon);

mainPanel.Controls.Add(table);
            Controls.Add(mainPanel);

 // Footer buttons
   var footer = new Panel { Dock = DockStyle.Bottom, Height = 70, BackColor = Color.FromArgb(245, 245, 245) };
  var btnLuu = new Button 
        { 
    Text = "Lưu", 
     Width = 110, 
          Height = 38,
BackColor = Color.FromArgb(47, 111, 237),
      ForeColor = Color.White,
             FlatStyle = FlatStyle.Flat,
    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
          };
     btnLuu.FlatAppearance.BorderSize = 0;
            
         var btnHuy = new Button 
     { 
         Text = "Hủy", 
    Width = 100, 
   Height = 38,
  BackColor = Color.White,
          ForeColor = Color.FromArgb(100, 100, 100),
                FlatStyle = FlatStyle.Flat,
      Font = new Font("Segoe UI", 10F),
     Cursor = Cursors.Hand
 };
      btnHuy.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);

  btnLuu.Location = new Point(footer.Width - btnLuu.Width - btnHuy.Width - 30, 16);
            btnHuy.Location = new Point(footer.Width - btnHuy.Width - 15, 16);
            
            footer.Resize += (s, e) =>
            {
              btnLuu.Location = new Point(footer.Width - btnLuu.Width - btnHuy.Width - 30, 16);
        btnHuy.Location = new Point(footer.Width - btnHuy.Width - 15, 16);
    };

    btnLuu.Click += (s, e) =>
       {
     if (ValidateAndSave())
    {
        DialogResult = DialogResult.OK;
    }
      };
        btnHuy.Click += (s, e) => DialogResult = DialogResult.Cancel;

      footer.Controls.AddRange(new Control[] { btnLuu, btnHuy });
 Controls.Add(footer);
        }

      private bool ValidateAndSave()
     {
  if (string.IsNullOrWhiteSpace(txtMa.Text))
     {
        MessageBox.Show("⚠️ Vui lòng nhập Mã thuốc!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          txtMa.Focus();
  return false;
            }
    if (string.IsNullOrWhiteSpace(txtTen.Text))
    {
      MessageBox.Show("⚠️ Vui lòng nhập Tên thuốc!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
   txtTen.Focus();
         return false;
  }
     if (cbDonVi.SelectedIndex < 0)
    {
                MessageBox.Show("⚠️ Vui lòng chọn Đơn vị tính!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
       return false;
            }
            if (cbNCC.SelectedIndex < 0)
  {
       MessageBox.Show("⚠️ Vui lòng chọn Nhà cung cấp!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    return false;
            }

            try
      {
     string cs = ConfigurationManager.ConnectionStrings["Db"].ConnectionString;
            using SqlConnection con = new(cs);
                con.Open();

         // Lấy mã đơn vị và nhà cung cấp
         string maDonVi = donViDict[cbDonVi.SelectedItem.ToString()];
   string maNCC = nccDict[cbNCC.SelectedItem.ToString()];

     // Kiểm tra mã thuốc đã tồn tại chưa
     string checkSql = "SELECT COUNT(*) FROM Thuoc WHERE MaThuoc = @MaThuoc";
         using SqlCommand checkCmd = new(checkSql, con);
     checkCmd.Parameters.AddWithValue("@MaThuoc", txtMa.Text.Trim());
            int count = (int)checkCmd.ExecuteScalar();
   if (count > 0)
        {
          MessageBox.Show("❌ Mã thuốc đã tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
          txtMa.Focus();
          return false;
    }

 // Insert vào bảng Thuoc
      string sqlThuoc = @"
    INSERT INTO Thuoc(MaThuoc, TenThuoc, HoatChat, DongGoi, MaDonViTinh, MaNCC, GiaBan)
      VALUES(@MaThuoc, @TenThuoc, @HoatChat, @DongGoi, @MaDonViTinh, @MaNCC, @GiaBan)";
     
    using SqlCommand cmdThuoc = new(sqlThuoc, con);
              cmdThuoc.Parameters.AddWithValue("@MaThuoc", txtMa.Text.Trim());
    cmdThuoc.Parameters.AddWithValue("@TenThuoc", txtTen.Text.Trim());
  cmdThuoc.Parameters.AddWithValue("@HoatChat", txtHoat.Text.Trim());
        cmdThuoc.Parameters.AddWithValue("@DongGoi", txtDongGoi.Text.Trim());
      cmdThuoc.Parameters.AddWithValue("@MaDonViTinh", maDonVi);
        cmdThuoc.Parameters.AddWithValue("@MaNCC", maNCC);
cmdThuoc.Parameters.AddWithValue("@GiaBan", nudGia.Value);
       cmdThuoc.ExecuteNonQuery();

     // Insert vào bảng TonKho
             string sqlKho = @"
          INSERT INTO TonKho(MaLoSX, MaThuoc, TonKho, HanSuDung)
           VALUES(@MaLoSX, @MaThuoc, @TonKho, @HanSuDung)";
           
    using SqlCommand cmdKho = new(sqlKho, con);
          cmdKho.Parameters.AddWithValue("@MaLoSX", txtLo.Text.Trim());
         cmdKho.Parameters.AddWithValue("@MaThuoc", txtMa.Text.Trim());
           cmdKho.Parameters.AddWithValue("@TonKho", (int)nudTon.Value);
      cmdKho.Parameters.AddWithValue("@HanSuDung", dtHSD.Value.Date);
     cmdKho.ExecuteNonQuery();

 MessageBox.Show("✅ Thêm thuốc mới thành công!", "Thành công", 
          MessageBoxButtons.OK, MessageBoxIcon.Information);
     return true;
    }
          catch (Exception ex)
            {
   MessageBox.Show("❌ Lỗi khi lưu: " + ex.Message, "Lỗi", 
          MessageBoxButtons.OK, MessageBoxIcon.Error);
   return false;
            }
        }
    }
}
