using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Core;

namespace OpenMLTD.MillionDance {
    public partial class FMain : Form {

        internal FMain() {
            InitializeComponent();
            RegisterEventHandlers();
        }

        ~FMain() {
            UnregisterEventHandlers();
        }

        #region Events

        private void UnregisterEventHandlers() {
            Load -= FMain_Load;
            chkGenerateModel.CheckedChanged -= ChkGenerateModel_CheckedChanged;
            chkGenerateCharAnim.CheckedChanged -= ChkGenerateCharAnim_CheckedChanged;
            chkGenerateCameraMotion.CheckedChanged -= ChkGenerateCameraMotion_CheckedChanged;
            radOptMotionSourceMltd.CheckedChanged -= RadOptMotionSourceMltd_CheckedChanged;
            radOptCamFormatVmd.CheckedChanged -= RadOptCamFormatVmd_CheckedChanged;
            btnGo.Click -= BtnGo_Click;
            btnInputHead.Click -= BtnInputHead_Click;
            btnInputBody.Click -= BtnInputBody_Click;
            btnInputDance.Click -= BtnInputDance_Click;
            btnInputFacialExpression.Click -= BtnInputFacialExpression_Click;
            btnInputCamera.Click -= BtnInputCamera_Click;
            btnOutputModel.Click -= BtnOutputModel_Click;
            btnOutputCharAnim.Click -= BtnOutputCharAnim_Click;
            btnOutputCameraMotion.Click -= BtnOutputCameraMotion_Click;
            chkOptApplyCharHeight.CheckedChanged -= ChkOptApplyCharHeight_CheckedChanged;
            chkOptScalePmx.CheckedChanged -= ChkOptScalePmx_CheckedChanged;
            btnClearLog.Click -= BtnClearLog_Click;
            btnOptSelectFEMappings.Click -= BtnOptSelectFEMappings_Click;
        }

        private void RegisterEventHandlers() {
            Load += FMain_Load;
            chkGenerateModel.CheckedChanged += ChkGenerateModel_CheckedChanged;
            chkGenerateCharAnim.CheckedChanged += ChkGenerateCharAnim_CheckedChanged;
            chkGenerateCameraMotion.CheckedChanged += ChkGenerateCameraMotion_CheckedChanged;
            radOptMotionSourceMltd.CheckedChanged += RadOptMotionSourceMltd_CheckedChanged;
            radOptCamFormatVmd.CheckedChanged += RadOptCamFormatVmd_CheckedChanged;
            btnGo.Click += BtnGo_Click;
            btnInputHead.Click += BtnInputHead_Click;
            btnInputBody.Click += BtnInputBody_Click;
            btnInputDance.Click += BtnInputDance_Click;
            btnInputFacialExpression.Click += BtnInputFacialExpression_Click;
            btnInputCamera.Click += BtnInputCamera_Click;
            btnOutputModel.Click += BtnOutputModel_Click;
            btnOutputCharAnim.Click += BtnOutputCharAnim_Click;
            btnOutputCameraMotion.Click += BtnOutputCameraMotion_Click;
            chkOptApplyCharHeight.CheckedChanged += ChkOptApplyCharHeight_CheckedChanged;
            chkOptScalePmx.CheckedChanged += ChkOptScalePmx_CheckedChanged;
            btnClearLog.Click += BtnClearLog_Click;
            btnOptSelectFEMappings.Click += BtnOptSelectFEMappings_Click;
        }

        private void BtnOptSelectFEMappings_Click(object sender, EventArgs e) {
            var (path, ok) = SelectOpenFile("JSON (*.json)|*.json");

            if (!ok) {
                return;
            }

            txtOptFEMappings.Text = path;
        }

        private void BtnClearLog_Click(object sender, EventArgs e) {
            txtLog.ResetText();
        }

        private void ChkOptScalePmx_CheckedChanged(object sender, EventArgs e) {
            var b = chkOptScalePmx.Checked;

            chkOptApplyCharHeight.Enabled = b;

            var b2 = chkOptApplyCharHeight.Checked;

            txtOptCharHeight.Enabled = b && b2;
            label15.Enabled = b && b2;
            label16.Enabled = b && b2;
        }

