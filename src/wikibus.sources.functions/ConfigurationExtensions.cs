using Microsoft.Extensions.Configuration;

namespace Wikibus.Sources.Functions
{
    public static class ConfigurationExtensions
    {
        public static bool IsDevelopment(this IConfiguration configuration)
        {
            return configuration["ASPNETCORE_ENVIRONMENT"] == "Development";
        }
    }
}
