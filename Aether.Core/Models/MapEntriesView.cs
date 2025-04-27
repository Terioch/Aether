namespace Aether.Core.Models;

public sealed class MapEntriesView
{
    public required MapEntry Centre { get; set; }

    public required List<MapEntry> NearbyEntries { get; set; }
}
