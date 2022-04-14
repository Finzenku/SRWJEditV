namespace SRWJData.DataHandlers
{
    public interface IModelHandler
    {
        List<T> GetList<T>();
        Dictionary<int, string> GetPointerDictionary<T>();
    }
}