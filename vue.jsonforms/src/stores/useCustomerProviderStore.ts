import {defineStore} from "pinia";
import {AppConstants} from "@/infra/AppConstants";
import {ProvidersEntity} from "@/entities/providers-and-introducers/ProvidersEntity";
import {container} from "tsyringe";
import {
  IProvidersService,
  IProvidersServiceInfo,
} from "@/infra/dependency-services/rest/providers/IProvidersService";

export const useCustomerProviderStore = defineStore(
    AppConstants.customerProviderStore,
    {
        state: () => ({
            providers: [] as ProvidersEntity[]
        }),
        actions: {
            async fetchInitialCustomerProvidersAsync(customerId: string): Promise<ProvidersEntity[]> {
                if (!customerId || customerId.length < 1) {
                    return [];
                }

                const providerService =
                    container.resolve<IProvidersService>(
                        IProvidersServiceInfo.name,
                    );

                const response =
                    await providerService.getProvidersByCustomerIdAsync(customerId);

                if (!response) {
                    return [];
                }

                this.providers = response;
                return response;
            },
        },
        persist: true
    }
);