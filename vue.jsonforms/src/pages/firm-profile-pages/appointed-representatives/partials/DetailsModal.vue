<script lang="ts">
import { defineAsyncComponent, defineComponent, inject, ref } from "vue";
import { container } from "tsyringe";
import moment from "moment";
import { AppointedRepresentative } from "@/entities/appointed-representatives/AppointedRepresentative";
import { ContactNumber } from "@/entities/ContactNumber";
import { FirmBasicInfo } from "@/entities/FirmBasicInfo";
import { ProfileStatuses } from "@/entities/enums/ProfileStatuses";
import { AppConstants } from "@/infra/AppConstants";
import { OnboardingType } from "@/infra/base";
import {
  IAppointedRepresentativeService,
  IAppointedRepresentativeServiceInfo,
} from "@/infra/dependency-services/rest/appointed-representative/IAppointedRepresentativeService";
import {
  IFcaService,
  IFcaServiceInfo,
} from "@/infra/dependency-services/rest/fca/IFcaService";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import StaticList from "@/infra/StaticListService";
import { AlertType, useAlert } from "@/composables/useAlert";
import {
  NotificationType,
  useNotification,
} from "@/composables/useNotification";
import { useAppointedRepresentativesStore } from "@/stores/useAppointedRepresentativeStore";
import { useCustomerStore } from "@/stores/useCustomerStore";
import { CollapsiblePanelItem } from "@/components/CollapsiblePanelComponent.vue";
import { NavPillItem } from "@/components/NavPillComponent.vue";
import {AppointedRepresentativeActivity} from "@/entities/appointed-representatives/AppointedRepresentativeActivity";

