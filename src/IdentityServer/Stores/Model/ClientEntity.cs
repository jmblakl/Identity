using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace IdentityServer.Stores.Model
{
    public class ClientEntity
    {
        public ClientEntity() 
        {
            AllowedGrantTypes = new List<string>();
            ClientSecrets = new List<string>();
            AllowedScopes = new List<string>();
            AdditionalClaims = new List<Claim>();
        }

        [JsonProperty]
        public string ClientId { get; set; }

        [JsonProperty]
        public ICollection<string> AllowedScopes { get; private set; }

        [JsonProperty]
        public ICollection<string> ClientSecrets { get; private set; }


        [JsonProperty]
        public ICollection<string> AllowedGrantTypes { get; private set; }

        [JsonProperty]
        public ICollection<Claim> AdditionalClaims { get; private set; }

        public sealed class Claim 
        {
            [JsonProperty]
            public string Type { get; set; }
            [JsonProperty]
            public string Value { get; set; }
        }
    }
}