using System;
using System.Net;
using System.Threading.Tasks;
using DevOps.App.Encryption;
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
        private const string encryptionKey= "ENCRYPTED";
        /// <summary>
        ///     Gets the encrypted text that is stored.
        /// </summary>
        [HttpGet]
        [Route("encryption")]
        public async Task<IActionResult> GetEncryptedText()
        {
            try
            {
                var tableStorageRepository = new EncryptedTableStorageRepository();
                var entity = await tableStorageRepository.GetAsync();

                return Ok(entity[encryptionKey]);
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
                //var entity = new EncryptionEntity(encryptedText);

                var tableStorageRepository = new EncryptedTableStorageRepository();
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
                var tableStorageRepository = new EncryptedTableStorageRepository();
                var entity = await tableStorageRepository.GetAsync();
                
                var encryptor = new Encryptor();
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