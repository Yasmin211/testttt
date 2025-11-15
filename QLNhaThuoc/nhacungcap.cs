using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace QLNhaThuoc
{
    public partial class nhacungcap : DevExpress.XtraEditors.XtraForm
    {
        public nhacungcap()
        {
            InitializeComponent();
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
            flpncc.Top = (pnlncc.ClientSize.Height - flpncc.Height) / 2;
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


        private void nhacungcap_Load(object sender, EventArgs e)
        {
            
            
            pnlncc.Controls.Add(flpncc);
            pnlncc.BackColor = Color.White;
            txttimkiem.Properties.AutoHeight = false;
            txttimkiem.Height = btnloctheo.Height; // Đặt chiều cao của txttimkiem bằng btnloc
            btnloc.Height = btnloctheo.Height;
            pnlncc.Resize += (s, ev) =>
            {
                CenterFlowLayoutPanel();
                CenterControlsVerticallyInFlowLayoutPanel(flpncc);
            };

            flpncc.Resize += (s, ev) =>
            {
                CenterControlsVerticallyInFlowLayoutPanel(flpncc);
            };

            // Căn ngay lúc khởi tạo
            CenterFlowLayoutPanel();
            CenterControlsVerticallyInFlowLayoutPanel(flpncc);
            //
            // nút thêm mới
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
           
            //
            txttimkiem.Properties.NullValuePrompt = "Nhập mã nhà cung cấp và nhấn enter để tìm kiếm";
            txttimkiem.Properties.NullValuePromptShowForEmptyValue = true;
            txttimkiem.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            txttimkiem.BackColor = Color.White;
            BoGocVaVien(txttimkiem, 12, Color.DarkGray, 1);
            //
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
            btnloctheo.LookAndFeel.UseDefaultLookAndFeel = false;
            btnloctheo.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            BoGocVaVien(btnloctheo, 12, Color.DarkGray, 1);
            //
            //Cấu hình chọn dòng bằng checkbox-- -
            gridViewncc.OptionsSelection.MultiSelect = true;
            gridViewncc.OptionsSelection.MultiSelectMode =
                DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;

            // Hiển thị ô vuông ở header để chọn tất cả
            gridViewncc.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.True;

            // Ẩn cột indicator (số thứ tự dòng) để checkbox ra ngoài cùng
            gridViewncc.OptionsView.ShowIndicator = false;

            // --- Cấu hình giao diện ---
            gridViewncc.OptionsView.ShowGroupPanel = false;
            gridViewncc.OptionsView.EnableAppearanceEvenRow = true;
            gridViewncc.Appearance.HeaderPanel.Font = new Font("Arial", 10, FontStyle.Bold);
            gridViewncc.Appearance.Row.Font = new Font("Arial", 10);
            gridViewncc.RowHeight = 28;
            gridViewncc.OptionsView.ShowIndicator = false;
            // 
            BuildPopupContent();

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
                Location = new Point((popuploctheo.Width - 210) / 2, 10)
            };
            lblTitle.Appearance.Font = new Font("Arial", 12, FontStyle.Bold);
            lblTitle.Width = 250;
            popuploctheo.Controls.Add(lblTitle);
            // Danh sách tiêu chí lọc của quản lý nhập hàng(có thể chỉnh lại theo phần của bạn)
            string[] options =
            {

             "Mã nhà cung cấp",
             "Tên nhà cung cấp",
             "Khu vực"
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
                if (text == "Mã nhà cung cấp")
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

        private void btnloctheo_Click(object sender, EventArgs e)
        {
            int offsetY = 80; // 👈 chỉnh giá trị này để dịch popup xuống bao nhiêu pixel tùy ý (10–30 là đẹp)
            int offsetX = 6;  // nếu muốn dịch ngang thì đổi giá trị này

            pceloctheo.Location = new Point(btnloctheo.Left + offsetX, btnloctheo.Bottom + offsetY);
            pceloctheo.ShowPopup();
        }

        private void btnloc_Click(object sender, EventArgs e)
        {

        }

        private void btnthem_Click(object sender, EventArgs e)
        {
            chitietncc chitietncc = new chitietncc();
            chitietncc.ShowDialog();
        }

        private void btnsua_Click(object sender, EventArgs e)
        {
            chitietncc chitietncc = new chitietncc();
            chitietncc.ShowDialog();
        }
    }
}
