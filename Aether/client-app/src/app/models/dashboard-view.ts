import { GlobalLocation } from "./global-location";

export interface DashboardView {
  location: GlobalLocation;
  airQualityIndex: AirQualityIndex;
}

export interface AirQualityIndex {
  location: Geolocation;
  index: number;
  sulfurDioxide: Pollutant;
  nitrogenOxide: Pollutant;
  nitrogenDioxide: Pollutant;
  particulateMatter10: Pollutant;
  particulateMatter2_5: Pollutant;
  ozone: Pollutant;
  carbonMonoxide: Pollutant;
  ammonia: Pollutant;
}

export interface Pollutant {
  index: number;
  concentration: number;
}

export interface GeoLocation {
  latitude: number;
  longitude: number;
}
