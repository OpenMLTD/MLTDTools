using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OpenMLTD.ManifestTools.Web.Matsuri;
using OpenMLTD.ManifestTools.Web.TDAssets;
using OpenMLTD.MiriTore;
using OpenMLTD.MiriTore.Database;

namespace OpenMLTD.ManifestTools.UI {
    public partial class FMain : Form {

        public FMain() {
            InitializeComponent();
            RegisterEventHandlers();
        }

        ~FMain() {
            UnregisterEventHandlers();
        }

        private void RegisterEventHandlers() {
            mnuFileOpenLocal.Click += MnuFileOpenLocal_Click;
            mnuFileOpenRemote.Click += MnuFileOpenRemote_Click;
            mnuHelpAbout.Click += MnuHelpAbout_Click;
        }

        private void UnregisterEventHandlers() {
            mnuFileOpenLocal.Click -= MnuFileOpenLocal_Click;
            mnuFileOpenRemote.Click -= MnuFileOpenRemote_Click;
            mnuHelpAbout.Click -= MnuHelpAbout_Click;
        }

        private void MnuFileOpenLocal_Click(object sender, EventArgs e) {
            ofd.CheckFileExists = true;
            ofd.DereferenceLinks = true;
            ofd.Filter = "Asset Database (*.data)|*.data";
            ofd.Multiselect = false;
            ofd.ReadOnlyChecked = false;
            ofd.ShowReadOnly = false;
            ofd.SupportMultiDottedExtensions = true;
            ofd.ValidateNames = true;

            var r = ofd.ShowDialog(this);

            if (r == DialogResult.Cancel) {
                return;
            }

            var filePath = ofd.FileName;
            var data = File.ReadAllBytes(filePath);

            var b = AssetInfoList.TryParse(data, Encoding.UTF8, out var assetInfoList);

            if (!b) {
                var message = $"'{filePath}' is not a valid asset database file.";
                MessageBox.Show(message, ApplicationHelper.GetApplicationTitle(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Debug.Assert(assetInfoList != null);

            var opening = ManifestOpening.Local(filePath);

            var form = new FManifest(assetInfoList, opening, null);

            form.MdiParent = this;

            form.Show();
        }

        private async void MnuFileOpenRemote_Click(object sender, EventArgs e) {
            var (r, config) = FManifestDownload.ShowModal(this);

            if (r == DialogResult.Cancel) {
                return;
            }

            Debug.Assert(config != null);

            byte[] assetInfoListData;

            try {
                assetInfoListData = await TDDownloader.DownloadData(config.ResourceVersion, config.ManifestAssetName);
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString(), ApplicationHelper.GetApplicationTitle(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var b = AssetInfoList.TryParse(assetInfoListData, Encoding.UTF8, out var assetInfoList);

            if (!b) {
                const string message = "Received data is not a valid asset database file.";
                MessageBox.Show(message, ApplicationHelper.GetApplicationTitle(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Debug.Assert(assetInfoList != null);

            var opening = ManifestOpening.Remote(config.ResourceVersion, config.IsLatest);

            var form = new FManifest(assetInfoList, opening, config);

            form.MdiParent = this;

            form.Show();
        }

        private void MnuHelpAbout_Click(object sender, EventArgs e) {
            using (var f = new FAbout()) {
                f.ShowDialog(this);
            }
        }

    }
}