        private void ChkOptApplyCharHeight_CheckedChanged(object sender, EventArgs e) {
            var b = chkOptApplyCharHeight.Checked;
            txtOptCharHeight.Enabled = b;
            label15.Enabled = b;
            label16.Enabled = b;
        }

        private void BtnOutputCameraMotion_Click(object sender, EventArgs e) {
            string filter;

            if (radOptCamFormatVmd.Checked) {
                filter = "MikuMikuDance Camera Motion (*.vmd)|*.vmd";
            } else if (radOptCamFormatMvd.Checked) {
                filter = "MikuMikuMoving Camera Motion (*.mvd)|*.mvd";
            } else {
                throw new ApplicationException("Not possible.");
            }

            var (r, ok) = SelectSaveFile(filter);
            if (ok) {
                txtOutputCameraMotion.Text = r;
            }
        }

        private void BtnOutputCharAnim_Click(object sender, EventArgs e) {
            var (r, ok) = SelectSaveFile("MikuMikuDance Model Motion (*.vmd)|*.vmd");
            if (ok) {
                txtOutputCharAnim.Text = r;
            }
        }

        private void BtnOutputModel_Click(object sender, EventArgs e) {
            var (r, ok) = SelectSaveFile("PMX Model 2.0 (*.pmx)|*.pmx");
            if (ok) {
                txtOutputModel.Text = r;
            }
        }

        private void BtnInputCamera_Click(object sender, EventArgs e) {
            var (r, ok) = SelectOpenFile("Camera Asset Bundle (cam_*.unity3d)|cam_*.unity3d");
            if (ok) {
                txtInputCamera.Text = r;
            }
        }

        private void BtnInputFacialExpression_Click(object sender, EventArgs e) {
            var (r, ok) = SelectOpenFile("Song Control Asset Bundle (scrobj_*.unity3d)|scrobj_*.unity3d");
            if (ok) {
                txtInputFacialExpression.Text = r;
            }
        }

        private void BtnInputDance_Click(object sender, EventArgs e) {
            var (r, ok) = SelectOpenFile("Dance Asset Bundle (dan_*.unity3d)|dan_*.unity3d");
            if (ok) {
                txtInputDance.Text = r;
            }
        }

        private void BtnInputBody_Click(object sender, EventArgs e) {
            var (r, ok) = SelectOpenFile("Character Body Asset Bundle (cb_*.unity3d)|cb_*.unity3d");
            if (ok) {
                txtInputBody.Text = r;
            }
        }

        private void BtnInputHead_Click(object sender, EventArgs e) {
            var (r, ok) = SelectOpenFile("Character Head Asset Bundle (ch_*.unity3d)|ch_*.unity3d");
            if (ok) {
                txtInputHead.Text = r;
            }
        }

        private void FMain_Load(object sender, EventArgs e) {
            cboOptSongPosition.SelectedIndex = 0;

            // globalization: Decimal point affects parsing result. Bear in mind.
            // In custom numeric format string, the "." means decimal point, which will be correctly translated to target culture.
            var defaultCharHeightStr = ScalingConfig.StandardCharacterHeight.ToString("0.00", GlobalizationHelper.Culture);

            label16.Text = $"(standard = {defaultCharHeightStr})";
            txtOptCharHeight.Text = defaultCharHeightStr;

            Trace.Listeners.Add(new TextBoxTracer(this));
        }

