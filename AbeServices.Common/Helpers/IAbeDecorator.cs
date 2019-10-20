using System.Threading.Tasks;

namespace AbeServices.Common.Helpers
{
    public interface IAbeDecorator
    {
        Task Setup();
        Task<byte[]> Encrypt(byte[] data, string[] accessStructure);
        Task<byte[]> Decrypt(byte[] data);
    }
}