namespace SRWJData.Models
{
    public interface IDataObject
    {
        public byte[] GetData();
        public int Index { get; set; }
    }
}
