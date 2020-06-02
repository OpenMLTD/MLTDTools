using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace OpenMLTD.ManifestTools.UI {
    public partial class AssetTreeList : UserControl {

        public AssetTreeList() {
            _items = new List<TreeListItem>();
            _hierarchy = new Dictionary<string, TreeNode>();

            InitializeComponent();
            RegisterEventHandlers();

            tv.Visible = false;
            _view = TreeListView.ListView;

            InitializeControls();
        }

        ~AssetTreeList() {
            UnregisterEventHandlers();
        }

        public TreeListView View {
            get => _view;
            set {
                if (value == _view) {
                    return;
                }

                switch (value) {
                    case TreeListView.ListView:
                        lv.Visible = true;
                        tv.Visible = false;
                        break;
                    case TreeListView.TreeView:
                        tv.Visible = true;
                        lv.Visible = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }

                _view = value;
            }
        }

        private void InitializeControls() {
            lv.Columns.Add("Local Name");
            lv.Columns.Add("Remote Name");
            lv.Columns.Add("Hash");
            lv.Columns.Add("Size");

            AddRootNode();
        }

        private void RegisterEventHandlers() {
            Resize += UserControl_Resize;
            lv.MouseDoubleClick += Lv_MouseDoubleClick;
            lv.MouseUp += Lv_MouseUp;
            tv.MouseDoubleClick += Tv_MouseDoubleClick;
            tv.MouseUp += Tv_MouseUp;
        }

        private void UnregisterEventHandlers() {
            Resize -= UserControl_Resize;
            lv.MouseDoubleClick -= Lv_MouseDoubleClick;
            lv.MouseUp -= Lv_MouseUp;
            tv.MouseDoubleClick -= Tv_MouseDoubleClick;
            tv.MouseUp -= Tv_MouseUp;
        }

        private void UserControl_Resize(object sender, EventArgs e) {
            var location = Point.Empty;
            var size = ClientSize;

            lv.Location = location;
            lv.Size = size;
            tv.Location = location;
            tv.Size = size;
        }

        private void Lv_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left && lv.SelectedItems.Count > 0) {
                ItemDoubleClick?.Invoke(lv, e);
            }
        }

        private void Lv_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right && lv.SelectedItems.Count > 0) {
                ItemsContextRequested?.Invoke(lv, e);
            }
        }

        private void Tv_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                var selectedNode = tv.SelectedNode;

                // Only raise on leaf nodes
                if (selectedNode != null && selectedNode.Nodes.Count == 0) {
                    ItemDoubleClick?.Invoke(tv, e);
                }
            }
        }

        private void Tv_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right && tv.SelectedNode != null) {
                ItemsContextRequested?.Invoke(tv, e);
            }
        }

        private TreeListView _view;

    }
}
