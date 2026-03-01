import { DivIcon, LatLng } from "leaflet";
import { Marker, Popup } from "react-leaflet";
import { MapEntry } from "../models/map-entries-view";
import { GeoLocation } from "../models/geo-location";

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
  const getMarkerColour = (aqi: number) => {
    if (aqi <= 50) return "bg-emerald-500";
    if (aqi <= 100) return "bg-amber-500";
    if (aqi <= 150) return "bg-orange-500";
    if (aqi <= 200) return "bg-red-500";
    if (aqi <= 300) return "bg-purple-500";
    return "bg-maroon-500";
  };

  function badgeColour(index: number | undefined) {
    switch (index) {
      case 1:
        return "bg-emerald-500";
      case 2:
        return "bg-amber-500";
      case 3:
        return "bg-orange-500";
      case 4:
        return "bg-red-500";
      case 5:
        return "bg-purple-500";
      default:
        return "bg-gray-400";
    }
  }

  const markerColour = getMarkerColour(airQualityReading.aqi);

  const markerHtml = `
    <div class="relative flex items-center justify-center">
      <div class="w-10 h-10 ${markerColour} text-white text-sm font-bold flex items-center justify-center rounded-full shadow-md">
        ${airQualityReading.aqi}
      </div>
      <div class="absolute bottom-0 left-1/2 transform -translate-x-1/2 translate-y-1 rotate-45 w-3 h-3 ${markerColour}"></div>
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
          className: `${markerColour} rounded text-white flex justify-center items-center h-full`,
          html: markerHtml,
        })
      }
      eventHandlers={{
        click: (e) => {
          console.log(
            "locations",
            e.latlng,
            position,
            airQualityReading.location,
          );
          onMarkerClick();
        },
      }}
    >
      <Popup className="w-max p-2" closeButton={false}>
        <div className="grid grid-cols-2 gap-x-4 gap-y-2 text-sm text-gray-800">
          {pollutants.map(({ label, index }, i) => (
            <div
              key={i}
              className="flex items-center justify-between min-w-0 gap-2"
            >
              <span
                className="text-gray-600 truncate whitespace-nowrap"
                title={label}
              >
                {label}
              </span>
              <span
                className={`px-2 py-0.5 text-xs font-semibold rounded-full text-white ${badgeColour(
                  index as number,
                )}`}
              >
                {index}
              </span>
            </div>
          ))}
        </div>
      </Popup>
    </Marker>
  );
}
