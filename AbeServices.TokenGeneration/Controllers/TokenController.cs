using AbeServices.TokenGeneration.Services;
using Microsoft.AspNetCore.Mvc;


namespace AbeServices.TokenGeneration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost]
        public ActionResult GenerateToken()
        {
            

            return Ok();
        }
    }
}
