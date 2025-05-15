namespace Aether.Core.Entities;

public class LocationEntity
{
    public int Id { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    #region navigation properties

    public virtual AirQualityReadingEntity? AirQualityReading { get; set; }

    #endregion
}
