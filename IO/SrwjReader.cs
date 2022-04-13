using SRWJEditV.Attributes;
using SRWJEditV.Models;
using SRWJEditV.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRWJEditV.IO
{
    public class SrwjReader
    {
        private FileStream fs;
        private Encoding enc;
        private int bitwise = 0xFFFFFF;
        
        public SrwjReader(FileStream fileStream)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            enc = Encoding.GetEncoding(932);
            fs = fileStream;
        }
        public string ReadStringFromPointer(int pointerAddress)
        {
            int pointer = LittleEndian.GetInt32(ReadData(pointerAddress, 4));
            return ReadString(pointer);
        }

        public string ReadString(int address)
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

        public byte[] ReadData(int address, int length)
        {
            byte[] data = new byte[length];
            fs.Position = address;
            fs.Read(data);
            return data;
        }

        public IList<IDataObject>? ReadDataObjects(Type t, GameObjectAttribute goa)
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

        public Dictionary<int, string> GetNamePointers(List<INameable> nameables)
        {
            Dictionary<int, string> dict = new();
            List<int> addresses = new();

            foreach (INameable named in nameables)
                addresses.AddRange(named.GetNameAddresses());
            addresses = addresses.Distinct().OrderBy(x => x).ToList();
            int count = addresses.Count;

            byte[] data = ReadData(addresses[0] & bitwise, addresses[count - 1] - addresses[0]);
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
                dict.Add(last, ReadString(last & bitwise));

            return dict;
        }
    }
}
