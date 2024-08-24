using Aether.Core.Models;

namespace Aether.Core.Utils;

public static class PollutantUtils
{
    public static Pollutant SulfurDioxide(double concentration)
    {
        if (concentration >= 0 && concentration <= 20) return new Pollutant(1, concentration);

        if (concentration > 20 && concentration <= 80) return new Pollutant(2, concentration);

        if (concentration > 80 && concentration <= 250) return new Pollutant(3, concentration); 

        if (concentration > 250 && concentration <= 350) return new Pollutant(4, concentration);

        return new Pollutant(5, concentration);
    }

    public static Pollutant NitrogenDioxide(double concentration)
    {
        if (concentration >= 0 && concentration <= 40) return new Pollutant(1, concentration);

        if (concentration > 40 && concentration <= 70) return new Pollutant(2, concentration);

        if (concentration > 70 && concentration <= 150) return new Pollutant(3, concentration);

        if (concentration > 150 && concentration <= 200) return new Pollutant(4, concentration);

        return new Pollutant(5, concentration);
    }

    public static Pollutant NitrogenOxide(double concentration)
    {
        if (concentration >= 0 && concentration <= 40) return new Pollutant(1, concentration);

        if (concentration > 40 && concentration <= 70) return new Pollutant(2, concentration);

        if (concentration > 70 && concentration <= 150) return new Pollutant(3, concentration);

        if (concentration > 150 && concentration <= 200) return new Pollutant(4, concentration);

        return new Pollutant(5, concentration);
    }

    public static Pollutant ParticulateMatter_10(double concentration)
    {
        if (concentration >= 0 && concentration <= 20) return new Pollutant(1, concentration);

        if (concentration > 20 && concentration <= 50) return new Pollutant(2, concentration);

        if (concentration > 50 && concentration <= 100) return new Pollutant(3, concentration);

        if (concentration > 100 && concentration <= 200) return new Pollutant(4, concentration);

        return new Pollutant(5, concentration);
    }

    public static Pollutant ParticulateMatter_25(double concentration)
    {
        if (concentration >= 0 && concentration <= 10) return new Pollutant(1, concentration);

        if (concentration > 10 && concentration <= 25) return new Pollutant(2, concentration);

        if (concentration > 25 && concentration <= 50) return new Pollutant(3, concentration);

        if (concentration > 50 && concentration <= 75) return new Pollutant(4, concentration);

        return new Pollutant(5, concentration);
    }

    public static Pollutant Ozone(double concentration)
    {
        if (concentration >= 0 && concentration <= 60) return new Pollutant(1, concentration);

        if (concentration > 60 && concentration <= 100) return new Pollutant(2, concentration);

        if (concentration > 100 && concentration <= 140) return new Pollutant(3, concentration);

        if (concentration > 140 && concentration <= 180) return new Pollutant(4, concentration);

        return new Pollutant(5, concentration);
    }

    public static Pollutant CarbonMonoxide(double concentration)
    {
        if (concentration >= 0 && concentration <= 4400) return new Pollutant(1, concentration);

        if (concentration > 4400 && concentration <= 9400) return new Pollutant(2, concentration);

        if (concentration > 9400 && concentration <= 12400) return new Pollutant(3, concentration); 

        if (concentration > 12400 && concentration <= 15400) return new Pollutant(4, concentration);

        return new Pollutant(5, concentration);
    }

    public static Pollutant Ammonia(double concentration)
    {
        if (concentration >= 0 && concentration <= 60) return new Pollutant(1, concentration);

        if (concentration > 60 && concentration <= 100) return new Pollutant(2, concentration);

        if (concentration > 100 && concentration <= 140) return new Pollutant(3, concentration);

        if (concentration > 140 && concentration <= 180) return new Pollutant(4, concentration);

        return new Pollutant(5, concentration);
    }
}
