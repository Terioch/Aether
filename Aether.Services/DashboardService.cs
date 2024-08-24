using Aether.Core;
using Aether.Core.Models;
using Aether.Core.Models.Api;
using Aether.Core.Requests;
using Aether.Core.Services;
using Aether.Core.Utils;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Aether.Services;

public class DashboardService : IDashboardService
{
    private readonly string _apiKey;

    public DashboardService(IConfiguration configuration)
    {
        _apiKey = configuration["ApiKeys:OpenWeatherMapApiKey"]!;
    }

    public async Task<DashboardView> GetDashboardView(GeoLocation geoLocation)
    {
        //var manchester = new GeoLocation(53.2844, -2.1443);
        var index = await GetAirQualityIndex(geoLocation);
        var location = await GetGlobalLocation(geoLocation);

        return new DashboardView
        {
            Location = location,
            AirQualityIndex = index
        };
    }

    private async Task<AirQualityIndex> GetAirQualityIndex(GeoLocation location)
    {
        var url = $"http://api.openweathermap.org/data/2.5/air_pollution?lat={location.Latitude}&lon={location.Longitude}&appid={_apiKey}";

        using var client = new HttpClient();

        var responseString = await client.GetStringAsync(url);

        var response = JsonConvert.DeserializeObject<ApiAirQualityIndex>(responseString)
            ?? throw new Exception("Air quality index not found for this location");

        var item = response.List[0];

        return new AirQualityIndex
        {
            Location = new GeoLocation(response.Coord.Lat, response.Coord.Lon),
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

    private async Task<GlobalLocation> GetGlobalLocation(GeoLocation geoLocation)
    {
        var url = $"http://api.openweathermap.org/geo/1.0/reverse?lat={geoLocation.Latitude}&lon={geoLocation.Longitude}&appid={_apiKey}";

        using var client = new HttpClient();

        var responseString = await client.GetStringAsync(url);

        var response = JsonConvert.DeserializeObject<List<ApiGlobalLocation>>(responseString)
            ?? throw new Exception("Air quality index not found for this location");

        var result = response.FirstOrDefault() ?? throw new Exception($"Location was not found for lat: {geoLocation.Latitude}, lon: {geoLocation.Longitude}");

        return new GlobalLocation
        {
            Country = result.Country,
            State = result.State,
            City = result.Name,
            Latitude = result.Latitude,
            Longitude = result.Longitude,
        };
    }

    public async Task<List<MapEntry>> GetNearbyMapEntries(GetNearbyMapEntriesRequest request)
    {
        var nearbyLocations = GetNearbyLocations(request.Location, 13);

        var responseTasksByLocation = new Dictionary<GeoLocation, Task<string>>();
        var mapEntries = new List<MapEntry>();
        using var client = new HttpClient();

        foreach (var nearbyLocation in nearbyLocations)
        {
            var url = $"http://api.openweathermap.org/data/2.5/air_pollution?lat={request.Location.Latitude}&lon={request.Location.Longitude}&appid={_apiKey}";

            var responseString = client.GetStringAsync(url);

            responseTasksByLocation.Add(nearbyLocation, responseString);
        }

        await Task.WhenAll(responseTasksByLocation.Values);

        foreach (var nearbyLocation in nearbyLocations)
        {
            var responseString = responseTasksByLocation[nearbyLocation].Result;

            var response = JsonConvert.DeserializeObject<ApiAirQualityIndex>(responseString)
                ?? throw new Exception("Air quality index not found for this location");

            var item = response.List[0];

            var index = new AirQualityIndex
            {
                Location = new GeoLocation(response.Coord.Lat, response.Coord.Lon),
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

            var mapEntry = new MapEntry
            {
                Location = new GeoLocation(response.Coord.Lat, response.Coord.Lon),
                AirQualityIndex = index
            };

            mapEntries.Add(mapEntry);
        }

        return mapEntries;
    }

    private List<GeoLocation> GetNearbyLocations(GeoLocation center, int zoom)
    {
        // Calculate scale factor based on zoom level
        double scale = Math.Max(0.1, 1.0 / Math.Pow(2, zoom - 5)); // Adjust the formula to fine-tune

        // Define nearby locations relative to the center
        var nearbyLocations = new List<GeoLocation>
        {
            new GeoLocation(center.Latitude + 0.2 * scale, center.Longitude + 0.2 * scale),
            new GeoLocation(center.Latitude + 0.1 * scale, center.Longitude + 0.1 * scale),
            new GeoLocation(center.Latitude - 0.1 * scale, center.Longitude - 0.1 * scale),
            new GeoLocation(center.Latitude - 0.2 * scale, center.Longitude - 0.2 * scale)
        };

        return nearbyLocations;
    }
}
