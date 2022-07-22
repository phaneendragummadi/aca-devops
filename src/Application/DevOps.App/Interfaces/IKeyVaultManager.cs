using System.Threading.Tasks;

namespace DevOps.App.Interfaces
{
    public interface IKeyVaultManager
    {
        public Task<string> GetSecret(string secretName);
    }
}
