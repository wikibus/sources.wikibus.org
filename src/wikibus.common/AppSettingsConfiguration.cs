using Argolis.Models;
using Microsoft.Extensions.Configuration;

namespace Wikibus.Common
{
    /// <summary>
    /// Retrieves settings from setting configuration section
    /// </summary>
    public class AppSettingsConfiguration : IWikibusConfiguration, IBaseUriProvider
    {
        private const string Prefix = "wikibus:";

        private const string BaseUrl = Prefix + "baseUrl";
        private const string ApiUrl = Prefix + "apiUrl";
        private const string WebUrl = Prefix + "websiteUrl";

        private readonly IConfiguration configuration;

        public AppSettingsConfiguration(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Gets the base namespace for data resources.
        /// </summary>
        public string BaseResourceNamespace
        {
            get { return this.configuration[BaseUrl]; }
        }

        /// <summary>
        /// Gets the base namespace for API resources.
        /// </summary>
        public string BaseApiNamespace
        {
            get { return this.configuration[ApiUrl]; }
        }

        /// <summary>
        /// Gets the base address for the wikibus.org website
        /// </summary>
        public string BaseWebNamespace
        {
            get { return this.configuration[WebUrl]; }
        }

        public string BaseResourceUri => this.BaseResourceNamespace;
    }
}
