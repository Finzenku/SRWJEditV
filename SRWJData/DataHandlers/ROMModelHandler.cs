using SRWJData.Attributes;
using SRWJData.Models;
using System.Reflection;
using System.Text;
using SRWJData.Extensions;
using SRWJData.IO;

namespace SRWJData.DataHandlers
{
    public class ROMModelHandler : IModelHandler
    {
        private List<Type> _types;
        private Encoding enc;
        private Dictionary<Type, IList<IDataObject>> modelLists;
        private Dictionary<Type, SortedDictionary<int, string>> stringLists;
        private ROMReader sReader;
        private ROMWriter sWriter;
        private string _filePath;
        private int stringMemoryOffset;

        internal ROMModelHandler(string filePath)
        {
            _filePath = filePath;
            stringMemoryOffset = 0x08000000;
            modelLists = new Dictionary<Type, IList<IDataObject>>();
            stringLists = new Dictionary<Type, SortedDictionary<int, string>>();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            enc = Encoding.GetEncoding(932);

            _types = Assembly.GetCallingAssembly().GetTypes().Where(t => t.GetCustomAttribute(typeof(GameObjectAttribute)) is not null).ToList();
            using (sReader = new ROMReader(filePath, enc, stringMemoryOffset))
            {
                foreach (Type t in _types)
                {
                    GameObjectAttribute goa = (GameObjectAttribute)t.GetCustomAttribute(typeof(GameObjectAttribute))!;
                    IList<IDataObject>? list = sReader.GetDataModels(t, goa);
                    if (list is not null)
                    {
                        modelLists.Add(t, list);
                        if (t.GetInterface(nameof(INameable)) is not null)
                        {
                            SortedDictionary<int, string> stringPointers = sReader.GetNamePointers(list.Cast<INameable>().ToList());
                            int last = stringPointers.Last().Key;
                            stringPointers.Add(last + stringPointers.GetStringByteLimit(last) + 1, string.Empty);
                            stringLists.Add(t, stringPointers);
                        }
                    }
                }
            }
        }

        public List<T> GetList<T>()
        {
            Type t = typeof(T);
            List<T> l = new();
            if (modelLists.ContainsKey(t))
                l = modelLists[t].Cast<T>().ToList();
            return l;
        }

        public SortedDictionary<int, string> GetPointerDictionary<T>()
        {
            Type t = typeof(T);
            SortedDictionary<int, string> dict = new();
            if (stringLists.ContainsKey(t))
                dict = stringLists[t];
            return dict;
        }

        public void SaveData()
        {
            List<Type> types = modelLists.Keys.ToList();
            using (sWriter = new ROMWriter(_filePath, enc, stringMemoryOffset))
            {
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
        }
        public void SaveAs(string filePath)
        {
            _filePath = filePath;
            SaveData();
        }

        internal void SetFilePath(string filePath) => _filePath = filePath;

    }
}
