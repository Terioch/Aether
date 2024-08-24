using Aether.Core.Extensions;
using Aether.Core.Utils;
using Serilog;

namespace Aether.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static void UseSerilog(this WebApplicationBuilder builder)
        {           
            var path = Logging.GetRequiredLogsFolder("aether-.txt");

            builder.Host.UseSerilog((_, config) => config
                .ReadFrom.Configuration(builder.Configuration)
                .WriteTo.Console()
                .WriteTo.File(path, shared: true, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7));
        }

        public static void AddCors(this WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy => policy
                    .WithOrigins(builder.Configuration.GetCorsOrigins())
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithExposedHeaders("Content-Disposition")
                );
            });
        }

        public static string GetConfigValue(this WebApplicationBuilder builder, string key)
        {
            var result = builder.Configuration[key];

            return result ?? throw new ApplicationException($"configuration value for key '{key}' does not exist");
        }

        public static TType GetConfigValue<TType>(this WebApplicationBuilder builder, string key) where TType : struct
        {
            return builder.Configuration.GetValue<TType>(key);
        }
    }
}
