using Aether.Core;
using Aether.Core.Models;
using Aether.Core.Models.Api;
using Aether.Core.Repositories;
using Aether.Core.Requests;
using Aether.Core.Services;
using Aether.Core.Utils;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Aether.Services;

public class DashboardService : IDashboardService
{
    private readonly string _apiKey;
    private readonly ILocationRepository _locationRepository;

    public DashboardService(IConfiguration configuration, IAetherUnitOfWork unitOfWork)
    {
        _apiKey = configuration["ApiKeys:OpenWeatherMapApiKey"]!;
        _locationRepository = unitOfWork.Locations;
    }

    public async Task<DashboardView> GetDashboardView(GeoLocation geoLocation)
    {
        //var manchester = new GeoLocation(53.2844, -2.1443);
        var index = await GetAirQualityIndex(geoLocation);
        var location = await GetGlobalLocation(geoLocation);

        return new DashboardView
        {
            Location = location,
            AirQuality = index
        };
    }

    private async Task<AirQuality> GetAirQualityIndex(GeoLocation location)
    {
        var url = $"http://api.openweathermap.org/data/2.5/air_pollution?lat={location.Latitude}&lon={location.Longitude}&appid={_apiKey}";

        using var client = new HttpClient();

        var responseString = await client.GetStringAsync(url);

        var response = JsonConvert.DeserializeObject<ApiAirQuality>(responseString)
            ?? throw new Exception("Air quality index not found for this location");

        var item = response.List[0];

        return new AirQuality
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

    public async Task<MapEntriesView> GetMapEntries(MapEntriesRequest request)
    {
        /* TODO: Pollutants should be a list within AirQuality and have a name and possibly a unit field.
        Urgent TODO: Cache air quality indexes in database to avoid querying openweathermap Could also cache indexes 
        using the built-in memory cache.
        TODO: Monthly background job that updates indexes every 3 months. 
        Urgent TODO: Ensure locations in database are all a minimum distance apart */

        var locations = new List<GeoLocation> { request.Centre };
        var nearbyLocations = await GetNearbyLocations(request);

        locations.AddRange(nearbyLocations);

        var responseTasksByLocation = new Dictionary<GeoLocation, Task<string>>();
        var mapEntries = new List<MapEntry>();
        using var client = new HttpClient();        

        ProcessLocations(locations, responseTasksByLocation, client);

        await Task.WhenAll(responseTasksByLocation.Values);

        foreach (var location in locations)
        {
            //var responseString = responseTasksByLocation[location].Result;
            // Temporary until indexes are cached
            var responseString = "{\"coord\":{\"lon\":-2.3637,\"lat\":53.4541},\"list\":[{\"main\":{\"aqi\":2},\"components\":{\"co\":119.47,\"no\":0,\"no2\":4.88,\"o3\":68.24,\"so2\":0.81,\"pm2_5\":2.94,\"pm10\":3.35,\"nh3\":7.54},\"dt\":1746995138}]}";

            var response = JsonConvert.DeserializeObject<ApiAirQuality>(responseString)
                ?? throw new Exception("Air quality index not found for this location");

            var item = response.List[0];

            var index = new AirQuality
            {
                Location = location,
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
                AirQuality = index
            };

            mapEntries.Add(mapEntry);
        }

        return new MapEntriesView 
        { 
            Centre = mapEntries[0],
            NearbyEntries = mapEntries.Skip(1).ToList()
        };
    }

    private async Task<List<GeoLocation>> GetNearbyLocations(MapEntriesRequest request)
    {
        var locations = await _locationRepository.GetAllWithinBounds(request.Bounds.NorthEast, request.Bounds.SouthWest);

        return locations;
    }

    private void ProcessLocations(
        List<GeoLocation> locations,
        Dictionary<GeoLocation, Task<string>> responseTasksByLocation,
        HttpClient client)
    {
        foreach (var nearbyLocation in locations)
        {
            var url = $"http://api.openweathermap.org/data/2.5/air_pollution?lat={nearbyLocation.Latitude}&lon={nearbyLocation.Longitude}&appid={_apiKey}";

            var responseString = client.GetStringAsync(url);

            responseTasksByLocation.Add(nearbyLocation, responseString);
        }
    }
}
