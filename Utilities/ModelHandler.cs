using SRWJEditV.Attributes;
using SRWJEditV.Extensions;
using SRWJEditV.IO;
using SRWJEditV.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SRWJEditV.Utilities
{
    public class ModelHandler : IDisposable
    {
        private static ModelHandler? Handler;
        private FileStream fs;
        private Encoding enc;
        private Dictionary<Type, IList> modelLists;
        private Dictionary<Type, Dictionary<int, string>> stringLists;
        private SrwjReader sReader;

        private ModelHandler(string filePath)
        {
            modelLists = new Dictionary<Type, IList>();
            stringLists = new Dictionary<Type, Dictionary<int, string>>();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            enc = Encoding.GetEncoding(932);
            fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            sReader = new SrwjReader(fs);

            List<Type> types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttribute(typeof(GameObjectAttribute)) is not null).ToList();
            foreach (Type t in types)
            {
                IList? list = TFactory.CreateList(t);
                var typeConstructor = TFactory.CreateConstructor(t, typeof(byte[]));
                bool isNameable = t.GetInterface(nameof(INameable)) is not null;
                if (list is not null)
                {
                    GameObjectAttribute goa = (GameObjectAttribute)t.GetCustomAttribute(typeof(GameObjectAttribute))!;
                    int add = goa.InitialAddress;
                    int len = goa.DataLength;
                    Dictionary<int, string> stringPointers = new();
                    for (int i = 0; i < goa.ObjectCount; i++)
                    {
                        object o = typeConstructor(sReader.ReadData(add + (i * len), len));
                        t.GetProperty("Index")?.SetValue(o, i);
                        if (o is not null)
                        {
                            if (isNameable)
                            {
                                int[] addresses = ((INameable)o).GetNameAddresses();
                                for (int j = 0; j < addresses.Length; j++)
                                {
                                    if (!stringPointers.ContainsKey(addresses[j]))
                                        stringPointers.Add(addresses[j], sReader.ReadString(addresses[j] & 0xFFFFFF));
                                }
                            }
                            list.Add(o);
                        }
                    }
                    modelLists.Add(t, list);
                    if (isNameable)
                    {
                        int last = stringPointers.GetLastAddress();
                        stringPointers.Add(last + stringPointers.GetByteLimit(last) + 1, string.Empty);
                        stringLists.Add(t, stringPointers);
                    }
                }
            }
        }

        public void Dispose()
        {
            sReader.Dispose();
            fs.Dispose();
        }

        public List<T> GetList<T>()
        {
            Type t = typeof(T);
            List<T> l = new List<T>();
            if (modelLists.ContainsKey(t))
                l = modelLists[t] as List<T>??l;
            return l;
        }

        public Dictionary<int, string> GetPointerDictionary<T>()
        {
            Type t = typeof(T);
            Dictionary<int, string> dict = new();
            if (stringLists.ContainsKey(t))
                dict = stringLists[t];
            return dict;
        }
        public static void SetFilePath(string filePath)
        {
            Handler?.Dispose();
            Handler = File.Exists(filePath) ? new ModelHandler(filePath) : null;
        }
        public static ModelHandler? GetInstance() => Handler;

    }
}
