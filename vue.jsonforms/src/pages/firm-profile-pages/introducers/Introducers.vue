<script lang="ts">
import { defineComponent, inject } from "vue";
import { container } from "tsyringe";
import { v4 as uuidv4 } from "uuid";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import {
  ICustomerService,
  ICustomerServiceInfo,
} from "@/infra/dependency-services/rest/forms-compliance/ICustomerService";
import {
  IFcaService,
  IFcaServiceInfo,
} from "@/infra/dependency-services/rest/fca/IFcaService";
import { CustomerEntity } from "@/entities/CustomerEntity";
import { AppConstants } from "@/infra/AppConstants";
import { Emitter, EventType } from "mitt";
import { KendoFlexibleDialogComponent } from "@/components/KendoFlexibleDialog.vue";
import { IntroducersEntity } from "@/entities/providers-and-introducers/IntroducersEntity";
import { Popup } from "@progress/kendo-vue-popup";
import { IntroducerReferalProduct } from "@/entities/providers-and-introducers/IntroducerReferalProduct";
import { FirmBasicInfo } from "@/entities/FirmBasicInfo";
import {
  IAppService,
  IAppServiceInfo,
} from "@/infra/dependency-services/app/IAppService";
import {
  NotificationType,
  useNotification,
} from "@/composables/useNotification";
import { ContactNumber } from "@/entities/ContactNumber";
import IntroducerList from "./partials/IntroducerList.vue";
import IntroducerCard from "./partials/IntroducerCard.vue";
import ViewIntroducerModal from "./partials/ViewIntroducerModal.vue";
import IntroducerFormModal from "./partials/IntroducerFormModal.vue";
import ViewIntroducerPermissions from "./partials/ViewIntroducerPermissions.vue";
import { usePageLifeCycleStore } from "@/stores/progress-bar/usePageLifeCycleStore";
import { ProviderIntroducerDetails } from "@/entities/providers-and-introducers/ProviderIntroducerDetails";
import { useAlert, AlertType } from "@/composables/useAlert";
import { FirmPermission } from "@/entities/FirmPermission";
import { FirmPermissionCategorized } from "@/entities/FirmPermissionCategorized";
import {
  IIntroducersService,
  IIntroducersServiceInfo,
} from "@/infra/dependency-services/rest/introducers/IIntroducersService";
import { ProfileStatuses } from "@/entities/enums/ProfileStatuses";
import { useCustomerIntroducerStore } from "@/stores/useCustomerIntroducerStore";

