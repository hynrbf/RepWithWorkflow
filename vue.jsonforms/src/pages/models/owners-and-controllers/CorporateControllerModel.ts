import { Director } from "@/entities/owners-and-controllers/Director";
import { CorporateControllerCollapsibleItem } from "@/pages/models/owners-and-controllers/CorporateControllerCollapsibleItem";
import { IndividualControllerModel } from "@/pages/models/owners-and-controllers/IndividualControllerModel";
import { IndividualControllerCollapsibleItemModel } from "@/pages/models/owners-and-controllers/IndividualControllerCollapsibleItemModel";
import { ContactNumber } from "@/entities/ContactNumber";
import { FirmBasicInfo } from "@/entities/FirmBasicInfo";
import {v4 as uuid} from "uuid";
import {IShareholder} from "@/entities/firm-details/IShareholder";

export class CorporateControllerModel implements IShareholder {
  public id: string = uuid();
  public companyName: string | undefined = "";
  public countryOfIncorporation: undefined | string = "";
  public companyNumber: string = "";
  public firmReferenceNumber: string = "";
  public firm: FirmBasicInfo = new FirmBasicInfo();
  public natureOfBusiness: undefined | string = "";
  public firmType: string = "";
  public financialSolvency: string = "";
  public registeredAddress: string = "";
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
  public individualOwnersActual: number = 0;
  public corporateOwners: number = 0;
  public corporateOwnersActual: number = 0;
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
  public individualControllers: IndividualControllerModel[] =
    [] as IndividualControllerModel[];
  public individualControllerItems: IndividualControllerCollapsibleItemModel[] =
    [];
  public corporateControllers: CorporateControllerModel[] =
    [] as CorporateControllerModel[];
  public corporateControllerItems: CorporateControllerCollapsibleItem[] = [];
  public corporateControllerTabItems: { title: string; content: string }[] = [];
  public supportingDocumentsUrls: string[] = [];
}
