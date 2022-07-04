using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Arcus.Security.Core;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Arcus.Demo.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeyVaultController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ISecretProvider _secretProvider;
        private string _vaulturi;
        public KeyVaultController(ISecretProvider secretProvider,IConfiguration configuration)
        {
            _configuration = configuration;
            _vaulturi = _configuration.GetValue<string>("VaultUri");
            _secretProvider = secretProvider;
        }
        [HttpGet]
        public IEnumerable<string> Get()
        {       
            return new string[] { _vaulturi };
            // return new string[] { "value1", "value2" };
        }
        [HttpGet("{id}")]
        public string Get(string secretname)
        {
            secretname = "storageconnectionstring";
            string secretvalue = _secretProvider.GetSecretAsync(secretname).Result.ToString();
            return secretvalue;
        }
    }
}
