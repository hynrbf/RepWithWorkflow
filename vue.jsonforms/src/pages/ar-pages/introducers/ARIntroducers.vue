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
  IAppointedRepresentativeService,
  IAppointedRepresentativeServiceInfo,
} from "@/infra/dependency-services/rest/appointed-representative/IAppointedRepresentativeService";
import { AppointedRepresentative } from "@/entities/appointed-representatives/AppointedRepresentative";
import {
  IFcaService,
  IFcaServiceInfo,
} from "@/infra/dependency-services/rest/fca/IFcaService";
import { AppConstants } from "@/infra/AppConstants";
import { Emitter, EventType } from "mitt";
import { KendoFlexibleDialogComponent } from "@/components/KendoFlexibleDialog.vue";
import { IntroducersEntity } from "@/entities/providers-and-introducers/IntroducersEntity";
import { MenuItemModel, MenuSelectEvent } from "@progress/kendo-vue-layout";
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
import IntroducerList from "./partials/ARIntroducerList.vue";
import ViewIntroducerModal from "./partials/ARViewIntroducerModal.vue";
import IntroducerFormModal from "./partials/ARIntroducerFormModal.vue";
import ViewIntroducerPermissions from "./partials/ARViewIntroducerPermissions.vue";
import { usePageLifeCycleStore } from "@/stores/progress-bar/usePageLifeCycleStore";
import { ProviderIntroducerDetails } from "@/entities/providers-and-introducers/ProviderIntroducerDetails";
import { useAlert, AlertType } from "@/composables/useAlert";
import { FirmPermission } from "@/entities/FirmPermission";
import { FirmPermissionCategorized } from "@/entities/FirmPermissionCategorized";

