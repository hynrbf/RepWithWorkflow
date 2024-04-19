import { defineStore, storeToRefs } from "pinia";
import { container } from "tsyringe";
import {
  IFcaService,
  IFcaServiceInfo,
} from "@/infra/dependency-services/rest/fca/IFcaService";
import { useCustomerStore } from "@/stores/useCustomerStore";
import { useArCustomerStore } from "@/stores/useArCustomerStore";
import { OnboardingType } from "@/infra/base";
export interface State {
  staffs: { id: string; name: string }[];
  isFetchingStaffs: boolean;
}

const fcaService = container.resolve<IFcaService>(IFcaServiceInfo.name);
const { currentCustomer, currentCustomerName } = storeToRefs(
  useCustomerStore()
);

const { currentArCustomer, currentArCustomerName } = storeToRefs(
  useArCustomerStore()
);

export const useStaffStore = defineStore("staffStore", {
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
        const onboardingType: string | null = localStorage.getItem("onboarding-type");
        let firmRefNo: string = ""; 
        
        // Include current customer
        switch(onboardingType) {
        case OnboardingType.Ar.toString():
          if (currentArCustomer.value) {
            items = [
              ...items,
              {
                name: currentArCustomerName.value,
                id: `${currentArCustomer.value.id}`,
              },
            ];

            firmRefNo = currentArCustomer.value.firmReferenceNumber as string;
          }
          break;
        case OnboardingType.Firm.toString():
        default:
          if (currentCustomer.value) {
            items = [
              ...items,
              {
                name: currentCustomerName.value,
                id: `${currentCustomer.value.id}`,
              },
            ];

            firmRefNo = currentCustomer.value.firmReferenceNumber as string;
          }
        }
 
        // Get from firm individuals
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
