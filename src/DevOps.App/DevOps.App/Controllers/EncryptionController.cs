using System;
using System.Net;
using System.Threading.Tasks;
using DevOps.App.Encryption;
using DevOps.App.Models;
using DevOps.App.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DevOps.App.Controllers
{
    /// <summary>
    ///     Controller that performs encryption and decryption.
    /// </summary>
    [Route("api/v1")]
    public class EncryptionController : Controller
    {
        /// <summary>
        ///     Gets the encrypted text that is stored.
        /// </summary>
        [HttpGet]
        [Route("encryption")]
        public async Task<IActionResult> GetEncryptedText()
        {
            try
            {
                var tableStorageRepository = new TableStorageRepository();
                var entity = await tableStorageRepository.GetAsync();

                return Ok(entity.EncryptedText);
            }
            catch (Exception exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, exception.Message);
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
                var encryptor = new Encryptor();
                var encryptedText = encryptor.Encrypt(text);
                var entity = new EncryptionEntity(encryptedText);

                var tableStorageRepository = new TableStorageRepository();
                await tableStorageRepository.UpdateAsync(entity);

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
                var tableStorageRepository = new TableStorageRepository();
                var entity = await tableStorageRepository.GetAsync();
                
                var encryptor = new Encryptor();
                var decryptedText = encryptor.Decrypt(entity.EncryptedText);

                return Ok(decryptedText);
            }
            catch (Exception exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, exception.Message);
            }
        }
    }
}