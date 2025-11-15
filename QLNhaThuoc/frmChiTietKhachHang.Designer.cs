namespace QLNhaThuoc
{
    partial class frmChiTietKhachHang
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
 if (disposing && (components != null))
            {
 components.Dispose();
  }
     base.Dispose(disposing);
        }

 private void InitializeComponent()
  {
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
       System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
 this.panelHeader = new System.Windows.Forms.Panel();
        this.lblTitle = new System.Windows.Forms.Label();
   this.panelThongTin = new System.Windows.Forms.Panel();
            this.lblDoanhThu = new System.Windows.Forms.Label();
 this.label5 = new System.Windows.Forms.Label();
            this.lblDiaChi = new System.Windows.Forms.Label();
     this.label4 = new System.Windows.Forms.Label();
  this.lblSDT = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
       this.lblTenKH = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
     this.lblMaKH = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
     this.label6 = new System.Windows.Forms.Label();
            this.dgvLichSuMuaHang = new System.Windows.Forms.DataGridView();
   this.STT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaHoaDon = new System.Windows.Forms.DataGridViewTextBoxColumn();
     this.NgayLap = new System.Windows.Forms.DataGridViewTextBoxColumn();
    this.TongTien = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PhuongThucTT = new System.Windows.Forms.DataGridViewTextBoxColumn();
    this.XemChiTiet = new System.Windows.Forms.DataGridViewButtonColumn();
         this.panelHeader.SuspendLayout();
            this.panelThongTin.SuspendLayout();
((System.ComponentModel.ISupportInitialize)(this.dgvLichSuMuaHang)).BeginInit();
 this.SuspendLayout();
            // 
      // panelHeader
        // 
            this.panelHeader.BackColor = System.Drawing.Color.SteelBlue;
       this.panelHeader.Controls.Add(this.lblTitle);
   this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
        this.panelHeader.Location = new System.Drawing.Point(0, 0);
     this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1000, 60);
this.panelHeader.TabIndex = 0;
     // 
     // lblTitle
            // 
            this.lblTitle.AutoSize = true;
   this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
      this.lblTitle.Location = new System.Drawing.Point(20, 15);
     this.lblTitle.Name = "lblTitle";
        this.lblTitle.Size = new System.Drawing.Size(252, 30);
   this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Thông Tin Khách Hàng";
            // 
       // panelThongTin
// 
   this.panelThongTin.BackColor = System.Drawing.Color.White;
        this.panelThongTin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.panelThongTin.Controls.Add(this.lblDoanhThu);
      this.panelThongTin.Controls.Add(this.label5);
         this.panelThongTin.Controls.Add(this.lblDiaChi);
          this.panelThongTin.Controls.Add(this.label4);
            this.panelThongTin.Controls.Add(this.lblSDT);
    this.panelThongTin.Controls.Add(this.label3);
 this.panelThongTin.Controls.Add(this.lblTenKH);
            this.panelThongTin.Controls.Add(this.label2);
            this.panelThongTin.Controls.Add(this.lblMaKH);
            this.panelThongTin.Controls.Add(this.label1);
  this.panelThongTin.Location = new System.Drawing.Point(25, 80);
        this.panelThongTin.Name = "panelThongTin";
            this.panelThongTin.Size = new System.Drawing.Size(950, 180);
          this.panelThongTin.TabIndex = 1;
  // 
         // lblDoanhThu
     // 
    this.lblDoanhThu.AutoSize = true;
   this.lblDoanhThu.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblDoanhThu.ForeColor = System.Drawing.Color.Green;
       this.lblDoanhThu.Location = new System.Drawing.Point(180, 140);
            this.lblDoanhThu.Name = "lblDoanhThu";
   this.lblDoanhThu.Size = new System.Drawing.Size(45, 20);
    this.lblDoanhThu.TabIndex = 9;
          this.lblDoanhThu.Text = "0 ?";
            // 
     // label5
    // 
   this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Segoe UI", 11F);
 this.label5.Location = new System.Drawing.Point(20, 140);
          this.label5.Name = "label5";
   this.label5.Size = new System.Drawing.Size(88, 20);
     this.label5.TabIndex = 8;
    this.label5.Text = "Doanh thu:";
          // 
  // lblDiaChi
   // 
