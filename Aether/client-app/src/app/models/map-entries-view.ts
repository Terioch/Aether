import { GeoLocation } from "./air-quality";
import { AirQuality as AirQuality } from "./dashboard-view";

export interface MapEntriesView {
  centre: MapEntry;
  nearbyEntries: MapEntry[];
}

export interface MapEntry {
  airQuality: AirQuality;
}
