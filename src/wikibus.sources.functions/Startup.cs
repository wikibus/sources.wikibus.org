using System;
using System.IO;
using Anotar.Serilog;
using Argolis.Hydra.Models;
using Argolis.Models;
using ImageMagick;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using wikibus.images.Cloudinary;
using Wikibus.Sources.EF;
using Wikibus.Sources.Images;

[assembly: FunctionsStartup(typeof(Wikibus.Sources.Functions.Startup))]

namespace Wikibus.Sources.Functions
{
    public class Startup : FunctionsStartup
    {
        public Startup(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            var ghostscriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Ghostscript");
            LogTo.Information("Setting Ghostscript path {0}", ghostscriptPath);
            MagickNET.SetGhostscriptDirectory(ghostscriptPath);
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.AddDbContext<SourceContext>();
            builder.Services.AddTransient<IImageStorage, CloudinaryImagesStore>();
            builder.Services.AddSingleton<IUriTemplateMatcher, DefaultUriTemplateMatcher>();
            builder.Services.AddSingleton<SourceImageService>();
            builder.Services.AddSingleton<IModelTemplateProvider, AttributeModelTemplateProvider>();
            builder.Services.AddSingleton<IBaseUriProvider, BaseUriProvider>();
        }
    }
}
