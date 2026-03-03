import { Pollutant } from "../models/dashboard-view";

interface Props {
  pollutant: Pollutant;
  change: number;
}

export function PollutantCard({ pollutant, change }: Props) {
  const { name, index, concentration, max } = pollutant;

  const getPollutantStatus = () => {
    // const thresholds: Record<
    //   string,
    //   { good: number; moderate: number; unhealthy: number }
    // > = {
    //   "Nitrogen Dioxide": { good: 40, moderate: 100, unhealthy: 200 },
    //   "Nitrogen Oxide": { good: 40, moderate: 100, unhealthy: 200 },
    //   "Particulate Matter 10": { good: 25, moderate: 50, unhealthy: 100 },
    //   "Particulate Matter 2.5": { good: 15, moderate: 35, unhealthy: 75 },
    //   "Sulfur Dioxide": { good: 20, moderate: 80, unhealthy: 350 },
    //   "Carbon Monoxide": { good: 4000, moderate: 10000, unhealthy: 30000 },
    //   Ozone: { good: 60, moderate: 120, unhealthy: 180 },
    //   Ammonia: { good: 100, moderate: 200, unhealthy: 400 },
    // };

    // const threshold = thresholds[name];
    const percentage = (concentration / max) * 100;

    if (index <= 1) {
      return {
        status: "Good",
        colour: "emerald",
        percentage,
        bgColor: "bg-emerald-500/10",
        borderColour: "border-emerald-500/20",
        textColour: "text-emerald-400",
      };
    }
    if (index <= 2) {
      return {
        status: "Fair",
        colour: "amber",
        percentage,
        bgColour: "bg-amber-500/10",
        borderColour: "border-amber-500/20",
        textColour: "text-amber-400",
      };
    }
    if (index <= 3) {
      return {
        status: "Moderate",
        colour: "orange",
        percentage,
        bgColour: "bg-orange-500/10",
        borderColour: "border-orange-500/20",
        textColour: "text-orange-400",
      };
    }
    if (index <= 4) {
      return {
        status: "Unhealthy",
        colour: "red",
        percentage,
        bgColour: "bg-red-500/10",
        borderColour: "border-red-500/20",
        textColour: "text-red-400",
      };
    }
    return {
      status: "Very Unhealthy",
      colour: "purple",
      percentage,
      bgColour: "bg-purple-500/10",
      borderColour: "border-purple-500/20",
      textColour: "text-purple-400",
    };
  };

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

  const { status, colour, percentage, bgColour, borderColour, textColour } =
    getPollutantStatus();
  const formula = getChemicalFormula(name);

  return (
    <div
      className={`${bgColour} border ${borderColour} rounded-2xl p-6 transition-all duration-300 hover:scale-[1.02] hover:shadow-lg hover:shadow-${colour}-500/10`}
      style={{ animationDelay: `${index * 50}ms` }}
    >
      <div className="flex justify-between items-start mb-4">
        <div>
          <h3 className="font-semibold text-white text-lg mb-1">{name}</h3>
          <p className="text-slate-400 text-sm font-mono">{formula}</p>
        </div>
        <div className="text-right">
          <div className={`${textColour} font-bold text-3xl`}>
            {concentration.toFixed(2)}
          </div>
          <div className="text-slate-500 text-xs font-mono mt-1">µg/m³</div>
        </div>
      </div>

      <div className="relative h-2 bg-slate-800/50 rounded-full overflow-hidden mb-3">
        <div
          className={`h-full bg-gradient-to-r from-${colour}-500 to-${colour}-400 rounded-full transition-all duration-1000 ease-out relative overflow-hidden`}
          style={{ width: `${Math.min(percentage, 100)}%` }}
        >
          <div className="absolute inset-0 bg-gradient-to-r from-transparent via-white/30 to-transparent animate-shimmer"></div>
        </div>
      </div>

      <div className="flex justify-between items-center">
        <span
          className={`${textColour} text-xs font-semibold uppercase tracking-wider`}
        >
          {status}
        </span>
        {change != 0 && (
          <div className="flex items-center gap-1 text-slate-400 text-xs font-mono">
            <span
              className={
                concentration > 0
                  ? "text-red-400"
                  : concentration < 0
                    ? "text-emerald-400"
                    : "text-slate-400"
              }
            >
              {change > 0 ? "↑" : change < 0 ? "↓" : "-"}
            </span>
            <span>{change}%</span>
          </div>
        )}
      </div>
    </div>
  );
}
