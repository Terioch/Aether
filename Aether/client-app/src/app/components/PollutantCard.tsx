import { Pollutant } from "../models/dashboard-view";

interface Props {
  pollutant: Pollutant;
}

export function PollutantCard({ pollutant }: Props) {
  // Helper function to get pollutant status
  const getPollutantStatus = (name: string, value: number) => {
    const thresholds: Record<
      string,
      { good: number; moderate: number; unhealthy: number }
    > = {
      "Nitrogen Dioxide": { good: 40, moderate: 100, unhealthy: 200 },
      "Nitrogen Oxide": { good: 40, moderate: 100, unhealthy: 200 },
      "Particulate Matter 10": { good: 25, moderate: 50, unhealthy: 100 },
      "Particulate Matter 2.5": { good: 15, moderate: 35, unhealthy: 75 },
      "Sulfur Dioxide": { good: 20, moderate: 80, unhealthy: 350 },
      "Carbon Monoxide": { good: 4000, moderate: 10000, unhealthy: 30000 },
      Ozone: { good: 60, moderate: 120, unhealthy: 180 },
      Ammonia: { good: 100, moderate: 200, unhealthy: 400 },
    };

    const threshold = thresholds[name] || {
      good: 50,
      moderate: 100,
      unhealthy: 200,
    };

    if (value <= threshold.good) {
      return {
        status: "Good",
        color: "emerald",
        percentage: (value / threshold.good) * 20,
        bgColor: "bg-emerald-500/10",
        borderColor: "border-emerald-500/20",
        textColor: "text-emerald-400",
      };
    }
    if (value <= threshold.moderate) {
      return {
        status: "Moderate",
        color: "amber",
        percentage:
          20 +
          ((value - threshold.good) / (threshold.moderate - threshold.good)) *
            40,
        bgColor: "bg-amber-500/10",
        borderColor: "border-amber-500/20",
        textColor: "text-amber-400",
      };
    }
    if (value <= threshold.unhealthy) {
      return {
        status: "Unhealthy",
        color: "orange",
        percentage:
          60 +
          ((value - threshold.moderate) /
            (threshold.unhealthy - threshold.moderate)) *
            30,
        bgColor: "bg-orange-500/10",
        borderColor: "border-orange-500/20",
        textColor: "text-orange-400",
      };
    }
    return {
      status: "Very Unhealthy",
      color: "red",
      percentage: Math.min(90 + (value / threshold.unhealthy) * 10, 100),
      bgColor: "bg-red-500/10",
      borderColor: "border-red-500/20",
      textColor: "text-red-400",
    };
  };

  // Helper function to get chemical formula
  const getChemicalFormula = (name: string): string => {
    const formulas: Record<string, string> = {
      "Nitrogen Dioxide": "NO₂",
      "Nitrogen Oxide": "NO",
      "Particulate Matter 10": "PM₁₀",
      "Particulate Matter 2.5": "PM₂.₅",
      "Sulfur Dioxide": "SO₂",
      "Carbon Monoxide": "CO",
      Ozone: "O₃",
      Ammonia: "NH₃",
    };
    return formulas[name] || "";
  };

  const { name, index, concentration, max } = pollutant;
  const { status, color, percentage, bgColor, borderColor, textColor } =
    getPollutantStatus(name, concentration);
  const formula = getChemicalFormula(name);

  return (
    <div
      className={`${bgColor} border ${borderColor} rounded-2xl p-6 transition-all duration-300 hover:scale-[1.02] hover:shadow-lg hover:shadow-${color}-500/10`}
      style={{ animationDelay: `${index * 50}ms` }}
    >
      <div className="flex justify-between items-start mb-4">
        <div>
          <h3 className="font-semibold text-white text-lg mb-1">{name}</h3>
          <p className="text-slate-400 text-sm font-mono">{formula}</p>
        </div>
        <div className="text-right">
          <div className={`${textColor} font-bold text-3xl`}>
            {concentration.toFixed(2)}
          </div>
          <div className="text-slate-500 text-xs font-mono mt-1">µg/m³</div>
        </div>
      </div>

      <div className="relative h-2 bg-slate-800/50 rounded-full overflow-hidden mb-3">
        <div
          className={`h-full bg-gradient-to-r from-${color}-500 to-${color}-400 rounded-full transition-all duration-1000 ease-out relative overflow-hidden`}
          style={{ width: `${Math.min(percentage, 100)}%` }}
        >
          <div className="absolute inset-0 bg-gradient-to-r from-transparent via-white/30 to-transparent animate-shimmer"></div>
        </div>
      </div>

      <div className="flex justify-between items-center">
        <span
          className={`${textColor} text-xs font-semibold uppercase tracking-wider`}
        >
          {status}
        </span>
        <div className="flex items-center gap-1 text-slate-400 text-xs font-mono">
          <span
            className={concentration > 50 ? "text-red-400" : "text-emerald-400"}
          >
            {concentration > 50 ? "↑" : "↓"}
          </span>
          <span>{Math.floor(Math.random() * 10)}%</span>
        </div>
      </div>
    </div>
  );
}
