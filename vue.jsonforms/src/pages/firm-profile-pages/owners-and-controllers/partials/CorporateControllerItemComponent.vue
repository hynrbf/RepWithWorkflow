<script lang="ts">
import { defineAsyncComponent, defineComponent, inject } from "vue";
import { CorporateControllerCollapsibleItem } from "@/pages/models/owners-and-controllers/CorporateControllerCollapsibleItem";
import { CorporateControllerModel } from "@/pages/models/owners-and-controllers/CorporateControllerModel";
import StaticList from "@/infra/StaticListService";
import { Director } from "@/entities/owners-and-controllers/Director";
import { IndividualControllerModel } from "@/pages/models/owners-and-controllers/IndividualControllerModel";
import { CompanyOfficerAppointment } from "@/entities/owners-and-controllers/CompanyOfficerAppointment";
import { Controller } from "@/entities/firm-details/Controller";
import { Emitter, EventType } from "mitt";
import { container } from "tsyringe";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import { TabStripSelectEventArguments } from "@progress/kendo-vue-layout";
import { IndividualControllerCollapsibleItemModel } from "@/pages/models/owners-and-controllers/IndividualControllerCollapsibleItemModel";
import { ContactNumber } from "@/entities/ContactNumber";
import { FirmBasicInfo } from "@/entities/FirmBasicInfo";
import { CustomerEntity } from "@/entities/CustomerEntity";
import BusinessIncorporation from "@/pages/models/owners-and-controllers/BusinessIncorporation";
import { AppConstants } from "@/infra/AppConstants";
import { usePageLifeCycleStore } from "@/stores/progress-bar/usePageLifeCycleStore";
import { AlertType, useAlert } from "@/composables/useAlert";
import {
  NotificationType,
  useNotification,
} from "@/composables/useNotification";
import { ScrollableTabItemModel } from "@/components/models/ScrollableTabItemModel";
import { v4 as uuid } from "uuid";
import ScrollableTabComponent from "@/components/ScrollableTabComponent.vue";
import {
  IFcaService,
  IFcaServiceInfo,
} from "@/infra/dependency-services/rest/fca/IFcaService";

