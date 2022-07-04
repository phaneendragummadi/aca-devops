using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Arcus.Demo.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigValuesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ConfigValuesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // GET: api/<ConfigValuesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            string vaulturi = _configuration.GetValue<string>("VaultUri");
            return new string[] { vaulturi };
           // return new string[] { "value1", "value2" };
        }

        // GET api/<ConfigValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ConfigValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ConfigValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ConfigValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
