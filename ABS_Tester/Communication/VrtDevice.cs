using System;
using System.Runtime.InteropServices;
using System.Text;
using ABS_Tester.Utils;

namespace ABS_Tester.Communication
{
    /// <summary>
    /// VRT_Device.dll P/Invoke Wrapper
    /// KI1A-ABST mod_VRTs.bas 기반으로 작성
    /// </summary>
    public class VrtDevice : IDisposable
    {
        #region DLL Imports

        [DllImport("VRT_Device.dll", EntryPoint = "_Dll_VRT_Create@4", CallingConvention = CallingConvention.StdCall)]
        private static extern byte Dll_VRT_Create(IntPtr callback);

        [DllImport("VRT_Device.dll", EntryPoint = "_Dll_VRT_Destroy@0", CallingConvention = CallingConvention.StdCall)]
        private static extern byte Dll_VRT_Destroy();

        [DllImport("VRT_Device.dll", EntryPoint = "_Dll_CommInit@0", CallingConvention = CallingConvention.StdCall)]
        private static extern byte Dll_CommInit();

        [DllImport("VRT_Device.dll", EntryPoint = "_Dll_DataDump@0", CallingConvention = CallingConvention.StdCall)]
        private static extern byte Dll_DataDump();

        [DllImport("VRT_Device.dll", EntryPoint = "_Dll_RTC_Reset@0", CallingConvention = CallingConvention.StdCall)]
        private static extern byte Dll_RTC_Reset();

        [DllImport("VRT_Device.dll", EntryPoint = "_Dll_Comm_Start@4", CallingConvention = CallingConvention.StdCall)]
        private static extern byte Dll_Comm_Start(int cType);

        [DllImport("VRT_Device.dll", EntryPoint = "_Dll_Comm_Stop@4", CallingConvention = CallingConvention.StdCall)]
        private static extern byte Dll_Comm_Stop(int cType);

        [DllImport("VRT_Device.dll", EntryPoint = "_Dll_CanModeSet@4", CallingConvention = CallingConvention.StdCall)]
        private static extern byte Dll_CanModeSet(IntPtr setData);

        [DllImport("VRT_Device.dll", EntryPoint = "_Dll_CanRxIdSet@12", CallingConvention = CallingConvention.StdCall)]
        private static extern byte Dll_CanRxIdSet(int count, IntPtr rxStart, IntPtr rxEnd);

        [DllImport("VRT_Device.dll", EntryPoint = "_Dll_CanFlowControl@4", CallingConvention = CallingConvention.StdCall)]
        private static extern byte Dll_CanFlowControl(IntPtr setData);

        [DllImport("VRT_Device.dll", EntryPoint = "_Dll_CanChannelSet@0", CallingConvention = CallingConvention.StdCall)]
        private static extern byte Dll_CanChannelSet();

        [DllImport("VRT_Device.dll", EntryPoint = "_Dll_DataTrans@16", CallingConvention = CallingConvention.StdCall)]
        private static extern byte Dll_DataTrans(int cType, int cLen, IntPtr data, int txMode);

        [DllImport("VRT_Device.dll", EntryPoint = "_Dll_Version@0", CallingConvention = CallingConvention.StdCall)]
        private static extern byte Dll_Version();

        [DllImport("VRT_Device.dll", EntryPoint = "_Dll_PowerChk@0", CallingConvention = CallingConvention.StdCall)]
        private static extern byte Dll_PowerChk();

        [DllImport("VRT_Device.dll", EntryPoint = "_Dll_USBChk@0", CallingConvention = CallingConvention.StdCall)]
        private static extern byte Dll_USBChk();

        [DllImport("VRT_Device.dll", EntryPoint = "_Dll_ChannelSel@12", CallingConvention = CallingConvention.StdCall)]
        private static extern byte Dll_ChannelSel(byte cType, byte cPin1, byte cPin2);

        #endregion

        #region Callback Delegate

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void VrtCallbackDelegate(byte idx, byte result,
            [MarshalAs(UnmanagedType.LPStr)] string rString, int rTime, int dLen);

        #endregion

        #region Enums

        public enum ResponseFunc
        {
            None = -1,
            Version = 0,
            Volt = 1,
            RxData = 2,
            OpenStatus = 3,
            VDScRes = 4
        }