export default defineComponent({
  name: "CorporateControllerItemComponent",
  components: {
    ScrollableTabComponent,
    IndividualControllerItemComponent: defineAsyncComponent(
      () => import("../partials/IndividualControllerItemComponent.vue"),
    ),
    KendoFirmFinderComponentAsync: defineAsyncComponent(
      () => import("@/components/form-fields/KendoFirmFinderComponent.vue"),
    ),
  },
  props: {
    individualItems: {
      type: Object as () => IndividualControllerCollapsibleItemModel[],
      default: [] as IndividualControllerCollapsibleItemModel[],
    },
    corporateItem: {
      type: Object as () => CorporateControllerCollapsibleItem,
      default: {
        corporateController: new CorporateControllerModel(),
        isCollapsed: false,
        fullName: "",
        items: [],
      } as CorporateControllerCollapsibleItem,
    },
    registeredCustomer: {
      type: Object as () => CustomerEntity,
      default: new CustomerEntity(),
    },
    identifier: {
      type: String,
      default: "",
    },
  },
  data() {
    return {
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      fcaService: container.resolve<IFcaService>(IFcaServiceInfo.name),
      eventBusControlElement: inject("$eventBusService") as Emitter<
        Record<EventType, string>
      >,
      firmTypes: [] as string[],
      titles: [] as string[],
      nationalities: [] as string[],
      financialSolvencies: [] as string[],
      natureOfBusinesses: [] as string[],
      tabs: [
        {
          title: "Individual Controller(s)",
          content: "IndividualTemplate",
          titleRender: "",
        },
        {
          title: "Corporate Controller(s)",
          content: "CorporateTemplate",
          titleRender: "",
        },
        {
          title: "",
          content: "",
          titleRender: "",
        },
      ],
      selectedControllerTabIndex: 0,
      selectedDirectorTabIndex: -1,

      currentIndividualControllerCollapsibleItems:
        [] as IndividualControllerCollapsibleItemModel[],
      hasIndividualControllers: undefined as boolean | undefined,
      selectedIndividualControllerTabIndex: 0,

      selectedCorporateControllerTabIndex: 0,
      currentCorporateControllerCollapsibleItems:
        [] as CorporateControllerCollapsibleItem[],
      hasCorporateController: undefined as boolean | undefined,

      inputType: this.$t("common-placeholder-text"),
      isInitializing: true,
      isTradingAddressChangedAlertOpened: false,
    };
  },
  computed: {
    CorporateControllerCollapsibleItem() {
      return CorporateControllerCollapsibleItem;
    },
    BusinessIncorporation() {
      return BusinessIncorporation;
    },
    FirmBasicInfo() {
      return FirmBasicInfo;
    },
    ContactNumber() {
      return ContactNumber;
    },
    eighteenYearsAgo(): Date {
      return this.helperService.getDateForGivenYearsAgo(18);
    },
    AppConstants() {
      return AppConstants;
    },
    directorsTab: {
      get() {
        return this.corporateItem.corporateController.directors.map(
          (director, index) => {
            let fullName = "<Director Name>";

            if (director.forename) {
              fullName = director.forename;
            }

            if (director.surname) {
              fullName += ` ${director.surname}`;
            }

            return {
              id: `director-${director.id}`,
              title: fullName,
              content: "",
              active: this.selectedDirectorTabIndex === index,
            } as ScrollableTabItemModel;
          },
        );
      },
      set(tabs: ScrollableTabItemModel[]) {
        for (const index in tabs) {
          if (tabs[index].active) {
            this.selectedDirectorTabIndex = +index;
            break;
          }
        }
      },
    },
    individualControllersTab: {
      get() {
        return this.currentIndividualControllerCollapsibleItems.map(
          ({ individualController }, index) => {
            return {
              id: `individual-controller-${individualController.id}`,
              title: individualController.getFullNameOrSetDefault(
                this.$t("individualControllerPage-newItemTitle"),
              ),
              content: "",
              active: this.selectedIndividualControllerTabIndex === index,
            } as ScrollableTabItemModel;
          },
        );
      },
      set(tabs: ScrollableTabItemModel[]) {
        for (const index in tabs) {
          if (tabs[index].active) {
            this.selectedIndividualControllerTabIndex = +index;
            break;
          }
        }
      },
    },
    corporateControllersTab: {
      get() {
        return this.currentCorporateControllerCollapsibleItems.map(
          ({ corporateController, fullName }, index) => {
            return {
              id: `corporate-controller-${corporateController.id}`,
              title: fullName ?? corporateController?.companyName,
              content: "",
              active: this.selectedCorporateControllerTabIndex === index,
            } as ScrollableTabItemModel;
          },
        );
      },
      set(tabs: ScrollableTabItemModel[]) {
        for (const index in tabs) {
          if (tabs[index].active) {
            this.selectedCorporateControllerTabIndex = +index;
            break;
          }
        }
      },
    },
  },
  setup() {
    const pageLifeCycleStore = usePageLifeCycleStore();
    const { changeLifeCycleName } = pageLifeCycleStore;

    return {
      changeLifeCycleName,
    };
  },
  created() {
    this.titles = StaticList.getTitles();
    this.nationalities = StaticList.getNationalities();
    this.firmTypes = StaticList.getFirmTypes();
    this.financialSolvencies = StaticList.getFinancialSolvency();
    this.natureOfBusinesses = StaticList.getNatureOfBusinesses();

    if (!this.corporateItem?.corporateController) {
      throw new Error("Corporate item should not be empty!");
    }

    this.setUniqueIdForDirectorsIfAny(
      this.corporateItem.corporateController.directors,
    );

    this.currentCorporateControllerCollapsibleItems =
      this.buildCorporateControllerCollapsibleItem(
        this.corporateItem.corporateController,
      );

    this.currentIndividualControllerCollapsibleItems =
      this.buildIndividualControllerCollapsibleItem(
        this.corporateItem.corporateController,
      );

    this.changeLifeCycleName(AppConstants.pageLifeCycleNameCreated);
  },
  mounted() {
    this.helperService.resizeExpander();
    this.isInitializing = false;
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameMounted);
  },
  methods: {
    setUniqueIdForDirectorsIfAny(directorships: Director[]) {
      if (directorships?.length) {
        directorships.forEach((d) => {
          if (!d.id) {
            d.id = uuid();
          }
        });

        this.selectedDirectorTabIndex = 0;
      }
    },

    toggleHasIndividualController(hasIndividualController: boolean) {
      this.hasIndividualControllers = hasIndividualController;

      if (this.hasIndividualControllers) {
        this.addNewIndividualControllerItem();
      } else {
        this.currentIndividualControllerCollapsibleItems = [];
      }
    },

    addNewIndividualControllerItem() {
      const newIndividualController = new IndividualControllerModel();
      const individualControllers = this.corporateItem.corporateController;
      newIndividualController.directorsAndDirectorship = {
        nationalities: [],
        dateOfBirth: 0,
        fullName: newIndividualController.getFullNameOrSetDefault(
          this.$t("individualControllerPage-newItemTitle"),
        ),
        items: [] as CompanyOfficerAppointment[],
      };
      newIndividualController.controllingInterests = [] as Controller[];

      individualControllers.individualControllers.push(newIndividualController);
      this.currentIndividualControllerCollapsibleItems.push({
        fullName: newIndividualController.getFullNameOrSetDefault(
          this.$t("individualControllerPage-newItemTitle"),
        ),
        individualController: newIndividualController,
        isCollapsed: false,
      });

      this.selectedIndividualControllerTabIndex =
        this.currentIndividualControllerCollapsibleItems.length - 1;

      individualControllers.individualOwners =
        this.currentCorporateControllerCollapsibleItems.length;
    },

    removeIndividualControllerItem(indexToRemove: number) {
      useAlert({
        title: "Confirm",
        content: "Are you sure you wish to remove this Individual Controller?",
        confirmButtonText: "Confirm & Delete",
        confirmButtonThemeColor: "error",
        onConfirm: () => {
          const item = this.corporateItem.corporateController;
          item.individualControllers.splice(indexToRemove, 1);
          this.currentIndividualControllerCollapsibleItems.splice(
            indexToRemove,
            1,
          );
          item.individualOwners =
            this.currentIndividualControllerCollapsibleItems.length;

          if (
            indexToRemove <= this.selectedIndividualControllerTabIndex &&
            this.selectedIndividualControllerTabIndex > 0
          ) {
            this.selectedIndividualControllerTabIndex -= 1;
          }

          if (item.individualOwners < 1) {
            this.hasIndividualControllers = false;
          }
        },
        onClose: () => {
          // Do nothing for now
        },
      });
    },

    toggleHasCorporateController(hasCorporateController: boolean) {
      this.hasCorporateController = hasCorporateController;

      if (this.hasCorporateController) {
        this.addNewCorporateControllerItem();
      } else {
        this.currentCorporateControllerCollapsibleItems = [];
      }
    },

    addNewCorporateControllerItem() {
      const corporateController = new CorporateControllerModel();
      const item = this.corporateItem.corporateController;
      corporateController.directors = [] as Director[];
      corporateController.corporateControllers =
        [] as CorporateControllerModel[];
      corporateController.individualControllers =
        [] as IndividualControllerModel[];

      const collapseItem: CorporateControllerCollapsibleItem = {
        corporateController: corporateController,
        isCollapsed: false,
        fullName: this.$t("corporateControllerPage-newItemTitle"),
        items: [],
      };

      item.corporateControllers.push(corporateController);
      this.currentCorporateControllerCollapsibleItems.push(collapseItem);

      this.selectedCorporateControllerTabIndex =
        this.currentCorporateControllerCollapsibleItems.length - 1;
      item.corporateOwners =
        this.currentCorporateControllerCollapsibleItems.length;
    },

    removeCorporateControllerItem(indexToRemove: number) {
      useAlert({
        title: "Confirm",
        content: "Are you sure you wish to remove this Corporate Controller?",
        confirmButtonText: "Confirm & Delete",
        confirmButtonThemeColor: "error",
        onConfirm: () => {
          const item = this.corporateItem.corporateController;
          item.corporateControllers.splice(indexToRemove, 1);
          this.currentCorporateControllerCollapsibleItems.splice(
            indexToRemove,
            1,
          );
          item.corporateOwners =
            this.currentCorporateControllerCollapsibleItems.length;

          if (
            indexToRemove <= this.selectedCorporateControllerTabIndex &&
            this.selectedCorporateControllerTabIndex > 0
          ) {
            this.selectedCorporateControllerTabIndex -= 1;
          }

          if (item.corporateOwners < 1) {
            this.hasCorporateController = false;
          }
        },
        onClose: () => {
          // Do nothing for now
        },
      });
    },

    onToggleTradingAddress(item: CorporateControllerModel) {
      item.isTradingAddressSameAsRegisteredAddress =
        !item.isTradingAddressSameAsRegisteredAddress;

      if (item.isTradingAddressSameAsRegisteredAddress) {
        item.tradingAddress = item.registeredAddress ?? "";
        return;
      } else {
        item.tradingAddress = "";
      }
    },

    onToggleHeadOfficeAddress(item: CorporateControllerModel) {
      item.isHeadOfficeAddressSameAsTradingAddress =
        !item.isHeadOfficeAddressSameAsTradingAddress;

      if (item.isHeadOfficeAddressSameAsTradingAddress) {
        item.headOfficeAddress = item.tradingAddress ?? "";
        return;
      } else {
        item.headOfficeAddress = "";
      }
    },

    onPercentageCapitalValueChange(
      value: number | null,
      item: CorporateControllerModel,
    ) {
      item.percentageOfCapital = value;
      item.percentageOfVotingRights = value;
    },

    onPercentageVotingRightsChange(
      value: number | null,
      item: CorporateControllerModel,
    ) {
      item.percentageOfVotingRights = value;
    },

    onAddDirector() {
      const item = this.corporateItem.corporateController;
      item.directors.push(new Director());
      this.selectedDirectorTabIndex = item.directors.length - 1;
    },

    onRemoveDirector(indexToRemove: number) {
      const item = this.corporateItem.corporateController;
      item.directors.splice(indexToRemove, 1);

      if (indexToRemove <= this.selectedDirectorTabIndex) {
        this.selectedDirectorTabIndex -= 1;
      }
    },

    onDateOfBirthChange(value: Date, item: Director) {
      item.dateOfBirth = this.helperService.dateStringToEpoch(
        value.toDateString(),
      );
    },

    onContactNumberChange(value: ContactNumber) {
      (
        this.corporateItem as CorporateControllerCollapsibleItem
      ).corporateController.contactNumber = value;
    },

    convertEpochValueToDate(input: number | undefined): Date | undefined {
      if (!input) {
        return new Date();
      }

      return this.helperService.convertEpochToDateTime(input);
    },

    onTabItemSelect(e: TabStripSelectEventArguments) {
      this.selectedControllerTabIndex = e.selected;
    },

    buildCorporateControllerCollapsibleItem(
      corporateController: CorporateControllerModel,
    ): CorporateControllerCollapsibleItem[] {
      let collapsibleItems = [] as CorporateControllerCollapsibleItem[];

      for (const item of corporateController.corporateControllers) {
        collapsibleItems.push({
          fullName: item.companyName ?? "",
          corporateController: item,
          isCollapsed: false,
          items: [] as CorporateControllerCollapsibleItem[],
        });
      }

      if (corporateController.corporateOwners) {
        this.hasCorporateController = true;
      }

      corporateController.corporateOwnersActual =
        corporateController.corporateOwners;
      return collapsibleItems;
    },

    buildIndividualControllerCollapsibleItem(
      corporateController: CorporateControllerModel,
    ): {
      fullName: string;
      isCollapsed: boolean;
      individualController: IndividualControllerModel;
    }[] {
      let collapsibleItems = [] as {
        fullName: string;
        isCollapsed: boolean;
        individualController: IndividualControllerModel;
      }[];

      for (const item of corporateController.individualControllers) {
        collapsibleItems.push({
          fullName: item.getFullNameOrSetDefault(
            this.$t("individualControllerPage-newItemTitle"),
          ),
          individualController: item,
          isCollapsed: false,
        });
      }

      if (corporateController.individualOwners) {
        this.hasIndividualControllers = true;
      }

      corporateController.individualOwnersActual =
        corporateController.individualOwners;
      return collapsibleItems;
    },

    mapCorporateControllerToFirmModel(
      corporateControllerModel: CorporateControllerModel,
    ): FirmBasicInfo {
      return {
        companyNumber: corporateControllerModel.companyNumber,
        firmName: corporateControllerModel.companyName,
        firmReferenceNumber: corporateControllerModel.firmReferenceNumber,
        fcaStatus: corporateControllerModel?.firm?.fcaStatus ?? "",
        companyHouseStatus:
          corporateControllerModel?.firm?.companyHouseStatus ?? "",
        address: corporateControllerModel?.registeredAddress ?? "",
        tradingAddress: corporateControllerModel?.tradingAddress ?? "",
        headOfficeAddress: corporateControllerModel?.headOfficeAddress ?? "",
        contactNumber: "",
        website: "",
        sicCode: "",
        countryCode: "",
      };
    },

    async onCorporateControllerUpdateAsync(corporateController: FirmBasicInfo) {
      (
        this.corporateItem as CorporateControllerCollapsibleItem
      ).corporateController.companyName = corporateController.firmName ?? "";
      (
        this.corporateItem as CorporateControllerCollapsibleItem
      ).corporateController.companyNumber =
        corporateController.companyNumber ?? "";
      (
        this.corporateItem as CorporateControllerCollapsibleItem
      ).corporateController.firmReferenceNumber =
        corporateController.firmReferenceNumber ?? "";
      (
        this.corporateItem as CorporateControllerCollapsibleItem
      ).corporateController.firm = corporateController;

      (
        this.corporateItem as CorporateControllerCollapsibleItem
      ).corporateController.registeredAddress =
        corporateController.address ?? "";
      (
        this.corporateItem as CorporateControllerCollapsibleItem
      ).corporateController.tradingAddress =
        corporateController.tradingAddress ?? "";
      (
        this.corporateItem as CorporateControllerCollapsibleItem
      ).corporateController.isRegisteredAddressChanged = false;
      (
        this.corporateItem as CorporateControllerCollapsibleItem
      ).corporateController.isTradingAddressChanged = false;
      (
        this.corporateItem as CorporateControllerCollapsibleItem
      ).corporateController.natureOfBusiness =
        StaticList.getNatureOfBusinessBySicCode(corporateController.sicCode);

      if (
        this.corporateItem.corporateController
          .isTradingAddressSameAsRegisteredAddress
      ) {
        (
          this.corporateItem as CorporateControllerCollapsibleItem
        ).corporateController.tradingAddress =
          this.corporateItem.corporateController.registeredAddress;
      }

      await this.populateContactNumberAsync(
        corporateController.firmReferenceNumber,
      );
    },

    async populateContactNumberAsync(firmRefNo: string | undefined) {
      if (!firmRefNo) {
        return;
      }

      const addressDetails = await this.fcaService.getFirmAddressesDetailsAsync(
        firmRefNo,
        "PPOB",
      );

      if (addressDetails[0]["Phone Number"] && addressDetails[0]["country"]) {
        (
          this.corporateItem as CorporateControllerCollapsibleItem
        ).corporateController.contactNumber =
          await this.helperService.convertToContactNoAsync(
            addressDetails[0]["Phone Number"],
            addressDetails[0]["country"],
          );
      }
    },

    mapCountryAndNatureOfBusinessToBusinessIncorporation(
      item: CorporateControllerModel,
    ): BusinessIncorporation {
      return {
        country: item.countryOfIncorporation ?? "",
        businessNature: item.natureOfBusiness ?? "",
      };
    },

    onBusinessIncorporationChange(
      businessIncorporation: { country: string; businessNature: string },
      item: CorporateControllerModel,
    ) {
      item.countryOfIncorporation = businessIncorporation.country;
      item.natureOfBusiness = businessIncorporation.businessNature;
    },

    onCorporateControllerRegisteredAddressChange(value: string) {
      const selectedInfo = this.corporateItem.corporateController;
      const firm = this.corporateItem.corporateController.firm;

      if (!firm?.address) {
        selectedInfo.isRegisteredAddressChanged = false;
        return;
      }

      selectedInfo.registeredAddress = value;
      selectedInfo.isRegisteredAddressChanged =
        selectedInfo.registeredAddress !== firm.address;

      if (selectedInfo.isTradingAddressSameAsRegisteredAddress) {
        selectedInfo.tradingAddress = selectedInfo.registeredAddress;
      }
    },

    onCorporateControllerTradingAddressDoneTyping(
      elementId: string,
      hasChanged: boolean,
    ) {
      if (!hasChanged || this.isTradingAddressChangedAlertOpened) {
        return;
      }

      const selectedInfo = this.corporateItem.corporateController;
      const firm = this.corporateItem.corporateController.firm;

      if (!firm?.tradingAddress) {
        selectedInfo.isTradingAddressChanged = false;
        return;
      }

      this.isTradingAddressChangedAlertOpened = true;

      useAlert({
        title: this.$t("common-alert-title"),
        content: "Are you sure you wish to update your Trading Address?",
        additionalContent: `Current Trading Address with the FCA is ${firm.tradingAddress}.`,
        confirmButtonText: "Confirm & Save",
        type: AlertType.ALERT,
        onConfirm: async () => {
          useNotification({
            type: NotificationType.SUCCESS,
            content: "Trading Address updated.",
            interval: AppConstants.notificationPopupTimeOut,
          });

          selectedInfo.isTradingAddressChanged =
            selectedInfo.tradingAddress !== firm.tradingAddress;

          if (selectedInfo.isHeadOfficeAddressSameAsTradingAddress) {
            selectedInfo.headOfficeAddress = selectedInfo.tradingAddress;
          }

          this.isTradingAddressChangedAlertOpened = false;
          this.eventBusControlElement.emit(
            AppConstants.updateTradingAddressControlStateEvent,
            elementId,
          );
        },
        onClose: () => {
          selectedInfo.tradingAddress = firm?.tradingAddress ?? "";

          if (!selectedInfo.isTradingAddressChanged) {
            selectedInfo.isTradingAddressChanged = false;
          }

          this.isTradingAddressChangedAlertOpened = false;
        },
      });
    },

    onCorporateControllerTradingAddressChange(value: string) {
      const selectedInfo = this.corporateItem.corporateController;
      selectedInfo.tradingAddress = value;
    },

    uploadFormatForCv(director: Director): string {
      const formatForeName = director.forename.split(" ")[0];
      const foreNameAndSurname = `${formatForeName}${director.surname}`;

      return `CorporateControllersCV-${this.registeredCustomer.companyNumber}-${this.corporateItem.corporateController.companyNumber}-${foreNameAndSurname}`;
    },

    setUniqueIdentifier(value: string): string {
      const identifier = `${AppConstants.ownersAndControllersRoute}-CC${this.identifier}${value}`;
      return identifier.replace(/\s+/g, "").replace("/", "");
    },
  },
});
</script>

