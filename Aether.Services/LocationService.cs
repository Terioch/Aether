using Aether.Core.Entities;
using Aether.Core.Models.Api;
using Aether.Core.Repositories;
using Aether.Core.Services;
using Newtonsoft.Json;

namespace Aether.Services;

public class LocationService : ILocationService
{
    private readonly IAetherUnitOfWork _unitOfWork;
    private readonly ILocationRepository _locationRepository;

    public LocationService(IAetherUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _locationRepository = unitOfWork.Locations;
    }    

    public async Task CreateLocations()
    {
        // 100 locations processed
        const string baseUrl = "http://geodb-free-service.wirefreethought.com/v1/geo/places?limit=10";

        var offset = 100;
        var tasks = new List<Task<List<LocationEntity>>>();
        var client = new HttpClient();        

        while (offset <= 90)
        {    
            var url = baseUrl + $"&offset={offset}";
            tasks.Add(GetLocationsAsync(url, client));
            offset += 10;
        }

        var results = await Task.WhenAll(tasks);
        var locations = results.SelectMany(x => x);       

        await _locationRepository.AddRangeAsync(locations);
        await _unitOfWork.CompleteAsync();
    }

    private static async Task<List<LocationEntity>> GetLocationsAsync(string url, HttpClient client)
    {
        var responseString = await client.GetStringAsync(url);
        var response = JsonConvert.DeserializeObject<GeodbCitiesResponse>(responseString)
            ?? throw new Exception("Cities could not be found");

        return response.Data.ConvertAll(x => new LocationEntity
        {
            Name = x.Name,
            Latitude = x.Latitude,
            Longitude = x.Longitude
        });
    }
}
