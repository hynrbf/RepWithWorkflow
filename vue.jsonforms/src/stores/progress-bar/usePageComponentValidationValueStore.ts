import { defineStore } from "pinia";
import { AppConstants } from "@/infra/AppConstants";
import { OnboardingType } from "@/infra/base";
import { RouteProps } from "@/entities/owners-and-controllers/RouteProps";

export const usePageComponentValidationValueStore = defineStore(
  AppConstants.pageComponentValidationValueStore,
  {
    state: () => ({
      componentThenValidationValue: [] as Record<string, string>[],
    }),
    actions: {
      addComponentValidationValue(
        keyName: string,
        newData: Record<string, string>,
      ) {
        if (!keyName || !newData || typeof newData[keyName] === "undefined") {
          return;
        }

        const onboardingType: string | null = localStorage.getItem(
          AppConstants.onboardingTypeKey,
        );

        let isLegitComponentId = false;
        let profilePages: RouteProps[] = [];

        switch (onboardingType) {
          case OnboardingType.Ar.toString(): {
            profilePages = AppConstants.arProfilePages;
            break;
          }
          case OnboardingType.Employee.toString(): {
            profilePages = AppConstants.employeeProfilePages;
            break;
          }
          case OnboardingType.Provider.toString(): {
            profilePages = AppConstants.providerProfilePages;
            break;
          }
          case OnboardingType.Introducer.toString(): {
            profilePages = AppConstants.introducerProfilePages;
            break;
          }
          default: {
            profilePages = AppConstants.firmProfilePages;
          }
        }

        profilePages.forEach((item) => {
          if (isLegitComponentId) {
            return;
          }

          let routeCleaned: string = "";

          const slashMatches: Array<string> | null = item.route.match(/\//g);

          routeCleaned = item.route.replace("/", "");

          if (slashMatches && slashMatches.length > 1) {
            const routeTemp: Array<string> = item.route.split("/");
            routeCleaned = routeTemp[2];
          }

          if (!keyName.toLowerCase().includes(routeCleaned.toLowerCase())) {
            return;
          }

          isLegitComponentId = true;
        });

        if (!isLegitComponentId) {
          return;
        }

        const existingEntryIndex = this.componentThenValidationValue.findIndex(
          (entry) => entry.key === newData.key,
        );

        if (existingEntryIndex === -1) {
          this.componentThenValidationValue.push(newData);
          return;
        }

        const existingEntry =
          this.componentThenValidationValue[existingEntryIndex];

        if (existingEntry[keyName] === newData[keyName]) {
          return;
        }

        this.componentThenValidationValue[existingEntryIndex][keyName] =
          newData[keyName];
      },

      clearValidationValuesByPrefix(prefix: string) {
        this.componentThenValidationValue =
          this.componentThenValidationValue.map((values) => {
            return Object.fromEntries(
              Object.entries(values).filter(([key]) => {
                return !new RegExp(`^${prefix}`).test(key);
              }),
            );
          });
      },

      removeValidationValueByFieldId(fieldId: string) {
        if (!fieldId) {
          return;
        }

        this.componentThenValidationValue =
          this.componentThenValidationValue.map((obj) => {
            const newObj = { ...obj };
            delete newObj[fieldId];
            return newObj;
          });
      },
    },
    persist: true,
  },
);
