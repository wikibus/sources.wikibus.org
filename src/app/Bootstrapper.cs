using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nancy;
using StructureMap;
using wikibus.images.Cloudinary;
using Wikibus.Sources;
using wikibus.storage.azure;

namespace Brochures.Wikibus.Org
{
    /// <summary>
    /// The Nancy bootstrapper
    /// </summary>
    public class Bootstrapper : global::Wikibus.Nancy.Bootstrapper
    {
        private readonly IConfiguration configuration;
        private readonly ILoggerFactory loggerFactory;

        public Bootstrapper(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.configuration = configuration;
            this.loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Configures the application container.
        /// </summary>
        /// <param name="existingContainer">The existing container.</param>
        protected override void ConfigureApplicationContainer(IContainer existingContainer)
        {
            existingContainer.Configure(_ =>
            {
                _.Forward<Settings, ISourcesDatabaseSettings>();
                _.Forward<Settings, ICloudinarySettings>();
                _.Forward<Settings, IAzureSettings>();
                _.For<IConfiguration>().Use(this.configuration);
                _.For<ILoggerFactory>().Use(this.loggerFactory);
            });

            base.ConfigureApplicationContainer(existingContainer);
        }

        protected override void ConfigureRequestContainer(IContainer container, NancyContext context)
        {
            container.Configure(_ =>
            {
                if (context.CurrentUser != null)
                {
                    _.For<ClaimsPrincipal>().Use(context.CurrentUser);
                }
            });
            base.ConfigureRequestContainer(container, context);
        }
    }
}
