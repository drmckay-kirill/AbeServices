using System;
using System.Threading.Tasks;

namespace AbeServices.TokenGeneration.Services
{
    public interface ITokensService
    {
        Task<(byte[], string)> ProcessTokenRequest(byte[] requestData, string inputSessionId);
    }
}