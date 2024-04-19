import { ContactNumber } from "@/entities/ContactNumber";

export class AffiliateDetails {
  public name: string | undefined;
  public companyNumber: string | undefined;
  public firmReferenceNumber: string | undefined;
  public isPRAAuthorised: boolean = false;
  public registeredAddress: string | undefined;
  public emailAddress: string | undefined;
  public contactNumber: ContactNumber | undefined;
  public website: string | undefined;
  public tradingAddress: string | undefined;
  public isTradingAddressChanged: boolean = false;
  public contactNumberDisplay?: string = "";
}