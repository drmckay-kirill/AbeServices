using System;
using System.Threading.Tasks;

namespace AbeServices.IoTA.Services
{
    public interface IFiwareService
    {
        Task<(byte[], Guid)> Authorize(string entityName, string sessionId, byte[] body, byte[] hmacHeader, bool read);  
    }
}