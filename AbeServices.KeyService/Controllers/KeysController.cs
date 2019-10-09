using System;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AbeServices.Common.Models.Protocols;
using AbeServices.Common.Protocols;
using AbeServices.KeyService.Settings;

namespace AbeServices.KeyService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeysController : ControllerBase
    {
        private readonly IKeyDistributionBuilder _builder;
        private readonly IOptions<MainSettings> _settings;
        private readonly HttpClient httpClient;

        public KeysController(IKeyDistributionBuilder builder,
            IOptions<MainSettings> settings)
        {
            _builder = builder;
            _settings = settings;
            httpClient = new HttpClient();
        }

        [HttpPost]
        public async Task<ActionResult<byte[]>> Transfer()
        {
            using (var ms = new MemoryStream())
            {
                await Request.Body.CopyToAsync(ms);
                var abonentRequest = ms.ToArray();

                var deserializedRequest = _builder.GetStepData<KeyDistrubutionStepOne>(abonentRequest);

                if (deserializedRequest.AttributeAuthorityId != _settings.Value.Authority)
                    throw new ArgumentException("Invalid attribute authority id");

                var authRequest = _builder.BuildStepTwo(_settings.Value.SharedKey, 
                    deserializedRequest.AbonentId, 
                    _settings.Value.Name, 
                    _settings.Value.Authority, 
                    deserializedRequest.Attributes, 
                    deserializedRequest.Payload);

            }

            return Ok();
        }
    }
}
