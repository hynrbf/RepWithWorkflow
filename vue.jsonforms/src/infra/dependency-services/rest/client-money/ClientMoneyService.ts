import { singleton } from "tsyringe";
import { IClientMoneyService } from "./IClientMoneyService";
import { InsurerRating } from "@/entities/client-money/InsurerRating";
import RestBase from "@/infra/dependency-services/rest/RestBase";
import { REMOTE_API } from "@/config";

@singleton()
export default class ClientMoneyMockService
  extends RestBase
  implements IClientMoneyService
{
  async getInsurerRatingsAsync(company: string): Promise<InsurerRating> {
    try {
      return await this.getRemoteAsync<InsurerRating>(
        `${REMOTE_API}/GetInsurerRatingsAsync?company=${encodeURIComponent(
          company,
        )}`,
      );
    } catch (error) {
      return Promise.reject(error);
    }
  }
}
