using Aether.Core;
using Aether.Core.Entities;
using Aether.Core.Models;
using Aether.Core.Models.Api;
using Aether.Core.Repositories;
using Aether.Core.Requests;
using Aether.Core.Services;
using Aether.Core.Utils;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace Aether.Services;

public class DashboardService : IDashboardService
{
    private readonly string _apiKey;
    private readonly IAetherUnitOfWork _unitOfWork;
    private readonly ILocationRepository _locationRepository;
    private readonly IAirQualityLocationRepository _airQualityLocationRepository;
    private readonly IAirQualityReadingRepository _airQualityReadingRepository;
    private readonly IMemoryCache _memoryCache;

    const string baseUrl = "http://api.openweathermap.org";

    public DashboardService(IConfiguration configuration, IAetherUnitOfWork unitOfWork, IMemoryCache memoryCache)
    {
        _apiKey = configuration["ApiKeys:OpenWeatherMapApiKey"]!;
        _unitOfWork = unitOfWork;
        _locationRepository = unitOfWork.Locations;
        _airQualityLocationRepository = unitOfWork.AirQualityLocations;
        _airQualityReadingRepository = unitOfWork.AirQualityReadings;
        _memoryCache = memoryCache;
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
        var reading = await _memoryCache.GetOrCreateAsync($"location({location.Latitude}:{location.Longitude})", async entry =>
        {
            using var client = new HttpClient();
            var url = $"{baseUrl}/data/2.5/air_pollution?lat={location.Latitude}&lon={location.Longitude}&appid={_apiKey}";            
            var responseString = await client.GetStringAsync(url);

            var response = JsonConvert.DeserializeObject<ApiAirQualityReading>(responseString)
                ?? throw new Exception("Air quality index not found for this location");

            return ApiAirQualityReading.ToReading(response);
        });

        return reading!;
    }

    private async Task<GlobalLocation> GetGlobalLocation(GeoLocation geoLocation)
    {
        using var client = new HttpClient();
        var url = $"{baseUrl}/geo/1.0/reverse?lat={geoLocation.Latitude}&lon={geoLocation.Longitude}&appid={_apiKey}";        
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
        /* TODOs: Pollutants should be a list within AirQuality and have a name and possibly a unit field.
        Monthly background job that updates indexes every 3 months.  */

        using var client = new HttpClient();

        var centreReading = await _memoryCache.GetOrCreateAsync($"location({request.Centre.Latitude}:{request.Centre.Longitude})", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

            var url = $"{baseUrl}/data/2.5/air_pollution?lat={request.Centre.Latitude}&lon={request.Centre.Longitude}&appid={_apiKey}";
            var response = await client.GetFromJsonAsync<ApiAirQualityReading>(url) 
                ?? throw new Exception("Air quality index not found for this location");

            return ApiAirQualityReading.ToReading(response);
        });         

        // Split bounds into cells where the lat/lng step represents the size of each cell.
        var latSpan = request.Bounds.NorthEast.Latitude - request.Bounds.SouthWest.Latitude;
        var lngSpan = request.Bounds.NorthEast.Longitude - request.Bounds.SouthWest.Longitude;
        var gridSize = 10;
        var latStep = latSpan / gridSize;
        var lngStep = lngSpan / gridSize;

        var airQualityData = await _airQualityLocationRepository.GetAirQualityDataWithinBounds(request.Bounds.NorthEast, request.Bounds.SouthWest, latStep, lngStep);
        var readingsData = airQualityData.Where(x => x.ReadingId.HasValue).ToList();
        var missingData = airQualityData.Where(x => x.ReadingId is null).ToList();

        var mapEntries = readingsData.ConvertAll(r => new MapEntry
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

        var missingLocations = missingData.ConvertAll(x => new GeoLocation(x.Latitude, x.Longitude)); 
        var responseTasksByLocation = new Dictionary<GeoLocation, Task<string>>();
        var readingsToCache = new List<AirQualityReadingEntity>();

        ProcessLocations(missingLocations, responseTasksByLocation, client);

        await Task.WhenAll(responseTasksByLocation.Values);

        foreach (var location in missingLocations)
        {
            var locationEntity = missingData.FirstOrDefault(x => x.Latitude == location.Latitude && x.Longitude == location.Longitude);

            // Temporary until readings are cached
            //var responseString = "{\"coord\":{\"lon\":-2.3637,\"lat\":53.4541},\"list\":[{\"main\":{\"aqi\":2},\"components\":{\"co\":119.47,\"no\":0,\"no2\":4.88,\"o3\":68.24,\"so2\":0.81,\"pm2_5\":2.94,\"pm10\":3.35,\"nh3\":7.54},\"dt\":1746995138}]}";
            var responseString = responseTasksByLocation[location].Result;
            var response = JsonConvert.DeserializeObject<ApiAirQualityReading>(responseString)
                ?? throw new Exception("Air quality index not found for this location");

            var reading = ApiAirQualityReading.ToReading(response);

            if (locationEntity is not null)
            {
                var readingToCache = new AirQualityReadingEntity
                {
                    LocationId = locationEntity.LocationId,
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

        await _airQualityReadingRepository.InsertReadingMultiple(readingsToCache);

        Console.WriteLine("Request finished");
        Thread.Sleep(1000);
        Console.WriteLine("Finished sleeping ..");

        return new MapEntriesView 
        { 
            Centre = new MapEntry { AirQualityReading = centreReading! },
            NearbyEntries = mapEntries.Skip(1).ToList()
        };
    }

    private void ProcessLocations(
        List<GeoLocation> locations,
        Dictionary<GeoLocation, Task<string>> responseTasksByLocation,
        HttpClient client)
    {
        foreach (var location in locations)
        {
            var url = $"{baseUrl}/data/2.5/air_pollution?lat={location.Latitude}&lon={location.Longitude}&appid={_apiKey}";
            var responseString = client.GetStringAsync(url);

            responseTasksByLocation.Add(location, responseString);
        }
    }
}
