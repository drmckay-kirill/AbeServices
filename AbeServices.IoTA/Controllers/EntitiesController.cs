using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AbeServices.IoTA.Models;
using AbeServices.IoTA.Services;

namespace AbeServices.IoTA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntitiesController : ControllerBase
    {
        private readonly IEntityService _entityService;

        public EntitiesController(IEntityService entityService)
        {
            _entityService = entityService;
        }

        [HttpPost]
        public async Task<ActionResult> CreateEntity([FromBody] EntityViewModel entityViewModel)
        {
            await _entityService.Create(entityViewModel);
            return CreatedAtAction(nameof(GetEntity), new { name = entityViewModel.Name }, entityViewModel);
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<EntityViewModel>> GetEntity(string name)
        {
            return await _entityService.Get(name);
        }
    }
}
