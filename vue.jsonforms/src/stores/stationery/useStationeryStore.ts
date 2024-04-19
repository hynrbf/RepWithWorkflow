import { defineStore, storeToRefs } from "pinia";
import isEmpty from "lodash/isEmpty";
import { StationeryEntity } from "@/entities/stationery/StationeryEntity";
import { useCustomerStore } from "@/stores/useCustomerStore";
import StaticList from "@/infra/StaticListService";

export interface State {
  stationery: StationeryEntity | undefined;
  stationeries: StationeryEntity[];
}

export const useStationeryStore = defineStore("stationeryStore", {
  state: (): State => ({
    stationery: undefined,
    stationeries: [],
  }),
  getters: {},
  actions: {
    setStationery(payload: State["stationery"]) {
      this.stationery = payload;
    },
    getStationery(id: string, cache = true) {
      const stationery = this.stationeries.find((item) => item.id === id);
      if (cache) {
        this.stationery = stationery;
      }
      return stationery;
    },
    appendStationery(item: StationeryEntity) {
      this.stationeries = [...this.stationeries, item];
      return item;
    },
    async fetchStationeriesAsync() {
      const customerStore = useCustomerStore();
      const { fetchCustomerByEmailAsync } = customerStore;
      const { currentCustomer } = storeToRefs(customerStore);

      try {
        await fetchCustomerByEmailAsync(currentCustomer.value.email as string);

        const { stationeries }: { stationeries: StationeryEntity[] } =
          currentCustomer.value;

        this.stationeries = isEmpty(stationeries)
          ? StaticList.getDefaultStationeries()
          : stationeries;
      } catch (error) {
        return Promise.reject(error);
      }
    },
    async createStationeryAsync(payload: StationeryEntity, cache = true) {
      const customerStore = useCustomerStore();
      const { updateCustomerByEmailAsync } = customerStore;
      const { currentCustomer } = storeToRefs(customerStore);

      try {
        const email = currentCustomer.value.email;
        let stationeries: StationeryEntity[] =
          currentCustomer.value.stationeries || [];

        if (!email) {
          throw new Error("Email not defined.");
        }

        const newItem = {
          ...payload,
        } as StationeryEntity;

        stationeries = isEmpty(stationeries)
          ? [...this.stationeries, newItem]
          : ([...stationeries, newItem] as StationeryEntity[]);

        await updateCustomerByEmailAsync(email, {
          stationeries,
        });

        if (cache) {
          this.appendStationery(newItem);
        }
      } catch (error) {
        return Promise.reject(error);
      }
    },
    async updateStationeryAsync(
      id: string,
      payload: StationeryEntity,
      cache = true
    ) {
      const customerStore = useCustomerStore();
      const { updateCustomerByEmailAsync } = customerStore;
      const { currentCustomer } = storeToRefs(customerStore);

      try {
        const email = currentCustomer.value.email;
        const stationeries: StationeryEntity[] =
          currentCustomer.value.stationeries || [];

        if (!email) {
          throw new Error("Email not defined.");
        }

        const index = stationeries.findIndex(
          (stationery) => stationery.id === id
        );

        if (index !== -1) {
          stationeries.splice(index, 1, payload);
        }

        await updateCustomerByEmailAsync(email, {
          stationeries,
        });

        if (cache) {
          this.stationeries = stationeries;
        }
      } catch (error) {
        return Promise.reject(error);
      }
    },
  },
});
