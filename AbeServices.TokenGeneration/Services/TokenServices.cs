using System.Threading.Tasks;
using AbeServices.Common.Protocols;
using AbeServices.Common.Models.Protocols;

namespace AbeServices.TokenGeneration.Services
{
    public class TokensService : ITokensService
    {
        private readonly IAbeAuthBuilder _abeAuthBuilder;

        public TokensService(IAbeAuthBuilder abeAuthBuilder)
        {
            _abeAuthBuilder = abeAuthBuilder;
        }

        public async Task ProcessTokenRequest(byte[] requestData)
        {
            var request = _abeAuthBuilder.GetStepData<AbeAuthStepTwo>(requestData);
        }
    }
}