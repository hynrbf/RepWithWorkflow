<script lang="ts">
import { defineAsyncComponent, defineComponent, inject } from "vue";
import { Emitter, EventType } from "mitt";
import { v4 as uuidv4 } from "uuid";
import { AppConstants } from "@/infra/AppConstants";
import { Affiliate } from "@/entities/Affiliate/Affiliate";
import { usePageLifeCycleStore } from "@/stores/progress-bar/usePageLifeCycleStore";
import { container } from "tsyringe";
import {
  IAppService,
  IAppServiceInfo,
} from "@/infra/dependency-services/app/IAppService";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import {
  IFcaService,
  IFcaServiceInfo,
} from "@/infra/dependency-services/rest/fca/IFcaService";
import { FirmBasicInfo } from "@/entities/FirmBasicInfo";
import {
  useNotification,
  NotificationType,
} from "@/composables/useNotification";
import { AffiliateDetails } from "@/entities/Affiliate/AffiliateDetails";
import { useAlert, AlertType } from "@/composables/useAlert";
import {
  IAppointedRepresentativeService,
  IAppointedRepresentativeServiceInfo,
} from "@/infra/dependency-services/rest/appointed-representative/IAppointedRepresentativeService";
import {AppointedRepresentative} from "@/entities/appointed-representatives/AppointedRepresentative";
import {
  ICustomerService,
  ICustomerServiceInfo,
} from "@/infra/dependency-services/rest/forms-compliance/ICustomerService";
import { FirmPermission } from "@/entities/FirmPermission";
import { FirmPermissionCategorized } from "@/entities/FirmPermissionCategorized";
import { IUserSubmittedChangesService, IUserSubmittedChangesServiceInfo } from "@/infra/dependency-services/user-submission/IUserSubmittedChangesService";

