<script lang="ts">
import { defineAsyncComponent, defineComponent, inject } from "vue";
import { Emitter, EventType } from "mitt";
import { container } from "tsyringe";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import { IndividualControllerCollapsibleItemModel } from "@/pages/models/owners-and-controllers/IndividualControllerCollapsibleItemModel";
import StaticList from "@/infra/StaticListService";
import { IndividualControllerModel } from "@/pages/models/owners-and-controllers/IndividualControllerModel";
import { CompanyOfficerAppointment } from "@/entities/owners-and-controllers/CompanyOfficerAppointment";
import { CompanyOfficerAppointmentDetails } from "@/entities/owners-and-controllers/CompanyOfficerAppointmentDetails";
import { AppConstants } from "@/infra/AppConstants";
import { Controller } from "@/entities/firm-details/Controller";
import { FirmBasicInfo } from "@/entities/FirmBasicInfo";
import { ContactNumber } from "@/entities/ContactNumber";
import AddressInfo from "../../../models/owners-and-controllers/AddressInfo";
import { Money } from "@/entities/Money";
import BusinessIncorporation from "@/pages/models/owners-and-controllers/BusinessIncorporation";
import { CustomerEntity } from "@/entities/CustomerEntity";
import { usePageLifeCycleStore } from "@/stores/progress-bar/usePageLifeCycleStore";
import { ScrollableTabItemModel } from "@/components/models/ScrollableTabItemModel";
import ScrollableTabComponent from "@/components/ScrollableTabComponent.vue";
import { v4 as uuid } from "uuid";
import { OnboardingType } from "@/infra/base";
import { BusinessIncorporationModel } from "@/pages/ar-pages/owners-and-controllers/partials/BusinessIncorporationModel";

