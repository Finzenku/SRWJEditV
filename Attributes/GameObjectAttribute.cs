using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRWJEditV.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GameObjectAttribute : Attribute
    {
        public int InitialAddress { get; }
        public int DataLength { get; }
        public int ObjectCount { get; }
        public int[] PointerOffsets { get; }

        public GameObjectAttribute(int initialAddress, int dataLength, int objectCount, int[] pointerOffsets)
        {
            InitialAddress = initialAddress;
            DataLength = dataLength;
            ObjectCount = objectCount;
            PointerOffsets = pointerOffsets;
        }
    }
}
