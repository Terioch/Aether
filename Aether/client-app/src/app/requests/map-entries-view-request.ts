import { GeoLocation } from "../models/geo-location";

export interface MapEntriesViewRequest {
  centre: GeoLocation;
  zoom: number;
  bounds: MapBounds;
}

export interface MapBounds {
  northEast: GeoLocation;
  southWest: GeoLocation;
}
