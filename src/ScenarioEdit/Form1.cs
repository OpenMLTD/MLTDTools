using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OpenMLTD.ScenarioEdit.Entities;
using UnityStudio.Extensions;
using UnityStudio.Models;
using UnityStudio.Serialization;

namespace OpenMLTD.ScenarioEdit {
    public partial class Form1 : Form {

        public Form1() {
            InitializeComponent();
            RegisterEventHandlers();
        }

        private void RegisterEventHandlers() {
            button1.Click += Button1_Click;
        }

        private void Button1_Click(object sender, System.EventArgs e) {
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
                using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    using (var bundle = new BundleFile(fileStream, false)) {
                        foreach (var asset in bundle.AssetFiles) {
                            foreach (var preloadData in asset.PreloadDataList) {
                                if (preloadData.KnownType != KnownClassID.MonoBehaviour) {
                                    continue;
                                }

                                var behaviour = preloadData.LoadAsMonoBehaviour(true);

                                if (!behaviour.Name.EndsWith("scenario_sobj")) {
                                    continue;
                                }

                                behaviour = preloadData.LoadAsMonoBehaviour(false);

                                var serializer = new MonoBehaviourSerializer();
                                scenarioObject = serializer.Deserialize<ScenarioObject>(behaviour);
                                break;
                            }
                        }
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
