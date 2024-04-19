import { defineStore } from "pinia";
import { useStorage, RemovableRef } from "@vueuse/core";
import { AppConstants } from "@/infra/AppConstants";
import { CustomerEntity } from "@/entities/CustomerEntity";
import { container } from "tsyringe";
import {
  ICustomerService,
  ICustomerServiceInfo,
} from "@/infra/dependency-services/rest/forms-compliance/ICustomerService";

export interface State {
  customers: RemovableRef<CustomerEntity[]>;
}

export const useCustomerStore = defineStore("customerStore", {
  state: (): State => ({
    customers: useStorage<CustomerEntity[]>(
      AppConstants.jsonFormsCustomersKey,
      [],
    ),
  }),
  getters: {
    currentCustomer({ customers }): CustomerEntity {
      return customers[0];
    },
    currentCustomerName(): string {
      if (!this.currentCustomer) {
        return "";
      }

      const { firstName, lastName } = this.currentCustomer;
      return `${firstName} ${lastName}`;
    },
    currentFirmName(): string {
      if (!this.currentCustomer) {
        return "";
      }

      const { companyName, isCompanyNotApplicable } = this.currentCustomer;

      if (isCompanyNotApplicable) {
        return this.currentCustomerName;
      }

      if (companyName) {
        return companyName;
      }

      return "<Firm Name>";
    },
    currentCompanyNumber(): string {
      const { companyNumber = "" } = this.currentCustomer;
      return companyNumber;
    },
  },
  actions: {
    async fetchCustomerByEmailAsync(email: string) {
      try {
        const customerService = container.resolve<ICustomerService>(
          ICustomerServiceInfo.name,
        );
        const response = await customerService.getCustomerByEmailAsync(email);

        this.customers = this.customers.map((customer) => {
          if (customer.id === response.id) {
            return {
              ...customer,
              ...response,
            };
          }
          return customer;
        }) as CustomerEntity[];

        return Promise.resolve(response);
      } catch (error) {
        return Promise.reject(error);
      }
    },

    async updateCustomerByEmailAsync(
      email: string,
      payload: Partial<CustomerEntity>,
    ): Promise<CustomerEntity> {
      try {
        const customerService = container.resolve<ICustomerService>(
          ICustomerServiceInfo.name,
        );
        const customer = await customerService.getCustomerByEmailAsync(email);
        const updatedCustomer = {
          ...customer,
          ...payload,
        };
        const response = await customerService.saveCustomerAsync(
          JSON.stringify(updatedCustomer),
        );

        this.customers = this.customers.map((customer) => {
          if (customer.id === response.id) {
            return {
              ...customer,
              ...response,
            };
          }
          return customer;
        }) as CustomerEntity[];

        return Promise.resolve(response);
      } catch (error) {
        return Promise.reject(error);
      }
    },
  },
});
