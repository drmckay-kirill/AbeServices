using System.Threading.Tasks;

namespace AbeServices.TokenGeneration.Services
{
    public interface ITokensService
    {
         Task ProcessTokenRequest(byte[] requestData);
    }
}