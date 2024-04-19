import { singleton } from "tsyringe";
import { IUserSubmittedChangesService } from "@/infra/dependency-services/user-submission/IUserSubmittedChangesService";

@singleton()
export default class UserSubmittedChangesService
  implements IUserSubmittedChangesService
{
  hasUserSubmittedChangesToRemoteApi: boolean = true;
}
