<script lang="ts">
import {defineAsyncComponent, defineComponent, inject} from "vue";
import { Emitter, EventType } from "mitt";
import { AppConstants } from "@/infra/AppConstants";
import { useCustomerStore } from "@/stores/useCustomerStore";
import { useClientMoneyStore } from "@/stores/useClientMoneyStore";
import { mapState, mapActions } from "pinia";
import { ClientMoneyEntity } from "@/entities/client-money/ClientMoneyEntity";
import { v4 as uuid } from "uuid";
import { ClassificationEnum } from "@/entities/client-money/ClassificationEnum";
import { FirmRepresentative } from "@/entities/FirmRepresentative";
import { InsuranceProvider } from "@/entities/client-money/InsuranceProvider";
import { ClientMoneyAudit } from "@/entities/client-money/ClientMoneyAudit";
import moment from "moment";
import { AlertType, useAlert } from "@/composables/useAlert";
import {
  NotificationType,
  useNotification,
} from "@/composables/useNotification";
import {
  IFcaService,
  IFcaServiceInfo,
} from "@/infra/dependency-services/rest/fca/IFcaService";
import InsurerRating from "./partials/InsurerRating.vue";
import InsuranceProviderDetails from "./partials/InsuranceProviderDetails.vue";
import InuranceProviderTabs from "./partials/InuranceProviderTabs.vue";
import { usePageLifeCycleStore } from "@/stores/progress-bar/usePageLifeCycleStore";
import { container } from "tsyringe";
import {
  ICompaniesHouseService,
  ICompaniesHouseServiceInfo,
} from "@/infra/dependency-services/rest/company-house/ICompaniesHouseService";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import {
  IUserSubmittedChangesService,
  IUserSubmittedChangesServiceInfo,
} from "@/infra/dependency-services/user-submission/IUserSubmittedChangesService";
import { usePageFieldsInvalidHandlerStore } from "@/stores/usePageFieldsInvalidHandlerStore";
import { ContactNumber } from "@/entities/ContactNumber";

