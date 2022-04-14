namespace SRWJData.DataHandlers
{
    public interface IModelHandler
    {
        List<T> GetList<T>();
        SortedDictionary<int, string> GetPointerDictionary<T>();
    }
}