namespace OpenMLTD.ManifestTools.UI
{
    partial class FMain
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.mnuFile = new System.Windows.Forms.MenuItem();
            this.mnuFileOpen = new System.Windows.Forms.MenuItem();
            this.mnuFileOpenLocal = new System.Windows.Forms.MenuItem();
            this.mnuFileOpenRemote = new System.Windows.Forms.MenuItem();
            this.mnuTools = new System.Windows.Forms.MenuItem();
            this.mnuToolsDiff = new System.Windows.Forms.MenuItem();
            this.mnuWindows = new System.Windows.Forms.MenuItem();
            this.mnuHelp = new System.Windows.Forms.MenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.MenuItem();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuFile,
            this.mnuTools,
            this.mnuWindows,
            this.mnuHelp});
            // 
            // mnuFile
            // 
            this.mnuFile.Index = 0;
            this.mnuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuFileOpen});
            this.mnuFile.Text = "&File";
            // 
            // mnuFileOpen
            // 
            this.mnuFileOpen.Index = 0;
            this.mnuFileOpen.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuFileOpenLocal,
            this.mnuFileOpenRemote});
            this.mnuFileOpen.Text = "&Open";
            // 
            // mnuFileOpenLocal
            // 
            this.mnuFileOpenLocal.Index = 0;
            this.mnuFileOpenLocal.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            this.mnuFileOpenLocal.Text = "&Local...";
            // 
            // mnuFileOpenRemote
            // 
            this.mnuFileOpenRemote.Index = 1;
            this.mnuFileOpenRemote.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftO;
            this.mnuFileOpenRemote.Text = "&Remote...";
            // 
            // mnuTools
            // 
            this.mnuTools.Index = 1;
            this.mnuTools.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuToolsDiff});
            this.mnuTools.Text = "&Tools";
            // 
            // mnuToolsDiff
            // 
            this.mnuToolsDiff.Index = 0;
            this.mnuToolsDiff.Text = "Di&ff..";
            // 
            // mnuWindows
            // 
            this.mnuWindows.Index = 2;
            this.mnuWindows.MdiList = true;
            this.mnuWindows.Text = "&Windows";
            // 
            // mnuHelp
            // 
            this.mnuHelp.Index = 3;
            this.mnuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuHelpAbout});
            this.mnuHelp.Text = "&Help";
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Index = 0;
            this.mnuHelpAbout.Text = "&About";
            // 
            // FMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(980, 605);
            this.IsMdiContainer = true;
            this.Menu = this.mainMenu1;
            this.Name = "FMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MLTD Manifest Tools";
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.MainMenu mainMenu1;

        private System.Windows.Forms.MenuItem mnuFile;

        private System.Windows.Forms.MenuItem mnuWindows;

        private System.Windows.Forms.MenuItem mnuHelp;

        private System.Windows.Forms.MenuItem mnuFileOpen;

        private System.Windows.Forms.MenuItem mnuFileOpenRemote;

        private System.Windows.Forms.MenuItem mnuTools;

        private System.Windows.Forms.MenuItem mnuToolsDiff;

        private System.Windows.Forms.MenuItem mnuFileOpenLocal;

        #endregion

        private System.Windows.Forms.MenuItem mnuHelpAbout;
        private System.Windows.Forms.OpenFileDialog ofd;
    }
}

