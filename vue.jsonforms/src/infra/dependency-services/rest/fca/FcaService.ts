import { singleton } from "tsyringe";
import RestBase from "../RestBase";
import { REMOTE_API, USE_REMOTE_DB } from "@/config";
import { CompanyEntity } from "@/entities/CompanyEntity";
import { AppConstants } from "@/infra/AppConstants";
import { FcaAuthStatus } from "@/entities/FcaAuthStatus";
import { IFcaService } from "./IFcaService";
import { PermissionEdit } from "@/entities/PermissionEdit";
import { PermissionResult } from "@/entities/PermissionResult";
import { FcaAddressDetail } from "@/entities/FcaAddressDetail";
import { FcaIndividual } from "@/entities/FcaIndividual";
import { AppointedRepresentative } from "@/entities/appointed-representatives/AppointedRepresentative";
import { FcaFirmDetail } from "@/entities/firm-details/FcaFirmDetail";
import { CustomerPermission } from "@/entities/CustomerPermission";
import { PermissionStateEnum } from "@/entities/enums/PermissionStateEnum";
import { TradingAddress } from "@/entities/firm-details/TradingAddress";

@singleton()
export default class FcaService extends RestBase implements IFcaService {
  private _fcaDefinedAuthStatusesMapping = [] as FcaAuthStatus[];

  public async initializeFcaDefinedPermissionsRemoteDataAsync(): Promise<string> {
    if (!USE_REMOTE_DB) {
      return "App is not connected to remote DB";
    }

    try {
      const response = await this.postRemoteAsync<string>(
        `${REMOTE_API}/InitializeFcaPermissionsAsync`,
        "",
      );
      return response?.toString();
    } catch {
      return "";
    }
  }

  public async initializeFcaStatusesRemoteDataAsync(): Promise<string> {
    if (!USE_REMOTE_DB) {
      return "App is not connected to remote DB";
    }

    try {
      const response = await this.postRemoteAsync<string>(
        `${REMOTE_API}/InitializeFcaStatusesAsync`,
        "",
      );
      return response?.toString();
    } catch {
      return "";
    }
  }

  public async getMatchedFirms(
    companyName: string,
    isSoleTrader: boolean,
    companyNumber: string = "",
    companyAddress = "",
    fromCache: boolean = false,
  ): Promise<CompanyEntity[] | undefined> {
    if (!USE_REMOTE_DB) {
      throw new Error(
        "Getting FCA firm matches will not work if run locally. Please connect to outside services.",
      );
    }

    const cacheKey = isSoleTrader
      ? AppConstants.fcaSoleTraderFirmsCacheKey
      : AppConstants.fcaFirmsCacheKey;

    if (fromCache) {
      const cacheFound = localStorage.getItem(cacheKey);

      if (cacheFound) {
        return JSON.parse(cacheFound) as CompanyEntity[];
      }
    }

    const endpoint = `${REMOTE_API}/GetMatchedFirmsAsync?q=${companyName}&isFirm=${!isSoleTrader}&companyNo=${companyNumber}&companyAddress=${companyAddress}`;
    const companies = await this.getRemoteAsync<CompanyEntity[]>(endpoint);

    if (!companies) {
      return undefined;
    }

    // get fca auth status
    if (
      !this._fcaDefinedAuthStatusesMapping ||
      this._fcaDefinedAuthStatusesMapping.length < 1
    ) {
      this._fcaDefinedAuthStatusesMapping =
        await this.getFcaDefinedStatusesAsync();
    }

    let isExistsInCompanyHouseOnly = false;

    if (!isSoleTrader && companies.length < 1) {
      const companyHouseFromLocal = localStorage.getItem(
        AppConstants.autoCompleteSelectedCompany,
      );

      if (companyHouseFromLocal) {
        const companyFromHouse = JSON.parse(
          companyHouseFromLocal,
        ) as CompanyEntity;
        companyFromHouse.isSoleTrader = isSoleTrader;
        companies.push(companyFromHouse);
        isExistsInCompanyHouseOnly = true;
      }
    }

    for (const company of companies) {
      if (!isExistsInCompanyHouseOnly) {
        company.isAuthorized = this.isFirmAuthorised(company.status);
      }

      if (isExistsInCompanyHouseOnly) {
        company.isAuthorized = false;
        company.status = AppConstants.notAuthorised;
      }

      company.isSoleTrader = isSoleTrader;
    }

    if (companies.length > 0) {
      localStorage.setItem(cacheKey, JSON.stringify(companies));
    }

    return companies;
  }

