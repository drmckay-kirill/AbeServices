using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using AbeServices.AttributeAuthority.Services;

namespace AbeServices.AttributeAuthority.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeysController : ControllerBase
    {
        private readonly IPrivateKeyGenerator _privateKeyGenerator;

        public KeysController(IPrivateKeyGenerator privateKeyGenerator)
        {
            _privateKeyGenerator = privateKeyGenerator;
        }

        [HttpGet("public")]
        public async Task<ActionResult> GetPublickKey()
        {
            var res = await _privateKeyGenerator.GetPublickKey();
            return new FileContentResult(res, "application/octet-stream");
        }

        [HttpPost]
        public async Task<ActionResult> Transfer()
        {
            using (var ms = new MemoryStream())
            {
                await Request.Body.CopyToAsync(ms);
                var authRequest = ms.ToArray();
                var authResponse = await _privateKeyGenerator.Generate(authRequest);
                
                var res = new FileContentResult(authResponse, "application/octet-stream");
                return res;
            }
        }
    }
}