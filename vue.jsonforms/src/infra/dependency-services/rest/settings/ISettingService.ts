import { SettingEntity } from "@/entities/SettingEntity";

export declare interface ISettingService {
  initializeSettingsAsync(): Promise<string>;

  getSettingValueByKeyAsync(key: string): Promise<string>;

  getAllSettingsAsync(): Promise<SettingEntity[]>;

  saveAllSettingsAsync(settings: SettingEntity[]): Promise<SettingEntity[]>;
}

export const ISettingServiceInfo = {
  name: "ISettingService",
};