  public async getFirmPermissionsAsync(
    firmRefNo: string,
  ): Promise<PermissionResult> {
    if (!USE_REMOTE_DB) {
      throw new Error(
        "Getting FCA firm matches will not work if run locally. Please connect to outside services.",
      );
    }

    return await this.getRemoteAsync<PermissionResult>(
      `${REMOTE_API}/GetFirmPermissionsAsync/${firmRefNo}`,
    );
  }

  public async getDefinedPermissionsAsync(): Promise<PermissionEdit[]> {
    if (!USE_REMOTE_DB) {
      return await this.getRemoteAsync<PermissionEdit[]>(
        "/api/list.permissionGroups.json",
      );
    }

    const url = `${REMOTE_API}/GetAllPermissionsAsync`;
    // use 'getRemoteAsync' since it will use the header for Azure Function and not for FCA endpoint
    return await this.getRemoteAsync<PermissionEdit[]>(url);
  }

  public async savePermissionsAsync(
    permissionEditModels: PermissionEdit[],
  ): Promise<PermissionEdit[]> {
    return await this.postRemoteAsync<PermissionEdit[]>(
      `${REMOTE_API}/SavePermissionsMultipleAsync`,
      JSON.stringify(permissionEditModels),
    );
  }

  public async getFcaDefinedStatusesAsync(): Promise<FcaAuthStatus[]> {
    if (!USE_REMOTE_DB) {
      return await this.getRemoteAsync<FcaAuthStatus[]>(
        "/api/list.fcaStatus.json",
      );
    }

    return await this.getRemoteAsync<FcaAuthStatus[]>(
      `${REMOTE_API}/GetAllFcaStatusesAsync`,
    );
  }

  public async saveFcaStatusesAsync(
    fcaAuthStatusModels: FcaAuthStatus[],
  ): Promise<FcaAuthStatus[]> {
    return await this.postRemoteAsync<FcaAuthStatus[]>(
      `${REMOTE_API}/SaveFcaStatusMultipleAsync`,
      JSON.stringify(fcaAuthStatusModels),
    );
  }

  public isFirmAuthorised(actualStatus: string): boolean {
    const found = this._fcaDefinedAuthStatusesMapping.find(
      (i) => i.actualStatus?.toLowerCase() === actualStatus.toLowerCase(),
    );

    if (!found) {
      return false;
    }

    return found.generalStatus === "Authorised";
  }

  public async getFirmTradingNamesAsync(
    firmReferenceNumber: string,
  ): Promise<string[]> {
    return await this.getRemoteAsync<string[]>(
      `${REMOTE_API}/GetFirmNameDetailsAsync/${firmReferenceNumber}`,
    );
  }

  public async getFirmAddressesDetailsAsync(
    firmReferenceNumber: string,
    type: string,
  ): Promise<FcaAddressDetail[]> {
    let endpoint = `${REMOTE_API}/GetFirmAddressDetailsAsync/${firmReferenceNumber}`;

    if (type) {
      endpoint += `?type=${type}`;
    }

    return await this.getRemoteAsync<FcaAddressDetail[]>(endpoint);
  }

  public async extractTradingAddressDetailsAsync(
    firmReference: string,
  ): Promise<TradingAddress> {
    const fcaAddress = await this.getFirmAddressesDetailsAsync(
      firmReference,
      "PPOB",
    );

    if (!fcaAddress || fcaAddress.length < 1) {
      return {
        address: "",
        phoneNumber: "",
        website: "",
      };
    }

    const firstAddress = fcaAddress[0];
    const jsonStr = JSON.parse(JSON.stringify(firstAddress));
    let address = "";

    if (jsonStr["Address Line 1"]) {
      address += jsonStr["Address Line 1"];
    }

    if (jsonStr["Address Line 2"]) {
      address += `, ${jsonStr["Address Line 2"]}`;
    }

    if (firstAddress?.town) {
      address += `, ${firstAddress.town}`;
    }

    if (firstAddress?.country) {
      address += `, ${firstAddress.country}`;
    }

    if (firstAddress?.postcode) {
      address += `, ${firstAddress.postcode}`;
    }

    return {
      address: address,
      phoneNumber: jsonStr["Phone Number"] ?? "",
      website: jsonStr["Website Address"] ?? "",
    } as TradingAddress;
  }