        public enum ResponseResult
        {
            None = -1,
            OK = 0,
            NG = 1,
            Done = 2
        }

        public enum BaudRate
        {
            Baud1M = 0,
            Baud500K = 1,
            Baud250K = 2,
            Baud100K = 3,
            Baud83K = 4,
            Baud50K = 5,
            Baud33K = 6,
            Baud25K = 7
        }

        #endregion

        #region Properties

        public bool IsOpen { get; private set; }
        public bool IsEcuConnected { get; set; }
        public uint TxId { get; set; } = 0x18DA0BF1;  // KNORR EBS5x 기본값
        public uint RxId { get; set; } = 0x18DAF10B;
        public byte[] LastReceivedData { get; private set; }
        public bool LastCommandSuccess { get; private set; }

        /// <summary>
        /// TX/RX 로깅용 Logger (외부에서 주입)
        /// </summary>
        public Logger Logger { get; set; }

        #endregion

        #region Events

        public event EventHandler<CanMessage> DataReceived;
        public event EventHandler<string> LogMessage;

        #endregion

        #region Fields

        private VrtCallbackDelegate _callback;
        private GCHandle _callbackHandle;
        private bool _disposed;
        private readonly object _syncLock = new object();
        private bool _waitingForResponse;

        #endregion

        #region Constructor & Destructor

        public VrtDevice()
        {
            _callback = new VrtCallbackDelegate(OnVrtCallback);
            _callbackHandle = GCHandle.Alloc(_callback);
        }

