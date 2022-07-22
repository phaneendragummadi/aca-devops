using DevOps.App.Interfaces;
using System.Threading.Tasks;
using Arcus.Security.Core;

namespace DevOps.App.Models
{
    public class KeyVaultManager : IKeyVaultManager
    {
        private readonly ISecretProvider _secretProvider;

        public KeyVaultManager(ISecretProvider secretProvider)
        {
            _secretProvider = secretProvider;
        }

        public async Task<string> GetSecret(string secretName)
        {
            var secretValue = await _secretProvider.GetRawSecretAsync(secretName);
            return secretValue.ToString();
        }
    }
}
