import { Icon, LatLng, LatLngBounds, Map as LeafletMap } from "leaflet";
import {
  MapContainer,
  Marker,
  Popup,
  TileLayer,
  useMapEvents,
} from "react-leaflet";
import "leaflet/dist/leaflet.css";
import { MapEntriesView, MapEntry } from "../models/map-entries-view";
import { Fragment, useEffect, useState } from "react";
import { MapEntriesRequest as MapEntriesRequest } from "../requests/get-nearby-map-entries-request";

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
  airQualityIndex: number;
}

export default function Map({ geoLocation, airQualityIndex }: Props) {
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
    console.log({ bounds });

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
    const center = bounds.getCenter();
    console.log(center, {
      lat: (northEast.lat - southWest.lat) / 2 + southWest.lat,
      lng: (northEast.lng - southWest.lng) / 2 + southWest.lng,
    });

    const request: MapEntriesRequest = {
      centre: {
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

      <Marker
        position={mapCentre}
        icon={
          new Icon({
            iconUrl: "/marker-icon.png",
            iconSize: [25, 41],
            iconAnchor: [12, 41],
          })
        }
      >
        <p> {mapCentreEntry?.airQualityIndex.index}</p>
        <Popup>Index: {mapCentreEntry?.airQualityIndex.index}</Popup>
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
