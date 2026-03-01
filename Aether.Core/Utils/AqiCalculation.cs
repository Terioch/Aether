namespace Aether.Core.Utils;
public static class AqiCalculation
{
    private class AqiBreakpoint
    {
        public double ConcentrationLow { get; set; }
        public double ConcentrationHigh { get; set; }
        public int AqiLow { get; set; }
        public int AqiHigh { get; set; }
    }

    // AQI Breakpoints for each pollutant (µg/m³, 24-hour average)
    private static readonly Dictionary<string, List<AqiBreakpoint>> Breakpoints = new()
    {
        ["PM2.5"] = new List<AqiBreakpoint>
        {
            new() { ConcentrationLow = 0.0, ConcentrationHigh = 12.0, AqiLow = 0, AqiHigh = 50 },
            new() { ConcentrationLow = 12.1, ConcentrationHigh = 35.4, AqiLow = 51, AqiHigh = 100 },
            new() { ConcentrationLow = 35.5, ConcentrationHigh = 55.4, AqiLow = 101, AqiHigh = 150 },
            new() { ConcentrationLow = 55.5, ConcentrationHigh = 150.4, AqiLow = 151, AqiHigh = 200 },
            new() { ConcentrationLow = 150.5, ConcentrationHigh = 250.4, AqiLow = 201, AqiHigh = 300 },
            new() { ConcentrationLow = 250.5, ConcentrationHigh = 500.4, AqiLow = 301, AqiHigh = 500 }
        },

        ["PM10"] = new List<AqiBreakpoint>
        {
            new() { ConcentrationLow = 0, ConcentrationHigh = 54, AqiLow = 0, AqiHigh = 50 },
            new() { ConcentrationLow = 55, ConcentrationHigh = 154, AqiLow = 51, AqiHigh = 100 },
            new() { ConcentrationLow = 155, ConcentrationHigh = 254, AqiLow = 101, AqiHigh = 150 },
            new() { ConcentrationLow = 255, ConcentrationHigh = 354, AqiLow = 151, AqiHigh = 200 },
            new() { ConcentrationLow = 355, ConcentrationHigh = 424, AqiLow = 201, AqiHigh = 300 },
            new() { ConcentrationLow = 425, ConcentrationHigh = 604, AqiLow = 301, AqiHigh = 500 }
        },

        ["O3"] = new List<AqiBreakpoint>
        {
            new() { ConcentrationLow = 0, ConcentrationHigh = 108, AqiLow = 0, AqiHigh = 50 },
            new() { ConcentrationLow = 109, ConcentrationHigh = 140, AqiLow = 51, AqiHigh = 100 },
            new() { ConcentrationLow = 141, ConcentrationHigh = 170, AqiLow = 101, AqiHigh = 150 },
            new() { ConcentrationLow = 171, ConcentrationHigh = 210, AqiLow = 151, AqiHigh = 200 },
            new() { ConcentrationLow = 211, ConcentrationHigh = 400, AqiLow = 201, AqiHigh = 300 }
        },

        ["CO"] = new List<AqiBreakpoint>
        {
            new() { ConcentrationLow = 0, ConcentrationHigh = 4400, AqiLow = 0, AqiHigh = 50 },
            new() { ConcentrationLow = 4500, ConcentrationHigh = 9400, AqiLow = 51, AqiHigh = 100 },
            new() { ConcentrationLow = 9500, ConcentrationHigh = 12400, AqiLow = 101, AqiHigh = 150 },
            new() { ConcentrationLow = 12500, ConcentrationHigh = 15400, AqiLow = 151, AqiHigh = 200 },
            new() { ConcentrationLow = 15500, ConcentrationHigh = 30400, AqiLow = 201, AqiHigh = 300 },
            new() { ConcentrationLow = 30500, ConcentrationHigh = 50400, AqiLow = 301, AqiHigh = 500 }
        },

        ["NO2"] = new List<AqiBreakpoint>
        {
            new() { ConcentrationLow = 0, ConcentrationHigh = 100, AqiLow = 0, AqiHigh = 50 },
            new() { ConcentrationLow = 101, ConcentrationHigh = 360, AqiLow = 51, AqiHigh = 100 },
            new() { ConcentrationLow = 361, ConcentrationHigh = 649, AqiLow = 101, AqiHigh = 150 },
            new() { ConcentrationLow = 650, ConcentrationHigh = 1244, AqiLow = 151, AqiHigh = 200 },
            new() { ConcentrationLow = 1245, ConcentrationHigh = 2049, AqiLow = 201, AqiHigh = 300 }
        },

        ["SO2"] = new List<AqiBreakpoint>
        {
            new() { ConcentrationLow = 0, ConcentrationHigh = 91, AqiLow = 0, AqiHigh = 50 },
            new() { ConcentrationLow = 92, ConcentrationHigh = 196, AqiLow = 51, AqiHigh = 100 },
            new() { ConcentrationLow = 197, ConcentrationHigh = 486, AqiLow = 101, AqiHigh = 150 },
            new() { ConcentrationLow = 487, ConcentrationHigh = 797, AqiLow = 151, AqiHigh = 200 },
            new() { ConcentrationLow = 798, ConcentrationHigh = 1583, AqiLow = 201, AqiHigh = 300 }
        }
    };

