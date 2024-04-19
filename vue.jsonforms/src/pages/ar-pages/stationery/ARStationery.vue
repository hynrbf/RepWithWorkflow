<script lang="ts">
import { defineComponent, inject } from "vue";
import { container } from "tsyringe";
import { Emitter, EventType } from "mitt";
import isEmpty from "lodash/isEmpty";
import { mapState, mapActions } from "pinia";
import moment from "moment";
import { v4 as uuid } from "uuid";
import { useArCustomerStore } from "@/stores/useArCustomerStore";
import { useArStationeryStore } from "@/stores/stationery/useArStationeryStore";
import { usePageLifeCycleStore } from "@/stores/progress-bar/usePageLifeCycleStore";
import { DisclosureArrayItem } from "@/pages/firm-profile-pages/stationery/StationeryTypes";
import { DisclosureEntity } from "@/entities/stationery/DisclosureEntity";
import { StationeryEntity } from "@/entities/stationery/StationeryEntity";
import { StationeryStatus } from "@/entities/stationery/StationeryStatus";
import { AppConstants } from "@/infra/AppConstants";
import StaticList from "@/infra/StaticListService";
import { useAlert } from "@/composables/useAlert";
import {
  NotificationType,
  useNotification,
} from "@/composables/useNotification";
import {
  IAppService,
  IAppServiceInfo,
} from "@/infra/dependency-services/app/IAppService";
import {
  IUserSubmittedChangesService,
  IUserSubmittedChangesServiceInfo,
} from "@/infra/dependency-services/user-submission/IUserSubmittedChangesService";
import Actions from "./partials/ARActions.vue";
import ARStationeryModal from "./partials/ARStationeryModal.vue";
import ARViewStationeryModal from "./partials/ARViewStationeryModal.vue";

