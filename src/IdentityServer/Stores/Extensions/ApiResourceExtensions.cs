using ApiResource = IdentityServer.Stores.Model.ApiResourceCollection.ApiResource;
using IdentityApiResource = IdentityServer4.Models.ApiResource;


namespace IdentityServer.Stores.Extensions
{
    public static class ApiResourceExtensions
    {
        public static IdentityApiResource AsIdentityModel(this ApiResource resource)=>
                resource != null 
                ? new IdentityApiResource(resource.ApiId, resource.ApiDisplayName)
                : null;
    }
}