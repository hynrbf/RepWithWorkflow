import { singleton } from "tsyringe";
import { IAppointedRepresentativeService } from "./IAppointedRepresentativeService";
import { AppointedRepresentative } from "@/entities/appointed-representatives/AppointedRepresentative";
import { AppointedRepresentativeCollection } from "@/entities/appointed-representatives/AppointedRepresentativeCollection";
import RestBase from "@/infra/dependency-services/rest/RestBase";
import { REMOTE_API } from "@/config";
import isEmpty from "lodash/isEmpty";

@singleton()
export default class AppointedRepresentativeService
  extends RestBase
  implements IAppointedRepresentativeService
{
  public async getAppointedRepresentativeAsync(
    id: string,
  ): Promise<AppointedRepresentative | undefined> {
    try {
      return await this.getRemoteAsync<AppointedRepresentative>(
        `${REMOTE_API}/GetAppointedRepresentativeAsync/${id}`,
      );
    } catch (error) {
      return Promise.reject(error);
    }
  }

  public async saveOrUpdateAppointedRepresentativeAsync(
    appointedRepresentative: Partial<AppointedRepresentative>,
  ): Promise<AppointedRepresentative> {
    try {
      return await this.postRemoteAsync<AppointedRepresentative>(
        `${REMOTE_API}/SaveOrUpdateAppointedRepresentativeAsync`,
        JSON.stringify(appointedRepresentative),
      );
    } catch (error) {
      return Promise.reject(error);
    }
  }

  public async getAppointedRepresentativesAsync(
    customerId?: string,
    params?: Record<string, unknown>,
  ): Promise<AppointedRepresentativeCollection> {
    try {
      let response: AppointedRepresentativeCollection | null;

      if (isEmpty(params)) {
        response = await this.getRemoteAsync<AppointedRepresentativeCollection>(
          `${REMOTE_API}/GetAppointedRepresentativesAsync/${customerId}`,
        );
      } else {
        response =
          await this.postRemoteAsync<AppointedRepresentativeCollection>(
            `${REMOTE_API}/GetAppointedRepresentativesAsync/${customerId}`,
            params,
          );
      }

      return response;
    } catch (error) {
      return Promise.reject(error);
    }
  }

  public async getAppointedRepresentativesByEmailAsync(
    email: string,
  ): Promise<AppointedRepresentative> {
    try {
      return await this.getRemoteAsync<AppointedRepresentative>(
        `${REMOTE_API}/GetAppointedRepresentativeByEmailAsync/${email}`,
      );
    } catch (error) {
      return Promise.reject(error);
    }
  }

  public async deleteAppointedRepresentativeAsync(
    id: string,
  ): Promise<boolean> {
    try {
      return await this.deleteRemoteAsync<boolean>(
        `${REMOTE_API}/DeleteAppointedRepresentativeAsync/${id}`,
      );
    } catch (error) {
      return Promise.reject(error);
    }
  }
}
