using SRWJEditV.Models;
using System.ComponentModel;
using System.Linq;

namespace SRWJEditV.Extensions
{
    public static class ExtensionMethods
    {
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
