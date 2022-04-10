namespace SRWJEditV.Utilities
{
    public static class LittleEndian
    {
        public static short GetInt16(byte[] bytes)
        {
            short value = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                if (i >= 2) break;
                value += (short) (bytes[i] << (i * 8));
            }
            return value;
        }
        public static int GetInt32(byte[] bytes)
        {
            int value = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                if (i >= 4) break;
                value += bytes[i] << (i * 8);
            }
            return value;
        }
        public static long GetInt64(byte[] bytes)
        {
            long value = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                if (i >= 8) break;
                value += ((long)bytes[i] << (i * 8));
            }
            return value;
        }

        public static byte[] GetBytes(short value)
        {
            return new byte[2] { (byte)(value & 0xFF), (byte)((value >> 2) & 0xFF) };
        }
        public static byte[] GetBytes(int value)
        {
            return new byte[4]
            {
                (byte)(value & 0xFF),
                (byte)((value >> 2) & 0xFF),
                (byte)((value >> 4) & 0xFF),
                (byte)((value >> 6) & 0xFF),
            };
        }
        public static byte[] GetBytes(long value)
        {
            return new byte[8]
            {
                (byte)(value & 0xFF),
                (byte)((value >> 2) & 0xFF),
                (byte)((value >> 4) & 0xFF),
                (byte)((value >> 6) & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)((value >> 10) & 0xFF),
                (byte)((value >> 12) & 0xFF),
                (byte)((value >> 14) & 0xFF),
            };
        }
    }
}
