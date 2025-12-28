import { LatLng, LatLngBounds, Map as LeafletMap } from "leaflet";
import "leaflet/dist/leaflet.css";
import { Fragment, useEffect, useState } from "react";
import {
  MapContainer,
  Marker,
  Popup,
  TileLayer,
  useMapEvents,
} from "react-leaflet";
import { MapEntriesView, MapEntry } from "../models/map-entries-view";
import { MapEntriesRequest } from "../requests/get-nearby-map-entries-request";
import MapMarker from "./MapMarker";

async function getMapEntries(
  request: MapEntriesRequest
): Promise<MapEntriesView> {
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
      console.log({ nearbyEntries: data });
      resolve(data);
    } catch (error) {
      reject(error);
    }
  });
}

interface Props {
  geoLocation: GeolocationPosition;
}

export default function Map({ geoLocation }: Props) {
  const [map, setMap] = useState<LeafletMap>();
  const [mapCentreEntry, setMapCentreEntry] = useState<MapEntry>();
  const [nearbyMapEntries, setNearbyMapEntries] = useState<MapEntry[]>();
  const [mapCentre, setMapCentre] = useState<LatLng>(
    new LatLng(geoLocation.coords.latitude, geoLocation.coords.longitude)
  );

  useEffect(() => {
    if (!map) return;

    const zoom = map.getZoom();
    const bounds = map.getBounds();

    const northEast = bounds.getNorthEast();
    const southWest = bounds.getSouthWest();
    console.log("Map changed", { bounds });

    const request: MapEntriesRequest = {
      centre: {
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

    getMapEntries(request).then((res) => {
      setMapCentreEntry(res.centre);
      setNearbyMapEntries(res.nearbyEntries);
    });
  }, [map]);

  const MapEventHandler = () => {
    const map = useMapEvents({
      zoomend: () => {
        onMapChange(map.getZoom(), map.getBounds());
      },
      moveend: () => {
        onMapChange(map.getZoom(), map.getBounds());
      },
    });

    setMap(map);

    return null;
  };

  const onMapChange = (zoom: number, bounds: LatLngBounds) => {
    const northEast = bounds.getNorthEast();
    const southWest = bounds.getSouthWest();
    const center = bounds.getCenter();
    console.log(center, {
      lat: (northEast.lat - southWest.lat) / 2 + southWest.lat,
      lng: (northEast.lng - southWest.lng) / 2 + southWest.lng,
    });

    const request: MapEntriesRequest = {
      centre: {
        latitude: center.lat,
        longitude: center.lng,
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

    setMapCentre(center);

    getMapEntries(request).then((res) => {
      setMapCentreEntry(res.centre);
      setNearbyMapEntries(res.nearbyEntries);
    });
  };

  return (
    <MapContainer
      center={mapCentre}
      zoom={13}
      scrollWheelZoom={false}
      style={{ height: 600, width: "100%" }}
    >
      <TileLayer
        attribution='&copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors'
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
      />

      {mapCentreEntry && (
        <MapMarker position={mapCentre} entry={mapCentreEntry} />
      )}

      {nearbyMapEntries?.map((mapEntry, idx) => (
        <Fragment key={idx}>
          <MapMarker
            position={
              new LatLng(
                mapEntry.airQualityReading.location.latitude,
                mapEntry.airQualityReading.location.longitude
              )
            }
            entry={mapEntry}
          />
        </Fragment>
      ))}

      <MapEventHandler />
    </MapContainer>
  );
}
