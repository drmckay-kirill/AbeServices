using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AbeServices.Common.Models.Protocols;
using AbeServices.Common.Protocols;

namespace AbeServices.KeyService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeysController : ControllerBase
    {
        private readonly IKeyDistributionBuilder _builder;

        public KeysController(IKeyDistributionBuilder builder)
        {
            _builder = builder;
        }

        [HttpGet]
        public ActionResult<byte[]> Get(byte[] abonentRequest)
        {
            var deserializedRequest = _builder.GetStepData<KeyDistrubutionStepOne>(abonentRequest);

            return Ok();
        }
    }
}
