using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Wikibus.Sources.Functions.Dependencies
{
    public static class GoogleDependenciesExtensions
    {
        public static void RegisterGoogleDrive(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.IsDevelopment())
            {
                services.AddSingleton(GoogleCredential
                    .FromFile(configuration["Google:TokenFile"])
                    .CreateScoped("https://www.googleapis.com/auth/drive"));
            }
            else
            {
                var secrets = new SecretClient(
                    new Uri("https://wikibus.vault.azure.net/"),
                    new DefaultAzureCredential());
                var tokenSecret = secrets.GetSecret("GoogleDriveToken").Value;

                services.AddSingleton(GoogleCredential
                    .FromJson(tokenSecret.Value)
                    .CreateScoped("https://www.googleapis.com/auth/drive"));
            }

            services.AddScoped<IDriveServiceFacade, DriveServiceFacade>();
            services.AddScoped(s => new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = s.GetService<GoogleCredential>(),
                ApplicationName = "Wikibus Drive robot Azure Function"
            }));
        }
    }
}
