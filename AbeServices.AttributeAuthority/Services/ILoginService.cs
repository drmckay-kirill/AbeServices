using AbeServices.AttributeAuthority.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbeServices.AttributeAuthority.Services
{
    public interface ILoginService
    {
        Task<Login> GetLogin(string name);
        Task<List<Login>> GetList();
        Task Create(Login login);
    }
}