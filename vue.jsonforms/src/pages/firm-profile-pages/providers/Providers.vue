<script lang="ts">
import { defineComponent, inject, defineAsyncComponent } from "vue";
import { v4 as uuidv4 } from "uuid";
import { container } from "tsyringe";
import { ProvidersEntity } from "@/entities/providers-and-introducers/ProvidersEntity";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import {
  ICustomerService,
  ICustomerServiceInfo,
} from "@/infra/dependency-services/rest/forms-compliance/ICustomerService";
import { AppConstants } from "@/infra/AppConstants";
import { Emitter, EventType } from "mitt";
import StaticList from "@/infra/StaticListService";
import {
  IFcaService,
  IFcaServiceInfo,
} from "@/infra/dependency-services/rest/fca/IFcaService";
import { AppointedRepresentative } from "@/entities/appointed-representatives/AppointedRepresentative";
import { ProviderIntroducerDetails } from "@/entities/providers-and-introducers/ProviderIntroducerDetails";
import { MenuItemModel, MenuSelectEvent } from "@progress/kendo-vue-layout";
import { ProvidersTaskDetails } from "@/entities/providers-and-introducers/ProvidersTaskDetails";
import { usePageLifeCycleStore } from "@/stores/progress-bar/usePageLifeCycleStore";
import {
  useNotification,
  NotificationType,
} from "@/composables/useNotification";
import { AlertType, useAlert } from "@/composables/useAlert";
import { useSafeCustomerStore } from "@/stores/useSafeCustomerStore";
import { CustomerEntity } from "@/entities/CustomerEntity";
import { FirmPermissionCategorized } from "@/entities/FirmPermissionCategorized";
import { FirmPermission } from "@/entities/FirmPermission";
import {
  IUserSubmittedChangesService,
  IUserSubmittedChangesServiceInfo,
} from "@/infra/dependency-services/user-submission/IUserSubmittedChangesService";
import {
  IProvidersService,
  IProvidersServiceInfo,
} from "@/infra/dependency-services/rest/providers/IProvidersService";
import { ProfileStatuses } from "@/entities/enums/ProfileStatuses";
import { useCustomerProviderStore } from "@/stores/useCustomerProviderStore";