    /// <summary>
    /// Calculate AQI for a single pollutant using EPA formula
    /// </summary>
    private static int CalculatePollutantAqi(double concentration, List<AqiBreakpoint> breakpoints)
    {
        if (concentration < 0) return 0;

        // Find the appropriate breakpoint range
        var breakpoint = breakpoints.FirstOrDefault(x => concentration >= x.ConcentrationLow && concentration <= x.ConcentrationHigh);

        // If concentration exceeds all breakpoints, use the highest range
        if (breakpoint == null && concentration > breakpoints.Last().ConcentrationHigh)
        {
            breakpoint = breakpoints.Last();
        }

        // If no breakpoint found, return 0
        if (breakpoint == null) return 0;

        // EPA AQI Formula:
        // AQI = [(AQI_high - AQI_low) / (C_high - C_low)] × (C - C_low) + AQI_low
        double aqi = ((breakpoint.AqiHigh - breakpoint.AqiLow) /
                     (breakpoint.ConcentrationHigh - breakpoint.ConcentrationLow)) *
                     (concentration - breakpoint.ConcentrationLow) + breakpoint.AqiLow;

        return (int)Math.Round(aqi);
    }

    /// <summary>
    /// Compute overall AQI from all pollutants
    /// </summary>
    public static int ComputeAqi(double pm2_5, double pm10, double o3, double co, double no2, double so2)
    {
        var aqiValues = new Dictionary<string, int>
        {
            ["PM2.5"] = CalculatePollutantAqi(pm2_5, Breakpoints["PM2.5"]),
            ["PM10"] = CalculatePollutantAqi(pm10, Breakpoints["PM10"]),
            ["O3"] = CalculatePollutantAqi(o3, Breakpoints["O3"]),
            ["CO"] = CalculatePollutantAqi(co, Breakpoints["CO"]),
            ["NO2"] = CalculatePollutantAqi(no2, Breakpoints["NO2"]),
            ["SO2"] = CalculatePollutantAqi(so2, Breakpoints["SO2"])
        };

        // The overall AQI is the HIGHEST individual pollutant AQI
        return aqiValues.Values.Max();
    }

    /// <summary>
    /// Compute AQI and identify the dominant pollutant
    /// </summary>
    public static (int Aqi, string DominantPollutant) ComputeAqiWithDominant(AirQualityReading reading)
    {
        var aqiValues = new Dictionary<string, int>
        {
            ["PM2.5"] = CalculatePollutantAqi(reading.ParticulateMatter2_5.Concentration, Breakpoints["PM2.5"]),
            ["PM10"] = CalculatePollutantAqi(reading.ParticulateMatter10.Concentration, Breakpoints["PM10"]),
            ["O3"] = CalculatePollutantAqi(reading.Ozone.Concentration, Breakpoints["O3"]),
            ["CO"] = CalculatePollutantAqi(reading.CarbonMonoxide.Concentration, Breakpoints["CO"]),
            ["NO2"] = CalculatePollutantAqi(reading.NitrogenDioxide.Concentration, Breakpoints["NO2"]),
            ["SO2"] = CalculatePollutantAqi(reading.SulfurDioxide.Concentration, Breakpoints["SO2"])
        };

        var maxEntry = aqiValues.OrderByDescending(x => x.Value).First();
        return (maxEntry.Value, maxEntry.Key);
    }
}