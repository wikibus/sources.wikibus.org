using System;
using Argolis.Hydra.Discovery.SupportedOperations;
using JsonLD.Entities;
using Microsoft.Extensions.Configuration;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Rdf.ModelBinding;
using Nancy.Routing;
using Wikibus.Common;
using Wikibus.Sources;

namespace Wikibus.Nancy
{
    /// <summary>
    /// Install all required components
    /// </summary>
    public class ComponentsInstaller : Registrations
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentsInstaller"/> class.
        /// </summary>
        /// <param name="databaseSettings">Database configuration provider</param>
        /// <param name="catalog">Nancy type catalog</param>
        public ComponentsInstaller(ITypeCatalog catalog, IConfiguration c)
            : base(catalog)
        {
            IWikibusConfiguration configuration = new AppSettingsConfiguration(c);

            this.Register(configuration);
            this.Register<IFrameProvider>(new WikibusModelFrames());
            this.Register<DefaultRouteResolver>();
            this.Register<IRdfConverter>(typeof(RdfConverter));
            this.RegisterAll<ISupportedOperations>(Lifetime.PerRequest);
        }
    }
}
