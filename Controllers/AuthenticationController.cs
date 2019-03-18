using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MoneyManagerApi.Models;
using System.Net.Http;
using System.Web;

namespace MoneyManagerApi.Controllers
{
    public class AuthenticationBinding
    {
        public string url { get; set; }
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
            var a = _configuration["AzureAdB2C:Secret"];
            var response = await _client.PostAsync($"{model.url}{_configuration["AzureAdB2C:Secret"]}", null);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
