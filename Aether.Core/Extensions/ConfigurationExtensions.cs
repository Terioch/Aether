using Microsoft.Extensions.Configuration;

namespace Aether.Core.Extensions
{
    public static class ConfigurationExtensions
    {
        public static string[] GetCorsOrigins(this IConfiguration configuration)
        {
            return configuration.GetRequiredSection("CorsOrigins")
                .AsEnumerable()
                .Where(pair => pair.Value is not null)
                .Select(pair => pair.Value)
                .ToArray()!;
        }
    }
}
