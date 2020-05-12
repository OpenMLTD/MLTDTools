namespace OpenMLTD.MLTDTools.Applications.TDFacial {
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtExprKey = new System.Windows.Forms.TextBox();
            this.txtExprDesc = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnExprAdd = new System.Windows.Forms.Button();
            this.btnExprDel = new System.Windows.Forms.Button();
            this.btnExprMod = new System.Windows.Forms.Button();
            this.txtValueEditor = new System.Windows.Forms.TextBox();
            this.btnLoadConfig = new System.Windows.Forms.Button();
            this.btnSaveConfig = new System.Windows.Forms.Button();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            this.lvExpressionData = new OpenMLTD.MLTDTools.Applications.TDFacial.ListViewEx();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvExpressions = new OpenMLTD.MLTDTools.Applications.TDFacial.ListViewEx();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnShowScrObj = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 457);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Key:";
            // 
            // txtExprKey
            // 
            this.txtExprKey.Location = new System.Drawing.Point(59, 454);
            this.txtExprKey.Name = "txtExprKey";
            this.txtExprKey.Size = new System.Drawing.Size(93, 20);
            this.txtExprKey.TabIndex = 3;
            // 
            // txtExprDesc
            // 
            this.txtExprDesc.Location = new System.Drawing.Point(59, 483);
            this.txtExprDesc.Name = "txtExprDesc";
            this.txtExprDesc.Size = new System.Drawing.Size(179, 20);
            this.txtExprDesc.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 486);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Desc.:";
            // 
            // btnExprAdd
            // 
            this.btnExprAdd.Location = new System.Drawing.Point(12, 512);
            this.btnExprAdd.Name = "btnExprAdd";
            this.btnExprAdd.Size = new System.Drawing.Size(50, 29);
            this.btnExprAdd.TabIndex = 6;
            this.btnExprAdd.Text = "Add";
            this.btnExprAdd.UseVisualStyleBackColor = true;
            // 
            // btnExprDel
            // 
            this.btnExprDel.Location = new System.Drawing.Point(68, 512);
            this.btnExprDel.Name = "btnExprDel";
            this.btnExprDel.Size = new System.Drawing.Size(50, 29);
            this.btnExprDel.TabIndex = 7;
            this.btnExprDel.Text = "Delete";
            this.btnExprDel.UseVisualStyleBackColor = true;
            // 
            // btnExprMod
            // 
            this.btnExprMod.Location = new System.Drawing.Point(124, 512);
            this.btnExprMod.Name = "btnExprMod";
            this.btnExprMod.Size = new System.Drawing.Size(50, 29);
            this.btnExprMod.TabIndex = 8;
            this.btnExprMod.Text = "Modify";
            this.btnExprMod.UseVisualStyleBackColor = true;
            // 
            // txtValueEditor
            // 
            this.txtValueEditor.Location = new System.Drawing.Point(228, 521);
            this.txtValueEditor.Name = "txtValueEditor";
            this.txtValueEditor.Size = new System.Drawing.Size(68, 20);
            this.txtValueEditor.TabIndex = 10;
            this.txtValueEditor.Visible = false;
            // 
            // btnLoadConfig
            // 
            this.btnLoadConfig.Location = new System.Drawing.Point(372, 512);
            this.btnLoadConfig.Name = "btnLoadConfig";
            this.btnLoadConfig.Size = new System.Drawing.Size(89, 29);
            this.btnLoadConfig.TabIndex = 11;
            this.btnLoadConfig.Text = "Load...";
            this.btnLoadConfig.UseVisualStyleBackColor = true;
            // 
            // btnSaveConfig
            // 
            this.btnSaveConfig.Location = new System.Drawing.Point(467, 512);
            this.btnSaveConfig.Name = "btnSaveConfig";
            this.btnSaveConfig.Size = new System.Drawing.Size(89, 29);
            this.btnSaveConfig.TabIndex = 12;
            this.btnSaveConfig.Text = "Save...";
            this.btnSaveConfig.UseVisualStyleBackColor = true;
            // 
            // lvExpressionData
            // 
            this.lvExpressionData.AllowColumnReorder = true;
            this.lvExpressionData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.lvExpressionData.DoubleClickActivation = true;
            this.lvExpressionData.FullRowSelect = true;
            this.lvExpressionData.GridLines = true;
            this.lvExpressionData.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvExpressionData.HideSelection = false;
            this.lvExpressionData.Location = new System.Drawing.Point(244, 13);
            this.lvExpressionData.MultiSelect = false;
            this.lvExpressionData.Name = "lvExpressionData";
            this.lvExpressionData.Size = new System.Drawing.Size(312, 493);
            this.lvExpressionData.TabIndex = 1;
            this.lvExpressionData.UseCompatibleStateImageBehavior = false;
            this.lvExpressionData.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Name";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Value (0-1)";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Description";
            this.columnHeader5.Width = 86;
            // 
            // lvExpressions
            // 
            this.lvExpressions.AllowColumnReorder = true;
            this.lvExpressions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvExpressions.FullRowSelect = true;
            this.lvExpressions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvExpressions.HideSelection = false;
            this.lvExpressions.Location = new System.Drawing.Point(12, 13);
            this.lvExpressions.MultiSelect = false;
            this.lvExpressions.Name = "lvExpressions";
            this.lvExpressions.Size = new System.Drawing.Size(226, 434);
            this.lvExpressions.TabIndex = 0;
            this.lvExpressions.UseCompatibleStateImageBehavior = false;
            this.lvExpressions.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Key";
            this.columnHeader1.Width = 47;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Description";
            this.columnHeader2.Width = 126;
            // 
            // btnShowScrObj
            // 
            this.btnShowScrObj.Location = new System.Drawing.Point(316, 512);
            this.btnShowScrObj.Name = "btnShowScrObj";
            this.btnShowScrObj.Size = new System.Drawing.Size(50, 29);
            this.btnShowScrObj.TabIndex = 13;
            this.btnShowScrObj.Text = "ScrObj";
            this.btnShowScrObj.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(565, 551);
            this.Controls.Add(this.btnShowScrObj);
            this.Controls.Add(this.btnSaveConfig);
            this.Controls.Add(this.btnLoadConfig);
            this.Controls.Add(this.txtValueEditor);
            this.Controls.Add(this.btnExprMod);
            this.Controls.Add(this.btnExprDel);
            this.Controls.Add(this.btnExprAdd);
            this.Controls.Add(this.txtExprDesc);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtExprKey);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lvExpressionData);
            this.Controls.Add(this.lvExpressions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Facial Expression Mapping Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ListViewEx lvExpressions;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private ListViewEx lvExpressionData;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtExprKey;
        private System.Windows.Forms.TextBox txtExprDesc;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnExprAdd;
        private System.Windows.Forms.Button btnExprDel;
        private System.Windows.Forms.Button btnExprMod;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.TextBox txtValueEditor;
        private System.Windows.Forms.Button btnLoadConfig;
        private System.Windows.Forms.Button btnSaveConfig;
        private System.Windows.Forms.OpenFileDialog ofd;
        private System.Windows.Forms.SaveFileDialog sfd;
        private System.Windows.Forms.Button btnShowScrObj;
    }
}

