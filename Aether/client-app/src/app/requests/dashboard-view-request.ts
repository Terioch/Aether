import { GeoLocation } from "../models/geo-location";

export interface DashboardViewRequest {
  readingId?: number;
  location: GeoLocation;
}
