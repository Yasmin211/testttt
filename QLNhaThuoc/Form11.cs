using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NhaThuoc_QLBH
{
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = tblDSHD.Rows[e.RowIndex];
                string maHoaDon = row.Cells["MaHoaDon"].Value?.ToString();

                MessageBox.Show(maHoaDon);
            }

        }
        private void tblDSHD_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == tblDSHD.Columns["XemChiTiet"].Index)
            {
                string maHoaDon = tblDSHD.Rows[e.RowIndex].Cells["MaHoaDon"].Value?.ToString();

                frmHoaDonBH fChiTiet = new frmHoaDonBH(maHoaDon);
                fChiTiet.StartPosition = FormStartPosition.CenterScreen;
                fChiTiet.ShowDialog();
            }
        }



        private void btnInHD_Click(object sender, EventArgs e)
        {

        }

        private void btnTaoMoiHD_Click(object sender, EventArgs e)
        {
            frmThemHD fThemHD = new frmThemHD();
            fThemHD.StartPosition = FormStartPosition.CenterScreen;
            fThemHD.ShowDialog();
        }
        

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private void Home_Load(object sender, EventArgs e)
        {
            // Test data
            tblDSHD.Rows.Add(false, 1, "HD001", "Paracetamol, Vitamin C", "Xem");
            tblDSHD.Rows.Add(false, 2, "HD002", "Thuốc ho, Siro mật ong", "Xem");
            tblDSHD.Rows.Add(false, 3, "HD003", "Khẩu trang, Nước muối", "Xem");
        
    }
    }
}

