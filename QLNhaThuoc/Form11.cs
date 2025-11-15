using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QLNhaThuoc
{
    public partial class Home : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["Db"].ConnectionString;

        public Home()
        {
            InitializeComponent();
        }

        private void Home_Load(object sender, EventArgs e)
        {
            // Kiểm tra kết nối database
            if (!KiemTraKetNoi())
            {
                MessageBox.Show("Không thể kết nối đến cơ sở dữ liệu!\nVui lòng kiểm tra lại cấu hình.",
                    "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Thiết lập DateTimePicker
            DateTimePicker fromDate = FindControl<DateTimePicker>("FromDate");
            DateTimePicker toDate = FindControl<DateTimePicker>("ToDate");

            if (fromDate != null)
            {
                fromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1); // Đầu tháng
                fromDate.ValueChanged += DateFilter_Changed;
            }

            if (toDate != null)
            {
                toDate.Value = DateTime.Now; // Hôm nay
                toDate.ValueChanged += DateFilter_Changed;
            }

            // Đăng ký event double-click cho DataGridView khách hàng
            DataGridView tblDsKhachHang = FindControl<DataGridView>("tblDsKhachHang");
            if (tblDsKhachHang != null)
            {
                tblDsKhachHang.CellDoubleClick += tblDsKhachHang_CellDoubleClick;
                tblDsKhachHang.CellClick += tblDsKhachHang_CellClick;
            }

            // Load danh sách hóa đơn
            LoadDanhSachHoaDon();

            // Load danh sách khách hàng
            LoadDanhSachKhachHang();
        }

        private void DateFilter_Changed(object sender, EventArgs e)
        {
            LoadDanhSachHoaDon();
        }

        private bool KiemTraKetNoi()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message);
                return false;
            }
        }

        private void LoadDanhSachHoaDon()
        {
            try
            {
                DataGridView tblDSHD = FindControl<DataGridView>("tblDSHD");
                if (tblDSHD == null)
                {
                    MessageBox.Show("Không tìm thấy DataGridView 'tblDSHD'", "Lỗi");
                    return;
                }

                tblDSHD.Rows.Clear();

                // Lấy giá trị từ DateTimePicker
                DateTimePicker fromDate = FindControl<DateTimePicker>("FromDate");
                DateTimePicker toDate = FindControl<DateTimePicker>("ToDate");

                DateTime tuNgay = fromDate != null ? fromDate.Value.Date : DateTime.MinValue;
                DateTime denNgay = toDate != null ? toDate.Value.Date.AddDays(1).AddSeconds(-1) : DateTime.MaxValue;

                string query = @"
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY NgayLap DESC) AS STT,
                        MaHoaDon,
                        NgayLap,
                        TenKH,
                        TongTien,
                        TenPTTT,
                        SanPhamRutGon
                    FROM v_HoaDon_List
                    WHERE NgayLap BETWEEN @TuNgay AND @DenNgay
                    ORDER BY NgayLap DESC";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                        cmd.Parameters.AddWithValue("@DenNgay", denNgay);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tblDSHD.Rows.Add(
                                    false,  // Checkbox
                                    reader["STT"],
                                    reader["MaHoaDon"],
                                    reader["SanPhamRutGon"] != DBNull.Value ? reader["SanPhamRutGon"].ToString() : "",
                                    "Xem"   // Button
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách hóa đơn: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDanhSachKhachHang()
        {
            try
            {
                DataGridView tblDsKhachHang = FindControl<DataGridView>("tblDsKhachHang");
                if (tblDsKhachHang == null) return;

                tblDsKhachHang.Rows.Clear();

                string query = @"
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY MaKH) AS STT,
                        MaKH,
                        TenKH,
                        SDT,
                        DiaChi,
                        ISNULL(DoanhThu, 0) AS DoanhThu
                    FROM KhachHang
                    ORDER BY MaKH DESC";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tblDsKhachHang.Rows.Add(
                                    reader["STT"],
                                    reader["MaKH"],
                                    reader["TenKH"] != DBNull.Value ? reader["TenKH"].ToString() : "",
                                    reader["SDT"],
                                    reader["DiaChi"] != DBNull.Value ? reader["DiaChi"].ToString() : "",
                                    string.Format("{0:N0} đ", reader["DoanhThu"]),
                                    "Xem Chi Tiết"  // Giá trị cho button
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách khách hàng: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tblDSHD_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            if (dgv == null || e.RowIndex < 0) return;

            // Tìm cột "Xem" hoặc "XemChiTiet"
            int xemColIndex = -1;
            if (dgv.Columns.Contains("XemChiTiet"))
                xemColIndex = dgv.Columns["XemChiTiet"].Index;
            else if (dgv.Columns.Contains("Xem"))
                xemColIndex = dgv.Columns["Xem"].Index;

            if (e.ColumnIndex == xemColIndex)
            {
                string maHoaDon = null;

                // Tìm cột MaHoaDon
                if (dgv.Columns.Contains("MaHoaDon"))
                    maHoaDon = dgv.Rows[e.RowIndex].Cells["MaHoaDon"].Value?.ToString();
                else if (dgv.Columns.Count > 2)
                    maHoaDon = dgv.Rows[e.RowIndex].Cells[2].Value?.ToString(); // Column index 2

                if (!string.IsNullOrEmpty(maHoaDon))
                {
                    frmHoaDonBH fChiTiet = new frmHoaDonBH(maHoaDon);
                    fChiTiet.StartPosition = FormStartPosition.CenterScreen;
                    fChiTiet.ShowDialog();
                }
            }
        }

        private void tblDsKhachHang_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridView dgv = sender as DataGridView;
            if (dgv == null) return;

            // Kiểm tra nếu click vào cột "Xem Chi Tiết"
            if (dgv.Columns.Contains("XemChiTietKH") && e.ColumnIndex == dgv.Columns["XemChiTietKH"].Index)
            {
                // Lấy mã khách hàng từ dòng được click
                string maKH = null;
                if (dgv.Columns.Contains("maKH"))
                    maKH = dgv.Rows[e.RowIndex].Cells["maKH"].Value?.ToString();
                else if (dgv.Columns.Count > 1)
                    maKH = dgv.Rows[e.RowIndex].Cells[1].Value?.ToString(); // Cột 1 là Mã KH

                if (!string.IsNullOrEmpty(maKH))
                {
                    frmChiTietKhachHang fChiTiet = new frmChiTietKhachHang(maKH);
                    fChiTiet.StartPosition = FormStartPosition.CenterScreen;
                    fChiTiet.ShowDialog();
                }
            }
        }

        private void btnTaoMoiHD_Click(object sender, EventArgs e)
        {
            frmThemHD fThemHD = new frmThemHD();
            fThemHD.StartPosition = FormStartPosition.CenterScreen;

            if (fThemHD.ShowDialog() == DialogResult.OK)
            {
                LoadDanhSachHoaDon();
                LoadDanhSachKhachHang();
                MessageBox.Show("Tạo hóa đơn thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TextBox txtSearch = sender as TextBox;
                if (txtSearch == null) return;

                string tuKhoa = txtSearch.Text.Trim();

                DataGridView tblDSHD = FindControl<DataGridView>("tblDSHD");
                if (tblDSHD == null) return;

                tblDSHD.Rows.Clear();

                // Lấy giá trị từ DateTimePicker
                DateTimePicker fromDate = FindControl<DateTimePicker>("FromDate");
                DateTimePicker toDate = FindControl<DateTimePicker>("ToDate");

                DateTime tuNgay = fromDate != null ? fromDate.Value.Date : DateTime.MinValue;
                DateTime denNgay = toDate != null ? toDate.Value.Date.AddDays(1).AddSeconds(-1) : DateTime.MaxValue;

                string query = @"
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY NgayLap DESC) AS STT,
                        MaHoaDon,
                        NgayLap,
                        TenKH,
                        TongTien,
                        TenPTTT,
                        SanPhamRutGon
                    FROM v_HoaDon_List
                    WHERE NgayLap BETWEEN @TuNgay AND @DenNgay
                    AND (@TuKhoa IS NULL OR @TuKhoa = '' 
                           OR MaHoaDon LIKE '%' + @TuKhoa + '%' 
                           OR TenKH LIKE N'%' + @TuKhoa + '%')
                    ORDER BY NgayLap DESC";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                        cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                        cmd.Parameters.AddWithValue("@TuKhoa", string.IsNullOrEmpty(tuKhoa) ? (object)DBNull.Value : tuKhoa);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tblDSHD.Rows.Add(
                                    false,
                                    reader["STT"],
                                    reader["MaHoaDon"],
                                    reader["SanPhamRutGon"] != DBNull.Value ? reader["SanPhamRutGon"].ToString() : "",
                                    "Xem"
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnInHD_Click(object sender, EventArgs e)
        {
            DataGridView tblDSHD = FindControl<DataGridView>("tblDSHD");
            if (tblDSHD == null) return;

            System.Collections.Generic.List<string> selectedInvoices = new System.Collections.Generic.List<string>();

            foreach (DataGridViewRow row in tblDSHD.Rows)
            {
                // Tìm cột checkbox (thường là cột đầu tiên hoặc tên "Select")
                int checkboxColIndex = 0;
                if (tblDSHD.Columns.Contains("Select"))
                    checkboxColIndex = tblDSHD.Columns["Select"].Index;

                if (row.Cells[checkboxColIndex].Value != null &&
                    (bool)row.Cells[checkboxColIndex].Value == true)
                {
                    // Tìm cột MaHoaDon
                    if (tblDSHD.Columns.Contains("MaHoaDon"))
                        selectedInvoices.Add(row.Cells["MaHoaDon"].Value?.ToString());
                    else if (tblDSHD.Columns.Count > 2)
                        selectedInvoices.Add(row.Cells[2].Value?.ToString());
                }
            }

            if (selectedInvoices.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một hóa đơn để in!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show($"Đã chọn {selectedInvoices.Count} hóa đơn để in:\n" +
                string.Join(", ", selectedInvoices),
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Helper method
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

        // Event handlers - GIỮ NGUYÊN TẤT CẢ
        private void label1_Click(object sender, EventArgs e) { }
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridView dgv = sender as DataGridView;
                if (dgv != null)
                {
                    DataGridViewRow row = dgv.Rows[e.RowIndex];

                    // Tìm cột MaHoaDon
                    string maHoaDon = null;
                    if (dgv.Columns.Contains("MaHoaDon"))
                        maHoaDon = row.Cells["MaHoaDon"].Value?.ToString();
                    else if (dgv.Columns.Count > 2)
                        maHoaDon = row.Cells[2].Value?.ToString();

                    if (!string.IsNullOrEmpty(maHoaDon))
                        MessageBox.Show(maHoaDon);
                }
            }
        }

        private void tblDsKhachHang_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridView dgv = sender as DataGridView;
            if (dgv == null) return;

            // Lấy mã khách hàng từ dòng được click
            string maKH = null;
            if (dgv.Columns.Contains("maKH"))
                maKH = dgv.Rows[e.RowIndex].Cells["maKH"].Value?.ToString();
            else if (dgv.Columns.Count > 1)
                maKH = dgv.Rows[e.RowIndex].Cells[1].Value?.ToString(); // Cột 1 là Mã KH

            if (!string.IsNullOrEmpty(maKH))
            {
                frmChiTietKhachHang fChiTiet = new frmChiTietKhachHang(maKH);
                fChiTiet.StartPosition = FormStartPosition.CenterScreen;
                fChiTiet.ShowDialog();
            }
        }
    }
}