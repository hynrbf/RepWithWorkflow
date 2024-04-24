<script lang="ts">
import { defineAsyncComponent, defineComponent, inject } from "vue";
import { Emitter, EventType } from "mitt";
import { AppConstants } from "@/infra/AppConstants";
import { container } from "tsyringe";
import { ProfessionalIndemnity } from "@/entities/professional-indemnity/ProfessionalIndemnity";
import {
  ICustomerService,
  ICustomerServiceInfo,
} from "@/infra/dependency-services/rest/forms-compliance/ICustomerService";
import { CustomerEntity } from "@/entities/CustomerEntity";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import { LimitIndemnity } from "@/entities/professional-indemnity/LimitIndemnity";
import { PolicyExcess } from "@/entities/professional-indemnity/PolicyExcess";
import { PolicyExclusion } from "@/entities/professional-indemnity/PolicyExclusion";
import StaticList from "../../../infra/StaticListService";
import { CompanyEntity } from "@/entities/CompanyEntity";
import { FirmBasicInfo } from "@/entities/FirmBasicInfo";
import {
  IAppService,
  IAppServiceInfo,
} from "@/infra/dependency-services/app/IAppService";
import { AlertType, useAlert } from "@/composables/useAlert";
import { Money } from "@/entities/Money";
import { usePageLifeCycleStore } from "@/stores/progress-bar/usePageLifeCycleStore";
import {
  NotificationType,
  useNotification,
} from "@/composables/useNotification";
import {
  IUserSubmittedChangesService,
  IUserSubmittedChangesServiceInfo,
} from "@/infra/dependency-services/user-submission/IUserSubmittedChangesService";
import { useAutoSaveStore } from "@/stores/useAutoSaveStore";
import { ProfessionalIndemnityModel } from "@/pages/models/professional-indemnity-insurance/ProfessionalIndemnityModel";
import {
  IMapperService,
  IMapperServiceInfo,
} from "@/infra/dependency-services/mapper/IMapperService";
import { ContactNumber } from "@/entities/ContactNumber";
import {
  IFcaService,
  IFcaServiceInfo,
} from "@/infra/dependency-services/rest/fca/IFcaService";

