using SRWJData.Attributes;
using SRWJData.Models;

namespace SRWJData.IO
{
    public interface IWriter : IDisposable
    {
        void WriteDataObjects(IList<IDataObject> objects, GameObjectAttribute goa);
        void WriteStringDictionary(SortedDictionary<int, string> dict);
    }
}