export default defineComponent({
  name: "EmployeeIndividualControllerItemComponent",
  components: {
    ScrollableTabComponent,
    KendoFirmFinderComponentAsync: defineAsyncComponent(
      () => import("@/components/form-fields/KendoFirmFinderComponent.vue"),
    ),
  },
  props: {
    individualItem: {
      type: Object as () => IndividualControllerCollapsibleItemModel,
      default: {
        individualController: new IndividualControllerModel(),
        isCollapsed: false,
        fullName: "",
      } as IndividualControllerCollapsibleItemModel,
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
      titles: [] as string[],
      natureOfBusinesses: [] as string[],
      dataItems: [] as IndividualControllerCollapsibleItemModel[],
      isInitializing: true,
      hasPreviousName: false,
      hasDirectorship: false,
      hasControllingInterests: false,
      isRequirePreviousAddress: false,
      inputType: this.$t("common-placeholder-text"),
      annualAmountChoices: [
        "Less than £20,000",
        "£20,000 - £50,000",
        "£50,001 - £100,000",
        "£100,001 - £500,000",
        "£500,001+",
      ],
      selectedDirectorshipTabIndex: -1,
      selectedControllingInterestTabIndex: -1,
    };
  },
  setup() {
    const pageLifeCycleStore = usePageLifeCycleStore();
    const { changeLifeCycleName } = pageLifeCycleStore;

    return {
      changeLifeCycleName,
    };
  },
  computed: {
    OnboardingType() {
      return OnboardingType;
    },
    IndividualControllerCollapsibleItemModel() {
      return IndividualControllerCollapsibleItemModel;
    },
    BusinessIncorporation() {
      return BusinessIncorporation;
    },
    Money() {
      return Money;
    },
    FirmBasicInfo() {
      return FirmBasicInfo;
    },
    ContactNumber() {
      return ContactNumber;
    },
    AppConstants() {
      return AppConstants;
    },
    directorshipTabs: {
      get() {
        return this.individualItem.individualController.directorsAndDirectorship.items.map(
          (directorship, index) => {
            return {
              id: `directorship-${directorship.id}`,
              title: directorship?.companyName ?? "<Company Name>",
              content: "",
              active: this.selectedDirectorshipTabIndex === index,
            };
          },
        );
      },
      set(tabs: ScrollableTabItemModel[]) {
        for (const index in tabs) {
          if (tabs[index].active) {
            this.selectedDirectorshipTabIndex = +index;
            break;
          }
        }
      },
    },
    controllingInterestTabs: {
      get() {
        return this.individualItem.individualController.controllingInterests.map(
          (controllingInterest, index) => {
            return {
              id: `controlling-interest-${controllingInterest.id}`,
              title: controllingInterest?.companyName ?? "<Company Name>",
              content: "",
              active: this.selectedControllingInterestTabIndex === index,
            };
          },
        );
      },
      set(tabs: ScrollableTabItemModel[]) {
        for (const index in tabs) {
          if (tabs[index].active) {
            this.selectedControllingInterestTabIndex = +index;
            break;
          }
        }
      },
    },
  },
  created() {
    this.titles = StaticList.getTitles();
    this.natureOfBusinesses = StaticList.getNatureOfBusinesses();
    // Directorships
    this.setUniqueIdForDirectorshipIfAny(
      this.individualItem?.individualController?.directorsAndDirectorship,
    );
    this.hasDirectorship =
      this.individualItem?.individualController?.directorsAndDirectorship?.items
        ?.length > 0 ?? false;
    this.selectedDirectorshipTabIndex = this.hasDirectorship ? 0 : -1;
    // Controlling Interests
    this.setUniqueIdForControllingInterestIfAny(
      this.individualItem?.individualController?.controllingInterests,
    );
    this.hasControllingInterests =
      this.individualItem?.individualController?.controllingInterests?.length >
        0 ?? false;
    this.selectedControllingInterestTabIndex = this.hasControllingInterests
      ? 0
      : -1;

    if (
      this.individualItem?.individualController?.detail
        ?.homeAddressResidenceDate ??
      false
    ) {
      if (
        !this.individualItem.individualController.detail
          .homeAddressResidenceDate
      ) {
        return;
      }

      const homeAddressStartDate = this.helperService.convertEpochToDateTime(
        this.individualItem.individualController.detail
          .homeAddressResidenceDate,
      );
      this.isRequirePreviousAddress = homeAddressStartDate
        ? this.checkIfAddressIsLessThan3Years(homeAddressStartDate)
        : false;

      if (this.isRequirePreviousAddress) {
        if (
          this.individualItem.individualController.detail.previousAddresses
            ?.length > 0 ??
          false
        ) {
          return;
        }

        if (
          !this.individualItem.individualController.detail.previousAddresses
        ) {
          (
            this.individualItem as IndividualControllerCollapsibleItemModel
          ).individualController.detail.previousAddresses = [] as AddressInfo[];
        }

        const newItem = new AddressInfo();
        newItem.maxAvailableDate = this.helperService.convertEpochToDateTime(
          this.individualItem.individualController.detail
            .homeAddressResidenceDate,
        );
        (
          this.individualItem as IndividualControllerCollapsibleItemModel
        ).individualController.detail.previousAddresses.push(newItem);
      }
    }
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameCreated);
  },
  updated() {
    this.hasDirectorship =
      this.individualItem?.individualController?.directorsAndDirectorship?.items
        ?.length > 0 ?? false;

    this.hasControllingInterests =
      this.individualItem?.individualController?.controllingInterests?.length >
        0 ?? false;
  },
  mounted() {
    this.helperService.resizeExpander();
    this.isInitializing = false;
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameMounted);
  },
  methods: {
    setUniqueIdForDirectorshipIfAny(
      directorships: CompanyOfficerAppointmentDetails,
    ) {
      if (directorships?.items?.length) {
        directorships.items.forEach((d) => {
          if (!d.id) {
            d.id = uuid();
          }
        });
      }
    },

    setUniqueIdForControllingInterestIfAny(controllingInterests: Controller[]) {
      if (controllingInterests?.length) {
        controllingInterests.forEach((d) => {
          if (!d.id) {
            d.id = uuid();
          }
        });
      }
    },

    onDateOfNameChange(value: Date, item: IndividualControllerModel) {
      item.detail.dateOfNameChange = this.helperService.dateStringToEpoch(
        value.toDateString(),
      );
    },

    convertEpochValueToDate(input: number | undefined): Date | undefined {
      if (!input) {
        return undefined;
      }

      if (input === -1) {
        return undefined;
      }

      return this.helperService.convertEpochToDateTime(input);
    },

    onHomeAddressResidenceDateChange(
      value: Date,
      item: IndividualControllerModel,
    ) {
      item.detail.homeAddressResidenceDate =
        this.helperService.dateStringToEpoch(value.toDateString());

      if (!item.detail.previousAddresses) {
        item.detail.previousAddresses = [] as AddressInfo[];
      }

      item.detail.previousAddresses.splice(0); // Clear all previous addresses

      this.isRequirePreviousAddress =
        this.checkIfAddressIsLessThan3Years(value);

      if (this.isRequirePreviousAddress) {
        const newItem = new AddressInfo();
        newItem.maxAvailableDate = value;
        item.detail.previousAddresses.push(newItem);
      }
    },

    onIndividualDateOfBirthChange(
      value: Date,
      item: IndividualControllerModel,
    ) {
      item.detail.dateOfBirth = this.helperService.dateStringToEpoch(
        value.toDateString(),
      );
    },

    isPassportNumberRequired(item: IndividualControllerModel): boolean {
      const notAvailableStr = "Not Available";
      const nAStr = "N/A";
      const nationalInsuranceNumber = item.detail?.nationalInsuranceNumber;

      return (
        nationalInsuranceNumber?.localeCompare(notAvailableStr, undefined, {
          sensitivity: "base",
        }) === 0 ||
        nationalInsuranceNumber?.localeCompare(nAStr, undefined, {
          sensitivity: "base",
        }) === 0
      );
    },

    onIndividualPercentageCapitalValueChange(
      value: number | null,
      individualController: IndividualControllerModel,
    ) {
      individualController.detail.percentageOfCapital = value;
      individualController.detail.percentageOfVotingRights = value;
    },

    onIndividualPercentageVotingRightsChange(
      value: number | null,
      individualController: IndividualControllerModel,
    ) {
      individualController.detail.percentageOfVotingRights = value;
    },

    onIndividualControllerDirectorshipStartDateChange(
      value: Date,
      item: CompanyOfficerAppointment,
    ) {
      item.directorshipStartDate = this.helperService.dateStringToEpoch(
        value.toDateString(),
      );
    },

    onIndividualControllerDirectorshipEndDateChange(
      value: Date,
      item: CompanyOfficerAppointment,
    ) {
      // value will be null if checkbox is uncheck AND orig resign on is empty
      if (!value) {
        item.directorshipEndDate = undefined;
        return;
      }

      item.directorshipEndDate = this.helperService.dateStringToEpoch(
        value.toDateString(),
      );
    },

    onBusinessIncorporationChange(
      businessIncorporation: { country: string; businessNature: string },
      item: CompanyOfficerAppointment,
    ) {
      item.country = businessIncorporation.country;
      item.natureOfBusiness = businessIncorporation.businessNature;
    },

    mapCountryAndNatureOfBusinessToBusinessIncorporation(
      item: CompanyOfficerAppointment,
    ): BusinessIncorporation {
      return {
        country:
          item.countryCode ?? AppConstants.DefaultCountryCode.toUpperCase(),
        businessNature: item.natureOfBusiness ?? "",
      };
    },

    onIndividualControllerControllingInterestPercentageCapitalValueChange(
      value: number | null,
      controllingInterest: Controller,
    ) {
      controllingInterest.percentageOfCapital = value;
      controllingInterest.percentageOfVotingRights = value;
    },

    onIndividualControllerControllingInterestPercentageVotingRightsValueChange(
      value: number | null,
      controllingInterest: Controller,
    ) {
      controllingInterest.percentageOfVotingRights = value;
    },

    onDirectorshipDetailUpdated(directorship: FirmBasicInfo, curIndex: number) {
      if (
        !this.individualItem.individualController.directorsAndDirectorship
          .items[curIndex]
      ) {
        return;
      }

      (
        this.individualItem as IndividualControllerCollapsibleItemModel
      ).individualController.directorsAndDirectorship.items[
        curIndex
      ].companyName = directorship.firmName;
      (
        this.individualItem as IndividualControllerCollapsibleItemModel
      ).individualController.directorsAndDirectorship.items[
        curIndex
      ].companyNumber = directorship.companyNumber;
      (
        this.individualItem as IndividualControllerCollapsibleItemModel
      ).individualController.directorsAndDirectorship.items[
        curIndex
      ].firmReferenceNumber = directorship.firmReferenceNumber;
      (
        this.individualItem as IndividualControllerCollapsibleItemModel
      ).individualController.directorsAndDirectorship.items[curIndex].firm =
        directorship;
    },

    mapDirectorshipToFirmModel(
      directorship: CompanyOfficerAppointment,
    ): FirmBasicInfo {
      return {
        companyNumber: directorship.companyNumber,
        firmName: directorship.companyName,
        firmReferenceNumber: directorship.firmReferenceNumber,
        fcaStatus: directorship?.firm?.fcaStatus ?? "",
        companyHouseStatus: directorship?.firm?.companyHouseStatus ?? "",
        address: "",
        tradingAddress: "",
        headOfficeAddress: "",
        contactNumber: "",
        website: "",
        sicCode: "",
        countryCode: "",
      };
    },

    onControllingInterestDetailUpdated(
      controllingInterest: FirmBasicInfo,
      curIndex: number,
    ) {
      if (
        !this.individualItem.individualController.directorsAndDirectorship
          .items[curIndex]
      ) {
        return;
      }

      if (
        controllingInterest.firmName &&
        controllingInterest.companyNumber &&
        controllingInterest.firmReferenceNumber
      ) {
        (
          this.individualItem as IndividualControllerCollapsibleItemModel
        ).individualController.controllingInterests[curIndex].companyName =
          controllingInterest.firmName;
        (
          this.individualItem as IndividualControllerCollapsibleItemModel
        ).individualController.controllingInterests[curIndex].companyNumber =
          controllingInterest.companyNumber;
        (
          this.individualItem as IndividualControllerCollapsibleItemModel
        ).individualController.controllingInterests[
          curIndex
        ].firmReferenceNumber = controllingInterest.firmReferenceNumber;
        (
          this.individualItem as IndividualControllerCollapsibleItemModel
        ).individualController.controllingInterests[curIndex].firm =
          controllingInterest;
      }
    },

    mapControllerToFirmModel(controller: Controller): FirmBasicInfo {
      return {
        firmName: controller?.companyName,
        companyNumber: controller?.companyNumber,
        firmReferenceNumber: controller?.firmReferenceNumber,
        fcaStatus: controller?.firm?.fcaStatus ?? "",
        companyHouseStatus: controller?.firm?.companyHouseStatus ?? "",
        address: "",
        tradingAddress: "",
        headOfficeAddress: "",
        contactNumber: "",
        website: "",
        sicCode: "",
        countryCode: "",
      };
    },

    convertToSafeNumber(input: string): number {
      const value = parseFloat(input);

      if (isNaN(value)) {
        return 0;
      }

      return value;
    },

    onContactNumberChange(value: ContactNumber) {
      if (this.individualItem.individualController.detail.contactNumber) {
        (
          this.individualItem as IndividualControllerCollapsibleItemModel
        ).individualController.detail.contactNumber = value;
      }
    },

    // Directorship
    onHasDirectorshipChange(value: boolean) {
      this.hasDirectorship = value;

      if (this.hasDirectorship) {
        this.onAddIndividualControllerDirectorshipItem();
      } else {
        (
          this.individualItem as IndividualControllerCollapsibleItemModel
        ).individualController.directorsAndDirectorship.items = [];
        this.selectedDirectorshipTabIndex = -1;
      }
    },

    onAddIndividualControllerDirectorshipItem() {
      const item =
        this.individualItem.individualController.directorsAndDirectorship;
      item.items.push(new CompanyOfficerAppointment());
      this.selectedDirectorshipTabIndex = item.items.length - 1;
    },

    onRemoveIndividualControllerDirectorshipItem(indexToRemove: number) {
      const item =
        this.individualItem.individualController.directorsAndDirectorship;
      item.items.splice(indexToRemove, 1);

      if (
        indexToRemove <= this.selectedDirectorshipTabIndex &&
        this.selectedDirectorshipTabIndex > 0
      ) {
        this.selectedDirectorshipTabIndex -= 1;
      }

      if (!item.items.length) {
        this.hasDirectorship = false;
      }

      this.eventBus.emit(AppConstants.formFieldChangedEvent);
    },

    // Controlling Interest
    mapControllingInterestCountryAndNatureOfBusinessToBusinessIncorporation(
      item: Controller,
    ): BusinessIncorporation {
      return {
        country:
          item.countryCode ?? AppConstants.DefaultCountryCode.toUpperCase(),
        businessNature: item.natureOfBusiness ?? "",
      };
    },

    onControllingInterestBusinessIncorporationChange(
      businessIncorporation: BusinessIncorporationModel,
      item: Controller,
    ) {
      item.countryOfIncorporation = businessIncorporation.country;
      item.natureOfBusiness = businessIncorporation.businessNature;
    },

    onHasControllingInterests(value: boolean) {
      this.hasControllingInterests = value;

      if (this.hasControllingInterests) {
        this.onAddIndividualControllerControllingInterestItem();
      } else {
        (
          this.individualItem as IndividualControllerCollapsibleItemModel
        ).individualController.controllingInterests = [];
        this.selectedControllingInterestTabIndex = -1;
      }
    },

    onAddIndividualControllerControllingInterestItem() {
      const item = this.individualItem.individualController;
      item.controllingInterests.push(new Controller());
      this.selectedControllingInterestTabIndex =
        item.controllingInterests.length - 1;
    },

    onRemoveIndividualControllerControllingInterestItem(indexToRemove: number) {
      const item = this.individualItem.individualController;
      item.controllingInterests.splice(indexToRemove, 1);

      if (
        indexToRemove <= this.selectedControllingInterestTabIndex &&
        this.selectedControllingInterestTabIndex > 0
      ) {
        this.selectedControllingInterestTabIndex -= 1;
      }

      if (!item.controllingInterests.length) {
        this.hasControllingInterests = false;
      }

      this.eventBus.emit(AppConstants.formFieldChangedEvent);
    },

    onTogglePreviousName() {
      this.hasPreviousName = !this.hasPreviousName;
    },

    checkIfAddressIsLessThan3Years(date: Date): boolean {
      const currentDate = new Date();
      const yearDiff = Math.abs(currentDate.getFullYear() - date.getFullYear());
      return yearDiff < 3;
    },

    onPreviousResidenceChange(
      value: string,
      individualController: IndividualControllerModel,
      index: number,
    ) {
      const selectedAddressInfo =
        individualController.detail.previousAddresses[index];

      if (!selectedAddressInfo.isPreviousHomeAddressChanged) {
        selectedAddressInfo.isPreviousHomeAddressChanged =
          selectedAddressInfo.previousHomeAddress !== value;
      }

      selectedAddressInfo.previousHomeAddress = value;
    },

    onPreviousResidenceStartDateChange(
      value: Date,
      individualController: IndividualControllerModel,
      index: number,
    ) {
      const selectedAddressInfo =
        individualController.detail.previousAddresses[index];
      selectedAddressInfo.previousHomeAddressResidenceDate =
        this.helperService.dateStringToEpoch(value.toDateString());

      if (this.checkIfAddressIsLessThan3Years(value)) {
        const newAddressInfo = new AddressInfo();
        newAddressInfo.maxAvailableDate = value;
        individualController.detail.previousAddresses.push(newAddressInfo);
      }
    },

    onHomeAddressChanged(value: string) {
      const selectedInfo = this.individualItem.individualController.detail;
      selectedInfo.homeAddress = value;

      if (
        !selectedInfo.originalHomeAddress &&
        !selectedInfo.isHomeAddressChanged
      ) {
        selectedInfo.isHomeAddressChanged = false;
        return;
      }

      selectedInfo.isHomeAddressChanged =
        selectedInfo.homeAddress !== selectedInfo.originalHomeAddress;
    },

    onSupportingDocumentsUploadedChange(
      individualController: IndividualControllerModel,
      url: string,
    ) {
      individualController.detail.supportingDocumentsUrls.push(url);
    },

    onCurriculumVitaeUploadedChange(
      individualController: IndividualControllerModel,
      url: string,
    ) {
      individualController.curriculumVitaeUrls.push(url);
    },

    setUniqueIdentifier(value: string): string {
      const identifier = `${AppConstants.employeeOwnersAndControllersRoute}-IC${this.identifier}${value}`;
      return this.helperService.removeStringSpacesThenSlash(identifier);
    },
  },
});
</script>

