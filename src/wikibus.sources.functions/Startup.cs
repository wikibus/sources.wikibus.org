using System.IO;
using System.Reflection;
using Anotar.Serilog;
using Argolis.Hydra.Models;
using Argolis.Models;
using Brochures.Wikibus.Org;
using CloudinaryDotNet;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using ImageMagick;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Wikibus.Common;
using wikibus.images.Cloudinary;
using Wikibus.Sources.EF;
using Wikibus.Sources.Images;
using wikibus.storage;
using wikibus.storage.azure;

[assembly: FunctionsStartup(typeof(Wikibus.Sources.Functions.Startup))]

namespace Wikibus.Sources.Functions
{
    public class Startup : FunctionsStartup
    {
        public Startup()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithThreadId()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .WriteTo.ApplicationInsights(TelemetryConfiguration.CreateDefault(), TelemetryConverter.Traces)
                .WriteTo.Console()
                .CreateLogger();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var currentDirectory = builder.Services.BuildServiceProvider()
                .GetService<IOptions<ExecutionContextOptions>>().Value.AppDirectory;

            var configuration = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile(@"appSettings.json", false, true)
                .AddJsonFile(@"appSettings.Development.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            if (configuration.GetValue<bool>("Ghostscript"))
            {
                var binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var ghostscriptPath = Path.Combine(binDirectory, "../Ghostscript");
                LogTo.Information("Setting Ghostscript path {0}", ghostscriptPath);
                MagickNET.SetGhostscriptDirectory(ghostscriptPath);
            }

            builder.Services.AddLogging();

            var account = new Account(
                configuration["cloudinary:name"],
                configuration["cloudinary:key"],
                configuration["cloudinary:secret"]);

            builder.Services.AddSingleton(new Cloudinary(account));

            builder.Services.AddTransient<ISourceContext>(provider => provider.GetService<SourceContext>());
            builder.Services.AddDbContext<SourceContext>(
                options => options.UseSqlServer(
                    configuration["wikibus:sources:sql"]));
            builder.Services.AddTransient<IImageStorage, CloudinaryImagesStore>();
            builder.Services.AddSingleton<IUriTemplateMatcher, DefaultUriTemplateMatcher>();
            builder.Services.AddSingleton<IUriTemplateExpander, DefaultUriTemplateExpander>();
            builder.Services.AddTransient<ISourceImageService, SourceImageService>();
            builder.Services.AddSingleton<IModelTemplateProvider, AttributeModelTemplateProvider>();
            builder.Services.AddSingleton<IBaseUriProvider, AppSettingsConfiguration>();
            builder.Services.AddTransient<ISourcesRepository, SourcesRepository>();
            builder.Services.AddTransient<EntityFactory>();
            builder.Services.AddSingleton<IWikibusConfiguration, AppSettingsConfiguration>();
            builder.Services.AddSingleton<IConfiguration>(configuration);
            builder.Services.AddSingleton<ISourcesDatabaseSettings, Settings>();
            builder.Services.AddSingleton<ICloudinarySettings, Settings>();
            builder.Services.AddSingleton<IAzureSettings, Settings>();
            builder.Services.AddSingleton<ManagementClientFactory>();
            builder.Services.AddScoped<IDriveServiceFacade, DriveServiceFacade>();
            builder.Services.AddScoped<ISourcesPersistence, SourcesPersistence>();
            builder.Services.AddScoped<IPdfService, PdfService>();
            builder.Services.AddScoped<IWishlistPersistence, WishlistPersistence>();

            if (configuration["ASPNETCORE_ENVIRONMENT"] == "Development")
            {
                builder.Services.AddSingleton<IStorageQueue, NullStorageQueue>();
                builder.Services.AddSingleton<IFileStorage, LocalFileStorage>();
            }
            else
            {
                builder.Services.AddSingleton<IStorageQueue, StorageQueue>();
                builder.Services.AddSingleton<IFileStorage, BlobFileStorage>();
            }

            builder.Services.AddScoped(_ =>
            {
                var credential = GoogleCredential
                    .FromFile("/Users/tomaszpluskiewicz/Downloads/wikibus-org-76fb423963bb.json")
                    .CreateScoped("https://www.googleapis.com/auth/drive");

                return new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Wikibus Drive robot Azure Function"
                });
            });
        }
    }
}
