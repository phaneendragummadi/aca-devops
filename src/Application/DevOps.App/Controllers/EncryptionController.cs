using System;
using System.Net;
using System.Threading.Tasks;
using DevOps.App.Encryption;
using DevOps.App.Interfaces;
using DevOps.App.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DevOps.App.Controllers
{
    /// <summary>
    ///     Controller that performs encryption and decryption.
    /// </summary>
    [Route("api/v1/[controller]")]
    public class EncryptionController : Controller
    {
        private const string encryptionKey= "ENCRYPTED";
        private readonly IKeyVaultManager _secretManager;
        private readonly IConfiguration _configuration;
        public EncryptionController(IKeyVaultManager secretManager, IConfiguration configuration)
        {
            _secretManager = secretManager;
            _configuration = configuration;
        }
        /// <summary>
        ///     Gets the encrypted text that is stored.
        /// </summary>
        [HttpGet]
        [Route("encryption")]
        public async Task<IActionResult> GetEncryptedText()
        {
            try
            {
                var tableStorageRepository = new EncryptedTableStorageRepository(_configuration);
                var entity = await tableStorageRepository.GetAsync();

                return Ok(entity[encryptionKey]);
            }
            catch (Exception exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, exception.Message);
            }
        }

        private async Task<string> GetKeyVaultSecretAsync(string secretName)
        {
            try
            {
                if (string.IsNullOrEmpty(secretName))
                {
                    throw new ApplicationException("Error: Secret is empty with name " + secretName);
                }
                string secretValue = await
                _secretManager.GetSecret(secretName);
                if (!string.IsNullOrEmpty(secretValue))
                {
                    return secretValue;
                }
                else
                {
                    throw new ApplicationException("Error: Secret not found with name " + secretName);
                }
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Error: Unable to read secret with name " + secretName + " - " + ex.Message);
            }
        }
        /// <summary>
        ///     Updates the encrypted text that is stored.
        /// </summary>
        [HttpPut]
        [Route("encryption")]
        public async Task<IActionResult> UpdateEncryptedText(string text)
        {
            try
            {
                string secretvalue = await GetKeyVaultSecretAsync("EncryptionKey");
                var encryptor = new Encryptor(secretvalue);
                var encryptedText = encryptor.Encrypt(text);

                var tableStorageRepository = new EncryptedTableStorageRepository(_configuration);
                await tableStorageRepository.UpdateAsync(encryptionKey, encryptedText);

                return Ok();
            }
            catch (Exception exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, exception.Message);
            }
        }

        /// <summary>
        ///     Gets the decrypted version of the encrypted text that is stored.
        /// </summary>
        [HttpGet]
        [Route("decryption")]
        public async Task<IActionResult> GetDecryptedText()
        {
            try
            {
                var tableStorageRepository = new EncryptedTableStorageRepository(_configuration);
                var entity = await tableStorageRepository.GetAsync();

                string secretvalue = await GetKeyVaultSecretAsync("EncryptionKey");
                var encryptor = new Encryptor(secretvalue);
                var decryptedText = encryptor.Decrypt(entity[encryptionKey].ToString());

                return Ok(decryptedText);
            }
            catch (Exception exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, exception.Message);
            }
        }
    }
}