using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenMLTD.MiriTore;
using OpenMLTD.MiriTore.Database;

namespace MltdInfoViewer {
    public partial class FAssetDownload : Form {

        static FAssetDownload() {
            CheckForIllegalCrossThreadCalls = false;
        }

        private FAssetDownload() {
            InitializeComponent();
            RegisterEventHandlers();
        }

        ~FAssetDownload() {
            UnregisterEventHandlers();
        }

        public static DialogResult ShowDownload(IReadOnlyList<AssetInfo> assetInfoList, IWin32Window owner) {
            DialogResult r;

            using (var f = new FAssetDownload()) {
                f._assetInfoList = assetInfoList;
                r = f.ShowDialog(owner);
            }

            return r;
        }

        private void UnregisterEventHandlers() {
            Load -= FAssetDownload_Load;
            btnGo.Click -= BtnGo_Click;
            btnAbort.Click -= BtnAbort_Click;
            btnBrowseDestinationFolder.Click -= BtnBrowseDestinationFolder_Click;
        }

        private void RegisterEventHandlers() {
            Load += FAssetDownload_Load;
            btnGo.Click += BtnGo_Click;
            btnAbort.Click += BtnAbort_Click;
            btnBrowseDestinationFolder.Click += BtnBrowseDestinationFolder_Click;
        }

        private void BtnBrowseDestinationFolder_Click(object sender, EventArgs e) {
            var r = folderBrowserDialog1.ShowDialog(this);

            if (r == DialogResult.Cancel) {
                return;
            }

            txtDestinationFolder.Text = folderBrowserDialog1.SelectedPath;
        }

        private void BtnAbort_Click(object sender, EventArgs e) {
            if (!_isDownloading) {
                return;
            }

            _isDownloading = false;

            var wc = _webClient;

            if (wc != null) {
                wc.CancelAsync();

                wc.DownloadFileCompleted -= WebClient_DownloadFileCompleted;
                wc.Dispose();

                _webClient = null;
            }

            btnAbort.Enabled = false;
        }

        private void BtnGo_Click(object sender, EventArgs e) {
            var checksPassed = false;

            do {
                var hostNameType = Uri.CheckHostName(cboServer.Text);

                if (hostNameType == UriHostNameType.Unknown) {
                    Log("Error: invalid server host name.");
                    break;
                }

                if (!AssetVersionPattern.IsMatch(txtAssetVersion.Text)) {
                    Log("Error: asset version should be a positive number.");
                    break;
                }

                if (!UnityHeaderVersionPattern.IsMatch(txtUnityHeaderVersion.Text)) {
                    Log("Error: Unity header version should be like \"x.y.zaw\".");
                    break;
                }

                if (!UnityAssetVersionPattern.IsMatch(txtUnityAssetVersion.Text)) {
                    Log("Error: Unity asset version should be like \"x.y\".");
                    break;
                }

                if (!Directory.Exists(txtDestinationFolder.Text)) {
                    Log($"Error: destination folder \"{txtDestinationFolder.Text}\" does not exist.");
                    break;
                }

                checksPassed = true;
            } while (false);

            if (!checksPassed) {
                return;
            }

            cboServer.Enabled = false;
            txtAssetVersion.Enabled = false;
            txtUnityHeaderVersion.Enabled = false;
            txtUnityAssetVersion.Enabled = false;
            cboPlatform.Enabled = false;
            txtDestinationFolder.Enabled = false;
            btnBrowseDestinationFolder.Enabled = false;
            btnGo.Enabled = false;
            btnAbort.Enabled = true;

            ShowProgress(_downloadedAssetCount, _assetInfoList.Count);
            ShowProgress(true);

            var wc = new WebClient();

            wc.Headers.Add("X-Unity-Version", txtUnityHeaderVersion.Text);
            wc.DownloadFileCompleted += WebClient_DownloadFileCompleted;

            _webClient = wc;

            BeginDownloadAsset(0);
        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e) {
            if (e.Cancelled) {
                Log("Downloading canceled.");
                return;
            }

            var downloadedIndex = _downloadedAssetCount;

            ++_downloadedAssetCount;
            ShowProgress(_downloadedAssetCount, _assetInfoList.Count);

            Log($"Asset downloaded: {_assetInfoList[downloadedIndex].ResourceName}");

            lvEntries.Items[downloadedIndex].Text = "√";

            if (_downloadedAssetCount >= _assetInfoList.Count) {
                Log("All assets downloaded.");
                btnAbort.Enabled = false;
            } else {
                Debug.Assert(_webClient != null);
                BeginDownloadAsset(downloadedIndex + 1);
            }
        }

