namespace MltdInfoViewer {
    partial class Form1 {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnManifestReset = new System.Windows.Forms.Button();
            this.btnManifestFilterString = new System.Windows.Forms.Button();
            this.txtManifestFilter = new System.Windows.Forms.TextBox();
            this.lvwManifest = new System.Windows.Forms.ListView();
            this.btnSelectManifestDatabase = new System.Windows.Forms.Button();
            this.txtManifestDatabasePath = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lvwCards = new System.Windows.Forms.ListView();
            this.btnSelectCardsDatabase = new System.Windows.Forms.Button();
            this.txtCardsDatabasePath = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.lvwCostumes = new System.Windows.Forms.ListView();
            this.btnSelectCostumesDatabase = new System.Windows.Forms.Button();
            this.txtCostumesDatabasePath = new System.Windows.Forms.TextBox();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.btnManifestFilterRegex = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(704, 372);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnManifestFilterRegex);
            this.tabPage1.Controls.Add(this.btnManifestReset);
            this.tabPage1.Controls.Add(this.btnManifestFilterString);
            this.tabPage1.Controls.Add(this.txtManifestFilter);
            this.tabPage1.Controls.Add(this.lvwManifest);
            this.tabPage1.Controls.Add(this.btnSelectManifestDatabase);
            this.tabPage1.Controls.Add(this.txtManifestDatabasePath);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(696, 346);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Manifest";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnManifestReset
            // 
            this.btnManifestReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnManifestReset.Location = new System.Drawing.Point(619, 319);
            this.btnManifestReset.Name = "btnManifestReset";
            this.btnManifestReset.Size = new System.Drawing.Size(70, 21);
            this.btnManifestReset.TabIndex = 8;
            this.btnManifestReset.Text = "Reset";
            this.btnManifestReset.UseVisualStyleBackColor = true;
            // 
            // btnManifestFilterString
            // 
            this.btnManifestFilterString.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnManifestFilterString.Location = new System.Drawing.Point(467, 319);
            this.btnManifestFilterString.Name = "btnManifestFilterString";
            this.btnManifestFilterString.Size = new System.Drawing.Size(70, 21);
            this.btnManifestFilterString.TabIndex = 7;
            this.btnManifestFilterString.Text = "String";
            this.btnManifestFilterString.UseVisualStyleBackColor = true;
            // 
            // txtManifestFilter
            // 
            this.txtManifestFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtManifestFilter.Location = new System.Drawing.Point(6, 319);
            this.txtManifestFilter.Name = "txtManifestFilter";
            this.txtManifestFilter.Size = new System.Drawing.Size(455, 21);
            this.txtManifestFilter.TabIndex = 6;
            // 
            // lvwManifest
            // 
            this.lvwManifest.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwManifest.FullRowSelect = true;
            this.lvwManifest.Location = new System.Drawing.Point(5, 33);
            this.lvwManifest.Name = "lvwManifest";
            this.lvwManifest.Size = new System.Drawing.Size(684, 279);
            this.lvwManifest.TabIndex = 5;
            this.lvwManifest.UseCompatibleStateImageBehavior = false;
            this.lvwManifest.View = System.Windows.Forms.View.Details;
            // 
            // btnSelectManifestDatabase
            // 
            this.btnSelectManifestDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectManifestDatabase.Location = new System.Drawing.Point(622, 6);
            this.btnSelectManifestDatabase.Name = "btnSelectManifestDatabase";
            this.btnSelectManifestDatabase.Size = new System.Drawing.Size(70, 21);
            this.btnSelectManifestDatabase.TabIndex = 4;
            this.btnSelectManifestDatabase.Text = "Select...";
            this.btnSelectManifestDatabase.UseVisualStyleBackColor = true;
            // 
            // txtManifestDatabasePath
            // 
            this.txtManifestDatabasePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtManifestDatabasePath.Location = new System.Drawing.Point(5, 6);
            this.txtManifestDatabasePath.Name = "txtManifestDatabasePath";
            this.txtManifestDatabasePath.ReadOnly = true;
            this.txtManifestDatabasePath.Size = new System.Drawing.Size(611, 21);
            this.txtManifestDatabasePath.TabIndex = 3;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.lvwCards);
            this.tabPage2.Controls.Add(this.btnSelectCardsDatabase);
            this.tabPage2.Controls.Add(this.txtCardsDatabasePath);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(696, 346);
            this.tabPage2.TabIndex = 0;
            this.tabPage2.Text = "Cards";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lvwCards
            // 
            this.lvwCards.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwCards.FullRowSelect = true;
            this.lvwCards.Location = new System.Drawing.Point(6, 33);
            this.lvwCards.Name = "lvwCards";
            this.lvwCards.Size = new System.Drawing.Size(684, 307);
            this.lvwCards.TabIndex = 2;
            this.lvwCards.UseCompatibleStateImageBehavior = false;
            this.lvwCards.View = System.Windows.Forms.View.Details;
            // 
            // btnSelectCardsDatabase
            // 
            this.btnSelectCardsDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectCardsDatabase.Location = new System.Drawing.Point(623, 6);
            this.btnSelectCardsDatabase.Name = "btnSelectCardsDatabase";
            this.btnSelectCardsDatabase.Size = new System.Drawing.Size(70, 21);
            this.btnSelectCardsDatabase.TabIndex = 1;
            this.btnSelectCardsDatabase.Text = "Select...";
            this.btnSelectCardsDatabase.UseVisualStyleBackColor = true;
            // 
            // txtCardsDatabasePath
            // 
            this.txtCardsDatabasePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCardsDatabasePath.Location = new System.Drawing.Point(6, 6);
            this.txtCardsDatabasePath.Name = "txtCardsDatabasePath";
            this.txtCardsDatabasePath.ReadOnly = true;
            this.txtCardsDatabasePath.Size = new System.Drawing.Size(611, 21);
            this.txtCardsDatabasePath.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.lvwCostumes);
            this.tabPage3.Controls.Add(this.btnSelectCostumesDatabase);
            this.tabPage3.Controls.Add(this.txtCostumesDatabasePath);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(696, 346);
            this.tabPage3.TabIndex = 1;
            this.tabPage3.Text = "Costumes";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // lvwCostumes
            // 
            this.lvwCostumes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwCostumes.FullRowSelect = true;
            this.lvwCostumes.Location = new System.Drawing.Point(5, 33);
            this.lvwCostumes.Name = "lvwCostumes";
            this.lvwCostumes.Size = new System.Drawing.Size(684, 307);
            this.lvwCostumes.TabIndex = 5;
            this.lvwCostumes.UseCompatibleStateImageBehavior = false;
            this.lvwCostumes.View = System.Windows.Forms.View.Details;
            // 
            // btnSelectCostumesDatabase
            // 
            this.btnSelectCostumesDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectCostumesDatabase.Location = new System.Drawing.Point(622, 6);
            this.btnSelectCostumesDatabase.Name = "btnSelectCostumesDatabase";
            this.btnSelectCostumesDatabase.Size = new System.Drawing.Size(70, 21);
            this.btnSelectCostumesDatabase.TabIndex = 4;
            this.btnSelectCostumesDatabase.Text = "Select...";
            this.btnSelectCostumesDatabase.UseVisualStyleBackColor = true;
            // 
            // txtCostumesDatabasePath
            // 
            this.txtCostumesDatabasePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCostumesDatabasePath.Location = new System.Drawing.Point(5, 6);
            this.txtCostumesDatabasePath.Name = "txtCostumesDatabasePath";
            this.txtCostumesDatabasePath.ReadOnly = true;
            this.txtCostumesDatabasePath.Size = new System.Drawing.Size(611, 21);
            this.txtCostumesDatabasePath.TabIndex = 3;
            // 
            // btnManifestFilterRegex
            // 
            this.btnManifestFilterRegex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnManifestFilterRegex.Location = new System.Drawing.Point(543, 319);
            this.btnManifestFilterRegex.Name = "btnManifestFilterRegex";
            this.btnManifestFilterRegex.Size = new System.Drawing.Size(70, 21);
            this.btnManifestFilterRegex.TabIndex = 9;
            this.btnManifestFilterRegex.Text = "Regex";
            this.btnManifestFilterRegex.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(728, 396);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MLTD Info Viewer";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button btnSelectCardsDatabase;
        private System.Windows.Forms.TextBox txtCardsDatabasePath;
        private System.Windows.Forms.ListView lvwCards;
        private System.Windows.Forms.OpenFileDialog ofd;
        private System.Windows.Forms.ListView lvwCostumes;
        private System.Windows.Forms.Button btnSelectCostumesDatabase;
        private System.Windows.Forms.TextBox txtCostumesDatabasePath;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ListView lvwManifest;
        private System.Windows.Forms.Button btnSelectManifestDatabase;
        private System.Windows.Forms.TextBox txtManifestDatabasePath;
        private System.Windows.Forms.Button btnManifestFilterString;
        private System.Windows.Forms.TextBox txtManifestFilter;
        private System.Windows.Forms.Button btnManifestReset;
        private System.Windows.Forms.Button btnManifestFilterRegex;
    }
}

