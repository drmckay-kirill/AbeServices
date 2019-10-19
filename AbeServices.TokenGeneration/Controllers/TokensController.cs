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
            }

            return Ok();
        }
    }
}
