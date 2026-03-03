import { DivIcon, LatLng } from "leaflet";
import { Marker, Popup } from "react-leaflet";
import { MapEntry } from "../models/map-entries-view";
import { GeoLocation } from "../models/geo-location";
import { getAqiStatus } from "../utils/aqi-utils";
import { useRef } from "react";
import { toDateLong } from "../utils/date";

interface Props {
  position: LatLng;
  entry: MapEntry;
  loadDashboardView: (
    location: GeoLocation,
    readingId?: number,
  ) => Promise<void>;
}

export default function MapMarker({
  position,
  entry: { airQualityReading },
  loadDashboardView,
}: Props) {
  const popupRef = useRef<L.Popup>(null);

  const aqiStatus = getAqiStatus(airQualityReading.aqi);
  const markerHtml = `
    <div class="relative flex items-center justify-center">
      <div class="w-10 h-10 ${aqiStatus.bgColour} text-white text-sm font-bold flex items-center justify-center rounded-full shadow-md">
        ${airQualityReading.aqi}
      </div>
      <div class="absolute bottom-0 left-1/2 transform -translate-x-1/2 translate-y-1 rotate-45 w-3 h-3 ${aqiStatus.bgColour}"></div>
    </div>
  `;

  const pollutants: { label: string; index: number }[] = [
    { label: "Sulfur Dioxide", index: airQualityReading.sulfurDioxide.index },
    { label: "Nitrogen Oxide", index: airQualityReading.nitrogenOxide.index },
    {
      label: "Nitrogen Dioxide",
      index: airQualityReading.nitrogenDioxide.index,
    },
    {
      label: "Particulate Matter 10",
      index: airQualityReading.particulateMatter10.index,
    },
    {
      label: "Particulate Matter 2.5",
      index: airQualityReading.particulateMatter2_5.index,
    },
    { label: "Ozone", index: airQualityReading.ozone.index },
    { label: "Carbon Monoxide", index: airQualityReading.carbonMonoxide.index },
    { label: "Ammonia", index: airQualityReading.ammonia.index },
  ];

  const onMarkerClick = async () => {
    await loadDashboardView(airQualityReading.location, airQualityReading.id);
  };

  return (
    <Marker
      position={position}
      icon={
        new DivIcon({
          iconSize: [30, 30],
          iconAnchor: [12, 41],
          className: `${aqiStatus.bgColour} rounded text-white flex justify-center items-center h-full`,
          html: markerHtml,
        })
      }
    >
      <Popup
        ref={popupRef}
        className="w-max max-h-[200px] p-3"
        aria-label="Location details"
        closeButton={false}
        autoPan={false}
      >
        <div className="flex flex-col gap-3">
          {/* Header */}
          <div className="flex items-start justify-between gap-3 pb-3 border-b border-gray-200">
            <div className="flex flex-col">
              <span className="text-md font-semibold text-gray-900">
                {airQualityReading.location.name}
              </span>
              <span className="text-xs text-gray-400 font-mono mt-0.5">
                Last updated: {toDateLong(airQualityReading.lastUpdated)}
              </span>
            </div>
            <button
              className="cursor-pointer text-gray-300 text-xl leading-none hover:text-gray-500 transition-colors"
              aria-label="Close"
              onClick={() => popupRef.current?.close()}
            >
              ×
            </button>
          </div>

          {/* AQI summary */}
          <div className="flex items-center gap-2 pb-3 border-b border-gray-200">
            <div
              className={`flex justify-center items-center gap-1.5 rounded-full ${aqiStatus.lightBgColour} border ${aqiStatus.borderColour} px-3 py-1 text-lg font-semibold ${aqiStatus.textColour} whitespace-nowrap`}
            >
              {airQualityReading.aqi}
            </div>
            <span className="text-xs text-gray-400">
              {aqiStatus.description}
            </span>
          </div>

          {/* Pollutant list */}
          <div className="flex flex-col divide-y divide-gray-100">
            {pollutants.map(({ label }) => (
              <div
                key={label}
                className="flex items-center justify-between py-1.5"
              >
                <span className="text-xs text-gray-700 font-medium">
                  {label}
                </span>
                <span
                  className={`text-xs font-semibold rounded-full px-2.5 py-0.5 ${aqiStatus.lightBgColour} ${aqiStatus.textColour}`}
                >
                  {aqiStatus.label}
                </span>
              </div>
            ))}
          </div>

          {/* CTA */}
          <button
            className="flex items-center justify-center gap-1.5 w-full py-2 px-3 bg-gray-900 hover:bg-gray-700 text-white text-xs font-semibold rounded-lg transition-colors mt-1"
            onClick={onMarkerClick}
          >
            <svg
              width="12"
              height="12"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2.5"
              strokeLinecap="round"
              strokeLinejoin="round"
            >
              <path d="M5 12h14M12 5l7 7-7 7" />
            </svg>
            View full details
          </button>
        </div>
      </Popup>
    </Marker>
  );
}
