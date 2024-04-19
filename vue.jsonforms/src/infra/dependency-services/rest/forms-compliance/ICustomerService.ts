import { CustomerEntity } from "@/entities/CustomerEntity";
import { CustomerProduct } from "@/entities/CustomerProduct";
import { FirmPermission } from "@/entities/FirmPermission";

export declare interface ICustomerService {
  saveCustomerAsync(customerModelJson: string): Promise<CustomerEntity>;

  getCustomerByEmailAsync(email: string): Promise<CustomerEntity>;

  checkAndUpdateScheduleMeetingWithCustomerEmail(
    calendlyEmail: string,
    customerEmail: string,
    eventTypeId: string,
    startDateEpoch: number,
  ): Promise<boolean>;

  checkIfCustomerHasLoggedOnAlready(): boolean;

  checkIfCompanyHasExistingSignedProposal(
    companyNumber: string,
  ): Promise<boolean>;

  getCustomerProducts(email: string | undefined): Promise<CustomerProduct[]>;

  getCompanyPermissionsAsync(firmReferenceNumber: string): Promise<FirmPermission[]>;
}

export const ICustomerServiceInfo = {
  name: "ICustomerService",
};