        ~VrtDevice()
        {
            Dispose(false);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// VRT 장치 초기화 및 열기
        /// </summary>
        public bool Open()
        {
            try
            {
                byte result = Dll_VRT_Create(Marshal.GetFunctionPointerForDelegate(_callback));
                IsOpen = (result == 1);
                Log($"VRT Open: {(IsOpen ? "OK" : "NG")}");
                return IsOpen;
            }
            catch (Exception ex)
            {
                Log($"VRT Open Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// VRT 장치 닫기
        /// </summary>
        public bool Close()
        {
            try
            {
                IsEcuConnected = false;
                byte result = Dll_VRT_Destroy();
                IsOpen = false;
                Log($"VRT Close: {(result == 1 ? "OK" : "NG")}");
                return result == 1;
            }
            catch (Exception ex)
            {
                Log($"VRT Close Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// USB 연결 상태 확인
        /// </summary>
        public bool CheckUsb()
        {
            try
            {
                return Dll_USBChk() == 1;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// CAN 통신 시작 (EBS5x용)
        /// </summary>
        public bool StartCanCommunication(BaudRate baudRate = BaudRate.Baud500K, bool useTerminalResistor = true)
        {
            try
            {
                // 1. 통신 정지
                if (Dll_Comm_Stop(0) != 1)
                {
                    Log("Comm Stop: NG");
                    return false;
                }
                Log("Comm Stop: OK");

                // 2. CAN 모드 설정
                byte[] modeData = new byte[5];
                modeData[0] = 0x00;  // H/W Mode: CAN_High
                modeData[1] = (byte)baudRate;  // Baudrate
                modeData[2] = 0x01;  // CAN Mode: Extended CAN
                modeData[3] = 0x01;  // Simulator Mode
                modeData[4] = (byte)(useTerminalResistor ? 0x01 : 0x00);  // Terminal Resistance

                GCHandle handle = GCHandle.Alloc(modeData, GCHandleType.Pinned);
                try
                {
                    if (Dll_CanModeSet(handle.AddrOfPinnedObject()) != 1)
                    {
                        Log("CAN Mode Set: NG");
                        return false;
                    }
                    Log($"CAN Mode Set: OK (Baud={baudRate})");
                }
                finally
                {
                    handle.Free();
                }

                // 3. 채널 선택
                if (Dll_ChannelSel(0x01, 0x06, 0x0E) != 1)
                {
                    Log("Channel Select: NG");
                    return false;
                }
                Log("Channel Select: OK");

                // 4. RX ID 설정
                if (!SetRxId())
                {
                    Log("RX ID Set: NG");
                    return false;
                }
                Log("RX ID Set: OK");

                // 5. Flow Control 설정
                if (!SetFlowControl())
                {
                    Log("Flow Control Set: NG");
                    return false;
                }
                Log("Flow Control Set: OK");

                // 6. 통신 시작
                if (Dll_Comm_Start(0) != 1)
                {
                    Log("Comm Start: NG");
                    return false;
                }
                Log("Comm Start: OK");

                return true;
            }
            catch (Exception ex)
            {
                Log($"StartCanCommunication Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// CAN 데이터 전송
        /// </summary>
        public bool SendData(byte[] data, int timeoutMs = 3000)
        {
            if (!IsOpen) return false;

            try
            {
                lock (_syncLock)
                {
                    LastCommandSuccess = false;
                    LastReceivedData = null;
                    _waitingForResponse = true;

                    // TxID + Data 조합
                    byte[] txData = new byte[4 + data.Length];
                    txData[0] = (byte)((TxId >> 24) & 0xFF);
                    txData[1] = (byte)((TxId >> 16) & 0xFF);
                    txData[2] = (byte)((TxId >> 8) & 0xFF);
                    txData[3] = (byte)(TxId & 0xFF);
                    Array.Copy(data, 0, txData, 4, data.Length);

                    GCHandle handle = GCHandle.Alloc(txData, GCHandleType.Pinned);
                    try
                    {
                        byte result = Dll_DataTrans(0, txData.Length, handle.AddrOfPinnedObject(), 0);
                        if (result != 1)
                        {
                            Log($"DataTrans: NG");
                            return false;
                        }
                        Log($"TX: {BytesToHex(data)}");

                        // TX 로깅 (패턴 기반)
                        Logger?.LogTx(TxId, data);
                    }
                    finally
                    {
                        handle.Free();
                    }

                    // 응답 대기
                    DateTime startTime = DateTime.Now;
                    while (_waitingForResponse)
                    {
                        Dll_DataDump();
                        System.Threading.Thread.Sleep(10);
                        System.Windows.Forms.Application.DoEvents();

                        if ((DateTime.Now - startTime).TotalMilliseconds > timeoutMs)
                        {
                            Log("Response Timeout!");
                            Logger?.LogError("Timeout waiting for response");
                            _waitingForResponse = false;
                            return false;
                        }
                    }

                    return LastCommandSuccess;
                }
            }
            catch (Exception ex)
            {
                Log($"SendData Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 헥스 문자열로 데이터 전송
        /// </summary>
        public bool SendHexString(string hexString, int timeoutMs = 3000)
        {
            byte[] data = HexStringToBytes(hexString);
            return SendData(data, timeoutMs);
        }

        #endregion

        #region Private Methods

        private bool SetRxId()
        {
            int[] rxStart = new int[] { (int)RxId };
            int[] rxEnd = new int[] { (int)RxId };

            GCHandle handleStart = GCHandle.Alloc(rxStart, GCHandleType.Pinned);
            GCHandle handleEnd = GCHandle.Alloc(rxEnd, GCHandleType.Pinned);
            try
            {
                return Dll_CanRxIdSet(1, handleStart.AddrOfPinnedObject(), handleEnd.AddrOfPinnedObject()) == 1;
            }
            finally
            {
                handleStart.Free();
                handleEnd.Free();
            }
        }

        private bool SetFlowControl()
        {
            byte[] flowData = new byte[12];
            flowData[0] = 0x01;  // Enable

            // RxID
            flowData[1] = (byte)((RxId >> 24) & 0xFF);
            flowData[2] = (byte)((RxId >> 16) & 0xFF);
            flowData[3] = (byte)((RxId >> 8) & 0xFF);
            flowData[4] = (byte)(RxId & 0xFF);

            // TxID
            flowData[5] = (byte)((TxId >> 24) & 0xFF);
            flowData[6] = (byte)((TxId >> 16) & 0xFF);
            flowData[7] = (byte)((TxId >> 8) & 0xFF);
            flowData[8] = (byte)(TxId & 0xFF);

            flowData[9] = 0x30;   // Response Data1
            flowData[10] = 0x00;  // Response Data2
            flowData[11] = 0x05;  // Response Data3

            GCHandle handle = GCHandle.Alloc(flowData, GCHandleType.Pinned);
            try
            {
                return Dll_CanFlowControl(handle.AddrOfPinnedObject()) == 1;
            }
            finally
            {
                handle.Free();
            }
        }

        private void OnVrtCallback(byte idx, byte result, string rString, int rTime, int dLen)
        {
            if (string.IsNullOrEmpty(rString?.Trim())) return;

            ResponseFunc func = (ResponseFunc)idx;
            ResponseResult res = (ResponseResult)result;

            switch (func)
            {
                case ResponseFunc.RxData:
                    if (res == ResponseResult.OK)
                    {
                        ProcessRxData(rString, dLen);
                    }
                    break;

                case ResponseFunc.Version:
                case ResponseFunc.Volt:
                    Log($"[{func}] {rString}");
                    break;
            }
        }

        private void ProcessRxData(string rString, int dLen)
        {
            try
            {
                byte[] data = HexStringToBytes(rString.Replace(" ", ""));

                // RxID 확인
                if (data.Length >= 4)
                {
                    uint receivedId = (uint)((data[0] << 24) | (data[1] << 16) | (data[2] << 8) | data[3]);

                    if (receivedId == RxId)
                    {
                        // CAN ID 제외한 UDS 프레임 추출
                        byte[] udsFrame = new byte[data.Length - 4];
                        Array.Copy(data, 4, udsFrame, 0, udsFrame.Length);

                        // Negative Response 확인 (7F)
                        if (udsFrame.Length >= 3 && udsFrame[1] == 0x7F)
                        {
                            if (udsFrame.Length >= 4 && udsFrame[3] == 0x78)
                            {
                                // Pending 응답 (NRC 0x78) - 계속 대기
                                Log($"RX: {BytesToHex(udsFrame)} (Pending)");
                                return;
                            }
                        }

                        // PCI, SID 제거하고 DID+DATA만 추출 (VB의 Ret_Data와 동일)
                        // 응답 구조: [PCI][SID][DID_H][DID_L][DATA...] 또는 [PCI][SID][SubFunc][DATA...]
                        byte[] responseData = ExtractResponseData(udsFrame);
                        LastReceivedData = responseData;

                        // 긍정 응답 확인
                        if (udsFrame.Length >= 2 && (udsFrame[1] & 0x40) != 0)
                        {
                            LastCommandSuccess = true;
                        }

                        Log($"RX: {BytesToHex(udsFrame)} -> Data: {BytesToHex(responseData)}");

                        // RX 로깅 (패턴 기반)
                        Logger?.LogRx(RxId, udsFrame);

                        DataReceived?.Invoke(this, new CanMessage(RxId, responseData));
                    }
                }

                _waitingForResponse = false;
            }
            catch (Exception ex)
            {
                Log($"ProcessRxData Error: {ex.Message}");
                _waitingForResponse = false;
            }
        }

        /// <summary>
        /// UDS 프레임에서 PCI, SID를 제거하고 순수 데이터만 추출
        /// </summary>
        private byte[] ExtractResponseData(byte[] udsFrame)
        {
            if (udsFrame == null || udsFrame.Length < 2)
                return udsFrame ?? new byte[0];

            int pci = udsFrame[0];
            int dataStart;

            // Single Frame: PCI 상위 4비트가 0
            if ((pci & 0xF0) == 0x00)
            {
                // [PCI=len][SID][DATA...] → DATA는 인덱스 2부터
                dataStart = 2;
            }
            // First Frame: PCI 상위 4비트가 1
            else if ((pci & 0xF0) == 0x10)
            {
                // [10+len_H][len_L][SID][DATA...] → DATA는 인덱스 3부터
                dataStart = 3;
            }
            else
            {
                // 기타 (Consecutive Frame 등) - 그대로 반환
                return udsFrame;
            }

            if (dataStart >= udsFrame.Length)
                return new byte[0];

            byte[] result = new byte[udsFrame.Length - dataStart];
            Array.Copy(udsFrame, dataStart, result, 0, result.Length);
            return result;
        }

        private void Log(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            LogMessage?.Invoke(this, $"[{timestamp}] {message}");
        }

        #endregion

        #region Static Helpers

        public static byte[] HexStringToBytes(string hex)
        {
            hex = hex.Replace(" ", "").Replace("-", "");
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return bytes;
        }

        public static string BytesToHex(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                if (sb.Length > 0) sb.Append(" ");
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
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
            if (!_disposed)
            {
                if (disposing)
                {
                    Close();
                }

                if (_callbackHandle.IsAllocated)
                {
                    _callbackHandle.Free();
                }

                _disposed = true;
            }
        }

        #endregion
    }
}
