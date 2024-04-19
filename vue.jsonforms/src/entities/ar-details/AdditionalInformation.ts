import {
  NonRegulatedFinancialServicesItems,
  RemoveRefusedItems,
  PreviouslyBeenAnARItems,
  PrimaryMarketCovered,
} from "@/entities/ar-details/IAdditionalInformation";

export class AdditionalInformation {
  public hasAnyInsurerEverDeclined?: boolean;
  public declinedInfo: string = "";
  public declinedSupportingDocumentsUrls: string[] = [];
  public hasBeenRemovedOrRefused?: boolean;
  public hasBeenRemovedOrRefusedItems: RemoveRefusedItems[] = [];
  public hasPreviouslyBeenAnAR?: boolean;
  public hasPreviouslyBeenAnARItems: PreviouslyBeenAnARItems[] = [];
  public primaryMarketCovered: PrimaryMarketCovered[] = [];
  public permittedToUndertakeRegulatedActivities: string = "";
  public contractedToPortfolioManagement?: boolean;
  public portfolioManagementInfo: string = "";
  public portfolioManagementSupportingDocumentsUrls: string[] = [];
  public willBeTiedAgent?: boolean;
  public willBeTiedAgentInfo: string = "";
  public willBeTiedAgentSupportingDocumentsUrls: string[] = [];
  public provideAnyRegulatedServices?: boolean;
  public provideAnyRegulatedServicesInfo: string = "";
  public provideAnyRegulatedServicesSupportingDocumentsUrls: string[] = [];
  public conductAnyNonRegulatedActivities?: boolean;
  public conductAnyNonRegulatedActivitiesInfo: string = "";
  public conductAnyNonRegulatedActivitiesSupportingDocumentsUrls: string[] = [];
  public includeAnyNonRegulatedFinancialServices?: boolean;
  public nonRegulatedFinancialServicesInfo: string = "";
  public nonRegulatedFinancialServicesSupportingDocumentsUrls: string[] = [];
  public nonRegulatedFinancialServicesItems: NonRegulatedFinancialServicesItems[] =
    [];

  [key: string]:
    | boolean
    | string
    | string[]
    | PreviouslyBeenAnARItems[]
    | RemoveRefusedItems[]
    | NonRegulatedFinancialServicesItems[]
    | PrimaryMarketCovered[]
    | undefined;
}