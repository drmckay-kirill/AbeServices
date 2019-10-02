namespace AbeServices.Common.Helpers
{
    public interface IDataSerializer
    {
        byte[] Serialize<T>(T data);
    }
}