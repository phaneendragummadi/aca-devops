using Azure.Security.KeyVault.Secrets;
using DevOps.App.Encryption;
using Moq;
using System;
using System.Threading;
using Xunit;

namespace DevOps.App.Tests
{
    [Trait("Category", "Unit")]
    public class EntryptorUnitTests
    {
        [Fact]
        public void Encrypt_Value_Text_Returns() 
        {
            KeyVaultSecret keyVaultSecret = SecretModelFactory.KeyVaultSecret(new SecretProperties("<encryptionKey>"), "<encryptionKeyValue>");
            Azure.Response<KeyVaultSecret> response = Azure.Response.FromValue(keyVaultSecret, Mock.Of<Azure.Response>());

            var encryptor = new Encryptor(response.Value.ToString());
           
            var encryptedText = encryptor.Encrypt("Text");
            
            Assert.Equal("<encryptionKeyValue>", encryptedText);
        }
    }
}
