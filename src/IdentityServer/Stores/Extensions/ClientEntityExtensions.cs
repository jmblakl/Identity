using System.Linq;
using SystemClaim = System.Security.Claims.Claim;
using IdentityServer4.Models;
using Client = IdentityServer.Stores.Model.ClientEntity;
using IdentityClient = IdentityServer4.Models.Client;

namespace IdentityServer.Stores.Extensions
{
    public static class ClientEntityExtensions
    {
        public static IdentityClient AsIdentityModel(this Client client) => new IdentityClient()
        {
            ClientId = client.ClientId,
            AllowedGrantTypes = client.AllowedGrantTypes,
            ClientSecrets = client.ClientSecrets.Select(s => new Secret(s.Sha256())).ToList(),
            AllowedScopes = client.AllowedScopes,
            Claims = client.AdditionalClaims.Select(c => new SystemClaim(c.Type, c.Value)).ToList()
        };
    }
}