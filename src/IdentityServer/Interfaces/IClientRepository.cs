using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace IdentityServer.Interfaces
{
    public interface IClientRepository
    {
         Task<Client> ChangeAllowedApisAsync(string clientId, IEnumerable<string> allowedApis);
         Task<(Client client, string secret)> CreateClientAsync(string instance, string branch, IEnumerable<string> allowedApis);
         Task<(Client client, string secret)> ChangeClientSecretAsync(string clientId);
    }
}