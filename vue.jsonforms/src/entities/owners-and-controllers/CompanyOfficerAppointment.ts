import { AppConstants } from "@/infra/AppConstants";
import { FirmBasicInfo } from "@/entities/FirmBasicInfo";
import { v4 as uuid } from "uuid";

export class CompanyOfficerAppointment {
  public id: string = uuid();
  public companyName?: string;
  public country?: string = AppConstants.DefaultCountryCode.toUpperCase();
  public countryCode?: string = AppConstants.DefaultCountryCode.toUpperCase();
  public companyNumber?: string;
  public firmReferenceNumber?: string;
  public natureOfBusiness?: string;
  public directorshipStartDate?: number;
  public directorshipEndDate?: number;
  public firm: FirmBasicInfo = new FirmBasicInfo();
  public name_elements:
    | { title: string; forename: string; surname: string }
    | undefined;
  public resigned_on?: string;
  public originalResignedOn?: string;
}
