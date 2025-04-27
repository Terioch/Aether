using Aether.Core.Repositories;
using Aether.Repositories.Configuration;
using Aether.Repositories.Hosted;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Aether.Repositories;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAetherRepositories(this IServiceCollection services, string? connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        services.AddDbContext<AetherContext>(options =>
        {
            options.UseNpgsql(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(12, TimeSpan.FromSeconds(12), null);
            });
        });

        services.AddTransient<IAetherConnectionFactory>(_ => new AetherConnectionFactory(connectionString));

        services.AddHostedService<DatabaseService>();

        services.AddScoped<IAetherUnitOfWork, AetherUnitOfWork>();

        return services;
    }
}
