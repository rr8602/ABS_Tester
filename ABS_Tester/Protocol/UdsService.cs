using System;

namespace ABS_Tester.Protocol
{
    /// <summary>
    /// UDS (Unified Diagnostic Services) 상수 및 유틸리티
    /// ISO 14229 기반
    /// </summary>
    public static class UdsService
    {
        #region Service IDs (SID)

        /// <summary>0x10 - Diagnostic Session Control</summary>
        public const byte DiagnosticSessionControl = 0x10;

        /// <summary>0x11 - ECU Reset</summary>
        public const byte EcuReset = 0x11;

        /// <summary>0x14 - Clear Diagnostic Information</summary>
        public const byte ClearDiagnosticInformation = 0x14;

        /// <summary>0x19 - Read DTC Information</summary>
        public const byte ReadDtcInformation = 0x19;

        /// <summary>0x22 - Read Data By Identifier</summary>
        public const byte ReadDataByIdentifier = 0x22;

        /// <summary>0x27 - Security Access</summary>
        public const byte SecurityAccess = 0x27;

        /// <summary>0x2E - Write Data By Identifier</summary>
        public const byte WriteDataByIdentifier = 0x2E;

        /// <summary>0x2F - Input Output Control By Identifier</summary>
        public const byte InputOutputControlByIdentifier = 0x2F;

        /// <summary>0x31 - Routine Control</summary>
        public const byte RoutineControl = 0x31;

        /// <summary>0x3E - Tester Present</summary>
        public const byte TesterPresent = 0x3E;

        #endregion

        #region Diagnostic Session Types

        public const byte DefaultSession = 0x01;
        public const byte ProgrammingSession = 0x02;
        public const byte ExtendedDiagnosticSession = 0x03;

        #endregion

        #region Security Access Sub-Functions

        /// <summary>Request Seed Level 1</summary>
        public const byte RequestSeedLevel1 = 0x01;
        /// <summary>Send Key Level 1</summary>
        public const byte SendKeyLevel1 = 0x02;
        /// <summary>Request Seed Level 3 (KNORR EBS5x)</summary>
        public const byte RequestSeedLevel3 = 0x03;
        /// <summary>Send Key Level 3 (KNORR EBS5x)</summary>
        public const byte SendKeyLevel3 = 0x04;

        #endregion

        #region Routine Control Sub-Functions

        public const byte StartRoutine = 0x01;
        public const byte StopRoutine = 0x02;
        public const byte RequestRoutineResults = 0x03;

        #endregion

        #region Input Output Control Sub-Functions

        public const byte ReturnControlToECU = 0x00;
        public const byte ResetToDefault = 0x01;
        public const byte FreezeCurrentState = 0x02;
        public const byte ShortTermAdjustment = 0x03;

        #endregion

        #region Negative Response Codes

        public const byte NegativeResponse = 0x7F;
        public const byte ServiceNotSupported = 0x11;
        public const byte SubFunctionNotSupported = 0x12;
        public const byte IncorrectMessageLength = 0x13;
        public const byte ConditionsNotCorrect = 0x22;
        public const byte RequestSequenceError = 0x24;
        public const byte RequestOutOfRange = 0x31;
        public const byte SecurityAccessDenied = 0x33;
        public const byte InvalidKey = 0x35;
        public const byte ExceededNumberOfAttempts = 0x36;
        public const byte RequiredTimeDelayNotExpired = 0x37;
        public const byte ResponsePending = 0x78;

        #endregion

        #region Data Identifiers (DID) - KNORR EBS5x

        /// <summary>Wheel Speed Sensor Data (WSS)</summary>
        public const ushort DID_WSS = 0xFD00;

        /// <summary>LWS Value</summary>
        public const ushort DID_LWS = 0xFD01;

        /// <summary>Steering Angle</summary>
        public const ushort DID_SteeringAngle = 0xFD02;

