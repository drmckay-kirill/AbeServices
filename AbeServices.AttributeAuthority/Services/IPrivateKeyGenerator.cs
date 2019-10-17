using System.Threading.Tasks;

namespace AbeServices.AttributeAuthority.Services
{
    public interface IPrivateKeyGenerator
    {
        Task<byte[]> Generate(byte[] data);
        Task<byte[]> GetPublickKey();
    }
}