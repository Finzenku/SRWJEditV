using SRWJData.Attributes;
using SRWJData.Extensions;
using SRWJData.Models;
using SRWJData.Utilities;
using System.Text;

namespace SRWJData.IO
{
    public class ROMReader : IReader
    {
        private FileStream fs;
        private Encoding enc;
        private int strOffset;
        private string _filePath;

        public ROMReader(string filePath, Encoding encoding, int stringAdrOffset)
        {
            enc = encoding;
            strOffset = stringAdrOffset;
            _filePath = filePath;
            fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
        private string ReadStringFromPointer(int pointerAddress)
        {
            int pointer = ReadData(pointerAddress, 4).GetInt();
            return ReadString(pointer);
        }

        private string ReadString(int address)
        {
            fs.Position = address;
            return ReadNullTerminatedString();
        }
        private string ReadNullTerminatedString()
        {
            List<byte> data = new List<byte>();
            byte ch;
            while ((ch = (byte)fs.ReadByte()) != 0)
                data.Add(ch);
            return enc.GetString(data.ToArray());
        }

        private byte[] ReadData(int address, int length)
        {
            byte[] data = new byte[length];
            fs.Position = address;
            fs.Read(data);
            return data;
        }

        public IList<IDataObject>? GetDataModels(Type t, GameObjectAttribute goa)
        {
            IList<IDataObject>? list = TFactory.CreateList(t)!.Cast<IDataObject>().ToList();
            if (list is not null)
            {
                int add = goa.InitialAddress;
                int len = goa.DataLength;
                int count = goa.ObjectCount;
                var typeConstructor = TFactory.CreateConstructor(t, typeof(byte[]));
                byte[] data = ReadData(add, len * count);
                for (int i = 0; i < count; i++)
                {
                    IDataObject obj = (IDataObject)typeConstructor(data[(i*len)..((i+1)*len)]);
                    obj.Index = i;
                    list.Add(obj);
                }
            }
            return list;
        }

        public SortedDictionary<int, string> GetNamePointers(List<INameable> nameables)
        {
            SortedDictionary<int, string> dict = new();
            List<int> addresses = new();

            foreach (INameable named in nameables)
                addresses.AddRange(named.GetNameAddresses());
            addresses = addresses.Distinct().OrderBy(x => x).ToList();
            int count = addresses.Count;

            byte[] data = ReadData(addresses[0] - strOffset, addresses[count - 1] - addresses[0]);
            int start = addresses[0];
            int offset = 0;
            byte b;
            List<byte> strData = new();
            for (int i = 0; i < addresses.Count - 1; i++)
            {
                if (!dict.ContainsKey(addresses[i]))
                {
                    offset = addresses[i] - start;
                    while (offset < data.Length && (b = data[offset]) != 0)
                    {
                        strData.Add(b);
                        offset++;
                    }
                    if (strData.Count > 0)
                        dict.Add(addresses[i], enc.GetString(strData.ToArray()));
                    strData.Clear();
                }
            }
            int last = addresses[addresses.Count-1];
            if (!dict.ContainsKey(last))
                dict.Add(last, ReadString(last - strOffset));

            return dict;
        }

        public void Dispose()
        {
            fs.Dispose();
        }
    }
}
