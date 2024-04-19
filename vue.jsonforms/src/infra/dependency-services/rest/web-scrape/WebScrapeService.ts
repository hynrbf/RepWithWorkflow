import { singleton } from "tsyringe";
import { IWebScrapeService } from "./IWebScrapeService";
import RestBase from "@/infra/dependency-services/rest/RestBase";
import { REMOTE_API } from "@/config";

@singleton()
export default class WebScrapeService
  extends RestBase
  implements IWebScrapeService
{
  public async webScrapeAndRegisterAsync(email: string, fpid: string) {
    return await this.getRemoteAsync(
      `${REMOTE_API}/WebScrapeAndRegisterAsync?email=${email}&fpId=${fpid}`,
    );
  }

  public async registerMediaAsync(id: string, url: string, customerId: string) {
    try {
      return await this.getRemoteAsync(
        `${REMOTE_API}/RegisterMediaAsync?customerId=${customerId}&mediaid=${id}&url=${url}`,
      );
    } catch (error) {
      return Promise.reject(error);
    }
  }
}
