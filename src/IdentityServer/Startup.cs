using Core.Framework.Storage.Blob;
using IdentityServer.Stores;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {            
            services.AddSingleton<BlobContext>(new BlobContext(Configuration.GetConnectionString("StorageConnectionString")));
            services.AddTransient<IBlobClient, BlobClient>();
            services.AddTransient<IResourceStore, ResourceStore>();
            services.AddTransient<IClientStore, ClientStore>();
            var builder = services.AddIdentityServer();                                                    
            builder.AddDeveloperSigningCredential();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();
        }
    }
}
