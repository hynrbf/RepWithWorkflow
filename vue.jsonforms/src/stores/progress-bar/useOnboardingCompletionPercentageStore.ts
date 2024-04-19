import { defineStore } from "pinia";
import { AppConstants } from "@/infra/AppConstants";

export const useOnboardingCompletionPercentageStore = defineStore(
  AppConstants.onboardingCompletionPercentageStore,
  {
    state: () => ({
      percentage: [] as { route: string; value: number }[],
    }),
    actions: {
      updateValue(route: string, value: number) {
        const existing = this.percentage.find((p) => p.route === route);

        if (existing) {
          existing.value = value;
          return;
        }

        this.percentage.push({
          route: route,
          value: value,
        });
      },

      getValue(route: string): number {
        const existing = this.percentage.find((p) => p.route === route);

        if (!existing) {
          return 0;
        }

        return existing.value;
      },
    },
    persist: true,
  },
);
