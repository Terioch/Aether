using Aether.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aether.Repositories.EntityMaps;

public class AirQualityReadingEntityMap
{
    public virtual void Configure(EntityTypeBuilder<AirQualityReadingEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder
            .HasOne(e => e.Location)
            .WithOne(e => e.AirQualityReading)
            .HasForeignKey<AirQualityReadingEntity>(e => e.Id);
    }
}
