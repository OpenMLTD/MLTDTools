using System;
using System.Windows.Forms;
using OpenMLTD.ManifestTools.UI;

namespace OpenMLTD.ManifestTools {
    internal static class Program {

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        private static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Control.CheckForIllegalCrossThreadCalls = false;
            Application.Run(new FMain());
        }

    }
}
