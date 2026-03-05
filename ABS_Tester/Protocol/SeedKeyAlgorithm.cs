using System;

namespace ABS_Tester.Protocol
{
    /// <summary>
    /// KNORR EBS5x Seed/Key 알고리즘
    /// KI1A-ABST mod_EBS5.bas Ret_1SeedKey 함수 기반
    /// </summary>
    public static class SeedKeyAlgorithm
    {
        /// <summary>
        /// KNORR EBS5x Security Access Key 계산
        /// </summary>
        /// <param name="seed">4바이트 Seed 값</param>
        /// <returns>4바이트 Key 값</returns>
        public static byte[] CalculateKey(byte[] seed)
        {
            if (seed == null || seed.Length != 4)
                throw new ArgumentException("Seed must be 4 bytes");

            // Mask 값 (VB 코드 기준)
            byte[] mask = { 0xD0, 0x20, 0x0D, 0x62 };
            int numShifts = 21;

            // Key 초기화 (Seed 복사)
            byte[] key = new byte[4];
            Array.Copy(seed, key, 4);

            // 알고리즘 수행
            for (int cnt = 1; cnt <= numShifts; cnt++)
            {
                int stCarry = 0;

                for (int ct = 0; ct <= 3; ct++)
                {
                    int idx = 3 - ct;

                    if (stCarry == 1)
                    {
                        stCarry = (key[idx] >= 128) ? 1 : 0;

                        int temp = (key[idx] * 2) + 1;
                        key[idx] = (byte)(temp > 255 ? temp - 256 : temp);
                    }
                    else
                    {
                        stCarry = (key[idx] >= 128) ? 1 : 0;

                        int temp = key[idx] * 2;
                        key[idx] = (byte)(temp > 255 ? temp - 256 : temp);
                    }
                }

                if (stCarry == 1)
                {
                    for (int ct = 0; ct <= 3; ct++)
                    {
                        key[ct] = (byte)(key[ct] ^ mask[ct]);
                    }
                }
            }

            return key;
        }

        /// <summary>
        /// Seed/Key 계산 및 결과 문자열 반환 (디버그용)
        /// </summary>
        public static string CalculateKeyWithLog(byte[] seed)
        {
            byte[] key = CalculateKey(seed);
            return $"SEED: {seed[0]:X2} {seed[1]:X2} {seed[2]:X2} {seed[3]:X2} -> " +
                   $"KEY: {key[0]:X2} {key[1]:X2} {key[2]:X2} {key[3]:X2}";
        }
    }
}
