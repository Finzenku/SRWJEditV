using System.Text;
using SRWJData.Models;
using SRWJData.Attributes;

namespace SRWJData.IO
{
    public class ROMWriter : IWriter
    {
        private FileStream fs;
        private Encoding enc;
        private int strOffset;
        private string _filePath;

        public ROMWriter(string filePath, Encoding encoding, int stringAdrOffset)
        {
            enc = encoding;
            strOffset = stringAdrOffset;
            _filePath = filePath;
            fs = new FileStream(_filePath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
        }

        private void WriteData(int address, byte[] data)
        {
            fs.Position = address;
            fs.Write(data);
        }
        public void WriteDataObjects(IList<IDataObject> objects, GameObjectAttribute goa)
        {
            int add = goa.InitialAddress;
            int len = goa.DataLength;
            int count = goa.ObjectCount;
            byte[] data = new byte[count * len];

            for (int i = 0; i < goa.ObjectCount; i++)
            {
                byte[] objData = objects[i].GetData();
                Buffer.BlockCopy(objData, 0, data, i*len, objData.Length);
            }
            WriteData(add, data);
        }

        private void WriteString(int address, string str)
        {
            fs.Position = address;
            fs.Write(enc.GetBytes(str));
            WriteEmptyByte();
        }

        public void WriteStringDictionary(SortedDictionary<int, string> dict)
        {
            using (fs = new FileStream(_filePath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
            {
                List<int> orderedKeys = dict.Keys.OrderBy(x => x).ToList();
                int count = orderedKeys.Count;
                int firstAdd = orderedKeys[0];
                int size = orderedKeys[count-1] - firstAdd + enc.GetByteCount(dict[orderedKeys.Last()]);
                byte[] data = new byte[size];
                byte[] strBytes;
                int key = 0, next = 0;
                int oldOff = firstAdd, newOff = 0;
                int extraBytes = 0;
                for (int i = 0; i < count - 1; i++)
                {
                    key = orderedKeys[i];
                    strBytes = enc.GetBytes(dict[key]);
                    Buffer.BlockCopy(strBytes, 0, data, key - firstAdd, strBytes.Length);
                    extraBytes = (orderedKeys[i + 1] - key) - strBytes.Length;
                    newOff = key + strBytes.Length;
                    next = newOff - oldOff;
                    if (extraBytes > 5 || i == count - 2)
                    {
                        WriteData(oldOff - strOffset, data[(oldOff-firstAdd)..(newOff-firstAdd)]);
                        if (newOff-firstAdd < data.Length -1)
                            WriteEmptyByte();
                        oldOff = orderedKeys[i + 1];
                    }
                }
            }
        }

        private void WriteEmptyByte() => fs.Write(new byte[1] { 0 });

        public void Dispose()
        {
            fs.Dispose();
        }
    }
}
