namespace OpenMLTD.ManifestTools.UI
{
    partial class FManifest
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
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.mnuView = new System.Windows.Forms.MenuItem();
            this.mnuViewList = new System.Windows.Forms.MenuItem();
            this.mnuViewTree = new System.Windows.Forms.MenuItem();
            this.mnuAction = new System.Windows.Forms.MenuItem();
            this.mnuActionDownload = new System.Windows.Forms.MenuItem();
            this.mnuActionSave = new System.Windows.Forms.MenuItem();
            this.mnuActionExport = new System.Windows.Forms.MenuItem();
            this.ctxL = new System.Windows.Forms.ContextMenu();
            this.ctxLAdd = new System.Windows.Forms.MenuItem();
            this.ctxR = new System.Windows.Forms.ContextMenu();
            this.ctxRRemove = new System.Windows.Forms.MenuItem();
            this.ctxRClear = new System.Windows.Forms.MenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.cboFilterField = new System.Windows.Forms.ComboBox();
            this.btnFilterReset = new System.Windows.Forms.Button();
            this.btnFilterByRegex = new System.Windows.Forms.Button();
            this.btnFilterByText = new System.Windows.Forms.Button();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.assetTreeList1 = new OpenMLTD.ManifestTools.UI.AssetTreeList();
            this.label2 = new System.Windows.Forms.Label();
            this.lvDownload = new OpenMLTD.ManifestTools.UI.ExplorerListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuView,
            this.mnuAction});
            // 
            // mnuView
            // 
            this.mnuView.Index = 0;
            this.mnuView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuViewList,
            this.mnuViewTree});
            this.mnuView.Text = "&View";
            // 
            // mnuViewList
            // 
            this.mnuViewList.Checked = true;
            this.mnuViewList.Index = 0;
            this.mnuViewList.Text = "&List";
            // 
            // mnuViewTree
            // 
            this.mnuViewTree.Index = 1;
            this.mnuViewTree.Text = "&Tree";
            // 
            // mnuAction
            // 
            this.mnuAction.Index = 1;
            this.mnuAction.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuActionDownload,
            this.mnuActionSave,
            this.mnuActionExport});
            this.mnuAction.Text = "&Action";
            // 
            // mnuActionDownload
            // 
            this.mnuActionDownload.Index = 0;
            this.mnuActionDownload.Text = "&Download...";
            // 
            // mnuActionSave
            // 
            this.mnuActionSave.Index = 1;
            this.mnuActionSave.Text = "&Save...";
            // 
            // mnuActionExport
            // 
            this.mnuActionExport.Index = 2;
            this.mnuActionExport.Text = "E&xport...";
            // 
            // ctxL
            // 
            this.ctxL.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.ctxLAdd});
            // 
            // ctxLAdd
            // 
            this.ctxLAdd.Index = 0;
            this.ctxLAdd.Text = "&Add to Pending Downloads";
            // 
            // ctxR
            // 
            this.ctxR.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.ctxRRemove,
            this.ctxRClear});
            // 
            // ctxRRemove
            // 
            this.ctxRRemove.Index = 0;
            this.ctxRRemove.Text = "&Remove";
            // 
            // ctxRClear
            // 
            this.ctxRClear.Index = 1;
            this.ctxRClear.Text = "&Clear";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.cboFilterField);
            this.splitContainer1.Panel1.Controls.Add(this.btnFilterReset);
            this.splitContainer1.Panel1.Controls.Add(this.btnFilterByRegex);
            this.splitContainer1.Panel1.Controls.Add(this.btnFilterByText);
            this.splitContainer1.Panel1.Controls.Add(this.txtFilter);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.assetTreeList1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.lvDownload);
            this.splitContainer1.Size = new System.Drawing.Size(973, 590);
            this.splitContainer1.SplitterDistance = 514;
            this.splitContainer1.TabIndex = 1;
            // 
            // cboFilterField
            // 
            this.cboFilterField.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cboFilterField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFilterField.FormattingEnabled = true;
            this.cboFilterField.Items.AddRange(new object[] {
            "Local Name",
            "Remote Name",
            "Hash"});
            this.cboFilterField.Location = new System.Drawing.Point(239, 564);
            this.cboFilterField.Name = "cboFilterField";
            this.cboFilterField.Size = new System.Drawing.Size(74, 21);
            this.cboFilterField.TabIndex = 7;
            // 
            // btnFilterReset
            // 
            this.btnFilterReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFilterReset.Location = new System.Drawing.Point(461, 563);
            this.btnFilterReset.Name = "btnFilterReset";
            this.btnFilterReset.Size = new System.Drawing.Size(50, 22);
            this.btnFilterReset.TabIndex = 6;
            this.btnFilterReset.Text = "Reset";
            this.btnFilterReset.UseVisualStyleBackColor = true;
            // 
            // btnFilterByRegex
            // 
            this.btnFilterByRegex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFilterByRegex.Location = new System.Drawing.Point(390, 563);
            this.btnFilterByRegex.Name = "btnFilterByRegex";
            this.btnFilterByRegex.Size = new System.Drawing.Size(65, 22);
            this.btnFilterByRegex.TabIndex = 5;
            this.btnFilterByRegex.Text = "By Regex";
            this.btnFilterByRegex.UseVisualStyleBackColor = true;
            // 
            // btnFilterByText
            // 
            this.btnFilterByText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFilterByText.Location = new System.Drawing.Point(319, 563);
            this.btnFilterByText.Name = "btnFilterByText";
            this.btnFilterByText.Size = new System.Drawing.Size(65, 22);
            this.btnFilterByText.TabIndex = 4;
            this.btnFilterByText.Text = "By Text";
            this.btnFilterByText.UseVisualStyleBackColor = true;
            // 
            // txtFilter
            // 
            this.txtFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilter.Location = new System.Drawing.Point(44, 565);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(189, 20);
            this.txtFilter.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 568);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Filter:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Assets";
            // 
            // assetTreeList1
            // 
            this.assetTreeList1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.assetTreeList1.Location = new System.Drawing.Point(3, 25);
            this.assetTreeList1.Name = "assetTreeList1";
            this.assetTreeList1.Size = new System.Drawing.Size(508, 534);
            this.assetTreeList1.TabIndex = 0;
            this.assetTreeList1.View = OpenMLTD.ManifestTools.UI.TreeListView.ListView;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Pending Downloads";
            // 
            // lvDownload
            // 
            this.lvDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvDownload.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvDownload.FullRowSelect = true;
            this.lvDownload.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvDownload.HideSelection = false;
            this.lvDownload.Location = new System.Drawing.Point(3, 25);
            this.lvDownload.Name = "lvDownload";
            this.lvDownload.Size = new System.Drawing.Size(449, 562);
            this.lvDownload.TabIndex = 0;
            this.lvDownload.UseCompatibleStateImageBehavior = false;
            this.lvDownload.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Size";
            // 
            // FManifest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(973, 590);
            this.Controls.Add(this.splitContainer1);
            this.Menu = this.mainMenu1;
            this.Name = "FManifest";
            this.Text = "Manifest";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.MainMenu mainMenu1;

        private System.Windows.Forms.MenuItem mnuAction;

        private System.Windows.Forms.MenuItem mnuActionDownload;

        private System.Windows.Forms.MenuItem mnuActionExport;

        private System.Windows.Forms.MenuItem mnuView;

        private System.Windows.Forms.MenuItem mnuViewList;

        private System.Windows.Forms.MenuItem mnuViewTree;

        #endregion
        private System.Windows.Forms.ContextMenu ctxL;
        private System.Windows.Forms.MenuItem ctxLAdd;
        private System.Windows.Forms.ContextMenu ctxR;
        private System.Windows.Forms.MenuItem ctxRRemove;
        private System.Windows.Forms.MenuItem ctxRClear;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private AssetTreeList assetTreeList1;
        private ExplorerListView lvDownload;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.SaveFileDialog sfd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MenuItem mnuActionSave;
        private System.Windows.Forms.Button btnFilterReset;
        private System.Windows.Forms.Button btnFilterByRegex;
        private System.Windows.Forms.Button btnFilterByText;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboFilterField;
    }
}