this.lblDiaChi.AutoSize = true;
     this.lblDiaChi.Font = new System.Drawing.Font("Segoe UI", 11F);
 this.lblDiaChi.Location = new System.Drawing.Point(180, 110);
 this.lblDiaChi.Name = "lblDiaChi";
     this.lblDiaChi.Size = new System.Drawing.Size(79, 20);
 this.lblDiaChi.TabIndex = 7;
   this.lblDiaChi.Text = "[Ch?a có]";
      // 
            // label4
            // 
   this.label4.AutoSize = true;
     this.label4.Font = new System.Drawing.Font("Segoe UI", 11F);
this.label4.Location = new System.Drawing.Point(20, 110);
            this.label4.Name = "label4";
          this.label4.Size = new System.Drawing.Size(60, 20);
  this.label4.TabIndex = 6;
      this.label4.Text = "??a ch?:";
            // 
     // lblSDT
         // 
this.lblSDT.AutoSize = true;
   this.lblSDT.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblSDT.Location = new System.Drawing.Point(180, 80);
            this.lblSDT.Name = "lblSDT";
        this.lblSDT.Size = new System.Drawing.Size(79, 20);
         this.lblSDT.TabIndex = 5;
    this.lblSDT.Text = "[Ch?a có]";
      // 
 // label3
     // 
            this.label3.AutoSize = true;
 this.label3.Font = new System.Drawing.Font("Segoe UI", 11F);
 this.label3.Location = new System.Drawing.Point(20, 80);
       this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(105, 20);
            this.label3.TabIndex = 4;
     this.label3.Text = "S? ?i?n tho?i:";
            // 
// lblTenKH
         // 
        this.lblTenKH.AutoSize = true;
            this.lblTenKH.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
  this.lblTenKH.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblTenKH.Location = new System.Drawing.Point(180, 50);
  this.lblTenKH.Name = "lblTenKH";
       this.lblTenKH.Size = new System.Drawing.Size(133, 20);
   this.lblTenKH.TabIndex = 3;
    this.lblTenKH.Text = "Tên khách hàng";
       // 
            // label2
            // 
       this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label2.Location = new System.Drawing.Point(20, 50);
   this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 20);
            this.label2.TabIndex = 2;
  this.label2.Text = "Tên khách hàng:";
     // 
  // lblMaKH
      // 
            this.lblMaKH.AutoSize = true;
     this.lblMaKH.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
       this.lblMaKH.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblMaKH.Location = new System.Drawing.Point(180, 20);
        this.lblMaKH.Name = "lblMaKH";
            this.lblMaKH.Size = new System.Drawing.Size(60, 20);
        this.lblMaKH.TabIndex = 1;
         this.lblMaKH.Text = "KH001";
     // 
            // label1
            // 
    this.label1.AutoSize = true;
   this.label1.Font = new System.Drawing.Font("Segoe UI", 11F);
          this.label1.Location = new System.Drawing.Point(20, 20);
            this.label1.Name = "label1";
     this.label1.Size = new System.Drawing.Size(120, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Mã khách hàng:";
     // 
            // label6
     // 
   this.label6.AutoSize = true;
          this.label6.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.SteelBlue;
   this.label6.Location = new System.Drawing.Point(25, 280);
       this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(156, 21);
   this.label6.TabIndex = 2;
            this.label6.Text = "L?ch S? Mua Hàng";
         // 
            // dgvLichSuMuaHang
   // 
  this.dgvLichSuMuaHang.AllowUserToAddRows = false;
  this.dgvLichSuMuaHang.AllowUserToDeleteRows = false;
     this.dgvLichSuMuaHang.BackgroundColor = System.Drawing.Color.White;
        dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
  dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.SteelBlue;
         dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
       dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLichSuMuaHang.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
        this.dgvLichSuMuaHang.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
       this.dgvLichSuMuaHang.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
        this.STT,
     this.MaHoaDon,
    this.NgayLap,
            this.TongTien,
        this.PhuongThucTT,
   this.XemChiTiet});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
     dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
   dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 10F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
    dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
       dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
        this.dgvLichSuMuaHang.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvLichSuMuaHang.Location = new System.Drawing.Point(25, 310);
            this.dgvLichSuMuaHang.Name = "dgvLichSuMuaHang";
       this.dgvLichSuMuaHang.ReadOnly = true;
            this.dgvLichSuMuaHang.RowHeadersVisible = false;
        this.dgvLichSuMuaHang.RowTemplate.Height = 30;
