import { defineStore } from "pinia";
import { container } from "tsyringe";
import {
  IClientMoneyService,
  IClientMoneyServiceInfo,
} from "@/infra/dependency-services/rest/client-money/IClientMoneyService";
import { InsurerRating } from "@/entities/client-money/InsurerRating";

export interface State {
  insurerRatings: InsurerRating | undefined;
}

const clientMoneyService = container.resolve<IClientMoneyService>(
  IClientMoneyServiceInfo.name,
);

export const useClientMoneyStore = defineStore("clientMoneyStore", {
  state: (): State => ({
    insurerRatings: undefined,
  }),
  actions: {
    async fetchInsurerRatingsAsync(company: string) {
      try {
        const response =
          await clientMoneyService.getInsurerRatingsAsync(company);
        this.insurerRatings = response;
        return Promise.resolve(response);
      } catch (error) {
        return Promise.reject(error);
      }
    },
  },
});
