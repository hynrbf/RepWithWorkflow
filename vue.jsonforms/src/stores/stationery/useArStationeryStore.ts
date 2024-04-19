import { defineStore, storeToRefs } from "pinia";
import isEmpty from "lodash/isEmpty";
import { StationeryEntity } from "@/entities/stationery/StationeryEntity";
import { useArCustomerStore } from "@/stores/useArCustomerStore";
import StaticList from "@/infra/StaticListService";

export interface State {
  stationery: StationeryEntity | undefined;
  stationeries: StationeryEntity[];
}

export const useArStationeryStore = defineStore("arStationeryStore", {
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
      const arCustomerStore = useArCustomerStore();
      const { fetchArCustomerByEmailAsync } = arCustomerStore;
      const { currentArCustomer } = storeToRefs(arCustomerStore);

      try {
        await fetchArCustomerByEmailAsync(
          currentArCustomer.value.email as string
        );

        const { stationeries }: { stationeries: StationeryEntity[] } =
          currentArCustomer.value;

        this.stationeries = isEmpty(stationeries)
          ? StaticList.getDefaultStationeries()
          : stationeries;
      } catch (error) {
        return Promise.reject(error);
      }
    },
    async createStationeryAsync(payload: StationeryEntity, cache = true) {
      const arCustomerStore = useArCustomerStore();
      const { updateCustomerArByEmailAsync } = arCustomerStore;
      const { currentArCustomer } = storeToRefs(arCustomerStore);

      try {
        const email = currentArCustomer.value.email;
        let stationeries: StationeryEntity[] =
          currentArCustomer.value.stationeries || [];

        if (!email) {
          throw new Error("Email not defined.");
        }

        const newItem = {
          ...payload,
        } as StationeryEntity;

        stationeries = isEmpty(stationeries)
          ? [...this.stationeries, newItem]
          : ([...stationeries, newItem] as StationeryEntity[]);

        await updateCustomerArByEmailAsync(email, {
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
      const arCustomerStore = useArCustomerStore();
      const { updateCustomerArByEmailAsync } = arCustomerStore;
      const { currentArCustomer } = storeToRefs(arCustomerStore);

      try {
        const email = currentArCustomer.value.email;
        const stationeries: StationeryEntity[] =
          currentArCustomer.value.stationeries || [];

        if (!email) {
          throw new Error("Email not defined.");
        }

        const index = stationeries.findIndex(
          (stationery) => stationery.id === id
        );

        if (index !== -1) {
          stationeries.splice(index, 1, payload);
        }

        await updateCustomerArByEmailAsync(email, {
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
