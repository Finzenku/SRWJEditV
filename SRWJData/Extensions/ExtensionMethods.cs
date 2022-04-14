using System.Text;

namespace SRWJData.Extensions
{
    public static class ExtensionMethods
    {
        public static string UpdateString(this Dictionary<int, string> dict, int address, string str)
        {
            int len = Encoding.GetEncoding(932).GetByteCount(str);
            if (len <= dict.GetStringByteLimit(address))
                return dict[address] = str;
            return dict[address];
        }

        public static int GetStringByteLimit(this Dictionary<int, string> dict, int address)
        {
            if (dict.ContainsKey(address))
            {
                var keys = dict.Keys.ToArray();
                int next = Array.IndexOf(keys, address) + 1;
                if (next >= keys.Length) return Encoding.GetEncoding(932).GetByteCount(dict[address]);
                int nextKey = keys[next];
                return (nextKey - address) - 1;
            }
            return 0;
        }


        public static int GetBytesRemaining(this Dictionary<int, string> dict, int address)
        {
            return dict.GetStringByteLimit(address) - Encoding.GetEncoding(932).GetByteCount(dict[address]);            
        }

        public static int GetCharLimit(this Dictionary<int, string> dict, int address)
        {
            return dict.GetStringByteLimit(address) - Encoding.GetEncoding(932).GetByteCount(dict[address]) + dict[address].Length;
        }

        public static byte[] GetLastCharBytes(this string str) => Encoding.GetEncoding(932).GetBytes(new char[] { str[str.Length-1] });
        public static byte[] GetShiftJISBytes(this string str) => Encoding.GetEncoding(932).GetBytes(str);

    }
}
