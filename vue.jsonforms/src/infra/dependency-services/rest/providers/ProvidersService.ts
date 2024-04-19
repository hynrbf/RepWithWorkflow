import { singleton } from "tsyringe";
import { IProvidersService } from "./IProvidersService";
import RestBase from "@/infra/dependency-services/rest/RestBase";
import { REMOTE_API } from "@/config";
import {ProvidersEntity} from "@/entities/providers-and-introducers/ProvidersEntity";
import {v4 as uuidV4} from "uuid";

@singleton()
export default class ProvidersService
  extends RestBase
  implements IProvidersService
{

  public async saveOrUpdateProvidersAsync(
      provider: Partial<ProvidersEntity>,
  ): Promise<ProvidersEntity> {
    try {
      if (!provider.id || provider.id.length === 0) {
        provider.id = uuidV4();
      }

      return await this.postRemoteAsync<ProvidersEntity>(
        `${REMOTE_API}/SaveOrUpdateProvidersAsync`,
        JSON.stringify(provider),
      );
    } catch (error) {
      return Promise.reject(error);
    }
  }

  public async getProviderByEmailAsync(
    email: string,
  ): Promise<ProvidersEntity> {
    try {
      return await this.getRemoteAsync<ProvidersEntity>(
        `${REMOTE_API}/GetProviderByEmailAsync/${email}`,
      );
    } catch (error) {
      return Promise.reject(error);
    }
  }

  public async getProvidersByCustomerIdAsync(
      customerId: string,
  ): Promise<ProvidersEntity[]> {
    try {
      return await this.getRemoteAsync<ProvidersEntity[]>(
          `${REMOTE_API}/GetProviderByCustomerIdAsync/${customerId}`,
      );
    } catch (error) {
      return Promise.reject(error);
    }
  }

  public async deleteProviderAsync(
    id: string,
  ): Promise<boolean> {
    try {
      return await this.deleteRemoteAsync<boolean>(
        `${REMOTE_API}/DeleteProviderAsync/${id}`,
      );
    } catch (error) {
      return Promise.reject(error);
    }
  }
}
