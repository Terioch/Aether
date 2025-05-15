using Aether.Core;
using Aether.Core.Entities;
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
    private readonly IAetherUnitOfWork _unitOfWork;
    private readonly ILocationRepository _locationRepository;
    private readonly IAirQualityLocationRepository _airQualityLocationRepository;
    private readonly IAirQualityReadingRepository _airQualityReadingRepository;

    public DashboardService(IConfiguration configuration, IAetherUnitOfWork unitOfWork)
    {
        _apiKey = configuration["ApiKeys:OpenWeatherMapApiKey"]!;
        _unitOfWork = unitOfWork;
        _locationRepository = unitOfWork.Locations;
        _airQualityLocationRepository = unitOfWork.AirQualityLocations;
        _airQualityReadingRepository = unitOfWork.AirQualityReadings;
    }

    public async Task<DashboardView> GetDashboardView(GeoLocation geoLocation)
    {
        //var manchester = new GeoLocation(53.2844, -2.1443);
        var index = await GetAirQualityIndex(geoLocation);
        var location = await GetGlobalLocation(geoLocation);

        return new DashboardView
        {
            Location = location,
            AirQualityReading = index
        };
    }

    private async Task<AirQualityReading> GetAirQualityIndex(GeoLocation location)
    {
        var url = $"http://api.openweathermap.org/data/2.5/air_pollution?lat={location.Latitude}&lon={location.Longitude}&appid={_apiKey}";

        using var client = new HttpClient();

        var responseString = await client.GetStringAsync(url);

        var response = JsonConvert.DeserializeObject<ApiAirQualityReading>(responseString)
            ?? throw new Exception("Air quality index not found for this location");

        var item = response.List[0];

        return new AirQualityReading
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
        Urgent TODO: Ensure locations in database are all a minimum distance apart 
        Urgent TODO: Delete all locations in database, add a name column, then re-import
        Duplicate key error location id: ... */

        var locationsForApi = new List<GeoLocation> { request.Centre };

        var result = await _airQualityLocationRepository.GetAirQualityDataWithinBounds(request.Bounds.NorthEast, request.Bounds.SouthWest);

        var mapEntries = result.Readings.ConvertAll(r => new MapEntry
        {
            AirQualityReading = new AirQualityReading
            {
                Location = new(r.Latitude, r.Longitude),
                Index = r.Index,
                CarbonMonoxide = PollutantUtils.CarbonMonoxide(r.CarbonMonoxide),
                SulfurDioxide = PollutantUtils.SulfurDioxide(r.SulfurDioxide),
                NitrogenDioxide = PollutantUtils.NitrogenDioxide(r.NitrogenDioxide),
                NitrogenOxide = PollutantUtils.NitrogenOxide(r.NitrogenOxide),
                Ozone = PollutantUtils.Ozone(r.Ozone),
                ParticulateMatter10 = PollutantUtils.ParticulateMatter_10(r.ParticulateMatter10),
                ParticulateMatter2_5 = PollutantUtils.ParticulateMatter_25(r.ParticulateMatter2_5),
                Ammonia = PollutantUtils.Ammonia(r.Ammonia)
            }
        });

        var missingLocations = result.MissingLocations.ConvertAll(x => new GeoLocation(x.Latitude, x.Longitude));
        locationsForApi.AddRange(missingLocations);

        var responseTasksByLocation = new Dictionary<GeoLocation, Task<string>>();
        var readingsToCache = new List<AirQualityReadingEntity>();
        using var client = new HttpClient();        

        ProcessLocations(locationsForApi, responseTasksByLocation, client);

        await Task.WhenAll(responseTasksByLocation.Values);

        foreach (var location in locationsForApi)
        {
            var locationEntity = result.MissingLocations.FirstOrDefault(x => x.Latitude ==  location.Latitude && x.Longitude == location.Longitude);

            // Temporary until readings are cached
            //var responseString = "{\"coord\":{\"lon\":-2.3637,\"lat\":53.4541},\"list\":[{\"main\":{\"aqi\":2},\"components\":{\"co\":119.47,\"no\":0,\"no2\":4.88,\"o3\":68.24,\"so2\":0.81,\"pm2_5\":2.94,\"pm10\":3.35,\"nh3\":7.54},\"dt\":1746995138}]}";
            var responseString = responseTasksByLocation[location].Result;
            var response = JsonConvert.DeserializeObject<ApiAirQualityReading>(responseString)
                ?? throw new Exception("Air quality index not found for this location");

            var item = response.List[0];

            var reading = new AirQualityReading
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

            if (locationEntity is not null)
            {
                var readingToCache = new AirQualityReadingEntity
                {
                    LocationId = locationEntity.Id,
                    Index = reading.Index,
                    CarbonMonoxide = reading.CarbonMonoxide.Concentration,
                    SulfurDioxide = reading.SulfurDioxide.Concentration,
                    NitrogenDioxide = reading.NitrogenDioxide.Concentration,
                    NitrogenOxide = reading.NitrogenOxide.Concentration,
                    Ozone = reading.Ozone.Concentration,
                    ParticulateMatter10 = reading.ParticulateMatter10.Concentration,
                    ParticulateMatter2_5 = reading.ParticulateMatter2_5.Concentration,
                    Ammonia = reading.Ammonia.Concentration
                };

                readingsToCache.Add(readingToCache);
            }          

            var mapEntry = new MapEntry
            {
                AirQualityReading = reading
            };

            mapEntries.Add(mapEntry);
        }

        await _airQualityReadingRepository.AddRangeAsync(readingsToCache);

        await _unitOfWork.CompleteAsync();

        return new MapEntriesView 
        { 
            Centre = mapEntries[0],
            NearbyEntries = mapEntries.Skip(1).ToList()
        };
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
