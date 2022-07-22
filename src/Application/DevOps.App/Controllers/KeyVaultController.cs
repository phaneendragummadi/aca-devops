using Arcus.Security.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
//example from https://www.loginradius.com/blog/async/guest-post/using-azure-key-vault-with-an-azure-web-app-in-c-sharp/

namespace DevOps.App.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class KeyVaultController : ControllerBase
    {
        private readonly ISecretProvider _secretProvider;
        public KeyVaultController(ISecretProvider secretProvider)
        {
            _secretProvider =secretProvider;
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
                string secretValue = await _secretProvider.GetRawSecretAsync(secretName);
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