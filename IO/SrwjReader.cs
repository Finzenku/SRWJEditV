using SRWJEditV.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRWJEditV.IO
{
    public class SrwjReader : IDisposable
    {
        private BinaryReader br;
        private FileStream fs;
        private Encoding enc;
        
        public SrwjReader(FileStream fileStream)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            enc = Encoding.GetEncoding(932);
            fs = fileStream;
            br = new BinaryReader(fs, enc, true);
        }
        public string ReadStringFromPointer(int pointerAddress)
        {
            int pointer = LittleEndian.GetInt32(ReadData(pointerAddress, 4));
            return ReadString(pointer);
        }

        public string ReadString(int address)
        {
            br.BaseStream.Position = address;
            return ReadNullTerminatedString();
        }
        private string ReadNullTerminatedString()
        {
            List<byte> data = new List<byte>();
            byte ch;
            while ((ch = br.ReadByte()) != 0)
                data.Add(ch);
            return enc.GetString(data.ToArray());
        }

        public byte[] ReadData(int address, int length)
        {
            byte[] data = new byte[length];
            br.BaseStream.Position = address;
            br.Read(data);
            return data;
        }

        public void Dispose() => br.Dispose();
    }
}
