using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Framework.Storage.Blob;
using IdentityServer.Stores.Model;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using IdentityServer.Stores.Extensions;

namespace IdentityServer.Stores
{
    public sealed class ResourceStore : IResourceStore
    {
        private const string ApiContainer = "apis";
        private const string ApiFile = "apis.json";

        private readonly ILogger<ResourceStore> logger;
        private readonly IBlobClient blobClient;


        public ResourceStore(IBlobClient blobClient, ILogger<ResourceStore> logger)
        {
            this.blobClient = blobClient ?? throw new ArgumentNullException(nameof(blobClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private IEnumerable<IdentityResource> IdResources => new IdentityResource[]
        {
            new IdentityResources.OpenId()
        };

        public async Task<ApiResource> FindApiResourceAsync(string name)
        {
            var collection = await GetApiCollectionAsync(this.blobClient);
            return collection.FindApiResourceByName(name).AsIdentityModel();
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var collection = await GetApiCollectionAsync(this.blobClient);
            return collection.FindApiResourcesByScope(scopeNames).Select(r => r.AsIdentityModel());
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var set = new HashSet<string>(scopeNames ?? Enumerable.Empty<string>());
            var ids = IdResources.Where(i => set.Contains(i.Name));
            return Task.FromResult(ids);
        }

        public async Task<Resources> GetAllResourcesAsync()
        {
            var collection = await GetApiCollectionAsync(this.blobClient);
            return new Resources(
                IdResources,
                collection
                    .Collection
                    .Select(r => r.AsIdentityModel())
            );
        }

        public static Task InitializeAsync(IBlobClient blobClient)
        {
            _ = blobClient ?? throw new ArgumentNullException(nameof(blobClient));
            return blobClient.GetContainer(ApiContainer).CreateIfNotExistsAsync();
        }

        private static async Task<ApiResourceCollection> GetApiCollectionAsync(IBlobClient blobClient)
        {
            _ = blobClient ?? throw new ArgumentNullException(nameof(blobClient));
            var container = blobClient.GetContainer(ApiContainer);

            var file = await container.GetItemAsync<ApiResourceCollection>(ApiFile);
            return file.Item;
        }
    }
}