namespace OpenMLTD.ManifestTools.UI
{
    partial class FManifestDownload
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
            this.label1 = new System.Windows.Forms.Label();
            this.cboHost = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtAssetVersion = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cboResVersion = new System.Windows.Forms.ComboBox();
            this.radResManual = new System.Windows.Forms.RadioButton();
            this.radResLatest = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.radPlatformIos = new System.Windows.Forms.RadioButton();
            this.radPlatformAndroid = new System.Windows.Forms.RadioButton();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Host:";
            // 
            // cboHost
            // 
            this.cboHost.FormattingEnabled = true;
            this.cboHost.Items.AddRange(new object[] {
            "td-assets.bn765.com",
            "d2sf4w9bkv485c.cloudfront.net"});
            this.cboHost.Location = new System.Drawing.Point(131, 16);
            this.cboHost.Name = "cboHost";
            this.cboHost.Size = new System.Drawing.Size(198, 21);
            this.cboHost.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Unity Asset Version:";
            // 
            // txtAssetVersion
            // 
            this.txtAssetVersion.Location = new System.Drawing.Point(131, 43);
            this.txtAssetVersion.Name = "txtAssetVersion";
            this.txtAssetVersion.Size = new System.Drawing.Size(67, 20);
            this.txtAssetVersion.TabIndex = 3;
            this.txtAssetVersion.Text = "2018";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Resource Version:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cboResVersion);
            this.panel1.Controls.Add(this.radResManual);
            this.panel1.Controls.Add(this.radResLatest);
            this.panel1.Location = new System.Drawing.Point(131, 69);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(217, 45);
            this.panel1.TabIndex = 5;
            // 
            // cboResVersion
            // 
            this.cboResVersion.Enabled = false;
            this.cboResVersion.FormattingEnabled = true;
            this.cboResVersion.Location = new System.Drawing.Point(72, 22);
            this.cboResVersion.Name = "cboResVersion";
            this.cboResVersion.Size = new System.Drawing.Size(126, 21);
            this.cboResVersion.TabIndex = 2;
            // 
            // radResManual
            // 
            this.radResManual.AutoSize = true;
            this.radResManual.Location = new System.Drawing.Point(3, 23);
            this.radResManual.Name = "radResManual";
            this.radResManual.Size = new System.Drawing.Size(63, 17);
            this.radResManual.TabIndex = 1;
            this.radResManual.Text = "Specify:";
            this.radResManual.UseVisualStyleBackColor = true;
            // 
            // radResLatest
            // 
            this.radResLatest.AutoSize = true;
            this.radResLatest.Checked = true;
            this.radResLatest.Location = new System.Drawing.Point(3, 0);
            this.radResLatest.Name = "radResLatest";
            this.radResLatest.Size = new System.Drawing.Size(54, 17);
            this.radResLatest.TabIndex = 0;
            this.radResLatest.TabStop = true;
            this.radResLatest.Text = "Latest";
            this.radResLatest.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(131, 182);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(105, 25);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(243, 182);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(105, 25);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 129);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Platform:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.radPlatformIos);
            this.panel2.Controls.Add(this.radPlatformAndroid);
            this.panel2.Location = new System.Drawing.Point(131, 120);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(217, 32);
            this.panel2.TabIndex = 9;
            // 
            // radPlatformIos
            // 
            this.radPlatformIos.AutoSize = true;
            this.radPlatformIos.Location = new System.Drawing.Point(112, 7);
            this.radPlatformIos.Name = "radPlatformIos";
            this.radPlatformIos.Size = new System.Drawing.Size(42, 17);
            this.radPlatformIos.TabIndex = 3;
            this.radPlatformIos.Text = "iOS";
            this.radPlatformIos.UseVisualStyleBackColor = true;
            // 
            // radPlatformAndroid
            // 
            this.radPlatformAndroid.AutoSize = true;
            this.radPlatformAndroid.Checked = true;
            this.radPlatformAndroid.Location = new System.Drawing.Point(3, 7);
            this.radPlatformAndroid.Name = "radPlatformAndroid";
            this.radPlatformAndroid.Size = new System.Drawing.Size(61, 17);
            this.radPlatformAndroid.TabIndex = 2;
            this.radPlatformAndroid.TabStop = true;
            this.radPlatformAndroid.Text = "Android";
            this.radPlatformAndroid.UseVisualStyleBackColor = true;
            // 
            // FManifestDownload
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(363, 219);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtAssetVersion);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cboHost);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FManifestDownload";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Download Manifest";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboHost;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtAssetVersion;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox cboResVersion;
        private System.Windows.Forms.RadioButton radResManual;
        private System.Windows.Forms.RadioButton radResLatest;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton radPlatformIos;
        private System.Windows.Forms.RadioButton radPlatformAndroid;
    }
}