using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AbeServices.Common.Models.Protocols;
using AbeServices.Common.Protocols;
using AbeServices.KeyService.Settings;
using AbeServices.Common.Exceptions;

namespace AbeServices.KeyService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeysController : ControllerBase
    {
        private readonly IKeyDistributionBuilder _builder;
        private readonly IOptions<MainSettings> _settings;
        private readonly HttpClient _httpClient;

        public KeysController(IKeyDistributionBuilder builder,
            IOptions<MainSettings> settings)
        {
            _builder = builder;
            _settings = settings;
            _httpClient = new HttpClient();
        }

        [HttpPost]
        public async Task<ActionResult> Transfer()
        {
            using (var ms = new MemoryStream())
            {
                await Request.Body.CopyToAsync(ms);
                var abonentRequest = ms.ToArray();

                var deserializedRequest = _builder.GetStepData<KeyDistrubutionStepOne>(abonentRequest);

                if (deserializedRequest.AttributeAuthorityId != _settings.Value.Authority)
                    throw new ProtocolArgumentException("Incorrect attribute authority id");

                var (authRequest, nonce) = _builder.BuildStepTwo(_settings.Value.SharedKey, 
                    deserializedRequest.AbonentId, 
                    _settings.Value.Name, 
                    _settings.Value.Authority, 
                    deserializedRequest.Attributes, 
                    deserializedRequest.Payload);

                var authRequestContent = new ByteArrayContent(authRequest);
                var authResponse = await _httpClient.PostAsync(_settings.Value.AuthorityUrl, authRequestContent);
            
                var authResponseData = await authResponse.Content.ReadAsByteArrayAsync();

                var deserializedAuthResponse = _builder.GetStepData<KeyDistributionStepThree>(authResponseData);
                var authPayload = _builder.GetPayload<KeyDistributionAuthToService>(deserializedAuthResponse.ServicePayload, 
                                        _settings.Value.SharedKey);

                if (authPayload.Nonce != nonce)
                    throw new ProtocolArgumentException("Incorrect nonce has been received");

                var res = new FileContentResult(deserializedAuthResponse.AbonentPayload, "application/octet-stream");
                return res;
            }
        }
    }
}
