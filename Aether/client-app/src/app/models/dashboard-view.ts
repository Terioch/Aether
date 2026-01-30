import { GlobalLocation } from "./global-location";

export interface DashboardView {
  location: GlobalLocation;
  airQualityReading: AirQualityReading;
}

export interface AirQualityReading {
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
  name: string;
  index: number;
  concentration: number;
  max: number;
}

export interface GeoLocation {
  latitude: number;
  longitude: number;
}
