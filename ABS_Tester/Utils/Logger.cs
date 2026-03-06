using System;
using System.IO;
using System.Text;

namespace ABS_Tester.Utils
{
    /// <summary>
    /// ECU 타입 (ABS Tester용)
    /// </summary>
    public enum EcuType
    {
        ABS,
        EBS
    }

    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }

    /// <summary>
    /// ECU 테스트 로거 - TX/RX 패턴 기반
    /// ECU_Test_Logger_Pattern.md 패턴 준수
    /// </summary>
    public class Logger : IDisposable
    {
        #region Singleton

        private static Logger _instance;
        private static readonly object _instanceLock = new object();

        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_instanceLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new Logger();
                        }
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Fields

        private StreamWriter _writer;
        private string _logFilePath;
        private string _workflowName;
        private DateTime _startTime;
        private bool _disposed = false;
        private readonly object _lock = new object();

        // 기본 로그 폴더 (실행 파일 위치 기준)
        private static readonly string BaseLogFolder = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Logs");

        #endregion

        #region Properties

        public string LogFilePath => _logFilePath;
        public bool IsLogging => _writer != null;

        #endregion

        #region Events

        public event EventHandler<string> LogWritten;

        #endregion

        #region Constructor

        private Logger()
        {
        }

        #endregion

        #region Session Management

        /// <summary>
        /// 워크플로우 로깅 세션 시작 (ECU 타입 지정)
        /// </summary>
        public void StartSession(string workflowName, EcuType ecuType)
        {
            StartSessionInternal(workflowName, ecuType.ToString());
        }

        /// <summary>
        /// 워크플로우 로깅 세션 시작 (전체 세션용 - ECU 테스트 추적)
        /// </summary>
        public void StartSession(string sessionName)
        {
            StartSessionInternal(sessionName, "ABS/EBS ECU");
        }

        /// <summary>
        /// 기존 StartLogging과 호환성을 위한 메서드
        /// </summary>
        public void StartLogging(string basePath = null)
        {
            StartSessionInternal("ABS_Test", "ABS/EBS ECU", basePath);
        }

        private void StartSessionInternal(string workflowName, string ecuTypeStr, string basePath = null)
        {
            if (_writer != null)
            {
                return; // 이미 로깅 중
            }

            _workflowName = workflowName;
            _startTime = DateTime.Now;

            try
            {
                // 기본 경로: 실행 파일 위치의 Logs 폴더
                if (string.IsNullOrEmpty(basePath))
                {
                    basePath = BaseLogFolder;
                }

                // 폴더 생성: Logs\yyyy\MM\dd
                string dateFolder = Path.Combine(
                    basePath,
                    _startTime.ToString("yyyy"),
                    _startTime.ToString("MM"),
                    _startTime.ToString("dd"));

                Directory.CreateDirectory(dateFolder);

                // 파일명: {WorkflowName}_{HHmmss}.log
                string fileName = $"{workflowName}_{_startTime:HHmmss}.log";
                _logFilePath = Path.Combine(dateFolder, fileName);

                _writer = new StreamWriter(_logFilePath, false, Encoding.UTF8);
                _writer.AutoFlush = true;

                WriteHeader(ecuTypeStr);
            }
            catch (Exception ex)
            {
                _writer = null;
                throw new Exception($"로그 파일 생성 실패: {ex.Message}");
            }
        }

        private void WriteHeader(string ecuTypeStr)
        {
            var sb = new StringBuilder();
            sb.AppendLine("================================================================================");
            sb.AppendLine($"  Workflow    : {_workflowName}");
            sb.AppendLine($"  ECU Type    : {ecuTypeStr}");
            sb.AppendLine($"  Start Time  : {_startTime:yyyy-MM-dd HH:mm:ss.fff}");
            sb.AppendLine("================================================================================");
            sb.AppendLine();
            sb.AppendLine("  Time           | Dir | CAN ID     | Data (HEX)                        | Description");
            sb.AppendLine("  ---------------+-----+------------+-----------------------------------+---------------------------");

            lock (_lock)
            {
                _writer?.WriteLine(sb.ToString());
            }
        }

        /// <summary>
        /// 워크플로우 로깅 세션 종료
        /// </summary>
        public void EndSession(bool success, string errorMessage = "")
        {
            if (_writer == null) return;

            DateTime endTime = DateTime.Now;
            TimeSpan duration = endTime - _startTime;

            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("================================================================================");
            sb.AppendLine($"  End Time    : {endTime:yyyy-MM-dd HH:mm:ss.fff}");
            sb.AppendLine($"  Duration    : {duration.TotalSeconds:F3} seconds");
            sb.AppendLine($"  Result      : {(success ? "SUCCESS" : "FAILED")}");
            if (!success && !string.IsNullOrEmpty(errorMessage))
            {
                sb.AppendLine($"  Error       : {errorMessage}");
            }
            sb.AppendLine("================================================================================");

            lock (_lock)
            {
                _writer?.WriteLine(sb.ToString());
                _writer?.Flush();
            }
        }

        /// <summary>
        /// 기존 StopLogging과 호환성을 위한 메서드
        /// </summary>
        public void StopLogging()
        {
            lock (_lock)
            {
                _writer?.Close();
                _writer?.Dispose();
                _writer = null;
            }
        }

        #endregion

        #region TX/RX Logging

        /// <summary>
        /// TX (송신) 로그 기록
        /// </summary>
        public void LogTx(uint canId, byte[] data, string description = "")
        {
            LogMessage("TX", canId, data, description);
        }

        /// <summary>
        /// RX (수신) 로그 기록
        /// </summary>
        public void LogRx(uint canId, byte[] data, string description = "")
        {
            LogMessage("RX", canId, data, description);
        }

        private void LogMessage(string direction, uint canId, byte[] data, string description)
        {
            if (_writer == null) return;

            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string dirArrow = direction == "TX" ? ">>" : "<<";
            string hexData = FormatHexData(data);
            string descText = string.IsNullOrEmpty(description)
                ? ParseUdsDescription(data, direction)
                : description;

            string line = $"  {timestamp}   | {direction} {dirArrow} | 0x{canId:X8} | {hexData,-33} | {descText}";

            lock (_lock)
            {
                _writer?.WriteLine(line);
            }

            // 이벤트 발생 (UI 업데이트용)
            LogWritten?.Invoke(this, line);
        }

        private string FormatHexData(byte[] data)
        {
            if (data == null || data.Length == 0) return "";

            var sb = new StringBuilder();
            for (int i = 0; i < data.Length && i < 8; i++)
            {
                if (i > 0) sb.Append(" ");
                sb.Append(data[i].ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// UDS 메시지 자동 설명 생성 (KNORR EBS5x 기준)
        /// </summary>
        private string ParseUdsDescription(byte[] data, string direction)
        {
            if (data == null || data.Length < 2) return "";

            int pci = data[0];

            // Multi-frame 처리
            if ((pci & 0xF0) == 0x10) return "First Frame (Multi)";
            if ((pci & 0xF0) == 0x20) return $"Consecutive Frame (SN={pci & 0x0F})";
            if (pci == 0x30) return "Flow Control";

            byte sid = data[1];

            // TX (Request)
            if (direction == "TX")
            {
                switch (sid)
                {
                    case 0x10: // DiagnosticSessionControl
                        if (data.Length > 2)
                            return $"DiagSessionControl (0x{data[2]:X2})";
                        return "DiagSessionControl";
                    case 0x11: return "ECUReset";
                    case 0x14: return "ClearDTC";
                    case 0x19: return "ReadDTCInfo";
                    case 0x22: // ReadDataByIdentifier
                        if (data.Length > 3)
                            return $"ReadDataByID (0x{data[2]:X2}{data[3]:X2})";
                        return "ReadDataByID";
                    case 0x27: // SecurityAccess
                        if (data.Length > 2)
                            return data[2] == 0x03 ? "SecurityAccess (Seed Req L3)" : $"SecurityAccess (Key Send L{data[2] - 1})";
                        return "SecurityAccess";
                    case 0x2E: // WriteDataByIdentifier
                        if (data.Length > 3)
                            return $"WriteDataByID (0x{data[2]:X2}{data[3]:X2})";
                        return "WriteDataByID";
                    case 0x2F: // InputOutputControlByIdentifier
                        if (data.Length > 3)
                            return $"IOControl (0x{data[2]:X2}{data[3]:X2})";
                        return "IOControl";
                    case 0x31: // RoutineControl
                        if (data.Length > 4)
                            return $"RoutineControl (0x{data[3]:X2}{data[4]:X2})";
                        return "RoutineControl";
                    case 0x3E: return "TesterPresent";
                    default: return $"SID 0x{sid:X2}";
                }
            }
            // RX (Response)
            else
            {
                if (sid == 0x7F && data.Length >= 4)
                    return $"Negative Response (NRC=0x{data[3]:X2})";

                switch (sid)
                {
                    case 0x50: return "DiagSessionControl +RSP";
                    case 0x51: return "ECUReset +RSP";
                    case 0x54: return "ClearDTC +RSP";
                    case 0x59: return "ReadDTCInfo +RSP";
                    case 0x62: return "ReadDataByID +RSP";
                    case 0x67: return "SecurityAccess +RSP";
                    case 0x6E: return "WriteDataByID +RSP";
                    case 0x6F: return "IOControl +RSP";
                    case 0x71: return "RoutineControl +RSP";
                    case 0x7E: return "TesterPresent +RSP";
                    default: return $"RSP 0x{sid:X2}";
                }
            }
        }

        #endregion

        #region General Logging

        /// <summary>
        /// 일반 정보 로그
        /// </summary>
        public void LogInfo(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string line = $"  {timestamp}   | --- | ---------- | {message}";

            lock (_lock)
            {
                _writer?.WriteLine(line);
            }

            LogWritten?.Invoke(this, line);
        }

        /// <summary>
        /// 에러 로그
        /// </summary>
        public void LogError(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string line = $"  {timestamp}   | ERR | ---------- | *** ERROR: {message} ***";

            lock (_lock)
            {
                _writer?.WriteLine(line);
            }

            LogWritten?.Invoke(this, line);
        }

        /// <summary>
        /// 단계 구분자 로그
        /// </summary>
        public void LogStep(string stepName)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");

            lock (_lock)
            {
                _writer?.WriteLine();
                _writer?.WriteLine($"  [{timestamp}] ===== {stepName} =====");
                _writer?.WriteLine();
            }

            LogWritten?.Invoke(this, $"[{timestamp}] ===== {stepName} =====");
        }

        /// <summary>
        /// 기존 Log 메서드와 호환성 유지
        /// </summary>
        public void Log(string message, LogLevel level = LogLevel.Info)
        {
            switch (level)
            {
                case LogLevel.Error:
                    LogError(message);
                    break;
                default:
                    LogInfo(message);
                    break;
            }
        }

        /// <summary>
        /// 테스트 결과 기록
        /// </summary>
        public void LogTestResult(string testName, bool passed, string details = null)
        {
            string result = passed ? "PASS" : "FAIL";
            string message = $"[TEST] {testName}: {result}";
            if (!string.IsNullOrEmpty(details))
            {
                message += $" - {details}";
            }
            Log(message, passed ? LogLevel.Info : LogLevel.Error);
        }

        /// <summary>
        /// 구분선 기록
        /// </summary>
        public void LogSeparator(string title = null)
        {
            lock (_lock)
            {
                if (string.IsNullOrEmpty(title))
                {
                    _writer?.WriteLine(new string('-', 80));
                }
                else
                {
                    _writer?.WriteLine();
                    _writer?.WriteLine($"  [{DateTime.Now:HH:mm:ss.fff}] ----- {title} {new string('-', 50 - title.Length)}");
                    _writer?.WriteLine();
                }
            }
        }

        /// <summary>
        /// CAN 데이터 기록 (기존 호환성)
        /// </summary>
        public void LogCanData(string direction, uint canId, byte[] data)
        {
            if (direction.ToUpper() == "TX")
                LogTx(canId, data);
            else
                LogRx(canId, data);
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
                lock (_lock)
                {
                    if (_writer != null)
                    {
                        EndSession(true);
                        _writer.Dispose();
                        _writer = null;
                    }
                }
            }

            _disposed = true;
        }

        ~Logger()
        {
            Dispose(false);
        }

        #endregion
    }
}
