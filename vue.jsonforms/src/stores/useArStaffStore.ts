import { defineStore, storeToRefs } from "pinia";
import { container } from "tsyringe";
import {
  IFcaService,
  IFcaServiceInfo,
} from "@/infra/dependency-services/rest/fca/IFcaService";
import { useArCustomerStore } from "@/stores/useArCustomerStore";

export interface State {
  staffs: { id: string; name: string }[];
  isFetchingStaffs: boolean;
}

const fcaService = container.resolve<IFcaService>(IFcaServiceInfo.name);
const { currentArCustomer, currentArCustomerName } = storeToRefs(
  useArCustomerStore()
);

export const useArStaffStore = defineStore("arStaffStore", {
  state: (): State => ({
    staffs: [],
    isFetchingStaffs: false,
  }),
  actions: {
    getStaff(id: string) {
      return this.staffs.find((staff) => staff.id === id);
    },
    async fetchStaffsAsync() {
      try {
        this.isFetchingStaffs = true;
        let items = [] as typeof this.staffs;

        // Include current customer
        if (currentArCustomer.value) {
          items = [
            ...items,
            {
              name: currentArCustomerName.value,
              id: `${currentArCustomer.value.id}`,
            },
          ];
        }

        // Get from firm individuals
        const firmRefNo = currentArCustomer.value.firmReferenceNumber as string;
        const individuals =
          (await fcaService.getFirmIndividualsAsync(firmRefNo)) || [];
        items = [
          ...items,
          ...individuals.map(({ IRN, name }) => ({
            id: `${IRN}`,
            name: `${name}`,
          })),
        ];

        this.staffs = items;
        return Promise.resolve(items);
      } catch (error) {
        return Promise.reject(error);
      } finally {
        this.isFetchingStaffs = false;
      }
    },
  },
});
