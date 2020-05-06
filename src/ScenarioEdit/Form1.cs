using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using AssetStudio;
using AssetStudio.Extended.MonoBehaviours.Serialization;
using OpenMLTD.ScenarioEdit.Entities;

namespace OpenMLTD.ScenarioEdit {
    public partial class Form1 : Form {

        public Form1() {
            InitializeComponent();
            RegisterEventHandlers();
        }

        private void RegisterEventHandlers() {
            button1.Click += Button1_Click;
        }

        private void Button1_Click(object sender, EventArgs e) {
            ofd.Filter = "Unity3D (*.unity3d)|*.unity3d";
            ofd.CheckFileExists = true;
            ofd.ValidateNames = true;
            ofd.Multiselect = false;
            ofd.ReadOnlyChecked = false;
            ofd.ShowReadOnly = false;

            var r = ofd.ShowDialog(this);

            if (r == DialogResult.Cancel) {
                return;
            }

            var filePath = ofd.FileName;

            textBox1.Text = filePath;

            ScenarioObject scenarioObject = null;

            try {
                const string scenarioEnds = "scenario_sobj";

                var manager = new AssetsManager();
                manager.LoadFiles(filePath);

                foreach (var assetFile in manager.assetsFileList) {
                    foreach (var obj in assetFile.Objects) {
                        if (obj.type != ClassIDType.MonoBehaviour) {
                            continue;
                        }

                        var behaviour = obj as MonoBehaviour;

                        if (behaviour == null) {
                            throw new ArgumentException("An object serialized as MonoBehaviour is actually not a MonoBehaviour.");
                        }

                        if (!behaviour.m_Name.EndsWith(scenarioEnds)) {
                            continue;
                        }

                        var ser = new ScriptableObjectSerializer();

                        scenarioObject = ser.Deserialize<ScenarioObject>(behaviour);

                        break;
                    }
                }
            } catch (Exception ex) {
                var error = ex.ToString();

                MessageBox.Show(error, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Debug.Print(error);
            }


            if (scenarioObject != null) {
                foreach (var s in scenarioObject.Scenario.Where(ev => ev.Type == ScenarioDataType.Lyrics)) {
                    Debug.Print("{0} @ {1}", s.Lyrics, s.AbsoluteTime);
                }

                Debug.Print("Success");
            }
        }

    }
}
