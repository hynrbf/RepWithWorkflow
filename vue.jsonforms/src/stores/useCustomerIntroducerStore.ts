import { defineStore } from "pinia";
import { AppConstants } from "@/infra/AppConstants";
import { IntroducersEntity } from "@/entities/providers-and-introducers/IntroducersEntity";
import { container } from "tsyringe";
import {
  IIntroducersService,
  IIntroducersServiceInfo,
} from "@/infra/dependency-services/rest/introducers/IIntroducersService";

export const useCustomerIntroducerStore = defineStore(
  AppConstants.customerIntroducerStore,
  {
    state: () => ({
      introducers: [] as IntroducersEntity[],
    }),
    actions: {
      async fetchInitialCustomerIntroducersAsync(
        customerId: string,
      ): Promise<IntroducersEntity[]> {
        if (!customerId || customerId.length < 1) {
          return [];
        }

        const introducerService = container.resolve<IIntroducersService>(
          IIntroducersServiceInfo.name,
        );

        const response =
          await introducerService.getIntroducersByCustomerIdAsync(customerId);

        if (!response) {
          return [];
        }

        this.introducers = response;
        return response;
      },
    },
    persist: true,
  },
);
