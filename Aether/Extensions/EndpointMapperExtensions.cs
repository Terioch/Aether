using Aether.Core;
using System.Reflection;

namespace Aether.Extensions
{
    public static class EndpointMapperExtensions
    {
        public static void AddEndpointMappers(this IServiceCollection services)
        {
            var assembly = Assembly.GetEntryAssembly();

            if (assembly is null) throw new Exception("Failed to get entry assembly");

            var endpoints = assembly.ExportedTypes
                .Where(
                    t => typeof(IEndpointMapper).IsAssignableFrom(t)
                         && t is { IsAbstract: false, IsInterface: false }
                )
                .Select(Activator.CreateInstance)
                .Cast<IEndpointMapper>()
                .ToList();

            services.AddSingleton<IReadOnlyCollection<IEndpointMapper>>(endpoints);
        }

        public static void UseEndpointMappers(this WebApplication app)
        {
            var mappers = app.Services.GetRequiredService<IReadOnlyCollection<IEndpointMapper>>();

            foreach (var mapper in mappers)
            {
                mapper.MapEndpoints(app);
            }
        }
    }
}
