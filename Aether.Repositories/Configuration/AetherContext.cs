using Aether.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

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

        // Set table and column names to lower case
        foreach (var entity in builder.Model.GetEntityTypes())
        {            
            entity.SetTableName(entity.GetTableName()?.ToLower());

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.Name.ToLower());
            }
        }        
    }
}
