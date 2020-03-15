using System;
using System.IO;
using System.Reflection;
using Anotar.Serilog;
using Argolis.Hydra.Models;
using Argolis.Models;
using Brochures.Wikibus.Org;
using ImageMagick;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters;
using Wikibus.Common;
using wikibus.images.Cloudinary;
using Wikibus.Sources.EF;
using Wikibus.Sources.Images;

[assembly: FunctionsStartup(typeof(Wikibus.Sources.Functions.Startup))]

namespace Wikibus.Sources.Functions
{
    public class Startup : FunctionsStartup
    {
        private readonly IConfigurationRoot configuration;

        public Startup()
        {
            this.configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile(@"appSettings.json", false, true)
                .AddJsonFile(@"appSettings.Development.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(this.configuration)
                .CreateLogger();

            if (this.configuration.GetValue<bool>("Ghostscript"))
            {
                var binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var ghostscriptPath = Path.Combine(binDirectory, "../Ghostscript");
                LogTo.Information("Setting Ghostscript path {0}", ghostscriptPath);
                MagickNET.SetGhostscriptDirectory(ghostscriptPath);
            }
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();

            builder.Services.AddTransient<ISourceContext>(provider => provider.GetService<SourceContext>());
            builder.Services.AddDbContext<SourceContext>(
                options => options.UseSqlServer(
                    this.configuration["wikibus:sources:sql"]));
            builder.Services.AddTransient<IImageStorage, CloudinaryImagesStore>();
            builder.Services.AddSingleton<IUriTemplateMatcher, DefaultUriTemplateMatcher>();
            builder.Services.AddSingleton<IUriTemplateExpander, DefaultUriTemplateExpander>();
            builder.Services.AddTransient<ISourceImageService, SourceImageService>();
            builder.Services.AddSingleton<IModelTemplateProvider, AttributeModelTemplateProvider>();
            builder.Services.AddSingleton<IBaseUriProvider, AppSettingsConfiguration>();
            builder.Services.AddTransient<ISourcesRepository, SourcesRepository>();
            builder.Services.AddTransient<EntityFactory>();
            builder.Services.AddSingleton<IWikibusConfiguration, AppSettingsConfiguration>();
            builder.Services.AddSingleton<IConfiguration>(this.configuration);
            builder.Services.AddSingleton<ISourcesDatabaseSettings, Settings>();
        }
    }
}
