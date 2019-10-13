using System.Threading.Tasks;
using AbeServices.IoTA.Models;

namespace AbeServices.IoTA.Services
{
    public interface IEntityService
    {
        Task Create(EntityViewModel entityViewModel);
        Task<EntityViewModel> Get(string name);
    }
}