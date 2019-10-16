using AbeServices.TokenGeneration.Services;
using Microsoft.AspNetCore.Mvc;


namespace AbeServices.TokenGeneration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokensController : ControllerBase
    {
        private readonly ITokensService _tokensService;

        public TokensController(ITokensService tokensService)
        {
            _tokensService = tokensService;
        }

        [HttpPost]
        public ActionResult GenerateToken()
        {
            

            return Ok();
        }
    }
}
