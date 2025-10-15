using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace QLNhaThuoc
{
    public class FormWarehouse : Form
    {
        // ===== Model =====
        public class Thuoc
        {
            public string TenThuoc { get; set; } = "";
            public string MaThuoc { get; set; } = "";
            public string HoatChat { get; set; } = "";
            public string QuyCach { get; set; } = "";
            public string DonViTinh { get; set; } = "";
            public string LoSX { get; set; } = "";
            public DateTime HanSuDung { get; set; }
            public int TonKho { get; set; }
            public int TonToiThieu { get; set; } = 10;
            public string NhaCungCap { get; set; } = "";
            public decimal Gia { get; set; }
            public bool DuoiToiThieu => TonKho < TonToiThieu;
        }

        // ===== Data =====
        private readonly BindingList<Thuoc> _all = new();
        private readonly BindingList<Thuoc> _view = new();

        // ===== UI trái: Bộ lọc =====
        private readonly GroupBox gbFilter = new() { Text = "Bộ lọc", Width = 410, Dock = DockStyle.Left, Padding = new Padding(12) };
        private readonly DateTimePicker dtFrom = new() { Format = DateTimePickerFormat.Short };
        private readonly DateTimePicker dtTo = new() { Format = DateTimePickerFormat.Short };
        private readonly ComboBox cbNcc = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly CheckBox chkLow = new() { Text = "Dưới mức" };
        private readonly NumericUpDown nudStock = new() { Minimum = 0, Maximum = 1000000, Width = 120, ThousandsSeparator = true };
        private readonly Button btnApply = new() { Text = "Áp dụng", Width = 110, Height = 32 };
        private readonly Button btnClear = new() { Text = "Bỏ lọc", Width = 100, Height = 32 };

        // ===== UI phải =====
        private readonly Panel rightPanel = new() { Dock = DockStyle.Fill, Padding = new Padding(12) };

        // Hàng 1: nút
        private readonly Button btnAdd = new() { Text = "+ Thêm thuốc", Height = 34, Width = 140, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(47, 111, 237), ForeColor = Color.White };
        private readonly Button btnInventory = new() { Text = "Tạo phiếu kiểm kê", Height = 34, Width = 160, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(47, 111, 237), ForeColor = Color.White };

        // Hàng 2: tra cứu
        private readonly TextBox txtSearch = new() { PlaceholderText = "Nhập tên/mã/hoạt chất và nhấn Enter..." };
        private readonly Button btnSearch = new() { Text = "Tra cứu", Width = 90, Height = 28 };

        // Hàng 3: bảng
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
            AutoScaleMode = AutoScaleMode.Dpi;

            BuildLeft();
            BuildRight();
            BuildGrid();
            Seed();
            ApplyFilter();
        }

        // ========= UI Left =========
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

            var pnlBtn = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, WrapContents = false, Dock = DockStyle.Fill };
            pnlBtn.Controls.AddRange(new Control[] { btnApply, btnClear });
            tbl.Controls.Add(new Label()); tbl.Controls.Add(pnlBtn);

            gbFilter.Controls.Add(tbl);

            btnApply.Click += (_, __) => ApplyFilter();
            btnClear.Click += (_, __) => { ClearFilter(); ApplyFilter(); };
        }

        // ========= UI Right =========
        private void BuildRight()
        {
            // Hàng 1: top bar với 2 nút
            var topBar = new Panel { Dock = DockStyle.Top, Height = 46 };
            topBar.Controls.AddRange(new Control[] { btnAdd, btnInventory });

            void LayoutTopButtons()
            {
                btnInventory.Location = new Point(topBar.Width - btnInventory.Width, 6);
                btnAdd.Location = new Point(btnInventory.Left - 12 - btnAdd.Width, 6);
            }
            topBar.Resize += (_, __) => LayoutTopButtons();
            LayoutTopButtons();

            // Hàng 2: tra cứu
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

            // Add controls in order so docking behaves predictably: topBar, searchBar, then dgv(fill)
            rightPanel.Controls.Add(topBar);
            rightPanel.Controls.Add(searchBar);
            rightPanel.Controls.Add(dgv);

            // events
            btnAdd.Click += (_, __) => AddThuoc();
            btnInventory.Click += (_, __) => OpenInventoryCount();
            btnSearch.Click += (_, __) => ApplyFilter();
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) ApplyFilter(); };
        }

        // ========= Grid =========
        private void BuildGrid()
        {
            // Reset columns and styles
            dgv.Columns.Clear();
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersVisible = true;

            // Explicit header style
            var hdrStyle = new DataGridViewCellStyle
            {
                BackColor = SystemColors.Control,
                ForeColor = SystemColors.ControlText,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                WrapMode = DataGridViewTriState.False
            };
            dgv.ColumnHeadersDefaultCellStyle = hdrStyle;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersHeight = 40;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            // Columns
            dgv.Columns.Add(MkText(nameof(Thuoc.TenThuoc), "Tên thuốc", 200));
            dgv.Columns.Add(MkText(nameof(Thuoc.MaThuoc), "Mã thuốc", 110));
            dgv.Columns.Add(MkText(nameof(Thuoc.HoatChat), "Hoạt chất", 160));
            dgv.Columns.Add(MkText(nameof(Thuoc.QuyCach), "Quy cách đóng gói", 200));
            dgv.Columns.Add(MkText(nameof(Thuoc.DonViTinh), "Đơn vị tính", 100));
            dgv.Columns.Add(MkText(nameof(Thuoc.LoSX), "Lô sản xuất", 120));
            dgv.Columns.Add(MkDate(nameof(Thuoc.HanSuDung), "Hạn sử dụng", 120));
            dgv.Columns.Add(MkInt(nameof(Thuoc.TonKho), "Số lượng tồn", 100));
            dgv.Columns.Add(MkText(nameof(Thuoc.NhaCungCap), "Nhà cung cấp", 160));
            dgv.Columns.Add(MkMoney(nameof(Thuoc.Gia), "Giá", 110));
            var editCol = new DataGridViewButtonColumn { HeaderText = "Sửa", Text = "Chỉnh sửa", UseColumnTextForButtonValue = true, Width = 100 };
            dgv.Columns.Add(editCol);

            // Layout
            dgv.RowTemplate.Height = 26;
            dgv.AllowUserToResizeRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgv.DataSource = _view;

            // Ensure headers are large enough and wrapped, force repaint
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersHeight = 48;
            dgv.AutoResizeColumnHeadersHeight();

            // Make sure DataGridView is on top of other controls so header isn't overlapped
            dgv.BringToFront();

            dgv.Refresh();
            dgv.Invalidate();
            dgv.Update();

            dgv.CellContentClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == dgv.Columns.Count - 1)
                    if (dgv.Rows[e.RowIndex].DataBoundItem is Thuoc t) EditThuoc(t);
            };
        }

        private static DataGridViewTextBoxColumn MkText(string prop, string header, int w) =>
            new() { DataPropertyName = prop, HeaderText = header, Width = w, SortMode = DataGridViewColumnSortMode.Automatic };
        private static DataGridViewTextBoxColumn MkInt(string prop, string header, int w)
        { var c = MkText(prop, header, w); c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; return c; }
        private static DataGridViewTextBoxColumn MkMoney(string prop, string header, int w)
        { var c = MkText(prop, header, w); c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; c.DefaultCellStyle.Format = "#,0"; return c; }
        private static DataGridViewTextBoxColumn MkDate(string prop, string header, int w)
        { var c = MkText(prop, header, w); c.DefaultCellStyle.Format = "dd/MM/yyyy"; return c; }

        // ========= Seed =========
        private void Seed()
        {
            _all.Add(new Thuoc { TenThuoc = "Paracetamol 500mg", MaThuoc = "PCM500", HoatChat = "Paracetamol", QuyCach = "Hộp 10 vỉ x 10 viên", DonViTinh = "Viên", LoSX = "L2408", HanSuDung = DateTime.Today.AddMonths(18), TonKho = 50, TonToiThieu = 30, NhaCungCap = "Dược Á Châu", Gia = 1200 });
            _all.Add(new Thuoc { TenThuoc = "Amoxicillin 500mg", MaThuoc = "AMO500", HoatChat = "Amoxicillin", QuyCach = "Hộp 10 vỉ x 10 viên", DonViTinh = "Viên", LoSX = "AX2509", HanSuDung = DateTime.Today.AddMonths(5), TonKho = 8, TonToiThieu = 20, NhaCungCap = "Dược Hồng Hà", Gia = 2500 });
            _all.Add(new Thuoc { TenThuoc = "Vitamin C 1000mg", MaThuoc = "C1000", HoatChat = "Ascorbic acid", QuyCach = "Hộp 10 ống", DonViTinh = "Ống", LoSX = "VC1009", HanSuDung = DateTime.Today.AddMonths(2), TonKho = 12, TonToiThieu = 15, NhaCungCap = "Thiên Y", Gia = 9000 });
            _all.Add(new Thuoc { TenThuoc = "Cefuroxime 250mg", MaThuoc = "CFX250", HoatChat = "Cefuroxime", QuyCach = "Hộp 2 vỉ x 10 viên", DonViTinh = "Viên", LoSX = "CF0910", HanSuDung = DateTime.Today.AddMonths(30), TonKho = 120, TonToiThieu = 25, NhaCungCap = "Dược Á Châu", Gia = 5400 });

            cbNcc.Items.Clear(); cbNcc.Items.Add("(Tất cả)");
            foreach (var n in _all.Select(x => x.NhaCungCap).Distinct().OrderBy(x => x)) cbNcc.Items.Add(n);
            cbNcc.SelectedIndex = 0;

            dtFrom.Value = DateTime.Today.AddMonths(-1);
            dtTo.Value = DateTime.Today.AddYears(3);
        }

        // ========= Filter =========
        private void ApplyFilter()
        {
            string kw = txtSearch.Text.Trim().ToLowerInvariant();
            string ncc = cbNcc.SelectedIndex <= 0 ? "" : cbNcc.SelectedItem?.ToString() ?? "";
            var from = dtFrom.Value.Date; var to = dtTo.Value.Date;
            int stockMax = (int)nudStock.Value;

            IEnumerable<Thuoc> q = _all;
            if (from <= to) q = q.Where(x => x.HanSuDung.Date >= from && x.HanSuDung.Date <= to);
            if (!string.IsNullOrEmpty(ncc)) q = q.Where(x => x.NhaCungCap == ncc);
            if (chkLow.Checked) q = q.Where(x => x.DuoiToiThieu);
            if (stockMax > 0) q = q.Where(x => x.TonKho <= stockMax);

            if (!string.IsNullOrEmpty(kw))
                q = q.Where(x => x.TenThuoc.ToLower().Contains(kw) ||
                                 x.MaThuoc.ToLower().Contains(kw) ||
                                 x.HoatChat.ToLower().Contains(kw) ||
                                 x.QuyCach.ToLower().Contains(kw) ||
                                 x.DonViTinh.ToLower().Contains(kw) ||
                                 x.LoSX.ToLower().Contains(kw) ||
                                 x.NhaCungCap.ToLower().Contains(kw));

            _view.Clear(); foreach (var it in q) _view.Add(it);
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

        // ========= CRUD =========
        private void AddThuoc()
        {
            var f = new FormThuocPro();
            if (f.ShowDialog(this) == DialogResult.OK)
            {
                _all.Add(f.Value);
                if (!cbNcc.Items.Cast<object>().Any(x => x.ToString() == f.Value.NhaCungCap))
                    cbNcc.Items.Add(f.Value.NhaCungCap);
                ApplyFilter();
            }
        }
        private void EditThuoc(Thuoc item)
        {
            var f = new FormThuocPro(item);
            if (f.ShowDialog(this) == DialogResult.OK) ApplyFilter();
        }

        // ========= INVENTORY COUNT =========
        private void OpenInventoryCount()
        {
            var f = new FormInventoryCount(_all.ToList());
            if (f.ShowDialog(this) == DialogResult.OK)
            {
                foreach (var rs in f.Results)
                {
                    var t = _all.FirstOrDefault(x => x.MaThuoc == rs.MaThuoc);
                    if (t != null) t.TonKho = rs.ThucTe;
                }
                ApplyFilter();

                if (f.ImportCount > 0 || f.ExportCount > 0)
                {
                    MessageBox.Show(
                        $"Đã duyệt phiếu kiểm kê:\n" +
                        (f.ImportCount > 0 ? $"- Tạo {f.ImportCount} dòng phiếu nhập điều chỉnh (NKADJ{DateTime.Now:yyMMddHHmm}).\n" : "") +
                        (f.ExportCount > 0 ? $"- Tạo {f.ExportCount} dòng phiếu xuất điều chỉnh (XKADJ{DateTime.Now:yyMMddHHmm})." : ""),
                        "Hoàn tất kiểm kê", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }

    // ====== Form thêm/sửa thuốc (PRO – nút to, đẹp) ======
    public class FormThuocPro : Form
    {
        public FormWarehouse.Thuoc Value { get; private set; }
        private readonly bool _isEdit;

        TextBox txtTen = new(), txtMa = new(), txtHoat = new(), txtQuy = new(), txtLo = new(), txtNCC = new();
        ComboBox cbDVT = new();
        DateTimePicker dtHSD = new();
        NumericUpDown nudTon = new(), nudGia = new();

        public FormThuocPro(FormWarehouse.Thuoc? edit = null)
        {
            _isEdit = edit != null;
            Text = _isEdit ? "Chỉnh sửa thuốc" : "Thêm thuốc";
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(600, 520);
            Size = new Size(640, 560);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;
            Font = new Font("Segoe UI", 9F);

            // Placeholders
            txtTen.PlaceholderText = "Ví dụ: Paracetamol 500mg";
            txtMa.PlaceholderText = "Ví dụ: PCM500";
            txtHoat.PlaceholderText = "Ví dụ: Paracetamol";
            txtQuy.PlaceholderText = "Ví dụ: Hộp 10 vỉ x 10 viên";
            txtLo.PlaceholderText = "Ví dụ: L2408";
            txtNCC.PlaceholderText = "Ví dụ: Dược Á Châu";

            // Đơn vị tính
            cbDVT.DropDownStyle = ComboBoxStyle.DropDownList;
            cbDVT.Items.AddRange(new object[] { "Viên", "Ống", "Gói", "Lọ", "Tuýp", "Chai", "Hộp", "Vỉ", "Khác" });
            if (cbDVT.Items.Count > 0) cbDVT.SelectedIndex = 0;

            // HSD
            dtHSD.Format = DateTimePickerFormat.Custom;
            dtHSD.CustomFormat = "dd/MM/yyyy";

            // NumericUpDown
            void StyleNum(NumericUpDown n, int dec, decimal inc, decimal max)
            { n.DecimalPlaces = dec; n.ThousandsSeparator = true; n.Increment = inc; n.Maximum = max; n.Minimum = 0; n.TextAlign = HorizontalAlignment.Right; n.Width = 200; }
            StyleNum(nudTon, 0, 1, 1_000_000);
            StyleNum(nudGia, 0, 100, 1_000_000_000);

            // Layout
            var table = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(18), ColumnCount = 2 };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            Controls.Add(table);

            void Row(string label, Control c)
            {
                var lb = new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 8, 0, 4) };
                c.Anchor = AnchorStyles.Left | AnchorStyles.Right; c.Margin = new Padding(0, 4, 0, 4);
                table.Controls.Add(lb); table.Controls.Add(c);
            }
            Row("Tên thuốc:", txtTen);
            Row("Mã thuốc:", txtMa);
            Row("Hoạt chất:", txtHoat);
            Row("Quy cách đóng gói:", txtQuy);
            Row("Đơn vị tính:", cbDVT);
            Row("Lô sản xuất:", txtLo);
            Row("Hạn sử dụng:", dtHSD);
            Row("Tồn kho:", nudTon);
            Row("Nhà cung cấp:", txtNCC);
            Row("Giá:", nudGia);

            // Footer buttons – to, rõ
            var footer = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Height = 60, Padding = new Padding(16, 12, 16, 12) };
            var btnOK = new Button { Text = _isEdit ? "Lưu" : "Thêm", Width = 140, Height = 36, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(47, 111, 237), ForeColor = Color.White };
            var btnCancel = new Button { Text = "Hủy", Width = 120, Height = 36, FlatStyle = FlatStyle.Flat };
            btnOK.FlatAppearance.BorderSize = 0; btnCancel.FlatAppearance.BorderColor = Color.Silver;
            btnOK.Click += (_, __) => { if (TryCollect(out var t)) { Value = t; DialogResult = DialogResult.OK; } };
            btnCancel.Click += (_, __) => DialogResult = DialogResult.Cancel;
            footer.Controls.AddRange(new Control[] { btnOK, btnCancel });
            Controls.Add(footer);
            AcceptButton = btnOK; CancelButton = btnCancel;

            // Bind edit
            if (_isEdit)
            {
                txtTen.Text = edit!.TenThuoc; txtMa.Text = edit.MaThuoc; txtHoat.Text = edit.HoatChat;
                txtQuy.Text = edit.QuyCach; cbDVT.SelectedItem = cbDVT.Items.Cast<object>().FirstOrDefault(x => x.ToString() == edit.DonViTinh) ?? cbDVT.Items[0];
                txtLo.Text = edit.LoSX; dtHSD.Value = edit.HanSuDung == default ? DateTime.Today : edit.HanSuDung;
                nudTon.Value = Math.Max(0, edit.TonKho); 
                txtNCC.Text = edit.NhaCungCap; nudGia.Value = Math.Max(0, edit.Gia);
                Value = edit;
            }
            else
            {
                dtHSD.Value = DateTime.Today.AddYears(1);
                Value = new FormWarehouse.Thuoc();
            }
        }

        private bool TryCollect(out FormWarehouse.Thuoc t)
        {
            t = Value;
            if (string.IsNullOrWhiteSpace(txtTen.Text))
            { MessageBox.Show("Vui lòng nhập Tên thuốc.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }

            t.TenThuoc = txtTen.Text.Trim();
            t.MaThuoc = txtMa.Text.Trim();
            t.HoatChat = txtHoat.Text.Trim();
            t.QuyCach = txtQuy.Text.Trim();
            t.DonViTinh = cbDVT.Text.Trim();
            t.LoSX = txtLo.Text.Trim();
            t.HanSuDung = dtHSD.Value.Date;
            t.TonKho = (int)nudTon.Value;
            t.NhaCungCap = txtNCC.Text.Trim();
            t.Gia = nudGia.Value;
            return true;
        }
    }

    // ====== Phiếu kiểm kê ======
    public class FormInventoryCount : Form
    {
        public class Line
        {
            public string TenThuoc { get; set; } = "";
            public string MaThuoc { get; set; } = "";
            public int HeThong { get; set; }
            public int ThucTe { get; set; }
            public int ChenhLech => ThucTe - HeThong;
            public string TrangThai => ChenhLech > 0 ? "Thừa" : (ChenhLech < 0 ? "Thiếu" : "Đủ");
        }

        public int ImportCount { get; private set; }
        public int ExportCount { get; private set; }
        public List<Line> Results { get; } = new();

        private readonly BindingList<Line> _rows = new();
        private readonly TextBox txtNguoiDem = new() { Width = 240, PlaceholderText = "Nhập tên người đếm" };
        private readonly DateTimePicker dtKiem = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm" };
        private readonly DataGridView grid = new() { Dock = DockStyle.Fill, AutoGenerateColumns = false, AllowUserToAddRows = false };

        public FormInventoryCount(List<FormWarehouse.Thuoc> items)
        {
            Text = "Phiếu kiểm kê";
            Size = new Size(900, 560);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;

            var header = new TableLayoutPanel { Dock = DockStyle.Top, Height = 60, ColumnCount = 4, Padding = new Padding(12) };
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            header.Controls.Add(new Label { Text = "Người đếm:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
            header.Controls.Add(txtNguoiDem, 1, 0);
            header.Controls.Add(new Label { Text = "Ngày/giờ:", AutoSize = true, Anchor = AnchorStyles.Left }, 2, 0);
            header.Controls.Add(dtKiem, 3, 0);

            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            grid.ColumnHeadersVisible = true;
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tên thuốc", DataPropertyName = nameof(Line.TenThuoc), Width = 220, ReadOnly = true });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mã", DataPropertyName = nameof(Line.MaThuoc), Width = 100, ReadOnly = true });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tồn hệ thống", DataPropertyName = nameof(Line.HeThong), Width = 110, ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
            var colThucTe = new DataGridViewTextBoxColumn { HeaderText = "Thực tế (nhập)", DataPropertyName = nameof(Line.ThucTe), Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } };
            grid.Columns.Add(colThucTe);
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Chênh lệch", DataPropertyName = nameof(Line.ChenhLech), Width = 100, ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Trạng thái", DataPropertyName = nameof(Line.TrangThai), Width = 90, ReadOnly = true });
            grid.DataSource = _rows;

            var footer = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Height = 60, Padding = new Padding(12) };
            var btnApprove = new Button { Text = "Duyệt phiếu", Width = 120, Height = 36, BackColor = Color.FromArgb(47, 111, 237), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            var btnCancel = new Button { Text = "Hủy", Width = 100, Height = 36, FlatStyle = FlatStyle.Flat };
            footer.Controls.Add(btnApprove); footer.Controls.Add(btnCancel);

            Controls.Add(grid); Controls.Add(footer); Controls.Add(header);

            foreach (var t in items)
                _rows.Add(new Line { TenThuoc = t.TenThuoc, MaThuoc = t.MaThuoc, HeThong = t.TonKho, ThucTe = t.TonKho });

            grid.CellEndEdit += (_, __) => grid.Refresh();
            btnCancel.Click += (_, __) => DialogResult = DialogResult.Cancel;
            btnApprove.Click += (_, __) => Approve();
        }

        private void Approve()
        {
            if (string.IsNullOrWhiteSpace(txtNguoiDem.Text))
            { MessageBox.Show("Hãy nhập tên người đếm."); return; }

            Results.Clear(); ImportCount = ExportCount = 0;
            foreach (var r in _rows)
            {
                Results.Add(new Line { TenThuoc = r.TenThuoc, MaThuoc = r.MaThuoc, HeThong = r.HeThong, ThucTe = r.ThucTe });
                if (r.ChenhLech > 0) ImportCount++;
                else if (r.ChenhLech < 0) ExportCount++;
            }

            var msg = $"Người đếm: {txtNguoiDem.Text}\nThời điểm: {dtKiem.Value:dd/MM/yyyy HH:mm}\n" +
                      $"Dòng thừa: {ImportCount}, dòng thiếu: {ExportCount}.\n\nXác nhận duyệt phiếu và tạo phiếu điều chỉnh?";
            if (MessageBox.Show(msg, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                DialogResult = DialogResult.OK;
        }
    }
}
