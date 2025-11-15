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
    public partial class chitietnhanvien : DevExpress.XtraEditors.XtraForm
    {
        public enum Mode { ThemMoi, ChinhSua }

        public Mode FormMode { get; set; }
        public string MaNCC { get; set; }
        public chitietnhanvien(Mode mode, string maNCC = "")
        {
            InitializeComponent();
            FormMode = mode;
            MaNCC = maNCC;
        }
        public chitietnhanvien()
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
        private void chitietnhanvien_Load(object sender, EventArgs e)
        {
            if (FormMode == Mode.ThemMoi)
            {
                lblTitle.Text = "THÊM MỚI NHÂN VIÊN";
            }
            else
            {
                lblTitle.Text = "CHỈNH SỬA THÔNG TIN NHÂN VIÊN";
                LoadData(); // nạp dữ liệu của NCC cần sửa
            }
            lblTitle.Left = (this.ClientSize.Width - lblTitle.Width) / 2;
            //cbogender
            cbogender.Properties.Items.Add("Nam");
            cbogender.Properties.Items.Add("Nữ");


            //nút thêm mới
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
           
            //nút lưu
            btnluu.FlatStyle = FlatStyle.Flat;
            btnluu.FlatAppearance.BorderSize = 0;
            btnluu.Margin = new Padding(10);
            //màu mặc định
            btnluu.BackColor = Color.FromArgb(66, 144, 242);
            btnluu.ForeColor = Color.White;
            //màu khi di chuột
            btnluu.MouseEnter += (s, e) =>
            {
                btnluu.BackColor = Color.FromArgb(118, 173, 243);
             
            };
            btnluu.MouseLeave += (s, e) =>
            {
                btnluu.BackColor = Color.FromArgb(66,144,242);
                
            };
            //màu khi nhấn
            btnluu.MouseDown += (s, e) =>
            {
                btnluu.BackColor = Color.FromArgb(118,173,243);
              
            };
           
            
        }
        private void LoadData()
        {
            // Giả sử bạn có một phương thức để lấy dữ liệu nhà cung cấp từ cơ sở dữ liệu
            // Ví dụ: var ncc = Database.GetNhaCungCapById(MaNCC);
            // Sau đó, gán dữ liệu vào các điều khiển trên form
            // txtTenNCC.Text = ncc.TenNCC;
            // txtDiaChi.Text = ncc.DiaChi;
            // txtSoDienThoai.Text = ncc.SoDienThoai;
            // txtEmail.Text = ncc.Email;
        }
        private void btnhuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}