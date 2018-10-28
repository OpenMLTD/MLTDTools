using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JetBrains.Annotations;
using Newtonsoft.Json;
using OpenMLTD.MLTDTools.Applications.TDFacial.Entities;

namespace OpenMLTD.MLTDTools.Applications.TDFacial {
    internal partial class Form1 : Form {

        public Form1() {
            InitializeComponent();
            RegisterEventHandlers();
        }

        private void RegisterEventHandlers() {
            Load += Form1_Load;
            lvExpressionData.SubItemClicked += LvExpressionData_SubItemClicked;
            lvExpressionData.SubItemEndEditing += LvExpressionData_SubItemEndEditing;
            btnSaveConfig.Click += BtnSaveConfig_Click;
            btnLoadConfig.Click += BtnLoadConfig_Click;
            lvExpressions.SelectedIndexChanged += LvExpressions_SelectedIndexChanged;
            btnExprAdd.Click += BtnExprAdd_Click;
            btnExprDel.Click += BtnExprDel_Click;
            btnExprMod.Click += BtnExprMod_Click;
        }

        private void BtnExprMod_Click(object sender, EventArgs e) {
            var expr = GetSelectedFacialExpression();

            if (expr == null) {
                return;
            }

            // Validate...
            {
                var b = int.TryParse(txtExprKey.Text, NumberStyles.Integer, LocalCulture, out var n);

                if (!b || n < 0) {
                    MessageBox.Show("Please enter a non-negative integer.", DefaultTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                foreach (var expr2 in _config.Expressions) {
                    if (expr2 == expr) {
                        continue;
                    }

                    if (n == expr2.Key) {
                        MessageBox.Show($"The key '{n}' has already been taken. Please use another key.", DefaultTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            expr.Key = int.Parse(txtExprKey.Text);
            expr.Description = txtExprDesc.Text;

            var index = GetSelectedFacialExpressionIndex();

            UpdateExpressionList();

            if (0 <= index && index < lvExpressions.Items.Count) {
                lvExpressions.Items[index].Selected = true;
            }
        }

        private void BtnExprDel_Click(object sender, EventArgs e) {
            var index = GetSelectedFacialExpressionIndex();

            if (index < 0 || index >= _config.Expressions.Count) {
                return;
            }

            _config.Expressions.RemoveAt(index);

            UpdateExpressionDataList();
        }

        private void BtnExprAdd_Click(object sender, EventArgs e) {
            var exp = GetSelectedFacialExpression();

            if (exp == null) {
                return;
            }

            var exp2 = exp.Clone();

            // Find the next available key.
            {
                var set = new HashSet<int>();

                foreach (var t in _config.Expressions) {
                    set.Add(t.Key);
                }

                var newKey = exp.Key + 1;

                while (set.Contains(newKey)) {
                    newKey = newKey + 1;
                }

                exp2.Key = newKey;
            }

            _config.Expressions.Add(exp2);

            UpdateExpressionList();

            lvExpressions.Items[lvExpressions.Items.Count - 1].Selected = true;
        }

        private void LvExpressions_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateExpressionDataList();

            var expression = GetSelectedFacialExpression();

            if (expression != null) {
                txtExprKey.Text = expression.Key.ToString(LocalCulture);
                txtExprDesc.Text = expression.Description;
            } else {
                txtExprKey.ResetText();
                txtExprDesc.ResetText();
            }
        }

        private void BtnLoadConfig_Click(object sender, EventArgs e) {
            ofd.CheckFileExists = true;
            ofd.ValidateNames = true;
            ofd.ShowReadOnly = false;
            ofd.ReadOnlyChecked = false;
            ofd.Multiselect = false;
            ofd.Filter = "JSON (*.json)|*.json";

            var r = ofd.ShowDialog(this);

            if (r == DialogResult.Cancel) {
                return;
            }

            var fileContent = File.ReadAllText(ofd.FileName, Encoding.UTF8);
            var obj = JsonConvert.DeserializeObject<FacialConfig>(fileContent);

            _config = obj;

            UpdateExpressionList();
            UpdateExpressionDataList();
        }

        private void BtnSaveConfig_Click(object sender, EventArgs e) {
            sfd.ValidateNames = true;
            sfd.OverwritePrompt = true;
            sfd.Filter = "JSON (*.json)|*.json";

            var r = sfd.ShowDialog(this);

            if (r == DialogResult.Cancel) {
                return;
            }

            var serialized = JsonConvert.SerializeObject(_config);

            File.WriteAllText(sfd.FileName, serialized, Utf8WithoutBom);

            MessageBox.Show($"Configuration saved to {sfd.FileName}.", DefaultTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LvExpressionData_SubItemEndEditing(object sender, SubItemEndEditingEventArgs e) {
            if (e.SubItem != EditableColumnIndex) {
                return;
            }

            var b = float.TryParse(e.DisplayText, NumberStyles.Float, LocalCulture, out var fl);

            if (!b || (fl < 0 || 1 < fl)) {
                MessageBox.Show("Please enter a number between 0 and 1.", DefaultTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            } else {
                Task.Delay(TimeSpan.FromSeconds(0.2))
                    .ContinueWith(prevTask => {
                        var action = new Action(() => {
                            lvExpressionData.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        });

                        if (InvokeRequired) {
                            Invoke(action);
                        } else {
                            action();
                        }
                    });

                var expression = GetSelectedFacialExpression();

                if (expression != null) {
                    expression.Data[e.Item.Text] = fl;
                }
            }
        }

        private void LvExpressionData_SubItemClicked(object sender, SubItemEventArgs e) {
            if (e.SubItem != EditableColumnIndex) {
                return;
            }

            lvExpressionData.StartEditing(txtValueEditor, e.Item, e.SubItem);
        }

        private void Form1_Load(object sender, EventArgs e) {
            UpdateExpressionList();

            if (lvExpressions.Items.Count > 0) {
                lvExpressions.Items[0].Selected = true;
            }
        }

        private void UpdateExpressionList() {
            lvExpressions.Items.Clear();

            foreach (var expr in _config.Expressions) {
                var lvi = lvExpressions.Items.Add(expr.Key.ToString());

                lvi.SubItems.Add(expr.Description);
            }

            lvExpressions.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void UpdateExpressionDataList() {
            lvExpressionData.EndEditing(false);

            lvExpressionData.Items.Clear();

            var expression = GetSelectedFacialExpression();

            if (expression == null) {
                return;
            }

            foreach (var expr in expression.Data) {
                var lvi = lvExpressionData.Items.Add(expr.Key);

                lvi.SubItems.Add(expr.Value.ToString(LocalCulture));

                string desc;

                if (Mappings.Descriptions.ContainsKey(expr.Key)) {
                    desc = Mappings.Descriptions[expr.Key];
                } else {
                    desc = string.Empty;
                }

                lvi.SubItems.Add(desc);
            }

            lvExpressionData.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private int GetSelectedFacialExpressionIndex() {
            if (lvExpressions.SelectedIndices.Count == 0) {
                return -1;
            }

            return lvExpressions.SelectedIndices[0];
        }

        [CanBeNull]
        private FacialExpression GetSelectedFacialExpression() {
            var index = GetSelectedFacialExpressionIndex();

            if (index < 0 || index >= _config.Expressions.Count) {
                return null;
            }

            var expression = _config.Expressions[index];

            return expression;
        }

        [NotNull]
        private FacialConfig _config = FacialConfig.CreateKnown();

        private const string DefaultTitle = "Facial Expression Editor";

        private const int EditableColumnIndex = 1;
        private static readonly CultureInfo LocalCulture = CultureInfo.CurrentUICulture;
        private static readonly UTF8Encoding Utf8WithoutBom = new UTF8Encoding(false);

    }
}
