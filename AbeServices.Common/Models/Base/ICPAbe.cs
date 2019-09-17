using System.Threading.Tasks;

namespace AbeServices.Common.Models.Base
{
    public interface ICPAbe
    {
        Task<SetupResult> Setup();
        ISecretKey Generate(IMasterKey masterKey, IPublicKey publicKey, IAttributes attributes);
        ICipherText Encrypt(string message, IPublicKey publicKey, IAccessPolicy accessPolicy);
        string Decrypt(ICipherText cipherText, ISecretKey secretKey);
    }
}