export default defineComponent({
  name: "ARIntroducers",
  components: {
    IntroducerList,
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
      tabs: [
        { title: "Current", content: "current" },
        { title: "All", content: "all" },
      ],
      firmName: "<Firm Name>",
      isLoading: true,
      isShowSavingText: false,
      pageDescription: "",
      isGrid: true,
      isAdd: true,
      customerArService: container.resolve<IAppointedRepresentativeService>(
        IAppointedRepresentativeServiceInfo.name,
      ),
      customerService: container.resolve<ICustomerService>(
        ICustomerServiceInfo.name,
      ),
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      appService: container.resolve<IAppService>(IAppServiceInfo.name),
      fcaService: container.resolve<IFcaService>(IFcaServiceInfo.name),
      arCustomer: new AppointedRepresentative(),
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      eventBusFormSaved: inject("$eventBusService") as Emitter<
        Record<EventType, boolean>
      >,
      kendoSaveIntroducersDialogInstance:
        null as KendoFlexibleDialogComponent | null,
      isInitializing: true,
      introducersList: [] as IntroducersEntity[],
      introducer: new IntroducersEntity(),
      isIntroducerAddressesTheSame: false,
      isCompany: undefined as boolean | undefined,
      introducerInputFirm: new FirmBasicInfo(),
      introducerContactNumber: new ContactNumber(),
      permissions: [] as FirmPermission[],
      permissionsCategorized: [] as FirmPermissionCategorized[],
      //Grid
      columnMenu: true,
      selectedField: "selected",
      gridPageable: {
        buttonCount: 5,
        info: true,
        type: "numeric",
        pageSizes: true,
        previousNext: true,
      },
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
    this.arCustomer =
      this.appService.getCachedCustomerAppointedRepresentative() ??
      new AppointedRepresentative();
    this.introducersList = this.arCustomer.introducers ?? [];

    for (const provider of this.introducersList) {
      provider.introducerName = `${provider.representative?.forename} ${provider.representative?.surname}`;

      const options: Intl.DateTimeFormatOptions = {
        year: "numeric",
        month: "2-digit",
        day: "2-digit",
      };

      const date = this.helperService.convertEpochToDateTime(
        provider.startDate ?? 0,
      );
      provider.startDateStr =
        date?.toLocaleDateString(undefined, options) ?? "";
    }
    
    this.isInitializing = false;
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameCreated);
  },
  mounted() {
    this.isLoading = false;
    this.eventBus.emit(AppConstants.bottomBarEnableEvent, true);
    this.eventBusFormSaved.on(AppConstants.formSavedEvent, (isAutoNext) => {
      this.saveChanges(isAutoNext);
    });

    if (this.$refs.kendoAddIntroducersDialog) {
      this.kendoSaveIntroducersDialogInstance = this.$refs
        .kendoAddIntroducersDialog as KendoFlexibleDialogComponent;
    }

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
    async saveChanges(_isAutoNext: boolean = false) {
      const updatedCustomer =
        await this.customerArService.getAppointedRepresentativesByEmailAsync(
          this.arCustomer.email ?? "",
        );
      updatedCustomer.introducers = this.introducersList;
      this.isShowSavingText = true;
      this.isLoading = true;

      for (let introducer of this.introducersList) {
        introducer.id = uuidv4();
        introducer.details.contactNumber =
          await this.helperService.convertToContactNoAsync(
            introducer.details.contactNumber?.number ?? "",
            introducer.details.contactNumber?.country ?? "",
          );
        introducer.representative.contactNumber =
          await this.helperService.convertToContactNoAsync(
            introducer.representative.contactNumber?.number ?? "",
            introducer.representative.contactNumber?.country ?? "",
          );
      }

      const content = this.isAdd
        ? "<div class='text-center'>Introducer Details Saved.</div> <br /> Please complete the Introducer Profile <a style='color: var(--color-primary)' href=''>here</a>."
        : "Changes Saved.";

      this.closeForm();
      await this.customerArService
        .saveOrUpdateAppointedRepresentativeAsync(updatedCustomer)
        .then(() => {
          this.isShowSavingText = false;
          this.isLoading = false;
          this.showModal(content);
        });
    },

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
        this.introducersList.find((employee) => employee.id === id) ??
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
      if (!introducer.details.isCompany) {
        introducer.details.name = `${introducer.details.foreName} ${introducer.details.lastName}`;
        introducer.representative.notApplicable = true;
      }
      this.introducer.details.contactNumber =
        await this.helperService.convertToContactNoAsync(
          introducer.details.contactNumber?.number as string,
          introducer.details.contactNumber?.country as string,
        );

      useAlert({
        title: this.$t("common-alert-title"),
        content: "Introducer is incomplete. Are you sure you wish to proceed?",
        type: AlertType.SAVEDETAILS,
        confirmButtonText: "Proceed",
        saveDetailsText: "Complete Now",
        width: 440,
        onConfirm: async () => {
          if (
            !this.isCompanyExisting(this.introducersList, introducer.details)
          ) {
            this.introducersList.push(introducer);
          }
          await this.saveChanges();
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

    onToggleTradingAddress() {
      this.isIntroducerAddressesTheSame = !this.isIntroducerAddressesTheSame;

      if (this.isIntroducerAddressesTheSame) {
        this.introducer.details.tradingAddress =
          this.introducer.details.registeredAddress ?? "";
      } else {
        this.introducer.details.tradingAddress = "";
      }
    },

    onViewOptions() {
      alert("To do later.");
    },

    displayFormattedContactNumber(contactNumber: string): string {
      // TEMP. to get back later formatting
      if (!contactNumber) {
        return "";
      }
      return `+44 ${this.helperService.cleanContactNumber(contactNumber)}`;
    },

    open(item: IntroducersEntity, event: PointerEvent) {
      if (!item.isContextMenuOpen) {
        this.introducersList.forEach((i) => (i.isContextMenuOpen = false));
      }

      this.offsetX = event.clientX + 10;
      this.offsetY = event.clientY + 10;
      item.isContextMenuOpen = !item.isContextMenuOpen;
    },

    onMenuSelect(select: MenuSelectEvent, introducer: IntroducersEntity) {
      try {
        const selectedItem = select.item as MenuItemModel;
        const selectedIntroducerName = introducer.details?.name;

        if (selectedItem.text === "Archive") {
          alert(`${selectedIntroducerName} is archived!`);
          return;
        }

        localStorage.setItem(
          AppConstants.contextMenuSelectionIntroducerKey,
          JSON.stringify(this.cleanIntroducersBeforeSavingLocally(introducer)),
        );
        this.$router.push("/introducer-details");
      } finally {
        this.introducersList.forEach((i) => (i.isContextMenuOpen = false));
      }
    },

    async onIntroducerDetailUpdated(introducer: FirmBasicInfo) {
      if (!this.introducer) {
        return;
      }

      this.introducer.details.name = introducer.firmName;
      this.introducer.details.companyNumber = introducer.companyNumber;
      this.introducer.details.fcaFirmRefNo = introducer.firmReferenceNumber;
      this.introducerInputFirm = introducer;

      this.introducer.details.registeredAddress = introducer.address;
      await this.getFCADetails();
    },

    async getFCADetails() {
      const addressFromFca = await this.fcaService.getFirmAddressesDetailsAsync(
        this.introducer.details.fcaFirmRefNo as string,
        "PPOB",
      );

      if (addressFromFca?.length > 0) {
        const selectedAddress = addressFromFca[0];
        const obj = JSON.parse(JSON.stringify(selectedAddress));
        this.introducerContactNumber = {
          dialCode: obj["dialCode"],
          country: obj["country"],
          countryCode: obj["countryCode"],
          number: obj["Phone Number"],
        };
        const formattedNumber = obj["Phone Number"].substring(3);
        this.introducer.details.contactNumber = formattedNumber;

        this.introducer.details.website = obj["Website Address"];
        // build address
        let address = "";
        address += `${obj["Address Line 1"] ? obj["Address Line 1"] : ""}`;
        address += `${
          obj["Address Line 2"] ? `, ${obj["Address Line 2"]}` : ""
        }`;
        address += `${selectedAddress.town ? `, ${selectedAddress.town}` : ""}`;
        address += `${
          selectedAddress.country ? `, ${selectedAddress.country}` : ""
        }`;
        address += `${
          selectedAddress.postcode ? `, ${selectedAddress.postcode}` : ""
        }`;

        this.introducer.details.tradingAddress = address;

        if (
          selectedAddress.country &&
          selectedAddress.postcode &&
          selectedAddress.town
        ) {
          if (
            this.introducer.details.registeredAddress
              ?.toUpperCase()
              .includes(selectedAddress?.country) &&
            this.introducer.details.registeredAddress?.includes(
              selectedAddress?.postcode,
            ) &&
            this.introducer.details.registeredAddress?.includes(
              selectedAddress?.town,
            )
          ) {
            this.introducer.details.isTradingSameAsRegisteredAddress = true;
            this.isIntroducerAddressesTheSame = true;
            return;
          }

          this.introducer.details.isTradingSameAsRegisteredAddress = false;
          this.isIntroducerAddressesTheSame = false;
        }
      }
    },

    async viewIntroducer(id: string) {
      this.isShowIntroducerViewer = true;
      this.introducer =
        this.introducersList.find((employee) => employee.id === id) ??
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

    cleanIntroducersBeforeSavingLocally(
      item: IntroducersEntity,
    ): IntroducersEntity {
      //ToDo.contactno
      // // contact numbers
      // item.details.contactNumber = this.helperService.cleanContactNumber(
      //   item.details.contactNumber,
      // );
      // item.departmentDetails.contactNumber =
      //   this.helperService.cleanContactNumber(
      //     item.departmentDetails.contactNumber,
      //   );
      // item.representative.contactNumber = this.helperService.cleanContactNumber(
      //   item.representative.contactNumber,
      // );
      // item.principalDetails.contactNumber =
      //   this.helperService.cleanContactNumber(
      //     item.principalDetails.contactNumber,
      //   );
      return item;
    },
  },
});
</script>

<template src="./ar-introducers.html" />

<style scoped src="./ar-introducers.css" />
