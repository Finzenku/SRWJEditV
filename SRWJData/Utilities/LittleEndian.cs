namespace SRWJData.Utilities
{
    public static class LittleEndian
    {
        public static short GetInt16(byte[] bytes)
        {
            short value = 0;
            for (int i = 0; i < bytes.Length && i < 2; i++)
            {
                value += (short) (bytes[i] << (i * 8));
            }
            return value;
        }
        public static int GetInt32(byte[] bytes)
        {
            int value = 0;
            for (int i = 0; i < bytes.Length && i < 4; i++)
            {
                value += bytes[i] << (i * 8);
            }
            return value;
        }
        public static long GetInt64(byte[] bytes)
        {
            long value = 0;
            for (int i = 0; i < bytes.Length && i < 8; i++)
            {
                value += ((long)bytes[i] << (i * 8));
            }
            return value;
        }

        public static byte[] GetBytes(short value)
        {
            return new byte[2] { (byte)(value & 0xFF), (byte)((value >> 8) & 0xFF) };
        }
        public static byte[] GetBytes(int value)
        {
            return new byte[4]
            {
                (byte)(value & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 24) & 0xFF),
            };
        }
        public static byte[] GetBytes(long value)
        {
            return new byte[8]
            {
                (byte)(value & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 32) & 0xFF),
                (byte)((value >> 40) & 0xFF),
                (byte)((value >> 48) & 0xFF),
                (byte)((value >> 56) & 0xFF),
            };
        }
    }
}
