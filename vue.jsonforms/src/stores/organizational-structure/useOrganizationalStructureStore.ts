import { defineStore } from "pinia";
import { AppConstants } from "@/infra/AppConstants";
import DropdownListItemModel from "@/components/models/DropdownListItemModel";
import { Employee } from "@/entities/firm-details/Employee";
import { container } from "tsyringe";
import {
  IOrganizationalStructureService,
  IOrganizationalStructureServiceInfo,
} from "@/infra/dependency-services/rest/organizational-structure/IOrganizationalStructureService";

export const useOrganizationalStructureStore = defineStore(
  AppConstants.organizationalStructureStore,
  {
    state: () => ({
      primaryRoles: [] as DropdownListItemModel[],
      otherRoles: [] as DropdownListItemModel[],
      employees: [] as Employee[],
    }),
    actions: {
      persistPrimaryRoles(roles: DropdownListItemModel[]) {
        if (this.primaryRoles.length) {
          this.primaryRoles.splice(0);
        }

        this.primaryRoles = [...roles];
      },

      persistOtherRoles(roles: DropdownListItemModel[]) {
        if (this.otherRoles.length) {
          this.otherRoles.splice(0);
        }

        this.otherRoles = [...roles];
      },

      clearAllRoles() {
        this.primaryRoles.splice(0);
        this.otherRoles.splice(0);
      },

      getPrimaryRoles() {
        return this.primaryRoles;
      },

      getOtherRoles() {
        return this.otherRoles;
      },

      async fetchInitialEmployeesAsync(companyNumber: string) {
        const organizationService =
          container.resolve<IOrganizationalStructureService>(
            IOrganizationalStructureServiceInfo.name,
          );

        const response =
          await organizationService.getEmployeesAsync(companyNumber);

        if (!response) {
          return;
        }

        this.employees = response;
      },

      getEmployees() {
        return this.employees;
      },
    },
    persist: true,
  },
);
