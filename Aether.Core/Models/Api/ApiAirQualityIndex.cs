namespace Aether.Core.Models.Api
{
    public sealed class ApiAirQualityIndex
    {
        public required ApiGeoLocation Coord { get; set; }

        public required List<ApiAirQualityItem> List { get; set; }
    }

    public class ApiGeoLocation
    {
        public double Lat { get; set; }

        public double Lon { get; set; }
    }

    public class ApiAirQualityItem
    {
        public required ApiMain Main { get; set; }

        public required ApiComponents Components { get; set; }
    }

    public class ApiMain
    {
        public int Aqi { get; set; }
    }

    public class ApiComponents
    {
        public double So2 { get; set; } // Sulfur Dioxide

        public double No2 { get; set; } // Nitrogen Dioxide

        public double No { get; set; } // Nitrogen Oxide

        public double Pm10 { get; set; } // Particulate Matter 10

        public double Pm2_5 { get; set; } // Particulate Matter 2.5

        public double O3 { get; set; } // Ozone

        public double Co { get; set; } // Carbon Monoxide

        public double Nh3 { get; set; } // Ammonia
    }
}
