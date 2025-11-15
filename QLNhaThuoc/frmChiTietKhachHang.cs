using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QLNhaThuoc
{
    public partial class frmChiTietKhachHang : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["Db"].ConnectionString;
        private string _maKH;

        public frmChiTietKhachHang(string maKH)
        {
            InitializeComponent();
            _maKH = maKH;
        }

        private void frmChiTietKhachHang_Load(object sender, EventArgs e)
        {
   try
         {
         LoadThongTinKhachHang();
    LoadLichSuMuaHang();
        }
  catch (Exception ex)
            {
    MessageBox.Show("L?i: " + ex.Message, "L?i",
    MessageBoxButtons.OK, MessageBoxIcon.Error);
 this.Close();
            }
        }

        private void LoadThongTinKhachHang()
 {
            string query = @"
         SELECT MaKH, TenKH, SDT, DiaChi, ISNULL(DoanhThu, 0) AS DoanhThu
        FROM KhachHang
    WHERE MaKH = @MaKH";

      using (SqlConnection conn = new SqlConnection(connectionString))
            {
      conn.Open();
      using (SqlCommand cmd = new SqlCommand(query, conn))
                {
    cmd.Parameters.AddWithValue("@MaKH", _maKH);

        using (SqlDataReader reader = cmd.ExecuteReader())
        {
  if (reader.Read())
   {
         SetLabelText("lblMaKH", reader["MaKH"].ToString());
    SetLabelText("lblTenKH", reader["TenKH"].ToString());
      SetLabelText("lblSDT", reader["SDT"] != DBNull.Value ? reader["SDT"].ToString() : "[Ch?a có]");
         SetLabelText("lblDiaChi", reader["DiaChi"] != DBNull.Value ? reader["DiaChi"].ToString() : "[Ch?a có]");
         SetLabelText("lblDoanhThu", string.Format("{0:N0} ?", reader["DoanhThu"]));
                  }
     else
     {
       MessageBox.Show("Không tìm th?y khách hàng!", "Thông báo",
     MessageBoxButtons.OK, MessageBoxIcon.Warning);
      this.Close();
            }
      }
}
       }
        }

     private void LoadLichSuMuaHang()
      {
            try
            {
        DataGridView dgvLichSu = FindControl<DataGridView>("dgvLichSuMuaHang");
     if (dgvLichSu == null) return;

                dgvLichSu.Rows.Clear();

  string query = @"
        SELECT 
           ROW_NUMBER() OVER (ORDER BY h.NgayLap DESC) AS STT,
            h.MaHoaDon,
                 h.NgayLap,
      h.TongTien,
  pttt.TenPTTT
    FROM HoaDon h
      LEFT JOIN PhuongThucTT pttt ON h.MaPTTT = pttt.MaPTTT
           WHERE h.MaKH = @MaKH
          ORDER BY h.NgayLap DESC";

      using (SqlConnection conn = new SqlConnection(connectionString))
                {
             conn.Open();
           using (SqlCommand cmd = new SqlCommand(query, conn))
  {
    cmd.Parameters.AddWithValue("@MaKH", _maKH);

    using (SqlDataReader reader = cmd.ExecuteReader())
              {
         while (reader.Read())
           {
           dgvLichSu.Rows.Add(
     reader["STT"],
    reader["MaHoaDon"],
              Convert.ToDateTime(reader["NgayLap"]).ToString("dd/MM/yyyy HH:mm"),
                   string.Format("{0:N0} ?", reader["TongTien"]),
             reader["TenPTTT"] != DBNull.Value ? reader["TenPTTT"].ToString() : "",
"Xem"
     );
          }
          }
      }
            }
   }
            catch (Exception ex)
{
            MessageBox.Show("L?i khi t?i l?ch s?: " + ex.Message);
          }
    }

     private void dgvLichSuMuaHang_CellClick(object sender, DataGridViewCellEventArgs e)
 {
            DataGridView dgv = sender as DataGridView;
  if (dgv == null || e.RowIndex < 0) return;

   // Ki?m tra n?u click vào c?t "Xem"
      if (e.ColumnIndex == dgv.Columns.Count - 1) // C?t cu?i cùng là nút Xem
   {
        string maHoaDon = dgv.Rows[e.RowIndex].Cells[1].Value?.ToString(); // C?t 1 là MaHoaDon

     if (!string.IsNullOrEmpty(maHoaDon))
       {
     frmHoaDonBH fChiTiet = new frmHoaDonBH(maHoaDon);
        fChiTiet.StartPosition = FormStartPosition.CenterScreen;
            fChiTiet.ShowDialog();
            }
            }
}

        private void SetLabelText(string labelName, string text)
        {
     Label lbl = FindControl<Label>(labelName);
            if (lbl != null) lbl.Text = text;
      }

  private T FindControl<T>(string name) where T : Control
{
          foreach (Control ctrl in this.Controls)
       {
      if (ctrl.Name == name && ctrl is T)
   return (T)ctrl;

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
    }
}