export default defineComponent({
  name: "Providers",
  components: {
    ProvidersList: defineAsyncComponent(
      () => import("./partials/ProvidersList.vue"),
    ),
    ProvidersCard: defineAsyncComponent(
      () => import("./partials/ProvidersCard.vue"),
    ),
    ViewProviderModal: defineAsyncComponent(
      () => import("./partials/ViewProviderModal.vue"),
    ),
    ViewProviderPermissions: defineAsyncComponent(
      () => import("./partials/ViewProviderPermissions.vue"),
    ),
    ProviderFormModal: defineAsyncComponent(
      () => import("./partials/ProviderFormModal.vue"),
    ),
    TasksModal: defineAsyncComponent(() => import("./partials/TasksModal.vue")),
  },
  data() {
    return {
      isShowProvidersAsList: true,
      isShowProviderViewer: false,
      isShowProviderForm: false,
      isShowTasksViewer: false,
      isShowProviderPermissions: false,
      isSavingAlertOpened: false,
      isEditMode: false,
      providerName: "<Provider Name>",
      isLoading: false,
      isShowSavingText: false,
      pageDescription: "",
      customerService: container.resolve<ICustomerService>(
        ICustomerServiceInfo.name,
      ),
      userSubmittedChangesService:
        container.resolve<IUserSubmittedChangesService>(
          IUserSubmittedChangesServiceInfo.name,
        ),
      providersService: container.resolve<IProvidersService>(
        IProvidersServiceInfo.name,
      ),
      fcaService: container.resolve<IFcaService>(IFcaServiceInfo.name),
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      eventBusFormSaved: inject("$eventBusService") as Emitter<
        Record<EventType, boolean>
      >,
      titles: [] as string[],
      isInitializing: true,
      customerProviders: [] as ProvidersEntity[],
      provider: new ProvidersEntity(),
      providerTasks: [] as ProvidersTaskDetails[],
      permissions: [] as FirmPermission[],
      permissionsCategorized: [] as FirmPermissionCategorized[],

      // Context Menu
      menuItems: [
        {
          text: "View Details",
        },
        {
          text: "Edit Details",
        },
        {
          text: "Archive",
        },
      ] as MenuItemModel[],
      offsetX: 0,
      offsetY: 0,
    };
  },
  computed: {
    firmName(): string {
      return this.customerStore.getCurrentFirmName;
    },
    currentCustomer(): CustomerEntity {
      return this.customerStore.getCurrentCustomer;
    },
  },
  setup() {
    const pageLifeCycleStore = usePageLifeCycleStore();
    const { changeLifeCycleName } = pageLifeCycleStore;

    const providerStore = useCustomerProviderStore();
    const { fetchInitialCustomerProvidersAsync } = providerStore;

    const customerStore = useSafeCustomerStore();

    return {
      changeLifeCycleName,
      customerStore,
      fetchInitialCustomerProvidersAsync,
    };
  },
  created() {
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameCreated);
    this.customerStore.setCustomerLocally();
  },
  async mounted() {
    this.eventBus.emit(AppConstants.bottomBarEnableEvent, true);
    this.eventBusFormSaved.on(AppConstants.formSavedEvent, (isAutoNext) => {
      this.handleAutoNext(isAutoNext);
    });

    this.titles = StaticList.getTitles();
    this.pageDescription = `All Current Providers of ${this.firmName} are listed below. To view Previous Providers please click “View All Providers”.`;
    const customerProviders =
      (await this.fetchInitialCustomerProvidersAsync(
        this.currentCustomer.id,
      )) ?? [];

    for (const provider of customerProviders) {
      provider.representativeName = `${
        provider.representative?.forename ?? ""
      } ${provider.representative?.surname ?? ""}`;

      const options: Intl.DateTimeFormatOptions = {
        year: "numeric",
        month: "2-digit",
        day: "2-digit",
      };

      const date = this.helperService.convertEpochToDateTime(
        provider.startDate ?? 0,
      );
      provider.startDateString =
        date?.toLocaleDateString(undefined, options) ?? "";

      this.customerProviders.push(provider);
    }

    //ToDo. to get back on this
    // // get providers here only if customer has no providers from db
    // if (
    //   this.providersList.length < 1 &&
    //   this.currentCustomer?.firmReferenceNumber
    // ) {
    //   const providers = await this.fcaService.getProvidersAsync(
    //     this.currentCustomer.firmReferenceNumber,
    //   );
    //
    //   if (providers && providers.length > 0) {
    //     this.providersList =
    //       this.mapAppointedRepresentativesToProviders(providers);
    //   }
    // }

    this.isLoading = false;
    this.isInitializing = false;
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameMounted);
  },
  unmounted() {
    this.eventBus.emit(AppConstants.bottomBarEnableEvent, false);
    this.eventBusFormSaved.off(AppConstants.formSavedEvent);
    this.isLoading = false;
  },
  methods: {
    viewProvider(id: string) {
      this.isShowProviderViewer = true;
      this.provider =
        this.customerProviders.find((employee) => employee.id === id) ??
        new ProvidersEntity();

      if (this.provider.details) {
        this.provider.details.contactNumberDisplay = `${
          this.provider.details.contactNumber?.dialCode ?? ""
        } ${this.provider.details.contactNumber?.number ?? ""}`;
      }

      if (this.provider.representative) {
        this.provider.representative.contactNumberDisplay = `${
          this.provider.representative.contactNumber?.dialCode ?? ""
        } ${this.provider.representative.contactNumber?.number ?? ""}`;
      }
    },

    addNewProvider() {
      this.provider = new ProvidersEntity();
      this.isEditMode = false;
      this.showProviderForm();
    },

    editProvider(id: string) {
      this.isEditMode = true;
      this.provider =
        this.customerProviders.find((employee) => employee.id === id) ??
        new ProvidersEntity();
      this.showProviderForm();
    },

    viewEditProvider(id: string) {
      this.provider =
        this.customerProviders.find((employee) => employee.id === id) ??
        new ProvidersEntity();
      this.showProviderForm();
    },

    viewTasks(providerName: string, tasks: ProvidersTaskDetails[]) {
      this.providerName = providerName;
      this.providerTasks = tasks;
      this.isShowTasksViewer = true;
    },

    async getCompanyPermissions(firmRef: string) {
      await this.customerService
        .getCompanyPermissionsAsync(firmRef)
        .then((data) => {
          this.permissions = data;
        });
    },

    async viewProviderPermissions(firmRef: string) {
      this.isShowProviderViewer = false;
      this.isLoading = true;
      await this.getCompanyPermissions(firmRef);
      const categorized: {
        [categoryName: string]: {
          categoryName: string;
          permissions: FirmPermission[];
        };
      } = {};
      for (const item of this.permissions) {
        if (!categorized[item.categoryName]) {
          categorized[item.categoryName] = {
            categoryName: item.categoryName,
            permissions: [],
          };
        }
        categorized[item.categoryName].permissions.push(item);
      }

      // Convert object back to array
      let permissionsCategorizedArray = Object.keys(categorized).map(
        (key) => categorized[key],
      );

      // Custom sorting function
      const customSort = (a: any, b: any) => {
        // Your specific category name that should be last
        const specificCategoryName = "Other Activities";

        if (a.categoryName === specificCategoryName) return 1; // Place specific category name at the end
        if (b.categoryName === specificCategoryName) return -1; // Place specific category name at the end
        // Sort alphabetically for other categories
        return a.categoryName.localeCompare(b.categoryName);
      };

      // Sort the array
      permissionsCategorizedArray.sort(customSort);
      this.permissionsCategorized = permissionsCategorizedArray;

      this.isShowProviderPermissions = true;
      this.isLoading = false;
    },

    handleAutoNext(isAutoNext: boolean) {
      // ToDo. later.
      if (this.isSavingAlertOpened) {
        return;
      }
      useAlert({
        title: this.$t("common-alert-title"),
        content: "Providers is incomplete. Are you sure you wish to proceed?",
        type: AlertType.SAVEDETAILS,
        confirmButtonText: "Proceed",
        saveDetailsText: "Complete Now",
        width: 440,
        onConfirm: async () => {
          // Logic here
          this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi =
            true;
          this.isSavingAlertOpened = false;

          useNotification({
            type: NotificationType.SUCCESS,
            content: this.$t("common-notification-saved"),
            interval: AppConstants.notificationPopupTimeOut,
          });

          if (isAutoNext) {
            setTimeout(() => this.eventBus.emit(AppConstants.autoNextEvent), 0);
          }
        },
        onSaveDetails: () => {
          this.isSavingAlertOpened = false;
        },
      });

      this.isSavingAlertOpened = true;
    },

    async saveOrUpdateProviderAsync(provider: ProvidersEntity) {
      if (!provider) {
        return;
      }

      if (provider && provider.id.length === 0) {
        provider.id = uuidv4();
      }

      provider.customerId = this.currentCustomer.id;
      provider.ddStatus = "Complete";
      provider.status = "Onboarding";
      provider.startDate = this.helperService.getCurrentDateTimeInEpoch();

      useAlert({
        title: this.$t("common-alert-title"),
        content: "Provider is incomplete. Are you sure you wish to proceed?",
        type: AlertType.SAVEDETAILS,
        confirmButtonText: "Proceed",
        saveDetailsText: "Complete Now",
        width: 440,
        onConfirm: async () => {
          //TODO: to change for better handling
          const providerIndex = this.customerProviders.findIndex(
            (p) => p.id === provider.id,
          );

          if (providerIndex > -1) {
            this.customerProviders[providerIndex] = provider;
          } else {
            this.customerProviders.push(provider);
          }

          provider.profileStatus = ProfileStatuses.Full;
          await this.providersService
            .saveOrUpdateProvidersAsync(provider)
            .then(() => {
              this.eventBus.emit(AppConstants.formFieldChangedEvent);
              this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
            });
        },
        onSaveDetails: () => {
          (this.$refs?.formElement as HTMLFormElement)?.$el?.requestSubmit?.();
        },
      });
    },

    showToastMessage(content: string) {
      useNotification({
        type: NotificationType.SUCCESS,
        interval: 4e3,
        content: content,
      });
    },

    requestToComplete(provider: ProvidersEntity) {
      if (!provider) {
        return;
      }

      if (provider.id.length === 0) {
        provider.id = uuidv4();
      }

      useAlert({
        title: this.$t("common-alert-title"),
        content: `Please confirm that you are happy for us to email ${provider.email} to complete their profile.`,
        confirmButtonText: "Confirm & Send Email",
        cancelButtonText: "Cancel",
        type: AlertType.SAVEDETAILS,
        saveDetailsText: "Save Details",
        isShowSaveDetailsButton: true,
        width: 440,
        onConfirm: async () => {
          //TODO: to change for better handling
          const providerIndex = this.customerProviders.findIndex(
            (p) => p.id === provider.id,
          );

          if (providerIndex > -1) {
            this.customerProviders[providerIndex] = provider;
          } else {
            this.customerProviders.push(provider);
          }

          provider.customerId = this.currentCustomer.id;
          provider.profileStatus = ProfileStatuses.Full;
          await this.providersService
            .saveOrUpdateProvidersAsync(provider)
            .then((provider) => {
              this.eventBus.emit(AppConstants.formFieldChangedEvent);
              this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);

              useNotification({
                type: NotificationType.SUCCESS,
                content: `<center>New Provider Added.</center> <br /> We have emailed ${provider.email} to complete their profile.`,
                interval: AppConstants.notificationPopupTimeOut,
              });
            });
        },
        onSaveDetails: () => {
          useNotification({
            type: NotificationType.SUCCESS,
            content:
              "<center>Provider Details Saved.</center> <br /> Please complete the Provider Profile <a style='color: var(--color-primary)' href=''>here</a>.",
            interval: 5e3,
          });
        },
      });
    },

    showProviderForm() {
      this.isShowProviderForm = true;
    },

    closeProviderForm() {
      this.isShowProviderForm = false;
      this.isShowProviderViewer = false;
      this.isShowTasksViewer = false;
      this.isShowProviderPermissions = false;
    },

    mapAppointedRepresentativesToProviders(
      appointedRepresentatives: AppointedRepresentative[],
    ): ProvidersEntity[] {
      const models: ProvidersEntity[] = [];

      for (const representative of appointedRepresentatives) {
        let principalDetails = new ProviderIntroducerDetails();
        principalDetails.name = representative.principalFirmName;
        principalDetails.fcaFirmRefNo = representative.principalFrn;

        const newProvider = new ProvidersEntity();
        newProvider.startDate = representative.effectiveDate;
        newProvider.startDateString = representative.effectiveDateStr ?? "";

        const newProviderDetails = new ProviderIntroducerDetails();
        newProviderDetails.name = representative.name ?? "";
        newProviderDetails.fcaFirmRefNo = representative.frn ?? "";

        newProvider.details = newProviderDetails;
        models.push(newProvider);
      }

      return models;
    },

    open(item: ProvidersEntity, event: PointerEvent) {
      if (!item.isContextMenuOpen) {
        this.customerProviders.forEach((p) => (p.isContextMenuOpen = false));
      }

      this.offsetX = event.clientX + 10;
      this.offsetY = event.clientY + 10;
      item.isContextMenuOpen = !item.isContextMenuOpen;
    },

    onMenuSelect(select: MenuSelectEvent, provider: ProvidersEntity) {
      try {
        const selectedItem = select.item as MenuItemModel;
        const selectedProviderName = provider.details?.name;

        if (selectedItem.text === "Archive") {
          alert(`${selectedProviderName} is archived!`);
          return;
        }
        this.$router.push("/my-providers");
      } finally {
        this.customerProviders.forEach((p) => (p.isContextMenuOpen = false));
      }
    },
  },
});
</script>

<template src="./providers.html" />

<style src="./providers.css" />