export default defineComponent({
  name: "ClientMoney",
  components: {
    InsurerRating,
    InsuranceProviderDetails,
    InuranceProviderTabs,
    KendoFirmFinderComponentAsync: defineAsyncComponent(
      () => import("@/components/form-fields/KendoFirmFinderComponent.vue"),
    ),
  },
  data() {
    return {
      moment,
      isLoading: false,
      isShowSavingText: false,
      currentTab: 0,
      tabs: [
        {
          title: this.$t("clientMoneyPage-providerDetails"),
          content: "providerDetails",
        },
        {
          title: this.$t("clientMoneyPage-clientMoneyAudit"),
          content: "clientMoneyAudit",
        },
      ],
      activeProviderIndex: 0,
      providers: [] as ClientMoneyEntity[],
      clientMoneyAudit: {} as ClientMoneyAudit,
      ClassificationEnum,
      classificationOptions: [
        {
          label: ClassificationEnum.Insurer,
          value: this.$t("clientMoneyPage-insurer"),
        },
        {
          label: ClassificationEnum.MGA,
          value: this.$t("clientMoneyPage-mga"),
        },
        {
          label: ClassificationEnum.WholesaleBroker,
          value: this.$t("clientMoneyPage-wholesaleBroker"),
        },
      ],
      accountTypeOptions: [
        {
          label: this.$t("clientMoneyPage-statutoryClientAccount"),
          value: "Statutory Client Account",
        },
        {
          label: this.$t("clientMoneyPage-nonStatutoryClientAccount"),
          value: "Non Statutory Client Account",
        },
        {
          label: this.$t("clientMoneyPage-insurerTrustAccount"),
          value: "Insurer Trust Account",
        },
      ],
      disabledAccountTypes: ["Insurer Trust Account"],
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      eventBusFormSaved: inject("$eventBusService") as Emitter<
        Record<EventType, boolean>
      >,
      isInitializing: true,
      companiesHouseService: container.resolve<ICompaniesHouseService>(
        ICompaniesHouseServiceInfo.name,
      ),
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      userSubmittedChangesService:
        container.resolve<IUserSubmittedChangesService>(
          IUserSubmittedChangesServiceInfo.name,
        ),
      isSavingAlertOpened: false,
      accountPeriodStartOn: moment().startOf("y").subtract(1, "y").unix(),
      accountPeriodEndOn: moment().endOf("y").subtract(1, "y").unix(),
      accountMadeUpTo: moment().endOf("y").subtract(1, "y").unix(),
      fcaService: container.resolve<IFcaService>(IFcaServiceInfo.name),
    };
  },
  provide() {
    return {
      populateRelatedFieldsAsync: this.populateRelatedFieldsAsync,
    };
  },
  computed: {
    ...mapState(useCustomerStore, [
      "currentCustomer",
      "currentFirmName",
      "currentCompanyNumber",
    ]),
    ...mapState(useClientMoneyStore, ["insurerRatings"]),
    AppConstants() {
      return AppConstants;
    },
    providersTabs: {
      get() {
        return this.providers.map(({ id, providerName }, index) => ({
          id: `provider-${id}`,
          title: providerName ?? `<Provider Name>`,
          content: "",
          active: this.activeProviderIndex === index,
        }));
      },
      set(tabs: any[]) {
        for (const index in tabs) {
          if (tabs[index].active) {
            this.activeProviderIndex = +index;
            break;
          }
        }
      },
    },
    providerPanels() {
      return this.providers.map(({ id }) => ({
        id: `provider-${id}`,
        title: this.$t("clientMoneyPage-title"),
        content: "",
      }));
    },
  },
  methods: {
    ...mapActions(useCustomerStore, ["updateCustomerByEmailAsync"]),
    ...mapActions(useClientMoneyStore, ["fetchInsurerRatingsAsync"]),
    setUniqueIdentifier(value: string): string {
      const identifier = `${AppConstants.clientMoneyRoute}${value}`;
      return identifier.replace(/\s+/g, "").replace("/", "");
    },
    selectTab(event: any) {
      this.currentTab = event.selected;
    },
    addProvider(index: number) {
      const provider = new ClientMoneyEntity();
      provider.id = uuid();
      provider.firmRepresentative = new FirmRepresentative();
      this.providers.push(provider);
      this.activeProviderIndex = index;
      this.$nextTick(() => {
        (this.$refs["clientMoneyPanel"] as any).expand(
          `provider-${provider.id}`,
        );
      });
    },
    removeProvider(index: number) {
      const id = this.providers[index].id;
      this.providers = this.providers.filter((provider) => provider.id !== id);
      if (!this.providers.length) {
        this.addProvider(0);
      }
      if (this.activeProviderIndex === index) {
        this.activeProviderIndex = 0;
      }
    },
    makeProviderNav(id: string) {
      return [
        {
          id: `provider-details-${id}`,
          anchorTo: `panel-provider-details-${id}`,
          label: this.$t("clientMoneyPage-providerDetails"),
          icon: "business-card",
          active: true,
        },
        {
          id: `risk-transfer-funds-${id}`,
          anchorTo: `panel-risk-transfer-funds-${id}`,
          label: this.$t("clientMoneyPage-riskTransferFunds"),
          icon: "money-transfer-57",
        },
        {
          id: `representative-details-${id}`,
          anchorTo: `panel-representative-details-${id}`,
          label: this.$t("clientMoneyPage-representativeDetails"),
          icon: "user-protection-check",
        },
      ];
    },
    makeSubProviderPanels(id: string) {
      return [
        {
          id: `provider-details-${id}`,
          title: this.$t("clientMoneyPage-providerDetails"),
          content: "",
          active: true,
        },
        {
          id: `insurance-provider-details-${id}`,
          title: this.$t("clientMoneyPage-insuranceProviderDetails"),
          content: "",
          active: true,
        },
        {
          id: `risk-transfer-funds-${id}`,
          title: this.$t("clientMoneyPage-riskTransferFunds"),
          content: "",
          active: true,
        },
        {
          id: `representative-details-${id}`,
          title: this.$t("clientMoneyPage-representativeDetails"),
          content: "",
          active: true,
        },
      ];
    },
    addInsuranceProvider(index: number) {
      this.providers[index].insuranceProviders.push(new InsuranceProvider());
    },
    setClientMoneyAudit() {
      const clientMoneyAudit = new ClientMoneyAudit();
      clientMoneyAudit.highestAmount1 = 0;
      clientMoneyAudit.highestAmount2 = 0;
      this.clientMoneyAudit = clientMoneyAudit;
    },
    submit(isAutoNext: boolean) {
      if (this.isSavingAlertOpened) {
        return;
      }

      if (isAutoNext) {
        useAlert({
          title: "Confirm",
          content:
            "Client Money is incomplete. Are you sure you wish to proceed?",
          confirmButtonText: "Proceed",
          saveDetailsText: "Complete Now",
          type: AlertType.SAVEDETAILS,
          width: 390,
          onSaveDetails: () => {
            this.isSavingAlertOpened = false;
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
      (this.$refs["formElement2"] as any)?.$el?.requestSubmit?.();
    },
    async saveInfoAsync(
      isShowLoader: boolean = false,
      isAutoNext: boolean = false,
    ): Promise<void> {
      if (!this.currentCustomer) {
        return;
      }

      this.processInvalidFields(AppConstants.clientMoneyRoute, this.eventBus);
      const { email: customerEmail = "" } = this.currentCustomer;
      this.isShowSavingText = isShowLoader;
      this.isLoading = isShowLoader;
      this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi =
        true;
      this.isSavingAlertOpened = false;

      this.updateCustomerByEmailAsync(customerEmail, {
        clientMonies: this.providers,
        clientMoneyAudit: this.clientMoneyAudit,
      })
        .then(() => {
          if (isShowLoader) {
            useNotification({
              type: NotificationType.SUCCESS,
              content: "Saved successfully.",
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
        })
        .catch();
    },
    addCustomAccountTypeOption(value: string) {
      this.accountTypeOptions = [
        ...this.accountTypeOptions,
        {
          label: value,
          value,
        },
      ];
    },
    async isPraAuthorisedAsync(value: any, firmReferenceNo: string) {
      const isAuthorised =
        await this.fcaService.getFirmPraStatusAsync(firmReferenceNo);
      value.isPraAuthorized = isAuthorised;
    },
    async populateRelatedFieldsAsync(
      value: any,
      firmReferenceNo: string,
      firmName?: string,
    ) {
      try {
        const addressDetails =
          await this.fcaService.getFirmAddressesDetailsAsync(
            firmReferenceNo,
            "PPOB",
          );

        if (!addressDetails.length) {
          return;
        }

        // Address
        const address = this.helperService.formatFcaAddress(addressDetails[0]);
        value.tradingAddress = value.registeredAddress = address;

        // Contact Number
        const contact = new ContactNumber();
        contact.number = this.helperService.cleanContactNumber(
          addressDetails[0]["Phone Number"],
        );
        value.contactNumber = contact;

        // Website Address
        value.website = addressDetails[0]["Website Address"];

        await this.isPraAuthorisedAsync(value, firmReferenceNo);

        // Insurer
        if (firmName) {
          const insurer = await this.fetchInsurerRatingsAsync(firmName);
          if (insurer?.rating) {
            value.insurerRating = insurer?.rating;
          }
        }
      } catch {
        throw new Error("Something went wrong. Please try again later.");
      }
    },
  },
  created() {
    if (!this.providers.length) {
      this.addProvider(0);
    }
    this.setClientMoneyAudit();
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameCreated);
    this.eventBus.on(AppConstants.formFieldChangedEvent, () => {
      this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi =
        false;
    });
  },
  async mounted() {
    this.isInitializing = false;

    try {
      this.isLoading = true;
      const { accounts = {} } =
        await this.companiesHouseService.getCompanyProfileAsync(
          this.currentCompanyNumber,
        );

      const { made_up_to, period_start_on, period_end_on } =
        accounts.last_accounts ?? {};

      const currentFormat = "YYYY-MM-DD";

      if (made_up_to) {
        this.accountMadeUpTo = moment(made_up_to, currentFormat).unix();
      }

      if (period_start_on) {
        this.accountPeriodStartOn = moment(
          period_start_on,
          currentFormat,
        ).unix();
      }

      if (period_end_on) {
        this.accountPeriodEndOn = moment(period_end_on, currentFormat).unix();
      }
    } catch (error) {
      throw new Error(this.$t("common-general-error"));
    } finally {
      this.isLoading = false;
    }

    this.eventBus.emit(AppConstants.bottomBarEnableEvent, true);
    this.eventBusFormSaved.on(AppConstants.formSavedEvent, (isAutoNext) => {
      this.submit(isAutoNext);
    });
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameMounted);
    this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi = false;
  },
  unmounted() {
    this.eventBus.emit(AppConstants.bottomBarEnableEvent, false);
    this.eventBusFormSaved.off(AppConstants.formSavedEvent);
    this.isLoading = false;
  },
  setup() {
    const pageLifeCycleStore = usePageLifeCycleStore();
    const { changeLifeCycleName } = pageLifeCycleStore;

    const pageFieldsInvalidHandlerStore = usePageFieldsInvalidHandlerStore();
    const { processInvalidFields } = pageFieldsInvalidHandlerStore;

    return {
      changeLifeCycleName,
      processInvalidFields,
    };
  },
});
</script>

<template src="./client-money.html" />

<style scoped src="./client-money.scss" />