        /// <summary>FBM Signal</summary>
        public const ushort DID_FBMSignal = 0xFD03;

        /// <summary>Switch Status</summary>
        public const ushort DID_Switch = 0xFD07;

        /// <summary>Voltage</summary>
        public const ushort DID_Voltage = 0xFD08;

        /// <summary>Air Gap</summary>
        public const ushort DID_AirGap = 0xFDF3;

        /// <summary>Valve Control / FBM Brake Force</summary>
        public const ushort DID_ValveControl = 0xFDF5;

        /// <summary>Lamp Control</summary>
        public const ushort DID_LampControl = 0xFDF7;

        /// <summary>ECU Hardware Number</summary>
        public const ushort DID_EcuHardwareNumber = 0xF191;

        /// <summary>System Supplier ECU Hardware Number</summary>
        public const ushort DID_SystemSupplierHardwareNumber = 0xF192;

        /// <summary>ECU Software Number</summary>
        public const ushort DID_EcuSoftwareNumber = 0xF188;

        /// <summary>ECU Manufacturing Date</summary>
        public const ushort DID_EcuManufacturingDate = 0xF18B;

        /// <summary>ECU Serial Number</summary>
        public const ushort DID_EcuSerialNumber = 0xF18C;

        /// <summary>ECU Configuration</summary>
        public const ushort DID_EcuConfig = 0xF1F2;

        #endregion

        #region Routine Identifiers (RID) - KNORR EBS5x

        /// <summary>Roller Bench Test Mode</summary>
        public const ushort RID_RollerBenchTestMode = 0xFE54;

        /// <summary>FBM Backup</summary>
        public const ushort RID_FBMBackup = 0xFE55;

        /// <summary>SAS Calibration Start</summary>
        public const ushort RID_SASCalibrationStart = 0xFE57;

        /// <summary>SAS Calibration</summary>
        public const ushort RID_SASCalibration = 0xFE58;

        /// <summary>YRA Calibration</summary>
        public const ushort RID_YRACalibration = 0xFE59;

        /// <summary>SAS/YRA Store</summary>
        public const ushort RID_SASYRAStore = 0xFE5A;

        #endregion

        #region Helper Methods

        /// <summary>
        /// 긍정 응답 SID 반환 (요청 SID + 0x40)
        /// </summary>
        public static byte GetPositiveResponseSid(byte requestSid)
        {
            return (byte)(requestSid + 0x40);
        }

        /// <summary>
        /// 응답이 긍정 응답인지 확인
        /// </summary>
        public static bool IsPositiveResponse(byte[] response, byte requestSid)
        {
            if (response == null || response.Length < 1)
                return false;

            return response[0] == GetPositiveResponseSid(requestSid);
        }

        /// <summary>
        /// 응답이 부정 응답인지 확인
        /// </summary>
        public static bool IsNegativeResponse(byte[] response)
        {
            if (response == null || response.Length < 3)
                return false;

            return response[1] == NegativeResponse;
        }

        /// <summary>
        /// Negative Response Code 설명 반환
        /// </summary>
        public static string GetNrcDescription(byte nrc)
        {
            switch (nrc)
            {
                case ServiceNotSupported: return "Service Not Supported";
                case SubFunctionNotSupported: return "Sub-Function Not Supported";
                case IncorrectMessageLength: return "Incorrect Message Length";
                case ConditionsNotCorrect: return "Conditions Not Correct";
                case RequestSequenceError: return "Request Sequence Error";
                case RequestOutOfRange: return "Request Out Of Range";
                case SecurityAccessDenied: return "Security Access Denied";
                case InvalidKey: return "Invalid Key";
                case ExceededNumberOfAttempts: return "Exceeded Number Of Attempts";
                case RequiredTimeDelayNotExpired: return "Required Time Delay Not Expired";
                case ResponsePending: return "Response Pending";
                default: return $"Unknown NRC (0x{nrc:X2})";
            }
        }

        #endregion
    }
}
