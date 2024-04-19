import { LocumSoleTraderDetails } from "@/entities/firm-details/LocumSoleTraderDetails";
import { LocumCompanyDetails } from "@/entities/firm-details/LocumCompanyDetails";

export class LocumDetails {
  public isDependentOnSingleKeyIndividual?: boolean;
  public isLocumCompany?: boolean;
  public hasAppointedALocum?: boolean;
  public hasNoAppointedLocumInfo?: string;
  public locumCompanyDetail?: LocumCompanyDetails;
  public locumSoleTraderDetail?: LocumSoleTraderDetails;
}