using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace OpenMLTD.ManifestTools.UI {
    partial class AssetTreeList {

        private sealed class ListViewItemSorter : IComparer {

            private ListViewItemSorter() {
            }

            public int Compare(object x, object y) {
                var a = x as ListViewItem;

                Debug.Assert(a != null, nameof(a) + " != null");

                var b = y as ListViewItem;

                Debug.Assert(b != null, nameof(b) + " != null");

                return string.CompareOrdinal(a.Text, b.Text);
            }

            [NotNull]
            public static readonly ListViewItemSorter Instance = new ListViewItemSorter();

        }

    }
}
