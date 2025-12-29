using Aether.Core;
using Newtonsoft.Json;

const string API_KEY = "249874698345";

var manchester = new GeoLocation(53.2844, -2.1443);
var shanghai = new GeoLocation(31.23041600, 121.47370100);
var tokyo = new GeoLocation(35.6895, 139.839478);

var manchesterAirQuality = await GetLocationAirQuality(manchester);
var shanghaiAirQuality = await GetLocationAirQuality(shanghai);
var tokyoAirQuality = await GetLocationAirQuality(tokyo);

LogAirQualityInfo("Manchester", manchesterAirQuality);
LogAirQualityInfo("Shanghai", shanghaiAirQuality);
LogAirQualityInfo("Tokyo", tokyoAirQuality);

Console.ReadLine();

static void LogAirQualityInfo(string LocationName, AirQuality? airQuality)
{
    Console.WriteLine($"{LocationName}\n");

    if (airQuality == null)
    {
        Console.WriteLine("No air quality information for this location");
        return;
    }

    Console.WriteLine($"Latitude: {airQuality.Location.Latitude}, Longitude: {airQuality.Location.Longitude}");

    foreach (var item in airQuality.Items)
    {
        Console.WriteLine($"Index: {item.AirQualityIndex}");
        Console.WriteLine($"Carbon Monoxide: {item.CarbonMonoxide}");
        Console.WriteLine($"Sulfur Dioxide: {item.SulfurDioxide}");
        Console.WriteLine($"Nitrogen Dioxide: {item.NitrogenDioxide}");
        Console.WriteLine($"Nitrogen Oxide: {item.NitrogenOxide}");
        Console.WriteLine($"Ozone: {item.Ozone}");
        Console.WriteLine($"Particulate Matter 10: {item.ParticulateMatter10}");
        Console.WriteLine($"Particulate Matter 2.5: {item.ParticulateMatter2_5}");
        Console.WriteLine($"Ammonia: {item.Ammonia}");
    }

    Console.WriteLine("-----------------");
}

static async Task<AirQuality?> GetLocationAirQuality(GeoLocation location)
{
    var url = $"http://api.openweathermap.org/data/2.5/air_pollution?lat={location.Latitude}&lon={location.Longitude}&appid={API_KEY}";

    using var client = new HttpClient();

    var responseString = await client.GetStringAsync(url);

    var response = JsonConvert.DeserializeObject<AirQualityResponse>(responseString);

    if (response == null) return null;

    var result = new AirQuality
    {
        Location = new GeoLocation(response.Coord.Lat, response.Coord.Lon),
        Items = new List<AirQualityItem>()
    };

    foreach (var item in response.List)
    {
        var airQualityItem = new AirQualityItem
        {
            AirQualityIndex = item.Main.Aqi,
            CarbonMonoxide = item.Components.Co,
            SulfurDioxide = item.Components.So2,
            NitrogenDioxide = item.Components.No2,
            NitrogenOxide = item.Components.No,
            Ozone = item.Components.O3,
            ParticulateMatter10 = item.Components.Pm10,
            ParticulateMatter2_5 = item.Components.Pm2_5,
            Ammonia = item.Components.Nh3
        };

        result.Items.Add(airQualityItem);
    }

    return result;
}