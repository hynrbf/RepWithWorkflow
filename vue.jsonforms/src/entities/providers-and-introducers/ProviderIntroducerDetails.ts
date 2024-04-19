import { ContactNumber } from "../ContactNumber";
import {ProductType} from "@/entities/org-structure/ProductType";

export class ProviderIntroducerDetails {
  public name?: string;
  public foreName?: string;
  public lastName?: string;
  public companyNumber?: string;
  public fcaFirmRefNo?: string;
  public praFirmRefNo?: string;
  public registeredAddress?: string;
  public tradingAddress?: string;
  public homeAddress?: string;
  public emailAddress?: string;
  public contactNumber?: ContactNumber;
  public website?: string;
  public tradingName?: string;
  public notes?: string;
  public praAuthorised?: boolean;
  public isTradingSameAsRegisteredAddress: boolean = false;
  public contactNumberDisplay?: string = "";
  public isCompany: boolean = true;
  public productType?: ProductType[] = [];
}
