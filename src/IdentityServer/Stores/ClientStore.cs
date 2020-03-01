using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Core.Framework.Storage.Blob;
using Core.Framework.Storage.Common;
using IdentityServer.Interfaces;
using IdentityServer.Stores.Extensions;
using IdentityServer.Stores.Model;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer.Stores
{
    public class ClientStore : IClientStore, IClientRepository
    {
        private const string ClientContainer = "clients";

        private readonly IBlobClient blobClient;
        public ClientStore(IBlobClient blobClient)
        {
            this.blobClient = blobClient ?? throw new ArgumentNullException(nameof(blobClient));
        }
        public async Task<Client> FindClientByIdAsync(string clientId) =>
            (await GetClientAsync(this.blobClient, clientId)).AsIdentityModel();

        public async Task<Client> ChangeAllowedApisAsync(string clientId, IEnumerable<string> allowedApis)
        {
            var entity = await UpdateClientAsync(this.blobClient, clientId, (r) => {
                r.AllowedScopes.Clear();
                
                foreach(var api in allowedApis)
                {
                    r.AllowedScopes.Add(api);
                }

                return r;
            });
            
            return entity.AsIdentityModel();
        }

        public async Task<(Client client, string secret)> CreateClientAsync(string instance, string branch, IEnumerable<string> allowedApis)
        {
            Guid clientId = Guid.NewGuid();
            string secret = GenerateSecret();

            ClientEntity entity = new ClientEntity();
            foreach(var grant in GrantTypes.ClientCredentials)
            {
                entity.AllowedGrantTypes.Add(grant);
            }

            foreach(var api in allowedApis)
            {
                entity.AllowedScopes.Add(api);
            }

            entity.ClientId = clientId.ToString();
            entity.ClientSecrets.Add(secret.Sha256());
            entity.AdditionalClaims.Add(new ClientEntity.Claim(){ Type = "Instance", Value = instance });
            entity.AdditionalClaims.Add(new ClientEntity.Claim(){ Type = "Branch", Value = branch });

            var contianer = this.blobClient.GetContainer(ClientContainer);
            await contianer.SetItemAsync(entity.ClientId, entity);

            return (entity.AsIdentityModel(), secret);
        }

        public Task<(Client client, string secret)> ChangeClientSecretAsync(string clientId)
        {
            throw new NotImplementedException();
        }

        public static Task InitializeAsync(IBlobClient blobClient)
        {
            _ = blobClient ?? throw new ArgumentNullException(nameof(blobClient));
            return blobClient.GetContainer(ClientContainer).CreateIfNotExistsAsync();
        }

        private static string GenerateSecret()
        {
            using var rng = new RNGCryptoServiceProvider();
            byte[] keyBytes = new byte[256];
            rng.GetNonZeroBytes(keyBytes);

            return Convert.ToBase64String(keyBytes);
        }

        private static async Task<ClientEntity> GetClientAsync(IBlobClient blobClient, string clientId)
        {
            if(string.IsNullOrWhiteSpace(clientId))
                throw new ArgumentNullException(nameof(clientId));

            _ = blobClient ?? throw new ArgumentNullException(nameof(blobClient));
            var container = blobClient.GetContainer(ClientContainer);

            var file = await container.GetItemAsync<ClientEntity>(clientId);
            return file.Item;
        }   

        private static async Task<ClientEntity> UpdateClientAsync(IBlobClient blobClient, string clientId, Func<ClientEntity, ClientEntity> updateFunc)
        {
            if(string.IsNullOrWhiteSpace(clientId))
                throw new ArgumentNullException(nameof(clientId));

            _ = blobClient ?? throw new ArgumentNullException(nameof(blobClient));
            var container = blobClient.GetContainer(ClientContainer);

            ClientEntity entity = null;
            await container.UpdateItemAsync<ClientEntity>(clientId, (r)=>{
               entity = null;
               var result = updateFunc(r.Item); 
               entity = result;

               return UpdateResult<ClientEntity>.Accept(result);
            });
            
            return entity;
        }
    }
}