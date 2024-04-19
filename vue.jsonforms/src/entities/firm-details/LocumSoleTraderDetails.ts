import { ContactNumber } from "@/entities/ContactNumber";

export class LocumSoleTraderDetails {
  public foreName?: string;
  public surName?: string;
  public firmReferenceNumber?: string;
  public tradingAddress?: string;
  public isTradingAddressChanged: boolean = false;
  public emailAddress?: string;
  public contactNumber?: ContactNumber;
  public website?: string;
}