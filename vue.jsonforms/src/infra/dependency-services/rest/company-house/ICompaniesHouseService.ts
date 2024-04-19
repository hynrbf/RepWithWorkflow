import { CompanyEntity } from "@/entities/CompanyEntity";
import { Controller } from "@/entities/firm-details/Controller";
import { CompanyHouseProfile } from "@/entities/firm-details/CompanyHouseProfile";
import { CompanyHouseStatus } from "@/entities/CompanyHouseStatus";

export declare interface ICompaniesHouseService {
  searchCompaniesAsync(keyword: string): Promise<CompanyEntity[]>;

  getIndividualControllersAsync(companyNumber: string): Promise<Controller[]>;

  getCorporateControllersAsync(companyNumber: string): Promise<Controller[]>;

  getCompanyProfileAsync(companyNumber: string): Promise<CompanyHouseProfile>;

  getCompanyHouseDefinedStatusesAsync(): Promise<CompanyHouseStatus[]>;
}

export const ICompaniesHouseServiceInfo = {
  name: "ICompaniesHouseService",
};
