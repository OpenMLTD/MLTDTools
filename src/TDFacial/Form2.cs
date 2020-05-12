using System;
using System.Globalization;
using System.Windows.Forms;
using AssetStudio;
using AssetStudio.Extended.MonoBehaviours.Serialization;
using Imas.Data.Serialized;
using JetBrains.Annotations;
using OpenMLTD.MiriTore;
using OpenMLTD.MLTDTools.Applications.TDFacial.Extensions;

namespace OpenMLTD.MLTDTools.Applications.TDFacial {
    public partial class Form2 : Form {

        public Form2() {
            InitializeComponent();
            RegisterEventHandlers();
        }

        private void RegisterEventHandlers() {
            Load += Form2_Load;
            btnSelectFile.Click += BtnSelectFile_Click;
        }

        private void Form2_Load(object sender, EventArgs e) {
            label3.Text = string.Empty;
            cboFESource.SelectedIndex = 0;
        }

        private void BtnSelectFile_Click(object sender, EventArgs e) {
            ofd.CheckFileExists = true;
            ofd.DereferenceLinks = true;
            ofd.Filter = "Scenario Data (scrobj_*.unity3d)|scrobj_*.unity3d";
            ofd.Multiselect = false;
            ofd.ReadOnlyChecked = false;
            ofd.ShowReadOnly = false;
            ofd.ValidateNames = true;

            var r = ofd.ShowDialog(this);

            if (r == DialogResult.Cancel) {
                return;
            }

            var filePath = ofd.FileName;

            var (main, yoko, tate) = LoadScenario(filePath);

            if (main == null || yoko == null || tate == null) {
                MessageBox.Show($"Failed to load '{filePath}'.", ApplicationHelper.GetApplicationTitle(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ScenarioObject facialExpr;
            string sourceDesc;

            {
                if (main.HasFacialExpressionFrames()) {
                    facialExpr = main;
                    sourceDesc = "base (scenario_sobj)";
                } else {
                    var fes = (FallbackFacialExpressionSource)cboFESource.SelectedIndex;

                    switch (fes) {
                        case FallbackFacialExpressionSource.Landscape: {
                            if (yoko.HasFacialExpressionFrames()) {
                                facialExpr = yoko;
                                sourceDesc = "landscape (scenario_yoko_sobj)";
                            } else if (tate.HasFacialExpressionFrames()) {
                                facialExpr = tate;
                                sourceDesc = "portrait (scenario_tate_sobj)";
                            } else {
                                facialExpr = null;
                                sourceDesc = null;
                            }

                            break;
                        }
                        case FallbackFacialExpressionSource.Portrait: {
                            if (tate.HasFacialExpressionFrames()) {
                                facialExpr = tate;
                                sourceDesc = "portrait (scenario_tate_sobj)";
                            } else if (yoko.HasFacialExpressionFrames()) {
                                facialExpr = yoko;
                                sourceDesc = "landscape (scenario_yoko_sobj)";
                            } else {
                                facialExpr = null;
                                sourceDesc = null;
                            }

                            break;
                        }
                        default:
                            throw new ArgumentOutOfRangeException(nameof(fes), fes, "Invalid facial expression source.");
                    }
                }
            }

            if (facialExpr == null) {
                return;
            }

            txtFilePath.Text = filePath;
            label3.Text = $"Using facial expressions in: {sourceDesc}";

            FillList(facialExpr);
        }

        private void FillList([NotNull] ScenarioObject scrobj) {
            lv.Items.Clear();

            var invariantCulture = CultureInfo.InvariantCulture;

            foreach (var obj in scrobj.Scenario) {
                var ts = TimeSpan.FromSeconds(obj.AbsoluteTime);

                var lvi = new ListViewItem(ts.ToString("g"));
                lvi.SubItems.Add(obj.AbsoluteTime.ToString(invariantCulture));
                lvi.SubItems.Add(obj.Param.ToString());
                lvi.SubItems.Add(obj.Idol.ToString());

                lv.Items.Add(lvi);
            }
        }

        private static (ScenarioObject, ScenarioObject, ScenarioObject) LoadScenario([NotNull] string filePath) {
            ScenarioObject main = null, landscape = null, portrait = null;

            const string mainScenarioEnds = "scenario_sobj";
            const string landscapeScenarioEnds = "scenario_yoko_sobj";
            const string portraitScenarioEnds = "scenario_tate_sobj";

            var manager = new AssetsManager();
            manager.LoadFiles(filePath);

            var ser = new ScriptableObjectSerializer();

            foreach (var assetFile in manager.assetsFileList) {
                foreach (var obj in assetFile.Objects) {
                    if (obj.type != ClassIDType.MonoBehaviour) {
                        continue;
                    }

                    var behaviour = obj as MonoBehaviour;

                    if (behaviour == null) {
                        throw new ArgumentException("An object serialized as MonoBehaviour is actually not a MonoBehaviour.");
                    }

                    if (behaviour.m_Name.EndsWith(mainScenarioEnds)) {
                        main = ser.Deserialize<ScenarioObject>(behaviour);
                    } else if (behaviour.m_Name.EndsWith(landscapeScenarioEnds)) {
                        landscape = ser.Deserialize<ScenarioObject>(behaviour);
                    } else if (behaviour.m_Name.EndsWith(portraitScenarioEnds)) {
                        portrait = ser.Deserialize<ScenarioObject>(behaviour);
                    }

                    if (main != null && landscape != null && portrait != null) {
                        break;
                    }
                }
            }

            return (main, landscape, portrait);
        }

        private enum FallbackFacialExpressionSource {

            Landscape = 0,

            Portrait = 1,

        }

    }
}
