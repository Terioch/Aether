import { GlobalLocation } from "./global-location";

export interface DashboardView {
  location: GlobalLocation;
  airQuality: AirQuality;
}

export interface AirQuality {
  location: GeoLocation;
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
