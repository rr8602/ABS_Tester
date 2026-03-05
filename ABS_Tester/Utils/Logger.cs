using System;
using System.IO;
using System.Text;

namespace ABS_Tester.Utils
{
    public class Logger : IDisposable
    {
        private static Logger _instance;
        private static readonly object _lock = new object();

        private StreamWriter _writer;
        private string _logFilePath;
        private bool _disposed;

        public event EventHandler<string> LogWritten;

        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
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

        public string LogFilePath => _logFilePath;
        public bool IsLogging => _writer != null;

        private Logger()
        {
        }

        /// <summary>
        /// 로그 파일 시작
        /// 경로 구조: Logs\yyyy\MM\dd\ABS_Test_HHmmss.log
        /// 예: Logs\2026\02\26\ABS_Test_143052.log
        /// </summary>
        public void StartLogging(string basePath = null)
        {
            if (_writer != null)
            {
                return; // 이미 로깅 중
            }

            try
            {
                // 기본 경로: 실행 파일 위치의 Logs 폴더
                if (string.IsNullOrEmpty(basePath))
                {
                    basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                }

                // 날짜 기반 폴더 구조: Logs\yyyy\MM\dd
                DateTime now = DateTime.Now;
                string datePath = Path.Combine(basePath,
                    now.ToString("yyyy"),
                    now.ToString("MM"),
                    now.ToString("dd"));

                // 폴더 생성 (하위 폴더까지 모두 생성)
                if (!Directory.Exists(datePath))
                {
                    Directory.CreateDirectory(datePath);
                }

                // 파일명: ABS_Test_HHmmss.log
                string fileName = $"ABS_Test_{now:HHmmss}.log";
                _logFilePath = Path.Combine(datePath, fileName);

                // 파일 스트림 열기
                _writer = new StreamWriter(_logFilePath, true, Encoding.UTF8);
                _writer.AutoFlush = true;

                // 헤더 작성
                WriteHeader();
            }
            catch (Exception ex)
            {
                _writer = null;
                throw new Exception($"로그 파일 생성 실패: {ex.Message}");
            }
        }

        /// <summary>
        /// 로그 파일 종료
        /// </summary>
        public void StopLogging()
        {
            if (_writer != null)
            {
                WriteFooter();
                _writer.Close();
                _writer.Dispose();
                _writer = null;
            }
        }

        /// <summary>
        /// 로그 기록
        /// </summary>
        public void Log(string message, LogLevel level = LogLevel.Info)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string levelStr = GetLevelString(level);
            string logLine = $"[{timestamp}] [{levelStr}] {message}";

            // 파일에 기록
            _writer?.WriteLine(logLine);

            // 이벤트 발생 (UI 업데이트용)
            LogWritten?.Invoke(this, logLine);
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
            if (string.IsNullOrEmpty(title))
            {
                _writer?.WriteLine(new string('-', 60));
            }
            else
            {
                _writer?.WriteLine($"--- {title} {new string('-', 50 - title.Length)}");
            }
        }

        /// <summary>
        /// CAN 데이터 기록
        /// </summary>
        public void LogCanData(string direction, uint canId, byte[] data)
        {
            string dataStr = BitConverter.ToString(data).Replace("-", " ");
            Log($"[CAN {direction}] ID=0x{canId:X8} Data=[{dataStr}]", LogLevel.Debug);
        }

        private void WriteHeader()
        {
            _writer.WriteLine("=".PadRight(60, '='));
            _writer.WriteLine("  ABS/EBS ECU 테스트 로그");
            _writer.WriteLine($"  시작 시간: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            _writer.WriteLine("=".PadRight(60, '='));
            _writer.WriteLine();
        }

        private void WriteFooter()
        {
            _writer.WriteLine();
            _writer.WriteLine("=".PadRight(60, '='));
            _writer.WriteLine($"  종료 시간: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            _writer.WriteLine("=".PadRight(60, '='));
        }

        private string GetLevelString(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug: return "DBG";
                case LogLevel.Info: return "INF";
                case LogLevel.Warning: return "WRN";
                case LogLevel.Error: return "ERR";
                default: return "INF";
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                StopLogging();
                _disposed = true;
            }
        }
    }

    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }
}
