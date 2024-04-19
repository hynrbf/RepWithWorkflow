import { ProviderRepresentative } from "@/entities/providers-and-introducers/ProviderRepresentative";
import { ProviderIntroducerDetails } from "@/entities/providers-and-introducers/ProviderIntroducerDetails";
import { ProvidersAndIntroducersBase } from "@/entities/providers-and-introducers/ProvidersAndIntroducersBase";
import { IntroducerReferalProduct } from "@/entities/providers-and-introducers/IntroducerReferalProduct";
import { ProvidersProductDetails } from "./ProvidersProductDetails";
import { ProvidersTaskDetails } from "./ProvidersTaskDetails";
import {OnboardingType} from "@/infra/base";

export class IntroducersEntity extends ProvidersAndIntroducersBase {
  public customerId?: string;
  public details: ProviderIntroducerDetails = new ProviderIntroducerDetails();
  public representative: ProviderRepresentative = new ProviderRepresentative();
  public fcaAuthorisationStatus: string = "";
  public introducerName: string = "";
  public startDateStr: string = "";
  public referral: IntroducerReferalProduct = new IntroducerReferalProduct();
  public products: ProvidersProductDetails[] = [] as ProvidersProductDetails[];
  public tasks: ProvidersTaskDetails[] = [] as ProvidersTaskDetails[];

  public bankName: string = "";
  public accountName: string = "";
  public sortCode: string = "";
  public accountNumber: number = 0;

  public departmentDetails: ProviderRepresentative =
    new ProviderRepresentative();
  public principalDetails: ProviderIntroducerDetails =
    new ProviderIntroducerDetails();

  public isContextMenuOpen: boolean = false; // for ui only
  public onboardingType: string = OnboardingType.Introducer;
}