import { CompanyEntity } from "@/entities/CompanyEntity";
import { FcaAuthStatus } from "@/entities/FcaAuthStatus";
import { PermissionEdit } from "@/entities/PermissionEdit";
import { PermissionResult } from "@/entities/PermissionResult";
import { FcaAddressDetail } from "@/entities/FcaAddressDetail";
import { FcaIndividual } from "@/entities/FcaIndividual";
import { AppointedRepresentative } from "@/entities/appointed-representatives/AppointedRepresentative";
import { FcaFirmDetail } from "@/entities/firm-details/FcaFirmDetail";
import { CustomerPermission } from "@/entities/CustomerPermission";

export declare interface IFcaService {
  initializeFcaDefinedPermissionsRemoteDataAsync(): Promise<string>;

  initializeFcaStatusesRemoteDataAsync(): Promise<string>;

  getMatchedFirms(
    companyName: string,
    isSoleTrader: boolean,
    companyNumber: string,
    companyAddress: string,
    fromCache: boolean,
  ): Promise<CompanyEntity[] | undefined>;

  getFirmPermissionsAsync(firmRefNo: string): Promise<PermissionResult>;

  getDefinedPermissionsAsync(): Promise<PermissionEdit[]>;

  savePermissionsAsync(
    permissionEditModels: PermissionEdit[],
  ): Promise<PermissionEdit[]>;

  getFcaDefinedStatusesAsync(): Promise<FcaAuthStatus[]>;

  saveFcaStatusesAsync(
    fcaAuthStatusModels: FcaAuthStatus[],
  ): Promise<FcaAuthStatus[]>;

  isFirmAuthorised(status: string): boolean;

  getFirmTradingNamesAsync(firmReferenceNumber: string): Promise<string[]>;

  getFirmAddressesDetailsAsync(
    firmReferenceNumber: string,
    type?: string,
  ): Promise<FcaAddressDetail[]>;

  extractTradingAddressDetailsAsync(firmReference: string): Promise<{
    address: string;
    website: string;
    phoneNumber: string;
  }>;

  getFirmIndividualsAsync(
    firmReferenceNumber: string,
  ): Promise<FcaIndividual[]>;

  getProvidersAsync(
    firmReferenceNumber: string,
  ): Promise<AppointedRepresentative[]>;

  getAppointedRepresentativesAsync(
    firmReferenceNumber: string,
  ): Promise<AppointedRepresentative[]>;

  searchFirmsByFirmNameKeywordAsync(keyword: string): Promise<CompanyEntity[]>;

  getFirmPraStatusAsync(firmReferenceNumber: string): Promise<boolean>;

  getFcaFirmDetailByRefNoAsync(
    firmReferenceNumber: string,
  ): Promise<FcaFirmDetail>;

  getCurrentFirmFcaPermissionsAsync(
    firmReferenceNumber: string | undefined,
  ): Promise<CustomerPermission[]>;
}

export const IFcaServiceInfo = {
    name: "IFcaService",
};
