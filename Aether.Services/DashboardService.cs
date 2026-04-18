using Aether.Core;
using Aether.Core.Entities;
using Aether.Core.Models;
using Aether.Core.Models.Api;
using Aether.Core.Models.DbQueries;
using Aether.Core.Repositories;
using Aether.Core.Requests;
using Aether.Core.Services;
using Aether.Core.Utils;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System.Xml.Linq;

namespace Aether.Services;

public class DashboardService : IDashboardService
{
    private readonly string _apiKey;
    private readonly IAetherUnitOfWork _unitOfWork;
    private readonly ILocationRepository _locationRepository;
    private readonly IAirQualityLocationRepository _airQualityLocationRepository;
    private readonly IAirQualityReadingRepository _airQualityReadingRepository;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger _logger;

    const string baseUrl = "http://api.openweathermap.org";

    public DashboardService(IConfiguration configuration, IAetherUnitOfWork unitOfWork, IMemoryCache memoryCache, ILogger logger)
    {
        _apiKey = configuration["ApiKeys:OpenWeatherMapApiKey"]!;
        _unitOfWork = unitOfWork;
        _locationRepository = unitOfWork.Locations;
        _airQualityLocationRepository = unitOfWork.AirQualityLocations;
        _airQualityReadingRepository = unitOfWork.AirQualityReadings;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task<DashboardView> GetDashboardView(DashboardViewRequest request)
    {
        //await _locationRepository.Run();

        //var manchester = new GeoLocation(53.2844, -2.1443);
        var globalLocation = await GetGlobalLocation(request.Location);
        var (readingEntity, locationEntity) = await TryGetAirQualityReadingAndLocation(request.ReadingId, globalLocation);
        var reading = await GetAirQualityReading(request, globalLocation, readingEntity, locationEntity);
        var changePercentages = GetChangePercentages(reading, readingEntity);

        return new DashboardView
        {
            Location = globalLocation,
            AirQualityReading = reading,
            ChangePercentages = changePercentages
        };
    }

    private async Task<AirQualityReading> GetAirQualityReading(
        DashboardViewRequest request, 
        GlobalLocation globalLocation,
        AirQualityReadingEntity? readingEntity,
        LocationEntity? locationEntity)
    {        
        var reading = await _memoryCache.GetOrCreateAsync($"reading_location({request.Location.Latitude}:{request.Location.Longitude})", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);            
            
            if (locationEntity is null)
            {
                locationEntity = new LocationEntity
                {
                    Name = globalLocation.City,
                    Latitude = request.Location.Latitude,
                    Longitude = request.Location.Longitude
                };

                await _locationRepository.AddAsync(locationEntity);
                await _unitOfWork.CompleteAsync();
            }
            
            if (readingEntity is null)
            {
                var reading = await GetAirQualityReadingFromApi(request.Location);      
                
                readingEntity = new AirQualityReadingEntity
                {
                    LocationId = locationEntity.Id,
                    Index = reading.Index,
                    Aqi = reading.Aqi,
                    CarbonMonoxide = reading.CarbonMonoxide.Concentration,
                    SulfurDioxide = reading.SulfurDioxide.Concentration,
                    NitrogenDioxide = reading.NitrogenDioxide.Concentration,
                    NitrogenOxide = reading.NitrogenOxide.Concentration,
                    Ozone = reading.Ozone.Concentration,
                    ParticulateMatter10 = reading.ParticulateMatter10.Concentration,
                    ParticulateMatter2_5 = reading.ParticulateMatter2_5.Concentration,
                    Ammonia = reading.Ammonia.Concentration,
                    LastUpdated = reading.LastUpdated
                };

                await _airQualityReadingRepository.AddAsync(readingEntity);
                await _unitOfWork.CompleteAsync();

                reading.Id = readingEntity.Id;
                return reading;
            }
            
            // Update if reading is more than a day old
            if (DateTimeOffset.UtcNow > readingEntity.LastUpdated.AddDays(1))
            {
                var reading = await GetAirQualityReadingFromApi(request.Location);                                

                readingEntity.Index = reading.Index;
                readingEntity.Aqi = reading.Aqi;
                readingEntity.CarbonMonoxide = reading.CarbonMonoxide.Concentration;
                readingEntity.CarbonMonoxide = reading.SulfurDioxide.Concentration;
                readingEntity.NitrogenDioxide = reading.NitrogenDioxide.Concentration;
                readingEntity.NitrogenOxide = reading.NitrogenOxide.Concentration;
                readingEntity.Ozone = reading.Ozone.Concentration;
                readingEntity.ParticulateMatter10 = reading.ParticulateMatter10.Concentration;
                readingEntity.ParticulateMatter2_5 = reading.ParticulateMatter2_5.Concentration;
                readingEntity.Ammonia = reading.Ammonia.Concentration;
                readingEntity.LastUpdated = DateTimeOffset.UtcNow;

                await _unitOfWork.CompleteAsync();

                reading.Id = readingEntity.Id;
                return reading;
            }

            return new AirQualityReading
            {
                Id = readingEntity.Id,
                Location = new AirQualityLocation
                {
                    Name = readingEntity.Location!.Name,
                    Latitude = readingEntity.Location!.Latitude,
                    Longitude = readingEntity.Location!.Longitude,
                },
                Index = readingEntity.Index,
                Aqi = readingEntity.Aqi,
                CarbonMonoxide = PollutantUtils.CarbonMonoxide(readingEntity.CarbonMonoxide),
                SulfurDioxide = PollutantUtils.SulfurDioxide(readingEntity.SulfurDioxide),
                NitrogenDioxide = PollutantUtils.NitrogenDioxide(readingEntity.NitrogenDioxide),
                NitrogenOxide = PollutantUtils.NitrogenOxide(readingEntity.NitrogenOxide),
                Ozone = PollutantUtils.Ozone(readingEntity.Ozone),
                ParticulateMatter10 = PollutantUtils.ParticulateMatter_10(readingEntity.ParticulateMatter10),
                ParticulateMatter2_5 = PollutantUtils.ParticulateMatter_25(readingEntity.ParticulateMatter2_5),
                Ammonia = PollutantUtils.Ammonia(readingEntity.Ammonia),
                LastUpdated = readingEntity.LastUpdated
            };
        });

