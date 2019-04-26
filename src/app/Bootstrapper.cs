using Microsoft.Extensions.Configuration;
using StructureMap;

namespace Brochures.Wikibus.Org
{
    /// <summary>
    /// The Nancy bootstrapper
    /// </summary>
    public class Bootstrapper : global::Wikibus.Nancy.Bootstrapper
    {
        private readonly IConfiguration configuration;

        public Bootstrapper(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Configures the application container.
        /// </summary>
        /// <param name="existingContainer">The existing container.</param>
        protected override void ConfigureApplicationContainer(IContainer existingContainer)
        {
            existingContainer.Configure(_ =>
            {
                _.For<global::Wikibus.Sources.DotNetRDF.ISourcesDatabaseSettings>().Use<Settings>();
                _.Forward<Settings, global::Wikibus.Sources.ISourcesDatabaseSettings>();
                _.For<IConfiguration>().Use(this.configuration);
            });

            base.ConfigureApplicationContainer(existingContainer);
        }
    }
}
