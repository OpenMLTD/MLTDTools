using System;
using System.Windows.Forms;

namespace MillionDance {
    internal static class Program {

        [STAThread]
        private static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var f = new FMain()) {
                Application.Run(f);
            }
        }

    }
}
