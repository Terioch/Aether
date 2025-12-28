namespace Aether.Core.Utils;
public static class LocationUtils
{
    public static GeoLocation ToRoundedGeoLocation(this GeoLocation location)
    {
        return new GeoLocation(Math.Round(location.Latitude, 3), Math.Round(location.Longitude, 3));
    }
}
