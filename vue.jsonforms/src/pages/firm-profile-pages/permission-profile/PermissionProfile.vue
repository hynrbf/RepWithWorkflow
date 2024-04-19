<script lang="ts">
import { defineComponent, inject } from "vue";
import { CompanyEntity } from "@/entities/CompanyEntity";
import { AppConstants } from "@/infra/AppConstants";
import { PermissionGroup } from "@/entities/PermissionGroup";
import { PermissionStateEnum } from "@/entities/enums/PermissionStateEnum";
import { CustomerPermission } from "@/entities/CustomerPermission";
import { PermissionResult } from "@/entities/PermissionResult";
import { PermissionEdit } from "@/entities/PermissionEdit";
import { v4 as uuidv4 } from "uuid";
import { SubPermission } from "@/entities/SubPermission";
import {
  IFcaService,
  IFcaServiceInfo,
} from "@/infra/dependency-services/rest/fca/IFcaService";
import { container } from "tsyringe";
import { Emitter, EventType } from "mitt";
import { CustomerEntity } from "@/entities/CustomerEntity";
import {
  ICustomerService,
  ICustomerServiceInfo,
} from "@/infra/dependency-services/rest/forms-compliance/ICustomerService";
import { mapState } from "pinia";
import { useCustomerStore } from "@/stores/useCustomerStore";
import {
  IAppService,
  IAppServiceInfo,
} from "@/infra/dependency-services/app/IAppService";
import PermissionsTable from "./partials/PermissionsTable.vue";
import { AlertType, useAlert } from "@/composables/useAlert";
import {
  NotificationType,
  useNotification,
} from "@/composables/useNotification";
import { usePageLifeCycleStore } from "@/stores/progress-bar/usePageLifeCycleStore";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import {
  IUserSubmittedChangesService,
  IUserSubmittedChangesServiceInfo,
} from "@/infra/dependency-services/user-submission/IUserSubmittedChangesService";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";
import { useAutoSaveStore } from "@/stores/useAutoSaveStore";
import { PermissionTabModel } from "@/pages/firm-profile-pages/permission-profile/models/permission-tab-model";

