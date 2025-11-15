using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QLNhaThuoc
{
    public partial class frmThemHD : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["Db"].ConnectionString;
        private string _maHoaDon;
        private DataTable _dtThuoc;
        private string _maKH = null;
        private string _maNV = "NV001";

        public frmThemHD()
        {
            InitializeComponent();
        }

        private void frmThemHD_Load(object sender, EventArgs e)
        {
            SetPlaceholder();

            // Hiển thị thời gian
            SetToolStripLabelText("toolStripStatusLabel3", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            SetToolStripLabelText("toolStripStatusLabel6", "Nhân viên bán hàng");

            LoadDanhSachThuoc();
            LoadPhuongThucThanhToan();
            ThemDongMoi();

            // Đăng ký events
            DataGridView dgv = FindControl<DataGridView>("frmChiTietHD");
            if (dgv != null)
            {
                dgv.CellValueChanged += FrmChiTietHD_CellValueChanged;
                dgv.CurrentCellDirtyStateChanged += FrmChiTietHD_CurrentCellDirtyStateChanged;
            }
        }

        private void SetPlaceholder()
        {
            SetLabelPlaceholder("txtTenKH", "Nhập tên khách hàng");
            SetLabelPlaceholder("txtSdt", "Nhập số điện thoại");
            SetLabelPlaceholder("txtDchi", "Nhập địa chỉ khách hàng");
        }

        private void SetLabelPlaceholder(string name, string placeholder)
        {
            Label lbl = FindControl<Label>(name);
            if (lbl != null)
            {
                lbl.Text = placeholder;
                lbl.ForeColor = Color.Gray;
                lbl.Enter += (s, e) => {
                    if (lbl.Text == placeholder)
                    {
                        lbl.Text = "";
                        lbl.ForeColor = Color.Black;
                    }
                };
                lbl.Leave += (s, e) => {
                    if (string.IsNullOrWhiteSpace(lbl.Text))
                    {
                        lbl.Text = placeholder;
                        lbl.ForeColor = Color.Gray;
                    }
                };
            }
        }

        private void LoadDanhSachThuoc()
        {
            try
            {
                string query = @"
                    SELECT t.MaThuoc, t.TenThuoc, t.GiaBan, dv.TenDonViTinh,
                           ISNULL(SUM(tk.TonKho), 0) AS TonKho
                    FROM Thuoc t
                    LEFT JOIN DonViTinh dv ON t.MaDonViTinh = dv.MaDonViTinh
                    LEFT JOIN TonKho tk ON tk.MaThuoc = t.MaThuoc
                    GROUP BY t.MaThuoc, t.TenThuoc, t.GiaBan, dv.TenDonViTinh
                    HAVING ISNULL(SUM(tk.TonKho), 0) > 0
                    ORDER BY t.TenThuoc";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        _dtThuoc = new DataTable();
                        da.Fill(_dtThuoc);
                    }
                }

                // Gán vào ComboBox column
                DataGridView dgv = FindControl<DataGridView>("frmChiTietHD");
                if (dgv != null && dgv.Columns["TenThuoc"] is DataGridViewComboBoxColumn comboCol)
                {
                    comboCol.DataSource = _dtThuoc;
                    comboCol.DisplayMember = "TenThuoc";
                    comboCol.ValueMember = "MaThuoc";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách thuốc: " + ex.Message);
            }
        }

        private void LoadPhuongThucThanhToan()
        {
            try
            {
                string query = "SELECT MaPTTT, TenPTTT FROM PhuongThucTT";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        cbbPTTT.DataSource = dt;
                        cbbPTTT.DisplayMember = "TenPTTT";
                        cbbPTTT.ValueMember = "MaPTTT";
                        cbbPTTT.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải phương thức thanh toán: " + ex.Message);
            }
        }

        private void ThemDongMoi()
        {
            DataGridView dgv = FindControl<DataGridView>("frmChiTietHD");
            if (dgv != null)
            {
                int stt = dgv.Rows.Count + 1;
                dgv.Rows.Add(stt, null, 1, "0 đ", "0 đ");
            }
        }

        private void FrmChiTietHD_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            if (dgv != null && dgv.IsCurrentCellDirty)
            {
                dgv.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void FrmChiTietHD_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            if (dgv == null || e.RowIndex < 0) return;

            DataGridViewRow row = dgv.Rows[e.RowIndex];

            if (e.ColumnIndex == dgv.Columns["TenThuoc"].Index)
            {
                if (row.Cells["TenThuoc"].Value != null)
                {
                    string maThuoc = row.Cells["TenThuoc"].Value.ToString();
                    DataRow[] thuocRows = _dtThuoc.Select($"MaThuoc = '{maThuoc}'");
                    if (thuocRows.Length > 0)
                    {
                        decimal giaBan = Convert.ToDecimal(thuocRows[0]["GiaBan"]);
                        row.Cells["DonGia"].Value = string.Format("{0:N0} đ", giaBan);
                        row.Cells["DonGia"].Tag = giaBan;
                        TinhThanhTien(row);

                        if (e.RowIndex == dgv.Rows.Count - 1)
                        {
                            ThemDongMoi();
                        }
                    }
                }
            }
            else if (e.ColumnIndex == dgv.Columns["SoLuong"].Index)
            {
                TinhThanhTien(row);
            }

            TinhTongTien();
        }

        private void TinhThanhTien(DataGridViewRow row)
        {
            try
            {
                if (row.Cells["DonGia"].Tag != null && row.Cells["SoLuong"].Value != null)
                {
                    decimal donGia = Convert.ToDecimal(row.Cells["DonGia"].Tag);
                    int soLuong = Convert.ToInt32(row.Cells["SoLuong"].Value);
                    decimal thanhTien = donGia * soLuong;

                    row.Cells["ThanhTien"].Value = string.Format("{0:N0} đ", thanhTien);
                    row.Cells["ThanhTien"].Tag = thanhTien;
                }
            }
            catch { }
        }

        private void TinhTongTien()
        {
            DataGridView dgv = FindControl<DataGridView>("frmChiTietHD");
            if (dgv == null) return;

            decimal tongTien = 0;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.Cells["ThanhTien"].Tag != null)
                {
                    tongTien += Convert.ToDecimal(row.Cells["ThanhTien"].Tag);
                }
            }

            Label lblTong = FindControl<Label>("lblTong");
            if (lblTong != null)
                lblTong.Text = string.Format("{0:N0} đ", tongTien);
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateData()) return;

                XuLyKhachHang();
                DataTable dtChiTiet = TaoBangChiTiet();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("usp_XuatKho_TaoHoaDon", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MaKH", (object)_maKH ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@MaNV", _maNV);
                        cmd.Parameters.AddWithValue("@MaPTTT", cbbPTTT.SelectedValue);
                        cmd.Parameters.AddWithValue("@GhiChu", DBNull.Value);

                        SqlParameter tvpParam = cmd.Parameters.AddWithValue("@ChiTiet", dtChiTiet);
                        tvpParam.SqlDbType = SqlDbType.Structured;
                        tvpParam.TypeName = "dbo.tvp_CTHD";

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                _maHoaDon = reader["MaHoaDon"].ToString();
                                this.DialogResult = DialogResult.OK;
                                this.Close();
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 50002 || sqlEx.Number == 50003)
                    MessageBox.Show(sqlEx.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show("Lỗi SQL: " + sqlEx.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private DataTable TaoBangChiTiet()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaThuoc", typeof(string));
            dt.Columns.Add("SoLuong", typeof(int));
            dt.Columns.Add("DonGia", typeof(decimal));

            DataGridView dgv = FindControl<DataGridView>("frmChiTietHD");
            if (dgv != null)
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.Cells["TenThuoc"].Value != null && row.Cells["DonGia"].Tag != null)
                    {
                        dt.Rows.Add(
                            row.Cells["TenThuoc"].Value.ToString(),
                            Convert.ToInt32(row.Cells["SoLuong"].Value),
                            DBNull.Value
                        );
                    }
                }
            }

            return dt;
        }

        private bool ValidateData()
        {
            if (cbbPTTT.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn phương thức thanh toán!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            DataGridView dgv = FindControl<DataGridView>("frmChiTietHD");
            bool coSanPham = false;
            if (dgv != null)
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.Cells["TenThuoc"].Value != null)
                    {
                        coSanPham = true;
                        break;
                    }
                }
            }

            if (!coSanPham)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một sản phẩm!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void XuLyKhachHang()
        {
            Label txtTenKH = FindControl<Label>("txtTenKH");
            Label txtSdt = FindControl<Label>("txtSdt");
            Label txtDchi = FindControl<Label>("txtDchi");

            if (txtTenKH == null || txtTenKH.ForeColor == Color.Gray || string.IsNullOrWhiteSpace(txtTenKH.Text))
            {
                _maKH = null;
                return;
            }

            string sdt = txtSdt != null ? txtSdt.Text.Trim() : "";
            if (txtSdt != null && txtSdt.ForeColor != Color.Gray && !string.IsNullOrWhiteSpace(sdt))
            {
                _maKH = TimKhachHangTheoSDT(sdt);

                if (_maKH == null)
                {
                    _maKH = TaoKhachHangMoi(
                        txtTenKH.Text.Trim(),
                        sdt,
                        txtDchi != null && txtDchi.ForeColor != Color.Gray ? txtDchi.Text.Trim() : null
                    );
                }
            }
        }

        private string TimKhachHangTheoSDT(string sdt)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT MaKH FROM KhachHang WHERE SDT = @SDT", conn))
                    {
                        cmd.Parameters.AddWithValue("@SDT", sdt);
                        object result = cmd.ExecuteScalar();
                        return result?.ToString();
                    }
                }
            }
            catch { return null; }
        }

        private string TaoKhachHangMoi(string tenKH, string sdt, string diaChi)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        DECLARE @MaKH CHAR(10)
                        SELECT @MaKH = 'KH' + RIGHT('000' + 
                            CAST(ISNULL(MAX(CAST(SUBSTRING(MaKH, 3, 10) AS INT)), 0) + 1 AS VARCHAR), 3)
                        FROM KhachHang WHERE MaKH LIKE 'KH%'
                        
                        INSERT INTO KhachHang (MaKH, TenKH, SDT, DiaChi, DoanhThu)
                        VALUES (@MaKH, @TenKH, @SDT, @DiaChi, 0)
                        
                        SELECT @MaKH";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TenKH", tenKH);
                        cmd.Parameters.AddWithValue("@SDT", sdt);
                        cmd.Parameters.AddWithValue("@DiaChi", (object)diaChi ?? DBNull.Value);

                        return cmd.ExecuteScalar()?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tạo khách hàng: " + ex.Message);
                return null;
            }
        }

        // Helper methods
        private T FindControl<T>(string name) where T : Control
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl.Name == name && ctrl is T) return (T)ctrl;
                T found = FindControlRecursive<T>(ctrl, name);
                if (found != null) return found;
            }
            return null;
        }

        private T FindControlRecursive<T>(Control parent, string name) where T : Control
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl.Name == name && ctrl is T) return (T)ctrl;
                T found = FindControlRecursive<T>(ctrl, name);
                if (found != null) return found;
            }
            return null;
        }

        private void SetToolStripLabelText(string name, string text)
        {
            StatusStrip ss = FindControl<StatusStrip>("statusStrip1");
            if (ss != null)
            {
                foreach (ToolStripItem item in ss.Items)
                {
                    if (item.Name == name)
                    {
                        item.Text = text;
                        break;
                    }
                }
            }
        }

        // Event handlers - GIỮ NGUYÊN
        private void frmChiTietHD_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void txtTenKH_Enter(object sender, EventArgs e) { }
        private void txtTenKH_Leave(object sender, EventArgs e) { }
        private void txtSdt_Enter(object sender, EventArgs e) { }
        private void txtSdt_Leave(object sender, EventArgs e) { }
        private void txtDchi_Enter(object sender, EventArgs e) { }
        private void txtDchi_Leave(object sender, EventArgs e) { }
        private void label7_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
    }
}