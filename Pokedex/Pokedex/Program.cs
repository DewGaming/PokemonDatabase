using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Pokedex
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var root = config.Build();
                    var vault = root["KeyVault:Vault"];
                    if(!string.IsNullOrEmpty(vault))
                    {
                        config.AddAzureKeyVault(
                            $"https://{root["KeyVault:Vault"]}.vault.azure.net/",
                            root["KeyVault:ClientId"],
                            root["KeyVault:ClientSecret"]
                        );
                    }
                })
                .UseStartup<Startup>();
    }
}
