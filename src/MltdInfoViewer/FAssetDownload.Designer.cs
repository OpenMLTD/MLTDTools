namespace MltdInfoViewer {
    partial class FAssetDownload {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new System.Windows.Forms.Label();
            this.cboServer = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtAssetVersion = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUnityHeaderVersion = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtDestinationFolder = new System.Windows.Forms.TextBox();
            this.txtUnityAssetVersion = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cboPlatform = new System.Windows.Forms.ComboBox();
            this.btnBrowseDestinationFolder = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.lvEntries = new System.Windows.Forms.ListView();
            this.btnGo = new System.Windows.Forms.Button();
            this.btnAbort = new System.Windows.Forms.Button();
            this.pgbProgress = new System.Windows.Forms.ProgressBar();
            this.lblProgress = new System.Windows.Forms.Label();
            this.lblEntryStats = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server:";
            // 
            // cboServer
            // 
            this.cboServer.FormattingEnabled = true;
            this.cboServer.Items.AddRange(new object[] {
            "d2sf4w9bkv485c.cloudfront.net",
            "td-assets.bn765.com"});
            this.cboServer.Location = new System.Drawing.Point(77, 12);
            this.cboServer.Name = "cboServer";
            this.cboServer.Size = new System.Drawing.Size(277, 20);
            this.cboServer.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Version:";
            // 
            // txtAssetVersion
            // 
            this.txtAssetVersion.Location = new System.Drawing.Point(77, 38);
            this.txtAssetVersion.Name = "txtAssetVersion";
            this.txtAssetVersion.Size = new System.Drawing.Size(124, 21);
            this.txtAssetVersion.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "Unity:";
            // 
            // txtUnityHeaderVersion
            // 
            this.txtUnityHeaderVersion.Location = new System.Drawing.Point(77, 65);
            this.txtUnityHeaderVersion.Name = "txtUnityHeaderVersion";
            this.txtUnityHeaderVersion.Size = new System.Drawing.Size(124, 21);
            this.txtUnityHeaderVersion.TabIndex = 5;
            this.txtUnityHeaderVersion.Text = "2017.4.0f1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 148);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "Folder:";
            // 
            // txtDestinationFolder
            // 
            this.txtDestinationFolder.Location = new System.Drawing.Point(77, 145);
            this.txtDestinationFolder.Name = "txtDestinationFolder";
            this.txtDestinationFolder.Size = new System.Drawing.Size(232, 21);
            this.txtDestinationFolder.TabIndex = 7;
            // 
            // txtUnityAssetVersion
            // 
            this.txtUnityAssetVersion.Location = new System.Drawing.Point(77, 92);
            this.txtUnityAssetVersion.Name = "txtUnityAssetVersion";
            this.txtUnityAssetVersion.Size = new System.Drawing.Size(124, 21);
            this.txtUnityAssetVersion.TabIndex = 9;
            this.txtUnityAssetVersion.Text = "2017.3";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 95);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "Asset:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 122);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 12);
            this.label6.TabIndex = 10;
            this.label6.Text = "Platform:";
            // 
            // cboPlatform
            // 
            this.cboPlatform.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPlatform.FormattingEnabled = true;
            this.cboPlatform.Items.AddRange(new object[] {
            "Android",
            "iOS"});
            this.cboPlatform.Location = new System.Drawing.Point(77, 119);
            this.cboPlatform.Name = "cboPlatform";
            this.cboPlatform.Size = new System.Drawing.Size(124, 20);
            this.cboPlatform.TabIndex = 11;
            // 
            // btnBrowseDestinationFolder
            // 
            this.btnBrowseDestinationFolder.Location = new System.Drawing.Point(315, 145);
            this.btnBrowseDestinationFolder.Name = "btnBrowseDestinationFolder";
            this.btnBrowseDestinationFolder.Size = new System.Drawing.Size(39, 21);
            this.btnBrowseDestinationFolder.TabIndex = 12;
            this.btnBrowseDestinationFolder.Text = "...";
            this.btnBrowseDestinationFolder.UseVisualStyleBackColor = true;
            // 
            // lvEntries
            // 
            this.lvEntries.FullRowSelect = true;
            this.lvEntries.Location = new System.Drawing.Point(360, 12);
            this.lvEntries.MultiSelect = false;
            this.lvEntries.Name = "lvEntries";
            this.lvEntries.Size = new System.Drawing.Size(331, 239);
            this.lvEntries.TabIndex = 13;
            this.lvEntries.UseCompatibleStateImageBehavior = false;
            this.lvEntries.View = System.Windows.Forms.View.Details;
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(77, 183);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(104, 32);
            this.btnGo.TabIndex = 14;
            this.btnGo.Text = "&Go";
            this.btnGo.UseVisualStyleBackColor = true;
            // 
            // btnAbort
            // 
            this.btnAbort.Location = new System.Drawing.Point(187, 183);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(104, 32);
            this.btnAbort.TabIndex = 15;
            this.btnAbort.Text = "&Abort";
            this.btnAbort.UseVisualStyleBackColor = true;
            // 
            // pgbProgress
            // 
            this.pgbProgress.Location = new System.Drawing.Point(14, 251);
            this.pgbProgress.Name = "pgbProgress";
            this.pgbProgress.Size = new System.Drawing.Size(340, 23);
            this.pgbProgress.TabIndex = 16;
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(12, 236);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(53, 12);
            this.lblProgress.TabIndex = 17;
            this.lblProgress.Text = "Progress";
            this.lblProgress.Visible = false;
            // 
            // lblEntryStats
            // 
            this.lblEntryStats.AutoSize = true;
            this.lblEntryStats.Location = new System.Drawing.Point(360, 254);
            this.lblEntryStats.Name = "lblEntryStats";
            this.lblEntryStats.Size = new System.Drawing.Size(35, 12);
            this.lblEntryStats.TabIndex = 18;
            this.lblEntryStats.Text = "Stats";
            this.lblEntryStats.Visible = false;
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(14, 280);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(677, 138);
            this.txtLog.TabIndex = 19;
            // 
            // FAssetDownload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(703, 430);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.lblEntryStats);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.pgbProgress);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.lvEntries);
            this.Controls.Add(this.btnBrowseDestinationFolder);
            this.Controls.Add(this.cboPlatform);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtUnityAssetVersion);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtDestinationFolder);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtUnityHeaderVersion);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtAssetVersion);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cboServer);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FAssetDownload";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Download Asset(s)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboServer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtAssetVersion;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUnityHeaderVersion;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtDestinationFolder;
        private System.Windows.Forms.TextBox txtUnityAssetVersion;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cboPlatform;
        private System.Windows.Forms.Button btnBrowseDestinationFolder;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ListView lvEntries;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.ProgressBar pgbProgress;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Label lblEntryStats;
        private System.Windows.Forms.TextBox txtLog;
    }
}