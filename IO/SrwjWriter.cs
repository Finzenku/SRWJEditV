using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using SRWJEditV.Models;
using SRWJEditV.Attributes;

namespace SRWJEditV.IO
{
    public class SrwjWriter
    {
        private FileStream fs;
        private Encoding enc;
        
        public SrwjWriter(FileStream fileStream)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            enc = Encoding.GetEncoding(932);
            fs = fileStream;
        }

        public void WriteData(int address, byte[] data)
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

        public void WriteString(int address, string str)
        {
            fs.Position = address;
            fs.Write(enc.GetBytes(str));
            fs.Write(new byte[1] { 0 });
        }

        public void WriteStringDictionary(Dictionary<int, string> dict)
        {
            List<int> orderedKeys = dict.Keys.OrderBy(x => x).ToList();
            int count = orderedKeys.Count;
            int firstAdd = orderedKeys[0];
            int size = orderedKeys[count-1] - firstAdd + enc.GetByteCount(dict[orderedKeys.Last()]);
            byte[] data = new byte[size];
            byte[] strBytes;
            int key = 0;
            int extraBytes = 0;
            for (int i = 0; i < count - 1; i++)
            {
                key = orderedKeys[i];
                strBytes = enc.GetBytes(dict[key]);
                Buffer.BlockCopy(strBytes, 0, data, key - firstAdd, strBytes.Length);
                extraBytes = (orderedKeys[i + 1] - orderedKeys[i]) - strBytes.Length;
                if (extraBytes > 0)
                    Buffer.BlockCopy(new byte[extraBytes], 0, data, (key - firstAdd) + strBytes.Length, extraBytes);
            }
            strBytes = enc.GetBytes(dict[orderedKeys[count-1]]);
            Buffer.BlockCopy(strBytes, 0, data, orderedKeys[count-1] - orderedKeys[0], strBytes.Length);

            WriteData(firstAdd & 0xFFFFFF, data);

        }
    }
}