export default defineComponent({
  name: "ARAffiliates",
  computed: {
    AppConstants() {
      return AppConstants;
    },
  },
  components: {
    AffiliatesList: defineAsyncComponent(
      () => import("./Partial/ARAffiliatesList.vue"),
    ),
    AffiliatesCard: defineAsyncComponent(
      () => import("./Partial/ARAffiliatesCard.vue"),
    ),
    AddAffiliatesModal: defineAsyncComponent(
      () => import("./Partial/ARAddAffiliatesModal.vue"),
    ),
    ViewAffiliateModal: defineAsyncComponent(
      () => import("./Partial/ARViewAffiliateModal.vue"),
    ),
    ViewAffiliatePermissions: defineAsyncComponent(
      () => import("./Partial/ARViewAffiliatePermissions.vue"),
    ),
  },
  data() {
    return {
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      eventBusFormSaved: inject("$eventBusService") as Emitter<
        Record<EventType, boolean>
      >,      
      currentTab: 0,
      isLoading: false,
      isShowSavingText: false,
      firmName: "<Firm Name>",
      tabs: [
        { title: "Current", content: "current" },
        { title: "All", content: "all" },
      ],
      isShowAffiliatesList: true,
      isShowAffiliatesCard: false,
      isShowAffiliatePermissions: false,
      isAdd: true,
      isInitializing: true,
      isShowAffiliateViewer: false,
      isSavingAlertOpened: false,
      arCustomer: new AppointedRepresentative(),
      affiliate: new Affiliate(),
      affiliateList: [] as Affiliate[],
      affiliateInputFirm: new FirmBasicInfo(),
      isAffiliateModalVisible: false,
      appService: container.resolve<IAppService>(IAppServiceInfo.name),
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      fcaService: container.resolve<IFcaService>(IFcaServiceInfo.name),
      customerArService: container.resolve<IAppointedRepresentativeService>(
        IAppointedRepresentativeServiceInfo.name,
      ),
      customerService: container.resolve<ICustomerService>(
        ICustomerServiceInfo.name,
      ),
      permissions: [] as FirmPermission[],
      permissionsCategorized: [] as FirmPermissionCategorized[],
      userSubmittedChangesService: container.resolve<IUserSubmittedChangesService>(
        IUserSubmittedChangesServiceInfo.name,
      ),
    };
  },
  created() {
    this.arCustomer =
      this.appService.getCachedCustomerAppointedRepresentative() ??
      new AppointedRepresentative();    
    this.affiliateList = this.arCustomer.affiliates ?? [];
    this.isInitializing = false;
    this.firmName = this.arCustomer.companyName ?? "";
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameCreated);
  },
  mounted() {
    this.eventBus.emit(AppConstants.bottomBarEnableEvent, true);
    this.eventBusFormSaved.on(AppConstants.formSavedEvent, (isAutoNext) => 
      this.handleAutoNext(isAutoNext),
    );
    this.arCustomer =
      this.appService.getCachedCustomerAppointedRepresentative() ??
      new AppointedRepresentative();
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

    return {
      changeLifeCycleName,
    };
  },
  methods: {
    closeForm() {
      this.isAffiliateModalVisible = false;
      this.isShowAffiliateViewer = false;
      this.isShowAffiliatePermissions = false;
    },

    showAffiliateForm() {
      this.isAffiliateModalVisible = true;
    },

    async onAffiliatesDetailUpdated(affiliate: FirmBasicInfo) {
      if (!this.affiliate) {
        return;
      }

      this.affiliate.details.name = affiliate.firmName;
      this.affiliate.details.companyNumber = affiliate.companyNumber;
      this.affiliate.details.firmReferenceNumber =
        affiliate.firmReferenceNumber;
      this.affiliateInputFirm = affiliate;
      this.affiliate.details.registeredAddress = affiliate.address;
    },

    isCompanyExisting(
      affiliateList: Affiliate[],
      detailsToCheck: AffiliateDetails,
    ): boolean {
      return affiliateList.some((affiliate) => {
        const existingDetails = affiliate.details;
        return (
          existingDetails.name === detailsToCheck.name &&
          existingDetails.companyNumber === detailsToCheck.companyNumber &&
          existingDetails.firmReferenceNumber ===
            detailsToCheck.firmReferenceNumber
        );
      });
    },

    viewAffiliate(id: string) {
      this.isShowAffiliateViewer = true;
      this.affiliate =
        this.affiliateList.find((employee) => employee.id === id) ??
        new Affiliate();

      if (this.affiliate.details) {
        this.affiliate.details.contactNumberDisplay = `${
          this.affiliate.details.contactNumber?.dialCode ?? ""
        } ${this.affiliate.details.contactNumber?.number ?? ""}`;
      }

      if (this.affiliate.representative) {
        this.affiliate.representative.contactNumberDisplay = `${
          this.affiliate.representative.contactNumber?.dialCode ?? ""
        } ${this.affiliate.representative.contactNumber?.number ?? ""}`;
      }
    },

    viewEditAffiliate(id: string) {
      this.isShowAffiliateViewer = false;
      this.affiliate =
        this.affiliateList.find((employee) => employee.id === id) ??
        new Affiliate();
      this.showAffiliateForm();
    },

    async viewAffiliatePermissions(firmRef: string) {
      this.isShowAffiliateViewer = false;
      this.isLoading = true;
      await this.getCompanyPermissions(firmRef);
      const categorized: { [categoryName: string]: { categoryName: string, permissions: FirmPermission[] } } = {};
      for (const item of this.permissions) {
        if (!categorized[item.categoryName]) {
          categorized[item.categoryName] = {
            categoryName: item.categoryName,
            permissions: []
          };
        }
        categorized[item.categoryName].permissions.push(item);
      }      

      // Convert object back to array
      let permissionsCategorizedArray =  Object.keys(categorized).map(key => categorized[key]);

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
      
      this.isShowAffiliatePermissions = true;
      this.isLoading = false;
    },

    addNewAffiliate() {
      this.isAdd = true;
      this.affiliate = new Affiliate();
      this.showAffiliateForm();
    },

    editAffiliate(id: string) {
      this.isAdd = false;
      this.affiliate =
        this.affiliateList.find((employee) => employee.id === id) ??
        new Affiliate();
      this.showAffiliateForm();
      //this.isShowIntroducerViewer = false;
    },

    async getCompanyPermissions(firmRef: string) {
      await this.customerService
        .getCompanyPermissionsAsync(firmRef)
        .then((data) => {
          this.permissions = data;
        });
    },

    async saveChanges() {
      const updatedCustomer =
        await this.customerArService.getAppointedRepresentativesByEmailAsync(
          this.arCustomer.email ?? "",
        );
      this.isShowSavingText = true;
      this.isLoading = true;

      for (let affiliate of this.affiliateList) {
        affiliate.id = uuidv4();
      }

      updatedCustomer.affiliates = this.affiliateList;
      this.closeForm();
      await this.customerArService
        .saveOrUpdateAppointedRepresentativeAsync(updatedCustomer)
        .then(() => {
          this.isShowSavingText = false;
          this.isLoading = false;
          this.showModal(
            this.isAdd
              ? "<center>Affiliate Details Saved.</center> <br /> Please complete the Affiliate Profile <a style='color: var(--color-primary)' href=''>here</a>."
              : "Changes Saved.",
          );
        });
    },

    async saveAffiliateDetailsAsync(affiliate: Affiliate) {
      // Below are temporary values for now
      affiliate.ddStatus = "Complete";
      affiliate.statusImg = "/bookmark_green.svg";
      affiliate.providerImg = "/add_profile_photo.png";
      affiliate.status = "Onboarding";

      useAlert({
        title: this.$t("common-alert-title"),
        content: "Affiliate is incomplete. Are you sure you wish to proceed?",
        type: AlertType.SAVEDETAILS,
        confirmButtonText: "Proceed",
        saveDetailsText: "Complete Now",
        width: 440,
        onConfirm: async () => {
          if (!this.isCompanyExisting(this.affiliateList, affiliate.details)) {
            this.affiliateList.push(affiliate);
          }
          await this.saveChanges();
        },
        onSaveDetails: () => {
          this.triggerSubmit();
        },
      });

      this.affiliate = new Affiliate();
      //this.eventBus.emit(AppConstants.formFieldChangedEvent);
      //this.eventBus.emit(AppConstants.formFieldChangedEventForSaving);
    },

    handleAutoNext(isAutoNext: boolean) {
      // ToDo. later.
      if (this.isSavingAlertOpened) {
        return;
      }
      useAlert({
        title: this.$t("common-alert-title"),
        content: "Affiliates is incomplete. Are you sure you wish to proceed?",
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

    triggerSubmit() {
      (this.$refs.affiliateFormModal as any).triggerSubmit();
    },

    showModal(content: string) {
      useNotification({
        type: NotificationType.SUCCESS,
        interval: 10000,
        content: content,
      });
    },

    requestToComplete(affiliate: Affiliate) {
      if (!affiliate) {
        return;
      }

      const affiliateName = "Affiliate Name";
      useAlert({
        title: this.$t("common-alert-title"),
        content: `Please confirm that you are happy for us to email ${affiliateName} to complete their profile.`,
        confirmButtonText: "Confirm & Send Email",
        cancelButtonText: "Cancel",
        type: AlertType.SAVEDETAILS,
        saveDetailsText: "Save Details",
        isShowSaveDetailsButton: true,
        width: 440,
        onConfirm: async () => {
          useNotification({
            type: NotificationType.SUCCESS,
            content: `<center>New Affiliate Added.</center> <br /> We have emailed ${affiliateName} to complete their profile.`,
            interval: AppConstants.notificationPopupTimeOut,
          });
        },
        onSaveDetails: async () => {
          await this.saveAffiliateDetailsAsync(affiliate);
          useNotification({
            type: NotificationType.SUCCESS,
            content:
              "<div class='text-center'>Affiliate Details Saved.</div> <br /> Please complete the Affiliate Profile <a style='color: var(--color-primary)' href=''>here</a>.",
            interval: 5e3,
          });
        },
      });
    },
  },
});
</script>

<template src="./ar-affiliates.html" />

<style scoped src="./ar-affiliates.css" />
