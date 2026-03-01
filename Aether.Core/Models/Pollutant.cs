namespace Aether.Core.Models;

public class Pollutant(string name, int index, double concentration, double max)
{
    public string Name { get; set; } = name;

    public int Index { get; set; } = index;

    public double Concentration { get; set; } = concentration;
    
    public double Max { get; set; } = max;
}
