import { singleton } from "tsyringe";
import { REMOTE_API, USE_REMOTE_DB } from "@/config";
import { AppConstants } from "@/infra/AppConstants";
import RestBase from "../RestBase";
import { CustomerEntity } from "@/entities/CustomerEntity";
import { ICustomerService } from "./ICustomerService";
import { CustomerProduct } from "@/entities/CustomerProduct";
import { FirmPermission } from "@/entities/FirmPermission";
import { AuthUser } from "@/entities/AuthUser";

@singleton()
export default class CustomerService
  extends RestBase
  implements ICustomerService
{
  constructor() {
    super();
  }

  public async saveCustomerAsync(
    customerModelJson: string,
  ): Promise<CustomerEntity> {
    // TODO. need more testing for local saving
    if (!USE_REMOTE_DB) {
      const schemasFromLocalStoreString = localStorage.getItem(
        AppConstants.jsonFormsCustomersKey,
      );
      const customer = JSON.parse(customerModelJson) as CustomerEntity;

      if (schemasFromLocalStoreString) {
        const customersFromLocalStoreObj = JSON.parse(
          schemasFromLocalStoreString,
        ) as CustomerEntity[];
        const index = customersFromLocalStoreObj.findIndex(
          (s) => s.email === customer.email,
        );
        customersFromLocalStoreObj[index] = customer;
        localStorage.setItem(
          AppConstants.jsonFormsCustomersKey,
          JSON.stringify(customersFromLocalStoreObj),
        );
        return customersFromLocalStoreObj[index];
      } else {
        const customers = [] as CustomerEntity[];
        customers.push(customer);
        localStorage.setItem(
          AppConstants.jsonFormsCustomersKey,
          JSON.stringify(customers),
        );
        return customer;
      }
    }

    const customerToSave = JSON.parse(customerModelJson) as CustomerEntity;
    const user = localStorage.getItem(AppConstants.authUserKey);
    if (user) {
      customerToSave.changedBy = (JSON.parse(user) as AuthUser).email;
    }

    const customerModel = await this.putRemoteAsync<CustomerEntity>(
      `${REMOTE_API}/SaveCustomerAsync`,
      customerToSave,
    );
    const customers = [] as CustomerEntity[];
    customers.push(customerModel);
    localStorage.setItem(
      AppConstants.jsonFormsCustomersKey,
      JSON.stringify(customers),
    );
    return customerModel;
  }

  public async getCustomerByEmailAsync(email: string): Promise<CustomerEntity> {
    if (!USE_REMOTE_DB) {
      return null as unknown as CustomerEntity;
    }

    return await this.getRemoteAsync<CustomerEntity>(
      `${REMOTE_API}/GetCustomerByEmailAsync/${email}`,
    );
  }

  public checkIfCustomerHasLoggedOnAlready(): boolean {
    const hasLogOnAlready = localStorage.getItem(
      AppConstants.hasLogOnAlreadyKey,
    );

    if (hasLogOnAlready && hasLogOnAlready === "true") {
      return true;
    }

    localStorage.setItem(AppConstants.hasLogOnAlreadyKey, "true");
    return false;
  }

  public async checkAndUpdateScheduleMeetingWithCustomerEmail(
    calendlyEmail: string,
    customerEmail: string,
    eventTypeId: string,
    startDateEpoch: number,
  ): Promise<boolean> {
    return await this.getRemoteAsync<boolean>(
      `${REMOTE_API}/CheckAndUpdateMeetingRequestIfFoundAsync?calendlyEmail=${calendlyEmail}&customerEmail=${customerEmail}&eventTypeId=${eventTypeId}&startDateTime=${startDateEpoch}`,
    );
  }

  public async checkIfCompanyHasExistingSignedProposal(
    companyNumber: string,
  ): Promise<boolean> {
    return await this.getRemoteAsync<boolean>(
      `${REMOTE_API}/CheckIfCompanyHasProposalSignedAsync/${companyNumber}`,
    );
  }

  public async getCustomerProducts(
    email: string | undefined,
  ): Promise<CustomerProduct[]> {
    return await this.getRemoteAsync<CustomerProduct[]>(
      `${REMOTE_API}/GetCustomerProductsAsync?email=${email}`,
    );
  }

  public async getCompanyPermissionsAsync(
    firmReferenceNumber: string,
  ): Promise<FirmPermission[]> {
    return await this.getRemoteAsync<FirmPermission[]>(
      `${REMOTE_API}/GetCompanyPermissionsAsync/${firmReferenceNumber}`,
    );
  }
}
