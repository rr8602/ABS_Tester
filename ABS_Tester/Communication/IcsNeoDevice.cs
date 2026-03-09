using System;
using ABS_Tester;
using ABS_Tester.Utils;

namespace ABS_Tester.Communication
{
    /// <summary>
    /// ICS neoVI 장치 고수준 래퍼 클래스
    /// CSnet1 예제 기반 구현
    /// </summary>
    public class IcsNeoDevice : IDisposable
    {
        #region Fields

        private IntPtr _hObject;
        private bool _isOpen;
        private bool _isEcuConnected;
        private icsSpyMessage[] _messageBuffer = new icsSpyMessage[20000];
        private OptionsNeoEx _deviceOption = new OptionsNeoEx();
        private int _openDeviceType;
        private string _deviceInfo;
        private bool _disposed = false;

        private byte[] _lastReceivedData;
        private bool _lastCommandSuccess;

        #endregion

        #region Properties

        public uint TxId { get; set; } = 0x18DA2AF1; // 0x18DA0BF1;
        public uint RxId { get; set; } = 0x18DAF12A;  // 0x18DAF10B;
        public bool IsOpen => _isOpen;
        public bool IsEcuConnected
        {
            get => _isEcuConnected;
            set => _isEcuConnected = value;
        }
        public Logger Logger { get; set; }
        public byte[] LastReceivedData => _lastReceivedData;
        public bool LastCommandSuccess => _lastCommandSuccess;
        public string DeviceInfo => _deviceInfo;

        #endregion

        #region Events

        public event EventHandler<string> LogMessage;
        public event EventHandler<CanMessage> DataReceived;

        #endregion

        #region Constructor

        public IcsNeoDevice()
        {
            _isOpen = false;
            _isEcuConnected = false;
        }

        #endregion

        #region Device Connection

