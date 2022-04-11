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

            List<string> strings = GetStringsFromBytes(ReadData(addresses[0] & 0xFFFFFF, addresses[count - 1] - addresses[0]));
            for (int i = 0; i < strings.Count; i++)
                dict.Add(addresses[i], strings[i]);
            dict.Add(addresses[count-1], ReadString(addresses[count-1]));

            return dict;
        }
        private List<string> GetStringsFromBytes(byte[] data)
        {
            List<string> list = new();
            List<byte> strData = new();
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != 0)
                    strData.Add(data[i]);
                else if (strData.Count > 0)
                {
                    list.Add(enc.GetString(strData.ToArray()));
                    strData.Clear();
                }
            }
            return list;
        }
    }
}
