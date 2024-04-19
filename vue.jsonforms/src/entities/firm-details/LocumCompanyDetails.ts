import { ContactNumber } from "@/entities/ContactNumber";

export class LocumCompanyDetails {
  public firmName?: string;
  public companyNumber?: string;
  public fcaStatus?: string;
  public companyHouseStatus?: string;
  public firmReferenceNumber?: string;
  public registeredAddress?: string;
  public isRegisteredAddressChanged: boolean = false;
  public tradingAddress?: string;
  public isTradingAddressChanged: boolean = false;
  public isTradingSameAsRegisteredAddress: boolean = false;
  public emailAddress?: string;
  public contactNumber?: ContactNumber;
  public website?: string;
  public representativeTitle?: string;
  public representativeForename?: string;
  public representativeSurname?: string;
  public representativeEmailAddress?: string;
  public representativeMobileNumber?: ContactNumber;
  public representativeRole?: string;
}