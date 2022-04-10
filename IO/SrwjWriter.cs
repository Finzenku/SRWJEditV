﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRWJEditV.Utilities;

namespace SRWJEditV.IO
{
    public class SrwjWriter : IDisposable
    {
        private BinaryWriter bw;
        private FileStream fs;
        private Encoding enc;
        
        public SrwjWriter(FileStream fileStream)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            enc = Encoding.GetEncoding(932);
            fs = fileStream;
            bw = new BinaryWriter(fs, enc, true);
        }

        public void WriteData(int address, byte[] data)
        {
            bw.BaseStream.Position = address;
            bw.Write(data);
        }

        public void WriteString(int address, string str)
        {
            bw.BaseStream.Position = address;
            bw.Write(enc.GetBytes(str));
            bw.Write((byte)0);
        }

        public void WriteObject<T>(T Model)
        {

        }

        public void Dispose() => bw.Dispose();

    }
}
