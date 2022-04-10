using SRWJEditV.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRWJEditV.Extensions
{
    public static class ExtensionMethods
    {
        public static string UpdateString(this Dictionary<int, string> dict, int address, string str)
        {
            int len = Encoding.GetEncoding(932).GetByteCount(str);
            if (len <= dict.GetByteLimit(address))
                return dict[address] = str;
            return dict[address];
        }

        public static int GetByteLimit(this Dictionary<int, string> dict, int address)
        {
            if (dict.ContainsKey(address))
            {
                var keys = dict.Keys.OrderBy(x => x).ToArray();
                int next = Array.IndexOf(keys, address) + 1;
                if (next >= keys.Length) return Encoding.GetEncoding(932).GetByteCount(dict[address]);
                int nextKey = keys[next];
                return (nextKey - address) - 1;
            }
            return 0;
        }

        public static int GetLastAddress(this Dictionary<int, string> dict) => dict.Keys.OrderByDescending(x => x).ToArray()[0];

        public static int GetBytesRemaining(this Dictionary<int, string> dict, int address)
        {
            return dict.GetByteLimit(address) - Encoding.GetEncoding(932).GetByteCount(dict[address]);            
        }

        public static int GetCharLimit(this Dictionary<int, string> dict, int address)
        {
            return dict.GetByteLimit(address) - Encoding.GetEncoding(932).GetByteCount(dict[address]) + dict[address].Length;
        }

        public static byte[] GetLastCharBytes(this string str) => Encoding.GetEncoding(932).GetBytes(new char[] { str[str.Length-1] });
        public static byte[] GetShiftJISBytes(this string str) => Encoding.GetEncoding(932).GetBytes(str);

        public static bool SetBoolsFromByte(this BindingList<ObservablePair<bool, string>> pairs, byte byt)
        {
            bool changed = false;
            for (int i = 0; i < pairs.Count && i < 8; i++)
            {
                bool flag = ((byt >> i) & 1) == 1 ? true : false;
                if (pairs[i].Item1 != flag)
                {
                    pairs[i].Item1 = flag;
                    changed = true;
                }
            }
            return changed;
        }
        public static byte GetByteFromBools(this BindingList<ObservablePair<bool, string>> pairs)
        {
            bool[] list = pairs.Select(x => x.Item1).ToArray();
            byte value = 0;
            for (int i = 0; i < list.Length && i < 8; i++)
            {
                value += (byte)((list[i] == true ? 1 : 0) << i);
            }
            return value;
        }
    }
}
