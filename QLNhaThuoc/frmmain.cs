using DevExpress.LookAndFeel;
using DevExpress.Map.Native;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Controls.Ribbon;
using System.Windows.Forms;

namespace QLNhaThuoc
{
    public partial class frmmain : DevExpress.XtraEditors.XtraForm
    {
        public frmmain()
        {
            InitializeComponent();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            // Giãn đều giữa các BarButtonItem
            // Thiết lập mặc định
            barStaticItemstt.Caption = "Ready";
            barStaticItemTime.Caption = DateTime.Now.ToString("HH:mm:ss");

            // Tạo timer cập nhật thời gian
            Timer timer = new Timer();
            timer.Interval = 1000; // 1 giây
            timer.Tick += (s, e) =>
            {
                barStaticItemTime.Caption = DateTime.Now.ToString("HH:mm:ss - dd/MM/yyyy");
            };
            timer.Start();


        }

        private void frmmain_Load(object sender, EventArgs e)
        {

            DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinMaskColor = Color.FromArgb(12, 154, 255);

        }

        private void btnthoat_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Close();
        }

        private void btnnv_ItemClick(object sender, ItemClickEventArgs e)
        {
            pnlform.Controls.Clear();
            nhanvien nhanvien = new nhanvien() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true, FormBorderStyle = FormBorderStyle.None };

            pnlform.Controls.Add(nhanvien);
            nhanvien.Show();

        }

        private void btnncc_ItemClick(object sender, ItemClickEventArgs e)
        {
            pnlform.Controls.Clear();
            nhacungcap nhacungcap = new nhacungcap() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true, FormBorderStyle = FormBorderStyle.None };

            pnlform.Controls.Add(nhacungcap);
            nhacungcap.Show();
        }

        private void btnnhaphang_ItemClick(object sender, ItemClickEventArgs e)
        {
            pnlform.Controls.Clear();
            nhaphang nhaphang = new nhaphang() { Dock = DockStyle.Fill, TopLevel = false, TopMost = true, FormBorderStyle = FormBorderStyle.None };
            pnlform.Controls.Add(nhaphang);
            nhaphang.Show();
        }
    }
}