export default defineComponent({
  name: "Introducers",
  components: {
    Popup,
    IntroducerList,
    IntroducerCard,
    ViewIntroducerModal,
    IntroducerFormModal,
    ViewIntroducerPermissions,
  },
  data() {
    return {
      isShowIntroducerAsList: true,
      isShowIntroducerViewer: false,
      isShowIntroducerForm: false,
      isShowTasksViewer: false,
      isShowIntroducerPermissions: false,
      currentTab: 0,
      firmName: "<Firm Name>",
      isLoading: true,
      isShowSavingText: false,
      pageDescription: "",
      isGrid: true,
      isAdd: true,
      customerService: container.resolve<ICustomerService>(
        ICustomerServiceInfo.name,
      ),
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      appService: container.resolve<IAppService>(IAppServiceInfo.name),
      fcaService: container.resolve<IFcaService>(IFcaServiceInfo.name),
      customer: new CustomerEntity(),
      introducersService: container.resolve<IIntroducersService>(
        IIntroducersServiceInfo.name,
      ),
      customerIntroducers: [] as IntroducersEntity[],
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      eventBusFormSaved: inject("$eventBusService") as Emitter<
        Record<EventType, boolean>
      >,
      kendoSaveIntroducersDialogInstance:
        null as KendoFlexibleDialogComponent | null,
      isInitializing: true,
      introducer: new IntroducersEntity(),
      isCompany: undefined as boolean | undefined,
      introducerInputFirm: new FirmBasicInfo(),
      introducerContactNumber: new ContactNumber(),
      permissions: [] as FirmPermission[],
      permissionsCategorized: [] as FirmPermissionCategorized[],
    };
  },
  computed: {
    AppConstants() {
      return AppConstants;
    },
    columns() {
      return [
        {
          children: [
            {
              field: "details.name",
              title: "INTRODUCER NAME",
              width: "200px",
              resizable: true,
            },
            {
              field: "fcaAuthorisationStatus",
              title: "FCA AUTHORISATION STATUS",
              width: "250px",
              resizable: true,
            },
            {
              field: "introducerName",
              title: "INTRODUCER REPRESENTATIVE",
              width: "300px",
              resizable: true,
              // TO.DO - This is for image
              //cell: 'contactTemplate',
            },
            {
              field: "startDateStr",
              title: "START DATE",
              width: "200px",
              resizable: true,
            },
            {
              field: "status",
              title: "STATUS",
              width: "200px",
              resizable: true,
            },
            {
              field: "products",
              title: "PRODUCTS",
              width: "200px",
              resizable: true,
              cell: "referralProductsTemplate",
            },
            {
              field: "ddStatus",
              title: "DD STATUS",
              width: "200px",
              resizable: true,
              cell: "ddStatusTemplate",
            },
          ],
        },
      ];
    },
    introducerName(): string {
      if (!this.introducer?.details?.name) {
        return "<Introducer Name>";
      }

      return this.introducer.details.name;
    },
  },
  async created() {
    this.firmName = this.appService.getCustomerFirmName();
    this.pageDescription = `Please provide below, details of all introducers that introduce business to ${this.firmName}.`;
    this.isInitializing = false;
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameCreated);
  },
  async mounted() {
    this.customer = this.appService.getCachedCustomer() ?? new CustomerEntity();
    const customerIntroducers =
      (await this.fetchInitialCustomerIntroducersAsync(this.customer.id)) ?? [];

    for (const introducer of customerIntroducers) {
      introducer.introducerName = `${introducer.representative?.forename} ${introducer.representative?.surname}`;
      const options: Intl.DateTimeFormatOptions = {
        year: "numeric",
        month: "2-digit",
        day: "2-digit",
      };
      const date = this.helperService.convertEpochToDateTime(
        introducer.startDate ?? 0,
      );
      introducer.startDateStr =
        date?.toLocaleDateString(undefined, options) ?? "";
      this.customerIntroducers.push(introducer);
    }

    this.isLoading = false;
    this.eventBus.emit(AppConstants.bottomBarEnableEvent, true);

    if (this.$refs.kendoAddIntroducersDialog) {
      this.kendoSaveIntroducersDialogInstance = this.$refs
        .kendoAddIntroducersDialog as KendoFlexibleDialogComponent;
    }

    this.changeLifeCycleName(AppConstants.pageLifeCycleNameMounted);
  },
  unmounted() {
    this.eventBus.emit(AppConstants.bottomBarEnableEvent, false);
    this.eventBusFormSaved.off(AppConstants.formSavedEvent);
    this.isLoading = false;
  },
  setup() {
    const pageLifeCycleStore = usePageLifeCycleStore();
    const { changeLifeCycleName } = pageLifeCycleStore;
    const providerStore = useCustomerIntroducerStore();
    const { fetchInitialCustomerIntroducersAsync } = providerStore;

    return {
      changeLifeCycleName,
      fetchInitialCustomerIntroducersAsync,
    };
  },
  methods: {
    showModal(content: string) {
      useNotification({
        type: NotificationType.SUCCESS,
        interval: 4e3,
        content: content,
      });
    },

    toggleViewType() {
      this.isGrid = !this.isGrid;
    },

    viewAllIntroducers() {
      alert("TODO. view all introducers");
    },

    addIntroducer() {
      this.isAdd = true;
      this.introducerInputFirm = new FirmBasicInfo();
      this.introducer = new IntroducersEntity();
      this.isCompany = undefined;
      this.showIntroducerForm();
    },

    editIntroducer(id: string) {
      this.isAdd = false;
      this.introducer =
        this.customerIntroducers.find((introducer) => introducer.id === id) ??
        new IntroducersEntity();
      this.isCompany = this.introducer.details.isCompany;
      this.showIntroducerForm();
      this.isShowIntroducerViewer = false;
    },

    showIntroducerForm() {
      this.isShowIntroducerForm = true;
    },

    closeForm() {
      this.isShowIntroducerForm = false;
      this.isShowIntroducerViewer = false;
      this.isShowIntroducerPermissions = false;
    },

    isCompanyExisting(
      introducersList: IntroducersEntity[],
      detailsToCheck: ProviderIntroducerDetails,
    ): boolean {
      return introducersList.some((introducer) => {
        const existingDetails = introducer.details;
        return (
          existingDetails.name === detailsToCheck.name &&
          existingDetails.companyNumber === detailsToCheck.companyNumber &&
          existingDetails.fcaFirmRefNo === detailsToCheck.fcaFirmRefNo
        );
      });
    },

    updateObject<T>(original: T, changes: Partial<T>): T {
      return { ...original, ...changes };
    },

    async viewIntroducerPermissions(firmRef: string) {
      this.isLoading = true;
      this.isShowIntroducerPermissions = false;
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

      this.isShowIntroducerPermissions = true;
      this.isLoading = false;
    },

    async getCompanyPermissions(firmRef: string) {
      await this.customerService
        .getCompanyPermissionsAsync(firmRef)
        .then((data) => {
          this.permissions = data;
        });
    },

    async saveIntroducersDetailsAsync(introducer: IntroducersEntity) {
      if (!introducer) {
        return;
      }

      const introducerIndex = this.customerIntroducers.findIndex(
        (p) =>
          p.id === introducer.id ||
          this.isCompanyExisting(this.customerIntroducers, introducer.details),
      );

      if (introducerIndex > -1) {
        introducer.id = this.customerIntroducers[introducerIndex].id;
      } else {
        introducer.id = uuidv4();
      }

      const options: Intl.DateTimeFormatOptions = {
        year: "numeric",
        month: "2-digit",
        day: "2-digit",
      };
      const date = this.helperService.convertEpochToDateTime(
        introducer.startDate ?? 0,
      );
      introducer.startDateStr =
        date?.toLocaleDateString(undefined, options) ?? "";
      introducer.introducerName = `${introducer.representative.forename} ${introducer.representative.surname}`;
      //this.introducer.details.contactNumber = `+44 ${this.helperService.cleanContactNumberOnLoad(this.introducer.details.contactNumber)}`;
      // Below are temporary values for now
      introducer.ddStatus = "Complete";
      introducer.statusImg = "/bookmark_green.svg";
      introducer.providerImg = "/add_profile_photo.png";
      introducer.status = "Onboarding";
      introducer.referral = new IntroducerReferalProduct();
      introducer.fcaAuthorisationStatus = "Authorised";
      introducer.customerId = this.customer.id;
      introducer.profileStatus = ProfileStatuses.Full;

      useAlert({
        title: this.$t("common-alert-title"),
        content: "Introducer is incomplete. Are you sure you wish to proceed?",
        type: AlertType.SAVEDETAILS,
        confirmButtonText: "Proceed",
        saveDetailsText: "Complete Now",
        width: 440,
        onConfirm: async () => {
          introducer.customerId = this.customer.id;
          introducer.profileStatus = ProfileStatuses.Full;
          await this.introducersService
            .saveOrUpdateIntroducersAsync(introducer)
            .then(() => {
              if (introducerIndex > -1) {
                this.customerIntroducers[introducerIndex] = introducer;
              } else {
                this.customerIntroducers.push(introducer);
              }

              this.closeForm();
              this.eventBus.emit(AppConstants.formFieldChangedEvent);
              this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
            });
        },
        onSaveDetails: () => {
          (this.$refs.introducerFormModal as any).triggerSubmit();
        },
      });

      this.kendoSaveIntroducersDialogInstance?.closeActionDialog();
      this.introducer = new IntroducersEntity();
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    },

    requestToComplete(introducer: IntroducersEntity) {
      if (!introducer) {
        return;
      }

      const introducerIndex = this.customerIntroducers.findIndex(
        (p) =>
          p.id === introducer.id ||
          this.isCompanyExisting(this.customerIntroducers, introducer.details),
      );

      if (introducerIndex > -1) {
        introducer.id = this.customerIntroducers[introducerIndex].id;
      } else {
        introducer.id = uuidv4();
      }

      useAlert({
        title: this.$t("common-alert-title"),
        content: `Please confirm that you are happy for us to email ${introducer.email} to complete their profile.`,
        confirmButtonText: "Confirm & Send Email",
        cancelButtonText: "Cancel",
        type: AlertType.SAVEDETAILS,
        saveDetailsText: "Save Details",
        isShowSaveDetailsButton: true,
        width: 440,
        onConfirm: async () => {
          //TODO: to change for better handling
          introducer.customerId = this.customer.id;
          introducer.profileStatus = ProfileStatuses.Full;
          introducer.status = "Onboarding";
          await this.introducersService
            .saveOrUpdateIntroducersAsync(introducer)
            .then((introducer) => {
              if (introducerIndex > -1) {
                this.customerIntroducers[introducerIndex] = introducer;
              } else {
                this.customerIntroducers.push(introducer);
              }

              this.closeForm();
              this.eventBus.emit(AppConstants.formFieldChangedEvent);
              this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
              useNotification({
                type: NotificationType.SUCCESS,
                content: `<center>New Introducer Added.</center> <br /> We have emailed ${introducer.email} to complete their profile.`,
                interval: AppConstants.notificationPopupTimeOut,
              });
            });
        },
        onSaveDetails: () => {
          useNotification({
            type: NotificationType.SUCCESS,
            content:
              "<center>Introducer Details Saved.</center> <br /> Please complete the Introducer Profile <a style='color: var(--color-primary)' href=''>here</a>.",
            interval: 5e3,
          });
        },
      });

      this.kendoSaveIntroducersDialogInstance?.closeActionDialog();
      this.introducer = new IntroducersEntity();
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    },

    displayFormattedContactNumber(contactNumber: string): string {
      // TEMP. to get back later formatting
      if (!contactNumber) {
        return "";
      }
      return `+44 ${this.helperService.cleanContactNumber(contactNumber)}`;
    },

    async viewIntroducer(id: string) {
      this.isShowIntroducerViewer = true;
      this.introducer =
        this.customerIntroducers.find((introducer) => introducer.id === id) ??
        new IntroducersEntity();
      if (this.introducer.details !== undefined) {
        this.introducer.details.contactNumberDisplay = `${
          this.introducer.details.contactNumber?.dialCode ?? ""
        } ${this.introducer.details.contactNumber?.number ?? ""}`;
      }
      if (this.introducer.representative !== undefined) {
        this.introducer.representative.contactNumberDisplay = `${
          this.introducer.representative.contactNumber?.dialCode ?? ""
        } ${this.introducer.representative.contactNumber?.number ?? ""}`;
      }
    },
  },
});
</script>

<template src="./introducers.html" />

<style scoped src="./introducers.css" />
