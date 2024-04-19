<script lang="ts">
import { defineComponent, defineAsyncComponent, inject } from "vue";
import { mapState, mapActions } from "pinia";
import { useCustomerStore } from "@/stores/useCustomerStore";
import { container } from "tsyringe";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import { Emitter, EventType } from "mitt";
import { AppConstants } from "@/infra/AppConstants";
import { usePageLifeCycleStore } from "@/stores/progress-bar/usePageLifeCycleStore";
import { useAppointedRepresentativesStore } from "@/stores/useAppointedRepresentativeStore";
import StaticList from "@/infra/StaticListService";
import { AppointedRepresentative } from "@/entities/appointed-representatives/AppointedRepresentative";
import { AppointedRepresentativeModel } from "@/pages/firm-profile-pages/appointed-representatives/model/AppointedRepresentativeModel";
import isEmpty from "lodash/isEmpty";
import { GridPageChangeEvent } from "@progress/kendo-vue-grid";
import {
  IUserSubmittedChangesService,
  IUserSubmittedChangesServiceInfo,
} from "@/infra/dependency-services/user-submission/IUserSubmittedChangesService";
import { AlertType, useAlert } from "@/composables/useAlert";
import {
  NotificationType,
  useNotification,
} from "@/composables/useNotification";

export default defineComponent({
  components: {
    DetailsModal: defineAsyncComponent(
      () => import("./partials/DetailsModal.vue")
    ),
    TableActions: defineAsyncComponent(
      () => import("./partials/TableActions.vue")
    ),
    StatusLabel: defineAsyncComponent(
      () => import("./partials/StatusLabel.vue")
    ),
  },
  data() {
    return {
      AppConstants,
      isLoading: false,
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      activeGroup: "current",
      tableGroups: [
        { title: "Current", value: "current" },
        { title: "All", value: "all" },
      ],
      columns: [
        {
          field: "name",
          title: "Appointed Representative Name",
          width: 260,
        },
        {
          field: "frn",
          title: "FRN",
          width: 150,
          className: "text-center",
        },
        {
          field: "activities",
          title: "Activities",
          width: 300,
        },
        {
          field: "cf1",
          title: "CF1 (AR)",
        },
        {
          field: "status",
          title: "Status",
          className: "text-center",
        },

        {
          field: "tasks",
          title: "Tasks",
          className: "text-center",
        },

        {
          field: "actions",
          title: " ",
          className: "text-center",
          width: 130,
        },
      ],
      isAddModalVisible: false,
      isViewModalVisible: false,
      isEditModalVisible: false,
      activeAppointedRepresentative: undefined as
        | AppointedRepresentative
        | undefined,
      isFetching: false,
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      eventBusFormSaved: inject("$eventBusService") as Emitter<
        Record<EventType, boolean>
      >,
      tableLoading: false,
      tableSettings: {
        pageSize: 10,
        pageNumber: 1,
        skip: 0,
        filters: {} as Record<string, unknown>,
      },
      userSubmittedChangesService:
        container.resolve<IUserSubmittedChangesService>(
          IUserSubmittedChangesServiceInfo.name
        ),
      isSavingAlertOpened: false,
      isShowSavingText: false,
    };
  },
  provide() {
    return {
      editItem: this.onEditItemAsync,
    };
  },
  computed: {
    ...mapState(useCustomerStore, ["currentCustomer", "currentFirmName"]),
    ...mapState(useAppointedRepresentativesStore, [
      "appointedRepresentatives",
      "appointedRepresentativesTotal",
    ]),
    products() {
      return Object.fromEntries(
        StaticList.getARProducts().map(({ id, title }) => [id, title])
      );
    },
    statusesOptions() {
      return StaticList.getARStatuses();
    },
    activitiesOptions() {
      return StaticList.getARProductTypes().map(({ title, id }) => ({
        value: id,
        label: title,
      }));
    },
    items() {
      if (this.activeGroup === "current") {
        return this.appointedRepresentatives.filter((item) =>
          [0, 1].includes(item.status)
        );
      }
      return this.appointedRepresentatives;
    },
  },
  created() {
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameCreated);
  },
  async mounted() {
    await this.fetchDataAsync();
    this.eventBus.emit(AppConstants.bottomBarEnableEvent, true);
    this.eventBusFormSaved.on(AppConstants.formSavedEvent, (isAutoNext) =>
      this.handleSubmit(isAutoNext)
    );
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameMounted);
  },
  unmounted() {
    this.eventBus.emit(AppConstants.bottomBarEnableEvent, false);
    this.eventBusFormSaved.off(AppConstants.formSavedEvent);
  },
  setup() {
    const pageLifeCycleStore = usePageLifeCycleStore();
    const { changeLifeCycleName } = pageLifeCycleStore;

    return {
      changeLifeCycleName,
    };
  },
  methods: {
    ...mapActions(useAppointedRepresentativesStore, [
      "fetchAppointedRepresentativesAsync",
      "fetchAppointedRepresentativeAsync",
    ]),

    async fetchDataAsync() {
      this.isLoading = true;
      await this.fetchARDataAsync();
      this.isLoading = false;
    },

    async fetchARDataAsync() {
      const { pageSize, pageNumber, filters } = this.tableSettings;
      this.tableLoading = true;
      try {
        await this.fetchAppointedRepresentativesAsync({
          pageSize,
          pageNumber,
          ...filters,
        });
      } catch {
        throw new Error("Something went wrong.");
      } finally {
        this.tableLoading = false;
      }
    },

    handleSubmit(isAutoNext: boolean) {
      if (this.isSavingAlertOpened) {
        return;
      }

      if (isAutoNext) {
        useAlert({
          title: "Confirm",
          content:
            "Appointed Representatives is incomplete. Are you sure you wish to proceed?",
          confirmButtonText: "Proceed",
          saveDetailsText: "Complete Now",
          type: AlertType.SAVEDETAILS,
          width: 390,
          onSaveDetails: () => {
            this.isSavingAlertOpened = false;
          },
          onConfirm: async () => {
            this.saveInfoAsync(true, isAutoNext);
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
          title: "Confirm",
          content: "Please make sure the details are correct",
          confirmButtonText: "Confirm & Save",
          onConfirm: async () => {
            this.saveInfoAsync(true);
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
    },

    saveInfoAsync(isShowLoader: boolean = false, isAutoNext: boolean = false) {
      this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi =
        true;
      this.isSavingAlertOpened = false;
      // ToDo. Do actual saving, currently only next page regardless of data...

      if (isShowLoader) {
        useNotification({
          type: NotificationType.SUCCESS,
          content: this.$t("common-notification-saved"),
          interval: AppConstants.notificationPopupTimeOut,
        });
      }

      this.isLoading = false;
      this.isShowSavingText = false;

      if (isAutoNext) {
        // made sure notification popup is dismissed first before auto next
        setTimeout(
          () => this.eventBus.emit(AppConstants.autoNextEvent),
          AppConstants.notificationPopupTimeOut
        );
      }
    },

    async onViewItemAsync(id: string) {
      this.isFetching = true;
      const item = await this.fetchAppointedRepresentativeAsync(id);
      this.isFetching = false;

      if (!item) {
        return;
      }

      this.activeAppointedRepresentative = item;
      this.isViewModalVisible = true;
    },

    async onEditItemAsync(id: string) {
      this.isFetching = true;
      const item = await this.fetchAppointedRepresentativeAsync(id);
      this.isFetching = false;

      if (!item) {
        return;
      }

      this.activeAppointedRepresentative = item;
      this.isEditModalVisible = true;
    },

    async tableFilterAsync(form: any) {
      const { values } = form;
      if (isEmpty(values)) {
        return;
      }
      const filters: Record<string, unknown> = {};

      if (!isEmpty(values.activities)) {
        filters.activities = values.activities.value;
      }

      if (!isEmpty(values.status)) {
        filters.status = values.status.value;
      }

      // @TODO Add more filters here...

      this.tableSettings = {
        ...this.tableSettings,
        filters: {
          ...this.tableSettings.filters,
          ...filters,
        },
        skip: 0,
        pageNumber: 1,
      };
      await this.fetchARDataAsync();
    },

    async tablePageChangedAsync(event: GridPageChangeEvent) {
      const { skip, take } = event.page;
      const pageNumber = Math.ceil(skip / take + 1);
      this.tableSettings = {
        ...this.tableSettings,
        skip,
        pageNumber,
      };
      await this.fetchARDataAsync();
    },

    async tableSearchAsync(keyword: string) {
      this.tableSettings = {
        ...this.tableSettings,
        filters: {
          ...this.tableSettings.filters,
          search: keyword,
        },
        skip: 0,
        pageNumber: 1,
      };
      await this.fetchARDataAsync();
    },

    async tableClearAsync() {
      this.tableSettings = {
        ...this.tableSettings,
        filters: {
          ...this.tableSettings.filters,
          activities: null,
          status: null,
        },
        skip: 0,
        pageNumber: 1,
      };
      await this.fetchARDataAsync();
    },

    formatItems(items: AppointedRepresentativeModel[]) {
      return items.map((item) => {
        if ([2, 3].includes(item.status)) {
          item.inactive = true;
        }
        return item;
      });
    },
    setUniqueIdentifier(value: string): string {
      const identifier = `${AppConstants.appointedRepresentativesRoute}${value}`;
      return identifier.replace(/\s+/g, "").replace("/", "");
    },
  },
});
</script>

<template src="./appointed-representatives.html" />

<style scoped src="./appointed-representatives.scss" />
