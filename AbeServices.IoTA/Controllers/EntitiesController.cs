using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AbeServices.IoTA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntitiesController : ControllerBase
    {
        [HttpPost]
        public ActionResult<IEnumerable<string>> CreateEntity()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
