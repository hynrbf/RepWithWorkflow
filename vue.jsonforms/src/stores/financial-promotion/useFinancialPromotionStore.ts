import { defineStore, storeToRefs } from "pinia";
import { useCustomerStore } from "@/stores/useCustomerStore";
import { FinancialPromotion } from "@/entities/financial-promotion/FinancialPromotion";
import { container } from "tsyringe";
import {
  IFinancialPromotionService,
  IFinancialPromotionServiceInfo,
} from "@/infra/dependency-services/rest/financial-promotion/IFinancialPromotionService";

export interface State {
  financialPromotion: FinancialPromotion | undefined;
  financialPromotions: FinancialPromotion[];
  financialPromotionTotal: number;
}

const financialPromotionService = container.resolve<IFinancialPromotionService>(
  IFinancialPromotionServiceInfo.name
);

export const useFinancialPromotionStore = defineStore(
  "financialPromotionStore",
  {
    state: (): State => ({
      financialPromotion: undefined,
      financialPromotions: [],
      financialPromotionTotal: 0,
    }),
    actions: {
      setFinancialPromotion(payload?: FinancialPromotion) {
        this.financialPromotion = payload;
      },
      getFinancialPromotion(id: string, cache = true) {
        const financialPromotion = this.financialPromotions.find(
          (item) => item.id === id
        );
        if (cache) {
          this.financialPromotion = financialPromotion;
        }
        return financialPromotion;
      },
      appendFinancialPromotion(item: FinancialPromotion) {
        this.financialPromotions = [...this.financialPromotions, item];
        return item;
      },
      amendFinancialPromotion(
        id: string,
        payload: Partial<FinancialPromotion>,
        cache = true
      ) {
        let amendedItem: FinancialPromotion | undefined;
        this.financialPromotions = this.financialPromotions.map((item) => {
          if (item.id === id) {
            amendedItem = {
              ...item,
              ...payload,
            };
            return amendedItem;
          }
          return item;
        });
        if (cache) {
          this.financialPromotion = amendedItem;
        }
        return amendedItem;
      },
      removeFinancialPromotion(id: string) {
        this.financialPromotions = this.financialPromotions.filter(
          (item) => item.id !== id
        );
      },
      async fetchFinancialPromotionsAsync(params?: Record<string, unknown>) {
        const customerStore = useCustomerStore();
        const { currentCustomer } = storeToRefs(customerStore);
        try {
          const response =
            await financialPromotionService.getFinancialPromotionsAsync(
              currentCustomer.value.id,
              params
            );

          const { data, total }: { data: FinancialPromotion[]; total: number } =
            response;

          this.financialPromotions = data;
          this.financialPromotionTotal = total;
        } catch (error) {
          return Promise.reject(error);
        }
      },
      async fetchFinancialPromotionAsync(
        id: string,
        cache = true
      ): Promise<FinancialPromotion | undefined> {
        try {
          const response =
            await financialPromotionService.getFinancialPromotionAsync(id);
          if (cache) {
            this.setFinancialPromotion(response);
          }
          return Promise.resolve(response);
        } catch (error) {
          return Promise.reject(error);
        }
      },
      async saveOrUpdateFinancialPromotionAsync(
        payload: Partial<FinancialPromotion>,
        cache = true
      ): Promise<FinancialPromotion> {
        try {
          const customerStore = useCustomerStore();
          const { currentCustomer } = storeToRefs(customerStore);
          const response =
            await financialPromotionService.saveOrUpdateFinancialPromotionAsync(
              { ...payload, customerId: currentCustomer.value.id }
            );
          if (cache) {
            let isUpdate = false;
            this.financialPromotions = this.financialPromotions.map((item) => {
              if (item.id === payload.id) {
                isUpdate = true;
                return {
                  ...item,
                  ...response,
                };
              }
              return item;
            });
            if (!isUpdate) {
              this.financialPromotions = [
                response,
                ...this.financialPromotions,
              ];
            }
            this.setFinancialPromotion(response);
          }
          return Promise.resolve(response);
        } catch (error) {
          return Promise.reject(error);
        }
      },
      async deleteFinancialPromotionAsync(
        id: string,
        cache = true
      ): Promise<boolean> {
        try {
          await financialPromotionService.deleteFinancialPromotionAsync(id);
          if (cache) {
            this.removeFinancialPromotion(id);
          }
          return Promise.resolve(true);
        } catch (error) {
          return Promise.reject(error);
        }
      },
    },
  }
);
