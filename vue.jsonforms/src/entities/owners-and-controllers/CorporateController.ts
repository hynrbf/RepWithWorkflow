import { Director } from "./Director";
import { CompanyDetailsBase } from "../firm-details/CompanyDetailsBase";
import { IndividualController } from "@/entities/owners-and-controllers/IndividualController";
import { ContactNumber } from "@/entities/ContactNumber";
import { IShareholder } from "@/entities/firm-details/IShareholder";

export class CorporateController
  extends CompanyDetailsBase
  implements IShareholder
{
  public firmType: string = "";
  public financialSolvency: string = "";
  public registeredAddress?: string = "";
  public isRegisteredAddressChanged: boolean = false;
  public isTradingAddressSameAsRegisteredAddress: boolean = false;
  public tradingAddress: string = "";
  public isTradingAddressChanged: boolean = false;
  public isHeadOfficeAddressSameAsTradingAddress: boolean = false;
  public headOfficeAddress: string = "";
  public emailAddress: string = "";
  public contactNumber: ContactNumber = new ContactNumber();
  public percentageOfCapital: number | null = null;
  public percentageOfVotingRights: number | null = null;
  public individualOwners: number = 0;
  public corporateOwners: number = 0;
  public isSubjectToRegulationByAnotherRegulator: boolean | undefined;
  public isThirdCountryFirm: boolean | undefined;
  public thirdCountryFirmInfo: string = "";
  public isMemberOfFinancialConglomerate: boolean | undefined;
  public memberOfFinancialConglomerateInfo: string = "";
  public isMemberOfThirdCountryFinancialConglomerate: boolean | undefined;
  public memberOfThirdCountryFinancialConglomerateInfo: string = "";
  public isMemberOfThirdCountryBanking: boolean | undefined;
  public memberOfThirdCountryBankingInfo: string = "";
  public hasBeenSubjectToAnyMaterialComplaints: boolean | undefined;
  public beenSubjectToAnyMaterialComplaintsInfo: string = "";
  public directors: Director[] = [] as Director[];
  public individualControllers: IndividualController[] =
    [] as IndividualController[];
  public corporateControllers: CorporateController[] =
    [] as CorporateController[];
  public supportingDocumentsUrls: string[] = [];
}