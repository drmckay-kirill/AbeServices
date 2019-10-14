using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AbeServices.IoTA.Filters;

namespace AbeServices.IoTA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FiwareController : ControllerBase
    {
        [AbeWriteAccessAuthorization]
        [HttpPost("{name}")]
        public async Task<ActionResult> ProxyData(string name)
        {
            return Ok();
        }
    }
}