export default defineComponent({
  name: "PermissionProfile",
  components: {
    PermissionsTable,
  },
  data() {
    return {
      AppConstants,
      currentTab: 0,
      tabs: [
        {
          title: "Mortgage Broker Authorisation Status",
          content: "tab-1",
        },
        {
          title: "Insurance and Protection Broker Authorisation Status",
          content: "tab-2",
        },
        {
          title: "Other Permissions",
          content: "tab-3",
        },
      ] as PermissionTabModel[],
      customerService: container.resolve<ICustomerService>(
        ICustomerServiceInfo.name,
      ),
      appService: container.resolve<IAppService>(IAppServiceInfo.name),
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      selectedFcaFirm: null as CompanyEntity | null,
      customer: new CustomerEntity(),
      fcaService: container.resolve<IFcaService>(IFcaServiceInfo.name),
      definedGroupedPermissions: [] as PermissionEdit[],
      currentFcaSubPermissions: [] as CustomerPermission[],
      customerSubPermissions: [] as CustomerPermission[],
      subPermissionsFromFca: null as unknown as PermissionResult,
      isLoading: false,
      complianceFirmName: "",
      isShowSavingText: false,
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
      debouncedAutoSaveFunction: () => {},
    };
  },
  computed: {
    ...mapState(useCustomerStore, ["currentFirmName"]),

    parsedPermissions() {
      const permissions = [];

      for (const permission of this.customerSubPermissions) {
        const {
          subPermissionName = "",
          subPermissionDisplayText = "",
          categoryName = "",
          permissionGroupName = "",
        } = permission;

        if (!subPermissionName.trim()) {
          permissions.push({
            id: uuidv4(),
            permissionGroupName: permission.permissionGroupName,
            categoryName: permission.categoryName,
            subPermissions: [],
          });
          continue;
        }

        const existingPermissionGroup = permissions.find(
          (permission) =>
            permission.permissionGroupName === permissionGroupName,
        );

        const subPermission: SubPermission = {
          name: subPermissionName,
          displayText: subPermissionDisplayText,
          categoryName: categoryName,
        };

        if (existingPermissionGroup) {
          existingPermissionGroup.subPermissions.push(subPermission);
        } else {
          const actualPermission: PermissionGroup = {
            id: uuidv4(),
            permissionGroupName,
            categoryName,
            subPermissions: [subPermission],
          };
          permissions.push(actualPermission);
        }
      }

      return permissions;
    },

    mortgageBrokerPermissions() {
      return this.getGroupedPermissions(
        AppConstants.PermissionCategoryMortgateBroker,
      );
    },

    insuranceBrokerPermissions() {
      return this.getGroupedPermissions(
        AppConstants.PermissionCategoryInsuranceBroker,
      );
    },

    otherPermissions() {
      return this.getGroupedPermissions(
        AppConstants.PermissionCategoryAdditional,
      );
    },

    isFirmAuthorised() {
      return this.selectedFcaFirm?.isAuthorized;
    },
  },
  setup() {
    const pageLifeCycleStore = usePageLifeCycleStore();
    const { changeLifeCycleName } = pageLifeCycleStore;

    const { addComponentValidationValue, clearValidationValuesByPrefix } =
      usePageComponentValidationValueStore();

    const { debounceFunction, setAutoSaveFunctionNotCompletedYet } =
      useAutoSaveStore();

    return {
      changeLifeCycleName,
      addComponentValidationValue,
      clearValidationValuesByPrefix,
      debounceFunction,
      setAutoSaveFunctionNotCompletedYet,
    };
  },
  created() {
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameCreated);
    this.debouncedAutoSaveFunction = this.debounceFunction(this.saveInfoAsync);
  },
  async mounted() {
    this.complianceFirmName =
      await this.appService.getComplianceFirmNameAsync();
    await this.setupData();
    this.eventBus.emit(AppConstants.bottomBarEnableEvent, true);
    this.eventBusFormSaved.on(AppConstants.formSavedEvent, (isAutoNext) => {
      this.handleSave(isAutoNext);
    });
    this.eventBus.on(AppConstants.formFieldPageLevelChangedEvent, () => {
      this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi =
        false;

      this.setAutoSaveFunctionNotCompletedYet();
      this.debouncedAutoSaveFunction();
    });
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameMounted);
  },
  unmounted() {
    this.eventBus.emit(AppConstants.bottomBarEnableEvent, false);
    this.eventBusFormSaved.off(AppConstants.formSavedEvent);
    this.eventBus.off(AppConstants.formFieldPageLevelChangedEvent);
    this.isLoading = false;
  },
  methods: {
    selectTab(event: {selected: number}) {
      this.currentTab = event.selected;
    },

    async setupData() {
      this.isLoading = true;

      this.definedGroupedPermissions =
        await this.fcaService.getDefinedPermissionsAsync();

      this.customer =
        this.appService.getCachedCustomer() ?? new CustomerEntity();
      this.selectedFcaFirm = this.customer.selectedCompany as CompanyEntity;

      this.currentFcaSubPermissions = this.customer
        .currentFcaPermissions as CustomerPermission[];
      this.customerSubPermissions = this.customer
        .customerPermissions as CustomerPermission[];

      const firmReferenceNo = this.selectedFcaFirm?.firmReferenceNo;

      // update permission Profile here
      if (!firmReferenceNo) {
        this.isLoading = false;
        return;
      }

      this.subPermissionsFromFca = await this.fcaService
        .getFirmPermissionsAsync(firmReferenceNo)
        .catch(() => {
          useNotification({
            type: NotificationType.ERROR,
            content: "Something went wrong.",
            interval: 5e3,
          });

          return {
            permissionNames: [] as string[],
            raw: "",
          };
        })
        .finally(() => {
          this.isLoading = false;
        });

      if (
        !this.currentFcaSubPermissions.length ||
        !this.customerSubPermissions.length
      ) {
        this.setCurrentFcaPermissions();
        this.setCustomerSubPermissions();
      }

      this.isLoading = false;
    },

    getGroupedPermissions(groupName: string) {
      return this.parsedPermissions.filter(
        ({ categoryName }) => categoryName === groupName,
      );
    },

    handleSave(isAutoNext: boolean) {
      if (this.isSavingAlertOpened) {
        return;
      }

      useAlert({
        title: this.$t("common-alert-title"),
        content: this.$t("common-alert-content"),
        confirmButtonText: this.$t("common-alert-buttonText"),
        onConfirm: async () => {
          await this.saveInfoAsync(isAutoNext, true);
        },
        onCancel: () => {
          this.isSavingAlertOpened = false;
        },
        onClose: () => {
          this.isSavingAlertOpened = false;
        },
      });

      this.isSavingAlertOpened = true;
    },

    async saveInfoAsync(
      isAutoNext: boolean = false,
      isShowLoader: boolean = false,
    ): Promise<void> {
      this.isLoading = isShowLoader;
      this.isShowSavingText = isShowLoader;
      this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi =
        true;
      this.isSavingAlertOpened = false;
      const updatedCustomer =
        await this.customerService.getCustomerByEmailAsync(
          this.customer?.email ?? "",
        );
      updatedCustomer.currentFcaPermissions = this.currentFcaSubPermissions;
      updatedCustomer.customerPermissions = this.customerSubPermissions;

      await this.customerService
        .saveCustomerAsync(JSON.stringify(updatedCustomer))
        .then(() => {
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
              AppConstants.notificationPopupTimeOut,
            );
          }
        })
        .catch(() => {
          useNotification({
            type: NotificationType.ERROR,
            content: "Something went wrong.",
            interval: 5e3,
          });
        })
        .finally(() => {
          this.isLoading = false;
          this.isShowSavingText = false;
        });
    },

    updatePermission(
      action: string,
      _checked: boolean,
      permissionName: string,
    ) {
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);

      const index = this.customerSubPermissions.findIndex(
        ({ subPermissionName = "", permissionGroupName = "" }) =>
          permissionName.toLowerCase() === subPermissionName.toLowerCase() ||
          permissionName.toLowerCase() === permissionGroupName.toLowerCase(),
      );

      if (index < 0) {
        throw new Error(
          `${permissionName} should not be null in customerSubPermissions`,
        );
      }

      const permission = this.customerSubPermissions[index];

      switch (action) {
        case AppConstants.seekAuthAdd:
          useAlert({
            type: AlertType.ALERT,
            title: "Confirm",
            content: permission.isModified
              ? this.$t("common-alert-permissionConfirmCancelAddText")
              : this.$t(
                  this.isFirmAuthorised
                    ? "common-alert-permissionConfirmAddText"
                    : "common-alert-permissionConfirmAddTextUnAuthorised",
                ),
            confirmButtonText: permission.isModified
              ? this.$t("common-alert-confirmAndCancelRequest")
              : this.$t("common-alert-confirmAndAdd"),
            confirmButtonThemeColor: permission.isModified
              ? "error"
              : "primary",
            onConfirm: () => {
              useNotification({
                type: NotificationType.SUCCESS,
                content: permission.isModified
                  ? this.$t("common-notification-CancelrequestSubmitted")
                  : this.$t("common-notification-requestSubmitted"),
                interval: 2e3,
              });
              permission.state = PermissionStateEnum.Added;
              permission.isModified = !permission.isModified;
            },
          });
          break;
        case AppConstants.seekAuthRemove:
          useAlert({
            type: AlertType.ALERT,
            title: "Confirm",
            content: permission.isModified
              ? this.$t("common-alert-permissionConfirmCancelRemoveText")
              : this.$t("common-alert-permissionConfirmRemoveText"),
            confirmButtonText: permission.isModified
              ? this.$t("common-alert-confirmAndCancelRequest")
              : this.$t("common-alert-confirmAndRemove"),
            confirmButtonThemeColor: "error",
            onConfirm: () => {
              useNotification({
                type: NotificationType.SUCCESS,
                content: permission.isModified
                  ? this.$t("common-notification-CancelrequestSubmitted")
                  : this.$t("common-notification-requestSubmitted"),
                interval: 2e3,
              });
              permission.state = PermissionStateEnum.Removed;
              permission.isModified = !permission.isModified;
            },
          });
          break;
        case AppConstants.seekAuthPending:
          permission.state = PermissionStateEnum.Pending;
          permission.hasPendingApplication = !permission.hasPendingApplication;
          permission.isModified = !permission.isModified;
          break;
        default:
          throw new Error("We don't support this action in updatePermissions");
      }
    },

    setCurrentFcaPermissions() {
      this.currentFcaSubPermissions = this.definedGroupedPermissions.map(
        (permission) => {
          const state = this.getPermissionState(permission);
          return <CustomerPermission>{
            id: permission.id,
            permissionGroupName: permission.permissionGroupName,
            categoryName: permission.categoryName,
            subPermissionName: permission.subPermissionName,
            subPermissionDisplayText: permission.subPermissionDisplayText,
            state: state,
            hasPendingApplication: state === PermissionStateEnum.Added,
            isModified: false,
          };
        },
      );
    },

    setCustomerSubPermissions() {
      this.customerSubPermissions = this.definedGroupedPermissions.map(
        (permission) => {
          const state = this.getPermissionState(permission);
          return <CustomerPermission>{
            id: permission.id,
            permissionGroupName: permission.permissionGroupName,
            categoryName: permission.categoryName,
            subPermissionName: permission.subPermissionName,
            subPermissionDisplayText: permission.subPermissionDisplayText,
            state,
            hasPendingApplication: state === PermissionStateEnum.Added,
            isModified: false,
          };
        },
      );
    },

    getPermissionState(permission: PermissionEdit): PermissionStateEnum {
      const { subPermissionName = "", permissionGroupName = "" } = permission;
      const { permissionNames = [] } = this.subPermissionsFromFca;
      const isFound = permissionNames.find(
        (name) =>
          name.toLowerCase() ===
          (subPermissionName || permissionGroupName).toLowerCase(),
      );
      return isFound ? PermissionStateEnum.Added : PermissionStateEnum.Removed;
    },

    updateProgress(isFull: boolean) {
      const prefix = this.helperService.removeStringSpacesThenSlash(
        AppConstants.permissionProfileRoute,
      );
      const key1 = `${prefix}-placeholder1`;
      const key2 = `${prefix}-placeholder2`;
      this.clearValidationValuesByPrefix(prefix);
      this.addComponentValidationValue(key1, {
        [key1]: "",
      });
      if (!isFull) {
        // Add error to be 50%
        this.addComponentValidationValue(key2, {
          [key2]: "error",
        });
      }
    },
  },
  watch: {
    customerSubPermissions: {
      handler(value: CustomerPermission[]) {
        if (value.some((a) => a.isModified)) {
          this.updateProgress(false);
        } else {
          this.updateProgress(true);
        }
      },
      deep: true,
      immediate: true,
    },
  },
});
</script>

<template src="./permission_profile.html" />

<style scoped src="./permission_profile.css" />