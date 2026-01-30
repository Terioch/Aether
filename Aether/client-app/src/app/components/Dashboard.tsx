"use client";

import dynamic from "next/dynamic";
import { useEffect, useState } from "react";
import { DashboardView } from "../models/dashboard-view";
import { PollutantCard } from "./PollutantCard";

async function getView(position: GeolocationPosition): Promise<DashboardView> {
  return new Promise(async (resolve, reject) => {
    try {
      const res = await fetch(
        `${process.env.NEXT_PUBLIC_API_URL}/api/dashboard?latitude=${position.coords.latitude}&longitude=${position.coords.longitude}`,
      );

      if (!res.ok) {
        throw new Error("Failed to fetch pollution data");
      }

      const data = await res.json();
      resolve(data);
    } catch (error) {
      reject(error);
    }
  });
}

export default function Dashboard() {
  const [view, setView] = useState<DashboardView>();

  const [geoLocation, setGeoLocation] = useState<GeolocationPosition>();

  const Map = dynamic(() => import("./Map"), { ssr: false });

  useEffect(() => {
    navigator.geolocation.getCurrentPosition((position) => {
      setGeoLocation(position);
      getView(position).then((res) => {
        setView(res);
      });
    });
  }, []);

  // Helper function to get AQI status
  const getAqiStatus = () => {
    if (aqi <= 50)
      return {
        label: "Good",
        color: "emerald",
        description: "Air quality is excellent",
      };
    if (aqi <= 100)
      return {
        label: "Moderate",
        color: "amber",
        description: "Generally acceptable air quality",
      };
    if (aqi <= 150)
      return {
        label: "Unhealthy for Sensitive Groups",
        color: "orange",
        description: "Sensitive groups may experience health effects",
      };
    if (aqi <= 200)
      return {
        label: "Unhealthy",
        color: "red",
        description: "Everyone may begin to experience health effects",
      };
    if (aqi <= 300)
      return {
        label: "Very Unhealthy",
        color: "red",
        description: "Health alert: everyone may experience serious effects",
      };
    return {
      label: "Hazardous",
      color: "red",
      description: "Emergency conditions: entire population affected",
    };
  };

  if (!view) {
    return (
      <div className="flex items-center justify-center min-h-screen bg-slate-950">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-emerald-500 mx-auto mb-4"></div>
        </div>
      </div>
    );
  }

  const aqi = view.airQualityReading.index;
  const aqiStatus = getAqiStatus();

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-950 via-slate-900 to-slate-950 text-white p-6 md:p-8 rounded">
      <div className="max-w-7xl mx-auto space-y-8">
        {/* Header Section */}
        <div className="animate-fadeIn">
          <div className="flex justify-between items-start mb-2">
            <h1 className="font-bold text-5xl md:text-6xl bg-gradient-to-r from-white to-slate-400 bg-clip-text text-transparent">
              Air Quality
            </h1>
            <div className="flex items-center gap-2 text-slate-400 font-mono text-sm">
              <span>📍</span>
              <span>{view.location.city}</span>
            </div>
          </div>
          <h2 className="font-medium text-3xl md:text-4xl text-slate-300">
            {view.location.state}, {view.location.country}
          </h2>
          <p className="text-slate-500 text-sm font-mono mt-2">
            Last updated:{" "}
            {new Date().toLocaleString("en-GB", {
              hour: "2-digit",
              minute: "2-digit",
              day: "numeric",
              month: "short",
              year: "numeric",
            })}
          </p>
        </div>

        {/* AQI Card */}
        <div
          className={`bg-gradient-to-br from-slate-900 to-slate-800 border border-slate-700/50 rounded-3xl p-8 relative overflow-hidden animate-scaleIn shadow-2xl`}
        >
          <div className="absolute top-0 right-0 w-96 h-96 bg-${aqiStatus.color}-500/10 rounded-full blur-3xl animate-pulse"></div>

          <div className="relative z-10">
            <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-6 mb-6">
              <div className="flex items-baseline gap-4">
                <div
                  className={`text-7xl md:text-8xl font-bold text-${aqiStatus.color}-400`}
                >
                  {aqi}
                </div>
                <div className="text-xl text-slate-400 font-semibold">AQI</div>
              </div>

              <div className="text-left md:text-right">
                <div
                  className={`inline-block px-5 py-2 rounded-full bg-${aqiStatus.color}-500 text-white font-semibold text-sm uppercase tracking-wide shadow-lg`}
                >
                  {aqiStatus.label}
                </div>
                <p className="text-slate-400 text-sm mt-2">
                  {aqiStatus.description}
                </p>
              </div>
            </div>

            <p className="text-slate-300 leading-relaxed max-w-3xl">
              {aqi <= 50
                ? "Air quality is excellent. It's a great day for outdoor activities."
                : aqi <= 100
                  ? "Air quality is acceptable for most people. However, sensitive groups may experience minor symptoms from long-term exposure."
                  : "Air quality is declining. Sensitive groups should consider reducing prolonged outdoor exertion."}
            </p>
          </div>
        </div>

        {/* Map Section */}
        {geoLocation && (
          <div className="rounded-3xl overflow-hidden border border-slate-800 shadow-2xl animate-fadeIn">
            <Map geoLocation={geoLocation} />
          </div>
        )}

        {/* Pollutants Grid */}
        <div className="space-y-6 animate-fadeIn">
          <h2 className="font-bold text-3xl text-white">
            Pollutant Concentrations
          </h2>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-5">
            <PollutantCard pollutant={view.airQualityReading.nitrogenDioxide} />
            <PollutantCard pollutant={view.airQualityReading.nitrogenOxide} />
            <PollutantCard
              pollutant={view.airQualityReading.particulateMatter10}
            />
            <PollutantCard
              pollutant={view.airQualityReading.particulateMatter2_5}
            />
            <PollutantCard pollutant={view.airQualityReading.sulfurDioxide} />
            <PollutantCard pollutant={view.airQualityReading.carbonMonoxide} />
            <PollutantCard pollutant={view.airQualityReading.ozone} />
            <PollutantCard pollutant={view.airQualityReading.ammonia} />
          </div>
        </div>

        {/* Health Recommendations */}
        <div className="bg-slate-900/50 border border-slate-800 rounded-3xl p-8 animate-fadeIn">
          <h3 className="font-bold text-2xl text-white mb-6 flex items-center gap-3">
            <span>💡</span>
            Health Recommendations
          </h3>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="flex gap-4 p-5 bg-slate-800/30 border border-slate-700/30 rounded-2xl">
              <div className="text-3xl">🏃</div>
              <div>
                <h4 className="font-semibold text-white mb-1">
                  General Public
                </h4>
                <p className="text-slate-400 text-sm leading-relaxed">
                  {aqi <= 50
                    ? "Enjoy your normal outdoor activities."
                    : "Outdoor activities are generally acceptable."}
                </p>
              </div>
            </div>

            <div className="flex gap-4 p-5 bg-slate-800/30 border border-slate-700/30 rounded-2xl">
              <div className="text-3xl">❤️</div>
              <div>
                <h4 className="font-semibold text-white mb-1">
                  Sensitive Groups
                </h4>
                <p className="text-slate-400 text-sm leading-relaxed">
                  {aqi <= 50
                    ? "No precautions necessary."
                    : "Consider reducing prolonged outdoor exertion."}
                </p>
              </div>
            </div>

            <div className="flex gap-4 p-5 bg-slate-800/30 border border-slate-700/30 rounded-2xl">
              <div className="text-3xl">🪟</div>
              <div>
                <h4 className="font-semibold text-white mb-1">Indoor Air</h4>
                <p className="text-slate-400 text-sm leading-relaxed">
                  {aqi <= 50
                    ? "Good time to ventilate your home."
                    : "Consider keeping windows closed."}
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <style jsx>{`
        @keyframes fadeIn {
          from {
            opacity: 0;
            transform: translateY(20px);
          }
          to {
            opacity: 1;
            transform: translateY(0);
          }
        }

        @keyframes scaleIn {
          from {
            opacity: 0;
            transform: scale(0.95);
          }
          to {
            opacity: 1;
            transform: scale(1);
          }
        }

        @keyframes shimmer {
          0% {
            transform: translateX(-100%);
          }
          100% {
            transform: translateX(100%);
          }
        }

        .animate-fadeIn {
          animation: fadeIn 0.6s ease-out forwards;
        }

        .animate-scaleIn {
          animation: scaleIn 0.6s ease-out forwards;
        }

        .animate-shimmer {
          animation: shimmer 2s infinite;
        }
      `}</style>
    </div>
  );
}
