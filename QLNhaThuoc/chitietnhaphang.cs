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

namespace QLNhaThuoc
{
    public partial class chitietnhaphang : DevExpress.XtraEditors.XtraForm
    {
        public chitietnhaphang()
        {
            InitializeComponent();
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

        private void chitietnhaphang_Load(object sender, EventArgs e)
        {
            //nút lưu
            btnsave.FlatStyle = FlatStyle.Flat;
            btnsave.FlatAppearance.BorderSize = 0;
            btnsave.Margin = new Padding(10);
            //màu mặc định
            btnsave.BackColor = Color.FromArgb(66, 144, 242);
            btnsave.ForeColor = Color.White;
            //màu khi di chuột
            btnsave.MouseEnter += (s, e) =>
            {
                btnsave.BackColor = Color.FromArgb(118, 173, 243);
            };

            btnsave.MouseLeave += (s, e) =>
            {
                btnsave.BackColor = Color.FromArgb(66, 144, 242);
            };
            //màu khi nhấn
            btnsave.MouseDown += (s, e) =>
            {
                btnsave.BackColor = Color.FromArgb(40, 116, 240);
            };
            btnsave.MouseUp += (s, e) =>
            {
                btnsave.BackColor = Color.FromArgb(118, 173, 243);
            };
            
            btnprint.FlatStyle = FlatStyle.Flat;
            btnprint.FlatAppearance.BorderSize = 0;
            btnprint.Margin = new Padding(10);
            //màu mặc định
            btnprint.BackColor = Color.FromArgb(66, 144, 242);
            btnprint.ForeColor = Color.White;
            //màu khi di chuột
            btnprint.MouseEnter += (s, e) =>
            {
                btnprint.BackColor = Color.FromArgb(118, 173, 243);
            };

            btnprint.MouseLeave += (s, e) =>
            {
                btnprint.BackColor = Color.FromArgb(66, 144, 242);
            };
            //màu khi nhấn
            btnprint.MouseDown += (s, e) =>
            {
                btnprint.BackColor = Color.FromArgb(40, 116, 240);
            };
            btnprint.MouseUp += (s, e) =>
            {
                btnprint.BackColor = Color.FromArgb(118, 173, 243);
            };
            
            //nút hủy
            btnhuy.FlatStyle = FlatStyle.Flat;
            btnhuy.FlatAppearance.BorderSize = 0;
            btnhuy.Margin = new Padding(10);
            //màu mặc định
            btnhuy.BackColor = Color.FromArgb(66, 144, 242);
            btnhuy.ForeColor = Color.White;
            //màu khi di chuột
            btnhuy.MouseEnter += (s, e) =>
            {
                btnhuy.BackColor = Color.FromArgb(118, 173, 243);
            };
            btnhuy.MouseLeave += (s, e) =>
            {
                btnhuy.BackColor = Color.FromArgb(66, 144, 242);
            };
            //màu khi nhấn
            btnhuy.MouseDown += (s, e) =>
            {
                btnhuy.BackColor = Color.FromArgb(40, 116, 240);
            };
            btnhuy.MouseUp += (s, e) =>
            {
                btnhuy.BackColor = Color.FromArgb(118, 173, 243);
            };
           
            //chỉnh tlp
            //chỉnhr màu



        }

        private void btnhuy_Click(object sender, EventArgs e)
        {
            chitietnhaphang.ActiveForm.Close();
        }

        private void btnprint_Click(object sender, EventArgs e)
        {

        }
    }
}