using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using System;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace QLNhaThuoc
{
    public partial class nhaphang : XtraForm
    {
        SimpleButton btnApply;
        SimpleButton btnCancel;
        SimpleButton btnMacDinh;
        SimpleButton btnApDung;
        private Timer timer;

        public nhaphang()
        {
            InitializeComponent();
            this.Load += nhaphang_Load;


        }

        // Tạo GraphicsPath bo góc
        private GraphicsPath TaoPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        // Bo góc + viền cho control chuẩn WinForms
        private void BoGocVaVien(Control control, int radius, Color borderColor, int borderSize)
        {
            control.Resize += (s, e) =>
            {
                if (control.Width > 0 && control.Height > 0)
                    control.Region = new Region(TaoPath(control.ClientRectangle, radius));
            };

            control.Paint += (s, e) =>
            {
                if (control.Width <= 0 || control.Height <= 0) return;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle rect = control.ClientRectangle;
                rect.Width -= 1;
                rect.Height -= 1;

                using (SolidBrush brush = new SolidBrush(control.BackColor))
                    e.Graphics.FillPath(brush, TaoPath(rect, radius));

                if (borderSize > 0)
                    using (Pen pen = new Pen(borderColor, borderSize))
                        e.Graphics.DrawPath(pen, TaoPath(rect, radius));
            };
        }

        // Căn giữa FlowLayoutPanel
        private void CenterFlowLayoutPanel()
        {
            // căn giữa theo chiều dọc
            flpnhaphang.Top = (pnlnhaphang.ClientSize.Height - flpnhaphang.Height) / 2;
        }
        private void CenterControlsVerticallyInFlowLayoutPanel(FlowLayoutPanel flp)
        {
            foreach (Control ctrl in flp.Controls)
            {
                // Nếu control có chiều cao nhỏ hơn FlowLayoutPanel thì căn giữa nó
                int topMargin = Math.Max(0, (flp.ClientSize.Height - ctrl.Height) / 2);

                // Giữ nguyên margin trái–phải, chỉnh lại top–bottom
                ctrl.Margin = new Padding(ctrl.Margin.Left, topMargin, ctrl.Margin.Right, topMargin);
            }
        }

        private void BtnCustom_Paint(object sender, PaintEventArgs e)
        {
            Button btn = sender as Button;
            int radius = 20; // bán kính bo góc
            Rectangle rect = new Rectangle(0, 0, btn.Width, btn.Height);

            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseAllFigures();

            btn.Region = new Region(path);

            // Vẽ chữ chính giữa (tùy chọn, thường Button tự canh giữa)
            TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, rect, btn.ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }



        private void nhaphang_Load(object sender, EventArgs e)
        {
            txttimkiem.Properties.AutoHeight = false;
            txttimkiem.Height = btnloctheo.Height; // Đặt chiều cao của txttimkiem bằng btnloc
            btnloc.Height = btnloctheo.Height;
            pnlnhaphang.Resize += (s, ev) =>
            {
                CenterFlowLayoutPanel();
                CenterControlsVerticallyInFlowLayoutPanel(flpnhaphang);
            };

            flpnhaphang.Resize += (s, ev) =>
            {
                CenterControlsVerticallyInFlowLayoutPanel(flpnhaphang);
            };

            // Căn ngay lúc khởi tạo
            CenterFlowLayoutPanel();
            CenterControlsVerticallyInFlowLayoutPanel(flpnhaphang);

            lblthoigian.Text = $"Ngày HĐ: {DateTime.Today:dd/MM/yyyy}";
            lblthoigian.Margin = new Padding(0, 19, 270, 20);
            btntailai.Margin = new Padding(0, 9, 15, 0);
            btnxuat.Margin = new Padding(0, 9, 15, 0);
            btncaidat.Margin = new Padding(0, 9, 0, 0);
            btnloc.Margin = new Padding(5, 10, 20, 0);

            //nút thêm mới
            btnthem.FlatStyle = FlatStyle.Flat;
            btnthem.FlatAppearance.BorderSize = 0;
            btnthem.Margin = new Padding(10);
            //màu mặc định
            btnthem.BackColor = Color.FromArgb(66, 144, 242);
            btnthem.ForeColor = Color.White;
            //màu khi di chuột
            btnthem.MouseEnter += (s, e) =>
            {
                btnthem.BackColor = Color.FromArgb(118, 173, 243);
            };

            btnthem.MouseLeave += (s, e) =>
            {
                btnthem.BackColor = Color.FromArgb(66, 144, 242);
            };
            //màu khi nhấn
            btnthem.MouseDown += (s, e) =>
            {
                btnthem.BackColor = Color.FromArgb(40, 116, 240);
            };
            btnthem.MouseUp += (s, e) =>
            {
                btnthem.BackColor = Color.FromArgb(118, 173, 243);
            };
           
            //nút sửa
            btnsua.FlatStyle = FlatStyle.Flat;
            btnsua.FlatAppearance.BorderSize = 0;
            btnsua.Margin = new Padding(10);
            //màu mặc định
            btnsua.BackColor = Color.FromArgb(66, 144, 242);
            btnsua.ForeColor = Color.White;
            //màu khi di chuột
            btnsua.MouseEnter += (s, e) =>
            {
                btnsua.BackColor = Color.FromArgb(118, 173, 243);
            };

            btnsua.MouseLeave += (s, e) =>
            {
                btnsua.BackColor = Color.FromArgb(66, 144, 242);
            };
            //màu khi nhấn
            btnsua.MouseDown += (s, e) =>
            {
                btnsua.BackColor = Color.FromArgb(40, 116, 240);
            };
            btnsua.MouseUp += (s, e) =>
            {
                btnsua.BackColor = Color.FromArgb(118, 173, 243);
            };
            
            //nút xóa
            btnxoa.FlatStyle = FlatStyle.Flat;
            btnxoa.FlatAppearance.BorderSize = 0;
            btnxoa.Margin = new Padding(10);
            //màu mặc định
            btnxoa.BackColor = Color.FromArgb(66, 144, 242);
            btnxoa.ForeColor = Color.White;
            //màu khi di chuột
            btnxoa.MouseEnter += (s, e) =>
            {
                btnxoa.BackColor = Color.FromArgb(118, 173, 243);
            };
            btnxoa.MouseLeave += (s, e) =>
            {
                btnxoa.BackColor = Color.FromArgb(66, 144, 242);
            };
            //màu khi nhấn
            btnxoa.MouseDown += (s, e) =>
            {
                btnxoa.BackColor = Color.FromArgb(40, 116, 240);
            };
            btnxoa.MouseUp += (s, e) =>
            {
                btnxoa.BackColor = Color.FromArgb(118, 173, 243);
            };
           
            // === Label Header ===
            


            // === TextEdit Tìm Kiếm ===
            txttimkiem.Properties.NullValuePrompt = "Nhập số hóa đơn và nhấn enter để tìm kiếm";
            txttimkiem.Properties.NullValuePromptShowForEmptyValue = true;
            txttimkiem.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            txttimkiem.BackColor = Color.White;
            BoGocVaVien(txttimkiem, 12, Color.DarkGray, 1);

            // === Panel nền ===
            pnlnhaphang.Appearance.BackColor = Color.White;
            pnlnhaphang.Appearance.Options.UseBackColor = true;

            // === Button Lọc ===
            btnloc.LookAndFeel.UseDefaultLookAndFeel = false;
            btnloc.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            btnloc.Text = "Lọc";
            btnloc.AllowFocus = false;

            btnloc.ImageOptions.SvgImage = Properties.Resources.filter;
            btnloc.ImageOptions.SvgImageSize = new Size(12, 12);
            btnloc.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            btnloc.ImageOptions.ImageToTextIndent = 10;

            btnloc.Appearance.BackColor = Color.White;
            btnloc.Appearance.ForeColor = Color.Black;
            btnloc.Appearance.Options.UseBackColor = true;
            btnloc.Appearance.Options.UseForeColor = true;
            //set hiệu ứng chuột
            //màu khi di chuột
            btnloc.MouseEnter += (s, e) =>
            {
                btnloc.Appearance.BackColor = Color.FromArgb(240, 240, 240);

            };
            btnloc.MouseLeave += (s, e) =>
            {
                btnloc.Appearance.BackColor = Color.White;
            };
            //màu khi nhấn
            btnloc.MouseDown += (s, e) =>
            {
                btnloc.Appearance.BackColor = Color.FromArgb(220, 220, 220);
            };
            btnloc.MouseUp += (s, e) =>
            {
                btnloc.Appearance.BackColor = Color.FromArgb(240, 240, 240);
            };

            //btn tải lại
            btntailai.AllowFocus = false;
            btntailai.ImageOptions.SvgImage = Properties.Resources.refresh;
            btntailai.ImageOptions.SvgImageSize = new Size(12, 12);
            btntailai.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.None;
            btntailai.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            btntailai.Text = "";
            //màu khi di chuột
            btntailai.MouseEnter += (s, e) =>
            {
                btntailai.Appearance.BackColor = Color.FromArgb(229, 238, 238);
            };
            btntailai.MouseLeave += (s, e) =>
            {
                btntailai.Appearance.BackColor = Color.White;
            };
            //màu khi nhấn
            btntailai.MouseDown += (s, e) =>
            {
                btntailai.Appearance.BackColor = Color.FromArgb(229, 238, 238);
            };
            btntailai.MouseUp += (s, e) =>
            {
                btntailai.Appearance.BackColor = Color.FromArgb(240, 240, 240);
            };
            //nút xuất
            btnxuat.AllowFocus = false;
            btnxuat.ImageOptions.SvgImage = Properties.Resources.excel;
            btnxuat.ImageOptions.SvgImageSize = new Size(12, 12);
            btnxuat.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.None;
            btnxuat.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            btnxuat.Text = "";
            //màu khi di chuột
            btnxuat.MouseEnter += (s, e) =>
            {
                btnxuat.Appearance.BackColor = Color.FromArgb(229, 238, 238);
            };
            btnxuat.MouseLeave += (s, e) =>
            {
                btnxuat.Appearance.BackColor = Color.White;
            };
            //màu khi nhấn
            btnxuat.MouseDown += (s, e) =>
            {
                btnxuat.Appearance.BackColor = Color.FromArgb(229, 238, 238);
            };
            btnxuat.MouseUp += (s, e) =>
            {
                btnxuat.Appearance.BackColor = Color.FromArgb(240, 240, 240);
            };
            //nút cài đặt
            btncaidat.AllowFocus = false;
            btncaidat.ImageOptions.SvgImage = Properties.Resources.settings;
            btncaidat.ImageOptions.SvgImageSize = new Size(12, 12);
            btncaidat.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.None;
            btncaidat.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            btncaidat.Text = "";
            //màu khi di chuột
            btncaidat.MouseEnter += (s, e) =>
            {
                btncaidat.Appearance.BackColor = Color.FromArgb(229, 238, 238);
            };
            btncaidat.MouseLeave += (s, e) =>
            {
                btncaidat.Appearance.BackColor = Color.White;
            };
            //màu khi nhấn
            btncaidat.MouseDown += (s, e) =>
            {
                btncaidat.Appearance.BackColor = Color.FromArgb(229, 238, 238);
            };
            btncaidat.MouseUp += (s, e) =>
            {
                btncaidat.Appearance.BackColor = Color.FromArgb(240, 240, 240);
            };
            //btnloctheo
            btnloctheo.AllowFocus = false;
            btnloctheo.ImageOptions.SvgImage = Properties.Resources.settings;
            btnloctheo.ImageOptions.SvgImageSize = new Size(12, 12);
            btnloctheo.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.None;
            btnloctheo.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            btnloctheo.Text = "";
            //màu khi di chuột
            btnloctheo.MouseEnter += (s, e) =>
            {
                btnloctheo.Appearance.BackColor = Color.FromArgb(240, 240, 240);
            };
            btnloctheo.MouseLeave += (s, e) =>
            {
                btnloctheo.Appearance.BackColor = Color.White;
            };
            //màu khi nhấn
            btnloctheo.MouseDown += (s, e) =>
            {
                btnloctheo.Appearance.BackColor = Color.FromArgb(220, 220, 220);
            };
            btnloctheo.MouseUp += (s, e) =>
            {
                btnloctheo.Appearance.BackColor = Color.FromArgb(240, 240, 240);
            };

            //bo góc + viền
            BoGocVaVien(btnloc, 12, Color.DarkGray, 1);
            btntailai.LookAndFeel.UseDefaultLookAndFeel = false;
            btntailai.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            BoGocVaVien(btntailai, 12, Color.DarkGray, 1);
            btnxuat.LookAndFeel.UseDefaultLookAndFeel = false;
            btnxuat.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            BoGocVaVien(btnxuat, 12, Color.DarkGray, 1);
            btncaidat.LookAndFeel.UseDefaultLookAndFeel = false;
            btncaidat.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            BoGocVaVien(btncaidat, 12, Color.DarkGray, 1);
            btnloctheo.LookAndFeel.UseDefaultLookAndFeel = false;
            btnloctheo.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            BoGocVaVien(btnloctheo, 12, Color.DarkGray, 1);

            //Cấu hình chọn dòng bằng checkbox-- -
            gridViewnhaphang.OptionsSelection.MultiSelect = true;
            gridViewnhaphang.OptionsSelection.MultiSelectMode =
                DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;

            // Hiển thị ô vuông ở header để chọn tất cả
            gridViewnhaphang.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.True;

            // Ẩn cột indicator (số thứ tự dòng) để checkbox ra ngoài cùng
            gridViewnhaphang.OptionsView.ShowIndicator = false;

            // --- Cấu hình giao diện ---
            gridViewnhaphang.OptionsView.ShowGroupPanel = false;
            gridViewnhaphang.OptionsView.EnableAppearanceEvenRow = true;
            gridViewnhaphang.Appearance.HeaderPanel.Font = new Font("Arial", 10, FontStyle.Bold);
            gridViewnhaphang.Appearance.Row.Font = new Font("Arial", 10);
            gridViewnhaphang.RowHeight = 28;
            gridViewnhaphang.OptionsView.ShowIndicator = false;
            //popuploctheo
            BuildPopupContent();
            TaoPopupLocTheo();
            ApplyPopupSelection();
            popuploctheo.BackColor = Color.White;
            popuplocchitiet.BackColor = Color.White;
            
        }

        private void btntailai_Click(object sender, EventArgs e)
        {

        }

        private void btncaidat_Click(object sender, EventArgs e)
        {
            DXPopupMenu menu = new DXPopupMenu();

            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gridViewnhaphang.Columns)
            {
                DXMenuCheckItem item = new DXMenuCheckItem(col.Caption, col.Visible, null, (o, args) =>
                {
                    col.Visible = !col.Visible;
                });

                menu.Items.Add(item);
            }
            if (menu.Items.Count == 0)
            {
                XtraMessageBox.Show("Không có cột nào để hiển thị!");
                return;
            }
            // Hiển thị menu ngay dưới nút
            menu.ShowPopup(btncaidat, new Point(0, btncaidat.Height));

        }

        private void BuildPopupContent()
        {
            // Xóa nội dung cũ (an toàn để gọi lại)
            popuploctheo.Controls.Clear();

            // Title
            var lblTitle = new LabelControl
            {
                Text = "Tìm kiếm theo",
                AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None,
                Location = new Point((popuploctheo.Width - 240) / 2, 10)
            };
            lblTitle.Appearance.Font = new Font("Arial", 12, FontStyle.Bold);
            lblTitle.Width = 250;
            popuploctheo.Controls.Add(lblTitle);

            // Danh sách tiêu chí lọc của quản lý nhập hàng (có thể chỉnh lại theo phần của bạn)
            string[] options =
            {
        "Số hóa đơn",
        "Mã phiếu nhập",
        "Nhà cung cấp",
        "Ngày nhập",
        "Nhân viên nhập",
        "Trạng thái phiếu"
    };

            int y = 45;
            foreach (var text in options)
            {
                CheckEdit chk = new CheckEdit
                {
                    Text = text,
                    Location = new Point(12, y),
                    Width = 220
                };
                chk.Properties.Appearance.Font = new Font("Arial", 8, FontStyle.Regular);
                chk.Properties.Appearance.Options.UseFont = true;

                // Tick mặc định
                if (text == "Số hóa đơn")
                    chk.Checked = true;

                // Khi thay đổi trạng thái CheckEdit thì gọi lọc ngay
                chk.CheckedChanged += (s, e) =>
                {
                    // Bỏ tick ở tất cả các CheckEdit khác để chỉ chọn 1 tiêu chí
                    foreach (var control in popuploctheo.Controls.OfType<CheckEdit>())
                    {
                        if (control != chk)
                            control.Checked = false;
                    }

                    // Gọi hàm lọc theo tiêu chí này
                    ApplyFilterByCriteria(chk.Text);
                };

                popuploctheo.Controls.Add(chk);
                y += 30;
            }

            // Đường kẻ nhẹ ngăn cách
            Panel line = new Panel
            {
                BackColor = Color.FromArgb(230, 230, 230),
                Height = 1,
                Width = 240,
                Location = new Point(10, y - 6)
            };
            popuploctheo.Controls.Add(line);

            // Set kích thước tổng thể popup (vì không có nút nên bớt chiều cao)
            popuploctheo.Size = new Size(270, y + 20);
        }
        private void ApplyFilterByCriteria(string criteria)
        {
            
            }
           
        

        private void ApplyPopupSelection()
        {
            // Lấy các tiêu chí được chọn
            var selectedOptions = new System.Collections.Generic.List<string>();
            foreach (Control ctrl in popuploctheo.Controls)
            {
                if (ctrl is CheckEdit chk && chk.Checked)
                {
                    selectedOptions.Add(chk.Text);
                }
            }
            // Hiển thị tiêu chí đã chọn trên nút
            if (selectedOptions.Count > 0)
            {
                btnloctheo.Text = string.Join(", ", selectedOptions);
            }
            else
            {
                btnloctheo.Text = "Lọc theo";
            }
        }

        private void btnloctheo_Click(object sender, EventArgs e)
        {
            int offsetY = 80; // 👈 chỉnh giá trị này để dịch popup xuống bao nhiêu pixel tùy ý (10–30 là đẹp)
            int offsetX = 20;  // nếu muốn dịch ngang thì đổi giá trị này

            pceloctheo.Location = new Point(btnloctheo.Left + offsetX, btnloctheo.Bottom + offsetY);
            pceloctheo.ShowPopup();
        }

        private void btnloc_Click(object sender, EventArgs e)
        {
            int offsetY = 80; // 👈 chỉnh giá trị này để dịch popup xuống bao nhiêu pixel tùy ý (10–30 là đẹp)
            int offsetX = 20;  // nếu muốn dịch ngang thì đổi giá trị này

            pcelocchitiet.Location = new Point(btnloc.Left + offsetX, btnloc.Bottom + offsetY);
            pcelocchitiet.ShowPopup();

        }
        private void TaoPopupLocTheo()
        {
            if (popuplocchitiet == null)
            {
                popuplocchitiet = new PopupContainerControl()
                {
                    Size = new Size(500, 350),
                    BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple,
                    BackColor = Color.White
                };
            }
            else
            {
                popuplocchitiet.Controls.Clear();
            }

            // === Title ===
            var lbltitle = new LabelControl
            {
                Text = "Lọc theo",
                AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None,
                Location = new Point(0, 10),
                Width = popuplocchitiet.Width,
                Appearance = { Font = new Font("Arial", 12, FontStyle.Bold), TextOptions = { HAlignment = DevExpress.Utils.HorzAlignment.Center } }
            };
            popuplocchitiet.Controls.Add(lbltitle);

            // === Dòng 1: Kỳ, Từ ngày, Đến ngày ===
            LabelControl lblKy = new LabelControl() { Text = "Kỳ:", Location = new Point(20, 60) };
            ComboBoxEdit cboKy = new ComboBoxEdit() { Location = new Point(60, 55), Width = 100 };
            cboKy.Properties.Items.AddRange(new[] { "Hôm nay", "Tuần này", "Tháng này", "Tùy chọn" });

            LabelControl lblTuNgay = new LabelControl() { Text = "Từ ngày:", Location = new Point(170, 60) };
            DateEdit dateTu = new DateEdit() { Location = new Point(240, 55), Width = 100 };

            LabelControl lblDenNgay = new LabelControl() { Text = "Đến ngày:", Location = new Point(350, 60) };
            DateEdit dateDen = new DateEdit() { Location = new Point(430, 55), Width = 100 };

            // === Các dòng dưới canh thẳng cột ===
            int labelX = 20;
            int inputX = 150;
            int widthInput = 380;

            // Dòng 2: Nhà cung cấp
            LabelControl lblNCC = new LabelControl() { Text = "Nhà cung cấp:", Location = new Point(labelX, 100) };
            ComboBoxEdit cboNCC = new ComboBoxEdit() { Location = new Point(inputX, 95), Width = widthInput };

            // Dòng 3: Số hóa đơn
            LabelControl lblSoHD = new LabelControl() { Text = "Số hóa đơn:", Location = new Point(labelX, 140) };
            TextEdit txtSoHD = new TextEdit() { Location = new Point(inputX, 135), Width = widthInput };

            // Dòng 4: Nhân viên nhập
            LabelControl lblNV = new LabelControl() { Text = "Nhân viên nhập:", Location = new Point(labelX, 180) };
            ComboBoxEdit cboNV = new ComboBoxEdit() { Location = new Point(inputX, 175), Width = widthInput };

            // Dòng 5: Trạng thái phiếu
            LabelControl lblTT = new LabelControl() { Text = "Trạng thái phiếu:", Location = new Point(labelX, 220) };
            ComboBoxEdit cboTT = new ComboBoxEdit() { Location = new Point(inputX, 215), Width = widthInput };
            cboTT.Properties.Items.AddRange(new[] { "Tất cả", "Đã nhập", "Chờ duyệt", "Hủy" });

            // === Nút bấm ===
            SimpleButton btnMacDinh = new SimpleButton()
            {
                Text = "Bộ lọc mặc định",
                Location = new Point(300, 280),
                Width = 120               
            };
            btnMacDinh.LookAndFeel.UseDefaultLookAndFeel = false;
            btnMacDinh.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            BoGocVaVien(btnMacDinh, 12, Color.DarkGray, 1);
            //màu khi di chuột
            btnMacDinh.MouseEnter += (s, e) =>
            {
                btnMacDinh.Appearance.BackColor = Color.FromArgb(229, 238, 238);
            };
            btnMacDinh.MouseLeave += (s, e) =>
            {
                btnMacDinh.Appearance.BackColor = Color.White;
            };
            //màu khi nhấn
            btnMacDinh.MouseDown += (s, e) =>
            {
                btnMacDinh.Appearance.BackColor = Color.FromArgb(229, 238, 238);
            };
            btnMacDinh.MouseUp += (s, e) =>
            {
                btnMacDinh.Appearance.BackColor = Color.FromArgb(240, 240, 240);
            };
            SimpleButton btnApDung = new SimpleButton()
            {
                Text = "Áp dụng",
                Location = new Point(450, 280),
                Width = 80
            };
            btnApDung.LookAndFeel.UseDefaultLookAndFeel = false;
            btnApDung.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            BoGocVaVien(btnApDung, 12, Color.DarkGray, 1);
            //màu khi di chuột
            btnApDung.MouseEnter += (s, e) =>
            {
                btnApDung.Appearance.BackColor = Color.FromArgb(229, 238, 238);
            };
            btnApDung.MouseLeave += (s, e) =>
            {
                btnApDung.Appearance.BackColor = Color.White;
            };
            //màu khi nhấn
            btnApDung.MouseDown += (s, e) =>
            {
                btnApDung.Appearance.BackColor = Color.FromArgb(229, 238, 238);
            };
            btnApDung.MouseUp += (s, e) =>
            {
                btnApDung.Appearance.BackColor = Color.FromArgb(240, 240, 240);
            };


            // === Thêm tất cả control vào popup ===
            popuplocchitiet.Controls.AddRange(new Control[]
            {
        lbltitle,
        lblKy, cboKy, lblTuNgay, dateTu, lblDenNgay, dateDen,
        lblNCC, cboNCC,
        lblSoHD, txtSoHD,
        lblNV, cboNV,
        lblTT, cboTT,
        btnMacDinh, btnApDung
            });
        }

        private void btnthem_Click(object sender, EventArgs e)
        {
            chitietnhaphang chitietnhaphang = new chitietnhaphang();
            chitietnhaphang.ShowDialog();

        }
    }
}
    