export default defineComponent({
  name: "DetailsModal",
  props: {
    value: {
      type: Object as () => AppointedRepresentative,
      default: new AppointedRepresentative(),
    },
    viewMode: {
      type: Boolean,
      default: false,
    },
    editMode: {
      type: Boolean,
      default: false,
    },
  },
  components: {
    Activities: defineAsyncComponent(() => import("./Activities.vue")),
    FeesChargeTable: defineAsyncComponent(
      () => import("./FeesChargeTable.vue"),
    ),
    KendoFirmFinderComponentAsync: defineAsyncComponent(
      () => import("@/components/form-fields/KendoFirmFinderComponent.vue"),
    ),
  },
  data() {
    return {
      fcaService: container.resolve<IFcaService>(IFcaServiceInfo.name),
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      appointedRepresentativeService:
        container.resolve<IAppointedRepresentativeService>(
          IAppointedRepresentativeServiceInfo.name,
        ),
      activePanels: ["ar-details"],
      isLoading: false,
      isInitializing: true,
      valueInternal: this.value,
    };
  },
  computed: {
    AppointedRepresentativeActivity() {
      return AppointedRepresentativeActivity;
    },
    ContactNumber() {
      return ContactNumber;
    },
    OnboardingType() {
      return OnboardingType;
    },
    FirmBasicInfo() {
      return FirmBasicInfo;
    },

    AppointedRepresentative() {
      return AppointedRepresentative;
    },

    navPills() {
      return [
        {
          id: "ar-details",
          label: "Appointed Representative Details",
          icon: "text-file-17",
          anchorTo: "panel-ar-details",
          active: this.activePanels.includes("ar-details"),
        },
        {
          id: "ar-rep-details",
          label: "Representative Details",
          icon: "user-protection-check",
          anchorTo: "panel-ar-rep-details",
          active: this.activePanels.includes("ar-rep-details"),
        },
        {
          id: "activities",
          label: "Activities",
          icon: "task-list-35",
          anchorTo: "panel-activities",
          active: this.activePanels.includes("activities"),
        },
        {
          id: "fees",
          label: "Fees",
          icon: "currency-pound-circle",
          anchorTo: "panel-fees",
          active: this.activePanels.includes("fees"),
        },
      ];
    },

    panels() {
      return [
        {
          id: "ar-details",
          title: "Appointed Representative Details",
          content: "",
          active: this.activePanels.includes("ar-details"),
        },
        {
          id: "ar-rep-details",
          title: `${this.valueInternal.name ?? ""} Representative Details`,
          content: "",
          active: this.activePanels.includes("ar-rep-details"),
        },
        {
          id: "activities",
          title: "Activities",
          content: "",
          active: this.activePanels.includes("activities"),
          contentClass: "pt-0",
        },
        {
          id: "fees",
          title: "Fees",
          content: "",
          active: this.activePanels.includes("fees"),
        },
      ];
    },

    reasonsOptions() {
      return StaticList.getAppointReasons().map((item) => ({
        label: item,
        value: item,
      }));
    },

    servicesARPayForOptions() {
      return StaticList.getServicesARPayFor().map((item) => ({
        label: item,
        value: item,
      }));
    },

    isEmailValid() {
      if (!this.valueInternal.email) {
        return false;
      }

      return this.helperService.checkIfEmailFormatIsValid(
        this.valueInternal.email,
      );
    },

    isValidPrimaryDetails() {
      if (this.valueInternal.isCompany) {
        return (
          !!this.valueInternal.name &&
          (!!this.valueInternal.companyNumber ||
            !!this.valueInternal.firmReferenceNumber)
        );
      }

      return (
        !!this.valueInternal.forename &&
        !!this.valueInternal.firmReferenceNumber
      );
    },

    areAllRequiredFieldsProvided() {
      const {
        isCompany,
        companyNumber,
        firmReferenceNumber,
        name,
        forename,
        surname,
        tradingNames,
        registeredAddress,
        tradingAddress,
        homeAddress,
        email,
        contactNumber,
        proposedCommencementDate,
        residenceStartDate,
        dateOfBirth,
        countryOfBirth,
        nationalities,
        nationalityInsuranceNumber,
        passportNumber,
        representative,
        isBeIntroducer,
        primaryReason,
        isPayForServices,
        serviceToPayFor,
        fees,
        isActivitiesCoveredByPii,
        piiActivitiesCovered,
        activities,
      } = this.valueInternal;

      const hasCompanyOrFirmReferenceNumber: boolean =
        !!companyNumber || !!firmReferenceNumber;
      const hasInsuranceOrPassportNumber: boolean =
          !!nationalityInsuranceNumber || !!passportNumber;
      const hasTradingNames: boolean =
        Array.isArray(tradingNames) && tradingNames.length > 0;
      const isCommencementDateProvided: boolean =
        proposedCommencementDate != null && proposedCommencementDate !== 0;
      const isResidenceDateProvided: boolean =
          residenceStartDate != null && residenceStartDate !== 0;
      const isDateOfBirthProvided: boolean =
          dateOfBirth != null && dateOfBirth !== 0;
      const isRepresentativeProvided: boolean =
        representative &&
        representative.forename !== "" &&
        representative.surname !== "" &&
        representative.emailAddress !== "" &&
        representative.contactNumber?.number !== undefined &&
        representative.jobTitle !== "";

      const hasActivities: boolean =
        Array.isArray(activities) && activities.length > 0;
      const hasNationality: boolean =
          Array.isArray(nationalities) && nationalities.length > 0;

      let conditions = [
        hasCompanyOrFirmReferenceNumber,
        name !== "",
        hasTradingNames,
        registeredAddress !== "",
        tradingAddress !== "",
        this.helperService.checkIfEmailFormatIsValid(email ?? ""),
        contactNumber?.number !== undefined,
        isCommencementDateProvided,
        isRepresentativeProvided,
        isBeIntroducer != null,
        primaryReason !== "",
        isPayForServices != null,
        hasActivities,
      ];

      if (!isCompany){
        conditions = [
          hasCompanyOrFirmReferenceNumber,
          forename !== "",
          surname !== "",
          homeAddress !== "",
          tradingAddress !== "",
          this.helperService.checkIfEmailFormatIsValid(email ?? ""),
          contactNumber?.number !== undefined,
          isCommencementDateProvided,
          isResidenceDateProvided,
          isDateOfBirthProvided,
          countryOfBirth !== "",
          hasNationality,
          hasInsuranceOrPassportNumber,
          hasTradingNames,
          isBeIntroducer != null,
          primaryReason !== "",
          isPayForServices != null,
          hasActivities,
        ];
      }

      if (isPayForServices) {
        conditions.push(isActivitiesCoveredByPii != null);
        conditions.push(serviceToPayFor !== "");
        conditions.push(!!fees && Array.isArray(fees) && fees.length > 0);
      }

      if (isActivitiesCoveredByPii) {
        conditions.push(piiActivitiesCovered !== "");
      }

      return conditions.every((condition) => condition);
    },
  },
  watch: {
    value(newValue: AppointedRepresentative) {
      this.valueInternal = newValue;
    },
    "valueInternal.isPayForServices": function (value: boolean | null) {
      if (!value) {
        this.valueInternal.serviceToPayFor = "";
        this.valueInternal.fees = [];
        this.valueInternal.isActivitiesCoveredByPii = null;
      }
    },
    "valueInternal.isActivitiesCoveredByPii": function (value: boolean | null) {
      if (!value) {
        this.valueInternal.piiActivitiesCovered = "";
      }
    },
    "valueInternal.proposedCommencementDate": function (value: number | undefined) {
      if (value && this.valueInternal.firmDetail) {
        this.valueInternal.firmDetail.proposedCommencementDate = value;
      }     
    }  
  },
  setup() {
    const appointedRepresentativeStore = useAppointedRepresentativesStore();
    const { saveOrUpdateAppointedRepresentativeAsync } =
      appointedRepresentativeStore;

    const { currentFirmName } = useCustomerStore();

    const modalElement = ref();
    const arPanelsElement = ref();
    const editDetails = inject<any>("editItem");

    const openARPanel = (items: NavPillItem[]): void => {
      const item: NavPillItem[] = items.filter((item) => item.active);
      if (item.length > 0) {
        const activePanel: string | undefined = item[0].anchorTo;

        if (activePanel !== undefined && activePanel !== "") {
          arPanelsElement.value.expand(
            activePanel.replace(/^panel-/, "").trim(),
          );
        }
      }
    };

    return {
      currentFirmName,
      saveOrUpdateAppointedRepresentativeAsync,
      editDetails,
      modalElement,
      arPanelsElement,
      openARPanel,
    };
  },
  mounted() {
    this.isInitializing = false;
  },
  methods: {
    moment(value: number | undefined) {
      return moment(value);
    },

    onPanelExpanded(item: CollapsiblePanelItem) {
      this.activePanels.push(item.id);
      this.activePanels = [...new Set(this.activePanels)];
    },

    onPanelCollapsed(item: CollapsiblePanelItem) {
      this.activePanels = this.activePanels.filter(
        (value) => value !== item.id,
      );
    },

    async save(message: string) {
      try {
        this.valueInternal.firstName = this.valueInternal.forename?.trim() ?? this.valueInternal.representative?.forename?.trim() ?? "Company";
        this.valueInternal.lastName = this.valueInternal.surname?.trim() ?? this.valueInternal.representative?.surname?.trim() ?? "User";

        if (!this.valueInternal.isCompany) {
          this.valueInternal.name =
            `${this.valueInternal.forename} ${this.valueInternal.surname}`.trim();
        }

        if (this.valueInternal.firmDetail) {
          this.valueInternal.firmDetail.firmName = this.valueInternal.name; 
          this.valueInternal.firmDetail.proposedCommencementDate = this.valueInternal.proposedCommencementDate;
          this.valueInternal.firmDetail.emailAddress = this.valueInternal.email;
          this.valueInternal.firmDetail.website = this.valueInternal.website;
        }

        await this.saveOrUpdateAppointedRepresentativeAsync(this.valueInternal);

        useNotification({
          type: NotificationType.SUCCESS,
          content: `
              <div class="text-center">
                New Appointed Representative Added.
                <br /><br />
                ${message}
              </div>
            `,
        });
      } catch {
        useNotification({
          type: NotificationType.ERROR,
          content: "Something went wrong. Please try again later.",
        });
      }
    },

    handleRequestToCompleteDetails() {
      if (!this.valueInternal.email) {
        return;
      }

      const emailAddress = this.valueInternal.email;

      useAlert({
        title: "Confirm",
        content: `Please confirm that you are happy for us to email ${this.currentFirmName} to complete their profile.`,
        confirmButtonText: "Confirm & Send Email",
        cancelButtonText: "Cancel",
        type: AlertType.SAVEDETAILS,
        saveDetailsText: "Save Details",
        isShowSaveDetailsButton: true,
        width: 440,
        onConfirm: async () => {
          this.isLoading = true;
          // ToDo. we need to fill a value to foreName and surName for auth0 otherwise we can't create the user
          // even if the user is a company
          const appointedRepresentative = this.valueInternal;
          appointedRepresentative.profileStatus =
            ProfileStatuses.Full.toString();

          appointedRepresentative.firstName = this.valueInternal.isCompany
            ? "Company"
            : this.valueInternal.forename?.trim();
          appointedRepresentative.lastName = this.valueInternal.isCompany
            ? "User"
            : this.valueInternal.surname?.trim();

          if (!this.valueInternal.isCompany) {
            appointedRepresentative.name =
              `${this.valueInternal.forename} ${this.valueInternal.surname}`.trim();
          }

          await this.saveOrUpdateAppointedRepresentativeAsync(
            appointedRepresentative,
          );
          this.isLoading = false;

          useNotification({
            type: NotificationType.SUCCESS,
            content: `<center>New Appointed Representative Added.</center> <br /> <center>We have emailed <b>${emailAddress}</b> to complete their profile.</center>`,
            interval: AppConstants.notificationPopupTimeOut,
          });
        },
        onSaveDetails: () => {
          this.save(
            "Please complete the Appointed Representative Profile here."
          );
        },
      });
    },

    // why there is no implementation for save details here?
    handleSubmit() {
      useAlert({
        type: AlertType.CONFIRM,
        width: 410,
        content: `Please confirm that you are happy for us to email ${
          this.valueInternal.name ?? "[Appointed Representative Name]"
        } to complete their profile.`,
        confirmButtonText: "Confirm & Send Email",
        cancelButtonText: "Save Details",
        cancelButtonThemeColor: "primary",
        cancelButtonFillMode: "outline",
        onConfirm: () => {
          this.save(
            `We have emailed <strong>${this.valueInternal.email}</strong> to complete their profile.`
          );
        },
        onCancel: () => {
          this.save(
            "Please complete the Appointed Representative Profile here."
          );
        },
      });
    },

    async populateRelatedFields(firmReferenceNo: string) {
      try {
        const addressDetails =
          await this.fcaService.getFirmAddressesDetailsAsync(
            firmReferenceNo,
            "PPOB"
          );

        if (!addressDetails.length) {
          return;
        }

        // Contact Number
        if (addressDetails[0]["Phone Number"] && addressDetails[0]["country"]) {
          //this is format comes from fca +4402077818019"
          this.valueInternal.contactNumber =
            await this.helperService.convertToContactNoAsync(
              addressDetails[0]["Phone Number"],
              addressDetails[0]["country"],
            );
        }

        // Website Address
        this.valueInternal.website = addressDetails[0]["Website Address"];

        // Trading Names
        const tradingNames =
          await this.fcaService.getFirmTradingNamesAsync(firmReferenceNo);
        this.valueInternal.tradingNames = tradingNames;
      } catch {
        throw new Error("Something went wrong. Please try again later.");
      }
    },

    setUniqueIdentifier(value: string): string {
      const identifier = `${AppConstants.appointedRepresentativesRoute}${value}`;
      return identifier.replace(/\s+/g, "").replace("/", "");
    },
  },
});
</script>

