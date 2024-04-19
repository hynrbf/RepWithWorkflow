import { defineStore } from "pinia";
import { AppConstants } from "@/infra/AppConstants";
import { container } from "tsyringe";
import { CustomerEntity } from "@/entities/CustomerEntity";
import {
  ICustomerService,
  ICustomerServiceInfo,
} from "@/infra/dependency-services/rest/forms-compliance/ICustomerService";
import {
  IAppService,
  IAppServiceInfo,
} from "@/infra/dependency-services/app/IAppService";

export const useSafeCustomerStore = defineStore(
  AppConstants.safeCustomerStore,
  {
    state: () => ({
      currentCustomer: new CustomerEntity(),
    }),
    getters: {
      getCurrentCustomer(): CustomerEntity {
        return this.currentCustomer;
      },
      getCurrentCustomerName(): string {
        const { firstName, lastName } = this.currentCustomer;
        return `${firstName} ${lastName}`;
      },
      getCurrentFirmName(): string {
        const { companyName, isCompanyNotApplicable } = this.currentCustomer;

        if (isCompanyNotApplicable) {
          return this.getCurrentCustomerName;
        }

        if (companyName) {
          return companyName;
        }

        return "<Firm Name>";
      },
    },
    actions: {
      setCustomerLocally(): void {
        const appService = container.resolve<IAppService>(IAppServiceInfo.name);
        this.currentCustomer =
          appService.getCachedCustomer() ?? new CustomerEntity();
      },

      async updateCustomerByEmailAsync(
        email: string,
        payload: Partial<CustomerEntity>,
      ): Promise<void> {
        const customerService = container.resolve<ICustomerService>(
          ICustomerServiceInfo.name,
        );
        const customer = await customerService.getCustomerByEmailAsync(email);
        const updatedCustomer = {
          ...customer,
          ...payload,
        };

        //once saveCustomerAsync is called, the getCachedCustomer is updated inside it
        const response = await customerService.saveCustomerAsync(
          JSON.stringify(updatedCustomer),
        );

        if (!response) {
          return;
        }

        this.currentCustomer = response;
      },
    },
    persist: true,
  },
);