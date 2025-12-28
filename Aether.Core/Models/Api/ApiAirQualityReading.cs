using Aether.Core.Utils;

namespace Aether.Core.Models.Api
{
    public sealed class ApiAirQualityReading
    {
        public required ApiGeoLocation Coord { get; set; }

        public required List<ApiAirQualityItem> List { get; set; }

        public static AirQualityReading ToReading(ApiAirQualityReading apiReading)
        {
            var item = apiReading.List[0];

            return new AirQualityReading
            {
                Location = new GeoLocation(apiReading.Coord.Lat, apiReading.Coord.Lon),
                Index = item.Main.Aqi,
                CarbonMonoxide = PollutantUtils.CarbonMonoxide(item.Components.Co),
                SulfurDioxide = PollutantUtils.SulfurDioxide(item.Components.So2),
                NitrogenDioxide = PollutantUtils.NitrogenDioxide(item.Components.No2),
                NitrogenOxide = PollutantUtils.NitrogenOxide(item.Components.No),
                Ozone = PollutantUtils.Ozone(item.Components.O3),
                ParticulateMatter10 = PollutantUtils.ParticulateMatter_10(item.Components.Pm10),
                ParticulateMatter2_5 = PollutantUtils.ParticulateMatter_25(item.Components.Pm2_5),
                Ammonia = PollutantUtils.Ammonia(item.Components.Nh3)
            };
        }
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
