import { DivIcon, LatLng, marker } from "leaflet";
import { Marker, Popup } from "react-leaflet";
import { MapEntry } from "../models/map-entries-view";

interface Props {
  position: LatLng;
  entry: MapEntry;
}

export default function MapMarker({
  position,
  entry: { airQualityReading },
}: Props) {
  const markerColour = (index: number | undefined) => {
    switch (index) {
      case 1:
        return "bg-green-500";

      case 2:
        return "bg-green-800";

      case 3:
        return "bg-yellow-500";

      case 4:
        return "bg-orange-500";

      case 5:
        return "bg-red-500";
    }
  };

  const markerHtml = `
    <div class="relative flex items-center justify-center">
      <div class="w-10 h-10 ${markerColour(
        airQualityReading.index
      )} text-white text-sm font-bold flex items-center justify-center rounded-full shadow-md">
        ${airQualityReading.index}
      </div>
      <div class="absolute bottom-0 left-1/2 transform -translate-x-1/2 translate-y-1 rotate-45 w-3 h-3 ${markerColour(
        airQualityReading.index
      )}"></div>
    </div>
  `;

  return (
    <Marker
      position={position}
      icon={
        new DivIcon({
          iconSize: [30, 30],
          iconAnchor: [12, 41],
          className: `${markerColour(
            airQualityReading.index
          )} rounded text-white flex justify-center items-center h-full`,
          html: markerHtml,
        })
      }
    >
      <Popup className="w-[200px]" maxWidth={200}>
        <div className="grid grid-cols-2">
          <p style={{ margin: "0.5rem 0" }}>
            Sulfur Dioxide: {airQualityReading.sulfurDioxide.index}
          </p>
          <p style={{ margin: "0.5rem 0" }}>
            Nitrogen Oxide: {airQualityReading.nitrogenOxide.index}
          </p>
          <p style={{ margin: "0.5rem 0" }}>
            Nitrogen Dioxide: {airQualityReading.nitrogenDioxide.index}
          </p>
          <p style={{ margin: "0.5rem 0" }}>
            Particulate Matter 10: {airQualityReading.particulateMatter10.index}
          </p>
          <p style={{ margin: "0.5rem 0" }}>
            Particulate Matter 2.5:{" "}
            {airQualityReading.particulateMatter2_5.index}
          </p>
          <p style={{ margin: "0.5rem 0" }}>
            Ozone: {airQualityReading.ozone.index}
          </p>
          <p style={{ margin: "0.5rem 0" }}>
            Carbon Monoxide: {airQualityReading.carbonMonoxide.index}
          </p>
          <p style={{ margin: "0.5rem 0" }}>
            Ammonia: {airQualityReading.ammonia.index}
          </p>
        </div>
      </Popup>
    </Marker>
  );
}
