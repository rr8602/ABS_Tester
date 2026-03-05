using System;
using System.Text;
using System.Threading;
using ABS_Tester.Communication;
using ABS_Tester.Models;

namespace ABS_Tester.Protocol
{
    /// <summary>
    /// KNORR EBS5x ECU 통신 프로토콜
    /// KI1A-ABST mod_EBS5.bas 기반
    /// </summary>
    public class EBS5Protocol
    {
        #region Fields

        private readonly VrtDevice _device;
        private const int DefaultTimeout = 3000;
        private const int RetryCount = 3;

        #endregion

        #region Constructor

        public EBS5Protocol(VrtDevice device)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));

            // KNORR EBS5x 기본 CAN ID 설정
            _device.TxId = 0x18DA0BF1;
            _device.RxId = 0x18DAF10B;
        }

        #endregion

        #region Connection & Security

        /// <summary>
        /// ECU 연결 및 Security Access 수행
        /// </summary>
        public bool Connect()
        {
            // 1. Diagnostic Session Control (Extended Session)
            if (!StartDiagnosticSession())
                return false;

            Thread.Sleep(100);

            // 2. Security Access
            if (!PerformSecurityAccess())
                return false;

            _device.IsEcuConnected = true;
            return true;
        }

        /// <summary>
        /// Extended Diagnostic Session 시작
        /// </summary>
        public bool StartDiagnosticSession()
        {
            // 02 10 03 00 00 00 00 00
            byte[] request = { 0x02, UdsService.DiagnosticSessionControl, UdsService.ExtendedDiagnosticSession,
                              0x00, 0x00, 0x00, 0x00, 0x00 };

            for (int i = 0; i < RetryCount; i++)
            {
                if (_device.SendData(request))
                {
                    // 긍정 응답 확인은 VrtDevice.LastCommandSuccess로
                    if (_device.LastCommandSuccess)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Security Access 수행 (Level 3)
        /// </summary>
        public bool PerformSecurityAccess()
        {
            // 1. Request Seed (02 27 03)
            byte[] seedRequest = { 0x02, UdsService.SecurityAccess, UdsService.RequestSeedLevel3,
                                   0x00, 0x00, 0x00, 0x00, 0x00 };

            for (int i = 0; i < RetryCount; i++)
            {
                if (!_device.SendData(seedRequest))
                    continue;

                var response = _device.LastReceivedData;
                // 응답 구조: [SubFunc=03][Seed1][Seed2][Seed3][Seed4] (PCI, SID는 VrtDevice에서 제거됨)
                if (response == null || response.Length < 5)
                    continue;

                // 긍정 응답 확인은 VrtDevice.LastCommandSuccess로
                if (!_device.LastCommandSuccess)
                    continue;

                // SubFunction 확인
                if (response[0] != UdsService.RequestSeedLevel3)
                    continue;

                // Seed 추출 (인덱스 1~4)
                byte[] seed = { response[1], response[2], response[3], response[4] };

                // Key 계산
                byte[] key = SeedKeyAlgorithm.CalculateKey(seed);

                Thread.Sleep(50);

                // 2. Send Key (06 27 04 [Key])
                byte[] keyRequest = { 0x06, UdsService.SecurityAccess, UdsService.SendKeyLevel3,
                                     key[0], key[1], key[2], key[3], 0x00 };

                if (_device.SendData(keyRequest))
                {
                    // 긍정 응답 확인
                    if (_device.LastCommandSuccess)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Tester Present 전송 (Keep-Alive)
        /// </summary>
        public bool SendTesterPresent()
        {
            byte[] request = { 0x02, UdsService.TesterPresent, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            return _device.SendData(request, 1000);
        }

        #endregion

        #region ECU Information

        /// <summary>
        /// ECU 정보 읽기
        /// </summary>
        public EcuInfo ReadEcuInfo()
        {
            var info = new EcuInfo();

            // Hardware Number (F192)
            info.HardwareNumber = ReadDataByIdentifier(UdsService.DID_SystemSupplierHardwareNumber);

            // Software Number (F188)
            info.SoftwareNumber = ReadDataByIdentifier(UdsService.DID_EcuSoftwareNumber);

            // Serial Number (F18C)
            info.SerialNumber = ReadDataByIdentifier(UdsService.DID_EcuSerialNumber);

            // Manufacturing Date (F18B)
            info.ManufacturingDate = ReadDataByIdentifier(UdsService.DID_EcuManufacturingDate);

            // Configuration (F1F2)
            info.Configuration = ReadDataByIdentifier(UdsService.DID_EcuConfig);

            return info;
        }

        /// <summary>
        /// DID로 데이터 읽기
        /// </summary>
        public string ReadDataByIdentifier(ushort did)
        {
            byte[] request = { 0x03, UdsService.ReadDataByIdentifier,
                              (byte)(did >> 8), (byte)(did & 0xFF),
                              0x00, 0x00, 0x00, 0x00 };

            for (int i = 0; i < RetryCount; i++)
            {
                if (_device.SendData(request))
                {
                    var response = _device.LastReceivedData;
                    if (response != null && response.Length > 2)
                    {
                        // 응답 구조: [DID_H][DID_L][DATA...] (PCI, SID는 VrtDevice에서 제거됨)
                        // 응답 데이터를 문자열로 변환 (인덱스 2부터 시작)
                        StringBuilder sb = new StringBuilder();
                        for (int j = 2; j < response.Length && response[j] != 0xFF; j++)
                        {
                            if (response[j] >= 0x20 && response[j] <= 0x7E)
                                sb.Append((char)response[j]);
                        }
                        return sb.ToString();
                    }
                }
            }
            return string.Empty;
        }

        #endregion

        #region DTC (Diagnostic Trouble Code)

        /// <summary>
        /// DTC 읽기
        /// </summary>
        public byte[] ReadDtc()
        {
            // 03 19 02 08 (Read DTC by Status Mask)
            byte[] request = { 0x03, UdsService.ReadDtcInformation, 0x02, 0x08,
                              0x00, 0x00, 0x00, 0x00 };

            if (_device.SendData(request))
            {
                return _device.LastReceivedData;
            }
            return null;
        }

        /// <summary>
        /// DTC 삭제
        /// </summary>
        public bool ClearDtc()
        {
            // 04 14 FF FF FF
            byte[] request = { 0x04, UdsService.ClearDiagnosticInformation,
                              0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00 };

            for (int i = 0; i < RetryCount; i++)
            {
                if (_device.SendData(request))
                {
                    // 긍정 응답 확인은 VrtDevice.LastCommandSuccess로
                    if (_device.LastCommandSuccess)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        #region Voltage & Sensors

        /// <summary>
        /// 전압 측정
        /// </summary>
        public VoltageData ReadVoltage()
        {
            var voltage = new VoltageData();

            // 03 22 FD 08
            byte[] request = { 0x03, UdsService.ReadDataByIdentifier,
                              (byte)(UdsService.DID_Voltage >> 8), (byte)(UdsService.DID_Voltage & 0xFF),
                              0x00, 0x00, 0x00, 0x00 };

            if (_device.SendData(request))
            {
                var response = _device.LastReceivedData;
                if (response != null && response.Length >= 6)
                {
                    // 응답 구조: [DID_H][DID_L][DATA...] (PCI, SID는 VrtDevice에서 제거됨)
                    // 응답 파싱 (1 bit = 1/1024 V)
                    int uzRaw = (response[2] << 8) | response[3];
                    int ubRaw = (response[4] << 8) | response[5];

                    voltage.IgnitionVoltage = uzRaw / 1024.0;
                    voltage.BatteryVoltage = ubRaw / 1024.0;
                }
            }

            return voltage;
        }

        /// <summary>
        /// 휠 속도 센서 읽기
        /// </summary>
        public WheelSpeedData ReadWheelSpeed()
        {
            var wss = new WheelSpeedData();

            // 03 22 FD 00
            byte[] request = { 0x03, UdsService.ReadDataByIdentifier,
                              (byte)(UdsService.DID_WSS >> 8), (byte)(UdsService.DID_WSS & 0xFF),
                              0x00, 0x00, 0x00, 0x00 };

            if (_device.SendData(request))
            {
                var response = _device.LastReceivedData;
                if (response != null && response.Length >= 10)
                {
                    // 응답 구조: [DID_H][DID_L][DATA...] (PCI, SID는 VrtDevice에서 제거됨)
                    // 응답 파싱 (1 bit = 1/512 * 3.6 km/h)
                    double factor = (1.0 / 512.0) * 3.6;

                    int flRaw = (response[2] << 8) | response[3];
                    int frRaw = (response[4] << 8) | response[5];
                    int rlRaw = (response[6] << 8) | response[7];
                    int rrRaw = (response[8] << 8) | response[9];

                    wss.FrontLeft = flRaw * factor;
                    wss.FrontRight = frRaw * factor;
                    wss.RearLeft = rlRaw * factor;
                    wss.RearRight = rrRaw * factor;
                }
            }

            return wss;
        }

        /// <summary>
        /// 조향각 읽기
        /// </summary>
        public double ReadSteeringAngle()
        {
            // 03 22 FD 02
            byte[] request = { 0x03, UdsService.ReadDataByIdentifier,
                              (byte)(UdsService.DID_SteeringAngle >> 8), (byte)(UdsService.DID_SteeringAngle & 0xFF),
                              0x00, 0x00, 0x00, 0x00 };

            if (_device.SendData(request))
            {
                var response = _device.LastReceivedData;
                if (response != null && response.Length >= 4)
                {
                    // 응답 구조: [DID_H][DID_L][DATA...] (PCI, SID는 VrtDevice에서 제거됨)
                    short raw = (short)((response[2] << 8) | response[3]);
                    return raw / 512.0;  // 1 bit = 1/512 degree
                }
            }

            return 0;
        }

        #endregion

        #region Lamp Actuator Test

        /// <summary>
        /// EBS RED 램프 테스트
        /// </summary>
        public bool ActuatorEbsRed()
        {
            // 07 2F FD F7 03 01 00 00
            byte[] request = { 0x07, UdsService.InputOutputControlByIdentifier,
                              (byte)(UdsService.DID_LampControl >> 8), (byte)(UdsService.DID_LampControl & 0xFF),
                              UdsService.ShortTermAdjustment, 0x01, 0x00, 0x00 };
            return SendActuatorCommand(request);
        }

        /// <summary>
        /// EBS AMBER (YELLOW) 램프 테스트
        /// </summary>
        public bool ActuatorEbsAmber()
        {
            // 07 2F FD F7 03 10 00 00
            byte[] request = { 0x07, UdsService.InputOutputControlByIdentifier,
                              (byte)(UdsService.DID_LampControl >> 8), (byte)(UdsService.DID_LampControl & 0xFF),
                              UdsService.ShortTermAdjustment, 0x10, 0x00, 0x00 };
            return SendActuatorCommand(request);
        }

        /// <summary>
        /// VDC 램프 테스트
        /// </summary>
        public bool ActuatorVdc()
        {
            // 07 2F FD F7 03 40 00 00
            byte[] request = { 0x07, UdsService.InputOutputControlByIdentifier,
                              (byte)(UdsService.DID_LampControl >> 8), (byte)(UdsService.DID_LampControl & 0xFF),
                              UdsService.ShortTermAdjustment, 0x40, 0x00, 0x00 };
            return SendActuatorCommand(request);
        }

        /// <summary>
        /// VDC Fully 램프 테스트
        /// </summary>
        public bool ActuatorVdcFully()
        {
            // 07 2F FD F7 03 00 01 00
            byte[] request = { 0x07, UdsService.InputOutputControlByIdentifier,
                              (byte)(UdsService.DID_LampControl >> 8), (byte)(UdsService.DID_LampControl & 0xFF),
                              UdsService.ShortTermAdjustment, 0x00, 0x01, 0x00 };
            return SendActuatorCommand(request);
        }

        /// <summary>
        /// Hill Holder 램프 테스트
        /// </summary>
        public bool ActuatorHillHolder()
        {
            // 07 2F FD F7 03 00 00 01
            byte[] request = { 0x07, UdsService.InputOutputControlByIdentifier,
                              (byte)(UdsService.DID_LampControl >> 8), (byte)(UdsService.DID_LampControl & 0xFF),
                              UdsService.ShortTermAdjustment, 0x00, 0x00, 0x01 };
            return SendActuatorCommand(request);
        }

        /// <summary>
        /// 램프 테스트 정지
        /// </summary>
        public bool ActuatorStop()
        {
            // 04 2F FD F7 00
            byte[] request = { 0x04, UdsService.InputOutputControlByIdentifier,
                              (byte)(UdsService.DID_LampControl >> 8), (byte)(UdsService.DID_LampControl & 0xFF),
                              UdsService.ReturnControlToECU, 0x00, 0x00, 0x00 };
            return SendActuatorCommand(request);
        }

        private bool SendActuatorCommand(byte[] request)
        {
            for (int i = 0; i < RetryCount; i++)
            {
                if (_device.SendData(request))
                {
                    // 긍정 응답 확인은 VrtDevice.LastCommandSuccess로
                    if (_device.LastCommandSuccess)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        #region Valve Test (VB 코드 기준 파라미터)

        /// <summary>
        /// 밸브 증압 테스트
        /// </summary>
        /// <param name="wheel">0=FL, 1=FR, 2=RL, 3=RR</param>
        public bool ValveIncrease(int wheel)
        {
            // VB 코드 기준: 18 2F FD F5 03 [위치별 04] ...
            byte[] data = new byte[25];
            data[0] = 0x18;  // Length
            data[1] = UdsService.InputOutputControlByIdentifier;
            data[2] = (byte)(UdsService.DID_ValveControl >> 8);
            data[3] = (byte)(UdsService.DID_ValveControl & 0xFF);
            data[4] = UdsService.ShortTermAdjustment;  // 03

            // 휠별 위치에 0x04 설정
            switch (wheel)
            {
                case 0: data[5] = 0x04; break;   // FL
                case 1: data[7] = 0x04; break;   // FR
                case 2: data[9] = 0x04; break;   // RL
                case 3: data[11] = 0x04; break;  // RR
            }

            return SendActuatorCommand(data);
        }

        /// <summary>
        /// 밸브 감압 테스트
        /// </summary>
        /// <param name="wheel">0=FL, 1=FR, 2=RL, 3=RR</param>
        public bool ValveDecrease(int wheel)
        {
            // VB 코드 기준: 18 2F FD F5 03 [위치별 0A] ...
            byte[] data = new byte[25];
            data[0] = 0x18;  // Length
            data[1] = UdsService.InputOutputControlByIdentifier;
            data[2] = (byte)(UdsService.DID_ValveControl >> 8);
            data[3] = (byte)(UdsService.DID_ValveControl & 0xFF);
            data[4] = UdsService.ShortTermAdjustment;  // 03

            // 휠별 위치에 0x0A 설정
            switch (wheel)
            {
                case 0: data[6] = 0x0A; break;   // FL
                case 1: data[8] = 0x0A; break;   // FR
                case 2: data[10] = 0x0A; break;  // RL
                case 3: data[12] = 0x0A; break;  // RR
            }

            return SendActuatorCommand(data);
        }

        /// <summary>
        /// 밸브 테스트 정지
        /// </summary>
        public bool ValveStop()
        {
            // 04 2F FD F5 00
            byte[] request = { 0x04, UdsService.InputOutputControlByIdentifier,
                              (byte)(UdsService.DID_ValveControl >> 8), (byte)(UdsService.DID_ValveControl & 0xFF),
                              UdsService.ReturnControlToECU, 0x00, 0x00, 0x00 };
            return SendActuatorCommand(request);
        }

        #endregion

        #region Air Gap

        /// <summary>
        /// Air Gap 초기화 (휠 1,2 또는 3,4)
        /// VB: ECU1AirGapWx
        /// </summary>
        /// <param name="axle">0=Front(Wheel 1,2), 1=Rear(Wheel 3,4)</param>
        public bool InitializeAirGap(int axle)
        {
            // 1. 먼저 현재 Air Gap 데이터 읽기
            // 03 22 FD F3
            byte[] readRequest = { 0x03, UdsService.ReadDataByIdentifier,
                                  (byte)(UdsService.DID_AirGap >> 8), (byte)(UdsService.DID_AirGap & 0xFF),
                                  0x00, 0x00, 0x00, 0x00 };

            byte[] currentData = null;
            for (int i = 0; i < RetryCount; i++)
            {
                if (_device.SendData(readRequest))
                {
                    var response = _device.LastReceivedData;
                    if (response != null && response.Length >= 24)
                    {
                        currentData = response;
                        break;
                    }
                }
            }

            if (currentData == null)
                return false;

            // 2. Air Gap 초기화 명령 구성
            // 18 2F FD F3 03 [CH_0] [CH_1]
            // axle=0: CH_0 = "00 8E 00 8E" + 기존데이터[19:30], CH_1 = 기존데이터[30:끝]
            // axle=1: CH_0 = 기존데이터[7:18] + "00 8E 00 8E", CH_1 = 기존데이터[30:끝]
            byte[] initRequest = new byte[25];
            initRequest[0] = 0x18;  // Length (24 bytes)
            initRequest[1] = UdsService.InputOutputControlByIdentifier;
            initRequest[2] = (byte)(UdsService.DID_AirGap >> 8);
            initRequest[3] = (byte)(UdsService.DID_AirGap & 0xFF);
            initRequest[4] = UdsService.ShortTermAdjustment;  // 03

            if (axle == 0)  // Front (Wheel 1, 2)
            {
                // 00 8E 00 8E (초기화 값) + 기존 데이터
                initRequest[5] = 0x00; initRequest[6] = 0x8E;
                initRequest[7] = 0x00; initRequest[8] = 0x8E;
                // 기존 데이터 복사 (rear wheel data)
                for (int j = 0; j < 16 && (9 + j) < 25 && (6 + j) < currentData.Length; j++)
                {
                    initRequest[9 + j] = currentData[6 + j];
                }
            }
            else  // Rear (Wheel 3, 4)
            {
                // 기존 데이터 + 00 8E 00 8E (초기화 값)
                for (int j = 0; j < 4 && (2 + j) < currentData.Length; j++)
                {
                    initRequest[5 + j] = currentData[2 + j];
                }
                initRequest[9] = 0x00; initRequest[10] = 0x8E;
                initRequest[11] = 0x00; initRequest[12] = 0x8E;
                // 나머지 기존 데이터 복사
                for (int j = 0; j < 12 && (13 + j) < 25 && (10 + j) < currentData.Length; j++)
                {
                    initRequest[13 + j] = currentData[10 + j];
                }
            }

            return SendActuatorCommand(initRequest);
        }

        /// <summary>
        /// Air Gap 읽기
        /// VB: ECU1AirGapRx
        /// </summary>
        public AirGapData ReadAirGap()
        {
            var airGap = new AirGapData();

            // 03 22 FD F3
            byte[] request = { 0x03, UdsService.ReadDataByIdentifier,
                              (byte)(UdsService.DID_AirGap >> 8), (byte)(UdsService.DID_AirGap & 0xFF),
                              0x00, 0x00, 0x00, 0x00 };

            for (int i = 0; i < RetryCount; i++)
            {
                if (_device.SendData(request))
                {
                    var response = _device.LastReceivedData;
                    if (response != null && response.Length >= 10)
                    {
                        // 응답 구조: [DID_H][DID_L][DATA...] (PCI, SID는 VrtDevice에서 제거됨)
                        // 응답 파싱 (1 bit = 1/512 * 3.6 km/h)
                        double factor = (1.0 / 512.0) * 3.6;

                        int flRaw = (response[2] << 8) | response[3];
                        int frRaw = (response[4] << 8) | response[5];
                        int rlRaw = (response[6] << 8) | response[7];
                        int rrRaw = (response[8] << 8) | response[9];

                        airGap.FrontLeft = flRaw * factor;
                        airGap.FrontRight = frRaw * factor;
                        airGap.RearLeft = rlRaw * factor;
                        airGap.RearRight = rrRaw * factor;
                        break;
                    }
                }
            }

            return airGap;
        }

        #endregion

        #region LWS & Switch

        /// <summary>
        /// LWS (Lining Wear Sensor) 값 읽기
        /// VB: ECU1LWSValue
        /// 브레이크 라이닝 잔량 및 마모 센서 전압
        /// </summary>
        public LwsData ReadLwsValue()
        {
            var lws = new LwsData();

            // 03 22 FD 01
            byte[] request = { 0x03, UdsService.ReadDataByIdentifier,
                              (byte)(UdsService.DID_LWS >> 8), (byte)(UdsService.DID_LWS & 0xFF),
                              0x00, 0x00, 0x00, 0x00 };

            if (_device.SendData(request))
            {
                var response = _device.LastReceivedData;
                if (response != null && response.Length >= 4)
                {
                    // 원시 데이터 저장 (VB와 동일하게 전체 응답 저장)
                    lws.RawData = BitConverter.ToString(response).Replace("-", " ");

                    // 응답 구조: [DID_H][DID_L][DATA...] (PCI, SID는 VrtDevice에서 제거됨)
                    // 브레이크 라이닝 잔량: X1~X10 (1 bit = 0.4%)
                    // 마모 센서 전압: X11~X20 (1 bit = 0.0196V)
                    if (response.Length >= 12)
                    {
                        // 휠별 라이닝 잔량 (첫 10바이트, 인덱스 2부터 시작)
                        for (int j = 0; j < 10 && (2 + j) < response.Length; j++)
                        {
                            lws.BrakeLiningRemaining[j] = response[2 + j] * 0.4;
                        }
                        // 휠별 마모 센서 전압 (다음 10바이트)
                        for (int j = 0; j < 10 && (12 + j) < response.Length; j++)
                        {
                            lws.WearSensorVoltage[j] = response[12 + j] * 0.0196;
                        }
                    }
                }
            }

            return lws;
        }

        /// <summary>
        /// 스위치 상태 읽기 (HILLHOLDER, BLENDING 등)
        /// VB: ECU1__Switch
        /// </summary>
        public SwitchData ReadSwitchStatus()
        {
            var switchData = new SwitchData();

            // 03 22 FD 07
            byte[] request = { 0x03, UdsService.ReadDataByIdentifier,
                              (byte)(UdsService.DID_Switch >> 8), (byte)(UdsService.DID_Switch & 0xFF),
                              0x00, 0x00, 0x00, 0x00 };

            if (_device.SendData(request))
            {
                var response = _device.LastReceivedData;
                if (response != null && response.Length >= 4)
                {
                    // 원시 데이터 저장
                    switchData.RawData = BitConverter.ToString(response).Replace("-", " ");
                }
            }

            return switchData;
        }

        #endregion

        #region FBM (Foundation Brake Module)

        /// <summary>
        /// FBM 백업 시작
        /// VB: ECU1FBMStart
        /// </summary>
        public bool StartFbmBackup()
        {
            // 04 31 01 FE 55
            byte[] request = { 0x04, UdsService.RoutineControl, UdsService.StartRoutine,
                              (byte)(UdsService.RID_FBMBackup >> 8), (byte)(UdsService.RID_FBMBackup & 0xFF),
                              0x00, 0x00, 0x00 };
            return SendRoutineCommand(request);
        }

        /// <summary>
        /// FBM 백업 중지
        /// VB: ECU1FBM_Stop
        /// </summary>
        public bool StopFbmBackup()
        {
            // 04 31 02 FE 55
            byte[] request = { 0x04, UdsService.RoutineControl, UdsService.StopRoutine,
                              (byte)(UdsService.RID_FBMBackup >> 8), (byte)(UdsService.RID_FBMBackup & 0xFF),
                              0x00, 0x00, 0x00 };
            return SendRoutineCommand(request);
        }

        /// <summary>
        /// FBM 신호 읽기
        /// VB: ECU1_FBM_Sig
        /// </summary>
        public FbmSignalData ReadFbmSignal()
        {
            var fbmSignal = new FbmSignalData();

            // 03 22 FD 03
            byte[] request = { 0x03, UdsService.ReadDataByIdentifier,
                              (byte)(UdsService.DID_FBMSignal >> 8), (byte)(UdsService.DID_FBMSignal & 0xFF),
                              0x00, 0x00, 0x00, 0x00 };

            if (_device.SendData(request))
            {
                var response = _device.LastReceivedData;
                if (response != null && response.Length >= 7)
                {
                    // 응답 구조: [DID_H][DID_L][DATA...] (PCI, SID는 VrtDevice에서 제거됨)
                    // X1 X2: FBM signal 1 voltage (1 bit = 1/1024 V)
                    // X3 X4: FBM signal 2 voltage (1 bit = 1/1024 V)
                    // X5: switch inputs
                    int signal1Raw = (response[2] << 8) | response[3];
                    int signal2Raw = (response[4] << 8) | response[5];

                    fbmSignal.Signal1Voltage = signal1Raw / 1024.0;
                    fbmSignal.Signal2Voltage = signal2Raw / 1024.0;
                    fbmSignal.SwitchInputs = response[6];

                    // 스위치 상태 해석
                    // bit 0,1 = FBM wakeup switch #1
                    // 00 = deactivated, 01 = activated, 10 = error, 11 = not defined
                    fbmSignal.WakeupSwitch1Status = (byte)(response[6] & 0x03);
                }
            }

            return fbmSignal;
        }

        /// <summary>
        /// FBM 브레이크력 읽기
        /// VB: ECU1FBM_Read
        /// </summary>
        public FbmBrakeForceData ReadFbmBrakeForce()
        {
            var fbm = new FbmBrakeForceData();

            // 03 22 FD F5
            byte[] request = { 0x03, UdsService.ReadDataByIdentifier,
                              (byte)(UdsService.DID_ValveControl >> 8), (byte)(UdsService.DID_ValveControl & 0xFF),
                              0x00, 0x00, 0x00, 0x00 };

            if (_device.SendData(request))
            {
                var response = _device.LastReceivedData;
                if (response != null && response.Length >= 10)
                {
                    // 응답 구조: [DID_H][DID_L][DATA...] (PCI, SID는 VrtDevice에서 제거됨)
                    // 응답 파싱 (1 bit = 1/1024 bar)
                    double factor = 1.0 / 1024.0;

                    int flRaw = (response[2] << 8) | response[3];
                    int frRaw = (response[4] << 8) | response[5];
                    int rlRaw = (response[6] << 8) | response[7];
                    int rrRaw = (response[8] << 8) | response[9];

                    fbm.FrontLeft = flRaw * factor;
                    fbm.FrontRight = frRaw * factor;
                    fbm.RearLeft = rlRaw * factor;
                    fbm.RearRight = rrRaw * factor;
                }
            }

            return fbm;
        }

        #endregion

        #region SAS Setting

        /// <summary>
        /// SAS (Steering Angle Sensor) 설정
        /// </summary>
        public bool PerformSasSetting()
        {
            // 1. Security Access 재수행 (Extended Session 필요)
            if (!StartDiagnosticSession())
                return false;

            Thread.Sleep(100);

            // 2. SAS Calibration Start (04 31 01 FE 57)
            byte[] request1 = { 0x04, UdsService.RoutineControl, UdsService.StartRoutine,
                               (byte)(UdsService.RID_SASCalibrationStart >> 8),
                               (byte)(UdsService.RID_SASCalibrationStart & 0xFF),
                               0x00, 0x00, 0x00 };
            if (!SendRoutineCommand(request1))
                return false;

            Thread.Sleep(300);

            // 3. SAS Calibration (05 31 01 FE 58 0F)
            byte[] request2 = { 0x05, UdsService.RoutineControl, UdsService.StartRoutine,
                               (byte)(UdsService.RID_SASCalibration >> 8),
                               (byte)(UdsService.RID_SASCalibration & 0xFF),
                               0x0F, 0x00, 0x00 };
            if (!SendRoutineCommand(request2))
                return false;

            Thread.Sleep(300);

            // 4. SAS/YRA Store (04 31 01 FE 5A)
            byte[] request3 = { 0x04, UdsService.RoutineControl, UdsService.StartRoutine,
                               (byte)(UdsService.RID_SASYRAStore >> 8),
                               (byte)(UdsService.RID_SASYRAStore & 0xFF),
                               0x00, 0x00, 0x00 };
            if (!SendRoutineCommand(request3))
                return false;

            return true;
        }

        private bool SendRoutineCommand(byte[] request)
        {
            for (int i = 0; i < RetryCount; i++)
            {
                if (_device.SendData(request))
                {
                    // 긍정 응답 확인은 VrtDevice.LastCommandSuccess로
                    if (_device.LastCommandSuccess)
                    {
                        return true;
                    }
                }
                Thread.Sleep(100);
            }
            return false;
        }

        #endregion

        #region Speed Lock (Roller Bench Test Mode)

        /// <summary>
        /// 롤러 벤치 테스트 모드 활성화
        /// </summary>
        public bool ActivateSpeedLock()
        {
            // 04 31 01 FE 54
            byte[] request = { 0x04, UdsService.RoutineControl, UdsService.StartRoutine,
                              (byte)(UdsService.RID_RollerBenchTestMode >> 8),
                              (byte)(UdsService.RID_RollerBenchTestMode & 0xFF),
                              0x00, 0x00, 0x00 };
            return SendRoutineCommand(request);
        }

        /// <summary>
        /// 롤러 벤치 테스트 모드 비활성화
        /// </summary>
        public bool DeactivateSpeedLock()
        {
            // 04 31 02 FE 54
            byte[] request = { 0x04, UdsService.RoutineControl, UdsService.StopRoutine,
                              (byte)(UdsService.RID_RollerBenchTestMode >> 8),
                              (byte)(UdsService.RID_RollerBenchTestMode & 0xFF),
                              0x00, 0x00, 0x00 };
            return SendRoutineCommand(request);
        }

        #endregion
    }

    #region Data Classes

    public class VoltageData
    {
        public double IgnitionVoltage { get; set; }
        public double BatteryVoltage { get; set; }
        public double EspVoltage { get; set; }
        public double TcmVoltage { get; set; }
    }

    public class WheelSpeedData
    {
        public double FrontLeft { get; set; }
        public double FrontRight { get; set; }
        public double RearLeft { get; set; }
        public double RearRight { get; set; }
    }

    /// <summary>
    /// Air Gap 데이터
    /// </summary>
    public class AirGapData
    {
        public double FrontLeft { get; set; }
        public double FrontRight { get; set; }
        public double RearLeft { get; set; }
        public double RearRight { get; set; }
    }

    /// <summary>
    /// LWS (Lining Wear Sensor) 데이터
    /// 브레이크 라이닝 잔량 및 마모 센서 전압
    /// </summary>
    public class LwsData
    {
        public string RawData { get; set; } = string.Empty;

        /// <summary>브레이크 라이닝 잔량 (휠 1~10, 1 bit = 0.4%)</summary>
        public double[] BrakeLiningRemaining { get; set; } = new double[10];

        /// <summary>마모 센서 전압 (휠 1~10, 1 bit = 0.0196V)</summary>
        public double[] WearSensorVoltage { get; set; } = new double[10];
    }

    /// <summary>
    /// 스위치 상태 데이터
    /// </summary>
    public class SwitchData
    {
        public string RawData { get; set; } = string.Empty;
    }

    /// <summary>
    /// FBM 신호 데이터
    /// </summary>
    public class FbmSignalData
    {
        /// <summary>FBM Signal 1 전압 (V)</summary>
        public double Signal1Voltage { get; set; }

        /// <summary>FBM Signal 2 전압 (V)</summary>
        public double Signal2Voltage { get; set; }

        /// <summary>스위치 입력 원시값</summary>
        public byte SwitchInputs { get; set; }

        /// <summary>Wakeup Switch #1 상태 (00=비활성, 01=활성, 10=오류, 11=미정의)</summary>
        public byte WakeupSwitch1Status { get; set; }
    }

    /// <summary>
    /// FBM 브레이크력 데이터 (bar)
    /// </summary>
    public class FbmBrakeForceData
    {
        public double FrontLeft { get; set; }
        public double FrontRight { get; set; }
        public double RearLeft { get; set; }
        public double RearRight { get; set; }
    }

    #endregion
}
