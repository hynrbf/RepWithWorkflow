import { defineStore } from "pinia";
import { AUTOSAVE_ENABLED } from "@/config";

export const useAutoSaveStore = defineStore("autoSaveStore", {
  state: () => ({
    isDisableAutoSave: !AUTOSAVE_ENABLED,
    isAutoSaveFunctionCompleted: false,
  }),
  getters: {
    getAutoSaveFunctionCompletedValue(): boolean {
      return this.isAutoSaveFunctionCompleted;
    },
  },
  actions: {
    debounceFunction(
      // ToDo. part of 18 IMPT errors to fix
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      func: (...args: any[]) => Promise<void>,
      // ToDo. part of 18 IMPT errors to fix
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
    ): (...args: any[]) => void {
      let timeout: number | undefined;
      const wait = 60000; //60000ms = 1 minute
      // eslint-disable-next-line @typescript-eslint/no-this-alias
      const thisStore = this;

      // ToDo. part of 18 IMPT errors to fix
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      return function executedFunction(...args: any[]) {
        const later = () => {
          clearTimeout(timeout);

          if (thisStore.isDisableAutoSave) {
            return;
          }

          func(...args)
            .then(() => {
              thisStore.isAutoSaveFunctionCompleted = true;
            })
            .catch((error) => {
              console.error("Auto save function failed", error);
            });
        };

        clearTimeout(timeout);
        timeout = window.setTimeout(later, wait);
      };
    },
    setAutoSaveFunctionNotCompletedYet() {
      this.isAutoSaveFunctionCompleted = false;
    },
    setDisableAutoSave(isDisable: boolean): void {
      this.isDisableAutoSave = isDisable;
    },
  },
});
