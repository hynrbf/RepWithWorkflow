<script lang="ts">
import { defineComponent, inject } from "vue";
import { CompanyEntity } from "@/entities/CompanyEntity";
import { FirmBasicInfo } from "@/entities/FirmBasicInfo";
import { AppConstants } from "@/infra/AppConstants";
import { Emitter, EventType } from "mitt";
import isEmpty from "lodash/isEmpty";
import { CompanySearchMode } from "@/entities/enums/CompanySearchMode";
import { container } from "tsyringe";
import {
  ICompaniesHouseService,
  ICompaniesHouseServiceInfo,
} from "@/infra/dependency-services/rest/company-house/ICompaniesHouseService";
import {
  IFcaService,
  IFcaServiceInfo,
} from "@/infra/dependency-services/rest/fca/IFcaService";
import { FcaAuthStatus } from "@/entities/FcaAuthStatus";
import {
  IAppService,
  IAppServiceInfo,
} from "@/infra/dependency-services/app/IAppService";
import { CustomerPermission } from "@/entities/CustomerPermission";
import { CustomerEntity } from "@/entities/CustomerEntity";
import { PermissionStateEnum } from "@/entities/enums/PermissionStateEnum";
import {
  ICalendlyService,
  ICalendlyServiceInfo,
} from "@/infra/dependency-services/rest/calendly/ICalendlyService";
import { KendoPromptDialogComponent } from "../KendoPromptDialog.vue";
import { KendoDialogComponent } from "../KendoDialog.vue";
import { KendoFlexibleDialogComponent } from "../KendoFlexibleDialog.vue";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import { CompanyHouseStatus } from "@/entities/CompanyHouseStatus";
import { OnboardingType } from "@/infra/base";
import { FirmDetailsBase } from "@/entities/FirmDetailsBase";

