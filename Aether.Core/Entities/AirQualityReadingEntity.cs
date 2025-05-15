namespace Aether.Core.Entities;

public class AirQualityReadingEntity
{
    public int Id { get; set; }

    public int LocationId { get; set; }

    public int Index { get; set; }

    public double SulfurDioxide { get; set; }

    public double NitrogenOxide { get; set; }

    public double NitrogenDioxide { get; set; }

    public double ParticulateMatter10 { get; set; }

    public double ParticulateMatter2_5 { get; set; }

    public double Ozone { get; set; }

    public double CarbonMonoxide { get; set; }

    public double Ammonia { get; set; }

    #region navigation properties

    public virtual LocationEntity? Location { get; set; }

    #endregion
}
