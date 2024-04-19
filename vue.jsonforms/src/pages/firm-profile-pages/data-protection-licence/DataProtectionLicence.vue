<script lang="ts">
import { defineAsyncComponent, defineComponent, inject } from "vue";
import { useCustomerStore } from "@/stores/useCustomerStore";
import { mapState, mapActions } from "pinia";
import { usePageLifeCycleStore } from "@/stores/progress-bar/usePageLifeCycleStore";
import { AppConstants } from "@/infra/AppConstants";
import { Emitter, EventType } from "mitt";
import { DataProtectionLicenceEntity } from "@/entities/data-protection-license/DataProtectionLicenceEntity";
import { container } from "tsyringe";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import { AlertType, useAlert } from "@/composables/useAlert";
import {
  NotificationType,
  useNotification,
} from "@/composables/useNotification";
import isEmpty from "lodash/isEmpty";

import CategoriesOfPersonalData from "./partials/CategoriesOfPersonalData.vue";
import TimingOfDataCollection from "./partials/TimingOfDataCollection.vue";
import MethodsOfDataCollection from "./partials/MethodsOfDataCollection.vue";
import PurposeOfDataCollection from "./partials/PurposeOfDataCollection.vue";
import DataProtectionLicenseViewer from "./partials/DataProtectionLicenseViewer.vue";
import { pick } from "lodash";
import {
  IUserSubmittedChangesService,
  IUserSubmittedChangesServiceInfo,
} from "@/infra/dependency-services/user-submission/IUserSubmittedChangesService";
import { toRaw } from "vue";
import { usePageFieldsInvalidHandlerStore } from "@/stores/usePageFieldsInvalidHandlerStore";
import { useAutoSaveStore } from "@/stores/useAutoSaveStore";
import { CustomerBasic } from "@/entities/CustomerBasic";