export default defineComponent({
  name: "KendoFirmFinderComponent",
  props: {
    id: String,
    name: {
      type: String,
      default: "",
    },
    companyNameLabel: {
      type: String,
      default: "Company Name",
    },
    companyNamePlaceholder: {
      type: String,
      default: "Type",
    },
    companyNumberLabel: {
      type: String,
      default: "Company Number",
    },
    firmReferenceNumberLabel: {
      type: String,
      default: "Firm Reference Number",
    },
    companyName: {
      type: String,
      default: "",
    },
    companyNumber: {
      type: String,
      default: "",
    },
    firmReferenceNumber: {
      type: String,
      default: "",
    },
    isInitializing: {
      type: Boolean,
      default: true,
    },
    company: {
      type: Object as () => FirmBasicInfo,
      default: new FirmBasicInfo(),
    },
    isCompanyDetailsEditAllowed: {
      type: Boolean,
      default: false,
    },
    mode: {
      type: Number,
      default: CompanySearchMode.FromFca,
    },
    excludedFirm: {
      type: Object as () => FirmDetailsBase | undefined,
      default: undefined,
    },
    isEnableSearch: {
      type: Boolean,
      default: true,
    },
    useDefaultSchedMeetingModal: {
      type: Boolean,
      default: true,
    },
    onboardingType: {
      type: String,
      default: OnboardingType.Firm.toString(),
    },
    maxLengthBeforeTruncate: {
      type: Number,
      default: 23,
    },
  },
  data() {
    return {
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, { payload: string | undefined }>
      >,
      companySearchText: "",
      companyInternal: new FirmBasicInfo(),
      userFirmFcaPermissions: [] as CustomerPermission[],
      isInitializingInternal: true,
      isAddFirmMarginTop: true,
      companiesHouseService: container.resolve<ICompaniesHouseService>(
        ICompaniesHouseServiceInfo.name,
      ),
      appService: container.resolve<IAppService>(IAppServiceInfo.name),
      fcaService: container.resolve<IFcaService>(IFcaServiceInfo.name),
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      fcaDefinedStatuses: [] as FcaAuthStatus[],
      companyHouseDefinedStatuses: [] as CompanyHouseStatus[],
      calendlyService: container.resolve<ICalendlyService>(
        ICalendlyServiceInfo.name,
      ),
      kendoDialogInstance: null as KendoDialogComponent | null,
      kendoPromptInstance: null as KendoPromptDialogComponent | null,
      kendoCalendarFlexibleDialogInstance:
        null as KendoFlexibleDialogComponent | null,
      eventBusCompanyOrFrnNo: inject("$eventBusService") as Emitter<
        Record<EventType, { payload: string }>
      >,
      colorCodingCompanyHouse: "",
      colorCodingFCA: "",
      companyStatusText: "",
      fcaStatusText: "",
    };
  },
  computed: {
    CompanySearchMode() {
      return CompanySearchMode;
    },
    computedPlaceholder() {
      if (!this.$t("common-placeholder-text")) {
        return this.companyNamePlaceholder;
      }

      return this.$t("common-placeholder-text");
    },
  },
  created() {
    this.setBindingAsync();
    this.getFcaDefinedStatusesFromCacheThenSet();
    this.getCompanyHouseDefinedStatusesFromCacheThenSet();

    if (this.onboardingType !== OnboardingType.Firm.toString()) {
      return;
    }

    const registeredCustomer =
      this.appService.getCachedCustomer() ?? new CustomerEntity();

    if (!registeredCustomer?.currentFcaPermissions) {
      return;
    }

    this.userFirmFcaPermissions = registeredCustomer.currentFcaPermissions;
  },
  async mounted() {
    if (
      !this.companyInternal?.fcaStatus &&
      this.companyInternal?.firmReferenceNumber
    ) {
      const firmFca = await this.fcaService.getFcaFirmDetailByRefNoAsync(
        this.companyInternal.firmReferenceNumber,
      );

      this.companyInternal.fcaStatus = firmFca?.status ?? "";
    }

    if (
      !this.companyInternal?.companyHouseStatus &&
      this.companyInternal?.companyNumber
    ) {
      const firmCompanyHouse =
        await this.companiesHouseService.getCompanyProfileAsync(
          this.companyInternal.companyNumber,
        );

      this.companyInternal.companyHouseStatus =
        firmCompanyHouse?.company_status ?? "";
    }

    // FCA
    this.assignFCAStatusAndColor();

    // Company House
    this.assignCompanyHouseStatusAndColor();
  },
  async updated() {
    await this.setBindingAsync();
  },
  methods: {
    async setBindingAsync() {
      if (
        !isEmpty(this.company) &&
        !(
          this.companyInternal?.companyNumber ||
          this.companyInternal?.firmReferenceNumber
        )
      ) {
        this.companyInternal = this.company;
      }

      this.isInitializingInternal = this.isInitializing;
    },

    getFCAStatus(): FcaAuthStatus {
      const mappedFcaStatus = this.fcaDefinedStatuses.find(
        (s) => s.actualStatus?.toLowerCase() === this.companyInternal.fcaStatus?.toLowerCase(),
      );
      return mappedFcaStatus as FcaAuthStatus;
    },

    getCompanyHouseStatus(): CompanyHouseStatus {
      const mappedCompanyHouseStatus = this.companyHouseDefinedStatuses.find(
        (s) =>
          s.status?.toLocaleLowerCase ==
          this.companyInternal.companyHouseStatus?.toLocaleLowerCase,
      );
      return mappedCompanyHouseStatus as CompanyHouseStatus;
    },

    async onCompanyValueChanged(company: CompanyEntity) {
      let firm = new FirmBasicInfo();
      firm.firmName = company.companyName;
      firm.companyNumber = company.companyNumber;
      firm.firmReferenceNumber = company.firmReferenceNo;
      firm.address = company.address;
      firm.tradingAddress = company.tradingAddress;
      firm.contactNumber = company.contactNumber;
      firm.website = company.website;
      firm.fcaStatus = company.status;
      firm.companyHouseStatus = company.companyHouseStatus;
      firm.sicCode = company.sicCode;
      firm.countryCode = company.countryCode;
      this.companyInternal = firm;
      this.isAddFirmMarginTop = true;

      if (!this.companyInternal?.fcaStatus) {
        return;
      }

      // FCA
      //const mappedFcaStatus = this.getFCAStatus();
      const isAuthorised = this.assignFCAStatusAndColor();

      if (!isAuthorised) {
        this.$emit("onCompanyNotAuthorised", this.companyInternal);
      }

      // Company House
      this.assignCompanyHouseStatusAndColor();
      this.$emit("onCompanyDetailUpdated", this.companyInternal);

      if (isAuthorised) {
        await this.checkIfFirmHasSamePermissionWithUserFirmAsync(
          this.companyInternal.firmReferenceNumber,
        );
      }
    },

    assignFCAStatusAndColor() {
      let isAuthorised = false;
      const mappedFcaStatus = this.getFCAStatus();

      if (mappedFcaStatus) {
        isAuthorised = mappedFcaStatus && mappedFcaStatus.isAuthorised;
        this.fcaStatusText = mappedFcaStatus.actualStatus as string;
        this.colorCodingFCA = mappedFcaStatus?.colorCoding as string;
      }
      else {
        this.fcaStatusText = this.companyInternal.fcaStatus as string;
        this.colorCodingFCA = "Red";
      }   
      
      return isAuthorised;
    },

    assignCompanyHouseStatusAndColor() {
      const mappedCompanyHouseStatus = this.getCompanyHouseStatus();

      if (mappedCompanyHouseStatus) {
        this.companyStatusText = mappedCompanyHouseStatus.statusDisplayText as string;
        this.colorCodingCompanyHouse = mappedCompanyHouseStatus?.colorCoding as string;
      }
      else {
        this.companyStatusText = this.companyInternal.companyHouseStatus as string;
        this.colorCodingCompanyHouse = "Red";
      }
    },

    onClearCompany() {
      this.companyInternal = new FirmBasicInfo();
      this.$emit("onCompanyDetailUpdated", this.companyInternal);
    },

    onCompanyNameInput(value: string) {
      this.companyInternal.firmName = value;
    },

    onCompanyNumberInput(value: string) {
      if (!(value && this.isCompanyDetailsEditAllowed)) {
        return;
      }

      if (this.companyInternal?.companyNumber === value) {
        return;
      }

      this.companyInternal.companyNumber = value;
      this.$emit("onCompanyDetailUpdated", this.companyInternal);
    },

    onFirmReferenceNumberInput(value: string) {
      if (!(value && this.isCompanyDetailsEditAllowed)) {
        return;
      }

      this.companyInternal.firmReferenceNumber = value;
      this.$emit("onCompanyDetailUpdated", this.companyInternal);
    },

    onAttemptEdit() {
      if (this.useDefaultSchedMeetingModal) {
        if (!this.kendoPromptInstance) {
          this.kendoPromptInstance = this.$refs
            .kendoActionDialogCompanyNumber as KendoPromptDialogComponent;
          this.kendoPromptInstance.setActionDialogMessage("Confirm");
        } else {
          this.kendoPromptInstance.setActionDialogMessage("Confirm");
        }
        return;
      }

      this.eventBus.emit(AppConstants.requestCompanyDetailsUpdateEvent, {
        payload: "",
      });
    },

    onFirmSelectionFailed() {
      this.eventBus.emit(AppConstants.firmSelectionErrorEvent, {
        payload: "",
      });
    },

    async getFcaDefinedStatusesFromCacheThenSet() {
      const definedStatusesLocalJson = localStorage.getItem(
        AppConstants.fcaDefinedStatusesKey,
      );

      if (definedStatusesLocalJson) {
        const definedStatusesLocal = JSON.parse(
          definedStatusesLocalJson,
        ) as FcaAuthStatus[];

        if (definedStatusesLocal) {
          this.fcaDefinedStatuses = definedStatusesLocal;
        }
      }
    },

    async getCompanyHouseDefinedStatusesFromCacheThenSet() {
      const definedStatusesLocalJson = localStorage.getItem(
        AppConstants.companyHouseDefinedStatusesKey,
      );

      if (definedStatusesLocalJson) {
        const definedStatusesLocal = JSON.parse(
          definedStatusesLocalJson,
        ) as CompanyHouseStatus[];

        if (definedStatusesLocal) {
          this.companyHouseDefinedStatuses = definedStatusesLocal;
        }
      }
    },

    async checkIfFirmHasSamePermissionWithUserFirmAsync(
      firmReferenceNumber: string | undefined,
    ) {
      const currentFcaPermissions =
        await this.fcaService.getCurrentFirmFcaPermissionsAsync(
          firmReferenceNumber,
        );

      if (!currentFcaPermissions || currentFcaPermissions.length < 1) {
        this.$emit("onPermissionNotSame", []);
        return;
      }

      // get all added permission on both firms
      const enabledUserFirmPermissions = this.userFirmFcaPermissions.filter(
        (p) => p.state === PermissionStateEnum.Added,
      );
      const currentEnabledFirmPermissions = currentFcaPermissions.filter(
        (p) => p.state === PermissionStateEnum.Added,
      );
      const enabledUserPermissionNames = enabledUserFirmPermissions.map(
        (i) => i.subPermissionName,
      );
      const currentFirmPermissionNames = currentEnabledFirmPermissions.map(
        (i) => i.subPermissionName,
      );
      const hasSamePermission = enabledUserPermissionNames.every((item) =>
        currentFirmPermissionNames.includes(item),
      );

      if (!hasSamePermission) {
        const missingPermissions = enabledUserPermissionNames.filter(
          (item) => !currentFirmPermissionNames.includes(item),
        ) as string[];
        this.$emit("onPermissionNotSame", missingPermissions);
      }
    },

    async scheduleAMeetingAsync() {
      this.kendoPromptInstance?.closeActionDialog();
      let eventTypes = await this.calendlyService.getSchedulingLinkAsync();
      let activeEventTypes = eventTypes.filter((e) => e.active);

      if (!activeEventTypes) {
        this.kendoDialogInstance?.setDialogMessage(
          "There is no active event.",
          "No event found",
        );
        return;
      }

      if (activeEventTypes.length > 1) {
        //this.activeEventTypes = activeEventTypes;
        this.kendoCalendarFlexibleDialogInstance?.showMessageAndContent(
          "",
          "Select Event Type",
        );
        return;
      }

      let firstEventType = activeEventTypes[0];
      window.open(firstEventType.scheduling_url);
    },

    capitalize1stLetterOfEveryWord(input: string | undefined): string {
      if (!input) {
        return "";
      }

      return this.helperService.capitalizeFirstLetterOfWord(input);
    },
  },
});
</script>

