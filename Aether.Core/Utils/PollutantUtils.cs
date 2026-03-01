using Aether.Core.Models;

namespace Aether.Core.Utils;

public static class PollutantUtils
{
    public static Pollutant SulfurDioxide(double concentration)
    {
        if (concentration >= 0 && concentration <= 20) return new Pollutant("Sulfur Dioxide", 1, concentration, 350);

        if (concentration > 20 && concentration <= 80) return new Pollutant("Sulfur Dioxide", 2, concentration, 350);

        if (concentration > 80 && concentration <= 250) return new Pollutant("Sulfur Dioxide", 3, concentration, 350); 

        if (concentration > 250 && concentration <= 350) return new Pollutant("Sulfur Dioxide", 4, concentration, 350);

        return new Pollutant("Sulfur Dioxide", 5, concentration, 350);
    }

    public static Pollutant NitrogenDioxide(double concentration)
    {
        if (concentration >= 0 && concentration <= 40) return new Pollutant("Nitrogen Dioxide", 1, concentration, 200);

        if (concentration > 40 && concentration <= 70) return new Pollutant("Nitrogen Dioxide", 2, concentration, 200);

        if (concentration > 70 && concentration <= 150) return new Pollutant("Nitrogen Dioxide", 3, concentration, 200);

        if (concentration > 150 && concentration <= 200) return new Pollutant("Nitrogen Dioxide", 4, concentration, 200);

        return new Pollutant("Nitrogen Dioxide", 5, concentration, 200);
    }

    public static Pollutant NitrogenOxide(double concentration)
    {
        if (concentration >= 0 && concentration <= 40) return new Pollutant("Nitrogen Oxide", 1, concentration, 200);

        if (concentration > 40 && concentration <= 70) return new Pollutant("Nitrogen Oxide", 2, concentration, 200);

        if (concentration > 70 && concentration <= 150) return new Pollutant("Nitrogen Oxide", 3, concentration, 200);

        if (concentration > 150 && concentration <= 200) return new Pollutant("Nitrogen Oxide", 4, concentration, 200);

        return new Pollutant("Nitrogen Oxide", 5, concentration, 200);
    }

    public static Pollutant ParticulateMatter_10(double concentration)
    {
        if (concentration >= 0 && concentration <= 20) return new Pollutant("Particulate Matter 10", 1, concentration, 200);

        if (concentration > 20 && concentration <= 50) return new Pollutant("Particulate Matter 10", 2, concentration, 200);

        if (concentration > 50 && concentration <= 100) return new Pollutant("Particulate Matter 10", 3, concentration, 200);

        if (concentration > 100 && concentration <= 200) return new Pollutant("Particulate Matter 10", 4, concentration, 200);

        return new Pollutant("Particulate Matter 10", 5, concentration, 200);
    }

    public static Pollutant ParticulateMatter_25(double concentration)
    {
        if (concentration >= 0 && concentration <= 10) return new Pollutant("Particulate Matter 25", 1, concentration, 75);

        if (concentration > 10 && concentration <= 25) return new Pollutant("Particulate Matter 25", 2, concentration, 75);

        if (concentration > 25 && concentration <= 50) return new Pollutant("Particulate Matter 25", 3, concentration, 75);

        if (concentration > 50 && concentration <= 75) return new Pollutant("Particulate Matter 25", 4, concentration, 75);

        return new Pollutant("Particulate Matter 25", 5, concentration, 75);
    }

    public static Pollutant Ozone(double concentration)
    {
        if (concentration >= 0 && concentration <= 60) return new Pollutant("Ozone", 1, concentration, 180);

        if (concentration > 60 && concentration <= 100) return new Pollutant("Ozone", 2, concentration, 180);

        if (concentration > 100 && concentration <= 140) return new Pollutant("Ozone", 3, concentration, 180);

        if (concentration > 140 && concentration <= 180) return new Pollutant("Ozone", 4, concentration, 180);

        return new Pollutant("Ozone", 5, concentration, 180);
    }

    public static Pollutant CarbonMonoxide(double concentration)
    {
        if (concentration >= 0 && concentration <= 4400) return new Pollutant("Carbon Monoxide", 1, concentration, 15400);

        if (concentration > 4400 && concentration <= 9400) return new Pollutant("Carbon Monoxide", 2, concentration, 15400);

        if (concentration > 9400 && concentration <= 12400) return new Pollutant("Carbon Monoxide", 3, concentration, 15400); 

        if (concentration > 12400 && concentration <= 15400) return new Pollutant("Carbon Monoxide", 4, concentration, 15400);

        return new Pollutant("Carbon Monoxide", 5, concentration, 15400);
    }

    public static Pollutant Ammonia(double concentration)
    {
        if (concentration >= 0 && concentration <= 60) return new Pollutant("Ammonia", 1, concentration, 180);

        if (concentration > 60 && concentration <= 100) return new Pollutant("Ammonia", 2, concentration, 180);

        if (concentration > 100 && concentration <= 140) return new Pollutant("Ammonia", 3, concentration, 180);

        if (concentration > 140 && concentration <= 180) return new Pollutant("Ammonia", 4, concentration, 180);

        return new Pollutant("Ammonia", 5, concentration, 180);
    }
}
