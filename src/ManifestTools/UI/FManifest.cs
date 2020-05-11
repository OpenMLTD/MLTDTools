using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using JetBrains.Annotations;
using OpenMLTD.ManifestTools.Extensions;
using OpenMLTD.ManifestTools.Web.TDAssets;
using OpenMLTD.MiriTore;
using OpenMLTD.MiriTore.Database;

namespace OpenMLTD.ManifestTools.UI {
    public partial class FManifest : Form {

        public FManifest([NotNull] AssetInfoList manifest, [NotNull] ManifestOpening opening, [CanBeNull] DownloadConfig downloadConfig) {
            Manifest = manifest;
            _opening = opening;
            _downloadConfig = downloadConfig;
            _downloadPendingSet = new HashSet<TreeListItem>();

            InitializeComponent();
            RegisterEventHandlers();
            InitializeControls();
        }

        ~FManifest() {
            UnregisterEventHandlers();
        }

        [NotNull]
        public AssetInfoList Manifest { get; }

        private void InitializeControls() {
            var opening = _opening;
            var config = _downloadConfig;

            {
                string formTitle;

                if (opening.IsLocal) {
                    formTitle = $"Local: {opening.FilePath}";
                } else {
                    formTitle = $"Remote: {opening.Version.ToString()}";

                    if (opening.IsLatest) {
                        formTitle += " (latest)";
                    }

                    var mobilePlatform = config?.Platform ?? UnityMobilePlatform.Unknown;
                    formTitle += $" - {mobilePlatform.ToStringFast()}";

                    if (!string.IsNullOrWhiteSpace(opening.FilePath)) {
                        formTitle += $" [{opening.FilePath}]";
                    }
                }

                Text = formTitle;
            }
        }

        private void AddSelectedItemsToPendingDownloads() {
            var items = assetTreeList1.GetSelectedItems();
            var set = _downloadPendingSet;
            var lv = lvDownload;

            lv.BeginUpdate();

            foreach (var item in items) {
                if (set.Contains(item)) {
                    continue;
                }

                set.Add(item);

                var lvi = new ListViewItem(item.LocalName);
                lvi.SubItems.Add(MathUtilities.GetHumanReadableFileSize(item.Size));
                lvi.Tag = item;

                lv.Items.Add(lvi);
            }

            lv.EndUpdate();
        }

        private void ClearPendingDownloads() {
            lvDownload.Items.Clear();
            _downloadPendingSet.Clear();
        }

        private void RegisterEventHandlers() {
            Load += FManifest_Load;
            mnuViewList.Click += MnuViewList_Click;
            mnuViewTree.Click += MnuViewTree_Click;
            assetTreeList1.ItemDoubleClick += AssetTreeList1_ItemDoubleClick;
            assetTreeList1.ItemsContextRequested += AssetTreeList1_ItemsContextRequested;
            ctxLAdd.Click += CtxLAdd_Click;
            ctxRRemove.Click += CtxRRemove_Click;
            ctxRClear.Click += CtxRClear_Click;
            lvDownload.MouseDown += LvDownload_MouseDown;
            mnuActionExport.Click += MnuActionExport_Click;
            mnuActionDownload.Click += MnuActionDownload_Click;
            mnuActionSave.Click += MnuActionSave_Click;
            btnFilterByText.Click += BtnFilterByText_Click;
            btnFilterByRegex.Click += BtnFilterByRegex_Click;
            btnFilterReset.Click += BtnFilterReset_Click;
        }

        private void UnregisterEventHandlers() {
            Load -= FManifest_Load;
            mnuViewList.Click -= MnuViewList_Click;
            mnuViewTree.Click -= MnuViewTree_Click;
            assetTreeList1.ItemDoubleClick -= AssetTreeList1_ItemDoubleClick;
            assetTreeList1.ItemsContextRequested -= AssetTreeList1_ItemsContextRequested;
            ctxLAdd.Click -= CtxLAdd_Click;
            ctxRRemove.Click -= CtxRRemove_Click;
            ctxRClear.Click -= CtxRClear_Click;
            lvDownload.MouseDown -= LvDownload_MouseDown;
            mnuActionExport.Click -= MnuActionExport_Click;
            mnuActionDownload.Click -= MnuActionDownload_Click;
            mnuActionSave.Click -= MnuActionSave_Click;
            btnFilterByText.Click -= BtnFilterByText_Click;
            btnFilterByRegex.Click -= BtnFilterByRegex_Click;
            btnFilterReset.Click -= BtnFilterReset_Click;
        }

