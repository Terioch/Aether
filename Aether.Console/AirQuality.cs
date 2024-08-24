namespace Aether.Core
{
    public class AirQuality
    {
        public required GeoLocation Location { get; set; }

        public required List<AirQualityItem> Items { get; set; }
    }

    public class GeoLocation 
    {
        public GeoLocation(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }

    public class AirQualityItem
    {
        public int AirQualityIndex { get; set; }

        public double SulfurDioxide { get; set; }

        public double NitrogenOxide { get; set; }

        public double NitrogenDioxide { get; set; }

        public double ParticulateMatter10 { get; set; }

        public double ParticulateMatter2_5 { get; set; }

        public double Ozone { get; set; }

        public double CarbonMonoxide { get; set; }

        public double Ammonia { get; set; }
    }
}
