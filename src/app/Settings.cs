using System;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Wikibus.Sources.DotNetRDF;

namespace Brochures.Wikibus.Org
{
    public class Settings : ISourcesDatabaseSettings
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
    }
}