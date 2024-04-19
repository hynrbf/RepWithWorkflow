import { ScopeofReferalProduct } from "./ScopeofReferalProduct";

export class IntroducerReferalProduct {

    public isReferralsprovided: boolean = false;
    public fixedFeePerReferral: string = "";
    public fixedFeeAndAPercentageOfRevenue: string = "";
    public remunerationVaryByProduct: string = "";
    public remuneratedInitiallyOnRenewal: string = "";
    public monthsNotice: number = 0;
    public products: ScopeofReferalProduct[] = [] as  ScopeofReferalProduct[];
  }