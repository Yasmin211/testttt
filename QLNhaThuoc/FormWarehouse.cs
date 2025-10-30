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
        private readonly Button btnInventory = new() { Text = "Tạo phiếu kiểm kê", Height = 34, Width = 160, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(47, 111, 237), ForeColor = Color.White };

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
            RowHeadersVisible = false
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
            topBar.Controls.AddRange(new Control[] { btnAdd, btnInventory });
            void LayoutTopButtons()
            {
                btnInventory.Location = new Point(topBar.Width - btnInventory.Width, 6);
                btnAdd.Location = new Point(btnInventory.Left - 12 - btnAdd.Width, 6);
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

            rightPanel.Controls.Add(topBar);
            rightPanel.Controls.Add(searchBar);
            rightPanel.Controls.Add(dgv);

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
                var t = f.Value;
                try
                {
                    string cs = ConfigurationManager.ConnectionStrings["Db"].ConnectionString;
                    using SqlConnection con = new(cs);
                    con.Open();

                    // Insert vào Thuoc
                    string sql1 = @"INSERT INTO Thuoc(MaThuoc, TenThuoc, HoatChat, DongGoi, GiaBan) 
                                    VALUES(@MaThuoc, @TenThuoc, @HoatChat, @DongGoi, @GiaBan)";
                    using SqlCommand cmd1 = new(sql1, con);
                    cmd1.Parameters.AddWithValue("@MaThuoc", t.MaThuoc);
                    cmd1.Parameters.AddWithValue("@TenThuoc", t.TenThuoc);
                    cmd1.Parameters.AddWithValue("@HoatChat", t.HoatChat);
                    cmd1.Parameters.AddWithValue("@DongGoi", t.DongGoi);
                    cmd1.Parameters.AddWithValue("@GiaBan", t.GiaBan);
                    cmd1.ExecuteNonQuery();

                    // Insert vào TonKho
                    string sql2 = @"INSERT INTO TonKho(MaLoSX, MaThuoc, TonKho, HanSuDung)
                                    VALUES(@MaLoSX, @MaThuoc, @TonKho, @HanSuDung)";
                    using SqlCommand cmd2 = new(sql2, con);
                    cmd2.Parameters.AddWithValue("@MaLoSX", t.MaLoSX);
                    cmd2.Parameters.AddWithValue("@MaThuoc", t.MaThuoc);
                    cmd2.Parameters.AddWithValue("@TonKho", t.TonKho);
                    cmd2.Parameters.AddWithValue("@HanSuDung", t.HanSuDung);
                    cmd2.ExecuteNonQuery();

                    MessageBox.Show("✅ Đã thêm thuốc mới vào hệ thống!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDataFromDatabase();
                    ApplyFilter();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("❌ Lỗi khi thêm thuốc: " + ex.Message);
                }
            }
        }
    }

    // ========== POPUP FORM ==========
    public class FormThuocPro : Form
    {
        public FormWarehouse.Thuoc Value { get; private set; } = new();
        TextBox txtMa = new(), txtTen = new(), txtHoat = new(), txtDongGoi = new(), txtLo = new();
        NumericUpDown nudGia = new(), nudTon = new();
        DateTimePicker dtHSD = new();

        public FormThuocPro()
        {
            Text = "Thêm thuốc mới";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(500, 500);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Font = new Font("Segoe UI", 9F);

            var table = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20), ColumnCount = 2 };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            void Row(string label, Control c)
            {
                table.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left });
                table.Controls.Add(c);
            }

            dtHSD.Format = DateTimePickerFormat.Custom;
            dtHSD.CustomFormat = "dd/MM/yyyy";
            nudGia.Maximum = 100000000;
            nudGia.ThousandsSeparator = true;
            nudGia.TextAlign = HorizontalAlignment.Right;
            nudTon.Maximum = 1000000;
            nudTon.ThousandsSeparator = true;

            Row("Mã thuốc:", txtMa);
            Row("Tên thuốc:", txtTen);
            Row("Hoạt chất:", txtHoat);
            Row("Đóng gói:", txtDongGoi);
            Row("Giá bán:", nudGia);
            Row("Lô SX:", txtLo);
            Row("Hạn SD:", dtHSD);
            Row("Tồn kho:", nudTon);
            Controls.Add(table);

            var footer = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Height = 60, Padding = new Padding(10) };
            var btnOK = new Button { Text = "Lưu", Width = 100, Height = 36, BackColor = Color.FromArgb(47, 111, 237), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            var btnCancel = new Button { Text = "Hủy", Width = 90, Height = 36 };
            btnOK.Click += (s, e) =>
            {
                if (TryCollect(out var t))
                {
                    Value = t;
                    DialogResult = DialogResult.OK;
                }
            };
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
            footer.Controls.AddRange(new Control[] { btnOK, btnCancel });
            Controls.Add(footer);
        }

        private bool TryCollect(out FormWarehouse.Thuoc t)
        {
            t = new FormWarehouse.Thuoc
            {
                MaThuoc = txtMa.Text.Trim(),
                TenThuoc = txtTen.Text.Trim(),
                HoatChat = txtHoat.Text.Trim(),
                DongGoi = txtDongGoi.Text.Trim(),
                GiaBan = nudGia.Value,
                MaLoSX = txtLo.Text.Trim(),
                HanSuDung = dtHSD.Value.Date,
                TonKho = (int)nudTon.Value
            };

            if (string.IsNullOrEmpty(t.MaThuoc) || string.IsNullOrEmpty(t.TenThuoc))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Mã thuốc và Tên thuốc.",
                    "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
    }
}
