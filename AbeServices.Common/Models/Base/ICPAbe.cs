using System.Threading.Tasks;

namespace AbeServices.Common.Models.Base
{
    public interface ICPAbe
    {
        Task<SetupResult> Setup();
        Task<ISecretKey> Generate(IMasterKey masterKey, IPublicKey publicKey, IAttributes attributes);
        Task<ICipherText> Encrypt(string message, IPublicKey publicKey, IAccessPolicy accessPolicy);
        Task<string> Decrypt(ICipherText cipherText, IPublicKey publicKey, ISecretKey secretKey);
    }
}