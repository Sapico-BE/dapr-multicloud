using Dapr.Client;
using Dapr.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DaprMulticloudDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            // Create DAPR Client
            var client = new DaprClientBuilder()
                .Build();

            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((services) =>
                {
            // Add the DAPR Client to Host.
            services.AddSingleton<DaprClient>(client);
                })
                .ConfigureAppConfiguration((configBuilder) =>
                {
            // Add the secret store Configuration Provider to the configuration builder.
            configBuilder.AddDaprSecretStore("demosecrets", client);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
