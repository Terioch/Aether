"use client";

import dynamic from "next/dynamic";
import { useEffect, useState } from "react";
import { DashboardView, Pollutant } from "../models/dashboard-view";
import ProgressBar from "./ProgressBar";
import { MapEntry } from "../models/map-entry";
import { getNearbyMapEntriesRequest } from "../requests/get-nearby-map-entries-request";
import { LatLngBounds } from "leaflet";

async function getView(position: GeolocationPosition): Promise<DashboardView> {
  return new Promise(async (resolve, reject) => {
    try {
      const res = await fetch(
        `https://localhost:7158/api/dashboard?latitude=${position.coords.latitude}&longitude=${position.coords.longitude}`
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
  const [nearbyMapEntries, setNearbyMapEntries] = useState<MapEntry[]>();
  const [geoLocation, setGeoLocation] = useState<GeolocationPosition>();

  const Map = dynamic(() => import("./Map"), { ssr: false });

  useEffect(() => {
    navigator.geolocation.getCurrentPosition((position) => {
      setGeoLocation(position);
      getView(position).then((res) => {
        console.log(res);
        setView(res);
      });
    });
  }, []);

  const showProgressBar = (title: string, pollutant: Pollutant) => {
    return (
      <div className="w-full">
        <h4 className="font-medium">{title}</h4>
        <ProgressBar index={pollutant.index} value={pollutant.concentration} />
      </div>
    );
  };

  return (
    <div>
      {view ? (
        <div className="flex flex-col gap-12">
          <div className="flex flex-col gap-2">
            <h1 className="font-bold text-4xl">Air Quality For</h1>
            <h1 className="font-medium text-4xl">
              {view.location.state} {view.location.country}
            </h1>
            <h1 className="font-light text-4xl">{view.location.city}</h1>
          </div>

          {geoLocation && (
            <Map
              geoLocation={geoLocation}
              airQualityIndex={view.airQualityIndex.index}
              nearbyMapEntries={nearbyMapEntries}
            />
          )}

          <div>
            <h1 className="font-semibold text-2xl">Breakdown</h1>

            <div className="flex flex-col gap-4">
              <div className="flex gap-4 justify-between">
                {showProgressBar(
                  "Nitrogen Dioxide",
                  view.airQualityIndex.nitrogenDioxide
                )}

                {showProgressBar(
                  "Nitrogen Oxide",
                  view.airQualityIndex.nitrogenOxide
                )}
              </div>

              <div className="flex gap-4 justify-between">
                {showProgressBar(
                  "Particulate Matter 10",
                  view.airQualityIndex.particulateMatter10
                )}

                {showProgressBar(
                  "Particulate Matter 2.5",
                  view.airQualityIndex.particulateMatter2_5
                )}
              </div>

              <div className="flex gap-4 justify-between">
                {showProgressBar(
                  "Sulfur Dioxide",
                  view.airQualityIndex.sulfurDioxide
                )}

                {showProgressBar(
                  "Carbon Monoxide",
                  view.airQualityIndex.carbonMonoxide
                )}
              </div>

              <div className="flex gap-4 justify-between">
                {showProgressBar("Ozone", view.airQualityIndex.ozone)}

                {showProgressBar("Ammonia", view.airQualityIndex.ammonia)}
              </div>
            </div>
          </div>
        </div>
      ) : (
        <h4>Dashboard loading...</h4>
      )}
    </div>
  );
}
