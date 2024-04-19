import { ContactNumber } from "../ContactNumber";

export class ProviderRepresentative {
  public title: string | undefined;
  public forename: string | undefined = "";
  public surname: string | undefined = "";
  public emailAddress: string | undefined;
  public contactNumber: ContactNumber | undefined;
  public jobTitle: string | undefined;
  public contactNumberDisplay?: string = "";
  public notApplicable: boolean = false;
}