<template>
  <div>
    <StackLayout
      orientation="horizontal"
      style="gap: 15px"
      :align="{ horizontal: 'stretch', vertical: 'top' }"
      :style="{ 'margin-top': isAddFirmMarginTop ? '8px' : '0px' }"
    >
      <KendoDialog ref="kendoDialog" />

      <KendoPromptDialog
        ref="kendoActionDialogCompanyNumber"
        primaryActionButtonLabel="Schedule a meeting"
        primaryActionButtonIcon="/calendar.svg"
        @onPrimaryActionClick="scheduleAMeetingAsync"
      >
        <div class="paragraph-m-figtree-regular">
          <!-- This is the old implementation -->
          <!--        <p>Are you sure you wish to update your company number? </p> @onPrimaryActionClick="proceedWithUpdateCompanyNumber"-->
          <!--        <p>Current company number with the FCA is {{ firmDetails?.companyNumber }}</p>-->
          <p>If you wish to change this, please schedule a meeting below.</p>
        </div>
      </KendoPromptDialog>

      <KendoCompaniesAutoCompleteComponent
        v-if="mode === CompanySearchMode.FromCompanyHouse"
        :id="id + '.companyInternal.companyName'"
        :name="id + '.companyInternal.companyName'"
        :label="companyNameLabel"
        :placeholder="computedPlaceholder"
        :value="companyInternal?.firmName"
        :isShowBottomResults="false"
        :isDataLoadedCompletely="!isInitializingInternal"
        :isValueReactive="true"
        :isEditable="isEnableSearch"
        @onAttemptEdit="onAttemptEdit"
        @onInput="onCompanyNameInput"
        @onClear="onClearCompany"
        @onValueChange="onCompanyValueChanged"
        :isShowHighLightColor="false"
      />

      <KendoCompaniesFcaAutoCompleteComponent
        v-else
        :id="id + '.companyInternal.companyName'"
        :name="id + '.companyInternal.companyName'"
        :label="companyNameLabel"
        :placeholder="computedPlaceholder"
        :value="companyInternal?.firmName"
        :isShowBottomResults="false"
        :isDataLoadedCompletely="!isInitializingInternal"
        :isValueReactive="true"
        :isEditable="isEnableSearch"
        :excludedFirm="excludedFirm"
        @onFirmSelectionNotAllowed="onFirmSelectionFailed"
        @onAttemptEdit="onAttemptEdit"
        @onInput="onCompanyNameInput"
        @onClear="onClearCompany"
        @onValueChange="onCompanyValueChanged"
      />

      <KendoCompanyNumberInputComponent
        :name="id + '.companyInternal.companyNumber'"
        :id="id + '.companyInternal.companyNumber'"
        :label="companyNumberLabel"
        :placeholder="$t('common-companynumber-text')"
        :isEditable="isCompanyDetailsEditAllowed"
        @onValueChange="onCompanyNumberInput"
        :indicatorLabel="companyStatusText"
        @onAttemptEdit="onAttemptEdit"
        :value="companyInternal?.companyNumber"
        :isValueReactive="true"
        :isDataLoadedCompletely="!isInitializingInternal"
        :isOverrideCommonPlaceHolderText="true"
        :statusLabelColor="colorCodingCompanyHouse"
      />

      <KendoFirmReferenceNumberInputComponent
        :name="id + '.companyInternal.referenceNumber'"
        :id="id + '.companyInternal.referenceNumber'"
        :label="firmReferenceNumberLabel"
        :indicatorLabel="
          fcaStatusText
        "
        :placeholder="$t('common-firmref-text')"
        :isEditable="isCompanyDetailsEditAllowed"
        @onValueChange="onFirmReferenceNumberInput"
        @onAttemptEdit="onAttemptEdit"
        :value="companyInternal?.firmReferenceNumber"
        :maxLength="7"
        :isValueReactive="true"
        :isDataLoadedCompletely="!isInitializingInternal"
        :isOverrideCommonPlaceHolderText="true"
        :statusLabelColor="colorCodingFCA"
        :maxLengthBeforeTruncate="maxLengthBeforeTruncate"
      />
    </StackLayout>
  </div>
</template>

<style scoped />