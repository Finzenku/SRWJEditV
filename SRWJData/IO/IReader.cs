using SRWJData.Attributes;
using SRWJData.Models;

namespace SRWJData.IO
{
    public interface IReader : IDisposable
    {
        Dictionary<int, string> GetNamePointers(List<INameable> nameables);
        IList<IDataObject>? GetDataModels(Type t, GameObjectAttribute goa);
    }
}