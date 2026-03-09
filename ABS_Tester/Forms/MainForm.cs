using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ABS_Tester.Communication;
using ABS_Tester.Protocol;
using ABS_Tester.Models;
using ABS_Tester.Utils;

namespace ABS_Tester.Forms
{
    public partial class MainForm : Form
    {
        #region Fields

        private IcsNeoDevice _neoDevice;
        private EBS5Protocol _protocol;
        private System.Windows.Forms.Timer _keepAliveTimer;
        private System.Windows.Forms.Timer _monitorTimer;
        private Logger _logger;
        private CancellationTokenSource _autoTestCts;
        private bool _isAutoTesting;

        #endregion

        #region Constructor

        public MainForm()
        {
            InitializeComponent();
            InitializeDevice();
            InitializeTimers();
        }

        #endregion

        #region Initialization

        private void InitializeDevice()
        {
            _neoDevice = new IcsNeoDevice();
            _neoDevice.LogMessage += OnLogMessage;
            _neoDevice.DataReceived += OnDataReceived;
            _protocol = new EBS5Protocol(_neoDevice);
            _logger = Logger.Instance;

            // Logger를 neoDevice에 연동 (TX/RX 자동 로깅)
            _neoDevice.Logger = _logger;
        }

        private void InitializeTimers()
        {
            // Keep-Alive Timer (3초마다 Tester Present 전송)
            _keepAliveTimer = new System.Windows.Forms.Timer();
            _keepAliveTimer.Interval = 3000;
            _keepAliveTimer.Tick += KeepAliveTimer_Tick;

            // Monitor Timer (실시간 데이터 갱신)
            _monitorTimer = new System.Windows.Forms.Timer();
            _monitorTimer.Interval = 500;
            _monitorTimer.Tick += MonitorTimer_Tick;
        }

        #endregion

        #region Event Handlers

        private void OnLogMessage(object sender, string message)
        {
            if (lstLog.InvokeRequired)
            {
                lstLog.Invoke(new Action(() => AddLog(message)));
            }
            else
            {
                AddLog(message);
            }
        }

        private void OnDataReceived(object sender, CanMessage message)
        {
            // 필요 시 추가 처리
        }

        private void KeepAliveTimer_Tick(object sender, EventArgs e)
        {
            if (_neoDevice.IsEcuConnected)
            {
                _protocol.SendTesterPresent();
            }
        }

        private void MonitorTimer_Tick(object sender, EventArgs e)
        {
            if (_neoDevice.IsEcuConnected && chkRealtime.Checked)
            {
                UpdateRealtimeData();
            }
        }

        #endregion

        #region UI Event Handlers

