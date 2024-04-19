import { ContactNumber } from "@/entities/ContactNumber";

export class InsuranceProvider {
  public providerName = "";
  public companyNumber: number | undefined;
  public firmReferenceNumber: number | undefined;
  public isPraAuthorized = false;
  public insurerRating: string | undefined;
  public registeredAddress: string | undefined;
  public tradingAddress: string | undefined;
  public emailAddress: string | undefined;
  public contactNumber: ContactNumber | undefined;
  public website: string | undefined;
  public withBindingAuthority: boolean | undefined;
  public haveRiskTransferAgreement: boolean | undefined;
  public allowToCascadeRiskTransferAgreement: boolean | undefined;
}
