using System;
using Microsoft.Extensions.Configuration;
using wikibus.images.Cloudinary;
using Wikibus.Sources.DotNetRDF;
using wikibus.storage.azure;

namespace Brochures.Wikibus.Org
{
    public class Settings : ISourcesDatabaseSettings, ICloudinarySettings, IAzureSettings
    {
        private readonly IConfiguration configuration;

        public Settings(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string ConnectionString
        {
            get { return this.configuration["wikibus:sources:sql"]; }
        }

        public Uri SourcesSparqlEndpoint
        {
            get
            {
                var setting = this.configuration["wikibus:sources:sparqlEndpoint"];
                return new Uri(setting);
            }
        }

        public string BrochuresFolder => this.configuration["cloudinary:folders:brochures"];

        public string ThumbnailTransformation => this.configuration["cloudinary:thumb_transformation"];

        public string DefaultTransformation => this.configuration["cloudinary:default_transformation"];

        public bool LogDatabaseAccess => this.configuration.GetValue<bool>("Logging:Database");

        string IAzureSettings.ConnectionString => this.configuration["azure:storage:connectionString"];
    }
}