        /// <summary>
        /// neoVI 장치 열기 및 CAN 통신 시작
        /// ICS는 USB 연결 단계가 별도로 필요 없음
        /// </summary>
        public bool Open(int baudRate = 500000)
        {
            if (_isOpen) return true;

            try
            {
                NeoDeviceEx[] neoDevices = new NeoDeviceEx[16];
                byte[] bNetwork = new byte[255];
                int iNumberOfDevices = 15;

                // 네트워크 ID 배열 초기화
                for (int i = 0; i < 255; i++)
                {
                    bNetwork[i] = (byte)i;
                }

                // 연결된 장치 찾기
                int iResult = icsNeoDll.icsneoFindDevices(ref neoDevices[0], ref iNumberOfDevices, 0, 0, ref _deviceOption, 0);
                if (iResult == 0)
                {
                    RaiseLogMessage("neoVI 장치 검색 실패");
                    return false;
                }

                if (iNumberOfDevices < 1)
                {
                    RaiseLogMessage("연결된 neoVI 장치 없음");
                    return false;
                }

                ABS_Tester.NeoDevice ndNeoToOpen = neoDevices[0].neoDevice;

                // 장치 열기
                iResult = icsNeoDll.icsneoOpenNeoDevice(ref ndNeoToOpen, ref _hObject, ref bNetwork[0], 1, 0);
                if (iResult != 1)
                {
                    RaiseLogMessage("neoVI 장치 열기 실패");
                    return false;
                }

                _openDeviceType = ndNeoToOpen.DeviceType;
                _deviceInfo = GetDeviceTypeName(ndNeoToOpen.DeviceType, ndNeoToOpen.SerialNumber);
                _isOpen = true;

                RaiseLogMessage($"neoVI 장치 연결 성공: {_deviceInfo}");

                // CAN 비트레이트 설정
                iResult = icsNeoDll.icsneoSetBitRate(_hObject, baudRate, (int)eNETWORK_ID.NETID_HSCAN);
                if (iResult != 1)
                {
                    RaiseLogMessage($"CAN 비트레이트 설정 실패 ({baudRate} bps)");
                    Close();
                    return false;
                }

                RaiseLogMessage($"CAN 통신 초기화 완료 ({baudRate} bps)");
                return true;
            }
            catch (Exception ex)
            {
                RaiseLogMessage($"neoVI 장치 열기 예외: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// neoVI 장치 닫기
        /// </summary>
        public void Close()
        {
            if (!_isOpen) return;

            try
            {
                int iNumberOfErrors = 0;
                icsNeoDll.icsneoClosePort(_hObject, ref iNumberOfErrors);
                _isOpen = false;
                _isEcuConnected = false;
                RaiseLogMessage("neoVI 장치 연결 해제");
            }
            catch (Exception ex)
            {
                RaiseLogMessage($"neoVI 장치 닫기 예외: {ex.Message}");
            }
        }

        #endregion

        #region Send/Receive

        /// <summary>
        /// CAN 데이터 전송 및 응답 대기
        /// </summary>
        public bool SendData(byte[] data, int timeout = 3000)
        {
            if (!_isOpen || data == null || data.Length == 0)
            {
                _lastCommandSuccess = false;
                return false;
            }

            try
            {
                // TX 로깅 (파일 + UI)
                Logger?.LogTx(TxId, data);
                RaiseLogMessage($"TX >> 0x{TxId:X8}: {BytesToHex(data)}");

                // 수신 버퍼 비우기
                ClearReceiveBuffer();

                // icsSpyMessage 구조체 생성
                icsSpyMessage txMessage = new icsSpyMessage();
                txMessage.ArbIDOrHeader = (int)TxId;
                txMessage.StatusBitField = (int)eDATA_STATUS_BITFIELD_1.SPY_STATUS_XTD_FRAME; // Extended CAN
                txMessage.NumberBytesData = (byte)Math.Min(data.Length, 8);

                // 데이터 복사
                if (data.Length >= 1) txMessage.Data1 = data[0];
                if (data.Length >= 2) txMessage.Data2 = data[1];
                if (data.Length >= 3) txMessage.Data3 = data[2];
                if (data.Length >= 4) txMessage.Data4 = data[3];
                if (data.Length >= 5) txMessage.Data5 = data[4];
                if (data.Length >= 6) txMessage.Data6 = data[5];
                if (data.Length >= 7) txMessage.Data7 = data[6];
                if (data.Length >= 8) txMessage.Data8 = data[7];

                // 메시지 전송
                int txResult = icsNeoDll.icsneoTxMessages(_hObject, ref txMessage, (int)eNETWORK_ID.NETID_HSCAN, 1);
                if (txResult != 1)
                {
                    _lastCommandSuccess = false;
                    return false;
                }

                // 응답 대기
                return WaitForResponse(timeout);
            }
            catch (Exception ex)
            {
                RaiseLogMessage($"CAN 전송 예외: {ex.Message}");
                _lastCommandSuccess = false;
                return false;
            }
        }

        /// <summary>
        /// 수신 버퍼 비우기
        /// </summary>
        private void ClearReceiveBuffer()
        {
            int lNumberOfMessages = 0;
            int lNumberOfErrors = 0;

            // 버퍼에 있는 메시지 모두 읽어서 버리기
            while (true)
            {
                int result = icsNeoDll.icsneoGetMessages(_hObject, ref _messageBuffer[0], ref lNumberOfMessages, ref lNumberOfErrors);
                if (result != 1 || lNumberOfMessages == 0)
                    break;
            }
        }

        /// <summary>
        /// 응답 대기 및 처리
        /// </summary>
        private bool WaitForResponse(int timeout)
        {
            _lastReceivedData = null;
            _lastCommandSuccess = false;

            DateTime startTime = DateTime.Now;
            int lNumberOfMessages = 0;
            int lNumberOfErrors = 0;

            while ((DateTime.Now - startTime).TotalMilliseconds < timeout)
            {
                // 메시지 수신
                int result = icsNeoDll.icsneoGetMessages(_hObject, ref _messageBuffer[0], ref lNumberOfMessages, ref lNumberOfErrors);

                if (result == 1 && lNumberOfMessages > 0)
                {
                    for (int i = 0; i < lNumberOfMessages; i++)
                    {
                        var msg = _messageBuffer[i];

                        // TX 메시지 스킵
                        if ((msg.StatusBitField & (int)eDATA_STATUS_BITFIELD_1.SPY_STATUS_TX_MSG) != 0)
                            continue;

                        // RxId 필터링
                        if ((uint)msg.ArbIDOrHeader != RxId)
                            continue;

                        // 데이터 추출
                        byte[] rxData = ExtractMessageData(msg);

                        // RX 로깅 (파일 + UI)
                        Logger?.LogRx(RxId, rxData);
                        RaiseLogMessage($"RX << 0x{RxId:X8}: {BytesToHex(rxData)}");

                        // DataReceived 이벤트
                        DataReceived?.Invoke(this, new CanMessage(RxId, rxData));

                        // 응답 처리
                        return ProcessUdsResponse(rxData);
                    }
                }

                System.Threading.Thread.Sleep(10);
            }

            RaiseLogMessage("응답 타임아웃");
            return false;
        }

        /// <summary>
        /// UDS 응답 처리
        /// </summary>
        private bool ProcessUdsResponse(byte[] rawData)
        {
            if (rawData == null || rawData.Length < 2)
            {
                _lastCommandSuccess = false;
                return false;
            }

            // PCI 바이트 분석
            byte pci = rawData[0];
            int pciType = (pci >> 4) & 0x0F;

            byte[] responseData;

            if (pciType == 0) // Single Frame
            {
                int dataLen = pci & 0x0F;
                if (dataLen == 0 || rawData.Length < dataLen + 1)
                {
                    _lastCommandSuccess = false;
                    return false;
                }

                // SID 확인
                byte sid = rawData[1];

                // 긍정 응답 (SID + 0x40)
                if ((sid & 0x40) != 0)
                {
                    // PCI, SID 제거 후 저장
                    responseData = new byte[dataLen - 1];
                    Array.Copy(rawData, 2, responseData, 0, dataLen - 1);
                    _lastReceivedData = responseData;
                    _lastCommandSuccess = true;
                    return true;
                }
                // 부정 응답 (0x7F)
                else if (sid == 0x7F)
                {
                    _lastCommandSuccess = false;
                    if (rawData.Length >= 4)
                    {
                        RaiseLogMessage($"부정 응답: SID=0x{rawData[2]:X2}, NRC=0x{rawData[3]:X2}");
                    }
                    return false;
                }
            }
            else if (pciType == 1) // First Frame (멀티프레임)
            {
                // TODO: 멀티프레임 처리 구현
                _lastCommandSuccess = false;
                return false;
            }

            _lastCommandSuccess = false;
            return false;
        }

        /// <summary>
        /// icsSpyMessage에서 데이터 추출
        /// </summary>
        private byte[] ExtractMessageData(icsSpyMessage msg)
        {
            byte[] data = new byte[msg.NumberBytesData];
            if (msg.NumberBytesData >= 1) data[0] = msg.Data1;
            if (msg.NumberBytesData >= 2) data[1] = msg.Data2;
            if (msg.NumberBytesData >= 3) data[2] = msg.Data3;
            if (msg.NumberBytesData >= 4) data[3] = msg.Data4;
            if (msg.NumberBytesData >= 5) data[4] = msg.Data5;
            if (msg.NumberBytesData >= 6) data[5] = msg.Data6;
            if (msg.NumberBytesData >= 7) data[6] = msg.Data7;
            if (msg.NumberBytesData >= 8) data[7] = msg.Data8;
            return data;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// 장치 타입 이름 반환
        /// </summary>
        private string GetDeviceTypeName(int deviceType, int serialNumber)
        {
            string typeName;
            switch (deviceType)
            {
                case (int)eHardwareTypes.NEODEVICE_FIRE:
                    typeName = "neoVI FIRE";
                    break;
                case (int)eHardwareTypes.NEODEVICE_FIRE2:
                    typeName = "neoVI FIRE 2";
                    break;
                case (int)eHardwareTypes.NEODEVICE_VCAN3:
                    typeName = "ValueCAN 3";
                    break;
                case (int)eHardwareTypes.NEODEVICE_VCAN41:
                    typeName = "ValueCAN 4-1";
                    break;
                case (int)eHardwareTypes.NEODEVICE_VCAN42:
                    typeName = "ValueCAN 4-2";
                    break;
                case (int)eHardwareTypes.NEODEVICE_RADGALAXY:
                    typeName = "RAD-Galaxy";
                    break;
                default:
                    typeName = "neoVI";
                    break;
            }

            // 시리얼 번호 변환 시도
            try
            {
                byte[] bSN = new byte[6];
                uint stringSize = 6;
                icsNeoDll.icsneoSerialNumberToString((uint)serialNumber, ref bSN[0], ref stringSize);
                string sn = System.Text.Encoding.ASCII.GetString(bSN).TrimEnd('\0');
                if (!string.IsNullOrEmpty(sn))
                    return $"{typeName} SN:{sn}";
            }
            catch { }

            return $"{typeName} SN:{serialNumber}";
        }

        /// <summary>
        /// 로그 메시지 발생
        /// </summary>
        private void RaiseLogMessage(string message)
        {
            LogMessage?.Invoke(this, message);
        }

        /// <summary>
        /// 바이트 배열을 16진수 문자열로 변환
        /// </summary>
        public static string BytesToHex(byte[] data)
        {
            if (data == null || data.Length == 0)
                return string.Empty;

            return BitConverter.ToString(data).Replace("-", " ");
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                Close();
            }

            _disposed = true;
        }

        ~IcsNeoDevice()
        {
            Dispose(false);
        }

        #endregion
    }
}
