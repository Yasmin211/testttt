namespace QLNhaThuoc
{
    partial class nhaphang
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
            lblheader = new DevExpress.XtraEditors.LabelControl();
            pnlnhaphang = new DevExpress.XtraEditors.PanelControl();
            flpnhaphang = new System.Windows.Forms.FlowLayoutPanel();
            btnloctheo = new DevExpress.XtraEditors.SimpleButton();
            txttimkiem = new DevExpress.XtraEditors.TextEdit();
            btnloc = new DevExpress.XtraEditors.SimpleButton();
            lblthoigian = new DevExpress.XtraEditors.LabelControl();
            btntailai = new DevExpress.XtraEditors.SimpleButton();
            btnxuat = new DevExpress.XtraEditors.SimpleButton();
            btncaidat = new DevExpress.XtraEditors.SimpleButton();
            flpheader = new System.Windows.Forms.FlowLayoutPanel();
            btnthem = new System.Windows.Forms.Button();
            btnsua = new System.Windows.Forms.Button();
            btnxoa = new System.Windows.Forms.Button();
            gridControlnhaphang = new DevExpress.XtraGrid.GridControl();
            gridViewnhaphang = new DevExpress.XtraGrid.Views.Grid.GridView();
            pceloctheo = new DevExpress.XtraEditors.PopupContainerEdit();
            popuploctheo = new DevExpress.XtraEditors.PopupContainerControl();
            checkEdit1 = new DevExpress.XtraEditors.CheckEdit();
            labelControl1 = new DevExpress.XtraEditors.LabelControl();
            pcelocchitiet = new DevExpress.XtraEditors.PopupContainerEdit();
            popuplocchitiet = new DevExpress.XtraEditors.PopupContainerControl();
            lbltitle = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)pnlnhaphang).BeginInit();
            pnlnhaphang.SuspendLayout();
            flpnhaphang.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)txttimkiem.Properties).BeginInit();
            flpheader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridControlnhaphang).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridViewnhaphang).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pceloctheo.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)popuploctheo).BeginInit();
            popuploctheo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)checkEdit1.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pcelocchitiet.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)popuplocchitiet).BeginInit();
            popuplocchitiet.SuspendLayout();
            SuspendLayout();
            // 
            // lblheader
            // 
            lblheader.Appearance.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            lblheader.Appearance.ForeColor = System.Drawing.Color.Black;
            lblheader.Appearance.Options.UseFont = true;
            lblheader.Appearance.Options.UseForeColor = true;
            lblheader.Location = new System.Drawing.Point(29, 26);
            lblheader.Name = "lblheader";
            lblheader.Size = new System.Drawing.Size(298, 33);
            lblheader.TabIndex = 0;
            lblheader.Text = "QUẢN LÝ NHẬP HÀNG";
            // 
            // pnlnhaphang
            // 
            pnlnhaphang.Appearance.BackColor = System.Drawing.Color.White;
            pnlnhaphang.Appearance.BorderColor = System.Drawing.Color.White;
            pnlnhaphang.Appearance.ForeColor = System.Drawing.Color.White;
            pnlnhaphang.Appearance.Options.UseBackColor = true;
            pnlnhaphang.Appearance.Options.UseBorderColor = true;
            pnlnhaphang.Appearance.Options.UseForeColor = true;
            pnlnhaphang.Controls.Add(flpnhaphang);
            pnlnhaphang.Location = new System.Drawing.Point(0, 81);
            pnlnhaphang.LookAndFeel.SkinName = "Office 2019 White";
            pnlnhaphang.LookAndFeel.UseDefaultLookAndFeel = false;
            pnlnhaphang.Name = "pnlnhaphang";
            pnlnhaphang.Size = new System.Drawing.Size(1927, 62);
            pnlnhaphang.TabIndex = 2;
            // 
            // flpnhaphang
            // 
            flpnhaphang.Anchor = System.Windows.Forms.AnchorStyles.None;
            flpnhaphang.Controls.Add(btnloctheo);
            flpnhaphang.Controls.Add(txttimkiem);
            flpnhaphang.Controls.Add(btnloc);
            flpnhaphang.Controls.Add(lblthoigian);
            flpnhaphang.Controls.Add(btntailai);
            flpnhaphang.Controls.Add(btnxuat);
            flpnhaphang.Controls.Add(btncaidat);
            flpnhaphang.Location = new System.Drawing.Point(0, 5);
            flpnhaphang.Name = "flpnhaphang";
            flpnhaphang.Size = new System.Drawing.Size(1927, 52);
            flpnhaphang.TabIndex = 0;
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
            // lblthoigian
            // 
            lblthoigian.Appearance.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lblthoigian.Appearance.ForeColor = System.Drawing.Color.FromArgb(123, 177, 241);
            lblthoigian.Appearance.Options.UseFont = true;
            lblthoigian.Appearance.Options.UseForeColor = true;
            lblthoigian.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Horizontal;
            lblthoigian.Location = new System.Drawing.Point(561, 3);
            lblthoigian.Name = "lblthoigian";
            lblthoigian.Size = new System.Drawing.Size(71, 18);
            lblthoigian.TabIndex = 2;
            lblthoigian.Text = "Ngày HĐ: ";
            // 
            // btntailai
            // 
            btntailai.Location = new System.Drawing.Point(638, 3);
            btntailai.Name = "btntailai";
            btntailai.Size = new System.Drawing.Size(35, 35);
            btntailai.TabIndex = 3;
            btntailai.ToolTip = "Lấy lại dữ liệu";
            btntailai.Click += btntailai_Click;
            // 
            // btnxuat
            // 
            btnxuat.Location = new System.Drawing.Point(679, 3);
            btnxuat.Name = "btnxuat";
            btnxuat.Size = new System.Drawing.Size(35, 35);
            btnxuat.TabIndex = 5;
            btnxuat.ToolTip = "Xuất khẩu chi tiết hóa đơn";
            // 
            // btncaidat
            // 
            btncaidat.Location = new System.Drawing.Point(720, 3);
            btncaidat.Name = "btncaidat";
            btncaidat.Size = new System.Drawing.Size(35, 35);
            btncaidat.TabIndex = 4;
            btncaidat.ToolTip = "Thiết lập cột hiển thị";
            btncaidat.Click += btncaidat_Click;
            // 
            // flpheader
            // 
            flpheader.Controls.Add(btnthem);
            flpheader.Controls.Add(btnsua);
            flpheader.Controls.Add(btnxoa);
            flpheader.Location = new System.Drawing.Point(1495, 12);
            flpheader.Name = "flpheader";
            flpheader.Size = new System.Drawing.Size(432, 52);
            flpheader.TabIndex = 3;
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
            // gridControlnhaphang
            // 
            gridControlnhaphang.Location = new System.Drawing.Point(0, 144);
            gridControlnhaphang.MainView = gridViewnhaphang;
            gridControlnhaphang.Name = "gridControlnhaphang";
            gridControlnhaphang.Size = new System.Drawing.Size(1927, 599);
            gridControlnhaphang.TabIndex = 4;
            gridControlnhaphang.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gridViewnhaphang });
            // 
            // gridViewnhaphang
            // 
            gridViewnhaphang.Appearance.HeaderPanel.Options.UseTextOptions = true;
            gridViewnhaphang.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridViewnhaphang.GridControl = gridControlnhaphang;
            gridViewnhaphang.Name = "gridViewnhaphang";
            gridViewnhaphang.OptionsBehavior.Editable = false;
            gridViewnhaphang.OptionsSelection.MultiSelect = true;
            gridViewnhaphang.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            gridViewnhaphang.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.True;
            gridViewnhaphang.OptionsView.EnableAppearanceEvenRow = true;
            gridViewnhaphang.OptionsView.ShowGroupPanel = false;
            gridViewnhaphang.RowHeight = 30;
            // 
            // pceloctheo
            // 
            pceloctheo.Location = new System.Drawing.Point(3, 149);
            pceloctheo.Name = "pceloctheo";
            pceloctheo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            pceloctheo.Properties.PopupControl = popuploctheo;
            pceloctheo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            pceloctheo.Size = new System.Drawing.Size(237, 26);
            pceloctheo.TabIndex = 5;
            pceloctheo.Visible = false;
            // 
            // popuploctheo
            // 
            popuploctheo.Controls.Add(checkEdit1);
            popuploctheo.Controls.Add(labelControl1);
            popuploctheo.Location = new System.Drawing.Point(3, 173);
            popuploctheo.Name = "popuploctheo";
            popuploctheo.Size = new System.Drawing.Size(237, 198);
            popuploctheo.TabIndex = 6;
            // 
            // checkEdit1
            // 
            checkEdit1.Location = new System.Drawing.Point(23, 33);
            checkEdit1.Name = "checkEdit1";
            checkEdit1.Properties.Caption = "checkEdit1";
            checkEdit1.Size = new System.Drawing.Size(184, 27);
            checkEdit1.TabIndex = 7;
            // 
            // labelControl1
            // 
            labelControl1.Appearance.ForeColor = System.Drawing.Color.Black;
            labelControl1.Appearance.Options.UseForeColor = true;
            labelControl1.Location = new System.Drawing.Point(70, 8);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new System.Drawing.Size(94, 19);
            labelControl1.TabIndex = 7;
            labelControl1.Text = "labelControl1";
            // 
            // pcelocchitiet
            // 
            pcelocchitiet.Location = new System.Drawing.Point(472, 144);
            pcelocchitiet.Name = "pcelocchitiet";
            pcelocchitiet.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            pcelocchitiet.Properties.PopupControl = popuplocchitiet;
            pcelocchitiet.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            pcelocchitiet.Size = new System.Drawing.Size(565, 26);
            pcelocchitiet.TabIndex = 7;
            pcelocchitiet.Visible = false;
            // 
            // popuplocchitiet
            // 
            popuplocchitiet.Controls.Add(lbltitle);
            popuplocchitiet.Location = new System.Drawing.Point(472, 173);
            popuplocchitiet.Name = "popuplocchitiet";
            popuplocchitiet.Size = new System.Drawing.Size(565, 324);
            popuplocchitiet.TabIndex = 8;
            // 
            // lbltitle
            // 
            lbltitle.Appearance.ForeColor = System.Drawing.Color.Black;
            lbltitle.Appearance.Options.UseForeColor = true;
            lbltitle.Location = new System.Drawing.Point(70, 8);
            lbltitle.Name = "lbltitle";
            lbltitle.Size = new System.Drawing.Size(94, 19);
            lbltitle.TabIndex = 7;
            lbltitle.Text = "labelControl2";
            // 
            // nhaphang
            // 
            Appearance.BackColor = System.Drawing.Color.FromArgb(246, 247, 249);
            Appearance.BorderColor = System.Drawing.Color.FromArgb(246, 247, 249);
            Appearance.ForeColor = System.Drawing.Color.FromArgb(246, 247, 249);
            Appearance.Options.UseBackColor = true;
            Appearance.Options.UseBorderColor = true;
            Appearance.Options.UseFont = true;
            Appearance.Options.UseForeColor = true;
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1923, 741);
            Controls.Add(popuplocchitiet);
            Controls.Add(pcelocchitiet);
            Controls.Add(pceloctheo);
            Controls.Add(popuploctheo);
            Controls.Add(gridControlnhaphang);
            Controls.Add(flpheader);
            Controls.Add(pnlnhaphang);
            Controls.Add(lblheader);
            Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            Name = "nhaphang";
            Text = "Nhập Hàng";
            Load += nhaphang_Load;
            ((System.ComponentModel.ISupportInitialize)pnlnhaphang).EndInit();
            pnlnhaphang.ResumeLayout(false);
            flpnhaphang.ResumeLayout(false);
            flpnhaphang.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)txttimkiem.Properties).EndInit();
            flpheader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridControlnhaphang).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridViewnhaphang).EndInit();
            ((System.ComponentModel.ISupportInitialize)pceloctheo.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)popuploctheo).EndInit();
            popuploctheo.ResumeLayout(false);
            popuploctheo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)checkEdit1.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)pcelocchitiet.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)popuplocchitiet).EndInit();
            popuplocchitiet.ResumeLayout(false);
            popuplocchitiet.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblheader;
        private DevExpress.XtraEditors.PanelControl pnlnhaphang;
        private System.Windows.Forms.FlowLayoutPanel flpnhaphang;
        private DevExpress.XtraEditors.TextEdit txttimkiem;
        private DevExpress.XtraEditors.SimpleButton btnloc;
        private System.Windows.Forms.FlowLayoutPanel flpheader;
        private System.Windows.Forms.Button btnthem;
        private System.Windows.Forms.Button btnsua;
        private System.Windows.Forms.Button btnxoa;
        private DevExpress.XtraEditors.LabelControl lblthoigian;
        private DevExpress.XtraEditors.SimpleButton btntailai;
        private DevExpress.XtraEditors.SimpleButton btnxuat;
        private DevExpress.XtraEditors.SimpleButton btncaidat;
        private DevExpress.XtraGrid.GridControl gridControlnhaphang;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewnhaphang;
        private DevExpress.XtraEditors.SimpleButton btnloctheo;
        private DevExpress.XtraEditors.PopupContainerEdit pceloctheo;
        private DevExpress.XtraEditors.PopupContainerControl popuploctheo;
        private DevExpress.XtraEditors.CheckEdit checkEdit1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.PopupContainerEdit pcelocchitiet;
        private DevExpress.XtraEditors.PopupContainerControl popuplocchitiet;
        private DevExpress.XtraEditors.LabelControl lbltitle;
    }
}