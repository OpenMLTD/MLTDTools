using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

// By mav.northwind
// https://www.codeproject.com/Articles/6646/In-place-Editing-of-ListView-subitems

// ReSharper disable once CheckNamespace
namespace OpenMLTD.MLTDTools.Applications.TDFacial {

    /// <summary>
    /// Event Handler for SubItem events
    /// </summary>
    public delegate void SubItemEventHandler(object sender, SubItemEventArgs e);

    /// <summary>
    /// Event Handler for SubItemEndEditing events
    /// </summary>
    public delegate void SubItemEndEditingEventHandler(object sender, SubItemEndEditingEventArgs e);

    /// <inheritdoc />
    /// <summary>
    /// Event Args for SubItemClicked event
    /// </summary>
    public class SubItemEventArgs : EventArgs {

        public SubItemEventArgs(ListViewItem item, int subItem) {
            SubItem = subItem;
            Item = item;
        }

        public int SubItem { get; } = -1;

        public ListViewItem Item { get; }

    }


    /// <summary>
    /// Event Args for SubItemEndEditingClicked event
    /// </summary>
    public class SubItemEndEditingEventArgs : SubItemEventArgs {

        public SubItemEndEditingEventArgs(ListViewItem item, int subItem, string display, bool cancel)
            : base(item, subItem) {
            DisplayText = display;
            Cancel = cancel;
        }

        public string DisplayText { get; set; } = string.Empty;

        public bool Cancel { get; set; } = true;

    }


    ///	<summary>
    ///	Inherited ListView to allow in-place editing of subitems
    ///	</summary>
    public class ListViewEx : ListView {

        #region Interop structs, imports and constants
        /// <summary>
        /// MessageHeader for WM_NOTIFY
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct NMHDR {

            public IntPtr HwndFrom;
            public int IdFrom;
            public int Code;

        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wPar, IntPtr lPar);
        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int len, ref int[] order);

        // ListView messages
        private const int LVM_FIRST = 0x1000;
        private const int LVM_GETCOLUMNORDERARRAY = (LVM_FIRST + 59);

        // Windows Messages that will abort editing
        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;
        private const int WM_SIZE = 0x05;
        private const int WM_NOTIFY = 0x4E;

        private const int HDN_FIRST = -300;
        private const int HDN_BEGINDRAG = (HDN_FIRST - 10);
        private const int HDN_ITEMCHANGINGA = (HDN_FIRST - 0);
        private const int HDN_ITEMCHANGINGW = (HDN_FIRST - 20);
        #endregion

        ///	<summary>
        ///	Required designer variable.
        ///	</summary>
        private Container _components = null;

        public event SubItemEventHandler SubItemClicked;
        public event SubItemEventHandler SubItemBeginEditing;
        public event SubItemEndEditingEventHandler SubItemEndEditing;

        public ListViewEx() {
            // This	call is	required by	the	Windows.Forms Form Designer.
            InitializeComponent();

            FullRowSelect = true;
            View = View.Details;
            AllowColumnReorder = true;
        }

        /// <inheritdoc />
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                _components?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        ///	<summary>
        ///	Required method	for	Designer support - do not modify 
        ///	the	contents of	this method	with the code editor.
        ///	</summary>
        private void InitializeComponent() {
            _components = new Container();
        }
        #endregion

        /// <summary>
        /// Is a double click required to start editing a cell?
        /// </summary>
        [DefaultValue(false)]
        public bool DoubleClickActivation { get; set; }

