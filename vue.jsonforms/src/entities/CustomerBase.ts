import { IndividualController } from "@/entities/owners-and-controllers/IndividualController";
import { CorporateController } from "@/entities/owners-and-controllers/CorporateController";
import { CloseLinkEntity } from "@/entities/CloseLinkEntity";
import { ProfessionalIndemnity } from "@/entities/professional-indemnity/ProfessionalIndemnity";
import { EmployersLiability } from "@/entities/employers-liability/EmployersLiability";
import { FirmRepresentative } from "@/entities/FirmRepresentative";
import { AccountDepartmentDetail } from "@/entities/AccountDepartmentDetail";
import { LocumDetails } from "@/entities/LocumDetails";
import { FirmProfileStatus } from "@/entities/enums/FirmProfileStatus";
import { CompanyEntity } from "@/entities/CompanyEntity";
import { OnBoardingUserBase } from "@/entities/OnBoardingUserBase";
import { Affiliate } from "@/entities/Affiliate/Affiliate";
import { StationeryEntity } from "@/entities/stationery/StationeryEntity";
import { DisclosureEntity } from "@/entities/stationery/DisclosureEntity";
import { MediaMarketingOutlet } from "@/entities/media-marketing-outlet/MediaMarketingOutlet";
import { FinancialPromotion } from "@/entities/financial-promotion/FinancialPromotion";
import { ProfileStatuses } from "@/entities/enums/ProfileStatuses";

export abstract class CustomerBase extends OnBoardingUserBase {
  public firmProfileEditStatus: string =
    FirmProfileStatus.Incomplete.toString();
  public profileStatus: string = ProfileStatuses.Basic.toString();
  public noOfIndividualShareholders: number = 0;
  public noOfCorporateShareholders: number = 0;
  public individualControllers: IndividualController[] = [];
  public corporateControllers: CorporateController[] = [];

  //ToDo. to check if all of these are required in every pages like employee, ar, and the normal customer
  public closeLinks: CloseLinkEntity[] = [];
  public professionalIndemnities: ProfessionalIndemnity[] = [];
  public employersLiabilities: EmployersLiability[] = [];
  public affiliates: Affiliate[] = [];
  public stationeries: StationeryEntity[] = [];
  public mediaMarketingOutlets: MediaMarketingOutlet[] = [];
  public financialPromotions: FinancialPromotion[] = [];

  public disclosure?: DisclosureEntity;
  public selectedCompany?: CompanyEntity;
  public firmRepresentativeDetail?: FirmRepresentative;
  public locumDetail?: LocumDetails;
  public accountDepartmentDetail?: AccountDepartmentDetail;
  public onboardingCompleted?: boolean;

  public changedBy?: string;
  public changedOn?: Date;
  public ipAddress?: string;
}
