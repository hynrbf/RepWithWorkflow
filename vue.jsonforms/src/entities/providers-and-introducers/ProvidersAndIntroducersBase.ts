import {CustomerBase} from "@/entities/CustomerBase";

export class ProvidersAndIntroducersBase extends CustomerBase {
  public startDate?: number; //this is in epoch
  public status: string = "";
  public ddStatus: string = "";
  public statusImg: string = "";
  public providerImg: string = "";
}