        return reading!;
    }

    private static ChangePercentages GetChangePercentages(AirQualityReading reading, AirQualityReadingEntity? readingEntity)
    {
        if (readingEntity is null)
            return new();

        var carbonMonoxide = MathUtils.PercentageChange(readingEntity.CarbonMonoxide, reading.CarbonMonoxide.Concentration);
        var sulfurDioxide = MathUtils.PercentageChange(readingEntity.SulfurDioxide, reading.SulfurDioxide.Concentration);
        var nitrogenDioxide = MathUtils.PercentageChange(readingEntity.NitrogenDioxide, reading.NitrogenDioxide.Concentration);
        var nitrogenOxide = MathUtils.PercentageChange(readingEntity.NitrogenOxide, reading.NitrogenOxide.Concentration);
        var ozone = MathUtils.PercentageChange(readingEntity.Ozone, reading.Ozone.Concentration);
        var particulateMatter10 = MathUtils.PercentageChange(readingEntity.ParticulateMatter10, reading.ParticulateMatter10.Concentration);
        var particulateMatter2_5 = MathUtils.PercentageChange(readingEntity.ParticulateMatter2_5, reading.ParticulateMatter2_5.Concentration);
        var ammonia = MathUtils.PercentageChange(readingEntity.Ammonia, reading.Ammonia.Concentration);

        return new ChangePercentages
        {
            Aqi = MathUtils.PercentageChange(readingEntity.Aqi, reading.Aqi),
            CarbonMonoxide = Math.Round(carbonMonoxide, 2),
            SulfurDioxide = Math.Round(sulfurDioxide, 2),
            NitrogenDioxide = Math.Round(nitrogenDioxide, 2),
            NitrogenOxide = Math.Round(nitrogenOxide, 2),
            Ozone = Math.Round(ozone, 2),
            ParticulateMatter10 = Math.Round(particulateMatter10, 2),
            ParticulateMatter2_5 = Math.Round(particulateMatter2_5, 2),
            Ammonia = Math.Round(ammonia, 2)
        };
    }

    private async Task<(AirQualityReadingEntity?, LocationEntity?)> TryGetAirQualityReadingAndLocation(int? readingId, GlobalLocation globalLocation)
    {
        AirQualityReadingEntity? readingEntity;
        LocationEntity? locationEntity;
        if (readingId.HasValue && readingId != 0)
        {
            readingEntity = await _airQualityReadingRepository.GetAsync(readingId.Value);
            if (readingEntity is null)
                return (null, null);

            locationEntity = await _locationRepository.GetAsync(readingEntity.LocationId);

            return (readingEntity, locationEntity);
        }

        locationEntity = await _locationRepository.GetByName(globalLocation.City);
        if (locationEntity is null)
            return (null, null);

        readingEntity = await _airQualityReadingRepository.GetByLocationId(locationEntity.Id);

        return (readingEntity, locationEntity);
    }

    private async Task<GlobalLocation> GetGlobalLocation(GeoLocation geoLocation)
    {
        var location = await _memoryCache.GetOrCreateAsync($"location({geoLocation.Latitude}:{geoLocation.Longitude})", async entry =>
        {
            using var client = new HttpClient();
            var url = $"{baseUrl}/geo/1.0/reverse?lat={geoLocation.Latitude}&lon={geoLocation.Longitude}&appid={_apiKey}";
            var responseString = await client.GetStringAsync(url);

            var response = JsonConvert.DeserializeObject<List<ApiGlobalLocation>>(responseString)
                ?? throw new Exception("Air quality index not found for this location");

            var result = response.FirstOrDefault() ?? throw new ArgumentException($"Location was not found for lat: {geoLocation.Latitude}, lon: {geoLocation.Longitude}");

            return new GlobalLocation
            {
                Country = result.Country,
                State = result.State,
                City = result.Name,
                Latitude = result.Latitude,
                Longitude = result.Longitude,
            };
        });

        return location!;
    }

    public async Task<MapEntriesView> GetMapEntries(MapEntriesViewRequest request)
    {
        /* TODOs:
        Monthly background job that updates indexes every 3 months.  
        Potential Bug - Some readings come through with no id and so get created despite the location 
        for that reading being present in the locations table. Centre reading below will never have an id, 
        which could be related.
        */
        var centreReading = await _memoryCache.GetOrCreateAsync($"reading_location({request.Centre.Latitude}:{request.Centre.Longitude})", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
            return await GetAirQualityReadingFromApi(request.Centre);
        });

        // Split bounds into cells where the lat/lng step represents the size of each cell.
        var latSpan = request.Bounds.NorthEast.Latitude - request.Bounds.SouthWest.Latitude;
        var lngSpan = request.Bounds.NorthEast.Longitude - request.Bounds.SouthWest.Longitude;
        var gridSize = 5;
        var latStep = latSpan / gridSize;
        var lngStep = lngSpan / gridSize;

        var airQualityData = await _airQualityLocationRepository.GetAirQualityDataWithinBounds(request.Bounds.NorthEast, request.Bounds.SouthWest, latStep, lngStep);
        var readingsData = airQualityData.Where(x => x.ReadingId.HasValue).ToList();
        var missingData = airQualityData.Where(x => x.ReadingId is null).ToList();       

        var mapEntries = readingsData.ConvertAll(r => new MapEntry
        {
            AirQualityReading = new AirQualityReading
            {
                Id = r.ReadingId!.Value,
                Location = new AirQualityLocation 
                { 
                    Name = r.LocationName,
                    Latitude = r.Latitude,
                    Longitude = r.Longitude,
                },
                Index = r.Index,
                Aqi = r.Aqi,
                CarbonMonoxide = PollutantUtils.CarbonMonoxide(r.CarbonMonoxide),
                SulfurDioxide = PollutantUtils.SulfurDioxide(r.SulfurDioxide),
                NitrogenDioxide = PollutantUtils.NitrogenDioxide(r.NitrogenDioxide),
                NitrogenOxide = PollutantUtils.NitrogenOxide(r.NitrogenOxide),
                Ozone = PollutantUtils.Ozone(r.Ozone),
                ParticulateMatter10 = PollutantUtils.ParticulateMatter_10(r.ParticulateMatter10),
                ParticulateMatter2_5 = PollutantUtils.ParticulateMatter_25(r.ParticulateMatter2_5),
                Ammonia = PollutantUtils.Ammonia(r.Ammonia),
                LastUpdated = r.LastUpdated
            }
        });

        if (missingData.Count > 0)
        {
            var responseTasksByLocation = new Dictionary<int, Task<string>>();
            var readingsToCache = new List<AirQualityReadingEntity>();            

            ProcessLocations(missingData, responseTasksByLocation);

            await Task.WhenAll(responseTasksByLocation.Values);

            foreach (var item in missingData)
            {
                //var responseString = "{\"coord\":{\"lon\":-2.3637,\"lat\":53.4541},\"list\":[{\"main\":{\"aqi\":2},\"components\":{\"co\":119.47,\"no\":0,\"no2\":4.88,\"o3\":68.24,\"so2\":0.81,\"pm2_5\":2.94,\"pm10\":3.35,\"nh3\":7.54},\"dt\":1746995138}]}";
                var responseString = responseTasksByLocation[item.LocationId].Result;
                var response = JsonConvert.DeserializeObject<ApiAirQualityReading>(responseString)
                    ?? throw new Exception("Air quality index not found for this location");
                
                var reading = ApiAirQualityReading.ToReading(response, item.LocationName);

                if (item is not null)
                {
                    var readingToCache = new AirQualityReadingEntity
                    {
                        LocationId = item.LocationId,
                        Index = reading.Index,
                        Aqi = reading.Aqi,
                        CarbonMonoxide = reading.CarbonMonoxide.Concentration,
                        SulfurDioxide = reading.SulfurDioxide.Concentration,
                        NitrogenDioxide = reading.NitrogenDioxide.Concentration,
                        NitrogenOxide = reading.NitrogenOxide.Concentration,
                        Ozone = reading.Ozone.Concentration,
                        ParticulateMatter10 = reading.ParticulateMatter10.Concentration,
                        ParticulateMatter2_5 = reading.ParticulateMatter2_5.Concentration,
                        Ammonia = reading.Ammonia.Concentration,
                        LastUpdated = reading.LastUpdated,
                    };

                    readingsToCache.Add(readingToCache);
                }
            }

            //await _airQualityReadingRepository.InsertReadingMultiple(readingsToCache);
            var locationIds = readingsToCache.Count > 0 ? await _airQualityReadingRepository.GetAllLocationIds() : [];
            if (locationIds.Count > 0)
                readingsToCache = readingsToCache.Where(x => locationIds.Contains(x.LocationId)).ToList();

            await _airQualityReadingRepository.AddRangeAsync(readingsToCache);
            await _unitOfWork.CompleteAsync();

            foreach (var readingEntity in readingsToCache)
            {
                var item = missingData.First(x => x.LocationId == readingEntity.LocationId);

                var reading = new AirQualityReading
                {
                    Id = readingEntity.Id,
                    Location = new AirQualityLocation
                    {
                        Name = item.LocationName,
                        Latitude = item.Latitude,
                        Longitude = item.Longitude,
                    },
                    Index = readingEntity.Index,
                    Aqi = readingEntity.Aqi,
                    CarbonMonoxide = PollutantUtils.CarbonMonoxide(readingEntity.CarbonMonoxide),
                    SulfurDioxide = PollutantUtils.SulfurDioxide(readingEntity.SulfurDioxide),
                    NitrogenDioxide = PollutantUtils.NitrogenDioxide(readingEntity.NitrogenDioxide),
                    NitrogenOxide = PollutantUtils.NitrogenOxide(readingEntity.NitrogenOxide),
                    Ozone = PollutantUtils.Ozone(readingEntity.Ozone),
                    ParticulateMatter10 = PollutantUtils.ParticulateMatter_10(readingEntity.ParticulateMatter10),
                    ParticulateMatter2_5 = PollutantUtils.ParticulateMatter_25(readingEntity.ParticulateMatter2_5),
                    Ammonia = PollutantUtils.Ammonia(readingEntity.Ammonia),
                    LastUpdated = readingEntity.LastUpdated
                };

                mapEntries.Add(new MapEntry
                {
                    AirQualityReading = reading
                });
            }
        }        

        return new MapEntriesView 
        { 
            Centre = new MapEntry { AirQualityReading = centreReading! },
            NearbyEntries = mapEntries.Skip(1).ToList()
        };
    }

    private async Task<AirQualityReading> GetAirQualityReadingFromApi(GeoLocation location)
    {
        var client = new HttpClient();
        var url = $"{baseUrl}/data/2.5/air_pollution?lat={location.Latitude}&lon={location.Longitude}&appid={_apiKey}";
        var responseString = await client.GetStringAsync(url);

        var response = JsonConvert.DeserializeObject<ApiAirQualityReading>(responseString)
            ?? throw new Exception("Air quality index not found for this location");

        var globalLocation = await GetGlobalLocation(location);

        return ApiAirQualityReading.ToReading(response, globalLocation.City);
    }

    private void ProcessLocations(
        List<DbAirQualityItem> items,
        Dictionary<int, Task<string>> responseTasksByLocation)
    {
        var client = new HttpClient();
        foreach (var item in items)
        {
            var url = $"{baseUrl}/data/2.5/air_pollution?lat={item.Latitude}&lon={item.Longitude}&appid={_apiKey}";
            var responseString = client.GetStringAsync(url);

            responseTasksByLocation.Add(item.LocationId, responseString);
        }
    }
}
