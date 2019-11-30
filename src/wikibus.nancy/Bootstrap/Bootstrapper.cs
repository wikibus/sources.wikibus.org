using System;
using System.Linq;
using Argolis.Hydra;
using Argolis.Models;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.StructureMap;
using Nancy.Configuration;
using Nancy.Diagnostics;
using Nancy.Responses.Negotiation;
using Nancy.Routing.UriTemplates;
using StructureMap;
using Wikibus.Common;
using Wikibus.Nancy.Hydra;
using Wikibus.Sources;
using Wikibus.Sources.EF;
using Wikibus.Sources.Images;

namespace Wikibus.Nancy
{
    /// <summary>
    /// Bootstrapper for wikibus.org API app
    /// </summary>
    public class Bootstrapper : StructureMapNancyBootstrapper
    {
        /// <summary>
        /// Gets overridden configuration
        /// </summary>
        protected override Func<ITypeCatalog, NancyInternalConfiguration> InternalConfiguration
        {
            get
            {
                return NancyInternalConfiguration.WithOverrides(c =>
                {
                    c.ResponseProcessors = c.ResponseProcessors.Where(IsNotNancyProcessor).ToList();
                    c.RouteResolver = typeof(UriTemplateRouteResolver);
                    c.StatusCodeHandlers.Clear();
                });
            }
        }

        /// <summary>
        /// Configures the Nancy environment
        /// </summary>
        /// <param name="environment">The <see cref="T:Nancy.Configuration.INancyEnvironment" /> instance to configure</param>
        public override void Configure(INancyEnvironment environment)
        {
            base.Configure(environment);

            environment.Tracing(true, true);
            environment.Diagnostics("wb");
        }

        /// <summary>
        /// Configures the application container.
        /// </summary>
        /// <param name="existingContainer">The existing container.</param>
        protected override void ConfigureApplicationContainer(IContainer existingContainer)
        {
            existingContainer.Configure(_ =>
            {
                _.ForConcreteType<AppSettingsConfiguration>();
                _.Forward<AppSettingsConfiguration, IWikibusConfiguration>();
                _.Forward<AppSettingsConfiguration, IBaseUriProvider>();

                _.For<IHydraDocumentationSettings>().Use<HydraDocumentationSettings>();
                _.For<IImageResizer>().Use<ImageResizer>();
                _.Scan(scan =>
                {
                    scan.Assembly(typeof(SourcesRepository).Assembly);
                    scan.RegisterConcreteTypesAgainstTheFirstInterface();
                });
                _.For<ISourceImagesRepository>().Use<SourceImagesRepository>();
                _.ForConcreteType<EntityFactory>().Configure.Transient();
            });

            base.ConfigureApplicationContainer(existingContainer);
        }

        /// <summary>
        /// Configures the request container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="context">The context.</param>
        protected override void ConfigureRequestContainer(IContainer container, NancyContext context)
        {
            container.Configure(_ =>
            {
                _.For<ISourceContext>().Use<SourceContext>();
            });

            base.ConfigureRequestContainer(container, context);
        }

        private static bool IsNotNancyProcessor(Type responseProcessor)
        {
            return responseProcessor.Assembly != typeof(INancyBootstrapper).Assembly || responseProcessor == typeof(ResponseProcessor);
        }
    }
}
