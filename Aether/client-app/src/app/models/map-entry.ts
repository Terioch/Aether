import { GeoLocation } from "./air-quality";
import { AirQualityIndex } from "./dashboard-view";

export interface MapEntry {
  location: GeoLocation;
  airQualityIndex: AirQualityIndex;
}
