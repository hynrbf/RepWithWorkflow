import { Money } from "@/entities/Money";

export interface NonRegulatedFinancialServicesItems {
  productOrService: string;
  totalProjectAnnualIncome: Money;
}

export interface RemoveRefusedItems {
  status: string;
  providerName: string;
  partyName: string;
  refusedDate: number;
  refusedInfo: string;
  refusedSupportingDocumentsUrls: string[];
}

export interface PrimaryMarketCovered {
  label: string;
  value: string;
  items?: PrimaryMarketCovered[];
  parent?: string;
}

export interface PreviouslyBeenAnARItems {
  principalFirmName?: string;
  firmReferenceNumber?: string;
  startDate?: number;
  endDate?: number;
  reasonForTermination: string;
}