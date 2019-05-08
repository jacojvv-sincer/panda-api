using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace Panda.API.Controllers
{
    public class AuthenticationBinding
    {
        public string Url { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private static IConfiguration _configuration;
        private static HttpClient _client;

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new HttpClient();
        }

        [HttpPost]
        public async Task<string> Post([FromBody] AuthenticationBinding model)
        {
            var response = await _client.PostAsync($"{model.Url}{_configuration["AzureAdB2C:Secret"]}", null);
            return await response.Content.ReadAsStringAsync();
        }
    }
}