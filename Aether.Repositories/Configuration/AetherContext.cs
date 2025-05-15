using Aether.Core.Entities;
using Aether.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Aether.Repositories.Configuration;

public sealed class AetherContext : DbContext
{
    public AetherContext() { }

    public AetherContext(DbContextOptions<AetherContext> options) : base(options) { }

    public DbSet<LocationEntity> Locations { get; set; } = null!;

    public DbSet<AirQualityReadingEntity> AirQualityReadings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(AetherContext).Assembly);

        //builder.Entity<LocationEntity>().ToTable("locations");

        // Set table, and column names to snake case
        /*foreach (var entity in builder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.GetTableName().ToSnakeCase());

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.Name.ToSnakeCase());
            }          
        }*/
    }
}
