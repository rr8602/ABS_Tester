namespace ABS_Tester.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.grpConnection = new System.Windows.Forms.GroupBox();
            this.chkRealtime = new System.Windows.Forms.CheckBox();
            this.chkTerminalResistor = new System.Windows.Forms.CheckBox();
            this.btnEcuConnect = new System.Windows.Forms.Button();
            this.btnUsbConnect = new System.Windows.Forms.Button();
            this.grpEcuInfo = new System.Windows.Forms.GroupBox();
            this.btnReadEcuInfo = new System.Windows.Forms.Button();
            this.txtConfig = new System.Windows.Forms.TextBox();
            this.txtMfgDate = new System.Windows.Forms.TextBox();
            this.txtSerialNo = new System.Windows.Forms.TextBox();
            this.txtSoftwareNo = new System.Windows.Forms.TextBox();
            this.txtHardwareNo = new System.Windows.Forms.TextBox();
            this.grpDtc = new System.Windows.Forms.GroupBox();
            this.btnClearDtc = new System.Windows.Forms.Button();
            this.btnReadDtc = new System.Windows.Forms.Button();
            this.txtDtc = new System.Windows.Forms.TextBox();
            this.grpVoltage = new System.Windows.Forms.GroupBox();
            this.btnReadVoltage = new System.Windows.Forms.Button();
            this.lblVoltageUb = new System.Windows.Forms.Label();
            this.lblVoltageUz = new System.Windows.Forms.Label();
            this.grpWss = new System.Windows.Forms.GroupBox();
            this.lblSteeringAngle = new System.Windows.Forms.Label();
            this.btnReadSteeringAngle = new System.Windows.Forms.Button();
            this.btnReadWss = new System.Windows.Forms.Button();
            this.lblWssRR = new System.Windows.Forms.Label();
            this.lblWssRL = new System.Windows.Forms.Label();
            this.lblWssFR = new System.Windows.Forms.Label();
            this.lblWssFL = new System.Windows.Forms.Label();
            this.grpLampTest = new System.Windows.Forms.GroupBox();
            this.btnSasSetting = new System.Windows.Forms.Button();
            this.btnLampStop = new System.Windows.Forms.Button();
            this.btnLampHillHolder = new System.Windows.Forms.Button();
            this.btnLampVdcFully = new System.Windows.Forms.Button();
            this.btnLampVdc = new System.Windows.Forms.Button();
            this.btnLampAmber = new System.Windows.Forms.Button();
            this.btnLampRed = new System.Windows.Forms.Button();
            this.grpAirGap = new System.Windows.Forms.GroupBox();
            this.lblAirGapRR = new System.Windows.Forms.Label();
            this.lblAirGapRL = new System.Windows.Forms.Label();
            this.lblAirGapFR = new System.Windows.Forms.Label();
            this.lblAirGapFL = new System.Windows.Forms.Label();
            this.btnAirGapRead = new System.Windows.Forms.Button();
            this.btnAirGapInitRear = new System.Windows.Forms.Button();
            this.btnAirGapInitFront = new System.Windows.Forms.Button();
            this.grpLwsFbm = new System.Windows.Forms.GroupBox();
            this.lblFbmRR = new System.Windows.Forms.Label();
            this.lblFbmRL = new System.Windows.Forms.Label();
            this.lblFbmFR = new System.Windows.Forms.Label();
            this.lblFbmFL = new System.Windows.Forms.Label();
            this.btnReadFbmForce = new System.Windows.Forms.Button();
            this.btnReadFbmSignal = new System.Windows.Forms.Button();
            this.btnFbmStop = new System.Windows.Forms.Button();
            this.btnFbmStart = new System.Windows.Forms.Button();
            this.btnReadSwitch = new System.Windows.Forms.Button();
            this.btnReadLws = new System.Windows.Forms.Button();
            this.grpValveTest = new System.Windows.Forms.GroupBox();
            this.btnValveStop = new System.Windows.Forms.Button();
            this.btnValveDec = new System.Windows.Forms.Button();
            this.btnValveInc = new System.Windows.Forms.Button();
            this.cboValveWheel = new System.Windows.Forms.ComboBox();
            this.grpAutoTest = new System.Windows.Forms.GroupBox();
            this.lblTestResult = new System.Windows.Forms.Label();
            this.lblTestStatus = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblLogFile = new System.Windows.Forms.Label();
            this.btnStopAutoTest = new System.Windows.Forms.Button();
            this.btnAutoTest = new System.Windows.Forms.Button();
            this.grpLog = new System.Windows.Forms.GroupBox();
            this.btnOpenLogFolder = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.grpConnection.SuspendLayout();
            this.grpEcuInfo.SuspendLayout();
            this.grpDtc.SuspendLayout();
            this.grpVoltage.SuspendLayout();
            this.grpWss.SuspendLayout();
            this.grpLampTest.SuspendLayout();
            this.grpAirGap.SuspendLayout();
            this.grpLwsFbm.SuspendLayout();
            this.grpValveTest.SuspendLayout();
            this.grpAutoTest.SuspendLayout();
            this.grpLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpConnection
            // 
            this.grpConnection.Controls.Add(this.chkRealtime);
            this.grpConnection.Controls.Add(this.chkTerminalResistor);
            this.grpConnection.Controls.Add(this.btnEcuConnect);
            this.grpConnection.Controls.Add(this.btnUsbConnect);
            this.grpConnection.Location = new System.Drawing.Point(12, 12);
            this.grpConnection.Name = "grpConnection";
            this.grpConnection.Size = new System.Drawing.Size(380, 80);
            this.grpConnection.TabIndex = 0;
            this.grpConnection.TabStop = false;
            this.grpConnection.Text = "1. 연결";
            // 
            // chkRealtime
            // 
            this.chkRealtime.AutoSize = true;
            this.chkRealtime.Location = new System.Drawing.Point(240, 48);
            this.chkRealtime.Name = "chkRealtime";
            this.chkRealtime.Size = new System.Drawing.Size(114, 19);
            this.chkRealtime.TabIndex = 3;
            this.chkRealtime.Text = "실시간 모니터링";
            this.chkRealtime.UseVisualStyleBackColor = true;
            this.chkRealtime.CheckedChanged += new System.EventHandler(this.chkRealtime_CheckedChanged);
            // 
            // chkTerminalResistor
            // 
            this.chkTerminalResistor.AutoSize = true;
            this.chkTerminalResistor.Checked = true;
            this.chkTerminalResistor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTerminalResistor.Location = new System.Drawing.Point(240, 25);
            this.chkTerminalResistor.Name = "chkTerminalResistor";
            this.chkTerminalResistor.Size = new System.Drawing.Size(100, 19);
            this.chkTerminalResistor.TabIndex = 2;
            this.chkTerminalResistor.Text = "종단저항 (AT)";
            this.chkTerminalResistor.UseVisualStyleBackColor = true;
            // 
            // btnEcuConnect
            // 
            this.btnEcuConnect.Location = new System.Drawing.Point(125, 30);
            this.btnEcuConnect.Name = "btnEcuConnect";
            this.btnEcuConnect.Size = new System.Drawing.Size(100, 35);
            this.btnEcuConnect.TabIndex = 1;
            this.btnEcuConnect.Text = "ECU 연결";
            this.btnEcuConnect.UseVisualStyleBackColor = true;
            this.btnEcuConnect.Click += new System.EventHandler(this.btnEcuConnect_Click);
            // 
            // btnUsbConnect
            // 
            this.btnUsbConnect.Location = new System.Drawing.Point(15, 30);
            this.btnUsbConnect.Name = "btnUsbConnect";
            this.btnUsbConnect.Size = new System.Drawing.Size(100, 35);
            this.btnUsbConnect.TabIndex = 0;
            this.btnUsbConnect.Text = "USB 연결";
            this.btnUsbConnect.UseVisualStyleBackColor = true;
            this.btnUsbConnect.Click += new System.EventHandler(this.btnUsbConnect_Click);
            // 
            // grpEcuInfo
            // 
            this.grpEcuInfo.Controls.Add(this.btnReadEcuInfo);
            this.grpEcuInfo.Controls.Add(this.txtConfig);
            this.grpEcuInfo.Controls.Add(this.txtMfgDate);
            this.grpEcuInfo.Controls.Add(this.txtSerialNo);
            this.grpEcuInfo.Controls.Add(this.txtSoftwareNo);
            this.grpEcuInfo.Controls.Add(this.txtHardwareNo);
            this.grpEcuInfo.Location = new System.Drawing.Point(12, 100);
            this.grpEcuInfo.Name = "grpEcuInfo";
            this.grpEcuInfo.Size = new System.Drawing.Size(380, 180);
            this.grpEcuInfo.TabIndex = 1;
            this.grpEcuInfo.TabStop = false;
            this.grpEcuInfo.Text = "2. ECU 정보";
            // 
            // btnReadEcuInfo
            // 
            this.btnReadEcuInfo.Location = new System.Drawing.Point(310, 25);
            this.btnReadEcuInfo.Name = "btnReadEcuInfo";
            this.btnReadEcuInfo.Size = new System.Drawing.Size(55, 30);
            this.btnReadEcuInfo.TabIndex = 5;
            this.btnReadEcuInfo.Text = "읽기";
            this.btnReadEcuInfo.UseVisualStyleBackColor = true;
            this.btnReadEcuInfo.Click += new System.EventHandler(this.btnReadEcuInfo_Click);
            // 
            // txtConfig
            // 
            this.txtConfig.Location = new System.Drawing.Point(100, 137);
            this.txtConfig.Name = "txtConfig";
            this.txtConfig.ReadOnly = true;
            this.txtConfig.Size = new System.Drawing.Size(200, 23);
            this.txtConfig.TabIndex = 4;
            // 
            // txtMfgDate
            // 
            this.txtMfgDate.Location = new System.Drawing.Point(100, 109);
            this.txtMfgDate.Name = "txtMfgDate";
            this.txtMfgDate.ReadOnly = true;
            this.txtMfgDate.Size = new System.Drawing.Size(200, 23);
            this.txtMfgDate.TabIndex = 3;
            // 
            // txtSerialNo
            // 
            this.txtSerialNo.Location = new System.Drawing.Point(100, 81);
            this.txtSerialNo.Name = "txtSerialNo";
            this.txtSerialNo.ReadOnly = true;
            this.txtSerialNo.Size = new System.Drawing.Size(200, 23);
            this.txtSerialNo.TabIndex = 2;
            // 
            // txtSoftwareNo
            // 
            this.txtSoftwareNo.Location = new System.Drawing.Point(100, 53);
            this.txtSoftwareNo.Name = "txtSoftwareNo";
            this.txtSoftwareNo.ReadOnly = true;
            this.txtSoftwareNo.Size = new System.Drawing.Size(200, 23);
            this.txtSoftwareNo.TabIndex = 1;
            // 
            // txtHardwareNo
            // 
            this.txtHardwareNo.Location = new System.Drawing.Point(100, 25);
            this.txtHardwareNo.Name = "txtHardwareNo";
            this.txtHardwareNo.ReadOnly = true;
            this.txtHardwareNo.Size = new System.Drawing.Size(200, 23);
            this.txtHardwareNo.TabIndex = 0;
            // 
            // grpDtc
            // 
            this.grpDtc.Controls.Add(this.btnClearDtc);
            this.grpDtc.Controls.Add(this.btnReadDtc);
            this.grpDtc.Controls.Add(this.txtDtc);
            this.grpDtc.Location = new System.Drawing.Point(12, 290);
            this.grpDtc.Name = "grpDtc";
            this.grpDtc.Size = new System.Drawing.Size(380, 100);
            this.grpDtc.TabIndex = 2;
            this.grpDtc.TabStop = false;
            this.grpDtc.Text = "3. DTC";
            // 
            // btnClearDtc
            // 
            this.btnClearDtc.Location = new System.Drawing.Point(305, 57);
            this.btnClearDtc.Name = "btnClearDtc";
            this.btnClearDtc.Size = new System.Drawing.Size(60, 28);
            this.btnClearDtc.TabIndex = 2;
            this.btnClearDtc.Text = "삭제";
            this.btnClearDtc.UseVisualStyleBackColor = true;
            this.btnClearDtc.Click += new System.EventHandler(this.btnClearDtc_Click);
            // 
            // btnReadDtc
            // 
            this.btnReadDtc.Location = new System.Drawing.Point(305, 25);
            this.btnReadDtc.Name = "btnReadDtc";
            this.btnReadDtc.Size = new System.Drawing.Size(60, 28);
            this.btnReadDtc.TabIndex = 1;
            this.btnReadDtc.Text = "읽기";
            this.btnReadDtc.UseVisualStyleBackColor = true;
            this.btnReadDtc.Click += new System.EventHandler(this.btnReadDtc_Click);
            // 
            // txtDtc
            // 
            this.txtDtc.Location = new System.Drawing.Point(15, 25);
            this.txtDtc.Multiline = true;
            this.txtDtc.Name = "txtDtc";
            this.txtDtc.ReadOnly = true;
            this.txtDtc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDtc.Size = new System.Drawing.Size(280, 60);
            this.txtDtc.TabIndex = 0;
            // 
            // grpVoltage
            // 
            this.grpVoltage.Controls.Add(this.btnReadVoltage);
            this.grpVoltage.Controls.Add(this.lblVoltageUb);
            this.grpVoltage.Controls.Add(this.lblVoltageUz);
            this.grpVoltage.Location = new System.Drawing.Point(400, 12);
            this.grpVoltage.Name = "grpVoltage";
            this.grpVoltage.Size = new System.Drawing.Size(200, 100);
            this.grpVoltage.TabIndex = 3;
            this.grpVoltage.TabStop = false;
            this.grpVoltage.Text = "4. 전압";
            // 
            // btnReadVoltage
            // 
            this.btnReadVoltage.Location = new System.Drawing.Point(15, 75);
            this.btnReadVoltage.Name = "btnReadVoltage";
            this.btnReadVoltage.Size = new System.Drawing.Size(55, 20);
            this.btnReadVoltage.TabIndex = 2;
            this.btnReadVoltage.Text = "읽기";
            this.btnReadVoltage.UseVisualStyleBackColor = true;
            this.btnReadVoltage.Click += new System.EventHandler(this.btnReadVoltage_Click);
            // 
            // lblVoltageUb
            // 
            this.lblVoltageUb.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Bold);
            this.lblVoltageUb.Location = new System.Drawing.Point(100, 52);
            this.lblVoltageUb.Name = "lblVoltageUb";
            this.lblVoltageUb.Size = new System.Drawing.Size(80, 20);
            this.lblVoltageUb.TabIndex = 1;
            this.lblVoltageUb.Text = "- V";
            // 
            // lblVoltageUz
            // 
            this.lblVoltageUz.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Bold);
            this.lblVoltageUz.Location = new System.Drawing.Point(100, 28);
            this.lblVoltageUz.Name = "lblVoltageUz";
            this.lblVoltageUz.Size = new System.Drawing.Size(80, 20);
            this.lblVoltageUz.TabIndex = 0;
            this.lblVoltageUz.Text = "- V";
            // 
            // grpWss
            // 
            this.grpWss.Controls.Add(this.lblSteeringAngle);
            this.grpWss.Controls.Add(this.btnReadSteeringAngle);
            this.grpWss.Controls.Add(this.btnReadWss);
            this.grpWss.Controls.Add(this.lblWssRR);
            this.grpWss.Controls.Add(this.lblWssRL);
            this.grpWss.Controls.Add(this.lblWssFR);
            this.grpWss.Controls.Add(this.lblWssFL);
            this.grpWss.Location = new System.Drawing.Point(400, 120);
            this.grpWss.Name = "grpWss";
            this.grpWss.Size = new System.Drawing.Size(200, 150);
            this.grpWss.TabIndex = 4;
            this.grpWss.TabStop = false;
            this.grpWss.Text = "5. WSS / Steering";
            // 
            // lblSteeringAngle
            // 
            this.lblSteeringAngle.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblSteeringAngle.Location = new System.Drawing.Point(50, 124);
            this.lblSteeringAngle.Name = "lblSteeringAngle";
            this.lblSteeringAngle.Size = new System.Drawing.Size(140, 20);
            this.lblSteeringAngle.TabIndex = 6;
            this.lblSteeringAngle.Text = "조향각: - °";
            // 
            // btnReadSteeringAngle
            // 
            this.btnReadSteeringAngle.Location = new System.Drawing.Point(140, 58);
            this.btnReadSteeringAngle.Name = "btnReadSteeringAngle";
            this.btnReadSteeringAngle.Size = new System.Drawing.Size(50, 25);
            this.btnReadSteeringAngle.TabIndex = 5;
            this.btnReadSteeringAngle.Text = "조향각";
            this.btnReadSteeringAngle.UseVisualStyleBackColor = true;
            this.btnReadSteeringAngle.Click += new System.EventHandler(this.btnReadSteeringAngle_Click);
            // 
            // btnReadWss
            // 
            this.btnReadWss.Location = new System.Drawing.Point(140, 28);
            this.btnReadWss.Name = "btnReadWss";
            this.btnReadWss.Size = new System.Drawing.Size(50, 25);
            this.btnReadWss.TabIndex = 4;
            this.btnReadWss.Text = "읽기";
            this.btnReadWss.UseVisualStyleBackColor = true;
            this.btnReadWss.Click += new System.EventHandler(this.btnReadWss_Click);
            // 
            // lblWssRR
            // 
            this.lblWssRR.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblWssRR.Location = new System.Drawing.Point(50, 100);
            this.lblWssRR.Name = "lblWssRR";
            this.lblWssRR.Size = new System.Drawing.Size(80, 20);
            this.lblWssRR.TabIndex = 3;
            this.lblWssRR.Text = "- km/h";
            // 
            // lblWssRL
            // 
            this.lblWssRL.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblWssRL.Location = new System.Drawing.Point(50, 76);
            this.lblWssRL.Name = "lblWssRL";
            this.lblWssRL.Size = new System.Drawing.Size(80, 20);
            this.lblWssRL.TabIndex = 2;
            this.lblWssRL.Text = "- km/h";
            // 
            // lblWssFR
            // 
            this.lblWssFR.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblWssFR.Location = new System.Drawing.Point(50, 52);
            this.lblWssFR.Name = "lblWssFR";
            this.lblWssFR.Size = new System.Drawing.Size(80, 20);
            this.lblWssFR.TabIndex = 1;
            this.lblWssFR.Text = "- km/h";
            // 
            // lblWssFL
            // 
            this.lblWssFL.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblWssFL.Location = new System.Drawing.Point(50, 28);
            this.lblWssFL.Name = "lblWssFL";
            this.lblWssFL.Size = new System.Drawing.Size(80, 20);
            this.lblWssFL.TabIndex = 0;
            this.lblWssFL.Text = "- km/h";
            // 
            // grpLampTest
            // 
            this.grpLampTest.Controls.Add(this.btnSasSetting);
            this.grpLampTest.Controls.Add(this.btnLampStop);
            this.grpLampTest.Controls.Add(this.btnLampHillHolder);
            this.grpLampTest.Controls.Add(this.btnLampVdcFully);
            this.grpLampTest.Controls.Add(this.btnLampVdc);
            this.grpLampTest.Controls.Add(this.btnLampAmber);
            this.grpLampTest.Controls.Add(this.btnLampRed);
            this.grpLampTest.Location = new System.Drawing.Point(610, 12);
            this.grpLampTest.Name = "grpLampTest";
            this.grpLampTest.Size = new System.Drawing.Size(280, 130);
            this.grpLampTest.TabIndex = 5;
            this.grpLampTest.TabStop = false;
            this.grpLampTest.Text = "6. 램프 테스트";
            // 
            // btnSasSetting
            // 
            this.btnSasSetting.Location = new System.Drawing.Point(100, 92);
            this.btnSasSetting.Name = "btnSasSetting";
            this.btnSasSetting.Size = new System.Drawing.Size(85, 28);
            this.btnSasSetting.TabIndex = 6;
            this.btnSasSetting.Text = "SAS 설정";
            this.btnSasSetting.UseVisualStyleBackColor = true;
            this.btnSasSetting.Click += new System.EventHandler(this.btnSasSetting_Click);
            // 
            // btnLampStop
            // 
            this.btnLampStop.Location = new System.Drawing.Point(15, 92);
            this.btnLampStop.Name = "btnLampStop";
            this.btnLampStop.Size = new System.Drawing.Size(75, 28);
            this.btnLampStop.TabIndex = 5;
            this.btnLampStop.Text = "정지";
            this.btnLampStop.UseVisualStyleBackColor = true;
            this.btnLampStop.Click += new System.EventHandler(this.btnLampStop_Click);
            // 
            // btnLampHillHolder
            // 
            this.btnLampHillHolder.Location = new System.Drawing.Point(100, 58);
            this.btnLampHillHolder.Name = "btnLampHillHolder";
            this.btnLampHillHolder.Size = new System.Drawing.Size(85, 28);
            this.btnLampHillHolder.TabIndex = 4;
            this.btnLampHillHolder.Text = "HILLHOLDER";
            this.btnLampHillHolder.UseVisualStyleBackColor = true;
            this.btnLampHillHolder.Click += new System.EventHandler(this.btnLampHillHolder_Click);
            // 
            // btnLampVdcFully
            // 
            this.btnLampVdcFully.Location = new System.Drawing.Point(15, 58);
            this.btnLampVdcFully.Name = "btnLampVdcFully";
            this.btnLampVdcFully.Size = new System.Drawing.Size(80, 28);
            this.btnLampVdcFully.TabIndex = 3;
            this.btnLampVdcFully.Text = "VDC FULLY";
            this.btnLampVdcFully.UseVisualStyleBackColor = true;
            this.btnLampVdcFully.Click += new System.EventHandler(this.btnLampVdcFully_Click);
            // 
            // btnLampVdc
            // 
            this.btnLampVdc.Location = new System.Drawing.Point(190, 25);
            this.btnLampVdc.Name = "btnLampVdc";
            this.btnLampVdc.Size = new System.Drawing.Size(75, 28);
            this.btnLampVdc.TabIndex = 2;
            this.btnLampVdc.Text = "VDC";
            this.btnLampVdc.UseVisualStyleBackColor = true;
            this.btnLampVdc.Click += new System.EventHandler(this.btnLampVdc_Click);
            // 
            // btnLampAmber
            // 
            this.btnLampAmber.BackColor = System.Drawing.Color.Gold;
            this.btnLampAmber.Location = new System.Drawing.Point(100, 25);
            this.btnLampAmber.Name = "btnLampAmber";
            this.btnLampAmber.Size = new System.Drawing.Size(85, 28);
            this.btnLampAmber.TabIndex = 1;
            this.btnLampAmber.Text = "EBS AMBER";
            this.btnLampAmber.UseVisualStyleBackColor = false;
            this.btnLampAmber.Click += new System.EventHandler(this.btnLampAmber_Click);
            // 
            // btnLampRed
            // 
            this.btnLampRed.BackColor = System.Drawing.Color.LightCoral;
            this.btnLampRed.Location = new System.Drawing.Point(15, 25);
            this.btnLampRed.Name = "btnLampRed";
            this.btnLampRed.Size = new System.Drawing.Size(80, 28);
            this.btnLampRed.TabIndex = 0;
            this.btnLampRed.Text = "EBS RED";
            this.btnLampRed.UseVisualStyleBackColor = false;
            this.btnLampRed.Click += new System.EventHandler(this.btnLampRed_Click);
            // 
            // grpAirGap
            // 
            this.grpAirGap.Controls.Add(this.lblAirGapRR);
            this.grpAirGap.Controls.Add(this.lblAirGapRL);
            this.grpAirGap.Controls.Add(this.lblAirGapFR);
            this.grpAirGap.Controls.Add(this.lblAirGapFL);
            this.grpAirGap.Controls.Add(this.btnAirGapRead);
            this.grpAirGap.Controls.Add(this.btnAirGapInitRear);
            this.grpAirGap.Controls.Add(this.btnAirGapInitFront);
            this.grpAirGap.Location = new System.Drawing.Point(400, 280);
            this.grpAirGap.Name = "grpAirGap";
            this.grpAirGap.Size = new System.Drawing.Size(200, 110);
            this.grpAirGap.TabIndex = 9;
            this.grpAirGap.TabStop = false;
            this.grpAirGap.Text = "8. Air Gap";
            // 
            // lblAirGapRR
            // 
            this.lblAirGapRR.Location = new System.Drawing.Point(100, 78);
            this.lblAirGapRR.Name = "lblAirGapRR";
            this.lblAirGapRR.Size = new System.Drawing.Size(85, 18);
            this.lblAirGapRR.TabIndex = 6;
            this.lblAirGapRR.Text = "RR: -";
            // 
            // lblAirGapRL
            // 
            this.lblAirGapRL.Location = new System.Drawing.Point(10, 78);
            this.lblAirGapRL.Name = "lblAirGapRL";
            this.lblAirGapRL.Size = new System.Drawing.Size(85, 18);
            this.lblAirGapRL.TabIndex = 5;
            this.lblAirGapRL.Text = "RL: -";
            // 
            // lblAirGapFR
            // 
            this.lblAirGapFR.Location = new System.Drawing.Point(100, 55);
            this.lblAirGapFR.Name = "lblAirGapFR";
            this.lblAirGapFR.Size = new System.Drawing.Size(85, 18);
            this.lblAirGapFR.TabIndex = 4;
            this.lblAirGapFR.Text = "FR: -";
            // 
            // lblAirGapFL
            // 
            this.lblAirGapFL.Location = new System.Drawing.Point(10, 55);
            this.lblAirGapFL.Name = "lblAirGapFL";
            this.lblAirGapFL.Size = new System.Drawing.Size(85, 18);
            this.lblAirGapFL.TabIndex = 3;
            this.lblAirGapFL.Text = "FL: -";
            // 
            // btnAirGapRead
            // 
            this.btnAirGapRead.Location = new System.Drawing.Point(130, 22);
            this.btnAirGapRead.Name = "btnAirGapRead";
            this.btnAirGapRead.Size = new System.Drawing.Size(55, 25);
            this.btnAirGapRead.TabIndex = 2;
            this.btnAirGapRead.Text = "읽기";
            this.btnAirGapRead.UseVisualStyleBackColor = true;
            this.btnAirGapRead.Click += new System.EventHandler(this.btnAirGapRead_Click);
            // 
            // btnAirGapInitRear
            // 
            this.btnAirGapInitRear.Location = new System.Drawing.Point(70, 22);
            this.btnAirGapInitRear.Name = "btnAirGapInitRear";
            this.btnAirGapInitRear.Size = new System.Drawing.Size(55, 25);
            this.btnAirGapInitRear.TabIndex = 1;
            this.btnAirGapInitRear.Text = "초기(R)";
            this.btnAirGapInitRear.UseVisualStyleBackColor = true;
            this.btnAirGapInitRear.Click += new System.EventHandler(this.btnAirGapInitRear_Click);
            // 
            // btnAirGapInitFront
            // 
            this.btnAirGapInitFront.Location = new System.Drawing.Point(10, 22);
            this.btnAirGapInitFront.Name = "btnAirGapInitFront";
            this.btnAirGapInitFront.Size = new System.Drawing.Size(55, 25);
            this.btnAirGapInitFront.TabIndex = 0;
            this.btnAirGapInitFront.Text = "초기(F)";
            this.btnAirGapInitFront.UseVisualStyleBackColor = true;
            this.btnAirGapInitFront.Click += new System.EventHandler(this.btnAirGapInitFront_Click);
            // 
            // grpLwsFbm
            // 
            this.grpLwsFbm.Controls.Add(this.lblFbmRR);
            this.grpLwsFbm.Controls.Add(this.lblFbmRL);
            this.grpLwsFbm.Controls.Add(this.lblFbmFR);
            this.grpLwsFbm.Controls.Add(this.lblFbmFL);
            this.grpLwsFbm.Controls.Add(this.btnReadFbmForce);
            this.grpLwsFbm.Controls.Add(this.btnReadFbmSignal);
            this.grpLwsFbm.Controls.Add(this.btnFbmStop);
            this.grpLwsFbm.Controls.Add(this.btnFbmStart);
            this.grpLwsFbm.Controls.Add(this.btnReadSwitch);
            this.grpLwsFbm.Controls.Add(this.btnReadLws);
            this.grpLwsFbm.Location = new System.Drawing.Point(610, 280);
            this.grpLwsFbm.Name = "grpLwsFbm";
            this.grpLwsFbm.Size = new System.Drawing.Size(280, 110);
            this.grpLwsFbm.TabIndex = 10;
            this.grpLwsFbm.TabStop = false;
            this.grpLwsFbm.Text = "9. LWS / FBM";
            // 
            // lblFbmRR
            // 
            this.lblFbmRR.Location = new System.Drawing.Point(210, 48);
            this.lblFbmRR.Name = "lblFbmRR";
            this.lblFbmRR.Size = new System.Drawing.Size(65, 18);
            this.lblFbmRR.TabIndex = 9;
            this.lblFbmRR.Text = "RR: -";
            // 
            // lblFbmRL
            // 
            this.lblFbmRL.Location = new System.Drawing.Point(145, 48);
            this.lblFbmRL.Name = "lblFbmRL";
            this.lblFbmRL.Size = new System.Drawing.Size(65, 18);
            this.lblFbmRL.TabIndex = 8;
            this.lblFbmRL.Text = "RL: -";
            // 
            // lblFbmFR
            // 
            this.lblFbmFR.Location = new System.Drawing.Point(210, 25);
            this.lblFbmFR.Name = "lblFbmFR";
            this.lblFbmFR.Size = new System.Drawing.Size(65, 18);
            this.lblFbmFR.TabIndex = 7;
            this.lblFbmFR.Text = "FR: -";
            // 
            // lblFbmFL
            // 
            this.lblFbmFL.Location = new System.Drawing.Point(145, 25);
            this.lblFbmFL.Name = "lblFbmFL";
            this.lblFbmFL.Size = new System.Drawing.Size(65, 18);
            this.lblFbmFL.TabIndex = 6;
            this.lblFbmFL.Text = "FL: -";
            // 
            // btnReadFbmForce
            // 
            this.btnReadFbmForce.Location = new System.Drawing.Point(75, 82);
            this.btnReadFbmForce.Name = "btnReadFbmForce";
            this.btnReadFbmForce.Size = new System.Drawing.Size(60, 22);
            this.btnReadFbmForce.TabIndex = 5;
            this.btnReadFbmForce.Text = "Force";
            this.btnReadFbmForce.UseVisualStyleBackColor = true;
            this.btnReadFbmForce.Click += new System.EventHandler(this.btnReadFbmForce_Click);
            // 
            // btnReadFbmSignal
            // 
            this.btnReadFbmSignal.Location = new System.Drawing.Point(10, 82);
            this.btnReadFbmSignal.Name = "btnReadFbmSignal";
            this.btnReadFbmSignal.Size = new System.Drawing.Size(60, 22);
            this.btnReadFbmSignal.TabIndex = 4;
            this.btnReadFbmSignal.Text = "Signal";
            this.btnReadFbmSignal.UseVisualStyleBackColor = true;
            this.btnReadFbmSignal.Click += new System.EventHandler(this.btnReadFbmSignal_Click);
            // 
            // btnFbmStop
            // 
            this.btnFbmStop.Location = new System.Drawing.Point(75, 52);
            this.btnFbmStop.Name = "btnFbmStop";
            this.btnFbmStop.Size = new System.Drawing.Size(60, 25);
            this.btnFbmStop.TabIndex = 3;
            this.btnFbmStop.Text = "FBM 정지";
            this.btnFbmStop.UseVisualStyleBackColor = true;
            this.btnFbmStop.Click += new System.EventHandler(this.btnFbmStop_Click);
            // 
            // btnFbmStart
            // 
            this.btnFbmStart.BackColor = System.Drawing.Color.LightGreen;
            this.btnFbmStart.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnFbmStart.Location = new System.Drawing.Point(10, 52);
            this.btnFbmStart.Name = "btnFbmStart";
            this.btnFbmStart.Size = new System.Drawing.Size(60, 25);
            this.btnFbmStart.TabIndex = 2;
            this.btnFbmStart.Text = "FBM 시작";
            this.btnFbmStart.UseVisualStyleBackColor = false;
            this.btnFbmStart.Click += new System.EventHandler(this.btnFbmStart_Click);
            // 
            // btnReadSwitch
            // 
            this.btnReadSwitch.Location = new System.Drawing.Point(75, 22);
            this.btnReadSwitch.Name = "btnReadSwitch";
            this.btnReadSwitch.Size = new System.Drawing.Size(60, 25);
            this.btnReadSwitch.TabIndex = 1;
            this.btnReadSwitch.Text = "Switch";
            this.btnReadSwitch.UseVisualStyleBackColor = true;
            this.btnReadSwitch.Click += new System.EventHandler(this.btnReadSwitch_Click);
            // 
            // btnReadLws
            // 
            this.btnReadLws.Location = new System.Drawing.Point(10, 22);
            this.btnReadLws.Name = "btnReadLws";
            this.btnReadLws.Size = new System.Drawing.Size(60, 25);
            this.btnReadLws.TabIndex = 0;
            this.btnReadLws.Text = "LWS";
            this.btnReadLws.UseVisualStyleBackColor = true;
            this.btnReadLws.Click += new System.EventHandler(this.btnReadLws_Click);
            // 
            // grpValveTest
            // 
            this.grpValveTest.Controls.Add(this.btnValveStop);
            this.grpValveTest.Controls.Add(this.btnValveDec);
            this.grpValveTest.Controls.Add(this.btnValveInc);
            this.grpValveTest.Controls.Add(this.cboValveWheel);
            this.grpValveTest.Location = new System.Drawing.Point(610, 150);
            this.grpValveTest.Name = "grpValveTest";
            this.grpValveTest.Size = new System.Drawing.Size(280, 120);
            this.grpValveTest.TabIndex = 6;
            this.grpValveTest.TabStop = false;
            this.grpValveTest.Text = "7. 밸브 테스트";
            // 
            // btnValveStop
            // 
            this.btnValveStop.Location = new System.Drawing.Point(185, 60);
            this.btnValveStop.Name = "btnValveStop";
            this.btnValveStop.Size = new System.Drawing.Size(75, 35);
            this.btnValveStop.TabIndex = 3;
            this.btnValveStop.Text = "정지";
            this.btnValveStop.UseVisualStyleBackColor = true;
            this.btnValveStop.Click += new System.EventHandler(this.btnValveStop_Click);
            // 
            // btnValveDec
            // 
            this.btnValveDec.BackColor = System.Drawing.Color.LightPink;
            this.btnValveDec.Location = new System.Drawing.Point(100, 60);
            this.btnValveDec.Name = "btnValveDec";
            this.btnValveDec.Size = new System.Drawing.Size(75, 35);
            this.btnValveDec.TabIndex = 2;
            this.btnValveDec.Text = "감압";
            this.btnValveDec.UseVisualStyleBackColor = false;
            this.btnValveDec.Click += new System.EventHandler(this.btnValveDec_Click);
            // 
            // btnValveInc
            // 
            this.btnValveInc.BackColor = System.Drawing.Color.LightBlue;
            this.btnValveInc.Location = new System.Drawing.Point(15, 60);
            this.btnValveInc.Name = "btnValveInc";
            this.btnValveInc.Size = new System.Drawing.Size(75, 35);
            this.btnValveInc.TabIndex = 1;
            this.btnValveInc.Text = "증압";
            this.btnValveInc.UseVisualStyleBackColor = false;
            this.btnValveInc.Click += new System.EventHandler(this.btnValveInc_Click);
            // 
            // cboValveWheel
            // 
            this.cboValveWheel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboValveWheel.FormattingEnabled = true;
            this.cboValveWheel.Items.AddRange(new object[] {
            "FL",
            "FR",
            "RL",
            "RR"});
            this.cboValveWheel.Location = new System.Drawing.Point(75, 25);
            this.cboValveWheel.Name = "cboValveWheel";
            this.cboValveWheel.Size = new System.Drawing.Size(80, 23);
            this.cboValveWheel.TabIndex = 0;
            // 
            // grpAutoTest
            // 
            this.grpAutoTest.Controls.Add(this.lblTestResult);
            this.grpAutoTest.Controls.Add(this.lblTestStatus);
            this.grpAutoTest.Controls.Add(this.progressBar);
            this.grpAutoTest.Controls.Add(this.lblLogFile);
            this.grpAutoTest.Controls.Add(this.btnStopAutoTest);
            this.grpAutoTest.Controls.Add(this.btnAutoTest);
            this.grpAutoTest.Location = new System.Drawing.Point(900, 12);
            this.grpAutoTest.Name = "grpAutoTest";
            this.grpAutoTest.Size = new System.Drawing.Size(280, 260);
            this.grpAutoTest.TabIndex = 11;
            this.grpAutoTest.TabStop = false;
            this.grpAutoTest.Text = "자동 테스트";
            // 
            // lblTestResult
            // 
            this.lblTestResult.Location = new System.Drawing.Point(15, 180);
            this.lblTestResult.Name = "lblTestResult";
            this.lblTestResult.Size = new System.Drawing.Size(250, 40);
            this.lblTestResult.TabIndex = 5;
            // 
            // lblTestStatus
            // 
            this.lblTestStatus.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblTestStatus.Location = new System.Drawing.Point(15, 150);
            this.lblTestStatus.Name = "lblTestStatus";
            this.lblTestStatus.Size = new System.Drawing.Size(250, 20);
            this.lblTestStatus.TabIndex = 4;
            this.lblTestStatus.Text = "대기 중...";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(15, 115);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(250, 23);
            this.progressBar.TabIndex = 3;
            // 
            // lblLogFile
            // 
            this.lblLogFile.Font = new System.Drawing.Font("맑은 고딕", 8F);
            this.lblLogFile.ForeColor = System.Drawing.Color.Gray;
            this.lblLogFile.Location = new System.Drawing.Point(15, 85);
            this.lblLogFile.Name = "lblLogFile";
            this.lblLogFile.Size = new System.Drawing.Size(250, 18);
            this.lblLogFile.TabIndex = 2;
            this.lblLogFile.Text = "ECU 연결 시 로그 자동 저장";
            // 
            // btnStopAutoTest
            // 
            this.btnStopAutoTest.Enabled = false;
            this.btnStopAutoTest.Location = new System.Drawing.Point(125, 25);
            this.btnStopAutoTest.Name = "btnStopAutoTest";
            this.btnStopAutoTest.Size = new System.Drawing.Size(70, 50);
            this.btnStopAutoTest.TabIndex = 1;
            this.btnStopAutoTest.Text = "중지";
            this.btnStopAutoTest.UseVisualStyleBackColor = true;
            this.btnStopAutoTest.Click += new System.EventHandler(this.btnStopAutoTest_Click);
            // 
            // btnAutoTest
            // 
            this.btnAutoTest.BackColor = System.Drawing.Color.LightGreen;
            this.btnAutoTest.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Bold);
            this.btnAutoTest.Location = new System.Drawing.Point(15, 25);
            this.btnAutoTest.Name = "btnAutoTest";
            this.btnAutoTest.Size = new System.Drawing.Size(100, 50);
            this.btnAutoTest.TabIndex = 0;
            this.btnAutoTest.Text = "자동 테스트\r\n시작";
            this.btnAutoTest.UseVisualStyleBackColor = false;
            this.btnAutoTest.Click += new System.EventHandler(this.btnAutoTest_Click);
            // 
            // grpLog
            // 
            this.grpLog.Controls.Add(this.btnOpenLogFolder);
            this.grpLog.Controls.Add(this.btnClearLog);
            this.grpLog.Controls.Add(this.lstLog);
            this.grpLog.Location = new System.Drawing.Point(12, 400);
            this.grpLog.Name = "grpLog";
            this.grpLog.Size = new System.Drawing.Size(1170, 340);
            this.grpLog.TabIndex = 8;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "통신 로그";
            // 
            // btnOpenLogFolder
            // 
            this.btnOpenLogFolder.Location = new System.Drawing.Point(1085, 85);
            this.btnOpenLogFolder.Name = "btnOpenLogFolder";
            this.btnOpenLogFolder.Size = new System.Drawing.Size(70, 50);
            this.btnOpenLogFolder.TabIndex = 2;
            this.btnOpenLogFolder.Text = "로그\r\n폴더";
            this.btnOpenLogFolder.UseVisualStyleBackColor = true;
            this.btnOpenLogFolder.Click += new System.EventHandler(this.btnOpenLogFolder_Click);
            // 
            // btnClearLog
            // 
            this.btnClearLog.Location = new System.Drawing.Point(1085, 25);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(70, 50);
            this.btnClearLog.TabIndex = 1;
            this.btnClearLog.Text = "로그\r\n지우기";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // lstLog
            // 
            this.lstLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.lstLog.FormattingEnabled = true;
            this.lstLog.HorizontalScrollbar = true;
            this.lstLog.ItemHeight = 14;
            this.lstLog.Location = new System.Drawing.Point(15, 25);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(1060, 298);
            this.lstLog.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 750);
            this.Controls.Add(this.grpLog);
            this.Controls.Add(this.grpAutoTest);
            this.Controls.Add(this.grpLwsFbm);
            this.Controls.Add(this.grpAirGap);
            this.Controls.Add(this.grpValveTest);
            this.Controls.Add(this.grpLampTest);
            this.Controls.Add(this.grpWss);
            this.Controls.Add(this.grpVoltage);
            this.Controls.Add(this.grpDtc);
            this.Controls.Add(this.grpEcuInfo);
            this.Controls.Add(this.grpConnection);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ABS/EBS ECU Tester - KNORR EBS5x";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.grpConnection.ResumeLayout(false);
            this.grpConnection.PerformLayout();
            this.grpEcuInfo.ResumeLayout(false);
            this.grpEcuInfo.PerformLayout();
            this.grpDtc.ResumeLayout(false);
            this.grpDtc.PerformLayout();
            this.grpVoltage.ResumeLayout(false);
            this.grpWss.ResumeLayout(false);
            this.grpLampTest.ResumeLayout(false);
            this.grpAirGap.ResumeLayout(false);
            this.grpLwsFbm.ResumeLayout(false);
            this.grpValveTest.ResumeLayout(false);
            this.grpAutoTest.ResumeLayout(false);
            this.grpLog.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpConnection;
        private System.Windows.Forms.Button btnUsbConnect;
        private System.Windows.Forms.Button btnEcuConnect;
        private System.Windows.Forms.CheckBox chkTerminalResistor;
        private System.Windows.Forms.CheckBox chkRealtime;
        private System.Windows.Forms.GroupBox grpEcuInfo;
        private System.Windows.Forms.TextBox txtHardwareNo;
        private System.Windows.Forms.TextBox txtSoftwareNo;
        private System.Windows.Forms.TextBox txtSerialNo;
        private System.Windows.Forms.TextBox txtMfgDate;
        private System.Windows.Forms.TextBox txtConfig;
        private System.Windows.Forms.Button btnReadEcuInfo;
        private System.Windows.Forms.GroupBox grpDtc;
        private System.Windows.Forms.TextBox txtDtc;
        private System.Windows.Forms.Button btnReadDtc;
        private System.Windows.Forms.Button btnClearDtc;
        private System.Windows.Forms.GroupBox grpVoltage;
        private System.Windows.Forms.Label lblVoltageUz;
        private System.Windows.Forms.Label lblVoltageUb;
        private System.Windows.Forms.Button btnReadVoltage;
        private System.Windows.Forms.GroupBox grpWss;
        private System.Windows.Forms.Label lblWssFL;
        private System.Windows.Forms.Label lblWssFR;
        private System.Windows.Forms.Label lblWssRL;
        private System.Windows.Forms.Label lblWssRR;
        private System.Windows.Forms.Button btnReadWss;
        private System.Windows.Forms.Button btnReadSteeringAngle;
        private System.Windows.Forms.Label lblSteeringAngle;
        private System.Windows.Forms.GroupBox grpLampTest;
        private System.Windows.Forms.Button btnLampRed;
        private System.Windows.Forms.Button btnLampAmber;
        private System.Windows.Forms.Button btnLampVdc;
        private System.Windows.Forms.Button btnLampVdcFully;
        private System.Windows.Forms.Button btnLampHillHolder;
        private System.Windows.Forms.Button btnLampStop;
        private System.Windows.Forms.Button btnSasSetting;
        private System.Windows.Forms.GroupBox grpAirGap;
        private System.Windows.Forms.Button btnAirGapInitFront;
        private System.Windows.Forms.Button btnAirGapInitRear;
        private System.Windows.Forms.Button btnAirGapRead;
        private System.Windows.Forms.Label lblAirGapFL;
        private System.Windows.Forms.Label lblAirGapFR;
        private System.Windows.Forms.Label lblAirGapRL;
        private System.Windows.Forms.Label lblAirGapRR;
        private System.Windows.Forms.GroupBox grpLwsFbm;
        private System.Windows.Forms.Button btnReadLws;
        private System.Windows.Forms.Button btnReadSwitch;
        private System.Windows.Forms.Button btnFbmStart;
        private System.Windows.Forms.Button btnFbmStop;
        private System.Windows.Forms.Button btnReadFbmSignal;
        private System.Windows.Forms.Button btnReadFbmForce;
        private System.Windows.Forms.Label lblFbmFL;
        private System.Windows.Forms.Label lblFbmFR;
        private System.Windows.Forms.Label lblFbmRL;
        private System.Windows.Forms.Label lblFbmRR;
        private System.Windows.Forms.GroupBox grpValveTest;
        private System.Windows.Forms.ComboBox cboValveWheel;
        private System.Windows.Forms.Button btnValveInc;
        private System.Windows.Forms.Button btnValveDec;
        private System.Windows.Forms.Button btnValveStop;
        private System.Windows.Forms.GroupBox grpAutoTest;
        private System.Windows.Forms.Button btnAutoTest;
        private System.Windows.Forms.Button btnStopAutoTest;
        private System.Windows.Forms.Label lblLogFile;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblTestStatus;
        private System.Windows.Forms.Label lblTestResult;
        private System.Windows.Forms.GroupBox grpLog;
        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.Button btnOpenLogFolder;
    }
}
