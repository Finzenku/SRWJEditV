using SRWJData.Attributes;
using SRWJData.Extensions;
using System.Text;

namespace SRWJData.Models
{
    [GameObject(initialAddress: 0xD9FC0, dataLength: 0x18, objectCount: 66, pointerOffsets: new int[] { 0, 4, 8, 12, 16 })]
    public class PowerPartModel : INameable, IDataObject
    {
        public int Index { get; set; }
        public int NamePointer { get; set; }
        public int Info1Pointer { get; set; }
        public int Info2Pointer { get; set; }
        public int Info3Pointer { get; set; }
        public int Info4Pointer { get; set; }
        public short SellPrice { get; set; }
        public byte Category { get; set; }

        public PowerPartModel(byte[] data)
        {
            NamePointer = data.GetInt(0);
            Info1Pointer = data.GetInt(4);
            Info2Pointer = data.GetInt(8);
            Info3Pointer = data.GetInt(12);
            Info4Pointer = data.GetInt(16);
            Category = data[20];
            SellPrice = data.GetShort(22);
        }
        public byte[] GetData()
        {
            byte[] n = NamePointer.GetBytes(),
                i1 = Info1Pointer.GetBytes(),
                i2 = Info2Pointer.GetBytes(),
                i3 = Info3Pointer.GetBytes(),
                i4 = Info4Pointer.GetBytes(),
                sp = SellPrice.GetBytes();
            return new byte[24] 
            {
                n[0], n[1], n[2], n[3],
                i1[0], i1[1], i1[2], i1[3],
                i2[0], i2[1], i2[2], i2[3],
                i3[0], i3[1], i3[2], i3[3],
                i4[0], i4[1], i4[2], i4[3],
                Category, 0, sp[0], sp[1]
            };
        }

        public int[] GetNameAddresses()
        {
            return new int[] { NamePointer, Info1Pointer, Info2Pointer, Info3Pointer, Info4Pointer };
        }
    }
}
