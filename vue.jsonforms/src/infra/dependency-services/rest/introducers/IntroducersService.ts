import { singleton } from "tsyringe";
import { IIntroducersService } from "./IIntroducersService";
import RestBase from "@/infra/dependency-services/rest/RestBase";
import { REMOTE_API } from "@/config";
import {IntroducersEntity} from "@/entities/providers-and-introducers/IntroducersEntity";
import {v4 as uuidV4} from "uuid";

@singleton()
export default class IntroducersService
  extends RestBase
  implements IIntroducersService
{

  public async saveOrUpdateIntroducersAsync(
      introducer: Partial<IntroducersEntity>,
  ): Promise<IntroducersEntity> {
    try {
      if (!introducer.id || introducer.id.length === 0) {
        introducer.id = uuidV4();
      }

      return await this.postRemoteAsync<IntroducersEntity>(
        `${REMOTE_API}/SaveOrUpdateIntroducersAsync`,
        JSON.stringify(introducer),
      );
    } catch (error) {
      return Promise.reject(error);
    }
  }

  public async getIntroducerByEmailAsync(
    email: string,
  ): Promise<IntroducersEntity> {
    try {
      return await this.getRemoteAsync<IntroducersEntity>(
        `${REMOTE_API}/GetIntroducerByEmailAsync/${email}`,
      );
    } catch (error) {
      return Promise.reject(error);
    }
  }

  public async getIntroducersByCustomerIdAsync(
      customerId: string,
  ): Promise<IntroducersEntity[]> {
    try {
      return await this.getRemoteAsync<IntroducersEntity[]>(
          `${REMOTE_API}/GetIntroducerByCustomerIdAsync/${customerId}`,
      );
    } catch (error) {
      return Promise.reject(error);
    }
  }

  public async deleteIntroducerAsync(
    id: string,
  ): Promise<boolean> {
    try {
      return await this.deleteRemoteAsync<boolean>(
        `${REMOTE_API}/DeleteIntroducerAsync/${id}`,
      );
    } catch (error) {
      return Promise.reject(error);
    }
  }
}
