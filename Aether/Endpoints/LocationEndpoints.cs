using Aether.Core;
using Aether.Core.Services;

namespace Aether.Endpoints
{
    public sealed class LocationEndpoints : IEndpointMapper
    {
        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            app.MapPost("api/locations", CreateLocations);            
        }

        private static async Task<IResult> CreateLocations(ILocationService service)
        {
            await service.CreateLocations();

            return Results.Ok();
        }
    }
}