        private void BtnGo_Click(object sender, EventArgs e) {
            void Alert(string text) {
                MessageBox.Show(text, ApplicationHelper.GetApplicationTitle(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            bool Confirm(string text) {
                var r = MessageBox.Show(text, ApplicationHelper.GetApplicationTitle(), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                return r == DialogResult.Yes;
            }

            bool CheckInputParams() {
                bool CheckOutputDir(string filePath) {
                    var outputDirExists = !string.IsNullOrEmpty(filePath);

                    if (outputDirExists) {
                        var file = new FileInfo(filePath);
                        var dir = file.Directory;
                        outputDirExists = dir != null && dir.Exists;
                    }

                    if (!outputDirExists) {
                        Alert($"Output folder of \"{filePath}\" does not exist.");
                    }

                    return outputDirExists;
                }

                if (!File.Exists(txtInputHead.Text)) {
                    Alert($"Head model \"{txtInputHead.Text}\" does not exist.");
                    return false;
                }

                if (!File.Exists(txtInputBody.Text)) {
                    Alert($"Body model \"{txtInputBody.Text}\" does not exist.");
                    return false;
                }

                if (chkGenerateModel.Checked) {
                    if (!CheckOutputDir(txtOutputModel.Text)) {
                        return false;
                    }
                }

                if (chkOptApplyCharHeight.Checked) {
                    if (!float.TryParse(txtOptCharHeight.Text, NumberStyles.Float, GlobalizationHelper.Culture, out var height) || height <= 0.1f) {
                        Alert("Please enter a valid idol height.");
                    }

                    if (height < 1.0f || 2.0f < height) {
                        var heightStr = height.ToString(GlobalizationHelper.Culture);
                        // Don't mind multiple string concatenations. It only executes once. :D
                        var message = "You entered a strange value for idol height." +
                                      " This may be caused by the differences of decimal point among countries." +
                                      " A wrong value may let this program generate wrong mesh and bones for the model." +
                                      Environment.NewLine + Environment.NewLine +
                                      $"The number in the numeric format of your culture ({GlobalizationHelper.Culture.DisplayName}) is: {heightStr}" +
                                      Environment.NewLine + Environment.NewLine +
                                      "Are you sure that you entered a correct value?";

                        if (!Confirm(message)) {
                            return false;
                        }
                    }
                }

                if (chkGenerateCharAnim.Checked) {
                    if (!File.Exists(txtInputDance.Text)) {
                        Alert($"Dance data \"{txtInputDance.Text}\" does not exist.");
                        return false;
                    }

                    if (!File.Exists(txtInputFacialExpression.Text)) {
                        Alert($"Facial expression data \"{txtInputFacialExpression.Text}\" does not exist.");
                        return false;
                    }

                    if (!CheckOutputDir(txtOutputCharAnim.Text)) {
                        return false;
                    }

                    if (!File.Exists(txtInputFacialExpression.Text)) {
                        Alert($"Facial expression mapping file \"{txtOptFEMappings.Text}\" does not exist.");
                        return false;
                    }
                }

                if (chkGenerateCameraMotion.Checked) {
                    if (!File.Exists(txtInputCamera.Text)) {
                        Alert($"Camera data \"{txtInputCamera.Text}\" does not exist.");
                        return false;
                    }

                    if (!CheckOutputDir(txtOutputCameraMotion.Text)) {
                        return false;
                    }
                }

                if (radOptCamFormatVmd.Checked) {
                    // globalization: The type is uint32 so only thousand separator appears. In either way,
                    // it will raise a parse error if the user does not conform the UI locale (e.g. input
                    // "12,345" in French locale, or "12.345" in US locale).
                    if (!uint.TryParse(txtOptFixedFov.Text, NumberStyles.Integer, GlobalizationHelper.Culture, out _)) {
                        Alert($"FOV value \"{txtOptFixedFov.Text}\" should be a valid positive integer.");
                        return false;
                    }
                }

                if (!Regex.IsMatch(txtInputHead.Text, @"ch_[a-z]{2}\d{3}_(?:\d{3}[a-z]{3}|[a-z])\.unity3d$", RegexOptions.CultureInvariant)) {
                    Alert($"File \"{txtInputHead.Text}\" does not look like a character head file from the game.");
                    return false;
                }

                if (!Regex.IsMatch(txtInputBody.Text, @"cb_[a-z]{2}\d{3}_(?:\d{3}[a-z]{3}|[a-z])\.unity3d$", RegexOptions.CultureInvariant)) {
                    Alert($"File \"{txtInputBody.Text}\" does not look like a character body file from the game.");
                    return false;
                }

                if (chkGenerateCharAnim.Checked) {
                    if (!Regex.IsMatch(txtInputDance.Text, @"dan_[a-z0-9]{6}_0[12345]\.imo\.unity3d$", RegexOptions.CultureInvariant)) {
                        Alert($"File \"{txtInputDance.Text}\" does not look like a dance data file from the game.");
                        return false;
                    }

                    if (!Regex.IsMatch(txtInputFacialExpression.Text, @"scrobj_[a-z0-9]{6}\.unity3d$", RegexOptions.CultureInvariant)) {
                        Alert($"File \"{txtInputFacialExpression.Text}\" does not look like a mixed data file from the game containing facial expressions.");
                        return false;
                    }
                }

                if (chkGenerateCameraMotion.Checked) {
                    if (!Regex.IsMatch(txtInputCamera.Text, @"cam_[a-z0-9]{6}\.imo\.unity3d$", RegexOptions.CultureInvariant)) {
                        Alert($"File \"{txtInputCamera.Text}\" does not look like a camera data file from the game.");
                        return false;
                    }

                    if (radOptCamFormatVmd.Checked) {
                        if (!txtOutputCameraMotion.Text.EndsWith(".vmd", StringComparison.OrdinalIgnoreCase)) {
                            if (!Confirm("The output camera file name does not ends with \".vmd\". Are you sure to continue?")) {
                                return false;
                            }
                        }
                    } else if (radOptCamFormatMvd.Checked) {
                        if (!txtOutputCameraMotion.Text.EndsWith(".mvd", StringComparison.OrdinalIgnoreCase)) {
                            if (!Confirm("The output camera file name does not ends with \".mvd\". Are you sure to continue?")) {
                                return false;
                            }
                        }
                    }
                }

                return true;
            }

            InputParams PrepareInputParams() {
                var ip = new InputParams();

                ip.GenerateModel = chkGenerateModel.Checked;
                ip.GenerateCharacterMotion = chkGenerateCharAnim.Checked;
                ip.GenerateCameraMotion = chkGenerateCameraMotion.Checked;

                ip.InputHead = txtInputHead.Text;
                ip.InputBody = txtInputBody.Text;
                ip.InputDance = txtInputDance.Text;
                ip.InputFacialExpression = txtInputFacialExpression.Text;
                ip.InputCamera = txtInputCamera.Text;

                ip.OutputModel = txtOutputModel.Text;
                ip.OutputCharacterAnimation = txtOutputCharAnim.Text;
                ip.OutputCamera = txtOutputCameraMotion.Text;

                ip.MotionSource = radOptMotionSourceMltd.Checked ? MotionFormat.Mltd : MotionFormat.Mmd;
                ip.ScalePmx = chkOptScalePmx.Checked;
                ip.ConsiderIdolHeight = chkOptApplyCharHeight.Checked;
                ip.IdolHeight = ip.ConsiderIdolHeight ? Convert.ToSingle(txtOptCharHeight.Text) : 0;
                ip.TranslateBoneNames = chkOptTranslateBoneNames.Checked;
                ip.AppendLegIkBones = chkOptAppendLegIKBones.Checked;
                ip.FixCenterBones = chkOptFixCenterBones.Checked;
                ip.ConvertBindingPose = chkOptConvertToTdaPose.Checked;
                ip.AppendEyeBones = chkOptAppendEyeBones.Checked;
                ip.HideUnityGeneratedBones = chkOptHideUnityGenBones.Checked;
                ip.TranslateFacialExpressionNames = chkOptTranslateFacialExpressionNames.Checked;
                ip.ImportPhysics = chkOptImportPhysics.Checked;

                ip.TransformTo30Fps = radOptAnimFrameRate30.Checked;
                ip.ScaleVmd = chkOptScaleVmd.Checked;
                ip.UseMvd = radOptCamFormatMvd.Checked;
                ip.FixedFov = ip.UseMvd ? Convert.ToUInt32(txtOptFixedFov.Text) : 0;
                ip.SongPosition = cboOptSongPosition.SelectedIndex + 1;

                ip.FacialExpressionMappingFilePath = txtOptFEMappings.Text;

                return ip;
            }

            InputParams p;

            try {
                if (!CheckInputParams()) {
                    return;
                }

                p = PrepareInputParams();
            } catch (Exception ex) {
                Alert(ex.Message);
                return;
            }

            var thread = new Thread(DoWork);

            thread.Name = "Conversion thread";
            thread.IsBackground = true;

            thread.Start(p);

            EnableMainControls(false);
        }

        private void RadOptCamFormatVmd_CheckedChanged(object sender, EventArgs e) {
            var b = radOptCamFormatVmd.Checked;
            txtOptFixedFov.Enabled = b;
            label12.Enabled = b;
            label17.Enabled = b;
        }

        private void RadOptMotionSourceMltd_CheckedChanged(object sender, EventArgs e) {
            var b = radOptMotionSourceMltd.Checked;

            chkOptAppendLegIKBones.Enabled = !b;
            chkOptFixCenterBones.Enabled = !b;
            chkOptConvertToTdaPose.Enabled = !b;
            chkOptAppendEyeBones.Enabled = !b;

            chkOptAppendLegIKBones.Checked = !b;
            chkOptFixCenterBones.Checked = !b;
            chkOptConvertToTdaPose.Checked = !b;
            chkOptAppendEyeBones.Checked = !b;
        }

        private void ChkGenerateCameraMotion_CheckedChanged(object sender, EventArgs e) {
            var b = chkGenerateCameraMotion.Checked;
            txtOutputCameraMotion.Enabled = b;
            btnOutputCameraMotion.Enabled = b;
            txtInputCamera.Enabled = b;
            btnInputCamera.Enabled = b;

            ValidateHasAtLeastOneTask();
        }

        private void ChkGenerateCharAnim_CheckedChanged(object sender, EventArgs e) {
            var b = chkGenerateCharAnim.Checked;
            txtOutputCharAnim.Enabled = b;
            btnOutputCharAnim.Enabled = b;
            txtInputDance.Enabled = b;
            btnInputDance.Enabled = b;
            txtInputFacialExpression.Enabled = b;
            btnInputFacialExpression.Enabled = b;

            if (!radOptMotionSourceMltd.Checked) {
                radOptMotionSourceMltd.Checked = true;
            }

            radOptMotionSourceMmd.Enabled = !b;

            ValidateHasAtLeastOneTask();
        }

        private void ChkGenerateModel_CheckedChanged(object sender, EventArgs e) {
            var b = chkGenerateModel.Checked;
            txtOutputModel.Enabled = b;
            btnOutputModel.Enabled = b;

            ValidateHasAtLeastOneTask();
        }

        #endregion

        private void Log([NotNull] string text) {
            if (InvokeRequired) {
                Invoke(() => {
                    Log(text);
                });

                return;
            }

            var now = DateTime.Now;
            var timeStr = now.ToString("T");

            var log = $"{timeStr} {text}";

            var origTextLength = txtLog.TextLength;

            if (origTextLength > 0) {
                log = Environment.NewLine + log;
            }

            txtLog.AppendText(log);
            txtLog.SelectionStart = origTextLength + log.Length;
        }

        private void ValidateHasAtLeastOneTask() {
            var b = chkGenerateModel.Checked || chkGenerateCharAnim.Checked || chkGenerateCameraMotion.Checked;

            txtInputHead.Enabled = b;
            btnInputHead.Enabled = b;
            txtInputBody.Enabled = b;
            btnInputBody.Enabled = b;
            tabControl1.Enabled = b;
            btnGo.Enabled = b;
        }

        private void EnableMainControls(bool enabled) {
            groupBox1.Enabled = enabled;
            groupBox2.Enabled = enabled;
            groupBox3.Enabled = enabled;
            tabControl1.Enabled = enabled;
            btnGo.Enabled = enabled;
            btnClearLog.Enabled = enabled;
        }

        private (string Result, bool OK) SelectOpenFile([NotNull] string filter) {
            ofd.CheckFileExists = true;
            ofd.DereferenceLinks = true;
            ofd.FileName = string.Empty;
            ofd.Filter = filter;
            ofd.FilterIndex = 0;
            ofd.Multiselect = false;
            ofd.ReadOnlyChecked = false;
            ofd.ShowReadOnly = false;
            ofd.SupportMultiDottedExtensions = true;
            ofd.ValidateNames = true;

            var r = ofd.ShowDialog(this);

            if (r == DialogResult.Cancel) {
                return (string.Empty, false);
            } else {
                return (ofd.FileName, true);
            }
        }

        private (string Result, bool OK) SelectSaveFile([NotNull] string filter) {
            sfd.DereferenceLinks = true;
            sfd.FileName = string.Empty;
            sfd.Filter = filter;
            sfd.FilterIndex = 0;
            sfd.OverwritePrompt = true;
            sfd.SupportMultiDottedExtensions = true;
            sfd.ValidateNames = true;

            var r = sfd.ShowDialog(this);

            if (r == DialogResult.Cancel) {
                return (string.Empty, false);
            } else {
                return (sfd.FileName, true);
            }
        }

    }
}
