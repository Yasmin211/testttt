using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.UserSkins;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QLNhaThuoc
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // ======== 🔍 KIỂM TRA KẾT NỐI SQL SERVER TRƯỚC ========
            try
            {
                string cs = ConfigurationManager.ConnectionStrings["Db"].ConnectionString;
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    MessageBox.Show("✅ Kết nối SQL Server thành công!",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi kết nối SQL Server:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Dừng chương trình nếu lỗi
            }

            // ======== 🚀 CHẠY FORM CHÍNH SAU KHI KẾT NỐI OK ========
            Application.Run(new FormWarehouse());
        }
    }
}
