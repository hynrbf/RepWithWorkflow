<script lang="ts">
import { defineAsyncComponent, defineComponent, inject } from "vue";
import { Emitter, EventType } from "mitt";
import { AppConstants } from "@/infra/AppConstants";
import { container } from "tsyringe";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import { EmployersLiability } from "@/entities/employers-liability/EmployersLiability";
import {
  IAppService,
  IAppServiceInfo,
} from "@/infra/dependency-services/app/IAppService";
import { AlertType, useAlert } from "@/composables/useAlert";
import { FirmBasicInfo } from "@/entities/FirmBasicInfo";
import { Money } from "@/entities/Money";
import { KendoAlertDialogComponent } from "@/components/KendoAlertDialog.vue";
import { usePageLifeCycleStore } from "@/stores/progress-bar/usePageLifeCycleStore";
import {
  IUserSubmittedChangesService,
  IUserSubmittedChangesServiceInfo,
} from "@/infra/dependency-services/user-submission/IUserSubmittedChangesService";
import { useAutoSaveStore } from "@/stores/useAutoSaveStore";
import { EmployersLiabilityModel } from "@/pages/models/employers-liability-insurance/EmployersLiabilityModel";
import {
  IAppointedRepresentativeService,
  IAppointedRepresentativeServiceInfo,
} from "@/infra/dependency-services/rest/appointed-representative/IAppointedRepresentativeService";
import { OnboardingType } from "@/infra/base";
import { AppointedRepresentative } from "@/entities/appointed-representatives/AppointedRepresentative";

