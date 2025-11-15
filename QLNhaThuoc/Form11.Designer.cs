namespace QLNhaThuoc
{
    partial class Home
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Home));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.FromDate = new System.Windows.Forms.DateTimePicker();
            this.ToDate = new System.Windows.Forms.DateTimePicker();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.tblDSHD = new System.Windows.Forms.DataGridView();
            this.btnTaoMoiHD = new System.Windows.Forms.Button();
            this.btnInHD = new System.Windows.Forms.Button();
            this.tblDsKhachHang = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.STT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaHoaDon = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DanhMuc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.XemChiTiet = new System.Windows.Forms.DataGridViewButtonColumn();
            this.sttKH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.maKH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tenKH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sdtKH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dcKH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DoanhThu = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.XemChiTietKH = new System.Windows.Forms.DataGridViewButtonColumn();
            this.btnSearch = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.tblDSHD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tblDsKhachHang)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.SteelBlue;
            this.label1.Name = "label1";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // FromDate
            // 
            resources.ApplyResources(this.FromDate, "FromDate");
            this.FromDate.Name = "FromDate";
            this.FromDate.Value = new System.DateTime(2025, 10, 1, 0, 0, 0, 0);
            // 
            // ToDate
            // 
            resources.ApplyResources(this.ToDate, "ToDate");
            this.ToDate.Name = "ToDate";
            // 
            // txtSearch
            // 
            resources.ApplyResources(this.txtSearch, "txtSearch");
            this.txtSearch.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // tblDSHD
            // 
            this.tblDSHD.BackgroundColor = System.Drawing.Color.White;
            this.tblDSHD.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.Desktop;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.tblDSHD.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.tblDSHD.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tblDSHD.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select,
            this.STT,
            this.MaHoaDon,
            this.DanhMuc,
            this.XemChiTiet});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 10F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.tblDSHD.DefaultCellStyle = dataGridViewCellStyle3;
            this.tblDSHD.GridColor = System.Drawing.SystemColors.ControlLight;
            resources.ApplyResources(this.tblDSHD, "tblDSHD");
            this.tblDSHD.Name = "tblDSHD";
            this.tblDSHD.RowHeadersVisible = false;
            this.tblDSHD.RowTemplate.Height = 24;
            this.tblDSHD.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.tblDSHD_CellClick);
            // 
            // btnTaoMoiHD
            // 
            this.btnTaoMoiHD.BackColor = System.Drawing.Color.SteelBlue;
            this.btnTaoMoiHD.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.btnTaoMoiHD, "btnTaoMoiHD");
            this.btnTaoMoiHD.ForeColor = System.Drawing.SystemColors.Control;
            this.btnTaoMoiHD.Name = "btnTaoMoiHD";
            this.btnTaoMoiHD.UseVisualStyleBackColor = false;
            this.btnTaoMoiHD.Click += new System.EventHandler(this.btnTaoMoiHD_Click);
            // 
            // btnInHD
            // 
            this.btnInHD.BackColor = System.Drawing.Color.SteelBlue;
            this.btnInHD.FlatAppearance.BorderSize = 0;
            this.btnInHD.FlatAppearance.MouseDownBackColor = System.Drawing.Color.SteelBlue;
            resources.ApplyResources(this.btnInHD, "btnInHD");
            this.btnInHD.ForeColor = System.Drawing.SystemColors.Control;
            this.btnInHD.Name = "btnInHD";
            this.btnInHD.UseVisualStyleBackColor = false;
            this.btnInHD.Click += new System.EventHandler(this.btnInHD_Click);
            // 
            // tblDsKhachHang
            // 
            this.tblDsKhachHang.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.tblDsKhachHang.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.tblDsKhachHang.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tblDsKhachHang.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.sttKH,
            this.maKH,
            this.tenKH,
            this.sdtKH,
            this.dcKH,
            this.DoanhThu,
            this.XemChiTietKH});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 10F);
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.tblDsKhachHang.DefaultCellStyle = dataGridViewCellStyle5;
            resources.ApplyResources(this.tblDsKhachHang, "tblDsKhachHang");
            this.tblDsKhachHang.Name = "tblDsKhachHang";
            this.tblDsKhachHang.RowHeadersVisible = false;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.tblDsKhachHang.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.tblDsKhachHang.RowTemplate.Height = 24;
            // 
            // Select
            // 
            resources.ApplyResources(this.Select, "Select");
            this.Select.Name = "Select";
            this.Select.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Select.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // STT
            // 
            resources.ApplyResources(this.STT, "STT");
            this.STT.Name = "STT";
            this.STT.ReadOnly = true;
            // 
            // MaHoaDon
            // 
            resources.ApplyResources(this.MaHoaDon, "MaHoaDon");
            this.MaHoaDon.Name = "MaHoaDon";
            this.MaHoaDon.ReadOnly = true;
            // 
            // DanhMuc
            // 
            resources.ApplyResources(this.DanhMuc, "DanhMuc");
            this.DanhMuc.Name = "DanhMuc";
            this.DanhMuc.ReadOnly = true;
            // 
            // XemChiTiet
            // 
            this.XemChiTiet.DataPropertyName = "Xem chi tiết";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HotTrack;
            this.XemChiTiet.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.XemChiTiet, "XemChiTiet");
            this.XemChiTiet.Name = "XemChiTiet";
            this.XemChiTiet.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.XemChiTiet.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.XemChiTiet.Text = "Xem chi tiết";
            this.XemChiTiet.UseColumnTextForButtonValue = true;
            // 
            // sttKH
            // 
            resources.ApplyResources(this.sttKH, "sttKH");
            this.sttKH.Name = "sttKH";
            this.sttKH.ReadOnly = true;
            // 
            // maKH
            // 
            resources.ApplyResources(this.maKH, "maKH");
            this.maKH.Name = "maKH";
            // 
            // tenKH
            // 
            resources.ApplyResources(this.tenKH, "tenKH");
            this.tenKH.Name = "tenKH";
            this.tenKH.ReadOnly = true;
            // 
            // sdtKH
            // 
            resources.ApplyResources(this.sdtKH, "sdtKH");
            this.sdtKH.Name = "sdtKH";
            // 
            // dcKH
            // 
            resources.ApplyResources(this.dcKH, "dcKH");
            this.dcKH.Name = "dcKH";
            // 
            // DoanhThu
            // 
            resources.ApplyResources(this.DoanhThu, "DoanhThu");
            this.DoanhThu.Name = "DoanhThu";
            this.DoanhThu.ReadOnly = true;
            // 
            // XemChiTietKH
            // 
            resources.ApplyResources(this.XemChiTietKH, "XemChiTietKH");
            this.XemChiTietKH.Name = "XemChiTietKH";
            this.XemChiTietKH.Text = "Xem Chi Tiết";
            this.XemChiTietKH.UseColumnTextForButtonValue = true;
            // 
            // btnSearch
            // 
            resources.ApplyResources(this.btnSearch, "btnSearch");
            this.btnSearch.Cursor = System.Windows.Forms.Cursors.Default;
            // this.btnSearch.Image = global::QLNhaThuoc.Properties.Resources._5402443_search_find_magnifier_magnifying_magnifying_glass_icon1;
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.UseVisualStyleBackColor = true;
            // 
            // Home
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.tblDsKhachHang);
            this.Controls.Add(this.btnInHD);
            this.Controls.Add(this.btnTaoMoiHD);
            this.Controls.Add(this.tblDSHD);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.ToDate);
            this.Controls.Add(this.FromDate);
            this.Controls.Add(this.label1);
            this.Name = "Home";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Home_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tblDSHD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tblDsKhachHang)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker FromDate;
        private System.Windows.Forms.DateTimePicker ToDate;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DataGridView tblDSHD;
        private System.Windows.Forms.Button btnTaoMoiHD;
        private System.Windows.Forms.Button btnInHD;
        private System.Windows.Forms.DataGridView tblDsKhachHang;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.DataGridViewTextBoxColumn STT;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaHoaDon;
        private System.Windows.Forms.DataGridViewTextBoxColumn DanhMuc;
        private System.Windows.Forms.DataGridViewButtonColumn XemChiTiet;
        private System.Windows.Forms.DataGridViewTextBoxColumn sttKH;
        private System.Windows.Forms.DataGridViewTextBoxColumn maKH;
        private System.Windows.Forms.DataGridViewTextBoxColumn tenKH;
        private System.Windows.Forms.DataGridViewTextBoxColumn sdtKH;
        private System.Windows.Forms.DataGridViewTextBoxColumn dcKH;
        private System.Windows.Forms.DataGridViewTextBoxColumn DoanhThu;
        private System.Windows.Forms.DataGridViewButtonColumn XemChiTietKH;
    }
}

