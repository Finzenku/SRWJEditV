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
        private Dictionary<Type, IList<IDataObject>> modelLists;
        private Dictionary<Type, Dictionary<int, string>> stringLists;
        private SrwjReader sReader;
        private SrwjWriter sWriter;

        private ModelHandler(string filePath)
        {
            modelLists = new Dictionary<Type, IList<IDataObject>>();
            stringLists = new Dictionary<Type, Dictionary<int, string>>();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            enc = Encoding.GetEncoding(932);
            fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            sReader = new SrwjReader(fs);
            sWriter = new SrwjWriter(fs);

            List<Type> types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttribute(typeof(GameObjectAttribute)) is not null).ToList();
            foreach (Type t in types)
            {
                GameObjectAttribute goa = (GameObjectAttribute)t.GetCustomAttribute(typeof(GameObjectAttribute))!;
                IList<IDataObject>? list = sReader.ReadDataObjects(t, goa);
                if (list is not null)
                {
                    modelLists.Add(t, list);
                    if (t.GetInterface(nameof(INameable)) is not null)
                    {
                        Dictionary<int, string> stringPointers = sReader.GetNamePointers(list.Cast<INameable>().ToList());
                        int last = stringPointers.GetLastAddress();
                        stringPointers.Add(last + stringPointers.GetByteLimit(last) + 1, string.Empty);
                        stringLists.Add(t, stringPointers);
                    }
                }
            }
        }

        public void Dispose()
        {
            fs.Dispose();
        }

        public List<T> GetList<T>()
        {
            Type t = typeof(T);
            List<T> l = new();
            if (modelLists.ContainsKey(t))
                l = modelLists[t].Cast<T>().ToList();
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

        public void SaveData()
        {
            List<Type> types = modelLists.Keys.ToList();
            foreach (Type t in types)
            {
                GameObjectAttribute goa = (GameObjectAttribute)t.GetCustomAttribute(typeof(GameObjectAttribute))!;
                IList<IDataObject> dataObjs = modelLists[t];

                if (dataObjs is not null)
                {
                    sWriter.WriteDataObjects(dataObjs, goa);
                    sWriter.WriteStringDictionary(stringLists[t]);
                }
            }
        }
        public void SaveAs(string filePath)
        {
            fs.Dispose();
            fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            SaveData();
        }

        public static void SetFilePath(string filePath)
        {
            Handler?.Dispose();
            Handler = File.Exists(filePath) ? new ModelHandler(filePath) : null;
        }
        public static ModelHandler? GetInstance() => Handler;

    }
}