export default defineComponent({
  name: "AREmployersLiabilityInsurance",
  components: {
    KendoFirmFinderComponentAsync: defineAsyncComponent(
      () => import("@/components/form-fields/KendoFirmFinderComponent.vue"),
    ),
  },
  data() {
    return {
      customerArService: container.resolve<IAppointedRepresentativeService>(
        IAppointedRepresentativeServiceInfo.name,
      ),
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      appService: container.resolve<IAppService>(IAppServiceInfo.name),
      kendoAlertDialogInstance: null as KendoAlertDialogComponent | null,
      arCustomer: new AppointedRepresentative(),
      firmName: "<Firm Name>",
      paymentFrequencyList: [
        "Monthly (x10)",
        "Monthly (x11)",
        "Monthly (x12)",
        "Bi-annually",
        "Bi-annually",
        "Annually",
      ],
      isLoading: true,
      isShowSavingText: false,
      employerLiabilityInsurancesItem: [] as EmployersLiabilityModel[],
      isComplete: false,
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      eventBusFormSaved: inject("$eventBusService") as Emitter<
        Record<EventType, boolean>
      >,
      isInitializing: true,
      userSubmittedChangesService:
        container.resolve<IUserSubmittedChangesService>(
          IUserSubmittedChangesServiceInfo.name,
        ),
      isSavingAlertOpened: false,
      debouncedAutoSaveFunction: () => {},
      activeEliTabIndex: 0,
      isTradingSameAsRegisteredAddress: false,
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
    OnboardingType() {
      return OnboardingType;
    },
    Money() {
      return Money;
    },
    AppConstants() {
      return AppConstants;
    },
    eliTabs: {
      get() {
        return this.employerLiabilityInsurancesItem.map(
          ({ id, employerLiability }, index) => ({
            id: `eli-${id}`,
            title: employerLiability.insurer.firmName ?? "New Insurer",
            content: "",
            active: this.activeEliTabIndex === index,
          }),
        );
      },
      set(tabs: { active: boolean }[]) {
        for (const index in tabs) {
          if (tabs[index].active) {
            this.activeEliTabIndex = +index;
            break;
          }
        }
      },
    },
    ARFullName() {
      return `${this.arCustomer.firstName} ${this.arCustomer.lastName}`;
    },
  },
  async created() {
    // Add initial item
    this.addEmployerLiabilityInsuranceItem();
    this.arCustomer =
      this.appService.getCachedCustomerAppointedRepresentative() ??
      new AppointedRepresentative();
    this.firmName = this.appService.getCustomerArFirmName();

    if (this.arCustomer.employersLiabilities?.length > 0) {
      this.employerLiabilityInsurancesItem = [];
      // setup item template
      for (const liability of this.arCustomer.employersLiabilities) {
        this.employerLiabilityInsurancesItem.push(
          new EmployersLiabilityModel(liability),
        );
      }
    }
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameCreated);
    this.debouncedAutoSaveFunction = this.debounceFunction(this.saveInfoAsync);
  },
  mounted() {
    this.isLoading = false;

    if (this.$refs.kendoAlertDialog) {
      this.kendoAlertDialogInstance = this.$refs
        .kendoAlertDialog as KendoAlertDialogComponent;
    }

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
    this.isInitializing = false;
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameMounted);
    this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi = false;
  },
  unmounted() {
    this.eventBus.emit(AppConstants.bottomBarEnableEvent, false);
    this.eventBusFormSaved.off(AppConstants.formSavedEvent);
    this.eventBus.off(AppConstants.formFieldPageLevelChangedEvent);
    this.isLoading = false;
  },
  methods: {
    async handleSubmit(isAutoNext: boolean) {
      if (this.isSavingAlertOpened) {
        return;
      }

      if (isAutoNext) {
        useAlert({
          title: "Confirm",
          content:
            "Employer's Liability Insurance (ELI) is incomplete. Are you sure you wish to proceed?",
          confirmButtonText: "Proceed",
          saveDetailsText: "Complete Now",
          type: AlertType.SAVEDETAILS,
          width: 390,
          onSaveDetails: () => {
            this.isSavingAlertOpened = false;
            (this.$refs?.formElement as HTMLFormElement)?.$el?.requestSubmit?.();
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
      const updatedArCustomer =
        await this.customerArService.getAppointedRepresentativesByEmailAsync(
          this.arCustomer?.email ?? "",
        );
      updatedArCustomer.employersLiabilities =
        this.employerLiabilityInsurancesItem.map((i) => i.employerLiability);
      await this.customerArService
        .saveOrUpdateAppointedRepresentativeAsync(updatedArCustomer)
        .then(() => {
          if (isShowLoader) {
            this.kendoAlertDialogInstance?.showAlertDialogMessage(
              "Employers’ Liability Insurance Added.",
            );
          }

          this.isLoading = false;
          this.isShowSavingText = false;

          if (isAutoNext) {
            setTimeout(() => this.eventBus.emit(AppConstants.autoNextEvent), 0);
          }
        });
    },

    addEmployerLiabilityInsuranceItem() {
      this.employerLiabilityInsurancesItem.push(new EmployersLiabilityModel());
      this.helperService.resizeExpander();
    },

    removeEmployerLiabilityInsuranceItem(indexToRemove: number) {
      this.employerLiabilityInsurancesItem.splice(indexToRemove, 1);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    },

    onRetroactiveStartDateChange(value: Date, item: EmployersLiability) {
      item.retroactiveStartDate = this.helperService.dateStringToEpoch(
        value.toDateString(),
      );
    },

    onStartDateChange(value: Date, item: EmployersLiability) {
      item.startDate = this.helperService.dateStringToEpoch(
        value.toDateString(),
      );
    },

    onEndDateChange(value: Date, item: EmployersLiability) {
      item.endDate = this.helperService.dateStringToEpoch(value.toDateString());
    },

    convertEpochValueToDate(input: number | undefined): Date | undefined {
      if (!input) {
        return undefined;
      }

      return this.helperService.convertEpochToDateTime(input);
    },

    showDetails(item: {
      employerLiability: EmployersLiability;
      isCollapsed: boolean;
    }) {
      item.isCollapsed = !item.isCollapsed;
      this.helperService.resizeExpander();
    },

    onInsurerUpdated(item: EmployersLiability, firm: FirmBasicInfo) {
      if (!item) {
        return;
      }

      item.insurer = firm;
      item.insurerRegisteredAddress = firm.address;
      item.insurerTradingAddress = firm.tradingAddress;
      item.insurerWebsite = firm.website;
    },

    onBrokerUpdated(item: EmployersLiability, firm: FirmBasicInfo) {
      if (!item) {
        return;
      }

      item.broker = firm;
      item.brokerRegisteredAddress = firm.address;
      item.brokerTradingAddress = firm.tradingAddress;
      item.brokerWebsite = firm.website;
    },

    onPremiumAmountChanged(item: EmployersLiability, value: Money) {
      if (!item) {
        return;
      }

      item.premiumAmount = value;
    },

    onSingleIndemnityLimitChanged(item: EmployersLiability, value: Money) {
      if (!item) {
        return;
      }

      item.singleIndemnityLimit = value;
    },

    onAggregateIndemnityLimitChanged(item: EmployersLiability, value: Money) {
      if (!item) {
        return;
      }

      item.aggregateIndemnityLimit = value;
    },

    onPolicyExcessChanged(item: EmployersLiability, value: Money) {
      if (!item) {
        return;
      }

      item.policyExcess = value;
    },

    setUniqueIdentifier(value: string): string {
      const identifier = `${AppConstants.arEmployersLiabilityRoute}${value}`;
      return identifier.replace(/\s+/g, "").replace("/", "");
    },

    onEliStartDateChange(value: Date, item: EmployersLiability) {
      item.startDate = this.helperService.dateStringToEpoch(
        value.toDateString(),
      );
    },

    onEliEndDateChange(value: Date, item: EmployersLiability) {
      item.endDate = this.helperService.dateStringToEpoch(value.toDateString());
    },

    makeEliNav(index: number) {
      return [
        {
          id: `eli-details-${index}`,
          anchorTo: `panel-eli-details-${index}`,
          label: this.$t("employersLiabilityInsurancePage-insurerDetails"),
          icon: "business-card",
          active: true,
        },
        {
          id: `broker-details-${index}`,
          anchorTo: `panel-broker-details-${index}`,
          label: this.$t("employersLiabilityInsurancePage-brokerDetails"),
          icon: "upload-square-4",
        },
        {
          id: `summary-of-cover-${index}`,
          anchorTo: `panel-summary-of-cover-${index}`,
          label: this.$t("employersLiabilityInsurancePage-summaryOfCover"),
          icon: "upload-square-4",
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

    onEliRegisteredAddressChanged(value: string, item: EmployersLiability) {
      item.insurerRegisteredAddress = value;
      this.isTradingSameAsRegisteredAddress = item.insurerRegisteredAddress?.toLowerCase() === item.insurerTradingAddress?.toLowerCase();
    },

    onEliTradingAddressChanged(value: string, item: EmployersLiability) {
      item.insurerTradingAddress = value;
      this.isTradingSameAsRegisteredAddress = item.insurerRegisteredAddress?.toLowerCase() === item.insurerTradingAddress?.toLowerCase();
    },

    onToggleHandler(item: EmployersLiability) {
      this.isTradingSameAsRegisteredAddress = !this.isTradingSameAsRegisteredAddress;
      item.insurerTradingAddress = this.isTradingSameAsRegisteredAddress ? item.insurerRegisteredAddress : "";  
    }
  },
});
</script>

<template src="./ar-employers-liability-insurance.html" />

<style scoped src="./ar-employers-liability-insurance.css" />