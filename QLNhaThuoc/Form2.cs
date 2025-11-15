using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QLNhaThuoc
{
    public partial class frmHoaDonBH : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["Db"].ConnectionString;
        private string _maHoaDon;

        public frmHoaDonBH(string maHoaDon)
        {
            InitializeComponent();
            _maHoaDon = maHoaDon;
        }

        private void frmHoaDonBH_Load(object sender, EventArgs e)
        {
            try
            {
                // Hiển thị mã hóa đơn
                if (lblMaHD != null)
                    lblMaHD.Text = _maHoaDon;

                // Load thông tin hóa đơn
                LoadThongTinHoaDon();

                // Load chi tiết sản phẩm
                LoadChiTietSanPham();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void LoadThongTinHoaDon()
        {
            string query = @"
                SELECT 
                    h.MaHoaDon, h.NgayLap, h.TongTien, h.GhiChu,
                    kh.TenKH, kh.SDT, kh.DiaChi,
                    nv.TenNV,
                    pttt.TenPTTT
                FROM HoaDon h
                LEFT JOIN KhachHang kh ON h.MaKH = kh.MaKH
                LEFT JOIN NhanVien nv ON h.MaNV = nv.MaNV
                LEFT JOIN PhuongThucTT pttt ON h.MaPTTT = pttt.MaPTTT
                WHERE h.MaHoaDon = @MaHoaDon";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaHoaDon", _maHoaDon);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Tìm control theo tên an toàn
                            SetLabelText("label5", reader["TenKH"] != DBNull.Value ? reader["TenKH"].ToString() : "[Khách lẻ]");
                            SetLabelText("label7", reader["SDT"] != DBNull.Value ? reader["SDT"].ToString() : "");
                            SetLabelText("label12", reader["DiaChi"] != DBNull.Value ? reader["DiaChi"].ToString() : "[Trống]");
                            SetLabelText("label10", string.Format("{0:N0} đ", reader["TongTien"]));
                            SetLabelText("label9", reader["TenPTTT"] != DBNull.Value ? reader["TenPTTT"].ToString() : "");

                            // Status bar
                            SetToolStripLabelText("toolStripStatusLabel3",
                                Convert.ToDateTime(reader["NgayLap"]).ToString("dd/MM/yyyy HH:mm"));
                            SetToolStripLabelText("toolStripStatusLabel6",
                                reader["TenNV"] != DBNull.Value ? reader["TenNV"].ToString() : "");
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy hóa đơn!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            this.Close();
                        }
                    }
                }
            }
        }

        private void LoadChiTietSanPham()
        {
            try
            {
                // Tìm DataGridView
                DataGridView dgv = FindControl<DataGridView>("frmChiTietHD");
                if (dgv == null) return;

                dgv.Rows.Clear();

                string query = @"
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY ct.MaCTHD) AS STT,
                        t.TenThuoc,
                        ct.SoLuong,
                        ct.DonGia,
                        ct.ThanhTien
                    FROM ChiTietHoaDon ct
                    JOIN Thuoc t ON ct.MaThuoc = t.MaThuoc
                    WHERE ct.MaHoaDon = @MaHoaDon";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaHoaDon", _maHoaDon);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dgv.Rows.Add(
                                    reader["STT"],
                                    reader["TenThuoc"],
                                    reader["SoLuong"],
                                    string.Format("{0:N0} đ", reader["DonGia"]),
                                    string.Format("{0:N0} đ", reader["ThanhTien"])
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải chi tiết: " + ex.Message);
            }
        }

        // Helper methods để tìm controls an toàn
        private void SetLabelText(string labelName, string text)
        {
            Label lbl = FindControl<Label>(labelName);
            if (lbl != null) lbl.Text = text;
        }

        private void SetToolStripLabelText(string name, string text)
        {
            if (statusStrip1 == null) return;
            foreach (ToolStripItem item in statusStrip1.Items)
            {
                if (item.Name == name && item is ToolStripStatusLabel)
                {
                    item.Text = text;
                    break;
                }
            }
        }

        private T FindControl<T>(string name) where T : Control
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl.Name == name && ctrl is T)
                    return (T)ctrl;

                // Tìm trong container controls
                T found = FindControlRecursive<T>(ctrl, name);
                if (found != null) return found;
            }
            return null;
        }

        private T FindControlRecursive<T>(Control parent, string name) where T : Control
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl.Name == name && ctrl is T)
                    return (T)ctrl;

                T found = FindControlRecursive<T>(ctrl, name);
                if (found != null) return found;
            }
            return null;
        }

        // Event handlers - GIỮ NGUYÊN KHÔNG XÓA
        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void lblSDT_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void toolStripStatusLabel2_Click(object sender, EventArgs e) { }
        private void toolStripStatusLabel4_Click(object sender, EventArgs e) { }
        private void label11_Click(object sender, EventArgs e) { }
        private void label10_Click(object sender, EventArgs e) { }
        private void toolStripStatusLabel1_Click(object sender, EventArgs e) { }
        private void toolStripStatusLabel1_Click_1(object sender, EventArgs e) { }
    }
}