        private void FManifest_Load(object sender, EventArgs e) {
            cboFilterField.SelectedIndex = 0;
            assetTreeList1.AddItems(Manifest);
            assetTreeList1.Sort();
        }

        private void MnuViewList_Click(object sender, EventArgs e) {
            mnuViewList.Checked = true;
            mnuViewTree.Checked = false;
            assetTreeList1.View = TreeListView.ListView;
        }

        private void MnuViewTree_Click(object sender, EventArgs e) {
            mnuViewList.Checked = false;
            mnuViewTree.Checked = true;
            assetTreeList1.View = TreeListView.TreeView;
        }

        private void AssetTreeList1_ItemDoubleClick(object sender, MouseEventArgs e) {
            AddSelectedItemsToPendingDownloads();
        }

        private void AssetTreeList1_ItemsContextRequested(object sender, MouseEventArgs e) {
            ctxL.Show(assetTreeList1, e.Location);
        }

        private void CtxLAdd_Click(object sender, EventArgs e) {
            AddSelectedItemsToPendingDownloads();
        }

        private void CtxRRemove_Click(object sender, EventArgs e) {
            var itemCollection = lvDownload.SelectedItems;
            var itemCount = itemCollection.Count;

            if (itemCount == 0) {
                return;
            }

            if (itemCount == 1) {
                var item = itemCollection[0];
                var tag = item.Tag as TreeListItem;
                Debug.Assert(tag != null);
                _downloadPendingSet.Remove(tag);
                lvDownload.Items.RemoveAt(item.Index);

                return;
            }

            var itemArray = new ListViewItem[itemCount];

            for (var i = 0; i < itemCount; i += 1) {
                itemArray[i] = itemCollection[i];
            }

            foreach (var item in itemArray) {
                var tag = item.Tag as TreeListItem;
                Debug.Assert(tag != null);
                _downloadPendingSet.Remove(tag);
                lvDownload.Items.Remove(item);
            }
        }

        private void CtxRClear_Click(object sender, EventArgs e) {
            ClearPendingDownloads();
        }

        private void LvDownload_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button != MouseButtons.Right) {
                return;
            }

            ctxRRemove.Enabled = lvDownload.SelectedItems.Count > 0;

