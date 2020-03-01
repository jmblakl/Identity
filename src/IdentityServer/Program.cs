using Core.Framework.Storage.Blob;
using IdentityServer.Stores;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace IdentityServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
           var host = CreateHostBuilder(args).Build();
           await Initialize(host.Services);
           await host.RunAsync();
        }

        private static async Task Initialize(IServiceProvider services)
        {
            var client = services.GetRequiredService<IBlobClient>();
            await ResourceStore.InitializeAsync(client);
            await ClientStore.InitializeAsync(client);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();                    
                });
    }
}