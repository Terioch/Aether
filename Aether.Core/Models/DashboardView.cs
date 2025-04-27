namespace Aether.Core.Models
{
    public sealed class DashboardView
    {
        public GlobalLocation? Location { get; set; }

        public required AirQuality AirQuality { get; set; }
    }
}