export default defineComponent({
  name: "ProfessionalIndemnityInsurance",
  components: {
    KendoFirmFinderComponentAsync: defineAsyncComponent(
      () => import("@/components/form-fields/KendoFirmFinderComponent.vue"),
    ),
  },
  data() {
    return {
      customerService: container.resolve<ICustomerService>(
        ICustomerServiceInfo.name,
      ),
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      appService: container.resolve<IAppService>(IAppServiceInfo.name),
      fcaService: container.resolve<IFcaService>(IFcaServiceInfo.name),
      isLoading: true,
      isShowSavingText: false,
      firmName: "<Firm Name>",
      customer: new CustomerEntity(),
      typesOfExclusion: [] as string[],
      timePeriodOfExclusion: [] as string[],
      coveredUnderPolicy: [
        "Select All",
        "Appointed Representative 1",
        "Appointed Representative 2",
        "Appointed Representative 3",
        "Appointed Representative 4",
      ],
      businessLinesCoveredItems: [] as string[],
      insurerNames: [] as string[],
      //this is a model. not explicitly typed?
      professionalIndemnityItems: [] as ProfessionalIndemnityModel[],
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      eventBusFormSaved: inject("$eventBusService") as Emitter<
        Record<EventType, boolean>
      >,
      hasAppointedRepresentatives: false,
      userSubmittedChangesService:
        container.resolve<IUserSubmittedChangesService>(
          IUserSubmittedChangesServiceInfo.name,
        ),
      mapperService: container.resolve<IMapperService>(IMapperServiceInfo.name),
      isSavingAlertOpened: false,
      isInitializing: true,
      debouncedAutoSaveFunction: () => {},
      activePiiTabIndex: 0,
    };
  },
  setup() {
    const pageLifeCycleStore = usePageLifeCycleStore();
    const { changeLifeCycleName } = pageLifeCycleStore;
    const { debounceFunction, setAutoSaveFunctionNotCompletedYet } =
      useAutoSaveStore();

    return {
      changeLifeCycleName,
      debounceFunction,
      setAutoSaveFunctionNotCompletedYet,
    };
  },
  computed: {
    ContactNumber() {
      return ContactNumber;
    },
    Money() {
      return Money;
    },
    AppConstants() {
      return AppConstants;
    },
    piiTabs: {
      get() {
        return this.professionalIndemnityItems.map(({ id, pii }, index) => ({
          id: `pii-${id}`,
          title: pii.insurerName ?? "<Insurer Name>",
          content: "",
          active: this.activePiiTabIndex === index,
        }));
      },
      set(tabs: any[]) {
        for (const index in tabs) {
          if (tabs[index].active) {
            this.activePiiTabIndex = +index;
            break;
          }
        }
      },
    },
  },
  created() {
    this.insurerNames = StaticList.getPiiInsurerNames();
    this.businessLinesCoveredItems = StaticList.getPiiBusinessLinesCovered();
    this.timePeriodOfExclusion = StaticList.getPiiTimePeriodOfExclusion();
    this.typesOfExclusion = StaticList.getPiiTypesOfExclusion();

    this.customer = this.appService.getCachedCustomer() ?? new CustomerEntity();
    this.firmName = this.appService.getCustomerFirmName();

    const selectedCompany = this.customer.selectedCompany as CompanyEntity;

    if (selectedCompany?.appointedRepresentatives) {
      this.hasAppointedRepresentatives =
        selectedCompany.appointedRepresentatives.length > 0 ?? false;
    }
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameCreated);
    this.isInitializing = false;
    this.debouncedAutoSaveFunction = this.debounceFunction(this.saveInfoAsync);
  },
  mounted() {
    try {
      let professionalIndemnities: ProfessionalIndemnity[] = [];
      this.professionalIndemnityItems = [];

      if (!(this.customer.professionalIndemnities?.length > 0)) {
        this.addInitialProfessionalIndemnityItem();
        return;
      }

      professionalIndemnities = this.customer.professionalIndemnities;

      for (const item of professionalIndemnities) {
        this.populateDynamicDropDownLists(item);
      }

      this.professionalIndemnityItems =
        this.mapperService.mapProfessionalIndemnityEntitiesToModels(
          professionalIndemnities,
        );
    } finally {
      this.isLoading = false;

      this.eventBus.emit(AppConstants.bottomBarEnableEvent, true);
      this.eventBusFormSaved.on(AppConstants.formSavedEvent, (isAutoNext) =>
        this.handleSubmit(isAutoNext),
      );
      this.eventBus.on(AppConstants.formFieldPageLevelChangedEvent, () => {
        this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi =
          false;
        this.setAutoSaveFunctionNotCompletedYet();
        this.debouncedAutoSaveFunction();
      });

      this.helperService.resizeExpander();
      this.changeLifeCycleName(AppConstants.pageLifeCycleNameMounted);
      this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi =
        false;
    }
  },
  unmounted() {
    this.eventBus.emit(AppConstants.bottomBarEnableEvent, false);
    this.eventBusFormSaved.off(AppConstants.formSavedEvent);
    this.eventBus.off(AppConstants.formFieldPageLevelChangedEvent);
    this.isLoading = false;
  },
  methods: {
    onBusinessLineCoveredValueChange(
      value: string[],
      item: ProfessionalIndemnity,
    ) {
      item.businessLinesCovered = value;
      this.populateDynamicDropDownLists(item);
    },

    populateDynamicDropDownLists(item: ProfessionalIndemnity) {
      const defaultBusinessLineCovered = this.businessLinesCoveredItems;
      const businessLineCovered =
        item.businessLinesCovered ?? defaultBusinessLineCovered;
      item.businessLineCategoriesSubjectToPolicyExcess =
        StaticList.getPiiBusinessLineCategoriesSubjectToPolicyExcess(
          businessLineCovered,
        );
      item.businessLineCategoriesSubjectToPolicyExclusions =
        StaticList.getPiiBusinessLineCategoriesSubjectToPolicyExclusions(
          businessLineCovered,
        );
      item.businessLineSubjectToLimitOfIndemnityItemsSingle =
        StaticList.getPiiBusinessLinesSubjectToLimitOfIndemnitySingle(
          businessLineCovered,
        );
      item.businessLineSubjectToLimitOfIndemnityItemsAggregate =
        StaticList.getPiiBusinessLinesSubjectToLimitOfIndemnityAggregate(
          businessLineCovered,
        );
    },

    addInitialProfessionalIndemnityItem() {
      const professionalIndemnity = new ProfessionalIndemnity();
      professionalIndemnity.limitIndemnitiesSingle = [new LimitIndemnity()];
      professionalIndemnity.limitIndemnitiesAggregate = [new LimitIndemnity()];
      professionalIndemnity.policyExcesses = [new PolicyExcess()];
      professionalIndemnity.policyExclusions = [new PolicyExclusion()];
      this.populateDynamicDropDownLists(professionalIndemnity);
      this.professionalIndemnityItems.push(
        new ProfessionalIndemnityModel(professionalIndemnity),
      );
      this.activePiiTabIndex = this.professionalIndemnityItems.length - 1;
    },

    removeProfessionalIndemnityItem(indexToRemove: number) {
      this.professionalIndemnityItems.splice(indexToRemove, 1);
      if (
        indexToRemove <= this.activePiiTabIndex &&
        this.activePiiTabIndex > 0
      ) {
        this.activePiiTabIndex -= 1;
      }
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    },

    addLimitOfIndemnitySingle(item: ProfessionalIndemnity) {
      item.limitIndemnitiesSingle.push(new LimitIndemnity());
    },

    addLimitOfIndemnityAggregate(item: ProfessionalIndemnity) {
      item.limitIndemnitiesAggregate.push(new LimitIndemnity());
    },

    addPolicyExcess(item: ProfessionalIndemnity) {
      item.policyExcesses.push(new PolicyExcess());
    },

    addPolicyExclusion(item: ProfessionalIndemnity) {
      item.policyExclusions.push(new PolicyExclusion());
    },

    convertEpochValueToDate(input: number | undefined): Date | undefined {
      if (!input) {
        return undefined;
      }

      return this.helperService.convertEpochToDateTime(input);
    },

    onRetroactiveStartDateChange(value: Date, item: ProfessionalIndemnity) {
      item.retroactiveStartDate = this.helperService.dateStringToEpoch(
        value.toDateString(),
      );
    },

    onPiiStartDateChange(value: Date, item: ProfessionalIndemnity) {
      item.startDate = this.helperService.dateStringToEpoch(
        value.toDateString(),
      );
    },

    onPiiEndDateChange(value: Date, item: ProfessionalIndemnity) {
      item.endDate = this.helperService.dateStringToEpoch(value.toDateString());
    },

    async onPiiInsurerDetailUpdatedAsync(
      piiInsurer: FirmBasicInfo,
      index: number,
    ) {
      if (!this.professionalIndemnityItems[index]) {
        return;
      }

      this.professionalIndemnityItems[index].pii.insurerName =
        piiInsurer?.firmName ?? "";

      if (!piiInsurer?.firmName) {
        this.professionalIndemnityItems[index].pii.companyNumber = "";
        this.professionalIndemnityItems[index].pii.firmReferenceNumber = "";
        this.professionalIndemnityItems[index].piiInsurerInputFirm =
          new FirmBasicInfo();
      } else {
        this.professionalIndemnityItems[index].pii.companyNumber =
          piiInsurer?.companyNumber ?? "";
        this.professionalIndemnityItems[index].pii.firmReferenceNumber =
          piiInsurer?.firmReferenceNumber ?? "";
        this.professionalIndemnityItems[index].pii.insurerRegisteredAddress =
          piiInsurer.address ?? "";
        this.professionalIndemnityItems[index].pii.insurerTradingAddress =
          piiInsurer.tradingAddress ?? "";
        this.professionalIndemnityItems[index].pii.insurerWebsite =
          piiInsurer.website ?? "";
        this.professionalIndemnityItems[index].piiInsurerInputFirm = piiInsurer;
      }

      if (piiInsurer.firmReferenceNumber) {
        const addressDetails =
          await this.fcaService.getFirmAddressesDetailsAsync(
            piiInsurer.firmReferenceNumber,
            "PPOB",
          );

        if (addressDetails[0]["Phone Number"] && addressDetails[0]["country"]) {
          this.professionalIndemnityItems[index].pii.insurerContactNumber =
            await this.helperService.convertToContactNoAsync(
              addressDetails[0]["Phone Number"],
              addressDetails[0]["country"],
            );
        }
      }
    },

    async onPiiBrokerDetailUpdated(piiBroker: FirmBasicInfo, index: number) {
      this.professionalIndemnityItems[index].pii.piiBrokerName =
        piiBroker?.firmName ?? "";

      if (!piiBroker.firmName) {
        this.professionalIndemnityItems[index].pii.brokerCompanyNumber = "";
        this.professionalIndemnityItems[index].pii.brokerFirmReferenceNumber =
          "";
        this.professionalIndemnityItems[index].piiBrokerInputFirm =
          new FirmBasicInfo();
      } else {
        this.professionalIndemnityItems[index].pii.brokerCompanyNumber =
          piiBroker?.companyNumber ?? "";
        this.professionalIndemnityItems[index].pii.brokerFirmReferenceNumber =
          piiBroker?.firmReferenceNumber ?? "";
        this.professionalIndemnityItems[index].pii.brokerRegisteredAddress =
          piiBroker.address ?? "";
        this.professionalIndemnityItems[index].pii.brokerTradingAddress =
          piiBroker.tradingAddress ?? "";
        this.professionalIndemnityItems[index].pii.brokerWebsite =
          piiBroker.website ?? "";
        this.professionalIndemnityItems[index].piiBrokerInputFirm = piiBroker;
      }

      if (piiBroker.firmReferenceNumber) {
        const addressDetails =
          await this.fcaService.getFirmAddressesDetailsAsync(
            piiBroker.firmReferenceNumber,
            "PPOB",
          );

        if (addressDetails[0]["Phone Number"] && addressDetails[0]["country"]) {
          this.professionalIndemnityItems[index].pii.brokerContactNumber =
            await this.helperService.convertToContactNoAsync(
              addressDetails[0]["Phone Number"],
              addressDetails[0]["country"],
            );
        }
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
            "Professional Indemnity Insurance (PII) is incomplete. Are you sure you wish to proceed?",
          confirmButtonText: "Proceed",
          saveDetailsText: "Complete Now",
          type: AlertType.SAVEDETAILS,
          width: 390,
          onSaveDetails: () => {
            this.isSavingAlertOpened = false;
            (
              this.$refs?.formElement as HTMLFormElement
            )?.$el?.requestSubmit?.();
          },
          onConfirm: async () => {
            await this.saveInfoAsync(true, isAutoNext);
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
          title: this.$t("common-alert-title"),
          content: this.$t("common-alert-content"),
          confirmButtonText: this.$t("common-alert-buttonText"),
          onConfirm: async () => {
            await this.saveInfoAsync(true);
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
      (this.$refs?.formElement as HTMLFormElement)?.$el?.requestSubmit?.();
    },

    async saveInfoAsync(
      isShowLoader: boolean = false,
      isAutoNext: boolean = false,
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
      updatedCustomer.professionalIndemnities =
        this.professionalIndemnityItems.map((i) => i.pii);
      const updateStringItem = JSON.stringify(updatedCustomer);
      await this.customerService
        .saveCustomerAsync(updateStringItem)
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
        });
    },

    getInsurerNameTitle(title: string, insurerName: string): string {
      if (!insurerName) {
        return title;
      }

      return `${title}:${insurerName}`;
    },

    makePiiNav(index: number) {
      return [
        {
          id: `pii-details-${index}`,
          anchorTo: `panel-pii-details-${index}`,
          label: this.$t("professionalIndemnityInsurancePage-anchor-details"),
          icon: "business-card",
          active: true,
        },
        {
          id: `broker-details-${index}`,
          anchorTo: `panel-broker-details-${index}`,
          label: this.$t("professionalIndemnityInsurancePage-brokerDetails"),
          icon: "upload-square-4",
        },
        {
          id: `summary-of-cover-${index}`,
          anchorTo: `panel-summary-of-cover-${index}`,
          label: this.$t("professionalIndemnityInsurancePage-summaryOfCover"),
          icon: "upload-square-4",
        },
        {
          id: `limit-exclusions-${index}`,
          anchorTo: `panel-limit-exclusions-${index}`,
          label: this.$t(
            "professionalIndemnityInsurancePage-anchor-limitations-exclusions",
          ),
          icon: "hand-held-tablet-writing-77",
        },
        {
          id: `document-upload-${index}`,
          anchorTo: `panel-document-upload-${index}`,
          label: this.$t(
            "professionalIndemnityInsurancePage-anchor-document-upload",
          ),
          icon: "upload-square-4",
        },
      ];
    },

    setUniqueIdentifier(value: string): string {
      const identifier = `${AppConstants.professionalIndemnityRoute}${value}`;
      return identifier.replace(/\s+/g, "").replace("/", "");
    },
  },
});
</script>

<template src="./professional-indemnity-insurance.html" />

<style scoped src="./professional-indemnity-insurance.css" />
