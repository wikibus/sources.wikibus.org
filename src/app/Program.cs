using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Brochures.Wikibus.Org
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
            BuildWebHost(args, configuration).Run();
        }

        public static IWebHost BuildWebHost(string[] args, IConfiguration configuration)
        {
            var webHostBuilder = WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(configuration)
                .UseUrls($"http://*:{configuration.GetValue("PORT", 17899)}")
                .UseStartup<Startup>();

            return webHostBuilder
                .Build();
        }
    }
}