export default defineComponent({
  name: "DataProtectionLicence",
  components: {
    CategoriesOfPersonalData,
    TimingOfDataCollection,
    MethodsOfDataCollection,
    PurposeOfDataCollection,
    DataProtectionLicenseViewer,
    KendoFirmFinderComponentAsync: defineAsyncComponent(
      () => import("@/components/form-fields/KendoFirmFinderComponent.vue"),
    ),
  },
  data() {
    return {
      AppConstants,
      isReady: false,
      isLoading: false,
      isShowSavingText: false,
      currentTab: 0,
      tabs: [
        {
          title: this.$t("dataProtectionLicencePage-details"),
          content: "details",
        },
        {
          title: this.$t("dataProtectionLicencePage-categoriesOfPersonalData"),
          content: "categoriesOfPersonalData",
        },
        {
          title: this.$t("dataProtectionLicencePage-timingOfDataCollection"),
          content: "timingOfDataCollection",
        },
        {
          title: this.$t("dataProtectionLicencePage-methodsOfDataCollection"),
          content: "methodsOfDataCollection",
        },
        {
          title: this.$t("dataProtectionLicencePage-purposeOfDataCollection"),
          content: "purposeOfDataCollection",
        },
      ],
      dataProtectionLicence: new DataProtectionLicenceEntity(),
      dataProtectionLicenceCache: {} as Partial<DataProtectionLicenceEntity>,
      isViewDocument: false,
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      eventBusFormSaved: inject("$eventBusService") as Emitter<
        Record<EventType, boolean>
      >,
      userSubmittedChangesService:
        container.resolve<IUserSubmittedChangesService>(
          IUserSubmittedChangesServiceInfo.name,
        ),
      isSavingAlertOpened: false,
      debounceTimerInMs: 60000,
      debouncedAutoSaveFunction: () => {},
    };
  },
  computed: {
    ...mapState(useCustomerStore, ["currentCustomer", "currentFirmName"]),
  },
  created() {
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameCreated);
    this.debouncedAutoSaveFunction = this.debounceFunction(this.saveInfoAsync);
  },
  mounted() {
    this.setInitialValues();
    this.isReady = true;
    this.eventBus.emit(AppConstants.bottomBarEnableEvent, true);
    this.eventBusFormSaved.on(AppConstants.formSavedEvent, (isAutoNext) =>
      this.handleSubmit(isAutoNext),
    );
    this.eventBus.on(AppConstants.formFieldPageLevelChangedEvent, () => {
      this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi =
        false;
      this.setAutoSaveFunctionNotCompletedYet();
      this.debouncedAutoSaveFunction();
    });
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameMounted);
    this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi = false;
  },
  unmounted() {
    this.isLoading = false;
    this.eventBusFormSaved.off(AppConstants.formSavedEvent);
    this.eventBus.off(AppConstants.formFieldPageLevelChangedEvent);
  },
  setup() {
    const pageLifeCycleStore = usePageLifeCycleStore();
    const { changeLifeCycleName } = pageLifeCycleStore;

    const pageFieldsInvalidHandlerStore = usePageFieldsInvalidHandlerStore();
    const { processInvalidFields } = pageFieldsInvalidHandlerStore;
    const { debounceFunction, setAutoSaveFunctionNotCompletedYet } =
      useAutoSaveStore();

    return {
      changeLifeCycleName,
      processInvalidFields,
      debounceFunction,
      setAutoSaveFunctionNotCompletedYet,
    };
  },
  methods: {
    ...mapActions(useCustomerStore, ["updateCustomerByEmailAsync"]),

    selectTab(event: any) {
      this.currentTab = event.selected;
    },

    setUniqueIdentifier(value: string): string {
      const identifier = `${AppConstants.dataProtectionRoute}-dataProtectionLicense${value}`;
      return identifier.replace(/\s+/g, "").replace("/", "");
    },

    setInitialValues() {
      if (!this.currentCustomer) {
        return;
      }

      const { dataProtectionLicense, dataProtectionLicenseCache } =
        this.currentCustomer;

      if (!dataProtectionLicense) {
        return;
      }

      const dataProtectionLicenseRaw = toRaw(dataProtectionLicense);
      this.dataProtectionLicence = {
        ...this.dataProtectionLicence,
        ...Object.fromEntries(
          Object.entries(dataProtectionLicenseRaw).map(([key, value]) => {
            const collection: Record<
              string,
              keyof DataProtectionLicenceEntity
            > = {
              categoriesOfPersonalDataService:
                "categoriesOfPersonalDataServiceConfirmed",
              categoriesOfPersonalDataNonService:
                "categoriesOfPersonalDataNonServiceConfirmed",
              methodsOfDataCollectionService:
                "methodsOfDataCollectionServiceConfirmed",
              methodsOfDataCollectionNonService:
                "methodsOfDataCollectionNonServiceConfirmed",
              purposeOfDataCollectionService:
                "purposeOfDataCollectionServiceConfirmed",
              purposeOfDataCollectionNonService:
                "purposeOfDataCollectionNonServiceConfirmed",
              timingOfDataCollectionService:
                "timingOfDataCollectionServiceConfirmed",
              timingOfDataCollectionNonService:
                "timingOfDataCollectionNonServiceConfirmed",
            };
            const temp = new DataProtectionLicenceEntity();
            if (collection[key]) {
              return [
                key,
                isEmpty(value) &&
                isEmpty(dataProtectionLicenseRaw[collection[key]])
                  ? temp[key as keyof DataProtectionLicenceEntity]
                  : value,
              ];
            }
            return [key, value];
          }),
        ),
      };
      this.dataProtectionLicenceCache = {
        ...pick(this.dataProtectionLicence, [
          "licenseNumber",
          "endDate",
          "renewalDate",
        ]),
        ...(dataProtectionLicenseCache || {}),
      };
      (this.$refs?.formElement as HTMLFormElement)?.setValues(this.dataProtectionLicence);
    },

    async handleSubmit(isAutoNext: boolean) {
      if (this.isSavingAlertOpened) {
        return;
      }

      if (isAutoNext) {
        useAlert({
          title: "Confirm",
          content:
            "Data Protection License is incomplete. Are you sure you wish to proceed?",
          confirmButtonText: "Proceed",
          saveDetailsText: "Complete Now",
          type: AlertType.SAVEDETAILS,
          width: 390,
          onSaveDetails: () => {
            this.isSavingAlertOpened = false;
          },
          onConfirm: async () => {
            await this.saveInfoAsync(true, isAutoNext);
          },
          onCancel: () => {
            this.isSavingAlertOpened = false;
          },
          onClose: () => {
            this.isSavingAlertOpened = false;
          },
        });
      } else {
        useAlert({
          title: this.$t("common-alert-title"),
          content: this.$t("common-alert-content"),
          confirmButtonText: this.$t("common-alert-buttonText"),
          onConfirm: async () => {
            await this.saveInfoAsync(true);
          },
          onCancel: () => {
            this.isSavingAlertOpened = false;
          },
          onClose: () => {
            this.isSavingAlertOpened = false;
          },
        });
      }

      this.isSavingAlertOpened = true;
      (this.$refs?.formElement as HTMLFormElement)?.$el?.requestSubmit?.();
    },

    async saveInfoAsync(
      isShowLoader: boolean = false,
      isAutoNext: boolean = false,
    ): Promise<void> {
      const { email } = this.currentCustomer;

      if (!email) {
        return;
      }

      this.processInvalidFields(
        AppConstants.dataProtectionRoute,
        this.eventBus,
      );
      this.isLoading = isShowLoader;
      this.isShowSavingText = isShowLoader;
      this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi =
        true;
      this.isSavingAlertOpened = false;

      try {
        await this.updateCustomerByEmailAsync(email, {
          dataProtectionLicense: this.dataProtectionLicence,
        } as CustomerBasic);

        const notificationPopupTimeOut = 4e3;
        if (isShowLoader) {
          useNotification({
            type: NotificationType.SUCCESS,
            content: "Saved successfully.",
            interval: notificationPopupTimeOut,
          });
        }

        this.isLoading = false;
        this.isShowSavingText = false;

        if (isAutoNext) {
          // made sure notification popup is dismissed first before auto next
          setTimeout(
            () => this.eventBus.emit(AppConstants.autoNextEvent),
            notificationPopupTimeOut,
          );
        }
      } catch (error) {
        useNotification({
          type: NotificationType.ERROR,
          content: "Something went wrong.",
          interval: 4e3,
        });
      }

      this.isLoading = false;
    },

    isFieldModified(key: keyof DataProtectionLicenceEntity) {
      if (
        !this.dataProtectionLicenceCache[key] ||
        this.dataProtectionLicenceCache[key] === this.dataProtectionLicence[key]
      ) {
        return false;
      }
      return true;
    },
  },
});
</script>

<template src="./data-protection-licence.html" />

<style scoped src="./data-protection-licence.scss" />