        private void FAssetDownload_Load(object sender, EventArgs e) {
            if (cboServer.Items.Count > 0) {
                cboServer.SelectedIndex = 0;
            }

            if (cboPlatform.Items.Count > 0) {
                cboPlatform.SelectedIndex = 0;
            }

            btnAbort.Enabled = false;

            ShowProgress(false);
            ShowProgress(0, 0);

            {
                lvEntries.Columns.Add("Complete");
                lvEntries.Columns.Add("Local");
                lvEntries.Columns.Add("Size");
                lvEntries.Columns.Add("Remote");

                long assetTotalSize = 0;

                lvEntries.BeginUpdate();

                foreach (var assetInfo in _assetInfoList) {
                    var lvi = new ListViewItem(string.Empty);
                    var readableSize = MathUtilities.GetHumanReadableFileSize(assetInfo.Size);

                    lvi.SubItems.Add(assetInfo.ResourceName);
                    lvi.SubItems.Add(readableSize);
                    lvi.SubItems.Add(assetInfo.RemoteName);

                    lvEntries.Items.Add(lvi);

                    assetTotalSize += assetInfo.Size;
                }

                lvEntries.EndUpdate();
                lvEntries.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

                var readableTotalSize = MathUtilities.GetHumanReadableFileSize(assetTotalSize);

                lblEntryStats.Text = $"{_assetInfoList.Count} asset(s) / {readableTotalSize}";
                lblEntryStats.Visible = true;
            }

            pgbProgress.Maximum = _assetInfoList.Count;
        }

        private void ShowProgress(bool v) {
            lblProgress.Visible = v;
        }

        private void ShowProgress(int current, int total) {
            float v;

            if (total == 0) {
                v = 0;
            } else {
                v = (float)current / total;
            }

            pgbProgress.Value = (int)(pgbProgress.Minimum + (pgbProgress.Maximum - pgbProgress.Minimum) * v);
            lblProgress.Text = $"{current}/{total} ({v:0.###})";
        }

        private void BeginDownloadAsset(int index) {
            if (index < 0 || index >= _assetInfoList.Count) {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }

            string platform;

            switch (cboPlatform.SelectedIndex) {
                case 0:
                    platform = "Android";
                    break;
                case 1:
                    platform = "iOS";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var assetInfo = _assetInfoList[index];
            var url = $"https://{cboServer.Text}/{txtAssetVersion.Text}/production/{txtUnityAssetVersion.Text}/{platform}/{assetInfo.RemoteName}";
            var uri = new Uri(url);

            var localPath = Path.Combine(txtDestinationFolder.Text, assetInfo.ResourceName);

            _webClient.DownloadFileAsync(uri, localPath);
        }

        private void Log(string text) {
            var now = DateTime.Now;
            var log = $"[{now:s}] {text}";

            if (txtLog.TextLength > 0) {
                log = Environment.NewLine + log;
            }

            txtLog.AppendText(log);
            txtLog.SelectionStart = txtLog.TextLength;
        }

        private WebClient _webClient;

        private bool _isDownloading;
        private int _downloadedAssetCount;

        private IReadOnlyList<AssetInfo> _assetInfoList;

        private static readonly Regex AssetVersionPattern = new Regex(@"^\d+$");
        private static readonly Regex UnityHeaderVersionPattern = new Regex(@"^\d+\.\d+\.\d+[fp]\d+$");
        private static readonly Regex UnityAssetVersionPattern = new Regex(@"^\d+\.\d+$");

    }
}
