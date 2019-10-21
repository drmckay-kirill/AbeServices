using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using AbeServices.IoTA.Filters;
using AbeServices.IoTA.Settings;
using System.Net.Http;
using System.Text;
using System.IO;

namespace AbeServices.IoTA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FiwareController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<MainSettings> _options;

        public FiwareController(IOptions<MainSettings> options)
        {
            _options = options;
            _httpClient = new HttpClient();
        }

        [AbeWriteAccessAuthorization]
        [HttpPost("{name}")]
        public async Task<ActionResult> CreateEntity(string name)
        {
            using (var reader = new StreamReader(HttpContext.Request.Body))
            {
                HttpContext.Request.Body.Position = 0;
                var json = await reader.ReadToEndAsync();

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(
                    _options.Value.ContextBrokerUrl, content);
            
                var contextBrokerResult = await response.Content.ReadAsStringAsync();
                return Ok(contextBrokerResult);
            }
        }

        [AbeWriteAccessAuthorization]
        [HttpPatch("{name}")]
        public async Task<ActionResult> EditEntity(string name)
        {
            using (var reader = new StreamReader(HttpContext.Request.Body))
            {
                HttpContext.Request.Body.Position = 0;
                var json = await reader.ReadToEndAsync();

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(
                    $"{_options.Value.ContextBrokerUrl}/{name}/attrs", 
                    content);
            
                var contextBrokerResult = await response.Content.ReadAsStringAsync();
                return Ok(contextBrokerResult);
            }
        }
    }
}