using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Core;
using OpenMLTD.MillionDance.Utilities;

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
            chkGenerateLipSync.CheckedChanged -= ChkGenerateLipSync_CheckedChanged;
            chkGenerateFacialExpression.CheckedChanged -= ChkGenerateFacialExpression_CheckedChanged;
            chkGenerateCameraMotion.CheckedChanged -= ChkGenerateCameraMotion_CheckedChanged;
            radOptMotionSourceMltd.CheckedChanged -= RadOptMotionSourceMltd_CheckedChanged;
            radOptCamFormatVmd.CheckedChanged -= RadOptCamFormatVmd_CheckedChanged;
            btnGo.Click -= BtnGo_Click;
            btnInputHead.Click -= BtnInputHead_Click;
            btnInputBody.Click -= BtnInputBody_Click;
            btnInputDance.Click -= BtnInputDance_Click;
            btnInputScenario.Click -= BtnInputScenario_Click;
            btnInputCamera.Click -= BtnInputCamera_Click;
            btnOutputModel.Click -= BtnOutputModel_Click;
            btnOutputCharAnim.Click -= BtnOutputCharAnim_Click;
            btnOutputLipSync.Click -= BtnOutputLipSync_Click;
            btnOutputFacialExpression.Click -= BtnOutputFacialExpression_Click;
            btnOutputCameraMotion.Click -= BtnOutputCameraMotion_Click;
            chkOptApplyCharHeight.CheckedChanged -= ChkOptApplyCharHeight_CheckedChanged;
            chkOptScalePmx.CheckedChanged -= ChkOptScalePmx_CheckedChanged;
            btnClearLog.Click -= BtnClearLog_Click;
            btnOptSelectFEMappings.Click -= BtnOptSelectFEMappings_Click;
            chkGameToon.CheckedChanged -= ChkGameToon_CheckedChanged;
            lnkHelp.LinkClicked -= LnkHelp_LinkClicked;
            cboOptAppealType.SelectedIndexChanged -= CboOptAppealType_SelectedIndexChanged;
            btnOptSelectExternalDanceAppealFile.Click -= BtnOptSelectExternalAppealFile_Click;
        }

        private void RegisterEventHandlers() {
            Load += FMain_Load;
            chkGenerateModel.CheckedChanged += ChkGenerateModel_CheckedChanged;
            chkGenerateCharAnim.CheckedChanged += ChkGenerateCharAnim_CheckedChanged;
            chkGenerateLipSync.CheckedChanged += ChkGenerateLipSync_CheckedChanged;
            chkGenerateFacialExpression.CheckedChanged += ChkGenerateFacialExpression_CheckedChanged;
            chkGenerateCameraMotion.CheckedChanged += ChkGenerateCameraMotion_CheckedChanged;
            radOptMotionSourceMltd.CheckedChanged += RadOptMotionSourceMltd_CheckedChanged;
            radOptCamFormatVmd.CheckedChanged += RadOptCamFormatVmd_CheckedChanged;
            btnGo.Click += BtnGo_Click;
            btnInputHead.Click += BtnInputHead_Click;
            btnInputBody.Click += BtnInputBody_Click;
            btnInputDance.Click += BtnInputDance_Click;
            btnInputScenario.Click += BtnInputScenario_Click;
            btnInputCamera.Click += BtnInputCamera_Click;
            btnOutputModel.Click += BtnOutputModel_Click;
            btnOutputCharAnim.Click += BtnOutputCharAnim_Click;
            btnOutputLipSync.Click += BtnOutputLipSync_Click;
            btnOutputFacialExpression.Click += BtnOutputFacialExpression_Click;
            btnOutputCameraMotion.Click += BtnOutputCameraMotion_Click;
            chkOptApplyCharHeight.CheckedChanged += ChkOptApplyCharHeight_CheckedChanged;
            chkOptScalePmx.CheckedChanged += ChkOptScalePmx_CheckedChanged;
            btnClearLog.Click += BtnClearLog_Click;
            btnOptSelectFEMappings.Click += BtnOptSelectFEMappings_Click;
            chkGameToon.CheckedChanged += ChkGameToon_CheckedChanged;
            lnkHelp.LinkClicked += LnkHelp_LinkClicked;
            cboOptAppealType.SelectedIndexChanged += CboOptAppealType_SelectedIndexChanged;
            btnOptSelectExternalDanceAppealFile.Click += BtnOptSelectExternalAppealFile_Click;
        }

        private void BtnOptSelectExternalAppealFile_Click(object sender, EventArgs e) {
            var (path, ok) = SelectOpenFile("External Appeal Data (*_ap.imo.unity3d)|*_ap.imo.unity3d");

            if (!ok) {
                return;
            }

            txtOptExternalDanceAppealFile.Text = path;
        }

        private void CboOptAppealType_SelectedIndexChanged(object sender, EventArgs e) {
            var externalFileEnabled = cboOptAppealType.SelectedIndex > 0;
            label27.Enabled = externalFileEnabled;
            txtOptExternalDanceAppealFile.Enabled = externalFileEnabled;
            btnOptSelectExternalDanceAppealFile.Enabled = externalFileEnabled;
        }

        private void LnkHelp_LinkClicked(object sender, EventArgs e) {
            const string helpUrl = "https://github.com/OpenMLTD/MLTDTools/wiki/MillionDance-Manual";

            try {
                UrlOpener.OpenUrl(helpUrl);
            } catch (Exception ex) {
                var sb = new StringBuilder();
                sb.AppendLine("Cannot open URL <" + helpUrl + ">; please manually open it in your browser.");
                sb.AppendLine();
                sb.Append(ex.ToString());

                var message = sb.ToString();

                MessageBox.Show(message, ApplicationHelper.GetApplicationTitle(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ChkGameToon_CheckedChanged(object sender, EventArgs e) {
            cboGameToonSkinNumber.Enabled = chkGameToon.Checked;
            cboGameToonClothesNumber.Enabled = chkGameToon.Checked;
        }

        private void BtnOptSelectFEMappings_Click(object sender, EventArgs e) {
            var (path, ok) = SelectOpenFile("JSON (*.json)|*.json");

            if (!ok) {
                return;
            }

            txtOptFEMappings.Text = path;
        }

        private void BtnClearLog_Click(object sender, EventArgs e) {
            txtLog.Clear();
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

        private void BtnOutputFacialExpression_Click(object sender, EventArgs e) {
            var (r, ok) = SelectSaveFile("MikuMikuDance Model Motion (*.vmd)|*.vmd");
            if (ok) {
                txtOutputFacialExpression.Text = r;
            }
        }

        private void BtnOutputLipSync_Click(object sender, EventArgs e) {
            var (r, ok) = SelectSaveFile("MikuMikuDance Model Motion (*.vmd)|*.vmd");
            if (ok) {
                txtOutputLipSync.Text = r;
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

        private void BtnInputScenario_Click(object sender, EventArgs e) {
            var (r, ok) = SelectOpenFile("Scenario Asset Bundle (scrobj_*.unity3d)|scrobj_*.unity3d");
            if (ok) {
                txtInputScenario.Text = r;
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
            cboOptMotionNumber.SelectedIndex = 0;
            cboOptFormationNumber.SelectedIndex = 0;
            cboGameToonSkinNumber.SelectedIndex = 5 - 1; // toon05, looks closest to MLTD models
            cboGameToonClothesNumber.SelectedIndex = 4 - 1; // toon 04, less yellow-ish
            cboOptAppealType.SelectedIndex = 0;

            var defaultCharHeightStr = ScalingConfig.StandardCharacterHeightInCentimeters.ToString();

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

                if (chkGenerateModel.Checked || chkGenerateCharAnim.Checked) {
                    if (!File.Exists(txtInputHead.Text)) {
                        Alert($"Head model \"{txtInputHead.Text}\" does not exist.");
                        return false;
                    }

                    if (!File.Exists(txtInputBody.Text)) {
                        Alert($"Body model \"{txtInputBody.Text}\" does not exist.");
                        return false;
                    }
                }

                if (chkGenerateModel.Checked) {
                    if (!CheckOutputDir(txtOutputModel.Text)) {
                        return false;
                    }
                }

                if (chkOptApplyCharHeight.Checked) {
                    if (!uint.TryParse(txtOptCharHeight.Text, out var height) || (height <= 100 || 200 <= height)) {
                        Alert("Please enter a valid idol height.");
                    }
                }

                if (chkGenerateCharAnim.Checked) {
                    if (!File.Exists(txtInputDance.Text)) {
                        Alert($"Dance data \"{txtInputDance.Text}\" does not exist.");
                        return false;
                    }

                    if (!CheckOutputDir(txtOutputCharAnim.Text)) {
                        return false;
                    }
                }

                if (chkGenerateCharAnim.Checked || chkGenerateLipSync.Checked || chkGenerateFacialExpression.Checked) {
                    if (!File.Exists(txtInputScenario.Text)) {
                        Alert($"Scenario data \"{txtInputScenario.Text}\" does not exist.");
                        return false;
                    }
                }

                if (chkGenerateLipSync.Checked) {
                    if (!CheckOutputDir(txtOutputLipSync.Text)) {
                        return false;
                    }
                }

                if (chkGenerateFacialExpression.Checked) {
                    if (!File.Exists(txtOptFEMappings.Text)) {
                        Alert($"Facial expression mapping file \"{txtOptFEMappings.Text}\" does not exist.");
                        return false;
                    }

                    if (!CheckOutputDir(txtOutputFacialExpression.Text)) {
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
                    if (!uint.TryParse(txtOptFixedFov.Text, out var u) || u == 0) {
                        Alert($"FOV value \"{txtOptFixedFov.Text}\" should be a valid positive integer.");
                        return false;
                    }
                }

                if (chkGenerateModel.Checked || chkGenerateCharAnim.Checked) {
                    if (!Regex.IsMatch(txtInputHead.Text, @"ch_[a-z]{2}\d{3}_(?:\d{3}[a-z]{3}|[a-z])(?:_v2)?\.unity3d$", RegexOptions.CultureInvariant)) {
                        Alert($"File \"{txtInputHead.Text}\" does not look like a character head file from the game.");
                        return false;
                    }

                    if (!Regex.IsMatch(txtInputBody.Text, @"cb_[a-z]{2}\d{3}_(?:\d{3}[a-z]{3}|[a-z])\.unity3d$", RegexOptions.CultureInvariant)) {
                        Alert($"File \"{txtInputBody.Text}\" does not look like a character body file from the game.");
                        return false;
                    }
                }

                if (chkGenerateCharAnim.Checked) {
                    if (!Regex.IsMatch(txtInputDance.Text, @"dan_[a-z0-9]{5}[a-z0-9+]_0[12345](?:\.imo)?\.unity3d$", RegexOptions.CultureInvariant)) {
                        Alert($"File \"{txtInputDance.Text}\" does not look like a dance data file from the game.");
                        return false;
                    }
                }

                if (chkGenerateCharAnim.Checked || chkGenerateLipSync.Checked || chkGenerateFacialExpression.Checked) {
                    if (!Regex.IsMatch(txtInputScenario.Text, @"scrobj_[a-z0-9]{5}[a-z0-9+]\.unity3d$", RegexOptions.CultureInvariant)) {
                        Alert($"File \"{txtInputScenario.Text}\" does not look like a scenario data file from the game.");
                        return false;
                    }
                }

                if (chkGenerateCameraMotion.Checked) {
                    if (!Regex.IsMatch(txtInputCamera.Text, @"cam_[a-z0-9]{5}[a-z0-9+](?:\.imo)?\.unity3d$", RegexOptions.CultureInvariant)) {
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

                if (cboOptAppealType.SelectedIndex > 0) {
                    if (!string.IsNullOrWhiteSpace(txtOptExternalDanceAppealFile.Text)) {
                        if (!Regex.IsMatch(txtOptExternalDanceAppealFile.Text, @"dan_[a-z0-9]{5}[a-z0-9+]_ap\.imo\.unity3d$", RegexOptions.CultureInvariant)) {
                            Alert($"File \"{txtOptExternalDanceAppealFile.Text}\" does not look like an external appeal data file from the game.");
                            return false;
                        }
                    }
                }

                return true;
            }

            MainWorkerInputParams PrepareInputParams() {
                var ip = new MainWorkerInputParams();

                ip.GenerateModel = chkGenerateModel.Checked;
                ip.GenerateCharacterMotion = chkGenerateCharAnim.Checked;
                ip.GenerateLipSync = chkGenerateLipSync.Checked;
                ip.GenerateFacialExpressions = chkGenerateFacialExpression.Checked;
                ip.GenerateCameraMotion = chkGenerateCameraMotion.Checked;

                ip.InputHead = txtInputHead.Text;
                ip.InputBody = txtInputBody.Text;
                ip.InputDance = txtInputDance.Text;
                ip.InputScenario = txtInputScenario.Text;
                ip.InputCamera = txtInputCamera.Text;

                ip.OutputModel = txtOutputModel.Text;
                ip.OutputCharacterAnimation = txtOutputCharAnim.Text;
                ip.OutputLipSync = txtOutputLipSync.Text;
                ip.OutputFacialExpressions = txtOutputFacialExpression.Text;
                ip.OutputCamera = txtOutputCameraMotion.Text;

                ip.MotionSource = radOptMotionSourceMltd.Checked ? MotionFormat.Mltd : MotionFormat.Mmd;
                ip.ScalePmx = chkOptScalePmx.Checked;
                ip.ConsiderIdolHeight = chkOptApplyCharHeight.Checked;
                var idolHeightInCm = Convert.ToUInt32(txtOptCharHeight.Text) / 100.0f;
                ip.IdolHeight = ip.ConsiderIdolHeight ? idolHeightInCm : 0;
                ip.TranslateBoneNames = chkOptTranslateBoneNames.Checked;
                ip.AppendLegIkBones = chkOptAppendLegIKBones.Checked;
                ip.FixCenterBones = chkOptFixCenterBones.Checked;
                ip.ConvertBindingPose = chkOptConvertToTdaPose.Checked;
                ip.AppendEyeBones = chkOptAppendEyeBones.Checked;
                ip.HideUnityGeneratedBones = chkOptHideUnityGenBones.Checked;
                ip.TranslateFacialExpressionNames = chkOptTranslateFacialExpressionNames.Checked;
                ip.ImportPhysics = chkOptImportPhysics.Checked;
                ip.ApplyGameStyledToon = chkGameToon.Checked;
                ip.SkinToonNumber = cboGameToonSkinNumber.SelectedIndex + 1;
                ip.ClothesToonNumber = cboGameToonClothesNumber.SelectedIndex + 1;
                ip.AddHairHighlights = chkOptAddHighlightHair.Checked;
                ip.AddEyesHighlights = chkOptAddHighlightEyes.Checked;

                ip.TransformTo30Fps = radOptAnimFrameRate30.Checked;
                ip.ScaleVmd = chkOptScaleVmd.Checked;
                ip.UseMvdForCamera = radOptCamFormatMvd.Checked;
                ip.FixedFov = ip.UseMvdForCamera ? 0 : Convert.ToUInt32(txtOptFixedFov.Text);
                ip.MotionNumber = cboOptMotionNumber.SelectedIndex + 1;
                ip.FormationNumber = cboOptFormationNumber.SelectedIndex + 1;
                ip.AppealType = (MainWorkerInputParams.FullComoboAppealType)cboOptAppealType.SelectedIndex;
                ip.ExternalDanceAppealFile = txtOptExternalDanceAppealFile.Text;

                ip.FacialExpressionMappingFilePath = txtOptFEMappings.Text;
                ip.PreferredFacialExpressionSource = radFESourceLandscape.Checked ? MainWorkerInputParams.FallbackFacialExpressionSource.Landscape : MainWorkerInputParams.FallbackFacialExpressionSource.Portrait;

                return ip;
            }

            txtLog.Clear();

            MainWorkerInputParams p;

            try {
                if (!CheckInputParams()) {
                    return;
                }

                p = PrepareInputParams();
            } catch (Exception ex) {
                Alert(ex.Message);
                return;
            }

            var worker = new MainWorker(this, p);

            worker.Start();

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
            SetInputOutputControlsAvailability();
            ValidateHasAtLeastOneTask();
        }

        private void ChkGenerateLipSync_CheckedChanged(object sender, EventArgs e) {
            SetInputOutputControlsAvailability();
            ValidateHasAtLeastOneTask();
        }

        private void ChkGenerateFacialExpression_CheckedChanged(object sender, EventArgs e) {
            SetInputOutputControlsAvailability();
            ValidateHasAtLeastOneTask();
        }

        private void ChkGenerateCharAnim_CheckedChanged(object sender, EventArgs e) {
            SetInputOutputControlsAvailability();

            if (!radOptMotionSourceMltd.Checked) {
                radOptMotionSourceMltd.Checked = true;
            }

            radOptMotionSourceMmd.Enabled = !chkGenerateCharAnim.Checked;

            ValidateHasAtLeastOneTask();
        }

        private void ChkGenerateModel_CheckedChanged(object sender, EventArgs e) {
            SetInputOutputControlsAvailability();
            ValidateHasAtLeastOneTask();
        }

        #endregion

        private void SetInputOutputControlsAvailability() {
            var model = chkGenerateModel.Checked;
            var charAnim = chkGenerateCharAnim.Checked;
            var lipSync = chkGenerateLipSync.Checked;
            var facialExpr = chkGenerateFacialExpression.Checked;
            var camAnim = chkGenerateCameraMotion.Checked;

            var b = model || charAnim;
            txtInputHead.Enabled = b;
            btnInputHead.Enabled = b;

            b = model || charAnim;
            txtInputBody.Enabled = b;
            btnInputBody.Enabled = b;

            b = charAnim;
            txtInputDance.Enabled = b;
            btnInputDance.Enabled = b;

            b = charAnim || lipSync || facialExpr;
            txtInputScenario.Enabled = b;
            btnInputScenario.Enabled = b;

            b = camAnim;
            txtInputCamera.Enabled = b;
            btnInputCamera.Enabled = b;

            b = model;
            txtOutputModel.Enabled = b;
            btnOutputModel.Enabled = b;

            b = charAnim;
            txtOutputCharAnim.Enabled = b;
            btnOutputCharAnim.Enabled = b;

            b = lipSync;
            txtOutputLipSync.Enabled = b;
            btnOutputLipSync.Enabled = b;

            b = facialExpr;
            txtOutputFacialExpression.Enabled = b;
            btnOutputFacialExpression.Enabled = b;

            b = camAnim;
            txtOutputCameraMotion.Enabled = b;
            btnOutputCameraMotion.Enabled = b;
        }

        internal void Log([NotNull] string text) {
            if (InvokeRequired) {
                Invoke(new Action(() => {
                    Log(text);
                }));

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
            var b = chkGenerateModel.Checked || chkGenerateCharAnim.Checked || chkGenerateLipSync.Checked || chkGenerateFacialExpression.Checked || chkGenerateCameraMotion.Checked;

            tabControl1.Enabled = b;
            btnGo.Enabled = b;
        }

        internal void EnableMainControls(bool enabled) {
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
