import { AuthUser } from "@/entities/AuthUser";
import { CustomerEntity } from "@/entities/CustomerEntity";
import { AppointedRepresentative } from "@/entities/appointed-representatives/AppointedRepresentative";
import {Employee} from "@/entities/firm-details/Employee";

export declare interface IAppService {
  checkCustomerSignedUpAlreadyAsync(): Promise<boolean>;

  getComplianceFirmNameAsync(): Promise<string>;

  saveAuthUserToLocal(authUser: AuthUser): void;

  getAuthUserFromLocal(): AuthUser | undefined;

  getCachedCustomer(): CustomerEntity | undefined;

  getCustomerFirmName(): string;

  clearAllLocalCache(): void;

  clearThankYouPageCache(): void;

  clearAllLocalCacheWithToken(): void;

  checkCustomerAppointedRepresentativeSignedUpAlreadyAsync(): Promise<boolean>;

  checkEmployeeSignedUpAlreadyAsync(): Promise<boolean>;

  checkProviderSignedUpAlreadyAsync(): Promise<boolean>;

  checkIntroducerSignedUpAlreadyAsync(): Promise<boolean>;

  getCachedCustomerAppointedRepresentative():
    | AppointedRepresentative
    | undefined;

  getCustomerArFirmName(): string;

  getCachedEmployee(): Employee | undefined;

  getEmployeeFirmName(): string;
}

export const IAppServiceInfo = {
  name: "IAppService",
};
