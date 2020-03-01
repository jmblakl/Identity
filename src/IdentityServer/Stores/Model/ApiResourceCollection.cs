using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace IdentityServer.Stores.Model
{
    public sealed class ApiResourceCollection
    {
        public ApiResourceCollection()
        {
            Collection = new List<ApiResource>(0);
        }

        [JsonProperty]
        public ICollection<ApiResource> Collection { get; private set; }


        public ApiResource FindApiResourceByName(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
                
            return Collection
                    .Where(r=> r.ApiId.Equals(name))
                    .FirstOrDefault();
        }

        public IEnumerable<ApiResource> FindApiResourcesByScope(IEnumerable<string> scopeNames) =>    
            Collection.Where(c => scopeNames?.Contains(c.ApiId) == true);        

        public sealed class ApiResource
        {
            public string ApiId { get; set; }
            public string ApiDisplayName { get; set; }
    
        }
    }
}