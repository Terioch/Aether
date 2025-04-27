using Aether.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aether.Repositories.EntityMaps;

public class LocationEntityMap
{
    public virtual void Configure(EntityTypeBuilder<LocationEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => new { e.Latitude, e.Longitude });
    }
}
