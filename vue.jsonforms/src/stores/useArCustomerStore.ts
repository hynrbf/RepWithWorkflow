import { defineStore } from "pinia";
import { useStorage, RemovableRef } from "@vueuse/core";
import { AppConstants } from "@/infra/AppConstants";
import { AppointedRepresentative } from "@/entities/appointed-representatives/AppointedRepresentative";
import { container } from "tsyringe";
import {
  IAppointedRepresentativeService,
  IAppointedRepresentativeServiceInfo,
} from "@/infra/dependency-services/rest/appointed-representative/IAppointedRepresentativeService";

export interface State {
  arCustomers: RemovableRef<AppointedRepresentative[]>;
}

export const useArCustomerStore = defineStore("arCustomerStore", {
  state: (): State => ({
    arCustomers: useStorage<AppointedRepresentative[]>(
      AppConstants.customerAppointedRepresentativeKey,
      []
    ),
  }),
  getters: {
    currentArCustomer({ arCustomers }): AppointedRepresentative {
      return arCustomers[0];
    },
    currentArCustomerName(): string {
      if (!this.currentArCustomer) {
        return "";
      }

      const { firstName, lastName } = this.currentArCustomer;
      return `${firstName} ${lastName}`;
    },
    currentArFirmName(): string {
      if (!this.currentArCustomer) {
        return "";
      }

      const { companyName, name, isCompanyNotApplicable } =
        this.currentArCustomer;

      if (isCompanyNotApplicable) {
        return this.currentArCustomerName;
      }

      return companyName ?? name ?? "<Firm Name>";
    },
  },
  actions: {
    async fetchArCustomerByEmailAsync(email: string) {
      try {
        const customerAppointedRepresentativeService =
          container.resolve<IAppointedRepresentativeService>(
            IAppointedRepresentativeServiceInfo.name
          );
        const response =
          await customerAppointedRepresentativeService.getAppointedRepresentativesByEmailAsync(
            email
          );

        this.arCustomers = this.arCustomers.map((customer) => {
          if (customer.id === response.id) {
            return {
              ...customer,
              ...response,
            };
          }
          return customer;
        }) as AppointedRepresentative[];

        return Promise.resolve(response);
      } catch (error) {
        return Promise.reject(error);
      }
    },

    async updateCustomerArByEmailAsync(
      email: string,
      payload: Partial<AppointedRepresentative>
    ): Promise<AppointedRepresentative> {
      try {
        const customerAppointedRepresentativeService =
          container.resolve<IAppointedRepresentativeService>(
            IAppointedRepresentativeServiceInfo.name
          );
        const customer =
          await customerAppointedRepresentativeService.getAppointedRepresentativesByEmailAsync(
            email
          );
        const updatedCustomer: AppointedRepresentative = {
          ...customer,
          ...payload,
        };
        const response =
          await customerAppointedRepresentativeService.saveOrUpdateAppointedRepresentativeAsync(
            updatedCustomer
          );

        this.arCustomers = this.arCustomers.map((arCustomer) => {
          if (arCustomer.id === response.id) {
            return {
              ...arCustomer,
              ...response,
            };
          }
          return arCustomer;
        }) as AppointedRepresentative[];

        return Promise.resolve(response);
      } catch (error) {
        return Promise.reject(error);
      }
    },
  },
});
