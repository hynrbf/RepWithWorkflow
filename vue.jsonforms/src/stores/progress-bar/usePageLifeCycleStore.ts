import { defineStore } from "pinia";

export const usePageLifeCycleStore = defineStore("pageLifeCycleStore", {
  state: () => ({
    lifeCycleName: "",
  }),
  getters: {
    currentLifeCycleName(): string {
      return this.lifeCycleName;
    },
  },
  actions: {
    changeLifeCycleName(name: string) {
      this.lifeCycleName = name;
    },
  },
});