export default defineComponent({
  name: "Stationery",
  components: {
    Actions,
    ARStationeryModal,
    ARViewStationeryModal,
  },
  data() {
    return {
      currentTab: 0,
      tabs: [
        { title: this.$t("stationaryPage-stationery"), content: "stationery" },
        {
          title: this.$t("stationaryPage-disclosures"),
          content: "disclosures",
        },
      ],
      isStationeryModalVisible: false,
      isSavingStationery: false,
      isLoading: false,
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      eventBusFormSaved: inject("$eventBusService") as Emitter<
        Record<EventType, boolean>
      >,
      appService: container.resolve<IAppService>(IAppServiceInfo.name),
      userSubmittedChangesService:
        container.resolve<IUserSubmittedChangesService>(
          IUserSubmittedChangesServiceInfo.name
        ),
      isSavingAlertOpened: false,
      isViewStationeryModalVisible: false,
      isFetchingStationery: false,
      isActionView: true,
      isViewFileDownloadModalVisible: false,
      disclosure: StaticList.getDefaultDisclosure() as DisclosureEntity,
    };
  },
  computed: {
    ...mapState(useArCustomerStore, ["currentArCustomer", "currentArFirmName"]),
    ...mapState(useArStationeryStore, ["stationery", "stationeries"]),
    stationeryColumns() {
      return [
        {
          field: "stationeryName",
          title: "Stationery Name",
          width: 300,
        },
        {
          field: "filesUploaded",
          title: "Files Uploaded",
          className: "text-center",
        },
        {
          field: "status",
          title: "Status",
          className: "text-center",
        },
        {
          field: "lastUpdated",
          title: "Last Updated",
        },
        {
          field: "actions",
          title: " ",
          className: "text-center",
          width: 160,
        },
      ];
    },
    disclosureTypes() {
      return AppConstants.disclosureTypes;
    },
    disclosuresNav() {
      return this.disclosureTypes.map((type: Record<string, unknown>) => {
        type.label = type.title;
        type.anchorTo = type.id;
        return type;
      });
    },
    disclosuresListBox() {
      return Object.fromEntries(
        this.disclosureTypes.map((type: Record<string, unknown>) => {
          const { id, title } = type as { id: string; title: string };
          return [
            id,
            [
              {
                id: `${id}Text`,
                title: `Text for ${title}`,
                initialItems: [],
                emptyText: "Text",
              },
              {
                id: `${id}ConfirmedText`,
                title: `Confirmed Text for ${title}`,
                initialItems: [],
                isFeatured: true,
                noAdd: true,
                emptyText: "Confirmed Text",
                keepEditting: true,
              },
            ] as DisclosureArrayItem[],
          ] as [string, DisclosureArrayItem[]];
        })
      );
    },
    selectedStationery() {
      return this.stationery;
    },
  },
  async mounted() {
    this.isLoading = true;
    this.setInitialValues();
    this.isLoading = false;

    this.eventBus.emit(AppConstants.bottomBarEnableEvent, true);
    this.eventBusFormSaved.on(AppConstants.formSavedEvent, (isAutoNext) =>
      this.handleSubmit(isAutoNext)
    );

    this.$watch(
      (vm) => [vm.stationery, vm.stationeries, vm.disclosure],
      () => {
        this.eventBus.emit(AppConstants.formFieldChangedEvent);
        this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
      },
      { deep: true }
    );

    this.changeLifeCycleName(AppConstants.pageLifeCycleNameMounted);
  },
  unmounted() {
    this.eventBus.emit(AppConstants.bottomBarEnableEvent, false);
    this.eventBusFormSaved.off(AppConstants.formSavedEvent);
    this.isLoading = false;
  },
  created() {
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameCreated);
    this.eventBus.on(AppConstants.formFieldChangedEvent, () => {
      this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi =
        false;
    });
  },
  setup() {
    const pageLifeCycleStore = usePageLifeCycleStore();
    const { changeLifeCycleName } = pageLifeCycleStore;

    return {
      changeLifeCycleName,
    };
  },
  methods: {
    ...mapActions(useArCustomerStore, ["updateCustomerArByEmailAsync"]),
    ...mapActions(useArStationeryStore, [
      "createStationeryAsync",
      "updateStationeryAsync",
      "fetchStationeriesAsync",
      "getStationery",
      "setStationery",
    ]),
    moment,
    selectTab(event: any) {
      this.currentTab = event.selected;
    },
    fetchDisclosure() {
      const { disclosure } = this.currentArCustomer;

      if (isEmpty(disclosure)) {
        return;
      }

      this.disclosure = Object.fromEntries(
        Object.entries(disclosure).map(([key, value]) => [
          key,
          (value || []).map((content: string) => ({ content })),
        ])
      ) as DisclosureEntity;
    },
    async setInitialValues() {
      await Promise.all([
        this.fetchStationeriesAsync(),
        this.fetchDisclosure(),
      ]);
    },

    viewStationery(id: string, action: string) {
      this.isViewStationeryModalVisible = true;
      this.isFetchingStationery = true;
      this.getStationery(id, true);
      this.isFetchingStationery = false;
      this.isActionView = action === AppConstants.viewAction;
    },

    uploadStationery(id: string) {
      this.isStationeryModalVisible = true;
      this.isFetchingStationery = true;
      this.getStationery(id, true);
      this.isFetchingStationery = false;
    },

    clearStationery() {
      this.setStationery(undefined);
    },

    async saveStationeryAsync(values: Partial<StationeryEntity>) {
      const stationery = new StationeryEntity();
      const dateNow = moment().unix();
      stationery.id = uuid();
      stationery.name = values.name;
      stationery.icon = "clipboard-text-22";
      stationery.status = StationeryStatus.Pending;
      stationery.files = values.files || [];
      stationery.createdAt = dateNow;
      stationery.updatedAt = dateNow;

      try {
        await this.createStationeryAsync(stationery);
        this.isStationeryModalVisible = false;
        this.isSavingStationery = false;
        this.clearStationery();
      } catch (error) {
        useNotification({
          type: NotificationType.ERROR,
          content: "Something went wrong.",
          interval: 4e3,
        });
      }
    },

    async updateStationeryDataAsync(values: StationeryEntity) {
      try {
        await this.updateStationeryAsync(values?.id ?? "", values, true);
        this.isViewStationeryModalVisible = false;
        this.isStationeryModalVisible = false;
        this.isSavingStationery = false;
        this.clearStationery();
      } catch (error) {
        useNotification({
          type: NotificationType.ERROR,
          content: "Something went wrong.",
          interval: 4e3,
        });
      }
    },

    handleSubmit(isAutoNext: boolean) {
      // ToDo. Integrate saving once API is ready.
      if (this.isSavingAlertOpened) {
        return;
      }

      useAlert({
        title: this.$t("common-alert-title"),
        content: this.$t("common-alert-content"),
        confirmButtonText: this.$t("common-alert-buttonText"),
        onConfirm: async () => {
          const { email } = this.currentArCustomer;

          if (!email) {
            return;
          }

          this.isLoading = true;
          this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi =
            true;
          this.isSavingAlertOpened = false;

          try {
            const disclosure = Object.fromEntries(
              Object.entries(this.disclosure).map(([key, value]) => [
                key,
                (value as any).map(
                  ({ content }: Record<string, unknown>) => content
                ),
              ])
            ) as DisclosureEntity;

            await this.updateCustomerArByEmailAsync(email, {
              disclosure,
              stationeries: this.stationeries,
            });
            const notificationPopupTimeOut = 4e3;
            useNotification({
              type: NotificationType.SUCCESS,
              content: "Saved successfully.",
              interval: notificationPopupTimeOut,
            });

            if (isAutoNext) {
              // made sure notification popup is dismissed first before auto next
              setTimeout(
                () => this.eventBus.emit(AppConstants.autoNextEvent),
                notificationPopupTimeOut
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
        onCancel: () => {
          this.isSavingAlertOpened = false;
        },
        onClose: () => {
          this.isSavingAlertOpened = false;
        },
      });

      this.isSavingAlertOpened = true;
      (this.$refs?.formElement as HTMLFormElement)?.$el?.requestSubmit?.();
    },
  },
  watch: {
    disclosure: {
      handler() {
        this.eventBus.emit(AppConstants.formFieldChangedEvent);
        this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
      },
      deep: true,
    },
  },
});
</script>

<template src="./ar-stationery.html" />

<style scoped src="./ar-stationery.scss" />
