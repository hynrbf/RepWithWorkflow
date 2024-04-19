import { singleton } from "tsyringe";
import { REMOTE_API } from "@/config";
import RestBase from "../RestBase";
import { ISigningService } from "./ISigningService";

@singleton()
export default class SigningService
  extends RestBase
  implements ISigningService
{
  public async sendInviteToSignDocument(
    receiverEmail: string,
  ): Promise<boolean> {
    const result = await this.postRemoteAsync<string>(
      `${REMOTE_API}/SendInviteDocumentSignAsync`,
      receiverEmail,
    );
    return JSON.parse(result);
  }
}