<template>
  <StackLayout
    class="template-content-container"
    v-if="individualItem?.individualController"
    orientation="vertical"
  >
    <StackLayout
      orientation="vertical"
      class="with-vertical-gap"
      :align="{ horizontal: 'start', vertical: 'middle' }"
    >
      <Label class="section-title k-color-primary flex-grow-1">{{
        $t("individualControllersPage-formTitle")
      }}</Label>

      <!-- Anchors -->
      <StackLayout
        class="with-horizontal-gap align-self-stretch"
        orientation="horizontal"
      >
        <div class="anchor-selected section-item">
          <IconComponent symbol="business-card" size="24" />

          <Label>{{ $t("individualControllersPage-tabText") }}</Label>
        </div>

        <div class="anchor section-item">
          <IconComponent symbol="hand-held-tablet-writing-77" size="24" />

          <Label>{{ $t("individualControllersPage-tabText1") }}</Label>
        </div>

        <div class="anchor section-item">
          <IconComponent symbol="user-multiple-accounts-1" size="24" />

          <Label>{{ $t("individualControllersPage-tabText2") }}</Label>
        </div>

        <div class="anchor section-item">
          <IconComponent symbol="payment-history-28" size="24" />

          <Label>{{ $t("individualControllersPage-tabText3") }}</Label>
        </div>

        <div class="anchor section-item">
          <IconComponent symbol="document-certificate-18" size="24" />

          <Label>{{ $t("individualControllersPage-tabText4") }}</Label>
        </div>
      </StackLayout>
    </StackLayout>

    <KendoAccordionItemComponent title="Individual Controller Details">
      <StackLayout orientation="vertical" class="with-vertical-gap">
        <StackLayout
          orientation="horizontal"
          class="horizontal-stack-container"
          :align="{ horizontal: 'start' }"
        >
          <KendoNameTitleComponent
            label="Title"
            :id="setUniqueIdentifier('-individualControllerModelDetail.title')"
            name="individualControllerModelDetail.title"
            :is-required="false"
            class="col"
            :value="individualItem.individualController?.detail?.title"
            @onValueChange="
              (value: string) =>
                ((
                  individualItem as IndividualControllerCollapsibleItemModel
                ).individualController.detail.title = value)
            "
          />

          <KendoGenericInputComponent
            :id="
              setUniqueIdentifier('-individualControllerModelDetail.forename')
            "
            name="individualControllerModelDetail.forename"
            class="col"
            label="Forename(s)"
            :placeholder="inputType"
            @onValueChange="
              (value: string) =>
                ((
                  individualItem as IndividualControllerCollapsibleItemModel
                ).individualController.detail.forename = value)
            "
            :value="individualItem.individualController?.detail?.forename"
            :isDataLoadedCompletely="!isInitializing"
            isValueRactive
          />

          <KendoGenericInputComponent
            :id="
              setUniqueIdentifier('-individualControllerModelDetail.surname')
            "
            name="'individualControllerModelDetail.surname"
            class="col"
            label="Surname"
            :placeholder="inputType"
            @onValueChange="
              (value: string) =>
                ((
                  individualItem as IndividualControllerCollapsibleItemModel
                ).individualController.detail.surname = value)
            "
            :value="individualItem.individualController?.detail?.surname"
            :isDataLoadedCompletely="!isInitializing"
            isValueRactive
          />

          <KendoGenericInputComponent
            :id="
              setUniqueIdentifier(
                '-individualControllerModelDetail.commonlyUsedName',
              )
            "
            name="individualControllerModelDetail.commonlyUsedName"
            class="col"
            :isRequired="false"
            label="Commonly Used Name"
            :placeholder="inputType"
            @onValueChange="
              (value: string) =>
                ((
                  individualItem as IndividualControllerCollapsibleItemModel
                ).individualController.detail.commonlyUsedName = value)
            "
            :value="
              individualItem.individualController?.detail?.commonlyUsedName
            "
            :isDataLoadedCompletely="!isInitializing"
            isValueRactive
          />
        </StackLayout>

        <StackLayout
          class="Switch"
          orientation="horizontal"
          :align="{ horizontal: 'start', vertical: 'middle' }"
        >
          <Switch
            size="small"
            :valid="true"
            :checked="hasPreviousName"
            off-label=""
            on-label=""
            @click="onTogglePreviousName"
          />

          <Label class="has-previous-name-text">{{
            $t("with-previous-name")
          }}</Label>
        </StackLayout>

        <StackLayout
          class="with-horizontal-gap"
          v-if="hasPreviousName"
          orientation="horizontal"
          :align="{ horizontal: 'start' }"
        >
          <KendoGenericInputComponent
            :id="
              setUniqueIdentifier(
                '-individualControllerModelDetail.previousFullName',
              )
            "
            name="individualControllerModelDetail.previousFullName"
            class="col"
            label="Previous Name"
            :isRequired="hasPreviousName"
            @onValueChange="
              (value: string) =>
                ((
                  individualItem as IndividualControllerCollapsibleItemModel
                ).individualController.detail.previousFullName = value)
            "
            :value="
              individualItem.individualController?.detail?.previousFullName
            "
            placeholder="Joe"
            :isDataLoadedCompletely="!isInitializing"
            isValueRactive
          />

          <KendoGenericInputComponent
            :id="
              setUniqueIdentifier(
                '-individualControllerModelDetail.reasonForChangeName',
              )
            "
            name="individualControllerModelDetail.reasonForChangeName"
            class="col"
            :isRequired="hasPreviousName"
            label="Reason for Name Change"
            placeholder="[Reason]"
            @onValueChange="
              (value: string) =>
                ((
                  individualItem as IndividualControllerCollapsibleItemModel
                ).individualController.detail.reasonForChangeName = value)
            "
            :value="
              individualItem.individualController?.detail?.reasonForChangeName
            "
            :isDataLoadedCompletely="!isInitializing"
            isValueRactive
          />

          <KendoDatePickerInputComponent
            :id="
              setUniqueIdentifier(
                '-individualControllerModelDetail.dateOfNameChange',
              )
            "
            name="individualControllerModelDetail.dateOfNameChange"
            class="col"
            :isRequired="hasPreviousName"
            label="Date of Name Change"
            @onValueChange="
              (value: Date) =>
                onDateOfNameChange(value, individualItem.individualController)
            "
            :value="
              convertEpochValueToDate(
                individualItem.individualController?.detail?.dateOfNameChange ??
                  undefined,
              )
            "
            :isDataLoadedCompletely="!isInitializing"
            isValueRactive
          />
        </StackLayout>

        <Label
          v-if="isRequirePreviousAddress"
          class="paragraph-l-figtree-semi-bold"
          >Address(es)</Label
        >

        <StackLayout
          class="with-horizontal-gap"
          orientation="horizontal"
          :align="{ horizontal: 'start' }"
        >
          <KendoPostCodeInputComponent
            :id="
              setUniqueIdentifier(
                '-individualControllerModelDetail.homeAddress',
              )
            "
            name="individualControllerModelDetail.homeAddress"
            label="Home Address"
            :isUserModified="
              individualItem.individualController.detail.isHomeAddressChanged
            "
            class="col-9"
            @onValueChange="(value: string) => onHomeAddressChanged(value)"
            :value="individualItem.individualController?.detail?.homeAddress"
            :isDataLoadedCompletely="!isInitializing"
            isValueRactive
          />

          <KendoDatePickerInputComponent
            :id="
              setUniqueIdentifier(
                '-individualControllerModelDetail.homeAddressResidenceDate',
              )
            "
            name="individualControllerModelDetail.homeAddressResidenceDate"
            label="Residence Start Date"
            class="col"
            @onValueChange="
              (value: Date) =>
                onHomeAddressResidenceDateChange(
                  value,
                  individualItem.individualController,
                )
            "
            :value="
              convertEpochValueToDate(
                individualItem.individualController?.detail
                  ?.homeAddressResidenceDate ?? undefined,
              )
            "
            :max-date="new Date()"
            :isDataLoadedCompletely="!isInitializing"
            isValueRactive
          />
        </StackLayout>

        <div v-if="isRequirePreviousAddress">
          <StackLayout
            v-for="(address, index) of individualItem.individualController
              .detail.previousAddresses"
            :key="index"
            style="gap: 15px"
            orientation="horizontal"
            :align="{ horizontal: 'start' }"
          >
            <KendoPostCodeInputComponent
              :id="
                setUniqueIdentifier(
                  `-${index}` +
                    '-individualControllerModelDetail.previousHomeAddress',
                )
              "
              name="individualControllerModelDetail.previousHomeAddress"
              class="col-9"
              label="Previous Home Address"
              :isUserModified="
                individualItem.individualController.detail.previousAddresses[
                  index
                ].isPreviousHomeAddressChanged
              "
              @onValueChange="
                (value: string) =>
                  onPreviousResidenceChange(
                    value,
                    individualItem.individualController,
                    index,
                  )
              "
              :value="address.previousHomeAddress"
              :isDataLoadedCompletely="!isInitializing"
              isValueRactive
            />

            <KendoDatePickerInputComponent
              :id="
                setUniqueIdentifier(
                  `-${index}` +
                    '-individualControllerModelDetail.previousHomeAddressResidenceDate',
                )
              "
              name="individualControllerModelDetail.previousHomeAddressResidenceDate"
              label="Residence Start Date"
              class="col"
              @onValueChange="
                (value: Date) =>
                  onPreviousResidenceStartDateChange(
                    value,
                    individualItem.individualController,
                    index,
                  )
              "
              :value="
                convertEpochValueToDate(
                  address.previousHomeAddressResidenceDate ?? undefined,
                )
              "
              :max-date="address.maxAvailableDate"
              :isDataLoadedCompletely="!isInitializing"
              isValueRactive
            />
          </StackLayout>
        </div>

        <StackLayout
          class="with-horizontal-gap"
          orientation="horizontal"
          :align="{ horizontal: 'start' }"
        >
          <KendoDatePickerInputComponent
            :id="
              setUniqueIdentifier(
                '-individualControllerModelDetail.dateOfBirth',
              )
            "
            name="individualControllerModelDetail.dateOfBirth"
            class="col"
            label="Date of Birth"
            @onValueChange="
              (value: Date) =>
                onIndividualDateOfBirthChange(
                  value,
                  individualItem.individualController,
                )
            "
            :value="
              convertEpochValueToDate(
                individualItem.individualController.detail.dateOfBirth,
              )
            "
            :max-date="helperService.getDateForGivenYearsAgo(18)"
            :isDataLoadedCompletely="!isInitializing"
            isValueRactive
          />

          <KendoCountryComponent
            :id="
              setUniqueIdentifier(
                '-individualControllerModelDetail.countryOfBirth',
              )
            "
            name="individualControllerModelDetail.countryOfBirth"
            class="col"
            label="Country of Birth"
            :value="individualItem.individualController?.detail?.countryOfBirth"
            @onValueChange="
              (value: string) =>
                ((
                  individualItem as IndividualControllerCollapsibleItemModel
                ).individualController.detail.countryOfBirth = value)
            "
            :isDataLoadedCompletely="!isInitializing"
            isValueRactive
          />

          <KendoNationalityInputComponent
            :id="
              setUniqueIdentifier(
                '-individualControllerModelDetail.currentNationalities',
              )
            "
            name="individualControllerModelDetail.currentNationalities"
            class="col"
            label="Nationalities (Current)"
            placeholder="Please select all that apply"
            @onValueChange="
              (value: string[]) =>
                ((
                  individualItem as IndividualControllerCollapsibleItemModel
                ).individualController.detail.nationalities = value)
            "
            :value="individualItem.individualController?.detail?.nationalities"
            :isDataLoadedCompletely="!isInitializing"
            isValueRactive
          />

          <KendoNationalityInputComponent
            :id="
              setUniqueIdentifier(
                '-individualControllerModelDetail.previousNationalities',
              )
            "
            name="individualControllerModelDetail.previousNationalities"
            class="col"
            label="Nationalities (Previous)"
            placeholder="Please select all that apply"
            :isRequired="false"
            optionalText="If applicable"
            @onValueChange="
              (value: string[]) =>
                ((
                  individualItem as IndividualControllerCollapsibleItemModel
                ).individualController.detail.previousNationalities = value)
            "
            :value="
              individualItem.individualController?.detail?.previousNationalities
            "
            :isDataLoadedCompletely="!isInitializing"
            isValueRactive
          />
        </StackLayout>

        <StackLayout
          class="with-horizontal-gap"
          orientation="horizontal"
          :align="{ horizontal: 'start' }"
        >
          <KendoGenericInputComponent
            :id="
              setUniqueIdentifier(
                '-individualControllerModelDetail.nationalInsuranceNumber',
              )
            "
            name="individualControllerModelDetail.nationalInsuranceNumber'"
            class="col"
            label="National Insurance Number"
            placeholder="LLNNNNNNL"
            @onValueChange="
              (value: string) =>
                ((
                  individualItem as IndividualControllerCollapsibleItemModel
                ).individualController.detail.nationalInsuranceNumber = value)
            "
            :value="
              individualItem.individualController?.detail
                ?.nationalInsuranceNumber
            "
            :isByPassGenericPlaceHolder="true"
            :isDataLoadedCompletely="!isInitializing"
            isValueRactive
          />

          <KendoGenericInputComponent
            :id="
              setUniqueIdentifier(
                '-individualControllerModelDetail.passportNumber',
              )
            "
            name="individualControllerModelDetail.passportNumber"
            class="col"
            label="Passport Number"
            placeholder="NNNNNNNN"
            optionalText="(if National Insurance Number is not available)"
            :isRequired="
              isPassportNumberRequired(individualItem.individualController)
            "
            @onValueChange="
              (value: string) =>
                ((
                  individualItem as IndividualControllerCollapsibleItemModel
                ).individualController.detail.passportNumber = value)
            "
            :value="individualItem.individualController?.detail?.passportNumber"
            :isByPassGenericPlaceHolder="true"
            :isDataLoadedCompletely="!isInitializing"
            isValueRactive
          />
        </StackLayout>

        <StackLayout
          class="with-horizontal-gap"
          orientation="horizontal"
          :align="{ horizontal: 'start' }"
        >
          <KendoPercentageInputComponent
            :id="
              setUniqueIdentifier(
                '-individualControllerModelDetail.percentageOfCapital',
              )
            "
            name="individualControllerModelDetail.percentageOfCapital"
            class="col"
            :minLimit="10"
            label="Percentage of Capital (Ownership)"
            placeholder="%"
            @onValueChange="
              (value: number | null) =>
                onIndividualPercentageCapitalValueChange(
                  value,
                  individualItem.individualController,
                )
            "
            :value="
              individualItem.individualController.detail?.percentageOfCapital
            "
            :isDataLoadedCompletely="!isInitializing"
            isValueRactive
          />

          <KendoPercentageInputComponent
            :id="
              setUniqueIdentifier(
                '-individualControllerModelDetail.percentageOfVotingRights',
              )
            "
            name="individualControllerModelDetail.percentageOfVotingRights"
            class="col"
            :minLimit="10"
            label="Percentage of Voting Rights"
            placeholder="%"
            @onValueChange="
              (value: number | null) =>
                onIndividualPercentageVotingRightsChange(
                  value,
                  individualItem.individualController,
                )
            "
            :value="
              individualItem.individualController?.detail
                ?.percentageOfVotingRights
            "
            :isDataLoadedCompletely="!isInitializing"
            isValueRactive
          />
        </StackLayout>

        <StackLayout
          class="with-horizontal-gap"
          orientation="horizontal"
          :align="{ horizontal: 'start' }"
        >
          <KendoEmailAddressInputComponent
            :id="
              setUniqueIdentifier(
                '-individualControllerModelDetail.emailAddress',
              )
            "
            name="individualControllerModelDetail.emailAddress"
            class="col"
            @onValueChange="
              (value: string) =>
                ((
                  individualItem as IndividualControllerCollapsibleItemModel
                ).individualController.detail.emailAddress = value)
            "
            :value="individualItem.individualController?.detail?.emailAddress"
            :isDataLoadedCompletely="!isInitializing"
            isValueRactive
          />

          <KendoTelInputComponent
            :id="
              setUniqueIdentifier(
                '-individualControllerModelDetail.contactNumber',
              )
            "
            name="individualControllerModelDetail.contactNumber"
            class="col"
            label="Contact Number"
            :value="
              individualItem.individualController?.detail?.contactNumber ??
              new ContactNumber()
            "
            @onValueChange="onContactNumberChange"
            :isDataLoadedCompletely="!isInitializing"
            isValueReactive
          />
        </StackLayout>

        <KendoYesOrNoQuestionInputComponent
          :id="
            setUniqueIdentifier(
              '-individualControllerModelDetail.hasBeenSubjectToComplaints',
            )
          "
          isRequired
          isValueReactive
          :isDataLoadedCompletely="!isInitializing"
          @onValueChange="
            (value: boolean) =>
              ((
                individualItem as IndividualControllerCollapsibleItemModel
              ).individualController.detail.hasBeenSubjectToAnyMaterialComplaints =
                value)
          "
          :questionText="
            'Has ' +
            individualItem.individualController?.detail?.forename +
            ' ' +
            individualItem.individualController?.detail?.surname +
            ' been subject to any material complaints made against them by their customers or previous customers in the last 5 years, which are awaiting determination by, or have been upheld by, an ombudsman?'
          "
          :value="
            individualItem.individualController?.detail
              ?.hasBeenSubjectToAnyMaterialComplaints
          "
        />

        <StackLayout
          v-if="
            individualItem.individualController.detail
              .hasBeenSubjectToAnyMaterialComplaints
          "
          orientation="vertical"
          class="with-vertical-gap"
        >
          <KendoTextAreaInputComponent
            :id="
              setUniqueIdentifier(
                '-individualControllerDetail.additionalInformation',
              )
            "
            name="individualControllerDetail.additionalInformation"
            label="Additional Information"
            corner-radius="8"
            :placeholder="inputType"
            @onValueChange="
              (value: string) =>
                ((
                  individualItem as IndividualControllerCollapsibleItemModel
                ).individualController.detail.additionalInformation = value)
            "
            :value="
              individualItem.individualController?.detail?.additionalInformation
            "
            :isDataLoadedCompletely="!isInitializing"
            isValueRactive
          />

          <KendoUploadInputComponent
            :isRequired="false"
            label="Supporting Documents"
            optional-text="(Optional) (Accepted file types: pdf, jpg, doc, Maximum 25 MB file size)"
            stretched
            multiple
            dropzone
            buttonText="Upload file"
            :disabled="!registeredCustomer.companyNumber ?? true"
            :allowed-extensions="['.pdf', 'doc', 'docx']"
            :uploadFor="`IndividualControllersSupportingDocuments-${registeredCustomer.companyNumber}`"
            @onFinishedUpload="
              (url: string) =>
                onSupportingDocumentsUploadedChange(
                  individualItem.individualController,
                  url,
                )
            "
          />
        </StackLayout>
      </StackLayout>
    </KendoAccordionItemComponent>

    <KendoAccordionItemComponent
      :title="
        individualItem.individualController.detail &&
        individualItem.individualController.detail.forename &&
        individualItem.individualController.detail.surname
          ? `Directorships of ${individualItem.individualController.detail.forename} ${individualItem.individualController.detail.surname}`
          : 'Directorships of &lt;Individual Controller Name&gt;'
      "
    >
      <StackLayout orientation="vertical" class="with-vertical-gap">
        <KendoYesOrNoQuestionInputComponent
          :id="
            setUniqueIdentifier(
              '-individualControllerModelDetail.hasDirectorship',
            )
          "
          :questionText="`Does ${individualItem.individualController?.detail?.forename} ${individualItem.individualController?.detail?.surname} have any Directorship in the UK or overseas?`"
          :value="hasDirectorship"
          @onValueChange="onHasDirectorshipChange"
        />

        <Label v-if="hasDirectorship" class="section-description">
          Please provide details of all (UK and Overseas) directorships of
          {{
            individualItem.individualController?.detail?.forename ??
            "&lt;Forename&gt;"
          }}
          {{
            individualItem.individualController?.detail?.surname ??
            "&lt;Surname&gt;"
          }}
          currently held or held previously at any point within the last 10
          years. (Where pre-populated below, please ensure that you add any
          details or entries that are missing, and correct any that are
          inaccurate).
        </Label>

        <ScrollableTabComponent
          v-if="
            hasDirectorship &&
            individualItem.individualController.directorsAndDirectorship
          "
          v-model:items="directorshipTabs"
          @add="onAddIndividualControllerDirectorshipItem"
          @remove="
            (index: number) =>
              onRemoveIndividualControllerDirectorshipItem(index)
          "
          addable
          removable
        >
          <template
            v-for="directorship in individualItem.individualController
              .directorsAndDirectorship.items"
            :key="`directorship-${directorship.id}`"
            #[`content(directorship-${directorship.id})`]="{ index }"
          >
            <Card class="col with-vertical-gap scrollable-tab-card-item">
              <Label class="section-title k-color-primary flex-grow-1"
                >Directorship Details</Label
              >

              <KendoFirmFinderComponentAsync
                :id="
                  setUniqueIdentifier(
                    `-directorship${directorship.id}` +
                      '-individualController.companyName',
                  )
                "
                companyNameLabel="Company Name"
                :company="mapDirectorshipToFirmModel(directorship)"
                :companyName="directorship?.companyName"
                :companyNumber="directorship?.companyNumber"
                :firmReferenceNumber="directorship?.firmReferenceNumber"
                :isInitializing="isInitializing"
                @onCompanyDetailUpdated="
                  (value: FirmBasicInfo) => {
                    onDirectorshipDetailUpdated(value, index);
                  }
                "
                :onboardingType="OnboardingType.Employee.toString()"
              />

              <KendoBusinessIncorporationComponent
                :id="
                  setUniqueIdentifier(
                    `-directorship${directorship.id}` +
                      '-individualController.businessNature',
                  )
                "
                :isInitializing="isInitializing"
                :value="
                  mapCountryAndNatureOfBusinessToBusinessIncorporation(
                    directorship,
                  )
                "
                @onBusinessIncorporationChange="
                  (businessNature: BusinessIncorporation) =>
                    onBusinessIncorporationChange(businessNature, directorship)
                "
              />

              <StackLayout
                orientation="horizontal"
                :align="{ horizontal: 'start' }"
                class="with-horizontal-gap"
              >
                <KendoDatePickerInputComponent
                  :id="
                    setUniqueIdentifier(
                      `-directorship${directorship.id}` +
                        '-individualDirectorship-directorshipStartDate',
                    )
                  "
                  :name="`individualDirectorship-directorshipStartDate${directorship.id}`"
                  class="col"
                  label="Directorship Start Date"
                  @onValueChange="
                    (value: Date) =>
                      onIndividualControllerDirectorshipStartDateChange(
                        value,
                        directorship,
                      )
                  "
                  :value="
                    convertEpochValueToDate(
                      directorship?.directorshipStartDate ?? undefined,
                    )
                  "
                  :isDataLoadedCompletely="!isInitializing"
                  isValueRactive
                />

                <KendoDatePickerInputComponent
                  :id="
                    setUniqueIdentifier(
                      `-directorship${directorship.id}` +
                        '-individualDirectorship-directorshipEndDate',
                    )
                  "
                  :name="`individualDirectorship-directorshipEndDate${directorship.id}`"
                  class="col"
                  label="Directorship End Date"
                  isShowPresentDate="true"
                  :minDate="
                    convertEpochValueToDate(
                      directorship?.directorshipStartDate ?? undefined,
                    )
                  "
                  :origDateString="directorship?.originalResignedOn"
                  @onValueChange="
                    (value: Date) =>
                      onIndividualControllerDirectorshipEndDateChange(
                        value,
                        directorship,
                      )
                  "
                  :value="
                    convertEpochValueToDate(
                      directorship?.directorshipEndDate ?? undefined,
                    )
                  "
                  :isDataLoadedCompletely="!isInitializing"
                  isValueRactive
                />
              </StackLayout>
            </Card>
          </template>
        </ScrollableTabComponent>
      </StackLayout>
    </KendoAccordionItemComponent>

    <KendoAccordionItemComponent
      :title="
        individualItem.individualController.detail &&
        individualItem.individualController.detail.forename &&
        individualItem.individualController.detail.surname
          ? `Other Controlling Interests of ${individualItem.individualController.detail.forename} ${individualItem.individualController.detail.surname}`
          : 'Other Controlling Interests of &lt;Individual Controller Name&gt;'
      "
    >
      <StackLayout orientation="vertical" class="with-vertical-gap">
        <KendoYesOrNoQuestionInputComponent
          :id="
            setUniqueIdentifier(
              '-individualControllerModelDetail.hasControllingInterests',
            )
          "
          :questionText="`Does ${individualItem.individualController?.detail?.forename} ${individualItem.individualController?.detail?.surname} have Other Controlling Interests?`"
          :value="hasControllingInterests"
          @onValueChange="onHasControllingInterests"
          :isDataLoadedCompletely="!isInitializing"
          isValueRactive
        />

        <ScrollableTabComponent
          v-if="hasControllingInterests"
          v-model:items="controllingInterestTabs"
          @add="onAddIndividualControllerControllingInterestItem"
          @remove="onRemoveIndividualControllerControllingInterestItem"
          addable
          removable
        >
          <template
            v-for="controllingInterest in individualItem.individualController
              .controllingInterests"
            :key="`controlling-interest-${controllingInterest.id}`"
            #[`content(controlling-interest-${controllingInterest.id})`]="{
              index,
            }"
          >
            <Card class="col with-vertical-gap scrollable-tab-card-item">
              <Label class="section-title k-color-primary flex-grow-1"
                >Controlling Interest Details</Label
              >

              <KendoFirmFinderComponentAsync
                :id="
                  setUniqueIdentifier(
                    `-controllingInterest-${controllingInterest.id}` +
                      '-individualControllerControllingInterest-companyName',
                  )
                "
                companyNameLabel="Company Name"
                :company="mapControllerToFirmModel(controllingInterest)"
                :companyName="controllingInterest?.companyName"
                :companyNumber="controllingInterest?.companyNumber"
                :firmReferenceNumber="controllingInterest?.firmReferenceNumber"
                :isInitializing="isInitializing"
                @onCompanyDetailUpdated="
                  (value: FirmBasicInfo) => {
                    onControllingInterestDetailUpdated(value, index);
                  }
                "
                :onboardingType="OnboardingType.Employee.toString()"
              />

              <KendoBusinessIncorporationComponent
                :id="
                  setUniqueIdentifier(
                    `-controllingInterest-${controllingInterest.id}`,
                  )
                "
                :isInitializing="isInitializing"
                :value="
                  mapControllingInterestCountryAndNatureOfBusinessToBusinessIncorporation(
                    controllingInterest,
                  )
                "
                @onBusinessIncorporationChange="
                  (businessNature: BusinessIncorporation) =>
                    onControllingInterestBusinessIncorporationChange(
                      businessNature,
                      controllingInterest,
                    )
                "
              />

              <StackLayout
                orientation="horizontal"
                :align="{ horizontal: 'stretch' }"
                class="with-horizontal-gap"
              >
                <KendoPercentageInputComponent
                  :id="
                    setUniqueIdentifier(
                      `-controllingInterest-${controllingInterest.id}` +
                        '-individualControllerControllingInterest-percentageOfCapital',
                    )
                  "
                  :name="`individualControllerControllingInterest-percentageOfCapital-${controllingInterest.id}`"
                  label="Percentage of Capital (Ownership)"
                  placeholder="%"
                  :minLimit="10"
                  @onValueChange="
                    (value: number | null) =>
                      onIndividualControllerControllingInterestPercentageCapitalValueChange(
                        value,
                        controllingInterest,
                      )
                  "
                  :value="controllingInterest?.percentageOfCapital"
                  :isDataLoadedCompletely="!isInitializing"
                  isValueRactive
                />

                <KendoPercentageInputComponent
                  :id="
                    setUniqueIdentifier(
                      `-controllingInterest-${controllingInterest.id}` +
                        '-individualControllerControllingInterest-percentageOfVotingRights',
                    )
                  "
                  :name="`individualControllerControllingInterest-percentageOfVotingRights-${controllingInterest.id}`"
                  label="Percentage of Voting Rights"
                  placeholder="%"
                  :minLimit="10"
                  @onValueChange="
                    (value: number | null) =>
                      onIndividualControllerControllingInterestPercentageVotingRightsValueChange(
                        value,
                        controllingInterest,
                      )
                  "
                  :value="controllingInterest?.percentageOfVotingRights"
                  :isDataLoadedCompletely="!isInitializing"
                  isValueRactive
                />
              </StackLayout>
            </Card>
          </template>
        </ScrollableTabComponent>
      </StackLayout>
    </KendoAccordionItemComponent>

    <KendoAccordionItemComponent
      :title="
        individualItem.individualController.detail &&
        individualItem.individualController.detail.forename &&
        individualItem.individualController.detail.surname
          ? `Financial Status of ${individualItem.individualController.detail.forename} ${individualItem.individualController.detail.surname}`
          : 'Financial Status of &lt;Individual Controller Name&gt;'
      "
    >
      <StackLayout orientation="vertical" class="with-vertical-gap">
        <Label class="section-description">
          Please provide details of
          {{
            individualItem.individualController?.detail?.forename ??
            "&lt;Forename&gt;"
          }}
          {{
            individualItem.individualController?.detail?.surname ??
            "&lt;Surname&gt;"
          }}’s financial position and strength, including details of source(s)
          of revenue, assets and liabilities, guarantees, etc.
        </Label>

        <StackLayout
          class="with-horizontal-gap"
          orientation="horizontal"
          :align="{ horizontal: 'start' }"
        >
          <KendoDropDownInputComponent
            :id="
              setUniqueIdentifier(
                '-individualControllerFinancialStatus-annualIncome',
              )
            "
            name="individualControllerFinancialStatus-annualIncome"
            class="col"
            label="Annual Income"
            placeholder="Please Select"
            :value="
              individualItem.individualController?.financialStatus?.annualIncome
            "
            @onValueChange="
              (value: string) =>
                ((
                  individualItem as IndividualControllerCollapsibleItemModel
                ).individualController.financialStatus.annualIncome = value)
            "
            :data-items="annualAmountChoices"
            :isDataLoadedCompletely="!isInitializing"
            isValueRactive
          />

          <KendoGenericInputComponent
            :id="
              setUniqueIdentifier(
                '-individualControllerFinancialStatus-sourceOfIncome',
              )
            "
            name="individualControllerFinancialStatus-sourceOfIncome"
            class="col"
            label="Source of Income"
            @onValueChange="
              (value: string) =>
                ((
                  individualItem as IndividualControllerCollapsibleItemModel
                ).individualController.financialStatus.sourceOfIncome = value)
            "
            :value="
              individualItem.individualController?.financialStatus
                ?.sourceOfIncome
            "
            :isDataLoadedCompletely="!isInitializing"
            isValueRactive
          />
        </StackLayout>

        <StackLayout
          class="with-horizontal-gap"
          orientation="horizontal"
          :align="{ horizontal: 'start' }"
        >
          <KendoDropDownInputComponent
            :id="
              setUniqueIdentifier(
                '-individualControllerFinancialStatus-totalAssets',
              )
            "
            name="individualControllerFinancialStatus-totalAssets"
            class="col"
            label="Total Assets"
            placeholder="Please Select"
            @onValueChange="
              (value: string) =>
                ((
                  individualItem as IndividualControllerCollapsibleItemModel
                ).individualController.financialStatus.totalAssets = value)
            "
            :value="
              individualItem.individualController?.financialStatus?.totalAssets
            "
            :data-items="annualAmountChoices"
            :isDataLoadedCompletely="!isInitializing"
            isValueRactive
          />

          <KendoDropDownInputComponent
            :id="
              setUniqueIdentifier(
                '-individualControllerFinancialStatus-totalLiabilities',
              )
            "
            name="individualControllerFinancialStatus-totalLiabilities"
            class="col"
            label="Total Liabilities"
            placeholder="Please Select"
            @onValueChange="
              (value: string) =>
                ((
                  individualItem as IndividualControllerCollapsibleItemModel
                ).individualController.financialStatus.totalLiabilities = value)
            "
            :value="
              individualItem.individualController?.financialStatus
                ?.totalLiabilities
            "
            :data-items="annualAmountChoices"
            :isDataLoadedCompletely="!isInitializing"
            isValueRactive
          />

          <KendoCurrencyInputComponent
            :id="
              setUniqueIdentifier(
                '-individualControllerFinancialStatus-totalAmountToActAsGuarantor',
              )
            "
            name="individualControllerFinancialStatus-totalAmountToActAsGuarantor"
            label="Total Amount as a Guarantor"
            optionalText="(If Applicable)"
            class="col"
            :isRequired="false"
            :value="
              individualItem.individualController?.financialStatus
                ?.totalAmountToActAsGuarantor
            "
            :isDataLoadedCompletely="!isInitializing"
            isValueReactive
            @onValueChange="
              (value: Money) =>
                ((
                  individualItem as IndividualControllerCollapsibleItemModel
                ).individualController.financialStatus.totalAmountToActAsGuarantor =
                  value)
            "
          />
        </StackLayout>

        <KendoTextAreaInputComponent
          :id="
            setUniqueIdentifier(
              '-individualControllerFinancialStatus-additionalInformation',
            )
          "
          name="individualControllerFinancialStatus-additionalInformation"
          class="flex-grow-1"
          label="Additional Information"
          :cornerRadius="8"
          :isRequired="false"
          @onValueChange="
            (value: string) =>
              ((
                individualItem as IndividualControllerCollapsibleItemModel
              ).individualController.financialStatus.additionalInformation =
                value)
          "
          :value="
            individualItem.individualController?.financialStatus
              ?.additionalInformation
          "
          :isDataLoadedCompletely="!isInitializing"
          isValueRactive
        />
      </StackLayout>
    </KendoAccordionItemComponent>

    <KendoAccordionItemComponent
      :title="
        individualItem.individualController.detail &&
        individualItem.individualController.detail.forename &&
        individualItem.individualController.detail.surname
          ? `Curriculum Vitae of ${individualItem.individualController.detail.forename} ${individualItem.individualController.detail.surname}`
          : 'Curriculum Vitae of &lt;Individual Controller Name&gt;'
      "
    >
      <StackLayout orientation="vertical" style="gap: 15px">
        <Label class="section-description section-description-upload-cv">
          Please upload an up to date CV which includes a description of
          {{ individualItem.individualController?.detail?.forename ?? "" }}
          {{ individualItem.individualController?.detail?.surname ?? "" }}'s
          current and past professional activities. (Accepted file types: pdf,
          jpg, doc. Maximum 25 MB file size)
        </Label>

        <KendoUploadInputComponent
          stretched
          multiple
          dropzone
          buttonText="Upload CV"
          :disabled="!registeredCustomer.companyNumber ?? true"
          :allowed-extensions="['.pdf', 'doc', 'docx']"
          :uploadFor="`IndividualControllersCV-${registeredCustomer.companyNumber}`"
          @onFinishedUpload="
            (url: string) =>
              onCurriculumVitaeUploadedChange(
                individualItem.individualController,
                url,
              )
          "
        />
      </StackLayout>
    </KendoAccordionItemComponent>
  </StackLayout>