        private void btnTestDiagSession_Click(object sender, EventArgs e)
        {
            // DiagnosticSession만 테스트하는 버튼
            if (!_neoDevice.IsOpen)
            {
                AddLog("neoVI 장치 연결 중...");
                if (!_neoDevice.Open(500000))
                {
                    AddLog("neoVI 장치 연결 실패");
                    MessageBox.Show("neoVI 장치를 연결할 수 없습니다.", "연결 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                AddLog($"neoVI 장치 연결 성공: {_neoDevice.DeviceInfo}");
            }

            AddLog("StartDiagnosticSession 테스트...");
            bool result = _protocol.StartDiagnosticSession();

            if (result)
            {
                AddLog("StartDiagnosticSession 성공");
                btnTestDiagSession.BackColor = Color.LightGreen;
            }
            else
            {
                AddLog("StartDiagnosticSession 실패");
                btnTestDiagSession.BackColor = Color.LightCoral;
            }
        }

        private void btnEcuConnect_Click(object sender, EventArgs e)
        {
            if (!_neoDevice.IsOpen)
            {
                // neoVI 장치 열기 + CAN 통신 초기화 (ICS는 한 번에 처리)
                AddLog("neoVI 장치 연결 중...");

                if (!_neoDevice.Open(500000))
                {
                    AddLog("neoVI 장치 연결 실패");
                    MessageBox.Show("neoVI 장치를 연결할 수 없습니다.\nUSB 연결 상태를 확인하세요.",
                        "연결 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                AddLog("ECU 연결 시도...");

                // 로그 파일 시작 (연결 전에 시작해서 연결 과정도 기록)
                StartFileLogging();

                _logger?.LogStep("ECU Connect (Extended Session + Security Access)");

                // ECU 연결 (Diagnostic Session + Security Access)
                if (_protocol.Connect())
                {
                    btnEcuConnect.Text = "ECU 해제";
                    btnEcuConnect.BackColor = Color.LightGreen;
                    _keepAliveTimer.Start();
                    AddLog("ECU 연결 성공 (Security Access 완료)");

                    // ECU 정보 읽기
                    _logger?.LogStep("Read ECU Information");
                    ReadEcuInfo();
                }
                else
                {
                    _logger?.LogError("ECU Connection Failed - Security Access Error");
                    AddLog("ECU 연결 실패 - Security Access 오류");
                    MessageBox.Show("ECU 연결에 실패했습니다.\nSecurity Access 오류", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    StopFileLogging(false, "ECU Connection Failed");
                    _neoDevice.Close();
                }
            }
            else
            {
                _keepAliveTimer.Stop();
                _monitorTimer.Stop();
                _neoDevice.IsEcuConnected = false;
                _neoDevice.Close();
                btnEcuConnect.Text = "ECU 연결";
                btnEcuConnect.BackColor = SystemColors.Control;
                AddLog("ECU 연결 해제");

                // 로그 파일 종료
                StopFileLogging();
            }
        }

        private void btnReadEcuInfo_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;
            _logger?.LogStep("Read ECU Information");
            ReadEcuInfo();
        }

        private void btnReadDtc_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;

            _logger?.LogStep("Read DTC");
            AddLog("DTC 읽기...");
            var dtc = _protocol.ReadDtc();
            if (dtc != null)
            {
                txtDtc.Text = IcsNeoDevice.BytesToHex(dtc);
                AddLog($"DTC: {IcsNeoDevice.BytesToHex(dtc)}");
            }
            else
            {
                AddLog("DTC 읽기 실패");
            }
        }

        private void btnClearDtc_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;

            if (MessageBox.Show("DTC를 삭제하시겠습니까?", "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _logger?.LogStep("Clear DTC");
                AddLog("DTC 삭제 중...");
                if (_protocol.ClearDtc())
                {
                    txtDtc.Text = "";
                    AddLog("DTC 삭제 완료");
                }
                else
                {
                    _logger?.LogError("Clear DTC Failed");
                    AddLog("DTC 삭제 실패");
                }
            }
        }

        private void btnReadVoltage_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;

            _logger?.LogStep("Read Voltage");
            AddLog("전압 읽기...");
            var voltage = _protocol.ReadVoltage();
            lblVoltageUz.Text = $"{voltage.IgnitionVoltage:F2} V";
            lblVoltageUb.Text = $"{voltage.BatteryVoltage:F2} V";
            AddLog($"전압: UZ={voltage.IgnitionVoltage:F2}V, UB={voltage.BatteryVoltage:F2}V");
        }

        private void btnReadWss_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;

            _logger?.LogStep("Read Wheel Speed Sensor");
            AddLog("휠 속도 읽기...");
            var wss = _protocol.ReadWheelSpeed();
            lblWssFL.Text = $"{wss.FrontLeft:F1} km/h";
            lblWssFR.Text = $"{wss.FrontRight:F1} km/h";
            lblWssRL.Text = $"{wss.RearLeft:F1} km/h";
            lblWssRR.Text = $"{wss.RearRight:F1} km/h";
        }

        private void btnReadSteeringAngle_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;

            _logger?.LogStep("Read Steering Angle");
            AddLog("조향각 읽기...");
            double angle = _protocol.ReadSteeringAngle();
            lblSteeringAngle.Text = $"조향각: {angle:F1} °";
            AddLog($"조향각: {angle:F1}°");
        }