this.dgvLichSuMuaHang.Size = new System.Drawing.Size(950, 340);
   this.dgvLichSuMuaHang.TabIndex = 3;
 this.dgvLichSuMuaHang.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLichSuMuaHang_CellClick);
     // 
   // STT
        // 
            this.STT.HeaderText = "STT";
      this.STT.Name = "STT";
    this.STT.ReadOnly = true;
            this.STT.Width = 60;
            // 
     // MaHoaDon
      // 
            this.MaHoaDon.HeaderText = "Mã Hóa ??n";
 this.MaHoaDon.Name = "MaHoaDon";
    this.MaHoaDon.ReadOnly = true;
            this.MaHoaDon.Width = 150;
  // 
  // NgayLap
 // 
this.NgayLap.HeaderText = "Ngày L?p";
        this.NgayLap.Name = "NgayLap";
            this.NgayLap.ReadOnly = true;
            this.NgayLap.Width = 180;
            // 
            // TongTien
    // 
       this.TongTien.HeaderText = "T?ng Ti?n";
        this.TongTien.Name = "TongTien";
   this.TongTien.ReadOnly = true;
            this.TongTien.Width = 150;
     // 
  // PhuongThucTT
      // 
         this.PhuongThucTT.HeaderText = "Ph??ng Th?c TT";
      this.PhuongThucTT.Name = "PhuongThucTT";
  this.PhuongThucTT.ReadOnly = true;
            this.PhuongThucTT.Width = 200;
          // 
   // XemChiTiet
       // 
            this.XemChiTiet.HeaderText = "Chi Ti?t";
            this.XemChiTiet.Name = "XemChiTiet";
    this.XemChiTiet.ReadOnly = true;
   this.XemChiTiet.Text = "Xem";
       this.XemChiTiet.UseColumnTextForButtonValue = true;
        this.XemChiTiet.Width = 150;
// 
          // frmChiTietKhachHang
     // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = System.Drawing.Color.WhiteSmoke;
        this.ClientSize = new System.Drawing.Size(1000, 670);
    this.Controls.Add(this.dgvLichSuMuaHang);
        this.Controls.Add(this.label6);
            this.Controls.Add(this.panelThongTin);
 this.Controls.Add(this.panelHeader);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
  this.MaximizeBox = false;
    this.MinimizeBox = false;
       this.Name = "frmChiTietKhachHang";
       this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
this.Text = "Chi Ti?t Khách Hàng";
            this.Load += new System.EventHandler(this.frmChiTietKhachHang_Load);
  this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
          this.panelThongTin.ResumeLayout(false);
            this.panelThongTin.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLichSuMuaHang)).EndInit();
    this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panelThongTin;
        private System.Windows.Forms.Label lblMaKH;
      private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblTenKH;
  private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblSDT;
        private System.Windows.Forms.Label label3;
     private System.Windows.Forms.Label lblDiaChi;
     private System.Windows.Forms.Label label4;
  private System.Windows.Forms.Label lblDoanhThu;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DataGridView dgvLichSuMuaHang;
  private System.Windows.Forms.DataGridViewTextBoxColumn STT;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaHoaDon;
        private System.Windows.Forms.DataGridViewTextBoxColumn NgayLap;
        private System.Windows.Forms.DataGridViewTextBoxColumn TongTien;
        private System.Windows.Forms.DataGridViewTextBoxColumn PhuongThucTT;
    private System.Windows.Forms.DataGridViewButtonColumn XemChiTiet;
    }
}
