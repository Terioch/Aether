import { Icon, LatLng, LatLngBounds, Map as LeafletMap } from "leaflet";
import {
  MapContainer,
  Marker,
  Popup,
  TileLayer,
  useMapEvents,
} from "react-leaflet";
import "leaflet/dist/leaflet.css";
import { MapEntry } from "../models/map-entry";
import { Fragment, useEffect, useState } from "react";
import { getNearbyMapEntriesRequest } from "../requests/get-nearby-map-entries-request";

async function getNearbyMapEntries(
  request: getNearbyMapEntriesRequest
): Promise<MapEntry[]> {
  return new Promise(async (resolve, reject) => {
    try {
      const res = await fetch(
        `https://localhost:7158/api/dashboard/map-entries`,
        {
          method: "POST",
          headers: {
            "content-type": "application/json",
          },
          body: JSON.stringify(request),
        }
      );

      if (!res.ok) {
        throw new Error("Failed to fetch nearby map entries");
      }

      const data = await res.json();
      resolve(data);
    } catch (error) {
      reject(error);
    }
  });
}

interface Props {
  geoLocation: GeolocationPosition;
  airQualityIndex: number;
  nearbyMapEntries: MapEntry[] | undefined;
}

export default function Map({
  geoLocation,
  airQualityIndex,
  nearbyMapEntries,
}: Props) {
  const [map, setMap] = useState<LeafletMap>();

  const center = new LatLng(
    geoLocation.coords.latitude,
    geoLocation.coords.longitude
  );

  useEffect(() => {
    if (!map) return;

    const zoom = map.getZoom();
    const bounds = map.getBounds();

    const northEast = bounds.getNorthEast();
    const southWest = bounds.getSouthWest();

    const request: getNearbyMapEntriesRequest = {
      location: {
        latitude: geoLocation.coords.latitude,
        longitude: geoLocation.coords.longitude,
      },
      zoom,
      bounds: {
        northEast: {
          latitude: northEast.lat,
          longitude: northEast.lng,
        },
        southWest: {
          latitude: southWest.lat,
          longitude: southWest.lng,
        },
      },
    };

    getNearbyMapEntries(request);
  }, [map]);

  const MapEventHandler = () => {
    const map = useMapEvents({
      zoomend: () => {
        console.log("Zoom level changed to:", map.getZoom(), map.getBounds());
        onMapChange(map.getZoom(), map.getBounds());
      },
      moveend: () => {
        console.log("Moved", map.getZoom(), map.getBounds());
        onMapChange(map.getZoom(), map.getBounds());
      },
    });

    setMap(map);

    return null;
  };

  const onMapChange = (zoom: number, bounds: LatLngBounds) => {
    const northEast = bounds.getNorthEast();
    const southWest = bounds.getSouthWest();

    const request: getNearbyMapEntriesRequest = {
      location: {
        latitude: geoLocation!.coords.latitude,
        longitude: geoLocation!.coords.longitude,
      },
      zoom,
      bounds: {
        northEast: {
          latitude: northEast.lat,
          longitude: northEast.lng,
        },
        southWest: {
          latitude: southWest.lat,
          longitude: southWest.lng,
        },
      },
    };

    getNearbyMapEntries(request);
  };

  return (
    <MapContainer
      center={center}
      zoom={13}
      scrollWheelZoom={false}
      style={{ height: 600, width: "100%" }}
    >
      <TileLayer
        attribution='&copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors'
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
      />

      <Marker
        position={center}
        icon={
          new Icon({
            iconUrl: "/marker-icon.png",
            iconSize: [25, 41],
            iconAnchor: [12, 41],
          })
        }
      >
        <p> {airQualityIndex}</p>
        <Popup>Index: {airQualityIndex}</Popup>
      </Marker>

      {nearbyMapEntries?.map((mapEntry, idx) => (
        <Fragment key={idx}>
          <Marker
            position={
              new LatLng(
                mapEntry.location.latitude,
                mapEntry.location.longitude
              )
            }
            icon={
              new Icon({
                iconUrl: "/marker-icon.png",
                iconSize: [25, 41],
                iconAnchor: [12, 41],
              })
            }
          >
            <p> {mapEntry.airQualityIndex.index}</p>
            <Popup>Index: {mapEntry.airQualityIndex.index}</Popup>
          </Marker>
        </Fragment>
      ))}

      <MapEventHandler />
    </MapContainer>
  );
}
