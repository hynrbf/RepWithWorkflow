<script lang="ts">
import { AppConstants } from "@/infra/AppConstants";
import { Emitter, EventType } from "mitt";
import { defineComponent, inject } from "vue";
import { container } from "tsyringe";
import {
  IAppService,
  IAppServiceInfo,
} from "@/infra/dependency-services/app/IAppService";
import { usePageLifeCycleStore } from "@/stores/progress-bar/usePageLifeCycleStore";
import { StackLayout } from "@progress/kendo-vue-layout";
import KendoProgressBarComponent from "@/components/KendoProgressBarComponent.vue";

export default defineComponent({
  name: "TestProgressBar",
  components: { KendoProgressBarComponent, StackLayout },
  data() {
    return {
      pageDescription: "",
      firstName: "", //chi
      lastName: "", //more
      eventBusFormSaved: inject("$eventBusService") as Emitter<Record<EventType, boolean>>,
      appService: container.resolve<IAppService>(IAppServiceInfo.name),
      isInitializing: true,
    };
  },
  created() {
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameCreated);
  },
  mounted() {
    this.getCompanyDetails();
    this.pageDescription = "This is a test page for the progress bar";
    this.eventBusFormSaved.on(AppConstants.formSavedEvent, (_isAutoNext) => {
      (this.$refs?.formElement as HTMLFormElement)?.$el?.requestSubmit?.();
    });

    this.isInitializing = false;
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameMounted);
  },
  computed: {
    // Create a computed property to reactively update the template when the store's state changes
    currentLifeCycleName(): string {
      const pageLifeCycleStore = usePageLifeCycleStore();
      return pageLifeCycleStore.currentLifeCycleName;
    },
  },
  setup() {
    const pageLifeCycleStore = usePageLifeCycleStore();
    const { changeLifeCycleName } = pageLifeCycleStore;

    return {
      changeLifeCycleName,
    };
  },
  methods: {
    getCompanyDetails() {
      this.firstName = this.appService.getCustomerFirmName();
      this.lastName = "res";
    },
  },
});
</script>

<template src="./TestProgressBar.html" />

<style scoped>
.main {
  margin: auto;
  width: 1080px;
  align-items: center;
  justify-content: center;
}

.title-component {
  margin-top: 20px;
  width: 720px;
}

.form-component {
  width: 698px;
  margin-top: 20px;
}
</style>
