﻿using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using JetBrains.Annotations;
using MillionDance.Core;

namespace MillionDance {
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
            var (r, ok) = SelectOpenFile("Camera Asset Bundle (cam_*.imo.unity3d)|cam_*.imo.unity3d");
            if (ok) {
                txtInputCamera.Text = r;
            }
        }

        private void BtnInputFacialExpression_Click(object sender, EventArgs e) {
            var (r, ok) = SelectOpenFile("ScriptableObject Asset Bundle (scrobj_*.unity3d)|scrobj_*.unity3d");
            if (ok) {
                txtInputFacialExpression.Text = r;
            }
        }

        private void BtnInputDance_Click(object sender, EventArgs e) {
            var (r, ok) = SelectOpenFile("Dance Asset Bundle (dan_*.imo.unity3d)|dan_*.imo.unity3d");
            if (ok) {
                txtInputDance.Text = r;
            }
        }

        private void BtnInputBody_Click(object sender, EventArgs e) {
            var (r, ok) = SelectOpenFile("Character Asset Bundle (cb_*.unity3d)|cb_*.unity3d");
            if (ok) {
                txtInputBody.Text = r;
            }
        }

        private void BtnInputHead_Click(object sender, EventArgs e) {
            var (r, ok) = SelectOpenFile("Character Asset Bundle (ch_*.unity3d)|ch_*.unity3d");
            if (ok) {
                txtInputHead.Text = r;
            }
        }

        private void FMain_Load(object sender, EventArgs e) {
            cboOptSongPosition.SelectedIndex = 0;
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
                    if (!uint.TryParse(txtOptFixedFov.Text, out _)) {
                        Alert($"FOV value \"{txtOptFixedFov.Text}\" is not a valid positive integer.");
                        return false;
                    }
                }

                if (!Regex.IsMatch(txtInputHead.Text, @"ch_[a-z]{2}\d{3}_\d{3}[a-z]{3}\.unity3d$", RegexOptions.CultureInvariant)) {
                    Alert($"File \"{txtInputHead.Text}\" does not look like a character head file from the game.");
                    return false;
                }

                if (!Regex.IsMatch(txtInputBody.Text, @"cb_[a-z]{2}\d{3}_\d{3}[a-z]{3}\.unity3d$", RegexOptions.CultureInvariant)) {
                    Alert($"File \"{txtInputBody.Text}\" does not look like a character body file from the game.");
                    return false;
                }

                if (chkGenerateCharAnim.Checked) {
                    if (!Regex.IsMatch(txtInputDance.Text, @"dan_[a-z]{3}\d{3}_0[12345]\.imo\.unity3d$", RegexOptions.CultureInvariant)) {
                        Alert($"File \"{txtInputDance.Text}\" does not look like a dance data file from the game.");
                        return false;
                    }

                    if (!Regex.IsMatch(txtInputFacialExpression.Text, @"scrobj_[a-z]{3}\d{3}\.unity3d$", RegexOptions.CultureInvariant)) {
                        Alert($"File \"{txtInputFacialExpression.Text}\" does not look like a mixed data file from the game containing facial expressions.");
                        return false;
                    }
                }

                if (!Regex.IsMatch(txtInputCamera.Text, @"cam_[a-z]{3}\d{3}\.imo\.unity3d$", RegexOptions.CultureInvariant)) {
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
        }

        private void RadOptMotionSourceMltd_CheckedChanged(object sender, EventArgs e) {
            var b = radOptMotionSourceMltd.Checked;

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
            var now = DateTime.Now;
            var timeStr = now.ToString("s");

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
            sfd.CheckFileExists = true;
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