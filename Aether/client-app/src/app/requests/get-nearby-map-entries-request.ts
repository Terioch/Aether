import { GeoLocation } from "../models/air-quality";

export interface MapEntriesRequest {
  centre: GeoLocation;
  zoom: number;
  bounds: MapBounds;
}

export interface MapBounds {
  northEast: GeoLocation;
  southWest: GeoLocation;
}
