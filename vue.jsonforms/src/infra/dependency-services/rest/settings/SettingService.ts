import { singleton } from "tsyringe";
import RestBase from "../RestBase";
import { ISettingService } from "./ISettingService";
import { REMOTE_API, USE_REMOTE_DB } from "@/config";
import { SettingEntity } from "@/entities/SettingEntity";

@singleton()
export default class SettingService
  extends RestBase
  implements ISettingService
{
  public async getSettingValueByKeyAsync(key: string): Promise<string> {
    const value = await this.getRemoteAsync<SettingEntity>(
      `${REMOTE_API}/GetSettingByKeyAsync/${key}`,
    );
    return value?.value;
  }

  public async getAllSettingsAsync(): Promise<SettingEntity[]> {
    return await this.getRemoteAsync<SettingEntity[]>(
      `${REMOTE_API}/GetAllSettingsAsync`,
    );
  }

  public async saveAllSettingsAsync(
    settings: SettingEntity[],
  ): Promise<SettingEntity[]> {
    return await this.postRemoteAsync<SettingEntity[]>(
      `${REMOTE_API}/SaveSettingsMultipleAsync`,
      JSON.stringify(settings),
    );
  }

  public async initializeSettingsAsync(): Promise<string> {
    if (!USE_REMOTE_DB) {
      return "App is not connected to remote DB";
    }

    try {
      const response = await this.postRemoteAsync<string>(
        `${REMOTE_API}/InitializeSettingsAsync`,
        "",
      );
      return response?.toString();
    } catch {
      return "";
    }
  }
}
