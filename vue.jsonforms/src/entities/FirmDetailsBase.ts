import { ContactNumber } from "@/entities/ContactNumber";

export abstract class FirmDetailsBase {
  public firmName?: string;
  public companyNumber?: string;
  public firmReferenceNumber?: string;
  public tradingNames: string[] = [];
  public isTradingNamesChanged?: boolean = false;
  public registeredAddress?: string;
  public isRegisteredAddressChanged: boolean = false;
  public tradingAddress?: string;
  public isTradingAddressChanged: boolean = false;
  public isTradingSameAsRegisteredAddress: boolean = false;
  public emailAddress?: string;
  public website?: string;
  public contactNumber?: ContactNumber;
}