        // 램프 테스트 버튼들
        private void btnLampRed_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;
            _logger?.LogStep("Lamp Test: EBS RED");
            AddLog("EBS RED 램프 테스트...");
            if (_protocol.ActuatorEbsRed())
                AddLog("EBS RED 램프 ON");
            else
                AddLog("EBS RED 램프 테스트 실패");
        }

        private void btnLampAmber_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;
            _logger?.LogStep("Lamp Test: EBS AMBER");
            AddLog("EBS AMBER 램프 테스트...");
            if (_protocol.ActuatorEbsAmber())
                AddLog("EBS AMBER 램프 ON");
            else
                AddLog("EBS AMBER 램프 테스트 실패");
        }

        private void btnLampVdc_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;
            _logger?.LogStep("Lamp Test: VDC");
            AddLog("VDC 램프 테스트...");
            if (_protocol.ActuatorVdc())
                AddLog("VDC 램프 ON");
            else
                AddLog("VDC 램프 테스트 실패");
        }

        private void btnLampVdcFully_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;
            _logger?.LogStep("Lamp Test: VDC FULLY");
            AddLog("VDC FULLY 램프 테스트...");
            if (_protocol.ActuatorVdcFully())
                AddLog("VDC FULLY 램프 ON");
            else
                AddLog("VDC FULLY 램프 테스트 실패");
        }

        private void btnLampHillHolder_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;
            _logger?.LogStep("Lamp Test: HILLHOLDER");
            AddLog("HILLHOLDER 램프 테스트...");
            if (_protocol.ActuatorHillHolder())
                AddLog("HILLHOLDER 램프 ON");
            else
                AddLog("HILLHOLDER 램프 테스트 실패");
        }

        private void btnLampStop_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;
            _logger?.LogStep("Lamp Test: STOP");
            AddLog("램프 테스트 정지...");
            if (_protocol.ActuatorStop())
                AddLog("램프 테스트 정지");
            else
                AddLog("램프 테스트 정지 실패");
        }

        // 밸브 테스트 버튼들
        private void btnValveInc_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;
            int wheel = cboValveWheel.SelectedIndex;
            string[] wheelNames = { "FL", "FR", "RL", "RR" };
            _logger?.LogStep($"Valve Test: Increase ({wheelNames[wheel]})");
            AddLog($"밸브 증압 테스트 (Wheel {wheel})...");
            if (_protocol.ValveIncrease(wheel))
                AddLog("밸브 증압 명령 전송 완료");
            else
                AddLog("밸브 증압 명령 실패");
        }

        private void btnValveDec_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;
            int wheel = cboValveWheel.SelectedIndex;
            string[] wheelNames = { "FL", "FR", "RL", "RR" };
            _logger?.LogStep($"Valve Test: Decrease ({wheelNames[wheel]})");
            AddLog($"밸브 감압 테스트 (Wheel {wheel})...");
            if (_protocol.ValveDecrease(wheel))
                AddLog("밸브 감압 명령 전송 완료");
            else
                AddLog("밸브 감압 명령 실패");
        }

        private void btnValveStop_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;
            _logger?.LogStep("Valve Test: STOP");
            AddLog("밸브 테스트 정지...");
            if (_protocol.ValveStop())
                AddLog("밸브 테스트 정지");
            else
                AddLog("밸브 테스트 정지 실패");
        }

        private void btnSasSetting_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;

            if (MessageBox.Show("SAS 설정을 수행하시겠습니까?\n차량이 직진 상태인지 확인하세요.",
                "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _logger?.LogStep("SAS (Steering Angle Sensor) Setting");
                AddLog("SAS 설정 수행 중...");
                if (_protocol.PerformSasSetting())
                    AddLog("SAS 설정 완료");
                else
                {
                    _logger?.LogError("SAS Setting Failed");
                    AddLog("SAS 설정 실패");
                }
            }
        }

        // Air Gap 버튼들
        private void btnAirGapInitFront_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;
            _logger?.LogStep("Air Gap Init (Front)");
            AddLog("Air Gap 초기화 (Front)...");
            if (_protocol.InitializeAirGap(0))
                AddLog("Air Gap 초기화 (Front) 완료");
            else
                AddLog("Air Gap 초기화 (Front) 실패");
        }

        private void btnAirGapInitRear_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;
            _logger?.LogStep("Air Gap Init (Rear)");
            AddLog("Air Gap 초기화 (Rear)...");
            if (_protocol.InitializeAirGap(1))
                AddLog("Air Gap 초기화 (Rear) 완료");
            else
                AddLog("Air Gap 초기화 (Rear) 실패");
        }

        private void btnAirGapRead_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;
            _logger?.LogStep("Read Air Gap");
            AddLog("Air Gap 읽기...");
            var airGap = _protocol.ReadAirGap();
            lblAirGapFL.Text = $"FL: {airGap.FrontLeft:F2}";
            lblAirGapFR.Text = $"FR: {airGap.FrontRight:F2}";
            lblAirGapRL.Text = $"RL: {airGap.RearLeft:F2}";
            lblAirGapRR.Text = $"RR: {airGap.RearRight:F2}";
            AddLog($"Air Gap: FL={airGap.FrontLeft:F2}, FR={airGap.FrontRight:F2}, RL={airGap.RearLeft:F2}, RR={airGap.RearRight:F2}");
        }

        // LWS/Switch 버튼들
        private void btnReadLws_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;
            _logger?.LogStep("Read LWS Value");
            AddLog("LWS 값 읽기...");
            var lws = _protocol.ReadLwsValue();
            AddLog($"LWS Raw: {lws.RawData}");
        }

        private void btnReadSwitch_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;
            _logger?.LogStep("Read Switch Status");
            AddLog("Switch 상태 읽기...");
            var sw = _protocol.ReadSwitchStatus();
            AddLog($"Switch Raw: {sw.RawData}");
        }

        // FBM 버튼들
        private void btnFbmStart_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;
            _logger?.LogStep("FBM Backup Start");
            AddLog("FBM 백업 시작...");
            if (_protocol.StartFbmBackup())
                AddLog("FBM 백업 시작 완료");
            else
                AddLog("FBM 백업 시작 실패");
        }

        private void btnFbmStop_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;
            _logger?.LogStep("FBM Backup Stop");
            AddLog("FBM 백업 정지...");
            if (_protocol.StopFbmBackup())
                AddLog("FBM 백업 정지 완료");
            else
                AddLog("FBM 백업 정지 실패");
        }

        private void btnReadFbmSignal_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;
            _logger?.LogStep("Read FBM Signal");
            AddLog("FBM Signal 읽기...");
            var signal = _protocol.ReadFbmSignal();
            AddLog($"FBM Signal: V1={signal.Signal1Voltage:F3}V, V2={signal.Signal2Voltage:F3}V, SW={signal.SwitchInputs:X2}");
        }

        private void btnReadFbmForce_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;
            _logger?.LogStep("Read FBM Brake Force");
            AddLog("FBM 브레이크력 읽기...");
            var fbm = _protocol.ReadFbmBrakeForce();
            lblFbmFL.Text = $"FL: {fbm.FrontLeft:F2}";
            lblFbmFR.Text = $"FR: {fbm.FrontRight:F2}";
            lblFbmRL.Text = $"RL: {fbm.RearLeft:F2}";
            lblFbmRR.Text = $"RR: {fbm.RearRight:F2}";
            AddLog($"FBM Force: FL={fbm.FrontLeft:F2}, FR={fbm.FrontRight:F2}, RL={fbm.RearLeft:F2}, RR={fbm.RearRight:F2} bar");
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            lstLog.Items.Clear();
        }

        private void btnOpenLogFolder_Click(object sender, EventArgs e)
        {
            string logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }
            Process.Start("explorer.exe", logFolder);
        }

        private async void btnAutoTest_Click(object sender, EventArgs e)
        {
            if (!CheckConnection()) return;

            if (_isAutoTesting)
            {
                return;
            }

            _isAutoTesting = true;
            _autoTestCts = new CancellationTokenSource();

            // UI 상태 변경
            btnAutoTest.Enabled = false;
            btnStopAutoTest.Enabled = true;
            progressBar.Value = 0;
            lblTestResult.Text = "";
            lblTestResult.ForeColor = Color.Black;

            try
            {
                await RunAutoTestAsync(_autoTestCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("자동 테스트가 중지되었습니다.");
                lblTestStatus.Text = "테스트 중지됨";
            }
            catch (Exception ex)
            {
                AddLog($"자동 테스트 오류: {ex.Message}");
                lblTestStatus.Text = "오류 발생";
            }
            finally
            {
                _isAutoTesting = false;
                btnAutoTest.Enabled = true;
                btnStopAutoTest.Enabled = false;
            }
        }

        private void btnStopAutoTest_Click(object sender, EventArgs e)
        {
            _autoTestCts?.Cancel();
            lblTestStatus.Text = "중지 요청...";
        }

        private async Task RunAutoTestAsync(CancellationToken token)
        {
            int totalSteps = 10;
            int currentStep = 0;
            int passCount = 0;
            int failCount = 0;

            void UpdateProgress(string status)
            {
                currentStep++;
                int percent = (currentStep * 100) / totalSteps;
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        progressBar.Value = percent;
                        lblTestStatus.Text = status;
                    }));
                }
                else
                {
                    progressBar.Value = percent;
                    lblTestStatus.Text = status;
                }
            }

            void LogResult(string testName, bool passed, string details = null)
            {
                if (passed) passCount++; else failCount++;
                _logger?.LogTestResult(testName, passed, details);
            }

            _logger?.LogSeparator("자동 테스트 시작");
            AddLog("========== 자동 테스트 시작 ==========");

            // 1. ECU 정보 읽기
            token.ThrowIfCancellationRequested();
            UpdateProgress("ECU 정보 읽기...");
            AddLog("[1/10] ECU 정보 읽기");
            var info = _protocol.ReadEcuInfo();
            bool ecuInfoOk = !string.IsNullOrEmpty(info.HardwareNumber);
            LogResult("ECU 정보 읽기", ecuInfoOk, info.HardwareNumber);
            UpdateEcuInfoUI(info);
            await Task.Delay(500, token);

            // 2. 전압 읽기
            token.ThrowIfCancellationRequested();
            UpdateProgress("전압 읽기...");
            AddLog("[2/10] 전압 읽기");
            var voltage = _protocol.ReadVoltage();
            bool voltageOk = voltage != null && voltage.IgnitionVoltage > 10 && voltage.BatteryVoltage > 10;
            LogResult("전압 읽기", voltageOk, voltage != null ? $"UZ={voltage.IgnitionVoltage:F2}V, UB={voltage.BatteryVoltage:F2}V" : "읽기 실패");
            if (voltage != null) UpdateVoltageUI(voltage);
            await Task.Delay(500, token);

            // 3. WSS 읽기
            token.ThrowIfCancellationRequested();
            UpdateProgress("휠 속도 읽기...");
            AddLog("[3/10] 휠 속도 센서 읽기");
            var wss = _protocol.ReadWheelSpeed();
            bool wssOk = wss != null;
            LogResult("WSS 읽기", wssOk, wss != null ? $"FL={wss.FrontLeft:F1}, FR={wss.FrontRight:F1}, RL={wss.RearLeft:F1}, RR={wss.RearRight:F1}" : "읽기 실패");
            if (wss != null) UpdateWssUI(wss);
            await Task.Delay(500, token);

            // 4. DTC 읽기
            token.ThrowIfCancellationRequested();
            UpdateProgress("DTC 읽기...");
            AddLog("[4/10] DTC 읽기");
            var dtc = _protocol.ReadDtc();
            bool dtcOk = dtc != null;
            string dtcStr = dtcOk ? IcsNeoDevice.BytesToHex(dtc) : "읽기 실패";
            LogResult("DTC 읽기", dtcOk, dtcStr);
            if (dtcOk)
            {
                Invoke(new Action(() => txtDtc.Text = dtcStr));
            }
            await Task.Delay(500, token);

            // 5. EBS RED 램프 테스트
            token.ThrowIfCancellationRequested();
            UpdateProgress("EBS RED 램프 테스트...");
            AddLog("[5/10] EBS RED 램프 테스트");
            bool redOk = _protocol.ActuatorEbsRed();
            LogResult("EBS RED 램프", redOk);
            await Task.Delay(1500, token);
            _protocol.ActuatorStop();

            // 6. EBS AMBER 램프 테스트
            token.ThrowIfCancellationRequested();
            UpdateProgress("EBS AMBER 램프 테스트...");
            AddLog("[6/10] EBS AMBER 램프 테스트");
            bool amberOk = _protocol.ActuatorEbsAmber();
            LogResult("EBS AMBER 램프", amberOk);
            await Task.Delay(1500, token);
            _protocol.ActuatorStop();

            // 7. VDC 램프 테스트
            token.ThrowIfCancellationRequested();
            UpdateProgress("VDC 램프 테스트...");
            AddLog("[7/10] VDC 램프 테스트");
            bool vdcOk = _protocol.ActuatorVdc();
            LogResult("VDC 램프", vdcOk);
            await Task.Delay(1500, token);
            _protocol.ActuatorStop();

            // 8-10. 밸브 테스트 (FL만 예시)
            for (int wheel = 0; wheel < 3 && !token.IsCancellationRequested; wheel++)
            {
                string[] wheelNames = { "FL", "FR", "RL" };
                int stepNum = 8 + wheel;

                token.ThrowIfCancellationRequested();
                UpdateProgress($"밸브 테스트 ({wheelNames[wheel]})...");
                AddLog($"[{stepNum}/10] {wheelNames[wheel]} 밸브 증압 테스트");

                bool valveOk = _protocol.ValveIncrease(wheel);
                await Task.Delay(1000, token);
                _protocol.ValveStop();
                LogResult($"{wheelNames[wheel]} 밸브 테스트", valveOk);
                await Task.Delay(500, token);
            }

            // 완료
            UpdateProgress("테스트 완료");
            _logger?.LogSeparator("자동 테스트 완료");
            AddLog("========== 자동 테스트 완료 ==========");
            AddLog($"결과: PASS={passCount}, FAIL={failCount}");

            Invoke(new Action(() =>
            {
                lblTestResult.Text = $"PASS: {passCount} / FAIL: {failCount}";
                lblTestResult.ForeColor = failCount == 0 ? Color.Green : Color.Red;
            }));
        }

        private void UpdateEcuInfoUI(EcuInfo info)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateEcuInfoUI(info)));
                return;
            }
            txtHardwareNo.Text = info.HardwareNumber;
            txtSoftwareNo.Text = info.SoftwareNumber;
            txtSerialNo.Text = info.SerialNumber;
            txtMfgDate.Text = info.ManufacturingDate;
            txtConfig.Text = info.Configuration;
        }

        private void UpdateVoltageUI(VoltageData voltage)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateVoltageUI(voltage)));
                return;
            }
            lblVoltageUz.Text = $"{voltage.IgnitionVoltage:F2} V";
            lblVoltageUb.Text = $"{voltage.BatteryVoltage:F2} V";
        }

        private void UpdateWssUI(WheelSpeedData wss)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateWssUI(wss)));
                return;
            }
            lblWssFL.Text = $"{wss.FrontLeft:F1} km/h";
            lblWssFR.Text = $"{wss.FrontRight:F1} km/h";
            lblWssRL.Text = $"{wss.RearLeft:F1} km/h";
            lblWssRR.Text = $"{wss.RearRight:F1} km/h";
        }

        private void chkRealtime_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRealtime.Checked && _neoDevice.IsEcuConnected)
            {
                _monitorTimer.Start();
                AddLog("실시간 모니터링 시작");
            }
            else
            {
                _monitorTimer.Stop();
                if (!chkRealtime.Checked)
                    AddLog("실시간 모니터링 중지");
            }
        }

        #endregion

        #region Helper Methods

        private bool CheckConnection()
        {
            if (!_neoDevice.IsOpen || !_neoDevice.IsEcuConnected)
            {
                MessageBox.Show("ECU가 연결되지 않았습니다.\n먼저 ECU 연결 버튼을 클릭하세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void AddLog(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string logLine = $"[{timestamp}] {message}";

            if (lstLog.InvokeRequired)
            {
                lstLog.Invoke(new Action(() =>
                {
                    lstLog.Items.Add(logLine);
                    lstLog.TopIndex = lstLog.Items.Count - 1;
                }));
            }
            else
            {
                lstLog.Items.Add(logLine);
                lstLog.TopIndex = lstLog.Items.Count - 1;
            }

            // 파일 로깅 중이면 파일에도 기록
            if (_logger != null && _logger.IsLogging)
            {
                _logger.Log(message);
            }
        }

        private void StartFileLogging()
        {
            try
            {
                if (!_logger.IsLogging)
                {
                    // 패턴 기반 세션 시작 (앱 시작 시 전체 세션)
                    _logger.StartSession("ABS_EBS_Test", EcuType.EBS);
                    lblLogFile.Text = Path.GetFileName(_logger.LogFilePath);
                    lblLogFile.ForeColor = Color.Blue;
                    AddLog($"로그 파일 저장 시작: {_logger.LogFilePath}");
                }
            }
            catch (Exception ex)
            {
                AddLog($"로그 파일 생성 실패: {ex.Message}");
            }
        }

        private void StopFileLogging(bool success = true, string errorMessage = "")
        {
            if (_logger != null && _logger.IsLogging)
            {
                // 패턴 기반 세션 종료
                _logger.EndSession(success, errorMessage);
                _logger.StopLogging();
                AddLog("로그 파일 저장 완료");
                lblLogFile.Text = "ECU 연결 시 로그 자동 저장";
                lblLogFile.ForeColor = Color.Gray;
            }
        }

        private void ReadEcuInfo()
        {
            AddLog("ECU 정보 읽기...");
            var info = _protocol.ReadEcuInfo();

            txtHardwareNo.Text = info.HardwareNumber;
            txtSoftwareNo.Text = info.SoftwareNumber;
            txtSerialNo.Text = info.SerialNumber;
            txtMfgDate.Text = info.ManufacturingDate;
            txtConfig.Text = info.Configuration;

            AddLog($"HW: {info.HardwareNumber}, SW: {info.SoftwareNumber}");
        }

        private void UpdateRealtimeData()
        {
            try
            {
                var wss = _protocol.ReadWheelSpeed();
                lblWssFL.Text = $"{wss.FrontLeft:F1} km/h";
                lblWssFR.Text = $"{wss.FrontRight:F1} km/h";
                lblWssRL.Text = $"{wss.RearLeft:F1} km/h";
                lblWssRR.Text = $"{wss.RearRight:F1} km/h";
            }
            catch { }
        }

        #endregion

        #region Form Events

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // 자동 테스트 취소
                _autoTestCts?.Cancel();

                // 타이머 정지 및 해제
                if (_keepAliveTimer != null)
                {
                    _keepAliveTimer.Stop();
                    _keepAliveTimer.Dispose();
                    _keepAliveTimer = null;
                }

                if (_monitorTimer != null)
                {
                    _monitorTimer.Stop();
                    _monitorTimer.Dispose();
                    _monitorTimer = null;
                }

                // 타이머 이벤트 완료 대기
                System.Threading.Thread.Sleep(100);

                // 장치 닫기
                if (_neoDevice != null)
                {
                    _neoDevice.Close();
                    _neoDevice.Dispose();
                    _neoDevice = null;
                }

                // 로거 종료
                _logger?.Dispose();
            }
            catch
            {
                // 종료 시 예외 무시
            }
        }

        #endregion
    }
}
