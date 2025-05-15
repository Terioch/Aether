import { AirQualityReading } from "./dashboard-view";

export interface MapEntriesView {
  centre: MapEntry;
  nearbyEntries: MapEntry[];
}

export interface MapEntry {
  airQualityReading: AirQualityReading;
}
