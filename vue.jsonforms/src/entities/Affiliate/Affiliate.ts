import { ProviderRepresentative } from "@/entities/providers-and-introducers/ProviderRepresentative";
import { ProvidersProductDetails } from "@/entities/providers-and-introducers/ProvidersProductDetails";
import { AffiliateDetails } from "@/entities/Affiliate/AffiliateDetails";
import { ProvidersAndIntroducersBase } from "../providers-and-introducers/ProvidersAndIntroducersBase";

export class Affiliate extends ProvidersAndIntroducersBase {
  public id: string = "";
  public details: AffiliateDetails = new AffiliateDetails();
  public marketingProviderDetails: AffiliateDetails = new AffiliateDetails();
  public representative: ProviderRepresentative = new ProviderRepresentative();
  public products: ProvidersProductDetails[] = [] as ProvidersProductDetails[];
  public isAffiliateMarketingProvider: boolean = false;

  public status: string = "";
  public tasks: string = "";
}