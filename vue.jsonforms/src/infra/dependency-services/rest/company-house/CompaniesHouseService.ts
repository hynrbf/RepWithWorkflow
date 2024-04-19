import { singleton } from "tsyringe";
import RestBase from "../RestBase";
import { REMOTE_API, USE_REMOTE_DB } from "@/config";
import { ICompaniesHouseService } from "./ICompaniesHouseService";
import { CompanyEntity } from "@/entities/CompanyEntity";
import { Controller } from "@/entities/firm-details/Controller";
import { CompanyHouseProfile } from "@/entities/firm-details/CompanyHouseProfile";
import { CompanyHouseStatus } from "@/entities/CompanyHouseStatus";

@singleton()
export default class CompaniesHouseService
  extends RestBase
  implements ICompaniesHouseService
{
  public async searchCompaniesAsync(keyword: string): Promise<CompanyEntity[]> {
    if (!USE_REMOTE_DB) {
      throw new Error(
        "Searching firms from Company house will not work if run locally. Please connect to outside services. eg",
      );
    }

    return await this.getRemoteAsync<CompanyEntity[]>(
      `${REMOTE_API}/SearchCompaniesAsync?keyword=${keyword}`,
    );
  }

  public async getIndividualControllersAsync(
    companyNumber: string,
  ): Promise<Controller[]> {
    const queryString = `companyNo=${companyNumber}`;
    return await this.getRemoteAsync<Controller[]>(
      `${REMOTE_API}/GetIndividualControllersAsync?${queryString}`,
    );
  }

  public async getCorporateControllersAsync(
    companyNumber: string,
  ): Promise<Controller[]> {
    const queryString = `companyNo=${companyNumber}`;
    return await this.getRemoteAsync<Controller[]>(
      `${REMOTE_API}/GetCorporateControllersAsync?${queryString}`,
    );
  }

  public async getCompanyProfileAsync(
    companyNumber: string,
  ): Promise<CompanyHouseProfile> {
    const queryString = `companyNo=${companyNumber}`;
    return await this.getRemoteAsync<CompanyHouseProfile>(
      `${REMOTE_API}/GetCompanyProfileAsync?${queryString}`,
    );
  }

  public async getCompanyHouseDefinedStatusesAsync(): Promise<
    CompanyHouseStatus[]
  > {
    return await this.getRemoteAsync<CompanyHouseStatus[]>(
      `${REMOTE_API}/GetAllCompanyHouseStatusesAsync`,
    );
  }
}
