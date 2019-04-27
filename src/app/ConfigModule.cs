using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Nancy;

namespace Brochures.Wikibus.Org
{
    public class ConfigModule : NancyModule
    {
        public ConfigModule(IConfiguration configuration)
        {
            this.Get("/config", _ => this.Response.AsText(string.Join(
                    Environment.NewLine,
                    configuration.AsEnumerable().Select(p => $"{p.Key} => {p.Value}"))));
        }
    }
}
