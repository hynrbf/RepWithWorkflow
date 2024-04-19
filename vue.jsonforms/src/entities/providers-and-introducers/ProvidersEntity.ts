import { ProviderIntroducerDetails } from "@/entities/providers-and-introducers/ProviderIntroducerDetails";
import { ProviderRepresentative } from "@/entities/providers-and-introducers/ProviderRepresentative";
import { ProvidersAndIntroducersBase } from "@/entities/providers-and-introducers/ProvidersAndIntroducersBase";
import { MarketingRep } from "./MarketingRep";
import { ProvidersProductDetails } from "@/entities/providers-and-introducers/ProvidersProductDetails";
import { ProvidersTaskDetails } from "@/entities/providers-and-introducers/ProvidersTaskDetails";
import { OnboardingType } from "@/infra/base";

export class ProvidersEntity extends ProvidersAndIntroducersBase {
  public isContextMenuOpen: boolean = false; // for ui only
  public customerId?: string;
  public details: ProviderIntroducerDetails = new ProviderIntroducerDetails();
  public representative: ProviderRepresentative = new ProviderRepresentative();
  public principalDetails: ProviderIntroducerDetails =
    new ProviderIntroducerDetails();
  public departmentDetails: ProviderRepresentative =
    new ProviderRepresentative();
  public marketingDetails: MarketingRep = new MarketingRep();
  public marketingRepDetails: ProviderRepresentative =
    new ProviderRepresentative();
  public financialNotes: string = "";
  public products: ProvidersProductDetails[] = [] as ProvidersProductDetails[];
  public tasks: ProvidersTaskDetails[] = [] as ProvidersTaskDetails[];
  public onboardingType: string = OnboardingType.Provider;

  // below properties are used in list view rendering
  public representativeName: string = "";
  public startDateString: string = "";
}
