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
        const string baseUrl = "http://geodb-free-service.wirefreethought.com/v1/geo/places?limit=10";
        var offset = 0;
        var allLocations = new List<LocationEntity>();

        while (offset < 2)
        {
            var url = baseUrl + $"&offset={offset}";
            var client = new HttpClient();
            var responseString = await client.GetStringAsync(url);

            var response = JsonConvert.DeserializeObject<GeodbCitiesResponse>(responseString)
                ?? throw new Exception("Cities could not be found");

            var locations = response.Data.ConvertAll(x => new LocationEntity 
            { 
                Latitude = x.Latitude, Longitude = x.Longitude 
            });    

            allLocations.AddRange(locations);

            offset += 10;
        }

        await _locationRepository.AddRangeAsync(allLocations);

        await _unitOfWork.CompleteAsync();
    }
}
