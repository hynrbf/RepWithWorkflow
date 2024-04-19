import axios, { AxiosRequestConfig } from "axios";
import { USE_REMOTE_DB, APP_CACHE_ENABLE, SECURE_APP_ENABLE } from "@/config";
import { AppConstants } from "../../AppConstants";
import { RequestConfigOptions } from "@/infra/dependency-services/rest/RequestConfigOptions";

export default abstract class RestBase {
  constructor() {
    if (!USE_REMOTE_DB && !APP_CACHE_ENABLE) {
      throw "You cannot have both REMOTE_DB_ENABLE and APP_CACHE_ENABLE be false";
    }
  }

  protected async getRemoteAsync<T>(endPoint: string): Promise<T> {
    const config =
      this.createHeaderConfig() as AxiosRequestConfig<RequestConfigOptions>;
    return await this.getRemoteWithConfigAsync<T>(endPoint, config);
  }

  protected async getRemoteWithConfigAsync<T>(
    endPoint: string,
    config: AxiosRequestConfig<RequestConfigOptions>,
  ): Promise<T> {
    return await axios.get<T>(endPoint, config).then((response) => {
      if (response.status < 200 || response.status >= 300) {
        throw new Error(response.statusText);
      }

      return response.data;
    });
  }

  protected async postRemoteAsync<T>(
    endPoint: string,
    data: unknown,
    isFormData: boolean = false,
  ): Promise<T> {
    return this.postRemoteWithConfigAsync<T>(endPoint, data, isFormData);
  }

  protected async postRemoteWithConfigAsync<T>(
    endPoint: string,
    data: unknown,
    isFormData: boolean = false,
    otherConfig: object = {},
  ): Promise<T> {
    let config = this.createHeaderConfig();

    if (isFormData) {
      config = this.createHeaderFormDataConfig();
    }

    return await axios
      .post<T>(endPoint, data, { ...config, ...otherConfig })
      .then((response) => {
        return response.data;
      });
  }

  protected async putRemoteAsync<T>(
    endPoint: string,
    data: unknown,
  ): Promise<T> {
    const config = this.createHeaderConfig();
    return await axios.put<T>(endPoint, data, config).then((response) => {
      return response.data;
    });
  }

  protected async deleteRemoteAsync<T>(endPoint: string): Promise<T> {
    const config = this.createHeaderConfig();
    return await axios.delete<T>(endPoint, config).then((response) => {
      return response.data;
    });
  }

  protected createHeaderConfig():
    | AxiosRequestConfig<RequestConfigOptions>
    | undefined {
    if (!SECURE_APP_ENABLE) {
      return undefined;
    }

    const authToken = localStorage.getItem(AppConstants.authTokenCacheKey);
    const config: RequestConfigOptions = {
      maxBodyLength: Infinity,
      headers: {
        "Content-Type": "application/json",
        "X-ZUMO-AUTH": authToken,
      },
    };
    return config as AxiosRequestConfig<RequestConfigOptions>;
  }

  private createHeaderFormDataConfig():
    | AxiosRequestConfig<RequestConfigOptions>
    | undefined {
    if (!SECURE_APP_ENABLE) {
      return undefined;
    }

    const authToken = localStorage.getItem(AppConstants.authTokenCacheKey);
    const config: RequestConfigOptions = {
      maxBodyLength: Infinity,
      headers: {
        "X-ZUMO-AUTH": authToken,
      },
    };
    return config as AxiosRequestConfig<RequestConfigOptions>;
  }
}
