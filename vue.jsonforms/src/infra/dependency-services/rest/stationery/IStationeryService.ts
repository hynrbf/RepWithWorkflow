import { StationeryEntity } from "@/entities/stationery/StationeryEntity";

export declare interface IStationeryService {
  getStationeriesAsync(): Promise<StationeryEntity[]>;
  getStationeryAsync(id: string): Promise<StationeryEntity | undefined>;
  addStationeryAsync(
    payload: Record<string, unknown>
  ): Promise<StationeryEntity>;
  updateStationeryAsync(
    id: string,
    payload: Record<string, unknown>
  ): Promise<StationeryEntity>;
}

export const IStationeryServiceInfo = {
  name: "IStationeryService",
};
