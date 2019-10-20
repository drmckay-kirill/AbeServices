using AbeServices.TokenGeneration.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace AbeServices.TokenGeneration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokensController : ControllerBase
    {
        private static string SessionHeader = "X-Session";
        private readonly ITokensService _tokensService;

        public TokensController(ITokensService tokensService)
        {
            _tokensService = tokensService;
        }

        [HttpPost]
        public async Task<ActionResult> GenerateToken()
        {
            using (var ms = new MemoryStream())
            {
                await Request.Body.CopyToAsync(ms);
            
                Request.Headers.TryGetValue(SessionHeader, out var sessionIdValue);
                var inputSessionId = sessionIdValue.Count > 0 ? sessionIdValue[0] : null;

                var (protocolResult, outputSessionId) = await _tokensService.ProcessTokenRequest(ms.ToArray(), inputSessionId);
                if (protocolResult != null)
                {
                    Response.Headers.Add(SessionHeader, outputSessionId);
                    return new FileContentResult(protocolResult, "application/octet-stream");
                }
            }

            return BadRequest();
        }
    }
}
