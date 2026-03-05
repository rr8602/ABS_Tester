using System;

namespace ABS_Tester.Models
{
    /// <summary>
    /// ECU 정보 모델
    /// </summary>
    public class EcuInfo
    {
        public string HardwareNumber { get; set; }
        public string SoftwareNumber { get; set; }
        public string SerialNumber { get; set; }
        public string ManufacturingDate { get; set; }
        public string Configuration { get; set; }

        public override string ToString()
        {
            return $"HW: {HardwareNumber}, SW: {SoftwareNumber}, SN: {SerialNumber}";
        }
    }
}
