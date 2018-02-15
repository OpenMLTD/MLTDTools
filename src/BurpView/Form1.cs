using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace BurpView {
    public partial class Form1 : Form {

        public Form1() {
            InitializeComponent();
            RegisterEventHandlers();
        }

        private void RegisterEventHandlers() {
            mnuFileOpen.Click += MnuFileOpen_Click;
            listView2.SelectedIndexChanged += ListView2_SelectedIndexChanged;
            button1.Click += Button1_Click;
            button2.Click += Button2_Click;
            button3.Click += Button3_Click;
        }

        private void Button3_Click(object sender, EventArgs e) {
            var selected = textBox2.SelectedText;

            if (selected.Length == 0) {
                return;
            }

            textBox3.Text = selected;

            Button1_Click(null, null);
        }

        private void Button2_Click(object sender, EventArgs e) {
            var selected = textBox1.SelectedText;

            if (selected.Length == 0) {
                return;
            }

            textBox3.Text = selected;

            Button1_Click(null, null);
        }

        private void Button1_Click(object sender, EventArgs e) {
            var encryptedBase64 = textBox3.Text.Trim();

            string decryptedRaw;
            try {
                decryptedRaw = Decrypt(encryptedBase64);
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            textBox4.Text = decryptedRaw;
            tabControl1.SelectedIndex = 3;
            tabPage4.Select();
        }

        private void ListView2_SelectedIndexChanged(object sender, EventArgs e) {
            if (listView2.SelectedIndices.Count == 0) {
                return;
            }

            var index = listView2.SelectedIndices[0];

            if (index < 0) {
                return;
            }

            UpdateDetails(index);
        }

        private void MnuFileOpen_Click(object sender, EventArgs e) {
            var dlg = openFileDialog1;

            dlg.Filter = "BurpSuite Exported XML (*.xml)|*.xml";
            dlg.CheckFileExists = true;
            dlg.ValidateNames = true;
            dlg.DereferenceLinks = true;
            dlg.ShowReadOnly = false;

            var r = dlg.ShowDialog(this);

            if (r == DialogResult.Cancel) {
                return;
            }

            LoadEntries(dlg.FileName);
        }

        private void LoadEntries(string filePath) {
            var fileContent = File.ReadAllText(filePath).Replace("?xml version=\"1.1\"", "?xml version=\"1.0\"");

            var xml = new XmlDocument();
            xml.LoadXml(fileContent);

            var itemsNode = xml.SelectSingleNode("/items");

            if (itemsNode == null) {
                MessageBox.Show("<items> node is null!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _items.Clear();

            foreach (var itemNodeAbs in itemsNode.SelectNodes("item")) {
                var itemNode = (XmlNode)itemNodeAbs;
                var burpItem = new BurpItem();

                burpItem.Time = BurpTimeParsing.Parse(itemNode.SelectSingleNode("time").InnerText);
                burpItem.Url = itemNode.SelectSingleNode("url").InnerText;
                burpItem.HostIP = itemNode.SelectSingleNode("host").Attributes["ip"].Value;
                burpItem.Host = itemNode.SelectSingleNode("host").InnerText;
                burpItem.Port = Convert.ToInt32(itemNode.SelectSingleNode("port").InnerText);
                burpItem.Protocol = itemNode.SelectSingleNode("protocol").InnerText;
                burpItem.Method = itemNode.SelectSingleNode("method").InnerText;
                burpItem.Path = itemNode.SelectSingleNode("path").InnerText;
                burpItem.Request = itemNode.SelectSingleNode("request").InnerText;
                burpItem.Status = Convert.ToInt32(itemNode.SelectSingleNode("status").InnerText);
                burpItem.ResponseLength = Convert.ToInt32(itemNode.SelectSingleNode("responselength").InnerText);
                burpItem.MimeType = itemNode.SelectSingleNode("mimetype").InnerText;
                burpItem.Response = itemNode.SelectSingleNode("response").InnerText;

                var isRequestEncoded = itemNode.SelectSingleNode("request").Attributes["base64"]?.Value == "true";
                var isResponseEncoded = itemNode.SelectSingleNode("response").Attributes["base64"]?.Value == "true";

                if (isRequestEncoded) {
                    burpItem.Request = Encoding.UTF8.GetString(Convert.FromBase64String(burpItem.Request));
                }

                if (isResponseEncoded) {
                    burpItem.Response = Encoding.UTF8.GetString(Convert.FromBase64String(burpItem.Response));
                }

                _items.Add(burpItem);
            }

            _items.Sort();

            RefreshItemsList();
        }

        private void RefreshItemsList() {
            listView2.Items.Clear();

            foreach (var item in _items) {
                var lvi = listView2.Items.Add(item.ResponseLength.ToString());
                lvi.SubItems.Add(item.Url);
                lvi.SubItems.Add(item.Status.ToString());
            }
        }

        private void UpdateDetails(int index) {
            var item = _items[index];

            SetItemText(0, item.Time.ToString(CultureInfo.InvariantCulture));
            SetItemText(1, item.Url);
            SetItemText(2, item.Host);
            SetItemText(3, item.HostIP);
            SetItemText(4, item.Port.ToString());
            SetItemText(5, item.Protocol);
            SetItemText(6, item.Method);
            SetItemText(7, item.Path);
            SetItemText(8, item.Status.ToString());
            SetItemText(9, item.ResponseLength.ToString());
            SetItemText(10, item.MimeType);

            textBox1.Text = item.Request;
            textBox2.Text = item.Response;

            void SetItemText(int i, string text) {
                var listViewItem = listView1.Items[i];

                if (listViewItem.SubItems.Count == 1) {
                    listViewItem.SubItems.Add(text);
                } else {
                    listViewItem.SubItems[1].Text = text;
                }
            }
        }

        private sealed class BurpItem : IComparable<BurpItem> {

            public DateTime Time;

            public string Url;

            public string Host;

            public string HostIP;

            public int Port;

            public string Protocol;

            public string Method;

            public string Path;

            public string Request;

            public int Status;

            public int ResponseLength;

            public string MimeType;

            public string Response;

            public int CompareTo(BurpItem other) {
                if (other == null) {
                    return 1;
                }

                return Time.CompareTo(other.Time);
            }

        }

        private readonly List<BurpItem> _items = new List<BurpItem>();

    }
}