<template>
  <ModalComponent
    ref="modalElement"
    :title="
      viewMode
        ? ''
        : `${editMode ? 'Edit' : 'Add New'} Appointed Representative`
    "
    width="1200"
    auto-scroll-content
    :class="['DetailsModal', viewMode && 'DetailsModal--viewMode']"
    @close="() => (valueInternal = new AppointedRepresentative())"
  >
    <div v-if="viewMode" class="text-center mb-3">
      <h4 class="font-weight-bold font-size-xl">
        {{ valueInternal.name }}
      </h4>
      <StatusLabelComponent no-icon :status="2" text="Onboarding" />
    </div>

    <NavPillComponent
      :items="navPills"
      :hidden-items="[!valueInternal.isCompany && 'ar-rep-details']"
      class="is-sticky-top is-bg-white"
      style="margin: 0 -10px 10px; z-index: 5"
      anchorable
      @update:items="openARPanel"
    >
    </NavPillComponent>

    <Form @submit="handleSubmit">
      <FormElement ref="formElement">
        <CollapsiblePanelComponent
          ref="arPanelsElement"
          class="ARPanels"
          :items="panels"
          :hidden-items="[!valueInternal.isCompany && 'ar-rep-details']"
          accordion
          @expanded="onPanelExpanded"
          @collapsed="onPanelCollapsed"
        >
          <template #after-title>
            <a
              href="javascript:;"
              v-if="viewMode"
              @click.stop="
                () => {
                  editDetails?.(valueInternal.id);
                  modalElement.close();
                }
              "
            >
              <IconComponent symbol="edit-pen" />
            </a>
          </template>

          <template #[`content-ar-details`]>
            <template v-if="viewMode">
              <div v-if="valueInternal.isCompany === true" class="row">
                <div class="col-lg-4">
                  <dl class="DescList">
                    <dt>Appointed Representative Name</dt>
                    <dd>{{ valueInternal.name || "-" }}</dd>
                  </dl>
                </div>
                <div class="col-lg-4">
                  <dl class="DescList">
                    <dt>Company Number</dt>
                    <dd>{{ valueInternal.companyNumber || "-" }}</dd>
                  </dl>
                </div>
                <div class="col-lg-4">
                  <dl class="DescList">
                    <dt>Firm Reference Number</dt>
                    <dd>{{ valueInternal.firmReferenceNumber || "-" }}</dd>
                  </dl>
                </div>
                <div class="col-lg-8">
                  <dl class="DescList">
                    <dt>Trading Names</dt>
                    <dd>
                      <SnipItemsComponent
                        :items="valueInternal.tradingNames"
                        popup-title="Trading Names"
                      >
                        <template #visible-items="{ items }">
                          <span class="is-truncated d-inline-block w-100">
                            {{ (items || []).join(", ") || "-" }}
                          </span>
                        </template>
                      </SnipItemsComponent>
                    </dd>
                  </dl>
                </div>
                <div class="col-lg-4">
                  <dl class="DescList">
                    <dt>
                      {{
                        valueInternal.firmReferenceNumber
                          ? "Commencement Date"
                          : "Proposed Commencement Date"
                      }}
                    </dt>
                    <dd>
                      {{
                        valueInternal?.proposedCommencementDate != 0
                          ? moment(
                              valueInternal?.proposedCommencementDate
                            ).format("LL")
                          : "-"
                      }}
                    </dd>
                  </dl>
                </div>
                <div class="col-lg-12">
                  <dl class="DescList">
                    <dt>Registered Address</dt>
                    <dd>{{ valueInternal.registeredAddress || "-" }}</dd>
                  </dl>
                </div>
                <div class="col-lg-12">
                  <dl class="DescList">
                    <dt>Trading Address</dt>
                    <dd>{{ valueInternal.tradingAddress || "-" }}</dd>
                  </dl>
                </div>
                <div class="col-lg-4">
                  <dl class="DescList">
                    <dt>Email Address</dt>
                    <dd>{{ valueInternal.email || "-" }}</dd>
                  </dl>
                </div>
                <div class="col-lg-4">
                  <dl class="DescList">
                    <dt>Contact Number</dt>
                    <dd>{{ valueInternal.contactNumber?.number || "-" }}</dd>
                  </dl>
                </div>
                <div class="col-lg-4">
                  <dl class="DescList">
                    <dt>Website</dt>
                    <dd>{{ valueInternal.website || "-" }}</dd>
                  </dl>
                </div>
              </div>
              <div v-else>
                <div class="row">
                  <div class="col-lg-3">
                    <dl class="DescList">
                      <dt>Title</dt>
                      <dd>{{ valueInternal.title || "-" }}</dd>
                    </dl>
                  </div>
                  <div class="col-lg-3">
                    <dl class="DescList">
                      <dt>Forename(s)</dt>
                      <dd>{{ valueInternal.forename || "-" }}</dd>
                    </dl>
                  </div>
                  <div class="col-lg-3">
                    <dl class="DescList">
                      <dt>Surname</dt>
                      <dd>{{ valueInternal.surname || "-" }}</dd>
                    </dl>
                  </div>
                  <div class="col-lg-3">
                    <dl class="DescList">
                      <dt>Firm Reference number</dt>
                      <dd>{{ valueInternal.firmReferenceNumber || "-" }}</dd>
                    </dl>
                  </div>
                  <div class="col-lg-6">
                    <dl class="DescList">
                      <dt>Home Address</dt>
                      <dd>{{ valueInternal.homeAddress || "-" }}</dd>
                    </dl>
                  </div>
                  <div class="col-lg-6">
                    <dl class="DescList">
                      <dt>Residence Start Date</dt>
                      <dd>
                        {{
                          moment(valueInternal.residenceStartDate).format(
                            "LL"
                          ) || "-"
                        }}
                      </dd>
                    </dl>
                  </div>
                  <div class="col-lg-4">
                    <dl class="DescList">
                      <dt>Date of Birth</dt>
                      <dd>
                        {{
                          moment(valueInternal.dateOfBirth).format("LL") || "-"
                        }}
                      </dd>
                    </dl>
                  </div>
                  <div class="col-lg-4">
                    <dl class="DescList">
                      <dt>Country of Birth</dt>
                      <dd>{{ valueInternal.countryOfBirth || "-" }}</dd>
                    </dl>
                  </div>
                  <div class="col-lg-4">
                    <dl class="DescList">
                      <dt>Nationality</dt>
                      <dd v-if="valueInternal.nationalities && valueInternal.nationalities.length>0">
                        <span v-for="(nat,key) in valueInternal.nationalities" :key="key">
                          {{`${nat}, `}}
                        </span>
                      </dd>
                      <dd v-else>
                        -
                      </dd>
                    </dl>
                  </div>
                  <div class="col-lg-4">
                    <dl class="DescList">
                      <dt>National Insurance Number</dt>
                      <dd>
                        {{ valueInternal.nationalityInsuranceNumber || "-" }}
                      </dd>
                    </dl>
                  </div>
                  <div class="col-lg-4">
                    <dl class="DescList">
                      <dt>Passport Number</dt>
                      <dd>{{ valueInternal.passportNumber || "-" }}</dd>
                    </dl>
                  </div>
                  <div class="col-lg-6">
                    <dl class="DescList">
                      <dt>Trading Names</dt>
                      <dd>
                        <SnipItemsComponent
                          :items="valueInternal.tradingNames"
                          popup-title="Trading Names"
                          style="display: inline-flex"
                        >
                          <template #visible-items="{ items }">
                            <span class="is-truncated d-inline-block w-100">
                              {{ (items || []).join(", ") }}
                            </span>
                          </template>
                        </SnipItemsComponent>
                      </dd>
                    </dl>
                  </div>
                  <div class="col-lg-4">
                    <dl class="DescList">
                      <dt>
                        {{
                          valueInternal.firmReferenceNumber
                            ? "Commencement Date"
                            : "Proposed Commencement Date"
                        }}
                      </dt>
                      <dd>
                        {{
                          moment(
                            valueInternal?.proposedCommencementDate
                          ).format("LL")
                        }}
                      </dd>
                    </dl>
                  </div>
                  <div class="col-lg-12">
                    <dl class="DescList">
                      <dt>Trading Address</dt>
                      <dd>{{ valueInternal.tradingAddress || "-" }}</dd>
                    </dl>
                  </div>
                  <div class="col-lg-3">
                    <dl class="DescList">
                      <dt>Email Address</dt>
                      <dd>{{ valueInternal.email || "-" }}</dd>
                    </dl>
                  </div>
                  <div class="col-lg-3">
                    <dl class="DescList">
                      <dt>Contact Number</dt>
                      <dd>
                        {{ valueInternal.contactNumber?.number || "-" }}
                      </dd>
                    </dl>
                  </div>
                  <div class="col-lg-3">
                    <dl class="DescList">
                      <dt>Website</dt>
                      <dd>{{ valueInternal.website || "-" }}</dd>
                    </dl>
                  </div>
                </div>
              </div>
            </template>

            <template v-else>
              <ToggleButtonComponent
                v-model="valueInternal.isCompany"
                text="Is the Appointed Representative a Company or a Sole Trader?"
                reverse
                wide
                on-label="Company"
                off-label="Sole Trader"
                class="mb-3"
                :hasDefaultModelValue="true"
                :disabled="valueInternal.isCompany !== null && editMode"
              />
              <template v-if="valueInternal.isCompany === true">
                <div class="row gy-3">
                  <KendoFirmFinderComponentAsync
                    :id="
                      setUniqueIdentifier('-arDetails.appointed-representative')
                    "
                    class="col-12"
                    companyNameLabel="Appointed Representative Name"
                    :company="{
                      firmName: valueInternal.name,
                      companyNumber: valueInternal.companyNumber,
                      firmReferenceNumber: valueInternal.firmReferenceNumber,
                    }"
                    :companyName="valueInternal.name"
                    :companyNumber="valueInternal.companyNumber"
                    :firmReferenceNumber="valueInternal.firmReferenceNumber"
                    :isInitializing="false"
                    @onCompanyDetailUpdated="
                      (firmBasicInfo: FirmBasicInfo) => {
                        valueInternal.name = firmBasicInfo.firmName;
                        valueInternal.companyNumber =
                          firmBasicInfo.companyNumber;
                        valueInternal.firmReferenceNumber =
                          firmBasicInfo.firmReferenceNumber;
                        valueInternal.tradingAddress =
                          firmBasicInfo.tradingAddress;
                        valueInternal.registeredAddress =
                          firmBasicInfo.address;
                        populateRelatedFields(
                          valueInternal?.firmReferenceNumber?.toString() ?? ''
                        );
                      }
                    "
                  />

                  <div class="col-lg-9">
                    <KendoTradingNameComponent
                      name="tradingNames"
                      :isValueReactive="true"
                      :value="valueInternal.tradingNames"
                      :isUserModified="valueInternal.isTradingNamesChanged"
                      :isDataLoadedCompletely="!isInitializing"
                      @onValueChange="valueInternal.tradingNames = $event"
                      @onRemoveItem="
                        (_item: string, index: number) => {
                          valueInternal.tradingNames?.splice(index, 1);
                        }
                      "
                      :isShowAllOnPopup="true"
                    />
                  </div>

                  <KendoDatePickerInputComponent
                    :id="setUniqueIdentifier('proposedCommencementDate')"
                    class="col-lg-3"
                    :label="
                      valueInternal.firmReferenceNumber
                        ? 'Commencement Date'
                        : 'Proposed Commencement Date'
                    "
                    name="proposedCommencementDate"
                    :epoch="true"
                    :value="valueInternal.proposedCommencementDate"
                    :isValueReactive="true"
                    :isRequired="true"
                    @onValueChange = "(date: number) =>
                        valueInternal.proposedCommencementDate = date
                    "
                  />

                  <KendoPostCodeInputComponent
                    :id="setUniqueIdentifier('-registeredAddress')"
                    class="col-12"
                    name="registeredAddress"
                    label="Registered Address"
                    :value="valueInternal.registeredAddress"
                    :isValueReactive="true"
                    :isDataLoadedCompletely="!isInitializing"
                    @onValueChange="valueInternal.registeredAddress = $event"
                  />

                  <KendoPostCodeInputWithSameAsComponent
                    :id="setUniqueIdentifier('-tradingAddress')"
                    class="col-12"
                    name="tradingAddress"
                    label="Trading Address"
                    :isRequired="true"
                    :value="valueInternal.tradingAddress"
                    :isShowCheckBox="true"
                    :isChecked="valueInternal.isTradingSameAsRegisteredAddress"
                    @onValueChange="valueInternal.tradingAddress = $event"
                    @onToggle="
                      () => {
                        valueInternal.isTradingSameAsRegisteredAddress =
                          !valueInternal.isTradingSameAsRegisteredAddress;

                        if (valueInternal.isTradingSameAsRegisteredAddress) {
                          valueInternal.tradingAddress =
                            valueInternal.registeredAddress;
                        } else {
                          valueInternal.tradingAddress = '';
                        }
                      }
                    "
                    :sameAsLabel="$t('same-as-registered-address')"
                    :isValueReactive="true"
                    :isDataLoadedCompletely="!isInitializing"
                  />

                  <KendoEmailAddressInputComponent
                    :id="setUniqueIdentifier('-email')"
                    class="col-lg-4"
                    label="Email Address"
                    name="email"
                    v-model="valueInternal.email"
                    :isValueReactive="true"
                  />

                  <KendoTelInputComponent
                    :id="setUniqueIdentifier('-contactNumber')"
                    name="contactNumber"
                    label="Contact Number"
                    class="col-lg-4"
                    :value="valueInternal.contactNumber"
                    @onValueChange="
                      (contactNumber: ContactNumber) =>
                        (valueInternal.contactNumber = contactNumber)
                    "
                    :isValueReactive="true"
                    :isDataLoadedCompletely="!isInitializing"
                  />

                  <KendoGenericInputComponent
                    :id="setUniqueIdentifier('-website')"
                    class="col-lg-4"
                    label="Website"
                    name="website"
                    v-model="valueInternal.website"
                    :isRequired="false"
                  />
                </div>
              </template>

              <!-- Sole Trader -->
              <template v-if="valueInternal.isCompany === false">
                <div class="row gy-3">
                  <div class="col-lg-3">
                    <KendoNameTitleComponent
                      name="title"
                      label="Title"
                      :isRequired="false"
                      :isValueReactive="true"
                      :isDataLoadedCompletely="!isInitializing"
                      :value="valueInternal.title"
                      @onValueChange="valueInternal.title = $event"
                    />
                  </div>

                  <KendoGenericInputComponent
                    :id="setUniqueIdentifier('-forename')"
                    class="col-lg-3"
                    name="forename"
                    label="Forename(s)"
                    v-model="valueInternal.forename"
                    :isCapitalizeFirstLetter="true"
                    :isCheckForProfanity="true"
                    :isValueReactive="true"
                    :isDataLoadedCompletely="!isInitializing"
                  />

                  <KendoGenericInputComponent
                    :id="setUniqueIdentifier('-surname')"
                    class="col-lg-3"
                    name="surname"
                    label="Surname"
                    v-model="valueInternal.surname"
                    :isCapitalizeFirstLetter="true"
                    :isCheckForProfanity="true"
                    :isValueReactive="true"
                    :isDataLoadedCompletely="!isInitializing"
                  />

                  <KendoGenericInputComponent
                    :id="setUniqueIdentifier('-firmReferenceNumber')"
                    class="col-lg-3"
                    name="firmReferenceNumber"
                    label="Firm Reference Number"
                    v-model="valueInternal.firmReferenceNumber"
                    :isCapitalizeFirstLetter="true"
                    :isCheckForProfanity="true"
                    :isValueReactive="true"
                    :isDataLoadedCompletely="!isInitializing"
                  />

                  <KendoPostCodeInputComponent
                    :id="setUniqueIdentifier('-homeAddress')"
                    class="col-9"
                    name="homeAddress"
                    label="Home Address"
                    :value="valueInternal.homeAddress"
                    :isValueReactive="true"
                    :isDataLoadedCompletely="!isInitializing"
                    @onValueChange="valueInternal.homeAddress = $event"
                  />

                  <KendoDatePickerInputComponent
                    :id="setUniqueIdentifier('-residenceStartDate')"
                    class="col-lg-3"
                    label="Residence Start Date"
                    name="residenceStartDate"
                    epoch
                    @onValueChange="(val: number)=> valueInternal.residenceStartDate = val"
                    :value="valueInternal.residenceStartDate"
                    :isValueReactive="true"
                  />

                  <KendoDatePickerInputComponent
                    :id="setUniqueIdentifier('-dateOfBirth')"
                    class="col-lg-4"
                    label="Date of Birth"
                    name="dateOfBirth"
                    epoch
                    @onValueChange="(val: number)=> valueInternal.dateOfBirth = val"
                    :value="valueInternal.dateOfBirth"
                    :isValueReactive="true"
                  />

                  <div class="col-lg-4">
                    <KendoCountryComponent
                      :id="setUniqueIdentifier('-countryOfBirth')"
                      name="countryOfBirth"
                      label="Country of Birth"
                      :value="valueInternal.countryOfBirth"
                      @onValueChange="valueInternal.countryOfBirth = $event"
                    />
                  </div>

                  <KendoNationalityInputComponent
                    :id="setUniqueIdentifier('-nationality')"
                    class="col-lg-4"
                    name="nationality"
                    label="Nationality"
                    placeholder="Please select all that apply"
                    :value="valueInternal.nationalities"
                    @onValueChange="valueInternal.nationalities = $event"
                  />

                  <KendoGenericInputComponent
                    :id="setUniqueIdentifier('-nationalityInsuranceNumber')"
                    class="col-lg-6"
                    name="nationalityInsuranceNumber"
                    label="Nationality Insurance Number"
                    placeholder="LLLNNLLL"
                    v-model="valueInternal.nationalityInsuranceNumber"
                    :isCapitalizeFirstLetter="true"
                    :isCheckForProfanity="true"
                    :isValueReactive="true"
                    :isDataLoadedCompletely="!isInitializing"
                  />

                  <KendoGenericInputComponent
                    :id="setUniqueIdentifier('-passportNumber')"
                    class="col-lg-6"
                    name="passportNumber"
                    label="Passport Number (If National Insurance Number is not available)"
                    placeholder="NNNNNNNN"
                    v-model="valueInternal.passportNumber"
                    :isCapitalizeFirstLetter="true"
                    :isCheckForProfanity="true"
                    :isValueReactive="true"
                    :isDataLoadedCompletely="!isInitializing"
                  />

                  <div class="col-lg-9">
                    <KendoTradingNameComponent
                      :id="setUniqueIdentifier('-tradingNames')"
                      name="tradingNames"
                      :isValueReactive="true"
                      :isDataLoadedCompletely="!isInitializing"
                      :value="valueInternal.tradingNames"
                      :isUserModified="valueInternal.isTradingNamesChanged"
                      @onValueChange="valueInternal.tradingNames = $event"
                      @onRemoveItem="
                        (_item: string, index: number) => {
                          valueInternal.tradingNames?.splice(index, 1);
                        }
                      "
                      :isShowAllOnPopup="true"
                    />
                  </div>

                  <KendoDatePickerInputComponent
                      :id="setUniqueIdentifier('-soletrader-proposedCommencementDate')"
                      class="col-lg-3"
                      :label="
                      valueInternal.firmReferenceNumber
                        ? 'Commencement Date'
                        : 'Proposed Commencement Date'
                    "
                      name="proposedCommencementDate"
                      :epoch="true"
                      :value="valueInternal.proposedCommencementDate"
                      :isValueReactive="true"
                      :isRequired="true"
                      @onValueChange = "(date: number) =>
                        valueInternal.proposedCommencementDate = date
                    "
                  />

                  <KendoPostCodeInputComponent
                    :id="setUniqueIdentifier('-tradingAddress')"
                    class="col-12"
                    name="tradingAddress"
                    label="Trading Address"
                    :value="valueInternal.tradingAddress"
                    :isValueReactive="true"
                    :isDataLoadedCompletely="!isInitializing"
                    @onValueChange="valueInternal.tradingAddress = $event"
                  />

                  <KendoEmailAddressInputComponent
                    :id="setUniqueIdentifier('-soletrader-email')"
                    class="col-lg-4"
                    label="Email Address"
                    name="soletrader-email"
                    v-model="valueInternal.email"
                    :isValueReactive="true"
                  />

                  <KendoTelInputComponent
                    :id="setUniqueIdentifier('-contactNumber')"
                    name="contactNumber"
                    label="Contact Number"
                    class="col-lg-4"
                    :value="valueInternal.contactNumber"
                    @onValueChange="
                      (contactNumber: ContactNumber) =>
                        (valueInternal.contactNumber = contactNumber)
                    "
                    :isValueReactive="true"
                    :isDataLoadedCompletely="!isInitializing"
                  />

                  <KendoGenericInputComponent
                    :id="setUniqueIdentifier('-website')"
                    class="col-lg-4"
                    label="Website"
                    name="website"
                    v-model="valueInternal.website"
                  />
                </div>
              </template>
            </template>
          </template>

          <template #[`content-ar-rep-details`]>
            <div v-if="viewMode" class="row">
              <div class="col-lg-4">
                <dl class="DescList">
                  <dt>Title</dt>
                  <dd>{{ valueInternal.representative?.title || "-" }}</dd>
                </dl>
              </div>
              <div class="col-lg-4">
                <dl class="DescList">
                  <dt>Forename(s)</dt>
                  <dd>{{ valueInternal.representative?.forename || "-" }}</dd>
                </dl>
              </div>
              <div class="col-lg-4">
                <dl class="DescList">
                  <dt>Surname</dt>
                  <dd>{{ valueInternal.representative?.surname || "-" }}</dd>
                </dl>
              </div>
              <div class="col-lg-4">
                <dl class="DescList">
                  <dt>Email Address</dt>
                  <dd>
                    {{ valueInternal.representative?.emailAddress || "-" }}
                  </dd>
                </dl>
              </div>
              <div class="col-lg-4">
                <dl class="DescList">
                  <dt>Contact Number</dt>
                  <dd>
                    {{
                      valueInternal.representative?.contactNumber?.number || "-"
                    }}
                  </dd>
                </dl>
              </div>
              <div class="col-lg-4">
                <dl class="DescList">
                  <dt>Job Title</dt>
                  <dd>{{ valueInternal.representative?.jobTitle || "-" }}</dd>
                </dl>
              </div>
            </div>
            <div v-else class="row gy-3">
              <div class="col-lg-4">
                <KendoNameTitleComponent
                  label="Title"
                  name="representative.title"
                  :value="valueInternal.representative.title"
                  @onValueChange="valueInternal.representative.title = $event"
                  :is-required="false"
                />
              </div>

              <KendoGenericInputComponent
                :id="setUniqueIdentifier('-representative.forename')"
                class="col-md-4"
                label="Forename(s)"
                name="representative.forename"
                v-model="valueInternal.representative.forename"
              />

              <KendoGenericInputComponent
                :id="setUniqueIdentifier('-representative.surname')"
                class="col-md-4"
                label="Surname"
                name="representative.surname"
                v-model="valueInternal.representative.surname"
              />

              <KendoEmailAddressInputComponent
                :id="setUniqueIdentifier('-representative.emailAddress')"
                class="col-md-4"
                label="Email Address"
                name="representative.emailAddress"
                v-model="valueInternal.representative.emailAddress"
              />

              <KendoTelInputComponent
                :id="setUniqueIdentifier('-representative.contactNumber')"
                name="contactNumber"
                label="Contact Number"
                class="col-lg-4"
                :value="valueInternal.representative.contactNumber"
                @onValueChange="
                  (contactNumber: ContactNumber) =>
                    (valueInternal.representative.contactNumber = contactNumber)
                "
                :isValueReactive="true"
                :isDataLoadedCompletely="!isInitializing"
              />

              <KendoGenericInputComponent
                :id="setUniqueIdentifier('-representative.jobTitle')"
                class="col-md-4"
                label="Job Title"
                name="representative.jobTitle"
                v-model="valueInternal.representative.jobTitle"
              />
            </div>
          </template>

          <template #[`content-activities`]>
            <Activities
              :modelValue="valueInternal.activities"
              :view-mode="viewMode"
              @update:modelValue="(val:AppointedRepresentativeActivity[]) =>
                                  valueInternal.activities = val"
            />
          </template>

          <template #[`content-fees`]>
            <div v-if="viewMode">
              <dl class="DescList list-group list-group-numbered">
                <dt>
                  1.
                  {{
                    `Will ${
                      valueInternal.name ?? "[Appointed Representative Name]"
                    } be an Introducer Appointed Representative?`
                  }}
                </dt>
                <dd>
                  {{
                    valueInternal.isBeIntroducer === undefined ||
                    valueInternal.isBeIntroducer === null
                      ? "-"
                      : !!valueInternal.isBeIntroducer
                      ? "Yes"
                      : "No"
                  }}
                </dd>
              </dl>
              <dl class="DescList">
                <dt>
                  2.
                  {{
                    `What is the primary reason for ${currentFirmName} seeking to appoint ${
                      valueInternal.name ?? "[Appointed Representative Name]"
                    }?`
                  }}
                </dt>
                <dd>
                  {{ valueInternal.primaryReason || "-" }}
                </dd>
              </dl>
              <dl class="DescList">
                <dt>
                  3.
                  {{
                    `Will ${
                      valueInternal.name ?? "[Appointed Representative Name]"
                    } pay ${currentFirmName} for the appointment or for any other services received?`
                  }}
                </dt>
                <dd>
                  {{
                    valueInternal.isPayForServices === undefined ||
                    valueInternal.isPayForServices === null
                      ? "-"
                      : !!valueInternal.isPayForServices
                      ? "Yes"
                      : "No"
                  }}
                </dd>
              </dl>
              <dl class="DescList">
                <dt>
                  4.
                  {{
                    `Which services will ${
                      valueInternal.name ?? "[Appointed Representative Name]"
                    } pay ${currentFirmName} for? `
                  }}
                </dt>
                <dd>
                  <span class="mb-10px w-100 d-block">{{
                    valueInternal.serviceToPayFor
                  }}</span>
                  <span
                    class="font-size-xs font-weight-bold mb-2px w-100 d-block"
                    >{{
                      `Fees Charged by ${currentFirmName} to ${valueInternal.name}:`
                    }}</span
                  >
                  {{ (valueInternal.fees || []).map(({type, amount}) =>
                  `${type}: ${amount?.symbol ?? ""} ${amount?.amount ?? ""}
                  ${amount?.currency ?? ""}`).join('<br />') }}
                </dd>
              </dl>
              <dl class="DescList">
                <dt>
                  5.
                  {{
                    `Will the activities undertaken by ${
                      valueInternal.name ?? "[Appointed Representative Name]"
                    } be covered by the PII of The Right Mortgage Limited?`
                  }}
                </dt>
                <dd>
                  {{
                    valueInternal.isActivitiesCoveredByPii === undefined ||
                    valueInternal.isActivitiesCoveredByPii === null
                      ? "-"
                      : !!valueInternal.isActivitiesCoveredByPii
                      ? "Yes"
                      : "No"
                  }}
                </dd>
              </dl>
              <dl class="DescList">
                <dt>
                  6.
                  {{
                    `Will the activities undertaken by ${
                      valueInternal.name ?? "[Appointed Representative Name]"
                    } be covered by the PII of The Right Mortgage Limited?`
                  }}
                </dt>
                <dd>
                  {{ valueInternal.piiActivitiesCovered || "-" }}
                </dd>
              </dl>
            </div>

            <NumberListComponent
              v-else
              :items="[
                `Will ${
                  valueInternal.name ?? '[Appointed Representative Name]'
                } be an Introducer Appointed Representative?`,
                `What is the primary reason for ${currentFirmName} seeking to appoint ${
                  valueInternal.name ?? '[Appointed Representative Name]'
                }?`,
                `Will ${
                  valueInternal.name ?? '[Appointed Representative Name]'
                } pay ${currentFirmName} for the appointment or for any other services received?`,
                `Which services will ${
                  valueInternal.name ?? '[Appointed Representative Name]'
                } pay ${currentFirmName} for? `,
                `Will the activities undertaken by ${
                  valueInternal.name ?? '[Appointed Representative Name]'
                } be covered by the PII of ${currentFirmName}?`,
                `Which PII of ${currentFirmName} will the activities undertaken by ${
                  valueInternal.name ?? '[Appointed Representative Name]'
                } be covered?`,
              ]"
              :hidden-items="[
                !valueInternal.isPayForServices && 4,
                !valueInternal.isPayForServices && 5,
                !valueInternal.isActivitiesCoveredByPii && 6,
              ]"
            >
              <template #item-1="{ item }">
                {{ valueInternal.isActivitiesCoveredByPii }}
                <ToggleButtonComponent
                  :text="item"
                  reverse
                  wide
                  v-model="valueInternal.isBeIntroducer"
                  :isRequired="true"
                />
              </template>
              <template #item-2="{ item }">
                <div class="row align-items-center">
                  <div class="col-lg-8">{{ item }}</div>
                  <div class="col-lg-4 text-right">
                    <KendoDropdownListComponent
                      :id="setUniqueIdentifier('-arDetails.primaryReason')"
                      name="primaryReason"
                      :data-items="reasonsOptions"
                      :is-required="false"
                      value-primitive
                      placeholder="Please Select"
                      v-model="valueInternal.primaryReason"
                    />
                  </div>
                </div>
              </template>
              <template #item-3="{ item }">
                <ToggleButtonComponent
                  :text="item"
                  reverse
                  wide
                  v-model="valueInternal.isPayForServices"
                />
              </template>
              <template
                #item-4="{ item }"
                v-if="valueInternal.isPayForServices"
              >
                <div
                  class="row align-items-center"
                  v-if="valueInternal.isPayForServices"
                >
                  <div class="col-lg-8">{{ item }}</div>
                  <div class="col-lg-4 text-right">
                    <KendoDropdownListComponent
                      :id="setUniqueIdentifier('-arDetails.isPayForService')"
                      name="servicesPayFor"
                      :data-items="servicesARPayForOptions"
                      v-model="valueInternal.serviceToPayFor"
                      :is-required="valueInternal.isPayForServices"
                      value-primitive
                      placeholder="Please Select"
                      :addable="true"
                    />
                  </div>
                </div>
              </template>
              <template #item-extend-4 v-if="valueInternal.isPayForServices">
                <PanelComponent
                  :title="`Fees Charged by ${currentFirmName} to ${valueInternal.name}`"
                  v-if="valueInternal.isPayForServices"
                >
                  <FeesChargeTable v-model="valueInternal.fees" />
                </PanelComponent>
              </template>
              <template #item-5="{ item }">
                <ToggleButtonComponent
                  :text="item"
                  reverse
                  wide
                  v-model="valueInternal.isActivitiesCoveredByPii"
                />
              </template>
              <template #item-6="{ item }">
                <div
                  class="row align-items-center"
                  v-if="valueInternal.isActivitiesCoveredByPii"
                >
                  <div class="col-lg-8">{{ item }}</div>
                  <div class="col-lg-4 text-right">
                    <KendoDropdownListComponent
                      :id="
                        setUniqueIdentifier('-arDetails.piiActivitiesCovered')
                      "
                      name="piiCovered"
                      :data-items="reasonsOptions"
                      :is-required="false"
                      v-model="valueInternal.piiActivitiesCovered"
                      value-primitive
                      placeholder="Please Select"
                    />
                  </div>
                </div>
              </template>
            </NumberListComponent>
          </template>
        </CollapsiblePanelComponent>
      </FormElement>
    </Form>

    <template v-if="!viewMode" #footer>
      <div class="d-flex justify-content-end" style="gap: 10px">
        <KendoButton
          style="font-weight: 600"
          type="button"
          fill-mode="outline"
          theme-color="primary"
          @click.prevent="handleRequestToCompleteDetails"
          :disabled="!isEmailValid || !isValidPrimaryDetails"
        >
          Save & Add Appointed Representative
        </KendoButton>

        <KendoButton
          type="button"
          theme-color="primary"

          :disabled="!areAllRequiredFieldsProvided"
          @click="handleSubmit"
        >

          Request Appointed Representative to Complete Details
        </KendoButton>
      </div>
    </template>

    <KendoLoadingComponent :isLoading="isLoading" />
  </ModalComponent>
</template>

<style scoped lang="scss">
.ARPanels {
  & > :deep(.is-active) {
    border-width: 2px;
    border-color: var(--color-primary);
  }
}

:global(.DetailsModal--viewMode .k-dialog-titlebar) {
  padding-bottom: 0;
  border-width: 0;
}

.DescList {
  dt {
    font-size: var(--font-size-xs);
    margin-bottom: 10px;
  }

  dd {
    font-size: var(--font-size-sm);
    overflow-wrap: break-word;
  }
}
</style>
