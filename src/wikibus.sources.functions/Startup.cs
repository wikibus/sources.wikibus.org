using Argolis.Hydra.Models;
using Argolis.Models;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using wikibus.images.Cloudinary;
using Wikibus.Sources.EF;
using Wikibus.Sources.Images;

[assembly: FunctionsStartup(typeof(Wikibus.Sources.Functions.Startup))]

namespace Wikibus.Sources.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddDbContext<SourceContext>();
            builder.Services.AddTransient<IImageStorage, CloudinaryImagesStore>();
            builder.Services.AddSingleton<IUriTemplateMatcher, DefaultUriTemplateMatcher>();
            builder.Services.AddSingleton<SourceImageService>();
            builder.Services.AddSingleton<IModelTemplateProvider, AttributeModelTemplateProvider>();
            builder.Services.AddSingleton<IBaseUriProvider, BaseUriProvider>();
        }
    }
}
