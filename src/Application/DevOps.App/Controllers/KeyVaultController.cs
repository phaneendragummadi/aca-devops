using DevOps.App.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//example from https://www.loginradius.com/blog/async/guest-post/using-azure-key-vault-with-an-azure-web-app-in-c-sharp/

namespace DevOps.App.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class KeyVaultController : ControllerBase
    {
        private readonly IKeyVaultManager _secretManager;
        public KeyVaultController(IKeyVaultManager secretManager)
        {
            _secretManager = secretManager;
        }
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string secretName)
        {
            try
            {
                if (string.IsNullOrEmpty(secretName))
                {
                    return BadRequest();
                }
                string secretValue = await
                _secretManager.GetSecret(secretName);
                if (!string.IsNullOrEmpty(secretValue))
                {
                    return Ok(secretValue);
                }
                else
                {
                    return NotFound("Secret key not found.");
                }
            }
            catch(Exception ex)
            {
                return BadRequest("Error: Unable to read secret - " + ex.Message);
            }
        }
    }
}