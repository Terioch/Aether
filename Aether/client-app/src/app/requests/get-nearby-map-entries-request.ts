import { GeoLocation } from "../models/air-quality";

export interface getNearbyMapEntriesRequest {
  location: GeoLocation;
  zoom: number;
  bounds: MapBounds;
}

export interface MapBounds {
  northEast: GeoLocation;
  southWest: GeoLocation;
}
