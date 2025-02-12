import { GeoLocation } from "./air-quality";
import { AirQualityIndex } from "./dashboard-view";

export interface MapEntriesView {
  centre: MapEntry;
  nearbyEntries: MapEntry[];
}

export interface MapEntry {
  location: GeoLocation;
  airQualityIndex: AirQualityIndex;
}