<template>
  <StackLayout
    class="template-content-container"
    v-if="corporateItem"
    orientation="vertical"
  >
    <StackLayout
      class="with-vertical-gap"
      orientation="vertical"
      :align="{ horizontal: 'start', vertical: 'middle' }"
    >
      <Label class="section-title k-color-primary flex-grow-1"
        >Corporate Controller</Label
      >

      <!-- Anchors -->
      <StackLayout
        class="with-horizontal-gap align-self-stretch"
        orientation="horizontal"
      >
        <div class="anchor-selected section-item">
          <IconComponent symbol="business-card" size="24" />

          <Label>Details</Label>
        </div>

        <div class="anchor section-item">
          <IconComponent symbol="hand-held-tablet-writing-77" size="24" />

          <Label>Additional Information</Label>
        </div>

        <div class="anchor section-item">
          <IconComponent symbol="user-square-single-17" size="24" />

          <Label>Directors</Label>
        </div>

        <div class="anchor section-item">
          <IconComponent symbol="user-multiple-accounts-1" size="24" />

          <Label>Controllers</Label>
        </div>
      </StackLayout>
    </StackLayout>

    <KendoAccordionItemComponent title="Corporate Controller Details">
      <StackLayout class="with-vertical-gap" orientation="vertical">
        <KendoFirmFinderComponentAsync
          :id="setUniqueIdentifier('-corporateController.companyName')"
          companyNameLabel="Company Name"
          :IsInitializing="isInitializing"
          :company="
            mapCorporateControllerToFirmModel(corporateItem.corporateController)
          "
          :companyName="corporateItem.corporateController?.companyName"
          :companyNumber="corporateItem.corporateController?.companyNumber"
          :firmReferenceNumber="
            corporateItem.corporateController?.firmReferenceNumber
          "
          @onCompanyDetailUpdated="
            (value: FirmBasicInfo) => {
              onCorporateControllerUpdateAsync(value);
            }
          "
        />

        <KendoBusinessIncorporationComponent
          :id="setUniqueIdentifier('-corporateController.businessNature')"
          :isInitializing="isInitializing"
          :value="
            mapCountryAndNatureOfBusinessToBusinessIncorporation(
              corporateItem.corporateController,
            )
          "
          @onBusinessIncorporationChange="
            (businessNature: BusinessIncorporation) =>
              onBusinessIncorporationChange(
                businessNature,
                corporateItem.corporateController,
              )
          "
        />

        <StackLayout
          class="with-horizontal-gap"
          orientation="horizontal"
          :align="{ horizontal: 'stretch' }"
        >
          <KendoDropDownInputComponent
            :id="setUniqueIdentifier('-corporateControllerDetails.firmType')"
            name="corporateControllerDetails.firmType"
            label="Firm Type"
            @onValueChange="
              (value: string) =>
                ((
                  corporateItem as CorporateControllerCollapsibleItem
                ).corporateController.firmType = value)
            "
            :value="corporateItem?.corporateController?.firmType"
            placeholder="Please select ..."
            :data-items="firmTypes"
            :isValueReactive="true"
            :isDataLoadedCompletely="!isInitializing"
          />

          <KendoDropDownInputComponent
            :id="
              setUniqueIdentifier(
                '-corporateControllerDetails.financialSolvency',
              )
            "
            label="Financial Solvency"
            placeholder="Please Select"
            name="corporateControllerDetails.financialSolvency"
            @onValueChange="
              (value: string) =>
                ((
                  corporateItem as CorporateControllerCollapsibleItem
                ).corporateController.financialSolvency = value)
            "
            :dataItems="financialSolvencies"
            :value="corporateItem?.corporateController?.financialSolvency"
            :isValueReactive="true"
            :isDataLoadedCompletely="!isInitializing"
          />
        </StackLayout>

        <KendoPostCodeInputComponent
          :id="
            setUniqueIdentifier('-corporateControllerDetails.registeredAddress')
          "
          name="'corporateControllerDetails.registeredAddress"
          label="Registered Address"
          :isUserModified="
            corporateItem.corporateController.isRegisteredAddressChanged
          "
          @onValueChange="
            (value: string) =>
              onCorporateControllerRegisteredAddressChange(value)
          "
          :value="corporateItem?.corporateController?.registeredAddress"
          :isValueReactive="true"
          :isDataLoadedCompletely="!isInitializing"
        />

        <KendoPostCodeInputWithSameAsComponent
          :id="
            setUniqueIdentifier('-corporateControllerDetails.tradingAddress')
          "
          name="corporateControllerDetails.tradingAddress"
          label="Trading Address"
          :isRequired="true"
          :sameAsLabel="$t('same-as-registered-address')"
          :isChecked="
            corporateItem.corporateController
              ?.isTradingAddressSameAsRegisteredAddress ?? false
          "
          :isUserModified="
            corporateItem.corporateController.isTradingAddressChanged
          "
          :value="corporateItem.corporateController?.tradingAddress"
          @onToggle="onToggleTradingAddress(corporateItem.corporateController)"
          @onValueChange="
            (value: string) => onCorporateControllerTradingAddressChange(value)
          "
          @onLostFocus="onCorporateControllerTradingAddressDoneTyping"
          :isValueReactive="true"
          :isDataLoadedCompletely="!isInitializing"
        />

        <KendoPostCodeInputWithSameAsComponent
          :id="
            setUniqueIdentifier('-corporateControllerDetails.headOfficeAddress')
          "
          name="corporateControllerDetails.headOfficeAddress"
          label="Head Office Address"
          :sameAsLabel="$t('same-as-trading-address')"
          :isRequired="false"
          :isChecked="
            corporateItem.corporateController
              ?.isHeadOfficeAddressSameAsTradingAddress ?? false
          "
          :value="corporateItem?.corporateController?.headOfficeAddress"
          @onToggle="
            onToggleHeadOfficeAddress(corporateItem.corporateController)
          "
          @onValueChange="
            (value: string) =>
              ((
                corporateItem as CorporateControllerCollapsibleItem
              ).corporateController.headOfficeAddress = value)
          "
          :isValueReactive="true"
          :isDataLoadedCompletely="!isInitializing"
        />

        <StackLayout
          class="with-horizontal-gap"
          orientation="horizontal"
          :align="{ horizontal: 'stretch' }"
        >
          <KendoPercentageInputComponent
            :id="
              setUniqueIdentifier(
                '-corporateControllerDetails.percentageOfCapital',
              )
            "
            name="corporateControllerDetails.percentageOfCapital"
            :minLimit="10"
            maxLimit="100"
            KendoNumericInputComponent
            label="Percentage of Capital (Ownership)"
            placeholder="%"
            @onValueChange="
              (value: number | null) =>
                onPercentageCapitalValueChange(
                  value,
                  corporateItem.corporateController,
                )
            "
            :value="corporateItem?.corporateController?.percentageOfCapital"
            :isValueReactive="true"
            :isDataLoadedCompletely="!isInitializing"
          />

          <KendoPercentageInputComponent
            :id="
              setUniqueIdentifier(
                '-corporateControllerDetails.percentageOfVotingRights',
              )
            "
            name="corporateControllerDetails.percentageOfVotingRights"
            :minLimit="10"
            maxLimit="100"
            label="Percentage of Voting Rights"
            placeholder="%"
            @onValueChange="
              (value: number | null) =>
                onPercentageVotingRightsChange(
                  value,
                  corporateItem.corporateController,
                )
            "
            :value="
              corporateItem?.corporateController?.percentageOfVotingRights
            "
            :isValueReactive="true"
            :isDataLoadedCompletely="!isInitializing"
          />
        </StackLayout>

        <StackLayout
          orientation="horizontal"
          class="with-horizontal-gap"
          :align="{ horizontal: 'start' }"
        >
          <KendoEmailAddressInputComponent
            :id="
              setUniqueIdentifier('-corporateControllerDetails.emailAddress')
            "
            name="'corporateControllerDetails.emailAddress"
            class="col"
            label="Email Address"
            @onValueChange="
              (value: string) =>
                ((
                  corporateItem as CorporateControllerCollapsibleItem
                ).corporateController.emailAddress = value)
            "
            :value="corporateItem?.corporateController?.emailAddress"
            :isValueReactive="true"
            :isDataLoadedCompletely="!isInitializing"
          />

          <KendoTelInputComponent
            :id="
              setUniqueIdentifier('-corporateControllerDetails.contactNumber')
            "
            name="corporateControllerDetails.contactNumber"
            class="col"
            label="Contact Number"
            :value="
              corporateItem?.corporateController?.contactNumber ??
              new ContactNumber()
            "
            @onValueChange="onContactNumberChange"
            :isValueReactive="true"
            :isDataLoadedCompletely="!isInitializing"
          />
        </StackLayout>
      </StackLayout>
    </KendoAccordionItemComponent>

    <KendoAccordionItemComponent title="Additional Information">
      <StackLayout class="with-vertical-gap" orientation="vertical">
        <KendoYesOrNoQuestionInputComponent
          :id="
            setUniqueIdentifier(
              '-corporateControllerDetails.isSubjectToRegulationByAnotherRegulator',
            )
          "
          :name="
            setUniqueIdentifier(
              '-corporateControllerDetails.isSubjectToRegulationByAnotherRegulator',
            )
          "
          :isRequired="true"
          :value="
            corporateItem?.corporateController
              ?.isSubjectToRegulationByAnotherRegulator
          "
          @onValueChange="
            (value: boolean) =>
              ((
                corporateItem as CorporateControllerCollapsibleItem
              ).corporateController.isSubjectToRegulationByAnotherRegulator =
                value)
          "
          :isValueReactive="true"
          :isDataLoadedCompletely="!isInitializing"
          numberText="1"
          :questionText="`Is ${corporateItem.corporateController?.companyName} subject to regulation by another regulator? `"
        />

        <KendoYesOrNoQuestionInputComponent
          :id="
            setUniqueIdentifier(
              '-corporateControllerDetails.isThirdCountryFirm',
            )
          "
          :name="
            setUniqueIdentifier(
              '-corporateControllerDetails.isThirdCountryFirm',
            )
          "
          numberText="2"
          :isRequired="true"
          :value="corporateItem?.corporateController?.isThirdCountryFirm"
          @onValueChange="
            (value: boolean) =>
              ((
                corporateItem as CorporateControllerCollapsibleItem
              ).corporateController.isThirdCountryFirm = value)
          "
          :isValueReactive="true"
          :isDataLoadedCompletely="!isInitializing"
          :questionText="`Is ${corporateItem.corporateController?.companyName} a third country investment firm, a third country credit institution, a third country insurance undertaking or a third country management company? `"
        />

        <StackLayout
          v-if="corporateItem?.corporateController?.isThirdCountryFirm ?? false"
          orientation="vertical"
          class="with-vertical-gap"
          :align="{ horizontal: 'stretch' }"
        >
          <KendoTextAreaInputComponent
            :id="
              setUniqueIdentifier(
                '-corporateControllerDetails.thirdCountryFirmInfo',
              )
            "
            name="corporateControllerDetails.thirdCountryFirmInfo"
            class="flex-grow-1"
            label="Additional Information"
            :corner-radius="8"
            :placeholder="inputType"
            @onValueChange="
              (value: string) =>
                ((
                  corporateItem as CorporateControllerCollapsibleItem
                ).corporateController.thirdCountryFirmInfo = value)
            "
            :value="corporateItem?.corporateController?.thirdCountryFirmInfo"
            :isValueReactive="true"
            :isDataLoadedCompletely="!isInitializing"
          />

          <KendoUploadInputComponent
            label="Supporting documents"
            optionalText="(Optional) (Accepted file types: pdf, jpg, doc, Maximum 25 MB file size)"
            stretched
            multiple
            dropzone
            buttonText="Upload file"
            :isRequired="false"
            :disabled="!registeredCustomer.companyNumber ?? true"
            :allowed-extensions="['.pdf', 'doc', 'docx']"
            :uploadFor="`CorporateControllersThirdCountryFirm-${registeredCustomer.companyNumber}-${corporateItem.corporateController.companyNumber}`"
            @onFinishedUpload="
              (url: string) =>
                (
                  corporateItem as CorporateControllerCollapsibleItem
                ).corporateController.supportingDocumentsUrls.push(url)
            "
            :isValueReactive="true"
            :isDataLoadedCompletely="!isInitializing"
          />
        </StackLayout>

        <KendoYesOrNoQuestionInputComponent
          :id="
            setUniqueIdentifier(
              '-corporateControllerDetails.isMemberOfFinancialConglomerate',
            )
          "
          :name="
            setUniqueIdentifier(
              '-corporateControllerDetails.isMemberOfFinancialConglomerate',
            )
          "
          numberText="3"
          :isRequired="true"
          :value="
            corporateItem?.corporateController?.isMemberOfFinancialConglomerate
          "
          @onValueChange="
            (value: boolean) =>
              ((
                corporateItem as CorporateControllerCollapsibleItem
              ).corporateController.isMemberOfFinancialConglomerate = value)
          "
          :isValueReactive="true"
          :isDataLoadedCompletely="!isInitializing"
          :questionText="`Is ${corporateItem.corporateController?.companyName} a member of a financial conglomerate? `"
        />

        <StackLayout
          v-if="
            corporateItem?.corporateController
              ?.isMemberOfFinancialConglomerate ?? false
          "
          orientation="vertical"
          class="with-vertical-gap"
          :align="{ horizontal: 'stretch' }"
        >
          <KendoTextAreaInputComponent
            :id="
              setUniqueIdentifier(
                '-corporateControllerDetails.memberOfFinancialConglomerateInfo',
              )
            "
            name="corporateControllerDetails.memberOfFinancialConglomerateInfo"
            class="flex-grow-1"
            label="Additional Information"
            :corner-radius="8"
            :placeholder="inputType"
            @onValueChange="
              (value: string) =>
                ((
                  corporateItem as CorporateControllerCollapsibleItem
                ).corporateController.memberOfFinancialConglomerateInfo = value)
            "
            :value="
              corporateItem.corporateController
                ?.memberOfFinancialConglomerateInfo
            "
            :isValueReactive="true"
            :isDataLoadedCompletely="!isInitializing"
          />

          <KendoUploadInputComponent
            label="Supporting documents"
            optionalText="(Optional) (Accepted file types: pdf, jpg, doc, Maximum 25 MB file size)"
            stretched
            multiple
            dropzone
            buttonText="Upload file"
            :isRequired="false"
            :disabled="!registeredCustomer.companyNumber ?? true"
            :allowed-extensions="['.pdf', 'doc', 'docx']"
            :uploadFor="`CorporateControllersConglomerate-${registeredCustomer.companyNumber}-${corporateItem.corporateController.companyNumber}`"
            @onFinishedUpload="
              (url: string) =>
                (
                  corporateItem as CorporateControllerCollapsibleItem
                ).corporateController.supportingDocumentsUrls.push(url)
            "
          />
        </StackLayout>

        <KendoYesOrNoQuestionInputComponent
          :id="
            setUniqueIdentifier(
              '-corporateControllerDetails.isMemberOfThirdCountryFinancialConglomerate',
            )
          "
          :name="
            setUniqueIdentifier(
              '-corporateControllerDetails.isMemberOfThirdCountryFinancialConglomerate',
            )
          "
          numberText="4"
          :isRequired="true"
          :value="
            corporateItem?.corporateController
              ?.isMemberOfThirdCountryFinancialConglomerate
          "
          @onValueChange="
            (value: boolean) =>
              ((
                corporateItem as CorporateControllerCollapsibleItem
              ).corporateController.isMemberOfThirdCountryFinancialConglomerate =
                value)
          "
          :isValueReactive="true"
          :isDataLoadedCompletely="!isInitializing"
          :questionText="`Is ${corporateItem.corporateController?.companyName} a member of a third-country financial conglomerate? `"
        />

        <StackLayout
          v-if="
            corporateItem?.corporateController
              ?.isMemberOfThirdCountryFinancialConglomerate ?? false
          "
          orientation="vertical"
          class="with-vertical-gap"
          :align="{ horizontal: 'stretch' }"
        >
          <KendoTextAreaInputComponent
            :id="
              setUniqueIdentifier(
                '-corporateControllerDetails.memberOfThirdCountryFinancialConglomerateInfo',
              )
            "
            name="corporateControllerDetails.memberOfThirdCountryFinancialConglomerateInfo"
            class="flex-grow-1"
            label="Additional Information"
            :corner-radius="8"
            :placeholder="inputType"
            @onValueChange="
              (value: string) =>
                ((
                  corporateItem as CorporateControllerCollapsibleItem
                ).corporateController.memberOfThirdCountryFinancialConglomerateInfo =
                  value)
            "
            :value="
              corporateItem.corporateController
                .memberOfThirdCountryFinancialConglomerateInfo
            "
            :isValueReactive="true"
            :isDataLoadedCompletely="!isInitializing"
          />

          <KendoUploadInputComponent
            label="Supporting documents"
            optionalText="(Optional) (Accepted file types: pdf, jpg, doc, Maximum 25 MB file size)"
            stretched
            multiple
            dropzone
            buttonText="Upload file"
            :isRequired="false"
            :disabled="!registeredCustomer.companyNumber ?? true"
            :allowed-extensions="['.pdf', 'doc', 'docx']"
            :uploadFor="`CorporateControllersThirdConglomerate-${registeredCustomer.companyNumber}-${corporateItem.corporateController.companyNumber}`"
            @onFinishedUpload="
              (url: string) =>
                (
                  corporateItem as CorporateControllerCollapsibleItem
                ).corporateController.supportingDocumentsUrls.push(url)
            "
          />
        </StackLayout>

        <KendoYesOrNoQuestionInputComponent
          :id="
            setUniqueIdentifier(
              '-corporateControllerDetails.isMemberOfThirdCountryBanking',
            )
          "
          :name="
            setUniqueIdentifier(
              '-corporateControllerDetails.isMemberOfThirdCountryBanking',
            )
          "
          numberText="5"
          :isRequired="true"
          :value="
            corporateItem.corporateController?.isMemberOfThirdCountryBanking
          "
          @onValueChange="
            (value: boolean) =>
              ((
                corporateItem as CorporateControllerCollapsibleItem
              ).corporateController.isMemberOfThirdCountryBanking = value)
          "
          :isValueReactive="true"
          :isDataLoadedCompletely="!isInitializing"
          :questionText="`Is ${corporateItem.corporateController?.companyName} a member of a third-country banking and investment group? `"
        />

        <StackLayout
          v-if="
            corporateItem.corporateController?.isMemberOfThirdCountryBanking ??
            false
          "
          orientation="vertical"
          class="with-vertical-gap"
          :align="{ horizontal: 'stretch' }"
        >
          <KendoTextAreaInputComponent
            :id="
              setUniqueIdentifier(
                '-corporateControllerDetails.memberOfThirdCountryBankingInfo',
              )
            "
            name="corporateControllerDetails.memberOfThirdCountryBankingInfo"
            class="flex-grow-1"
            label="Additional Information"
            :corner-radius="8"
            :placeholder="inputType"
            @onValueChange="
              (value: string) =>
                ((
                  corporateItem as CorporateControllerCollapsibleItem
                ).corporateController.memberOfThirdCountryBankingInfo = value)
            "
            :value="
              corporateItem.corporateController.memberOfThirdCountryBankingInfo
            "
            :isValueReactive="true"
            :isDataLoadedCompletely="!isInitializing"
          />

          <KendoUploadInputComponent
            label="Supporting documents"
            optionalText="(Optional) (Accepted file types: pdf, jpg, doc, Maximum 25 MB file size)"
            stretched
            multiple
            dropzone
            buttonText="Upload file"
            :isRequired="false"
            :disabled="!registeredCustomer.companyNumber ?? true"
            :allowed-extensions="['.pdf', 'doc', 'docx']"
            :uploadFor="`CorporateControllersThirdCountryBanking-${registeredCustomer.companyNumber}-${corporateItem.corporateController.companyNumber}`"
            @onFinishedUpload="
              (url: string) =>
                (
                  corporateItem as CorporateControllerCollapsibleItem
                ).corporateController.supportingDocumentsUrls.push(url)
            "
          />
        </StackLayout>

        <KendoYesOrNoQuestionInputComponent
          :id="
            setUniqueIdentifier(
              '-corporateControllerDetails.hasBeenSubjectToAnyMaterialComplaints',
            )
          "
          :name="
            setUniqueIdentifier(
              '-corporateControllerDetails.hasBeenSubjectToAnyMaterialComplaints',
            )
          "
          numberText="6"
          :isRequired="true"
          :value="
            corporateItem.corporateController
              ?.hasBeenSubjectToAnyMaterialComplaints
          "
          @onValueChange="
            (value: boolean) =>
              ((
                corporateItem as CorporateControllerCollapsibleItem
              ).corporateController.hasBeenSubjectToAnyMaterialComplaints =
                value)
          "
          :isValueReactive="true"
          :isDataLoadedCompletely="!isInitializing"
          :questionText="`Has ${corporateItem.corporateController?.companyName} or any persons with a position of influence or who effectively run the business of ${corporateItem.corporateController?.companyName}, been subject to any material complaints made against them by their customers or former customers in the last 5 years, which are awaiting determination by, or have been upheld by, an ombudsman? `"
        />

        <StackLayout
          v-if="
            corporateItem.corporateController
              ?.hasBeenSubjectToAnyMaterialComplaints
          "
          orientation="vertical"
          class="with-vertical-gap"
          :align="{ horizontal: 'stretch' }"
        >
          <KendoTextAreaInputComponent
            :id="
              setUniqueIdentifier(
                '-corporateControllerDetails.beenSubjectToAnyMaterialComplaintsInfo',
              )
            "
            name="corporateControllerDetails.beenSubjectToAnyMaterialComplaintsInfo"
            class="flex-grow-1"
            label="Additional Information"
            :corner-radius="8"
            :placeholder="inputType"
            @onValueChange="
              (value: string) =>
                ((
                  corporateItem as CorporateControllerCollapsibleItem
                ).corporateController.beenSubjectToAnyMaterialComplaintsInfo =
                  value)
            "
            :value="
              corporateItem.corporateController
                .beenSubjectToAnyMaterialComplaintsInfo
            "
            :isValueReactive="true"
            :isDataLoadedCompletely="!isInitializing"
          />

          <KendoUploadInputComponent
            label="Supporting documents"
            optionalText="(Optional) (Accepted file types: pdf, jpg, doc, Maximum 25 MB file size)"
            stretched
            multiple
            dropzone
            buttonText="Upload file"
            :isRequired="false"
            :disabled="!registeredCustomer.companyNumber ?? true"
            :allowed-extensions="['.pdf', 'doc', 'docx']"
            :uploadFor="`CorporateControllersMaterialComplaints-${registeredCustomer.companyNumber}-${corporateItem.corporateController.companyNumber}`"
            @onFinishedUpload="
              (url: string) =>
                (
                  corporateItem as CorporateControllerCollapsibleItem
                ).corporateController.supportingDocumentsUrls.push(url)
            "
          />
        </StackLayout>
      </StackLayout>
    </KendoAccordionItemComponent>

    <KendoAccordionItemComponent
      :title="
        corporateItem.corporateController?.companyName
          ? `Directors of ${corporateItem.corporateController?.companyName}`
          : 'Directors of &lt;Company Name&gt;'
      "
    >
      <StackLayout orientation="vertical" class="with-vertical-gap">
        <ScrollableTabComponent
          v-model:items="directorsTab"
          :addable="true"
          @add="onAddDirector"
          :removable="true"
          @remove="onRemoveDirector"
        >
          <template
            v-for="director in corporateItem.corporateController.directors"
            :key="`director-${director.id}`"
            #[`content(director-${director.id})`]
          >
            <Card class="col with-vertical-gap scrollable-tab-card-item">
              <Label class="section-title k-color-primary flex-grow-1"
                >Director Details</Label
              >

              <StackLayout
                orientation="horizontal"
                class="with-horizontal-gap"
                :align="{ horizontal: 'stretch' }"
              >
                <KendoNameTitleComponent
                  :id="
                    setUniqueIdentifier(
                      `-director-${director.id}` + '-director.title',
                    )
                  "
                  :name="`director.title-${director.id}`"
                  label="Title"
                  :isRequired="false"
                  :value="director.title"
                  @onValueChange="(value: string) => (director.title = value)"
                />

                <KendoGenericInputComponent
                  :id="
                    setUniqueIdentifier(
                      `-director-${director.id}` + '-director.forename',
                    )
                  "
                  :name="`director.forename--${director.id}`"
                  label="Forename(s)"
                  :isCapitalizeFirstLetter="true"
                  :placeholder="inputType"
                  @onValueChange="
                    (value: string) => (director.forename = value)
                  "
                  :value="director.forename"
                  :isValueReactive="true"
                  :isDataLoadedCompletely="!isInitializing"
                />

                <KendoGenericInputComponent
                  :id="
                    setUniqueIdentifier(
                      `-director-${director.id}` + '-director.surname',
                    )
                  "
                  :name="`director.surname-${director.id}`"
                  label="Surname"
                  :placeholder="inputType"
                  :isCapitalizeFirstLetter="true"
                  @onValueChange="(value: string) => (director.surname = value)"
                  :value="director.surname"
                  :isValueReactive="true"
                  :isDataLoadedCompletely="!isInitializing"
                />
              </StackLayout>

              <StackLayout
                class="with-horizontal-gap"
                orientation="horizontal"
                :align="{ horizontal: 'stretch' }"
              >
                <KendoDatePickerInputComponent
                  :id="
                    setUniqueIdentifier(
                      `-director-${director.id}` + '-director.dateOfBirth',
                    )
                  "
                  :name="`director.dateOfBirth-${director.id}`"
                  :maxDate="eighteenYearsAgo"
                  label="Date of Birth"
                  @onValueChange="
                    (value: Date) => onDateOfBirthChange(value, director)
                  "
                  :value="convertEpochValueToDate(director.dateOfBirth)"
                  :isValueReactive="true"
                  :isDataLoadedCompletely="!isInitializing"
                />

                <KendoGenericInputComponent
                  :id="
                    setUniqueIdentifier(
                      `-director-${director.id}` + '-director.position',
                    )
                  "
                  :name="`director.position-${director.id}`"
                  label="Position"
                  placeholder="Type..."
                  @onValueChange="
                    (value: string) => (director.position = value)
                  "
                  :value="director.position"
                  :isValueReactive="true"
                  :isDataLoadedCompletely="!isInitializing"
                />
              </StackLayout>

              <StackLayout orientation="vertical" style="gap: 15px">
                <Label
                  class="section-description section-description-upload-cv"
                >
                  Please upload a copy of CV (Accepted file types: pdf, jpg,
                  doc, Maximum 25 MB file size)
                </Label>

                <KendoUploadInputComponent
                  stretched
                  multiple
                  dropzone
                  buttonText="Upload CV"
                  :disabled="!registeredCustomer.companyNumber ?? true"
                  :allowed-extensions="['.pdf', 'doc', 'docx']"
                  :uploadFor="uploadFormatForCv(director)"
                  @onFinishedUpload="
                    (url: string) => (director.cvUrlLink = url)
                  "
                />
              </StackLayout>
            </Card>
          </template>
        </ScrollableTabComponent>
      </StackLayout>
    </KendoAccordionItemComponent>

    <KendoAccordionItemComponent
      :title="
        corporateItem.corporateController?.companyName
          ? `Controllers of ${corporateItem.corporateController?.companyName}`
          : 'Controllers of &lt;Company Name&gt;'
      "
    >
      <TabStrip
        class="TabStripControllers TabStripControllers--lined"
        :selected="selectedControllerTabIndex"
        @select="onTabItemSelect"
        :tabs="tabs"
      >
        <template v-slot:IndividualTemplate>
          <StackLayout orientation="vertical" :gap="8">
            <KendoCard class="mb-4">
              <KendoCardBody>
                <KendoYesOrNoQuestionInputComponent
                  :id="
                    setUniqueIdentifier(
                      '-corporateControllerDetails.hasIndividualController',
                    )
                  "
                  :questionText="
                    $t(
                      'individualControllersPage-hasIndividualControllerText',
                      { firmName: corporateItem.fullName },
                    )
                  "
                  :value="hasIndividualControllers"
                  :isValueReactive="true"
                  :isDataLoadedCompletely="!isInitializing"
                  @onValueChange="toggleHasIndividualController"
                />
              </KendoCardBody>
            </KendoCard>

            <ScrollableTabComponent
              v-if="hasIndividualControllers"
              v-model:items="individualControllersTab"
              :addable="true"
              @add="addNewIndividualControllerItem"
              :removable="true"
              @remove="removeIndividualControllerItem"
            >
              <template
                v-for="item in currentIndividualControllerCollapsibleItems"
                :key="`individual-controller-${item.individualController.id}`"
                #[`content(individual-controller-${item.individualController.id})`]="{
                  index,
                }"
              >
                <Suspense>
                  <IndividualControllerItemComponent
                    class="flex-grow-1"
                    :individualItem="
                      currentIndividualControllerCollapsibleItems[index]
                    "
                    :identifier="
                      identifier + '[' + item.individualController.id + ']'
                    "
                  />

                  <template #fallback>
                    <div class="d-flex justify-content-center">
                      <Loader
                        style="color: var(--color-primary)"
                        :size="'medium'"
                        type="infinite-spinner"
                        :theme-color="'info'"
                      />
                    </div>
                  </template>
                </Suspense>
              </template>
            </ScrollableTabComponent>

            <EmptyStateComponent v-else label="No Individual Controllers" />
          </StackLayout>
        </template>

        <template v-slot:CorporateTemplate>
          <StackLayout orientation="vertical" :gap="8">
            <KendoCard class="mb-4">
              <KendoCardBody>
                <KendoYesOrNoQuestionInputComponent
                  :id="
                    setUniqueIdentifier(
                      '-corporateControllerDetails.hasCorporateController',
                    )
                  "
                  :questionText="
                    $t('corporateControllersPage-hasCorporateControllerText', {
                      firmName: corporateItem.fullName,
                    })
                  "
                  :value="hasCorporateController"
                  :isValueReactive="true"
                  :isDataLoadedCompletely="!isInitializing"
                  @onValueChange="toggleHasCorporateController"
                />
              </KendoCardBody>
            </KendoCard>

            <ScrollableTabComponent
              v-if="hasCorporateController"
              v-model:items="corporateControllersTab"
              :addable="true"
              @add="addNewCorporateControllerItem"
              :removable="true"
              @remove="removeCorporateControllerItem"
            >
              <template
                v-for="item in currentCorporateControllerCollapsibleItems"
                :key="`corporate-controller-${item.corporateController.id}`"
                #[`content(corporate-controller-${item.corporateController.id})`]="{
                  index,
                }"
              >
                <Suspense>
                  <CorporateControllerItemComponent
                    class="flex-grow-1"
                    :individualItems="
                      currentIndividualControllerCollapsibleItems
                    "
                    :corporateItem="
                      currentCorporateControllerCollapsibleItems[index]
                    "
                    :identifier="`${identifier}[${item.corporateController.id}]`"
                  />

                  <template #fallback>
                    <div class="d-flex justify-content-center">
                      <Loader
                        style="color: var(--color-primary)"
                        :size="'medium'"
                        type="infinite-spinner"
                        :theme-color="'info'"
                      />
                    </div>
                  </template>
                </Suspense>
              </template>
            </ScrollableTabComponent>

            <EmptyStateComponent
              v-else
              label="No Corporate Controllers"
              icon="bank"
            />
          </StackLayout>
        </template>
      </TabStrip>
    </KendoAccordionItemComponent>
  </StackLayout>
</template>

<style scoped>
.template-content-container {
  display: flex;
  padding: 20px;
  flex-direction: column;
  gap: 20px;

  border-radius: 0px;
  border: 0.3px solid var(--text-text-disabled);
  background: var(--screen-background-background-primary);
}

/* for sticky anchors, will consolidate soon */
.section-item,
.section-item * {
  cursor: pointer;
}

.section-item label {
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semi-bold);
  line-height: 125%;
  /*17.5px*/
}

.section-item svg {
  width: 18px;
  height: 18px;
}

.with-vertical-gap {
  gap: 20px;
}

.with-horizontal-gap {
  gap: 15px;
}

.item-action-btn {
  border-radius: 100px;
  background-color: transparent;
  border: 1px solid var(--brand-color-brand-primary);
  color: var(--brand-color-brand-primary);
}

.directors-btn-add {
  width: 220px;
  padding: 6px 8px; /* Estimated as per design because 'height' does not work here for some reason */
}

.directors-btn-minus {
  width: 200px;
  height: 29px;
}

.scrollable-tab-card-item {
  padding: 20px;
  border: 1px solid var(--color-neutral);
}
</style>