            ctxR.Show(lvDownload, e.Location);
        }

        private void MnuActionExport_Click(object sender, EventArgs e) {
            sfd.AddExtension = true;
            sfd.AutoUpgradeEnabled = true;
            sfd.DereferenceLinks = true;
            sfd.Filter = "Text Files (*.txt)|*.txt|All Files (*)|*";
            sfd.OverwritePrompt = true;
            sfd.SupportMultiDottedExtensions = true;
            sfd.ValidateNames = true;

            var r = sfd.ShowDialog(this);

            if (r == DialogResult.Cancel) {
                return;
            }

            var manifest = Manifest;

            using (var fileStream = File.Open(sfd.FileName, FileMode.Create, FileAccess.Write, FileShare.Write)) {
                using (var writer = new StreamWriter(fileStream, MltdConstants.Utf8WithoutBom)) {
                    writer.WriteLine("Asset count: {0}", manifest.Assets.Count.ToString());

                    foreach (var asset in manifest.Assets) {
                        writer.WriteLine();
                        writer.WriteLine("Resource name: {0}", asset.ResourceName);
                        writer.WriteLine("Resource hash: {0}", asset.ContentHash);
                        writer.WriteLine("Remote name: {0}", asset.RemoteName);
                        writer.WriteLine("File size: {0} ({1})", asset.Size.ToString(), MathUtilities.GetHumanReadableFileSize(asset.Size));
                    }
                }
            }

            MessageBox.Show($"Info exported to '{sfd.FileName}'.", ApplicationHelper.GetApplicationTitle(), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MnuActionDownload_Click(object sender, EventArgs e) {
            var listItems = lvDownload.Items;

            if (listItems.Count == 0) {
                return;
            }

            if (_downloadConfig == null) {
                const string message = "You are trying to download assets according to a local manifest. You can only perform this when you know the exact details of the manifest (resource version, engine version, platform info, etc.) otherwise it always fails.\nAgain, only proceed if you understand exactly what you are doing. Do you want to continue?";
                var r = MessageBox.Show(message, ApplicationHelper.GetApplicationTitle(), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (r == DialogResult.No) {
                    return;
                }
            }

            var items = new TreeListItem[listItems.Count];

            for (var i = 0; i < items.Length; i += 1) {
                items[i] = listItems[i].Tag as TreeListItem;
            }

            using (var f = new FAssetDownload(items, _downloadConfig)) {
                f.ShowDialog(this);
            }
        }

        private void MnuActionSave_Click(object sender, EventArgs e) {
            sfd.AddExtension = true;
            sfd.AutoUpgradeEnabled = true;
            sfd.DereferenceLinks = true;
            sfd.Filter = "Asset Database (*.data)|*.data";
            sfd.OverwritePrompt = true;
            sfd.SupportMultiDottedExtensions = true;
            sfd.ValidateNames = true;

            if (_downloadConfig != null) {
                sfd.FileName = _downloadConfig.ManifestAssetName;
            } else {
                if (!string.IsNullOrWhiteSpace(_opening.FilePath)) {
                    sfd.FileName = (new FileInfo(_opening.FilePath)).Name;
                } else {
                    sfd.FileName = string.Empty;
                }
            }

            var r = sfd.ShowDialog(this);

            if (r == DialogResult.Cancel) {
                return;
            }

            using (var fileStream = File.Open(sfd.FileName, FileMode.Create, FileAccess.Write, FileShare.Write)) {
                Manifest.SaveTo(fileStream, MltdConstants.Utf8WithoutBom);
            }

            MessageBox.Show($"Manifest saved to '{sfd.FileName}'.", ApplicationHelper.GetApplicationTitle(), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnFilterByText_Click(object sender, EventArgs e) {
            var input = txtFilter.Text;

            if (string.IsNullOrWhiteSpace(input)) {
                return;
            }

            var filtered = new List<AssetInfo>();
            var field = (FilterField)cboFilterField.SelectedIndex;

            foreach (var assetInfo in Manifest.Assets) {
                switch (field) {
                    case FilterField.ResourceName: {
                        if (assetInfo.ResourceName.Contains(input)) {
                            filtered.Add(assetInfo);
                        }

                        break;
                    }
                    case FilterField.RemoteName: {
                        if (assetInfo.RemoteName.Contains(input)) {
                            filtered.Add(assetInfo);
                        }

                        break;
                    }
                    case FilterField.ContentHash: {
                        if (assetInfo.ContentHash.Contains(input)) {
                            filtered.Add(assetInfo);
                        }

                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(field), field, null);
                }
            }

            _isFiltered = true;

            assetTreeList1.Clear();

            if (filtered.Count > 0) {
                assetTreeList1.AddItems(filtered.ToArray());
                assetTreeList1.Sort();
            }
        }

        private void BtnFilterByRegex_Click(object sender, EventArgs e) {
            var input = txtFilter.Text;

            if (string.IsNullOrWhiteSpace(input)) {
                return;
            }

            Regex regex;

            try {
                regex = new Regex(input, RegexOptions.CultureInvariant | RegexOptions.ECMAScript);
            } catch (ArgumentException ex) {
                var message = $"The input is not a valid regular expression:\n{input}";
                MessageBox.Show(message, ApplicationHelper.GetApplicationTitle(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var filtered = new List<AssetInfo>();
            var field = (FilterField)cboFilterField.SelectedIndex;

            foreach (var assetInfo in Manifest.Assets) {
                switch (field) {
                    case FilterField.ResourceName: {
                        if (regex.IsMatch(assetInfo.ResourceName)) {
                            filtered.Add(assetInfo);
                        }

                        break;
                    }
                    case FilterField.RemoteName: {
                        if (regex.IsMatch(assetInfo.RemoteName)) {
                            filtered.Add(assetInfo);
                        }

                        break;
                    }
                    case FilterField.ContentHash: {
                        if (regex.IsMatch(assetInfo.ContentHash)) {
                            filtered.Add(assetInfo);
                        }

                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(field), field, null);
                }
            }

            _isFiltered = true;

            assetTreeList1.Clear();

            if (filtered.Count > 0) {
                assetTreeList1.AddItems(filtered.ToArray());
                assetTreeList1.Sort();
            }
        }

        private void BtnFilterReset_Click(object sender, EventArgs e) {
            if (!_isFiltered) {
                return;
            }

            _isFiltered = false;

            assetTreeList1.Clear();
            assetTreeList1.AddItems(Manifest);
            assetTreeList1.Sort();
        }

        [NotNull]
        private readonly ManifestOpening _opening;

        [CanBeNull]
        private readonly DownloadConfig _downloadConfig;

        [NotNull, ItemNotNull]
        private readonly HashSet<TreeListItem> _downloadPendingSet;

        private bool _isFiltered;

        private enum FilterField {

            ResourceName = 0,

            RemoteName = 1,

            ContentHash = 2,

        }

    }
}
