using System;
using System.Diagnostics;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace OpenMLTD.ManifestTools.UI {
    public partial class FAbout : Form {

        public FAbout() {
            InitializeComponent();
            RegisterEventHandlers();
        }

        ~FAbout() {
            UnregisterEventHandlers();
        }

        private void RegisterEventHandlers() {
            linkLabel1.LinkClicked += LinkLabel1_LinkClicked;
            linkLabel2.LinkClicked += LinkLabel2_LinkClicked;
            linkLabel3.LinkClicked += LinkLabel3_LinkClicked;
        }

        private void UnregisterEventHandlers() {
            linkLabel1.LinkClicked -= LinkLabel1_LinkClicked;
            linkLabel2.LinkClicked -= LinkLabel2_LinkClicked;
            linkLabel3.LinkClicked -= LinkLabel3_LinkClicked;
        }

        private void LinkLabel1_LinkClicked(object sender, EventArgs e) {
            OpenUrl("https://github.com/OpenMLTD/MLTDTools");
        }

        private void LinkLabel2_LinkClicked(object sender, EventArgs e) {
            OpenUrl("https://github.com/KinoMyu/MLTD-Asset-Downloader");
        }

        private void LinkLabel3_LinkClicked(object sender, EventArgs e) {
            OpenUrl("https://www.matsurihi.me");
        }

        private static void OpenUrl([NotNull] string url) {
            var psi = new ProcessStartInfo(url);

            psi.UseShellExecute = true;

            Process.Start(psi);
        }

    }
}
