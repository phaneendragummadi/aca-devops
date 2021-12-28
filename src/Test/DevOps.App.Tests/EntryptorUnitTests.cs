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
            KeyVaultSecret keyVaultSecret = SecretModelFactory.KeyVaultSecret(new SecretProperties("EncryptionKey"), "EC545C08-43AF-4B49-965F-75B27300CFA0");
            Azure.Response<KeyVaultSecret> response = Azure.Response.FromValue(keyVaultSecret, Mock.Of<Azure.Response>());

            var encryptor = new Encryptor(response.Value.ToString());
           
            var encryptedText = encryptor.Encrypt("Text");
            
            Assert.Equal("R5IvW8qLrOo4ChFmjRzc+Q==", encryptedText);
        }
    }
}
