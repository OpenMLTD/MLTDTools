using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using JetBrains.Annotations;
using OpenMLTD.MiriTore;
using OpenMLTD.MiriTore.Database;

namespace OpenMLTD.ManifestTools.UI {
    partial class AssetTreeList {

        public event EventHandler<MouseEventArgs> ItemDoubleClick;

        public event EventHandler<MouseEventArgs> ItemsContextRequested;

        public void AddItem([NotNull] AssetInfo assetInfo) {
            var item = new TreeListItem(assetInfo);
            AddItem(item);
        }

        // Warning: this method produces a great number of garbage
        public void AddItem([NotNull] TreeListItem item) {
            {
                var lvi = new ListViewItem(item.LocalName);

                lv.Items.Add(lvi);

                lvi.SubItems.Add(item.RemoteName);
                lvi.SubItems.Add(item.Hash);
                var sizeStr = MathUtilities.GetHumanReadableFileSize(item.Size);
                lvi.SubItems.Add(sizeStr);

                lvi.Tag = item;
            }

            {
                var tvn = new TreeNode(item.LocalName);

                var hierarchy = item.GetCategoryHierarchy();
                var hierarchyDepth = hierarchy.Length;

                Debug.Assert(hierarchyDepth > 0);

                var currentParentCollection = tv.Nodes[0].Nodes;
                var currentPath = string.Empty;
                var currentDepth = 0;

                while (currentDepth < hierarchyDepth) {
                    TreeNode parentNode;
                    var currentLayer = hierarchy[currentDepth];
                    currentPath = currentPath + $"/{currentLayer}";

                    if (_hierarchy.ContainsKey(currentPath)) {
                        parentNode = _hierarchy[currentPath];
                    } else {
                        parentNode = new TreeNode(currentLayer);
                        currentParentCollection.Add(parentNode);
                        _hierarchy.Add(currentPath, parentNode);
                    }

                    currentParentCollection = parentNode.Nodes;

                    currentDepth += 1;
                }

                currentParentCollection.Add(tvn);

                tvn.Tag = item;
            }
        }

        public void AddItems([NotNull] AssetInfoList assetInfoList) {
            lv.BeginUpdate();
            tv.BeginUpdate();

            foreach (var assetInfo in assetInfoList.Assets) {
                AddItem(assetInfo);
            }

            tv.EndUpdate();
            lv.EndUpdate();
        }

        public void AddItems([NotNull] AssetInfo[] assetInfos) {
            lv.BeginUpdate();
            tv.BeginUpdate();

            foreach (var assetInfo in assetInfos) {
                AddItem(assetInfo);
            }

            tv.EndUpdate();
            lv.EndUpdate();
        }

        public void AddItems([NotNull] TreeListItem[] items) {
            lv.BeginUpdate();
            tv.BeginUpdate();

            foreach (var item in items) {
                AddItem(item);
            }

            tv.EndUpdate();
            lv.EndUpdate();
        }

        public void Clear() {
            _items.Clear();
            _hierarchy.Clear();

            lv.Items.Clear();
            tv.Nodes.Clear();

            AddRootNode();
        }

        [NotNull, ItemNotNull]
        public List<TreeListItem> Items => _items;

        [NotNull, ItemNotNull]
        public TreeListItem[] GetSelectedItems() {
            TreeListItem[] result;

            switch (View) {
                case TreeListView.ListView: {
                    var selectedListItems = lv.SelectedItems;

                    if (selectedListItems.Count == 0) {
                        result = Array.Empty<TreeListItem>();
                    } else {
                        var count = selectedListItems.Count;

                        result = new TreeListItem[count];

                        for (var i = 0; i < count; i += 1) {
                            result[i] = selectedListItems[i].Tag as TreeListItem;
                        }
                    }

                    break;
                }
                case TreeListView.TreeView: {
                    var selectedNode = tv.SelectedNode;

                    if (selectedNode == null) {
                        result = Array.Empty<TreeListItem>();
                    } else {
                        result = GetTreeViewChildNodeItems(selectedNode);
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }

        public void Sort() {
            // Strange... if we assign ListViewSorter in the beginning, item insertion becomes very slow.
            lv.ListViewItemSorter = ListViewItemSorter.Instance;
            tv.TreeViewNodeSorter = TreeNodeSorter.Instance;

            lv.BeginUpdate();
            tv.BeginUpdate();
            lv.Sort();
            tv.Sort();
            tv.EndUpdate();
            lv.EndUpdate();

            lv.ListViewItemSorter = null;
            tv.TreeViewNodeSorter = null;
        }

        [NotNull, ItemNotNull]
        private static TreeListItem[] GetTreeViewChildNodeItems([NotNull] TreeNode node) {
            var result = new List<TreeListItem>();

            GetTreeViewChildNodeItems(node, result);

            return result.ToArray();
        }

        private static void GetTreeViewChildNodeItems([NotNull] TreeNode node, [NotNull, ItemNotNull] List<TreeListItem> result) {
            Stack<TreeNode> stack;

            {
                var nodes = node.Nodes;
                var nodeCount = nodes.Count;

                if (nodeCount == 0) {
                    result.Add(node.Tag as TreeListItem);
                    return;
                }

                stack = new Stack<TreeNode>();

                for (var i = nodeCount - 1; i >= 0; i -= 1) {
                    stack.Push(nodes[i]);
                }
            }

            // DFS
            while (stack.Count > 0) {
                var currentNode = stack.Pop();

                var nodes = currentNode.Nodes;
                var nodeCount = nodes.Count;

                if (nodeCount == 0) {
                    result.Add(currentNode.Tag as TreeListItem);
                } else {
                    for (var i = nodeCount - 1; i >= 0; i -= 1) {
                        stack.Push(nodes[i]);
                    }
                }
            }
        }

        private void AddRootNode() {
            var node = new TreeNode("[root]");
            tv.Nodes.Add(node);
        }

        [NotNull, ItemNotNull]
        private readonly List<TreeListItem> _items;

        [NotNull]
        private readonly Dictionary<string, TreeNode> _hierarchy;

    }
}