        /// <summary>
        /// Retrieve the order in which columns appear
        /// </summary>
        /// <returns>Current display order of column indices</returns>
        public int[] GetColumnOrder() {
            var lPar = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)) * Columns.Count);

            var res = SendMessage(Handle, LVM_GETCOLUMNORDERARRAY, new IntPtr(Columns.Count), lPar);
            if (res.ToInt32() == 0) // Something went wrong
            {
                Marshal.FreeHGlobal(lPar);
                return null;
            }

            var order = new int[Columns.Count];
            Marshal.Copy(lPar, order, 0, Columns.Count);

            Marshal.FreeHGlobal(lPar);

            return order;
        }


        /// <summary>
        /// Find ListViewItem and SubItem Index at position (x,y)
        /// </summary>
        /// <param name="x">relative to ListView</param>
        /// <param name="y">relative to ListView</param>
        /// <param name="item">Item at position (x,y)</param>
        /// <returns>SubItem index</returns>
        public int GetSubItemAt(int x, int y, out ListViewItem item) {
            item = GetItemAt(x, y);

            if (item != null) {
                var order = GetColumnOrder();
                Rectangle lviBounds;
                int subItemX;

                lviBounds = item.GetBounds(ItemBoundsPortion.Entire);
                subItemX = lviBounds.Left;
                for (var i = 0; i < order.Length; i++) {
                    var h = Columns[order[i]];
                    if (x < subItemX + h.Width) {
                        return h.Index;
                    }
                    subItemX += h.Width;
                }
            }

            return -1;
        }


        /// <summary>
        /// Get bounds for a SubItem
        /// </summary>
        /// <param name="item">Target ListViewItem</param>
        /// <param name="subItem">Target SubItem index</param>
        /// <returns>Bounds of SubItem (relative to ListView)</returns>
        public Rectangle GetSubItemBounds(ListViewItem item, int subItem) {
            var order = GetColumnOrder();

            var subItemRect = Rectangle.Empty;
            if (subItem >= order.Length)
                throw new IndexOutOfRangeException($"SubItem {subItem} out of range");

            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var lviBounds = item.GetBounds(ItemBoundsPortion.Entire);
            var subItemX = lviBounds.Left;

            ColumnHeader col;
            int i;
            for (i = 0; i < order.Length; i++) {
                col = Columns[order[i]];
                if (col.Index == subItem)
                    break;
                subItemX += col.Width;
            }
            subItemRect = new Rectangle(subItemX, lviBounds.Top, Columns[order[i]].Width, lviBounds.Height);
            return subItemRect;
        }


        protected override void WndProc(ref Message msg) {
            switch (msg.Msg) {
                // Look	for	WM_VSCROLL,WM_HSCROLL or WM_SIZE messages.
                case WM_VSCROLL:
                case WM_HSCROLL:
                case WM_SIZE:
                    EndEditing(false);
                    break;
                case WM_NOTIFY:
                    // Look for WM_NOTIFY of events that might also change the
                    // editor's position/size: Column reordering or resizing
                    var h = (NMHDR)Marshal.PtrToStructure(msg.LParam, typeof(NMHDR));
                    if (h.Code == HDN_BEGINDRAG ||
                        h.Code == HDN_ITEMCHANGINGA ||
                        h.Code == HDN_ITEMCHANGINGW)
                        EndEditing(false);
                    break;
            }

            base.WndProc(ref msg);
        }


        #region Initialize editing depending of DoubleClickActivation property
        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);

            if (DoubleClickActivation) {
                return;
            }

            EditSubitemAt(new Point(e.X, e.Y));
        }

        protected override void OnDoubleClick(EventArgs e) {
            base.OnDoubleClick(e);

            if (!DoubleClickActivation) {
                return;
            }

            var pt = PointToClient(Cursor.Position);

            EditSubitemAt(pt);
        }

        ///<summary>
        /// Fire SubItemClicked
        ///</summary>
        ///<param name="p">Point of click/doubleclick</param>
        private void EditSubitemAt(Point p) {
            ListViewItem item;
            var idx = GetSubItemAt(p.X, p.Y, out item);
            if (idx >= 0) {
                OnSubItemClicked(new SubItemEventArgs(item, idx));
            }
        }

        #endregion

        #region In-place editing functions
        // The control performing the actual editing
        private Control _editingControl;
        // The LVI being edited
        private ListViewItem _editItem;
        // The SubItem being edited
        private int _editSubItem;

        protected void OnSubItemBeginEditing(SubItemEventArgs e) {
            SubItemBeginEditing?.Invoke(this, e);
        }

        protected void OnSubItemEndEditing(SubItemEndEditingEventArgs e) {
            SubItemEndEditing?.Invoke(this, e);
        }

        protected void OnSubItemClicked(SubItemEventArgs e) {
            SubItemClicked?.Invoke(this, e);
        }


        /// <summary>
        /// Begin in-place editing of given cell
        /// </summary>
        /// <param name="c">Control used as cell editor</param>
        /// <param name="item">ListViewItem to edit</param>
        /// <param name="subItem">SubItem index to edit</param>
        public void StartEditing(Control c, ListViewItem item, int subItem) {
            OnSubItemBeginEditing(new SubItemEventArgs(item, subItem));

            var rcSubItem = GetSubItemBounds(item, subItem);

            if (rcSubItem.X < 0) {
                // Left edge of SubItem not visible - adjust rectangle position and width
                rcSubItem.Width += rcSubItem.X;
                rcSubItem.X = 0;
            }
            if (rcSubItem.X + rcSubItem.Width > Width) {
                // Right edge of SubItem not visible - adjust rectangle width
                rcSubItem.Width = Width - rcSubItem.Left;
            }

            // Subitem bounds are relative to the location of the ListView!
            rcSubItem.Offset(Left, Top);

            // In case the editing control and the listview are on different parents,
            // account for different origins
            var origin = new Point(0, 0);
            var lvOrigin = Parent.PointToScreen(origin);
            var ctlOrigin = c.Parent.PointToScreen(origin);

            rcSubItem.Offset(lvOrigin.X - ctlOrigin.X, lvOrigin.Y - ctlOrigin.Y);

            // Position and show editor
            c.Bounds = rcSubItem;
            c.Text = item.SubItems[subItem].Text;
            c.Visible = true;
            c.BringToFront();
            c.Focus();

            _editingControl = c;
            _editingControl.Leave += _editControl_Leave;
            _editingControl.KeyPress += _editControl_KeyPress;

            _editItem = item;
            _editSubItem = subItem;
        }


        private void _editControl_Leave(object sender, EventArgs e) {
            // cell editor losing focus
            EndEditing(true);
        }

        private void _editControl_KeyPress(object sender, KeyPressEventArgs e) {
            switch (e.KeyChar) {
                case (char)(int)Keys.Escape: {
                        EndEditing(false);
                        e.Handled = true; // prevent the beep
                        break;
                    }

                case (char)(int)Keys.Enter: {
                        EndEditing(true);
                        e.Handled = true;
                        break;
                    }
            }
        }

        /// <summary>
        /// Accept or discard current value of cell editor control
        /// </summary>
        /// <param name="acceptChanges">Use the _editingControl's Text as new SubItem text or discard changes?</param>
        public void EndEditing(bool acceptChanges) {
            if (_editingControl == null)
                return;

            var e = new SubItemEndEditingEventArgs(
                _editItem,      // The item being edited
                _editSubItem,   // The subitem index being edited
                acceptChanges ?
                    _editingControl.Text :  // Use editControl text if changes are accepted
                    _editItem.SubItems[_editSubItem].Text,  // or the original subitem's text, if changes are discarded
                !acceptChanges  // Cancel?
            );

            OnSubItemEndEditing(e);

            if (!e.Cancel) {
                _editItem.SubItems[_editSubItem].Text = e.DisplayText;
            }

            _editingControl.Leave -= _editControl_Leave;
            _editingControl.KeyPress -= _editControl_KeyPress;

            _editingControl.Visible = false;

            _editingControl = null;
            _editItem = null;
            _editSubItem = -1;
        }
        #endregion
    }
}
