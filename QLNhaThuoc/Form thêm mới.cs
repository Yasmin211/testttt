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
    public partial class frmThemHD : Form
    {
        public frmThemHD()
        {
            InitializeComponent();
            
        }

        private void frmChiTietHD_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void txtTenKH_Enter(object sender, EventArgs e)
        {
            if (txtTenKH.Text == "Nhập tên khách hàng")
            {
                txtTenKH.Text = "";
                txtTenKH.ForeColor = Color.Black;
            }
        }

        private void txtTenKH_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenKH.Text))
            {
                txtTenKH.Text = "Nhập tên khách hàng";
                txtTenKH.ForeColor = Color.Gray;
            }
        }
        private void frmThemHD_Load(object sender, EventArgs e)
        {
            txtTenKH.Text = "Nhập tên khách hàng";
            txtTenKH.ForeColor = Color.Gray;

            txtSdt.Text = "Nhập số điện thoại";
            txtSdt.ForeColor = Color.Gray;

            txtDchi.ForeColor = Color.Gray;
            txtDchi.Text = "Nhập địa chỉ khách hàng";
        }
        private void txtSdt_Enter(object sender, EventArgs e)
        {
            if (txtSdt.Text == "Nhập số điện thoại")
            {
                txtSdt.Text = "";
                txtSdt.ForeColor = Color.Black;
            }
        }
        private void txtSdt_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSdt.Text))
            {
                txtSdt.Text = "Nhập số điện thoại";
                txtSdt.ForeColor = Color.Gray;
            }
        }
        private void txtDchi_Enter(object sender, EventArgs e)
        {
            if (txtDchi.Text == "Nhập địa chỉ khách hàng")
            {
                txtDchi.Text = "";
                txtDchi.ForeColor = Color.Black;
            }
        }
        private void txtDchi_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDchi.Text))
            {
                txtDchi.Text = "Nhập địa chỉ khách hàng";
                txtDchi.ForeColor = Color.Gray;
            }
        }
        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
            
