namespace OpenMLTD.ManifestTools.UI
{
    partial class AssetTreeList
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.lv = new OpenMLTD.ManifestTools.UI.ExplorerListView();
            this.tv = new OpenMLTD.ManifestTools.UI.ExplorerTreeView();
            this.SuspendLayout();
            // 
            // lv
            // 
            this.lv.FullRowSelect = true;
            this.lv.HideSelection = false;
            this.lv.Location = new System.Drawing.Point(53, 42);
            this.lv.Name = "lv";
            this.lv.Size = new System.Drawing.Size(232, 178);
            this.lv.TabIndex = 0;
            this.lv.UseCompatibleStateImageBehavior = false;
            this.lv.View = System.Windows.Forms.View.Details;
            // 
            // tv
            // 
            this.tv.FullRowSelect = true;
            this.tv.Location = new System.Drawing.Point(277, 105);
            this.tv.Name = "tv";
            this.tv.Size = new System.Drawing.Size(281, 233);
            this.tv.TabIndex = 1;
            // 
            // AssetTreeList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tv);
            this.Controls.Add(this.lv);
            this.Name = "AssetTreeList";
            this.Size = new System.Drawing.Size(607, 441);
            this.ResumeLayout(false);

        }

        private OpenMLTD.ManifestTools.UI.ExplorerListView lv;

        private OpenMLTD.ManifestTools.UI.ExplorerTreeView tv;

        #endregion
    }
}
