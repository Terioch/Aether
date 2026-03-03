namespace Aether.Core.Models
{
    public sealed class DashboardView
    {
        public GlobalLocation? Location { get; set; }

        public required AirQualityReading AirQualityReading { get; set; }

        public required ChangePercentages ChangePercentages { get; set; }
    }

    public class ChangePercentages
    {
        public int Aqi { get; set; }

        public double CarbonMonoxide { get; set; }
        public double SulfurDioxide { get; set; }
        public double NitrogenDioxide { get; set; }
        public double NitrogenOxide { get; set; }
        public double Ozone { get; set; }
        public double ParticulateMatter10 { get; set; }
        public double ParticulateMatter2_5 { get; set; }
        public double Ammonia { get; set; }
    }
}
