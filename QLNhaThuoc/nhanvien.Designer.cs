namespace QLNhaThuoc
{
    partial class nhanvien
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
            flpnv = new System.Windows.Forms.FlowLayoutPanel();
            btnloctheo = new DevExpress.XtraEditors.SimpleButton();
            txttimkiem = new DevExpress.XtraEditors.TextEdit();
            btnloc = new DevExpress.XtraEditors.SimpleButton();
            flpheader = new System.Windows.Forms.FlowLayoutPanel();
            btnthem = new System.Windows.Forms.Button();
            btnsua = new System.Windows.Forms.Button();
            btnxoa = new System.Windows.Forms.Button();
            lblheader = new DevExpress.XtraEditors.LabelControl();
            pnlnv = new DevExpress.XtraEditors.PanelControl();
            gridControlnv = new DevExpress.XtraGrid.GridControl();
            gridViewnv = new DevExpress.XtraGrid.Views.Grid.GridView();
            pceloctheo = new DevExpress.XtraEditors.PopupContainerEdit();
            popuploctheo = new DevExpress.XtraEditors.PopupContainerControl();
            checkEdit1 = new DevExpress.XtraEditors.CheckEdit();
            lblTitle = new DevExpress.XtraEditors.LabelControl();
            flpnv.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)txttimkiem.Properties).BeginInit();
            flpheader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlnv).BeginInit();
            pnlnv.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridControlnv).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridViewnv).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pceloctheo.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)popuploctheo).BeginInit();
            popuploctheo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)checkEdit1.Properties).BeginInit();
            SuspendLayout();
            // 
            // flpnv
            // 
            flpnv.Anchor = System.Windows.Forms.AnchorStyles.None;
            flpnv.BackColor = System.Drawing.Color.White;
            flpnv.Controls.Add(btnloctheo);
            flpnv.Controls.Add(txttimkiem);
            flpnv.Controls.Add(btnloc);
            flpnv.Location = new System.Drawing.Point(5, 0);
            flpnv.Name = "flpnv";
            flpnv.Size = new System.Drawing.Size(2078, 52);
            flpnv.TabIndex = 1;
            // 
            // btnloctheo
            // 
            btnloctheo.Location = new System.Drawing.Point(3, 3);
            btnloctheo.Name = "btnloctheo";
            btnloctheo.Size = new System.Drawing.Size(35, 35);
            btnloctheo.TabIndex = 6;
            btnloctheo.ToolTip = "Nhấn vào đây để thay đổi thông tin cần tìm kiếm.";
            btnloctheo.Click += btnloctheo_Click;
            // 
            // txttimkiem
            // 
            txttimkiem.Location = new System.Drawing.Point(44, 3);
            txttimkiem.Name = "txttimkiem";
            txttimkiem.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            txttimkiem.Properties.Appearance.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            txttimkiem.Properties.Appearance.Options.UseFont = true;
            txttimkiem.Size = new System.Drawing.Size(422, 28);
            txttimkiem.TabIndex = 0;
            txttimkiem.ToolTipIconType = DevExpress.Utils.ToolTipIconType.Information;
            // 
            // btnloc
            // 
            btnloc.Appearance.BorderColor = System.Drawing.Color.DarkGray;
            btnloc.Appearance.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            btnloc.Appearance.Options.UseBorderColor = true;
            btnloc.Appearance.Options.UseFont = true;
            btnloc.AppearanceDisabled.BorderColor = System.Drawing.Color.DarkGray;
            btnloc.AppearanceDisabled.Options.UseBorderColor = true;
            btnloc.Location = new System.Drawing.Point(472, 3);
            btnloc.Name = "btnloc";
            btnloc.Size = new System.Drawing.Size(83, 28);
            btnloc.TabIndex = 1;
            btnloc.Text = "simpleButton1";
            btnloc.ToolTip = "Lọc";
            btnloc.Click += btnloc_Click;
            // 
            // flpheader
            // 
            flpheader.Controls.Add(btnthem);
            flpheader.Controls.Add(btnsua);
            flpheader.Controls.Add(btnxoa);
            flpheader.Location = new System.Drawing.Point(1491, 12);
            flpheader.Name = "flpheader";
            flpheader.Size = new System.Drawing.Size(432, 52);
            flpheader.TabIndex = 7;
            // 
            // btnthem
            // 
            btnthem.BackColor = System.Drawing.Color.FromArgb(66, 144, 242);
            btnthem.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            btnthem.ForeColor = System.Drawing.Color.White;
            btnthem.Location = new System.Drawing.Point(3, 3);
            btnthem.Name = "btnthem";
            btnthem.Size = new System.Drawing.Size(114, 40);
            btnthem.TabIndex = 0;
            btnthem.Text = "Thêm mới";
            btnthem.UseVisualStyleBackColor = false;
            btnthem.Click += btnthem_Click;
            // 
            // btnsua
            // 
            btnsua.BackColor = System.Drawing.Color.FromArgb(66, 144, 242);
            btnsua.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            btnsua.ForeColor = System.Drawing.Color.White;
            btnsua.Location = new System.Drawing.Point(123, 3);
            btnsua.Name = "btnsua";
            btnsua.Size = new System.Drawing.Size(114, 40);
            btnsua.TabIndex = 1;
            btnsua.Text = "Sửa";
            btnsua.UseVisualStyleBackColor = false;
            btnsua.Click += btnsua_Click;
            // 
            // btnxoa
            // 
            btnxoa.BackColor = System.Drawing.Color.FromArgb(66, 144, 242);
            btnxoa.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            btnxoa.ForeColor = System.Drawing.Color.White;
            btnxoa.Location = new System.Drawing.Point(243, 3);
            btnxoa.Name = "btnxoa";
            btnxoa.Size = new System.Drawing.Size(114, 40);
            btnxoa.TabIndex = 2;
            btnxoa.Text = "Xóa";
            btnxoa.UseVisualStyleBackColor = false;
            // 
            // lblheader
            // 
            lblheader.Appearance.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            lblheader.Appearance.ForeColor = System.Drawing.Color.Black;
            lblheader.Appearance.Options.UseFont = true;
            lblheader.Appearance.Options.UseForeColor = true;
            lblheader.Location = new System.Drawing.Point(29, 26);
            lblheader.Name = "lblheader";
            lblheader.Size = new System.Drawing.Size(314, 33);
            lblheader.TabIndex = 8;
            lblheader.Text = "DANH MỤC NHÂN VIÊN";
            // 
            // pnlnv
            // 
            pnlnv.Appearance.BackColor = System.Drawing.Color.White;
            pnlnv.Appearance.Options.UseBackColor = true;
            pnlnv.Controls.Add(flpnv);
            pnlnv.Location = new System.Drawing.Point(-5, 88);
            pnlnv.Name = "pnlnv";
            pnlnv.Size = new System.Drawing.Size(1928, 52);
            pnlnv.TabIndex = 9;
            // 
            // gridControlnv
            // 
            gridControlnv.Location = new System.Drawing.Point(0, 146);
            gridControlnv.MainView = gridViewnv;
            gridControlnv.Name = "gridControlnv";
            gridControlnv.Size = new System.Drawing.Size(1923, 595);
            gridControlnv.TabIndex = 10;
            gridControlnv.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gridViewnv });
            // 
            // gridViewnv
            // 
            gridViewnv.GridControl = gridControlnv;
            gridViewnv.Name = "gridViewnv";
            // 
            // pceloctheo
            // 
            pceloctheo.Location = new System.Drawing.Point(0, 143);
            pceloctheo.Name = "pceloctheo";
            pceloctheo.Properties.Appearance.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            pceloctheo.Properties.Appearance.Options.UseFont = true;
            pceloctheo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            pceloctheo.Properties.PopupControl = popuploctheo;
            pceloctheo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            pceloctheo.Size = new System.Drawing.Size(237, 28);
            pceloctheo.TabIndex = 11;
            pceloctheo.Visible = false;
            // 
            // popuploctheo
            // 
            popuploctheo.Appearance.BackColor = System.Drawing.Color.White;
            popuploctheo.Appearance.Options.UseBackColor = true;
            popuploctheo.Controls.Add(checkEdit1);
            popuploctheo.Controls.Add(lblTitle);
            popuploctheo.Location = new System.Drawing.Point(0, 166);
            popuploctheo.Name = "popuploctheo";
            popuploctheo.Size = new System.Drawing.Size(237, 198);
            popuploctheo.TabIndex = 12;
            // 
            // checkEdit1
            // 
            checkEdit1.Location = new System.Drawing.Point(23, 33);
            checkEdit1.Name = "checkEdit1";
            checkEdit1.Properties.Caption = "checkEdit1";
            checkEdit1.Size = new System.Drawing.Size(184, 27);
            checkEdit1.TabIndex = 7;
            // 
            // lblTitle
            // 
            lblTitle.Appearance.ForeColor = System.Drawing.Color.Black;
            lblTitle.Appearance.Options.UseForeColor = true;
            lblTitle.Location = new System.Drawing.Point(70, 8);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new System.Drawing.Size(94, 19);
            lblTitle.TabIndex = 7;
            lblTitle.Text = "labelControl1";
            // 
            // nhanvien
            // 
            Appearance.BackColor = System.Drawing.Color.FromArgb(246, 247, 249);
            Appearance.Options.UseBackColor = true;
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1923, 741);
            Controls.Add(popuploctheo);
            Controls.Add(pceloctheo);
            Controls.Add(gridControlnv);
            Controls.Add(pnlnv);
            Controls.Add(lblheader);
            Controls.Add(flpheader);
            Name = "nhanvien";
            Text = "Danh mục nhà cung cấp";
            Load += nhanvien_Load;
            flpnv.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)txttimkiem.Properties).EndInit();
            flpheader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pnlnv).EndInit();
            pnlnv.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridControlnv).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridViewnv).EndInit();
            ((System.ComponentModel.ISupportInitialize)pceloctheo.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)popuploctheo).EndInit();
            popuploctheo.ResumeLayout(false);
            popuploctheo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)checkEdit1.Properties).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpnv;
        private DevExpress.XtraEditors.SimpleButton btnloctheo;
        private DevExpress.XtraEditors.TextEdit txttimkiem;
        private DevExpress.XtraEditors.SimpleButton btnloc;
        private System.Windows.Forms.FlowLayoutPanel flpheader;
        private System.Windows.Forms.Button btnthem;
        private System.Windows.Forms.Button btnsua;
        private System.Windows.Forms.Button btnxoa;
        private DevExpress.XtraEditors.LabelControl lblheader;
        private DevExpress.XtraEditors.PanelControl pnlnv;
        private DevExpress.XtraGrid.GridControl gridControlnv;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewnv;
        private DevExpress.XtraEditors.PopupContainerEdit pceloctheo;
        private DevExpress.XtraEditors.PopupContainerControl popuploctheo;
        private DevExpress.XtraEditors.CheckEdit checkEdit1;
        private DevExpress.XtraEditors.LabelControl lblTitle;
    }
}