  public async getFirmIndividualsAsync(
    firmReferenceNumber: string,
  ): Promise<FcaIndividual[]> {
    return await this.getRemoteAsync<FcaIndividual[]>(
      `${REMOTE_API}/GetFirmIndividualsAsync/?firmRefNo=${firmReferenceNumber}`,
    );
  }

  public async getProvidersAsync(
    firmReferenceNumber: string,
  ): Promise<AppointedRepresentative[]> {
    return await this.getRemoteAsync<AppointedRepresentative[]>(
      `${REMOTE_API}/GetProvidersAsync/?firmRefNo=${firmReferenceNumber}`,
    );
  }

  public async getAppointedRepresentativesAsync(
    firmReferenceNumber: string,
  ): Promise<AppointedRepresentative[]> {
    return await this.getRemoteAsync<AppointedRepresentative[]>(
      `${REMOTE_API}/GetAppointedRepresentativesAsync/?firmRefNo=${firmReferenceNumber}`,
    );
  }

  public async searchFirmsByFirmNameKeywordAsync(
    keyword: string,
  ): Promise<CompanyEntity[]> {
    return await this.getRemoteAsync<CompanyEntity[]>(
      `${REMOTE_API}/SearchFirmsByFirmNameKeywordAsync/?keyword=${keyword}`,
    );
  }

  public async getFirmPraStatusAsync(
    firmReferenceNumber: string,
  ): Promise<boolean> {
    const response = await this.getRemoteAsync<string>(
      `${REMOTE_API}/GetFirmPraStatusAsync/?firmRefNo=${firmReferenceNumber}`,
    );

    return response === "Authorised";
  }

  public async getFcaFirmDetailByRefNoAsync(
    firmReferenceNumber: string,
  ): Promise<FcaFirmDetail> {
    const endPoint = `${REMOTE_API}/GetFcaFirmDetailsAsync/${firmReferenceNumber}`;
    return await this.getRemoteAsync<FcaFirmDetail>(endPoint);
  }

  public async getCurrentFirmFcaPermissionsAsync(
    firmReferenceNumber: string | undefined,
  ): Promise<CustomerPermission[]> {
    if (!firmReferenceNumber) {
      return [] as CustomerPermission[];
    }

    const definedGroupedPermissions = await this.getDefinedPermissionsAsync();
    const subPermissionsFromFca = await this.getFirmPermissionsAsync(
      firmReferenceNumber,
    ).catch(() => {
      return {
        permissionNames: [] as string[],
        raw: "",
      };
    });

    return definedGroupedPermissions.map((permission) => {
      const state = this.getPermissionState(permission, subPermissionsFromFca);
      return <CustomerPermission>{
        id: permission.id,
        permissionGroupName: permission.permissionGroupName,
        categoryName: permission.categoryName,
        subPermissionName: permission.subPermissionName,
        subPermissionDisplayText: permission.subPermissionDisplayText,
        state: state,
        hasPendingApplication: state === PermissionStateEnum.Added,
        isModified: false,
      };
    });
  }

  private getPermissionState(
    permission: PermissionEdit,
    permissionResult: PermissionResult,
  ): PermissionStateEnum {
    const { subPermissionName = "", permissionGroupName = "" } = permission;
    const { permissionNames = [] } = permissionResult;
    const isFound = permissionNames.find(
      (name) =>
        name.toLowerCase() ===
        (subPermissionName || permissionGroupName).toLowerCase(),
    );
    return isFound ? PermissionStateEnum.Added : PermissionStateEnum.Removed;
  }
}
