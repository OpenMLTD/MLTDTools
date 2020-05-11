using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using JetBrains.Annotations;
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

            if (opening.IsLocal) {
                mnuActionDownload.Enabled = false;
            }

            {
                string formTitle;

                if (opening.IsLocal) {
                    formTitle = $"Local: {opening.FilePath}";
                } else {
                    formTitle = $"Remote: {opening.Version.ToString()}";

                    if (opening.IsLatest) {
                        formTitle += " (latest)";
                    }

                    if (!string.IsNullOrWhiteSpace(opening.FilePath)) {
                        formTitle += $" [{opening.FilePath}]";
                    }
                }

                Text = formTitle;
            }
        }

        private void LoadManifest() {
            assetTreeList1.AddItems(Manifest);
            assetTreeList1.Sort();
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
        }

        private void FManifest_Load(object sender, EventArgs e) {
            LoadManifest();
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

        [NotNull]
        private readonly ManifestOpening _opening;

        [CanBeNull]
        private readonly DownloadConfig _downloadConfig;

        [NotNull, ItemNotNull]
        private readonly HashSet<TreeListItem> _downloadPendingSet;

    }
}
