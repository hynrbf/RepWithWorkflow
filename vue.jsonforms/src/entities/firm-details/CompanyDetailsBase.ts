import { AppConstants } from "@/infra/AppConstants";
import { FirmBasicInfo } from "@/entities/FirmBasicInfo";

export class CompanyDetailsBase {
  public companyName?: string;
  public countryOfIncorporation: string | undefined =
    AppConstants.DefaultCountryCode.toUpperCase();
  public companyNumber: string = "";
  public firmReferenceNumber: string = "";
  public natureOfBusiness: string | undefined;
  public countryCode?: string | undefined =
    AppConstants.DefaultCountryCode.toUpperCase();
  public firm: FirmBasicInfo = new FirmBasicInfo();
} 