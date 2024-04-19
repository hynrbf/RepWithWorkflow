import { defineStore, storeToRefs } from "pinia";
import { container } from "tsyringe";
import { useCustomerStore } from "@/stores/useCustomerStore";
import { AppointedRepresentative } from "@/entities/appointed-representatives/AppointedRepresentative";
import {
  IAppointedRepresentativeService,
  IAppointedRepresentativeServiceInfo,
} from "@/infra/dependency-services/rest/appointed-representative/IAppointedRepresentativeService";

export interface State {
  appointedRepresentatives: AppointedRepresentative[];
  appointedRepresentative: AppointedRepresentative | undefined;
  appointedRepresentativesTotal: number;
}

const appointedRepresentativeService =
  container.resolve<IAppointedRepresentativeService>(
    IAppointedRepresentativeServiceInfo.name,
  );

export const useAppointedRepresentativesStore = defineStore(
  "appointedRepresentativesStore",
  {
    state: (): State => ({
      appointedRepresentatives: [],
      appointedRepresentative: undefined,
      appointedRepresentativesTotal: 0,
    }),
    actions: {
      setAppointedRepresentative(payload: AppointedRepresentative) {
        this.appointedRepresentative = payload;
      },
      getAppointedRepresentative(
        id: string,
      ): AppointedRepresentative | undefined {
        return this.appointedRepresentatives.find((item) => item.id === id);
      },
      appendAppointedRepresentative(item: AppointedRepresentative) {
        this.appointedRepresentatives = [
          ...this.appointedRepresentatives,
          item,
        ];
        return item;
      },
      amendAppointedRepresentative(
        id: string,
        payload: Partial<AppointedRepresentative>,
      ) {
        let amendedItem = undefined;
        this.appointedRepresentatives = this.appointedRepresentatives.map(
          (item) => {
            if (item.id === id) {
              amendedItem = {
                ...item,
                ...payload,
              };
              return amendedItem;
            }
            return item;
          },
        );
        return amendedItem;
      },
      removeAppointedRepresentative(id: string) {
        this.appointedRepresentatives = this.appointedRepresentatives.filter(
          (item) => item.id !== id,
        );
      },
      async fetchAppointedRepresentativesAsync(
        params?: Record<string, unknown>,
      ) {
        const customerStore = useCustomerStore();
        const { currentCustomer } = storeToRefs(customerStore);
        try {
          const response =
            await appointedRepresentativeService.getAppointedRepresentativesAsync(
              currentCustomer.value.id,
              params,
            );

          this.appointedRepresentativesTotal = response.total;
          this.appointedRepresentatives = response.data;
          return Promise.resolve(response.data);
        } catch (error) {
          return Promise.reject(error);
        }
      },
      async fetchAppointedRepresentativeAsync(id: string, cache = true) {
        try {
          const response =
            (await appointedRepresentativeService.getAppointedRepresentativeAsync(
              id,
            )) as AppointedRepresentative;
          if (cache) {
            this.setAppointedRepresentative(response);
          }
          return Promise.resolve(this.appointedRepresentative);
        } catch (error) {
          return Promise.reject(error);
        }
      },
      async saveOrUpdateAppointedRepresentativeAsync(
        payload: Partial<AppointedRepresentative>,
        cache = true,
      ): Promise<AppointedRepresentative> {
        try {
          const customerStore = useCustomerStore();
          const { currentCustomer } = storeToRefs(customerStore);
          const response =
            await appointedRepresentativeService.saveOrUpdateAppointedRepresentativeAsync(
              { ...payload, customerId: currentCustomer.value.id },
            );
          if (cache) {
            let isUpdate = false;
            this.appointedRepresentatives = this.appointedRepresentatives.map(
              (item) => {
                if (item.id === payload.id) {
                  isUpdate = true;
                  return {
                    ...item,
                    ...response,
                  };
                }
                return item;
              },
            );
            if (!isUpdate) {
              this.appointedRepresentatives = [
                ...this.appointedRepresentatives,
                response,
              ];
            }
            this.setAppointedRepresentative(response);
          }
          return Promise.resolve(response);
        } catch (error) {
          return Promise.reject(error);
        }
      },
    },
  },
);
