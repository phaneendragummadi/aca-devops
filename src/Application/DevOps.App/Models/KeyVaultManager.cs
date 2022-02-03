using Azure.Security.KeyVault.Secrets;
using DevOps.App.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOps.App.KeyVault
{
    public class KeyVaultManager : IKeyVaultManager
    {
        private readonly SecretClient _secretClient;

        public KeyVaultManager(SecretClient secretClient)
        {
            _secretClient = secretClient;
        }

        public async Task<string> GetSecret(string secretName)
        {
            try
            {
                KeyVaultSecret keyValueSecret = await
                _secretClient.GetSecretAsync(secretName);
                return keyValueSecret.Value;
            }
            catch
            {
                throw;
            }
        }
    }
}
