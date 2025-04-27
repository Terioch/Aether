using Aether.Core.Models;

namespace Aether.Core
{
    public class AirQuality
    {
        public required GeoLocation Location { get; set; }

        public int Index { get; set; }

        public required Pollutant SulfurDioxide { get; set; }

        public required Pollutant NitrogenOxide { get; set; }

        public required Pollutant NitrogenDioxide { get; set; }

        public required Pollutant ParticulateMatter10 { get; set; }

        public required Pollutant ParticulateMatter2_5 { get; set; }

        public required Pollutant Ozone { get; set; }

        public required Pollutant CarbonMonoxide { get; set; }

        public required Pollutant Ammonia { get; set; }
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
}
