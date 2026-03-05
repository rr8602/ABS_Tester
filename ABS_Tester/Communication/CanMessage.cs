using System;

namespace ABS_Tester.Communication
{
    /// <summary>
    /// CAN 메시지 클래스
    /// </summary>
    public class CanMessage
    {
        public uint Id { get; set; }
        public byte[] Data { get; set; }
        public DateTime Timestamp { get; set; }

        public CanMessage()
        {
            Timestamp = DateTime.Now;
            Data = new byte[0];
        }

        public CanMessage(uint id, byte[] data)
        {
            Id = id;
            Data = data ?? new byte[0];
            Timestamp = DateTime.Now;
        }

        public string ToHexString()
        {
            return VrtDevice.BytesToHex(Data);
        }

        public override string ToString()
        {
            return $"[{Id:X8}] {ToHexString()}";
        }
    }
}