</template>

<style scoped>
.with-vertical-gap {
  gap: 20px;
}

.with-horizontal-gap {
  gap: 15px;
}

.template-content-container {
  display: flex;
  padding: 20px;
  flex-direction: column;
  gap: 20px;

  border-radius: 0px;
  border: 0.3px solid var(--text-text-disabled);
  background: var(--screen-background-background-primary);
}

.scrollable-tab-card-item {
  padding: 20px;
  border: 1px solid var(--color-neutral);
}

.paragraph-l-figtree-semi-bold {
  color: var(--color-black);
  font-size: var(--font-size-lg);
  font-style: normal;
  font-weight: var(--font-weight-semi-bold);
  line-height: 125%; /* 22.5px */
}

.horizontal-stack-container {
  gap: 15px;
}

.section-description {
  color: black; /*var(--text-text-primary);*/
  text-align: justify;

  font-size: var(--font-size-default);
  font-style: normal;
  font-weight: var(--font-weight-medium);
  line-height: 130%; /* 20.8px */
}

.section-description-upload-cv {
  font-weight: var(--font-weight-normal);
}

/* for sticky anchors, will consolidate soon */
.section-item,
.section-item * {
  cursor: pointer;
}

.section-item label {
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semi-bold);
  line-height: 125%; /*17.5px*/
}

.section-item svg {
  width: 18px;
  height: 18px;
}

.has-previous-name-text {
  color: var(--text-text-secondary);
  font-size: var(--font-size-sm);
  font-style: normal;
  font-weight: var(--font-weight-medium);
  line-height: 125%; /* 17.5px */
}
</style>
