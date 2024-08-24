namespace Aether.Core.Models;

public class Pollutant(int index, double concentration)
{
    public int Index { get; set; } = index;

    public double Concentration { get; set; } = concentration;
}
