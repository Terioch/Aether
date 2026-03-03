import { GlobalLocation } from "./global-location";

export interface DashboardView {
  location: GlobalLocation;
  airQualityReading: AirQualityReading;
  changePercentages: ChangePercentages;
}

export interface AirQualityReading {
  id: number;
  location: AirQualityLocation;
  index: number;
  aqi: number;
  sulfurDioxide: Pollutant;
  nitrogenOxide: Pollutant;
  nitrogenDioxide: Pollutant;
  particulateMatter10: Pollutant;
  particulateMatter2_5: Pollutant;
  ozone: Pollutant;
  carbonMonoxide: Pollutant;
  ammonia: Pollutant;
  lastUpdated: string;
}

export interface AirQualityLocation {
  name: string;
  latitude: number;
  longitude: number;
}

export interface Pollutant {
  name: string;
  index: number;
  concentration: number;
  max: number;
}

interface ChangePercentages {
  aqi: number;
  carbonMonoxide: number;
  sulfurDioxide: number;
  nitrogenDioxide: number;
  nitrogenOxide: number;
  ozone: number;
  particulateMatter10: number;
  particulateMatter2_5: number;
  ammonia: number;
}
