namespace OpenMLTD.MillionDance {
    partial class FMain {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkGenerateCameraMotion = new System.Windows.Forms.CheckBox();
            this.chkGenerateCharAnim = new System.Windows.Forms.CheckBox();
            this.chkGenerateModel = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnInputCamera = new System.Windows.Forms.Button();
            this.txtInputCamera = new System.Windows.Forms.TextBox();
            this.btnInputDance = new System.Windows.Forms.Button();
            this.txtInputDance = new System.Windows.Forms.TextBox();
            this.btnInputBody = new System.Windows.Forms.Button();
            this.txtInputBody = new System.Windows.Forms.TextBox();
            this.btnInputHead = new System.Windows.Forms.Button();
            this.txtInputHead = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.chkOptImportPhysics = new System.Windows.Forms.CheckBox();
            this.chkOptTranslateFacialExpressionNames = new System.Windows.Forms.CheckBox();
            this.chkOptHideUnityGenBones = new System.Windows.Forms.CheckBox();
            this.chkOptAppendEyeBones = new System.Windows.Forms.CheckBox();
            this.chkOptConvertToTdaPose = new System.Windows.Forms.CheckBox();
            this.chkOptFixCenterBones = new System.Windows.Forms.CheckBox();
            this.chkOptAppendLegIKBones = new System.Windows.Forms.CheckBox();
            this.chkOptTranslateBoneNames = new System.Windows.Forms.CheckBox();
            this.chkOptApplyCharHeight = new System.Windows.Forms.CheckBox();
            this.chkOptScalePmx = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radOptMotionSourceMmd = new System.Windows.Forms.RadioButton();
            this.radOptMotionSourceMltd = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.chkOptScaleVmd = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.radOptAnimFrameRate30 = new System.Windows.Forms.RadioButton();
            this.radOptAnimFrameRate60 = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnOutputCameraMotion = new System.Windows.Forms.Button();
            this.txtOutputCameraMotion = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnOutputCharAnim = new System.Windows.Forms.Button();
            this.txtOutputCharAnim = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnOutputModel = new System.Windows.Forms.Button();
            this.txtOutputModel = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.radOptCamFormatVmd = new System.Windows.Forms.RadioButton();
            this.radOptCamFormatMvd = new System.Windows.Forms.RadioButton();
            this.label11 = new System.Windows.Forms.Label();
            this.txtOptFixedFov = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btnInputFacialExpression = new System.Windows.Forms.Button();
            this.txtInputFacialExpression = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.cboOptSongPosition = new System.Windows.Forms.ComboBox();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkGenerateCameraMotion);
            this.groupBox1.Controls.Add(this.chkGenerateCharAnim);
            this.groupBox1.Controls.Add(this.chkGenerateModel);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(270, 92);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tasks";
            // 
            // chkGenerateCameraMotion
            // 
            this.chkGenerateCameraMotion.AutoSize = true;
            this.chkGenerateCameraMotion.Checked = true;
            this.chkGenerateCameraMotion.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGenerateCameraMotion.Location = new System.Drawing.Point(19, 64);
            this.chkGenerateCameraMotion.Name = "chkGenerateCameraMotion";
            this.chkGenerateCameraMotion.Size = new System.Drawing.Size(216, 16);
            this.chkGenerateCameraMotion.TabIndex = 2;
            this.chkGenerateCameraMotion.Text = "Generate camera motion (VMD/MVD)";
            this.chkGenerateCameraMotion.UseVisualStyleBackColor = true;
            // 
            // chkGenerateCharAnim
            // 
            this.chkGenerateCharAnim.AutoSize = true;
            this.chkGenerateCharAnim.Checked = true;
            this.chkGenerateCharAnim.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGenerateCharAnim.Location = new System.Drawing.Point(19, 42);
            this.chkGenerateCharAnim.Name = "chkGenerateCharAnim";
            this.chkGenerateCharAnim.Size = new System.Drawing.Size(210, 16);
            this.chkGenerateCharAnim.TabIndex = 2;
            this.chkGenerateCharAnim.Text = "Generate character motion (VMD)";
            this.chkGenerateCharAnim.UseVisualStyleBackColor = true;
            // 
            // chkGenerateModel
            // 
            this.chkGenerateModel.AutoSize = true;
            this.chkGenerateModel.Checked = true;
            this.chkGenerateModel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGenerateModel.Location = new System.Drawing.Point(19, 20);
            this.chkGenerateModel.Name = "chkGenerateModel";
            this.chkGenerateModel.Size = new System.Drawing.Size(144, 16);
            this.chkGenerateModel.TabIndex = 0;
            this.chkGenerateModel.Text = "Generate model (PMX)";
            this.chkGenerateModel.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnInputFacialExpression);
            this.groupBox2.Controls.Add(this.txtInputFacialExpression);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.btnInputCamera);
            this.groupBox2.Controls.Add(this.txtInputCamera);
            this.groupBox2.Controls.Add(this.btnInputDance);
            this.groupBox2.Controls.Add(this.txtInputDance);
            this.groupBox2.Controls.Add(this.btnInputBody);
            this.groupBox2.Controls.Add(this.txtInputBody);
            this.groupBox2.Controls.Add(this.btnInputHead);
            this.groupBox2.Controls.Add(this.txtInputHead);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(12, 110);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(270, 167);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Input Files";
            // 
            // btnInputCamera
            // 
            this.btnInputCamera.Location = new System.Drawing.Point(222, 130);
            this.btnInputCamera.Name = "btnInputCamera";
            this.btnInputCamera.Size = new System.Drawing.Size(32, 21);
            this.btnInputCamera.TabIndex = 11;
            this.btnInputCamera.Text = "...";
            this.btnInputCamera.UseVisualStyleBackColor = true;
            // 
            // txtInputCamera
            // 
            this.txtInputCamera.Location = new System.Drawing.Point(70, 131);
            this.txtInputCamera.Name = "txtInputCamera";
            this.txtInputCamera.Size = new System.Drawing.Size(146, 21);
            this.txtInputCamera.TabIndex = 10;
            // 
            // btnInputDance
            // 
            this.btnInputDance.Location = new System.Drawing.Point(222, 76);
            this.btnInputDance.Name = "btnInputDance";
            this.btnInputDance.Size = new System.Drawing.Size(32, 21);
            this.btnInputDance.TabIndex = 9;
            this.btnInputDance.Text = "...";
            this.btnInputDance.UseVisualStyleBackColor = true;
            // 
            // txtInputDance
            // 
            this.txtInputDance.Location = new System.Drawing.Point(70, 77);
            this.txtInputDance.Name = "txtInputDance";
            this.txtInputDance.Size = new System.Drawing.Size(146, 21);
            this.txtInputDance.TabIndex = 8;
            // 
            // btnInputBody
            // 
            this.btnInputBody.Location = new System.Drawing.Point(222, 49);
            this.btnInputBody.Name = "btnInputBody";
            this.btnInputBody.Size = new System.Drawing.Size(32, 21);
            this.btnInputBody.TabIndex = 7;
            this.btnInputBody.Text = "...";
            this.btnInputBody.UseVisualStyleBackColor = true;
            // 
            // txtInputBody
            // 
            this.txtInputBody.Location = new System.Drawing.Point(70, 50);
            this.txtInputBody.Name = "txtInputBody";
            this.txtInputBody.Size = new System.Drawing.Size(146, 21);
            this.txtInputBody.TabIndex = 6;
            // 
            // btnInputHead
            // 
            this.btnInputHead.Location = new System.Drawing.Point(222, 22);
            this.btnInputHead.Name = "btnInputHead";
            this.btnInputHead.Size = new System.Drawing.Size(32, 21);
            this.btnInputHead.TabIndex = 5;
            this.btnInputHead.Text = "...";
            this.btnInputHead.UseVisualStyleBackColor = true;
            // 
            // txtInputHead
            // 
            this.txtInputHead.Location = new System.Drawing.Point(70, 23);
            this.txtInputHead.Name = "txtInputHead";
            this.txtInputHead.Size = new System.Drawing.Size(146, 21);
            this.txtInputHead.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 134);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "Camera:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "Dance:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Body:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Head:";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(288, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(417, 276);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.chkOptImportPhysics);
            this.tabPage1.Controls.Add(this.chkOptTranslateFacialExpressionNames);
            this.tabPage1.Controls.Add(this.chkOptHideUnityGenBones);
            this.tabPage1.Controls.Add(this.chkOptAppendEyeBones);
            this.tabPage1.Controls.Add(this.chkOptConvertToTdaPose);
            this.tabPage1.Controls.Add(this.chkOptFixCenterBones);
            this.tabPage1.Controls.Add(this.chkOptAppendLegIKBones);
            this.tabPage1.Controls.Add(this.chkOptTranslateBoneNames);
            this.tabPage1.Controls.Add(this.chkOptApplyCharHeight);
            this.tabPage1.Controls.Add(this.chkOptScalePmx);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(409, 250);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Model";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // chkOptImportPhysics
            // 
            this.chkOptImportPhysics.AutoSize = true;
            this.chkOptImportPhysics.Checked = true;
            this.chkOptImportPhysics.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOptImportPhysics.Location = new System.Drawing.Point(8, 227);
            this.chkOptImportPhysics.Name = "chkOptImportPhysics";
            this.chkOptImportPhysics.Size = new System.Drawing.Size(108, 16);
            this.chkOptImportPhysics.TabIndex = 21;
            this.chkOptImportPhysics.Text = "Import physics";
            this.chkOptImportPhysics.UseVisualStyleBackColor = true;
            // 
            // chkOptTranslateFacialExpressionNames
            // 
            this.chkOptTranslateFacialExpressionNames.AutoSize = true;
            this.chkOptTranslateFacialExpressionNames.Checked = true;
            this.chkOptTranslateFacialExpressionNames.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOptTranslateFacialExpressionNames.Location = new System.Drawing.Point(8, 205);
            this.chkOptTranslateFacialExpressionNames.Name = "chkOptTranslateFacialExpressionNames";
            this.chkOptTranslateFacialExpressionNames.Size = new System.Drawing.Size(354, 16);
            this.chkOptTranslateFacialExpressionNames.TabIndex = 20;
            this.chkOptTranslateFacialExpressionNames.Text = "Translate facial expression names to MMD TDA convention";
            this.chkOptTranslateFacialExpressionNames.UseVisualStyleBackColor = true;
            // 
            // chkOptHideUnityGenBones
            // 
            this.chkOptHideUnityGenBones.AutoSize = true;
            this.chkOptHideUnityGenBones.Checked = true;
            this.chkOptHideUnityGenBones.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOptHideUnityGenBones.Location = new System.Drawing.Point(8, 183);
            this.chkOptHideUnityGenBones.Name = "chkOptHideUnityGenBones";
            this.chkOptHideUnityGenBones.Size = new System.Drawing.Size(276, 16);
            this.chkOptHideUnityGenBones.TabIndex = 19;
            this.chkOptHideUnityGenBones.Text = "Hide Unity-generated bones (in previewers)";
            this.chkOptHideUnityGenBones.UseVisualStyleBackColor = true;
            // 
            // chkOptAppendEyeBones
            // 
            this.chkOptAppendEyeBones.AutoSize = true;
            this.chkOptAppendEyeBones.Location = new System.Drawing.Point(8, 161);
            this.chkOptAppendEyeBones.Name = "chkOptAppendEyeBones";
            this.chkOptAppendEyeBones.Size = new System.Drawing.Size(120, 16);
            this.chkOptAppendEyeBones.TabIndex = 16;
            this.chkOptAppendEyeBones.Text = "Append eye bones";
            this.chkOptAppendEyeBones.UseVisualStyleBackColor = true;
            // 
            // chkOptConvertToTdaPose
            // 
            this.chkOptConvertToTdaPose.AutoSize = true;
            this.chkOptConvertToTdaPose.Location = new System.Drawing.Point(8, 139);
            this.chkOptConvertToTdaPose.Name = "chkOptConvertToTdaPose";
            this.chkOptConvertToTdaPose.Size = new System.Drawing.Size(282, 16);
            this.chkOptConvertToTdaPose.TabIndex = 18;
            this.chkOptConvertToTdaPose.Text = "Convert standard T-pose to TDA binding pose";
            this.chkOptConvertToTdaPose.UseVisualStyleBackColor = true;
            // 
            // chkOptFixCenterBones
            // 
            this.chkOptFixCenterBones.AutoSize = true;
            this.chkOptFixCenterBones.Location = new System.Drawing.Point(8, 117);
            this.chkOptFixCenterBones.Name = "chkOptFixCenterBones";
            this.chkOptFixCenterBones.Size = new System.Drawing.Size(120, 16);
            this.chkOptFixCenterBones.TabIndex = 17;
            this.chkOptFixCenterBones.Text = "Fix center bones";
            this.chkOptFixCenterBones.UseVisualStyleBackColor = true;
            // 
            // chkOptAppendLegIKBones
            // 
            this.chkOptAppendLegIKBones.AutoSize = true;
            this.chkOptAppendLegIKBones.Location = new System.Drawing.Point(8, 95);
            this.chkOptAppendLegIKBones.Name = "chkOptAppendLegIKBones";
            this.chkOptAppendLegIKBones.Size = new System.Drawing.Size(138, 16);
            this.chkOptAppendLegIKBones.TabIndex = 15;
            this.chkOptAppendLegIKBones.Text = "Append leg IK bones";
            this.chkOptAppendLegIKBones.UseVisualStyleBackColor = true;
            // 
            // chkOptTranslateBoneNames
            // 
            this.chkOptTranslateBoneNames.AutoSize = true;
            this.chkOptTranslateBoneNames.Checked = true;
            this.chkOptTranslateBoneNames.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOptTranslateBoneNames.Location = new System.Drawing.Point(8, 73);
            this.chkOptTranslateBoneNames.Name = "chkOptTranslateBoneNames";
            this.chkOptTranslateBoneNames.Size = new System.Drawing.Size(276, 16);
            this.chkOptTranslateBoneNames.TabIndex = 13;
            this.chkOptTranslateBoneNames.Text = "Translate bone names to MMD TDA convention";
            this.chkOptTranslateBoneNames.UseVisualStyleBackColor = true;
            // 
            // chkOptApplyCharHeight
            // 
            this.chkOptApplyCharHeight.AutoSize = true;
            this.chkOptApplyCharHeight.Checked = true;
            this.chkOptApplyCharHeight.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOptApplyCharHeight.Location = new System.Drawing.Point(24, 51);
            this.chkOptApplyCharHeight.Name = "chkOptApplyCharHeight";
            this.chkOptApplyCharHeight.Size = new System.Drawing.Size(144, 16);
            this.chkOptApplyCharHeight.TabIndex = 14;
            this.chkOptApplyCharHeight.Text = "Consider idol height";
            this.chkOptApplyCharHeight.UseVisualStyleBackColor = true;
            // 
            // chkOptScalePmx
            // 
            this.chkOptScalePmx.AutoSize = true;
            this.chkOptScalePmx.Checked = true;
            this.chkOptScalePmx.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOptScalePmx.Location = new System.Drawing.Point(8, 29);
            this.chkOptScalePmx.Name = "chkOptScalePmx";
            this.chkOptScalePmx.Size = new System.Drawing.Size(204, 16);
            this.chkOptScalePmx.TabIndex = 12;
            this.chkOptScalePmx.Text = "Scale model to normal PMX size";
            this.chkOptScalePmx.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radOptMotionSourceMmd);
            this.panel1.Controls.Add(this.radOptMotionSourceMltd);
            this.panel1.Location = new System.Drawing.Point(101, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(151, 21);
            this.panel1.TabIndex = 11;
            // 
            // radOptMotionSourceMmd
            // 
            this.radOptMotionSourceMmd.AutoSize = true;
            this.radOptMotionSourceMmd.Location = new System.Drawing.Point(86, 3);
            this.radOptMotionSourceMmd.Name = "radOptMotionSourceMmd";
            this.radOptMotionSourceMmd.Size = new System.Drawing.Size(41, 16);
            this.radOptMotionSourceMmd.TabIndex = 1;
            this.radOptMotionSourceMmd.Text = "MMD";
            this.radOptMotionSourceMmd.UseVisualStyleBackColor = true;
            // 
            // radOptMotionSourceMltd
            // 
            this.radOptMotionSourceMltd.AutoSize = true;
            this.radOptMotionSourceMltd.Checked = true;
            this.radOptMotionSourceMltd.Location = new System.Drawing.Point(3, 3);
            this.radOptMotionSourceMltd.Name = "radOptMotionSourceMltd";
            this.radOptMotionSourceMltd.Size = new System.Drawing.Size(47, 16);
            this.radOptMotionSourceMltd.TabIndex = 0;
            this.radOptMotionSourceMltd.TabStop = true;
            this.radOptMotionSourceMltd.Text = "MLTD";
            this.radOptMotionSourceMltd.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "Motion source:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.cboOptSongPosition);
            this.tabPage2.Controls.Add(this.label14);
            this.tabPage2.Controls.Add(this.label12);
            this.tabPage2.Controls.Add(this.txtOptFixedFov);
            this.tabPage2.Controls.Add(this.label11);
            this.tabPage2.Controls.Add(this.panel3);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.chkOptScaleVmd);
            this.tabPage2.Controls.Add(this.panel2);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(409, 250);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Motions";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // chkOptScaleVmd
            // 
            this.chkOptScaleVmd.AutoSize = true;
            this.chkOptScaleVmd.Checked = true;
            this.chkOptScaleVmd.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOptScaleVmd.Location = new System.Drawing.Point(6, 35);
            this.chkOptScaleVmd.Name = "chkOptScaleVmd";
            this.chkOptScaleVmd.Size = new System.Drawing.Size(240, 16);
            this.chkOptScaleVmd.TabIndex = 2;
            this.chkOptScaleVmd.Text = "Scale motions to normal VMD/MVD size";
            this.chkOptScaleVmd.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.radOptAnimFrameRate30);
            this.panel2.Controls.Add(this.radOptAnimFrameRate60);
            this.panel2.Location = new System.Drawing.Point(144, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(170, 24);
            this.panel2.TabIndex = 1;
            // 
            // radOptAnimFrameRate30
            // 
            this.radOptAnimFrameRate30.AutoSize = true;
            this.radOptAnimFrameRate30.Location = new System.Drawing.Point(108, 3);
            this.radOptAnimFrameRate30.Name = "radOptAnimFrameRate30";
            this.radOptAnimFrameRate30.Size = new System.Drawing.Size(59, 16);
            this.radOptAnimFrameRate30.TabIndex = 3;
            this.radOptAnimFrameRate30.Text = "30 fps";
            this.radOptAnimFrameRate30.UseVisualStyleBackColor = true;
            // 
            // radOptAnimFrameRate60
            // 
            this.radOptAnimFrameRate60.AutoSize = true;
            this.radOptAnimFrameRate60.Checked = true;
            this.radOptAnimFrameRate60.Location = new System.Drawing.Point(3, 4);
            this.radOptAnimFrameRate60.Name = "radOptAnimFrameRate60";
            this.radOptAnimFrameRate60.Size = new System.Drawing.Size(59, 16);
            this.radOptAnimFrameRate60.TabIndex = 2;
            this.radOptAnimFrameRate60.TabStop = true;
            this.radOptAnimFrameRate60.Text = "60 fps";
            this.radOptAnimFrameRate60.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(131, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "Animation frame rate:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnOutputCameraMotion);
            this.groupBox3.Controls.Add(this.txtOutputCameraMotion);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.btnOutputCharAnim);
            this.groupBox3.Controls.Add(this.txtOutputCharAnim);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.btnOutputModel);
            this.groupBox3.Controls.Add(this.txtOutputModel);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Location = new System.Drawing.Point(12, 283);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(270, 138);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Output Files";
            // 
            // btnOutputCameraMotion
            // 
            this.btnOutputCameraMotion.Location = new System.Drawing.Point(222, 98);
            this.btnOutputCameraMotion.Name = "btnOutputCameraMotion";
            this.btnOutputCameraMotion.Size = new System.Drawing.Size(32, 21);
            this.btnOutputCameraMotion.TabIndex = 14;
            this.btnOutputCameraMotion.Text = "...";
            this.btnOutputCameraMotion.UseVisualStyleBackColor = true;
            // 
            // txtOutputCameraMotion
            // 
            this.txtOutputCameraMotion.Location = new System.Drawing.Point(19, 99);
            this.txtOutputCameraMotion.Name = "txtOutputCameraMotion";
            this.txtOutputCameraMotion.Size = new System.Drawing.Size(197, 21);
            this.txtOutputCameraMotion.TabIndex = 13;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(17, 84);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(89, 12);
            this.label9.TabIndex = 12;
            this.label9.Text = "Camera motion:";
            // 
            // btnOutputCharAnim
            // 
            this.btnOutputCharAnim.Location = new System.Drawing.Point(222, 59);
            this.btnOutputCharAnim.Name = "btnOutputCharAnim";
            this.btnOutputCharAnim.Size = new System.Drawing.Size(32, 21);
            this.btnOutputCharAnim.TabIndex = 11;
            this.btnOutputCharAnim.Text = "...";
            this.btnOutputCharAnim.UseVisualStyleBackColor = true;
            // 
            // txtOutputCharAnim
            // 
            this.txtOutputCharAnim.Location = new System.Drawing.Point(19, 60);
            this.txtOutputCharAnim.Name = "txtOutputCharAnim";
            this.txtOutputCharAnim.Size = new System.Drawing.Size(197, 21);
            this.txtOutputCharAnim.TabIndex = 10;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(17, 45);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(107, 12);
            this.label8.TabIndex = 9;
            this.label8.Text = "Character motion:";
            // 
            // btnOutputModel
            // 
            this.btnOutputModel.Location = new System.Drawing.Point(222, 20);
            this.btnOutputModel.Name = "btnOutputModel";
            this.btnOutputModel.Size = new System.Drawing.Size(32, 21);
            this.btnOutputModel.TabIndex = 8;
            this.btnOutputModel.Text = "...";
            this.btnOutputModel.UseVisualStyleBackColor = true;
            // 
            // txtOutputModel
            // 
            this.txtOutputModel.Location = new System.Drawing.Point(70, 21);
            this.txtOutputModel.Name = "txtOutputModel";
            this.txtOutputModel.Size = new System.Drawing.Size(146, 21);
            this.txtOutputModel.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 6;
            this.label7.Text = "Model:";
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(288, 292);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(331, 129);
            this.txtLog.TabIndex = 4;
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(625, 373);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(80, 48);
            this.btnGo.TabIndex = 5;
            this.btnGo.Text = "&Go";
            this.btnGo.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 62);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(89, 12);
            this.label10.TabIndex = 3;
            this.label10.Text = "Camera format:";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.radOptCamFormatMvd);
            this.panel3.Controls.Add(this.radOptCamFormatVmd);
            this.panel3.Location = new System.Drawing.Point(102, 57);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(301, 24);
            this.panel3.TabIndex = 4;
            // 
            // radOptCamFormatVmd
            // 
            this.radOptCamFormatVmd.AutoSize = true;
            this.radOptCamFormatVmd.Location = new System.Drawing.Point(3, 3);
            this.radOptCamFormatVmd.Name = "radOptCamFormatVmd";
            this.radOptCamFormatVmd.Size = new System.Drawing.Size(113, 16);
            this.radOptCamFormatVmd.TabIndex = 5;
            this.radOptCamFormatVmd.Text = "VMD (fixed FOV)";
            this.radOptCamFormatVmd.UseVisualStyleBackColor = true;
            // 
            // radOptCamFormatMvd
            // 
            this.radOptCamFormatMvd.AutoSize = true;
            this.radOptCamFormatMvd.Checked = true;
            this.radOptCamFormatMvd.Location = new System.Drawing.Point(150, 3);
            this.radOptCamFormatMvd.Name = "radOptCamFormatMvd";
            this.radOptCamFormatMvd.Size = new System.Drawing.Size(125, 16);
            this.radOptCamFormatMvd.TabIndex = 6;
            this.radOptCamFormatMvd.TabStop = true;
            this.radOptCamFormatMvd.Text = "MVD (dynamic FOV)";
            this.radOptCamFormatMvd.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 90);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 12);
            this.label11.TabIndex = 5;
            this.label11.Text = "Fixed FOV:";
            // 
            // txtOptFixedFov
            // 
            this.txtOptFixedFov.Enabled = false;
            this.txtOptFixedFov.Location = new System.Drawing.Point(78, 87);
            this.txtOptFixedFov.Name = "txtOptFixedFov";
            this.txtOptFixedFov.Size = new System.Drawing.Size(60, 21);
            this.txtOptFixedFov.TabIndex = 6;
            this.txtOptFixedFov.Text = "20";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(141, 90);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(23, 12);
            this.label12.TabIndex = 7;
            this.label12.Text = "deg";
            // 
            // btnInputFacialExpression
            // 
            this.btnInputFacialExpression.Location = new System.Drawing.Point(222, 103);
            this.btnInputFacialExpression.Name = "btnInputFacialExpression";
            this.btnInputFacialExpression.Size = new System.Drawing.Size(32, 21);
            this.btnInputFacialExpression.TabIndex = 14;
            this.btnInputFacialExpression.Text = "...";
            this.btnInputFacialExpression.UseVisualStyleBackColor = true;
            // 
            // txtInputFacialExpression
            // 
            this.txtInputFacialExpression.Location = new System.Drawing.Point(70, 104);
            this.txtInputFacialExpression.Name = "txtInputFacialExpression";
            this.txtInputFacialExpression.Size = new System.Drawing.Size(146, 21);
            this.txtInputFacialExpression.TabIndex = 13;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(17, 107);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(47, 12);
            this.label13.TabIndex = 12;
            this.label13.Text = "Facial:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(7, 117);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(59, 12);
            this.label14.TabIndex = 8;
            this.label14.Text = "Position:";
            // 
            // cboOptSongPosition
            // 
            this.cboOptSongPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOptSongPosition.FormattingEnabled = true;
            this.cboOptSongPosition.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.cboOptSongPosition.Location = new System.Drawing.Point(78, 114);
            this.cboOptSongPosition.Name = "cboOptSongPosition";
            this.cboOptSongPosition.Size = new System.Drawing.Size(107, 20);
            this.cboOptSongPosition.TabIndex = 9;
            // 
            // FMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(715, 433);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MillionDance";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkGenerateModel;
        private System.Windows.Forms.CheckBox chkGenerateCameraMotion;
        private System.Windows.Forms.CheckBox chkGenerateCharAnim;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnInputCamera;
        private System.Windows.Forms.TextBox txtInputCamera;
        private System.Windows.Forms.Button btnInputDance;
        private System.Windows.Forms.TextBox txtInputDance;
        private System.Windows.Forms.Button btnInputBody;
        private System.Windows.Forms.TextBox txtInputBody;
        private System.Windows.Forms.Button btnInputHead;
        private System.Windows.Forms.TextBox txtInputHead;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.CheckBox chkOptImportPhysics;
        private System.Windows.Forms.CheckBox chkOptTranslateFacialExpressionNames;
        private System.Windows.Forms.CheckBox chkOptHideUnityGenBones;
        private System.Windows.Forms.CheckBox chkOptAppendEyeBones;
        private System.Windows.Forms.CheckBox chkOptConvertToTdaPose;
        private System.Windows.Forms.CheckBox chkOptFixCenterBones;
        private System.Windows.Forms.CheckBox chkOptAppendLegIKBones;
        private System.Windows.Forms.CheckBox chkOptTranslateBoneNames;
        private System.Windows.Forms.CheckBox chkOptApplyCharHeight;
        private System.Windows.Forms.CheckBox chkOptScalePmx;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radOptMotionSourceMmd;
        private System.Windows.Forms.RadioButton radOptMotionSourceMltd;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton radOptAnimFrameRate30;
        private System.Windows.Forms.RadioButton radOptAnimFrameRate60;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chkOptScaleVmd;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnOutputModel;
        private System.Windows.Forms.TextBox txtOutputModel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnOutputCameraMotion;
        private System.Windows.Forms.TextBox txtOutputCameraMotion;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnOutputCharAnim;
        private System.Windows.Forms.TextBox txtOutputCharAnim;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton radOptCamFormatMvd;
        private System.Windows.Forms.RadioButton radOptCamFormatVmd;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtOptFixedFov;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnInputFacialExpression;
        private System.Windows.Forms.TextBox txtInputFacialExpression;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox cboOptSongPosition;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.OpenFileDialog ofd;
        private System.Windows.Forms.SaveFileDialog sfd;
    }
}