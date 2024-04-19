import { FileModel } from "@/pages/firm-profile-pages/stationery/model/FileModel";
import { StationeryStatus } from "./StationeryStatus";

export class StationeryEntity {
  id: string | undefined;
  name?: string;
  icon?: string;
  status?: StationeryStatus;
  files?: FileModel[];
  createdAt?: number;
  updatedAt?: number;
}
