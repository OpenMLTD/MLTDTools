using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using JetBrains.Annotations;
using Microsoft.WindowsAPICodePack.Dialogs;
using OpenMLTD.ManifestTools.Web.Matsuri;
using OpenMLTD.ManifestTools.Web.Matsuri.Entities;
using OpenMLTD.ManifestTools.Web.TDAssets;
using OpenMLTD.MiriTore;

namespace OpenMLTD.ManifestTools.UI {
    public partial class FAssetDownload : Form {

        public FAssetDownload([NotNull, ItemNotNull] TreeListItem[] items, [CanBeNull] DownloadConfig downloadConfig) {
            InitializeComponent();
            RegisterEventHandlers();

            _items = items;
            _downloaded = new bool[items.Length];
            _originalDownloadConfig = downloadConfig;
        }

        ~FAssetDownload() {
            UnregisterEventHandlers();
        }

        private void InitializeControls() {
            lblCurrent.Text = string.Empty;

            var items = _items;

            {
                uint totalSize = 0;

                foreach (var item in items) {
                    totalSize += item.Size;
                }

                lblTotalSize.Text = $"Total: {MathUtilities.GetHumanReadableFileSize(totalSize)}";
            }

            lvState.BeginUpdate();

            foreach (var item in items) {
                var lvi = new ListViewItem();
                lvi.SubItems.Add(item.LocalName);
                lvi.SubItems.Add(MathUtilities.GetHumanReadableFileSize(item.Size));
                lvState.Items.Add(lvi);
            }

            lvState.EndUpdate();

            var downloadConfig = _originalDownloadConfig;

            if (downloadConfig != null) {
                cboHost.Text = downloadConfig.Host;
                txtAssetVersion.Text = downloadConfig.UnityVersion;

                radResLatest.Checked = false;
                radResLatest.Enabled = false;
                radResManual.Checked = true;
                radResManual.Enabled = false;
                cboResVersion.Text = downloadConfig.ResourceVersion.ToString();

                switch (downloadConfig.Platform) {
                    case UnityMobilePlatform.Android:
                        radPlatformAndroid.Checked = true;
                        break;
                    case UnityMobilePlatform.iOS:
                        radPlatformIos.Checked = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                panel1.Enabled = false;
            } else {
                cboHost.SelectedIndex = 0;
            }
        }

        private void RegisterEventHandlers() {
            Load += FAssetDownload_Load;
            btnBrowseFolder.Click += BtnBrowseFolder_Click;
            btnGo.Click += BtnGo_Click;
            btnCancel.Click += BtnCancel_Click;
            radResLatest.Click += RadResLatest_Click;
            radResManual.Click += RadResManual_Click;
        }

        private void UnregisterEventHandlers() {
            Load -= FAssetDownload_Load;
            btnBrowseFolder.Click -= BtnBrowseFolder_Click;
            btnGo.Click -= BtnGo_Click;
            btnCancel.Click -= BtnCancel_Click;
            radResLatest.Click -= RadResLatest_Click;
            radResManual.Click -= RadResManual_Click;
        }

        private void FAssetDownload_Load(object sender, EventArgs e) {
            InitializeControls();
        }

        private void BtnBrowseFolder_Click(object sender, EventArgs e) {
            using (var dlg = new CommonOpenFileDialog()) {
                dlg.AllowNonFileSystemItems = false;
                dlg.EnsureFileExists = true;
                dlg.EnsureValidNames = true;
                dlg.IsFolderPicker = true;
                dlg.Multiselect = false;

                var r = dlg.ShowDialog(Handle);

                if (r == CommonFileDialogResult.Cancel) {
                    return;
                }

                txtSaveToDir.Text = dlg.FileName;
            }
        }

        private async void BtnGo_Click(object sender, EventArgs e) {
            var (b, message) = ValidateFields();

            if (!b) {
                MessageBox.Show(message, ApplicationHelper.GetApplicationTitle(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var downloadConfig = await GetDownloadConfig();

            _thread = new WorkerThread(this, downloadConfig, txtSaveToDir.Text);

            panel1.Enabled = false;
            panel4.Enabled = false;
            btnGo.Enabled = false;
            btnCancel.Enabled = true;
            _thread.Start();
        }

        private void BtnCancel_Click(object sender, EventArgs e) {
            var thread = _thread;

            Debug.Assert(thread != null);

            thread.Abort();

            panel1.Enabled = true;
            panel4.Enabled = true;
            btnGo.Enabled = true;
            btnCancel.Enabled = false;
        }

        private void RadResLatest_Click(object sender, EventArgs e) {
            cboResVersion.Enabled = false;
        }

        private async void RadResManual_Click(object sender, EventArgs e) {
            cboResVersion.Enabled = true;

            if (cboResVersion.Items.Count > 0) {
                return;
            }

            Enabled = false;

            try {
                var resVersions = await Hime.GetAssetVersions();

                foreach (var resVersion in resVersions) {
                    cboResVersion.Items.Add(resVersion.Version.ToString());
                }

                if (cboResVersion.Items.Count > 0) {
                    cboResVersion.SelectedIndex = 0;
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString(), ApplicationHelper.GetApplicationTitle(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                radResManual.Enabled = false;
                radResLatest.PerformClick();
            } finally {
                Enabled = true;
            }
        }

        private void ProgressFile(int index, int total, [NotNull] string fileName) {
            var progress = (float)(index + 1) / total;
            var progressBarValue = pb.Minimum + (int)((pb.Maximum - pb.Minimum) * progress);
            pb.Value = progressBarValue;

            var percentage = ((int)(progress * 10000)) / 100f;
            var percValStr = percentage.ToString("F2");
            var percentageStr = $"{percValStr}%";
            lblCurrent.Text = $"{percentageStr}: {fileName}";

            lvState.Items[index].Text = "√";
        }

        private (bool, string) ValidateFields() {
            if (string.IsNullOrWhiteSpace(cboHost.Text)) {
                return (false, "Host cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(txtAssetVersion.Text)) {
                return (false, "Unity asset version cannot be empty.");
            }

            if (radResManual.Checked) {
                if (!int.TryParse(cboResVersion.Text, out var n) || n <= 0) {
                    return (false, "Please specify a valid resource version.");
                }
            }

            {
                var saveDir = txtSaveToDir.Text;
                var pathValid = !string.IsNullOrWhiteSpace(saveDir);

                if (pathValid) {
                    if (!Directory.Exists(saveDir)) {
                        try {
                            Directory.CreateDirectory(saveDir);
                        } catch (IOException) {
                            pathValid = false;
                        }
                    }
                }

                if (!pathValid) {
                    return (false, $"Path '{saveDir}' is not valid.");
                }
            }

            return (true, null);
        }

        [NotNull]
        private async Task<DownloadConfig> GetDownloadConfig() {
            if (_originalDownloadConfig == null) {
                int resourceVersion;
                string resourceAssetName;

                if (radResLatest.Checked) {
                    var latest = await Hime.GetLatestVersion();
                    resourceVersion = latest.Res.Version;
                    resourceAssetName = latest.Res.IndexName;
                } else {
                    resourceVersion = int.Parse(cboResVersion.Text);
                    ResVersion res;

                    try {
                        res = await Hime.GetAssetVersion(resourceVersion);
                    } catch (HttpRequestException ex) {
                        throw new ApplicationException($"Cannot get manifest of resource version {resourceVersion.ToString()}. Maybe this version does not exist.", ex);
                    }

                    resourceAssetName = res.IndexName;
                }

                var platform = radPlatformAndroid.Checked ? UnityMobilePlatform.Android : UnityMobilePlatform.iOS;

                return new DownloadConfig(cboHost.Text, txtAssetVersion.Text, resourceVersion, radResLatest.Checked, platform, resourceAssetName);
            } else {
                return _originalDownloadConfig;
            }
        }

        private void OnAllDownloadsComplete() {
            btnGo.Enabled = false;
            btnCancel.Enabled = false;
        }

        [NotNull, ItemNotNull]
        private readonly TreeListItem[] _items;

        [NotNull]
        private readonly bool[] _downloaded;

        [CanBeNull]
        private readonly DownloadConfig _originalDownloadConfig;

        [CanBeNull]
        private WorkerThread _thread;

    }
}
