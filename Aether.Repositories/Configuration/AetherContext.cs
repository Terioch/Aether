using Aether.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aether.Repositories.Configuration;

public sealed class AetherContext : DbContext
{
    public AetherContext() { }

    public AetherContext(DbContextOptions<AetherContext> options) : base(options) { }

    public DbSet<LocationEntity> Locations { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(AetherContext).Assembly);
    }
}
