<script lang="ts">
import { defineAsyncComponent, defineComponent, inject } from "vue";
import { container } from "tsyringe";
import { AppConstants } from "@/infra/AppConstants";
import {
  IAuth0Service,
  IAuth0ServiceInfo,
} from "@/infra/dependency-services/rest/auth0/IAuth0Service";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import {
  IAppService,
  IAppServiceInfo,
} from "@/infra/dependency-services/app/IAppService";
import { CompanyEntity } from "@/entities/CompanyEntity";
import { KendoDialogComponent } from "@/components/KendoDialog.vue";
import {
  ICalendlyService,
  ICalendlyServiceInfo,
} from "@/infra/dependency-services/rest/calendly/ICalendlyService";
import KendoFlexibleDialog, {
  KendoFlexibleDialogComponent,
} from "@/components/KendoFlexibleDialog.vue";
import { Emitter, EventType } from "mitt";
import { ProfileModel } from "@/pages/sign-up-pages/signup/models/ProfileModel";
import { SignUpEventTypeModel } from "@/pages/sign-up-pages/signup/models/SignUpEventTypeModel";
import {
  IAppointedRepresentativeService,
  IAppointedRepresentativeServiceInfo,
} from "@/infra/dependency-services/rest/appointed-representative/IAppointedRepresentativeService";
import { Money } from "@/entities/Money";
import { AppointedRepresentative } from "@/entities/appointed-representatives/AppointedRepresentative";
import { AppointedRepresentativeActivity } from "@/entities/appointed-representatives/AppointedRepresentativeActivity";
import {ProfileStatuses} from "@/entities/enums/ProfileStatuses";

export default defineComponent({
  name: "Signup",
  components: {
    KendoFlexibleDialog,
    TopBarAsync: defineAsyncComponent(
      () => import("../../common-pages/portal/partials/TopBar.vue"),
    ),
  },
  data() {
    return {
      isProcessingProposal: false,
      authService: container.resolve<IAuth0Service>(IAuth0ServiceInfo.name),
      appService: container.resolve<IAppService>(IAppServiceInfo.name),
      calendlyService: container.resolve<ICalendlyService>(
        ICalendlyServiceInfo.name,
      ),
      customerARService: container.resolve<IAppointedRepresentativeService>(
        IAppointedRepresentativeServiceInfo.name,
      ),
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      kendoDialogInstance: null as KendoDialogComponent | null,
      kendoCalendarFlexibleDialogInstance:
        null as KendoFlexibleDialogComponent | null,
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, { msg: string; company: string }>
      >,
      selectedSigningType: "",
      companyNameToProceed: "",
      isFinishedSignup: false,
      isProposalReady: false,
      activeEventTypes: [] as SignUpEventTypeModel[],
      selectedEventType: null as SignUpEventTypeModel | null,
      profile: new ProfileModel(),
      isFirmIsNotCompanyHouseRegistered: false,
      complianceFirmName: "",
      privacyURL: this.$t("privacy-url"),
      privacyWebsite: this.$t("privacy-website"),
      isPrivacyDialogOpen: false,
      stepIndex: 0,
      steps: [
        AppConstants.signInStep1,
        AppConstants.signInStep2,
        AppConstants.signInStep3,
        AppConstants.signInStep4,
      ],
      activityToggles: [
        {
          value: "Mortgage Broking",
          toggled: false,
          items: [
            {
              value: "Owner Occupier and Consumer Buy To Let",
              toggled: false,
            },
            {
              value: "Mortgage Equity Release",
              toggled: false,
            },
            {
              value: "Professional Buy To Let",
              toggled: false,
            },
            {
              value: "Commercial",
              toggled: false,
            },
          ],
          others: {
            value: "",
            toggled: false,
          },
        },
        {
          value: "Protection Broking",
          toggled: false,
          items: [
            {
              value: "Life and Critical Illness Cover",
              toggled: false,
            },
            {
              value: "Income Protection",
              toggled: false,
            },
            {
              value: "Personal Medical Insurance",
              toggled: false,
            },
            {
              value: "Group Cover",
              toggled: false,
            },
          ],
          others: {
            value: "",
            toggled: false,
          },
        },
        {
          value: "General Insurance Broking",
          toggled: false,
          items: [
            {
              value: "Buildings and Contents",
              toggled: false,
            },
            {
              value: "Liability Insurance",
              toggled: false,
            },
            {
              value: "Motor Insurance",
              toggled: false,
            },
            {
              value: "Pet Insurance",
              toggled: false,
            },
          ],
          others: {
            value: "",
            toggled: false,
          },
        },
        {
          value: "Consumer Credit",
          toggled: false,
          items: [
            {
              value: "Credit Broking",
              toggled: false,
            },
            {
              value: "Debt Counselling",
              toggled: false,
            },
            {
              value: "Debt Adjusting",
              toggled: false,
            },
            {
              value: "Debt Collection",
              toggled: false,
            },
          ],
          others: {
            value: "",
            toggled: false,
          },
        },
        {
          value: "Investments",
          toggled: false,
          items: [
            {
              value: "Advising on Investments",
              toggled: false,
              subItems: [
                {
                  value: "Readily Releasable Securities",
                  toggled: false,
                },
              ],
              subOthers: {
                value: "",
                toggled: false,
              },
            },
            {
              value: "Arranging (bringing about) deals in Investments",
              toggled: false,
              subItems: [
                {
                  value: "Readily Releasable Securities",
                  toggled: false,
                },
              ],
              subOthers: {
                value: "",
                toggled: false,
              },
            },
          ],
          others: {
            value: "",
            toggled: false,
          },
        },
        {
          value: "Financial Promotions",
          toggled: false,
          items: [
            {
              value: "Consumer Credit",
              toggled: false,
            },
            {
              value: "Consumer Hire",
              toggled: false,
            },
            {
              value: "Claims Management Activity",
              toggled: false,
            },
            {
              value: "Deposits",
              toggled: false,
            },
            {
              value: "Insurance",
              toggled: false,
            },
            {
              value: "Mortgages",
              toggled: false,
            },
            {
              value: "Pensions",
              toggled: false,
            },
            {
              value: "Listed Shares",
              toggled: false,
            },
            {
              value: "Certificates representing Certain Securities",
              toggled: false,
            },
            {
              value: "Government and Public Security",
              toggled: false,
            },
            {
              value: "Rights to or Interests in Investments",
              toggled: false,
            },
            {
              value: "Units",
              toggled: false,
            },
            {
              value: "Options",
              toggled: false,
            },
            {
              value: "Futures",
              toggled: false,
            },
            {
              value: "Cryptoassets",
              toggled: false,
            },
          ],
          others: {
            value: "",
            toggled: false,
          },
        },
        {
          value: "Other",
          toggled: false,
          items: [],
          others: {
            value: "",
            toggled: false,
          },
        },
      ],
      appointedRepresentativeActivities:
        [] as AppointedRepresentativeActivity[],
      isAdvisorDriven: undefined as boolean | undefined,
      hasAdditionalServices: undefined as boolean | undefined,
      forecastedAnnualRevenueActivities: undefined as Money | undefined,
      forecastedAnnualRevenueServices: undefined as Money | undefined,
      preferredCommencementDate: undefined as Date | undefined,
      calendlyUrl: undefined as string | undefined,
    };
  },
  computed: {
    Money() {
      return Money;
    },
    AppConstants() {
      return AppConstants;
    },
    isSoleTrader(): boolean {
      return this.selectedSigningType !== "company";
    },
    isNextActivityButtonDisabled(): boolean {
      let hasMissingBasicDetails =
        this.profile.foreName?.length === 0 ||
        this.profile.lastName?.length === 0 ||
        this.profile.email?.length === 0 ||
        !this.profile.contactNumber.number ||
        this.profile.contactNumber.number?.length === 0;

      if (this.isSoleTrader) {
        return (
          hasMissingBasicDetails ||
          this.profile.homeAddress?.length === 0 ||
          this.profile.dateOfBirth?.length === 0
        );
      }

      return hasMissingBasicDetails || !this.profile.selectedCompany;
    },
    layoutAlignment(): string {
      if (this.stepIndex === 0 || this.isProposalReady) {
        return "centered-container center-padding";
      } else {
        return "form-padding";
      }
    },
    eighteenYearsAgo(): Date {
      return this.helperService.getDateForGivenYearsAgo(18);
    },
  },
  async created() {
    this.eventBus.on(
      AppConstants.companyNotFoundEvent,
      (data: { msg: string; company: string }) => {
        this.companyNameToProceed = data.company;
        this.isFirmIsNotCompanyHouseRegistered = true;
      },
    );

    await this.authenticateForSignupOnlyAsync();
    const signingType = localStorage.getItem(AppConstants.signingTypeKey);

    if (signingType) {
      this.selectedSigningType = signingType;
    }

    const profile = localStorage.getItem(AppConstants.profileKey);

    if (!profile) {
      return;
    }

    this.profile = JSON.parse(profile) as ProfileModel;
  },
  async mounted() {
    if (this.$refs.kendoDialog) {
      this.kendoDialogInstance = this.$refs.kendoDialog as KendoDialogComponent;
    }

    if (this.$refs.kendoCalendarFlexibleDialog) {
      this.kendoCalendarFlexibleDialogInstance = this.$refs
        .kendoCalendarFlexibleDialog as KendoFlexibleDialogComponent;
    }

    this.complianceFirmName =
      await this.appService.getComplianceFirmNameAsync();
    this.calendlyUrl = await this.getCalendlyUrlAsync();
  },
  unmounted() {
    this.eventBus.off(AppConstants.companyNotFoundEvent);
  },
  methods: {
    setSigningType(type: string) {
      this.selectedSigningType = type;
      this.stepIndex = 0;
      if (!type) {
        return;
      }

      localStorage.setItem(AppConstants.signingTypeKey, type);
    },

    async authenticateForSignupOnlyAsync(): Promise<boolean> {
      let authToken = localStorage.getItem(AppConstants.authTokenCacheKey);

      if (!authToken) {
        return await this.authService.getTokenForSignupAndSaveLocallyAsync();
      }

      let authUser = this.appService.getAuthUserFromLocal();

      if (!authUser) {
        throw new Error("auth user should have been saved already!");
      }

      let currentTime = this.helperService.getCurrentDateTimeInEpoch();

      if (currentTime >= authUser.tokenExpiry) {
        return await this.authService.getTokenForSignupAndSaveLocallyAsync();
      }

      return true;
    },

    onDateOfBirthChange(date: Date) {
      this.profile.dateOfBirth = date.toDateString();
    },

    convertStringToDate(inputDate: string): Date | undefined {
      if (!inputDate) {
        return new Date();
      }

      let epoch = this.helperService.dateStringToEpoch(inputDate);
      return this.helperService.convertEpochToDateTime(epoch);
    },

    onFullNameValueChanged(names: string[]) {
      this.profile.foreName = names[0];
      this.profile.lastName = names[1];
    },

    onSearchSoleTraderResult(selectedCompany: CompanyEntity) {
      this.profile.selectedCompany = selectedCompany;
    },

    onSelectEventType(event: any) {
      this.selectedEventType = event.target.value;
    },

    onNavigateToScheduling() {
      if (!this.selectedEventType) {
        return;
      }

      window.open(this.selectedEventType.scheduling_url);
      this.kendoCalendarFlexibleDialogInstance?.closeActionDialog();
      this.activeEventTypes = [];
    },

    async navigateToScheduleMeetingPage() {
      localStorage.setItem(AppConstants.emailKey, this.profile.email);
      let eventTypes = await this.calendlyService.getSchedulingLinkAsync();

      if (!eventTypes.length) {
        this.kendoDialogInstance?.setDialogMessage(
          "There are no event types found.",
          "No event types found",
        );
        return;
      }

      let activeEventTypes = eventTypes.filter((e) => e.active);

      if (!activeEventTypes) {
        this.kendoDialogInstance?.setDialogMessage(
          "There is no active event.",
          "No event found",
        );
        return;
      }

      if (activeEventTypes.length > 1) {
        this.activeEventTypes = activeEventTypes;
        this.kendoCalendarFlexibleDialogInstance?.showMessageAndContent(
          "",
          "Select Event Type",
        );
        return;
      }

      let firstEventType = activeEventTypes[0];
      window.open(firstEventType.scheduling_url);
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

    onProjectedAnnualFeeIncomeChange(
      value: Money,
      item: {
        value: string;
        toggled: boolean;
        fee: Money;
        commission: Money;
        years: number;
      },
      category: string,
    ) {
      item.fee = value;
      this.addOrUpdateActivities(`${category} > ${item.value}`, item);
    },

    onProjectedAnnualCommissionIncomeChange(
      value: Money,
      item: {
        value: string;
        toggled: boolean;
        fee: Money;
        commission: Money;
        years: number;
      },
      category: string,
    ) {
      item.commission = value;
      this.addOrUpdateActivities(`${category} > ${item.value}`, item);
    },

    onYearsOfExperienceChange(
      value: number,
      item: {
        value: string;
        toggled: boolean;
        fee: Money;
        commission: Money;
        years: number;
      },
      category: string,
    ) {
      item.years = value;
      this.addOrUpdateActivities(`${category} > ${item.value}`, item);
    },

    onSubItemToggled(
      e: { value: boolean },
      item: {
        value: string;
        toggled: boolean;
      },
      categoryName: string,
    ) {
      item.toggled = e?.value ?? false;
      const activityName = `${categoryName} > ${item.value}`;

      if (!item.toggled) {
        this.removeActivity(activityName);
        return;
      }

      this.addOrUpdateActivities(activityName, {
        value: item.value,
        toggled: item.toggled,
      });
    },

    addOrUpdateActivities(
      activityName: string,
      item: {
        value: string;
        toggled: boolean;
        fee?: Money;
        commission?: Money;
        years?: number;
      },
    ) {
      if (this.appointedRepresentativeActivities) {
        const foundActivity = this.appointedRepresentativeActivities.find(
          (a) => a.name === activityName,
        );

        if (foundActivity) {
          foundActivity.annualFeeIncome = item.fee;
          foundActivity.annualCommissionIncome = item.commission;
          foundActivity.projectedAnnualFee = item.fee;
          foundActivity.projectedAnnualCommissionIncome = item.commission;
          foundActivity.yearsOfExperience = item.years;
        } else {
          this.appointedRepresentativeActivities.push({
            name: activityName,
            annualFeeIncome: item.fee?.amount,
            annualCommissionIncome: item.commission?.amount,
            projectedAnnualFee: item.fee,
            projectedAnnualCommissionIncome: item.commission,
            yearsOfExperience: item.years,
          } as AppointedRepresentativeActivity);
        }

        return;
      }

      this.appointedRepresentativeActivities = [
        {
          name: activityName,
          annualFeeIncome: item.fee?.amount,
          projectedAnnualFee: item.fee,
          annualCommissionIncome: item.commission?.amount,
          projectedAnnualCommissionIncome: item.commission,
          yearsOfExperience: item.years,
        } as AppointedRepresentativeActivity,
      ];
    },

    onMainActivityToggle(
      e: { value: boolean },
      item: {
        value?: string;
        toggled?: boolean;
        fee?: Money;
        commission?: Money;
        years?: number;
      },
      categoryName: string,
    ) {
      item.toggled = e?.value ?? false;

      if (!item.toggled) {
        this.removeActivity(`${categoryName} > ${item.value}`);
      }
    },

    removeActivity(activityName: string) {
      if (!this.appointedRepresentativeActivities?.length) {
        return;
      }

      // Remove
      const foundActivity = this.appointedRepresentativeActivities?.find(
        (a) => a.name === activityName,
      );

      if (foundActivity) {
        const indexToRemove =
          this.appointedRepresentativeActivities?.indexOf(foundActivity);
        this.appointedRepresentativeActivities?.splice(indexToRemove, 1);
      }
    },

    async signupAsync() {
      this.isProcessingProposal = true;
      try {
        let customerAR = new AppointedRepresentative();
        customerAR.firstName = this.profile.foreName;
        customerAR.lastName = this.profile.lastName;
        customerAR.email = this.profile.email;
        customerAR.contactNumber = this.profile.contactNumber;
        customerAR.profileStatus = ProfileStatuses.Full.toString();
        customerAR.isCompanyNotApplicable = this.isSoleTrader;

        if (!this.isSoleTrader) {
          customerAR.selectedCompany = this.profile.selectedCompany;
          customerAR.companyName = this.profile.selectedCompany?.companyName;
          customerAR.companyNumber =
            this.profile.selectedCompany?.companyNumber;
          customerAR.firmReferenceNumber =
            this.profile.selectedCompany?.firmReferenceNo;
        }

        customerAR.dateOfBirth = this.helperService.dateStringToEpoch(
          this.profile.dateOfBirth,
        );
        customerAR.companyAddress = this.profile.homeAddress;

        if (this.appointedRepresentativeActivities?.length) {
          customerAR.activities = this.appointedRepresentativeActivities;
        }

        customerAR.isFinishedSignUp = true;
        customerAR.isActivityAdvisorDriven = this.isAdvisorDriven;
        customerAR.hasAdditionalServices = this.hasAdditionalServices;
        customerAR.forecastedAnnualRevenueActivities =
          this.forecastedAnnualRevenueActivities;
        customerAR.forecastedAnnualRevenueServices =
          this.forecastedAnnualRevenueServices;

        if (this.preferredCommencementDate) {
          customerAR.preferredCommencementDate =
            this.helperService.dateStringToEpoch(
              this.preferredCommencementDate.toDateString(),
            );
        }

        let finalCustomerAR = await this.customerARService
          .saveOrUpdateAppointedRepresentativeAsync(customerAR)
          .finally(() => {
            this.isProposalReady = true;
          });

        localStorage.setItem("customer-ar", JSON.stringify(finalCustomerAR));
        localStorage.removeItem(AppConstants.signingTypeKey);
        localStorage.removeItem(AppConstants.profileKey);
      } finally {
        this.isProcessingProposal = false;
        this.stepIndex = 0;
      }
    },

    showPrivacyNotice() {
      this.isPrivacyDialogOpen = true;
    },

    closePrivacyDialog() {
      this.isPrivacyDialogOpen = false;
    },

    onCompanyInput(value: string) {
      this.profile.selectedCompany.companyName = value;
    },

    onCompanyValueChanged(company: CompanyEntity) {
      this.isFirmIsNotCompanyHouseRegistered = false;
      this.profile.selectedCompany = company;
    },

    onClearCompany() {
      this.profile.initializeSelectedCompany();
    },

    onBack() {
      if (this.stepIndex === 0) {
        localStorage.removeItem(AppConstants.signingTypeKey);
        this.selectedSigningType = "";
      } else {
        this.stepIndex -= 1;
      }
    },

    onNextStep() {
      this.stepIndex += 1;
    },

    setIdentifier(value: string): string {
      const tempIdentifier = AppConstants.signUpArRoute + "-" + value;
      return tempIdentifier.replace(/\s+/g, "").replace("/", "");
    },

    async getCalendlyUrlAsync(): Promise<string> {
      const eventTypes = await this.calendlyService.getSchedulingLinkAsync();

      if (!eventTypes.length) {
        this.kendoDialogInstance?.setDialogMessage(
          "There are no event types found.",
          "No event types found",
        );
        return "";
      }

      let activeEventTypes = eventTypes.filter((e) => e.active);

      if (!activeEventTypes) {
        this.kendoDialogInstance?.setDialogMessage(
          "There is no active event.",
          "No event found",
        );
        return "";
      }

      let firstEventType = activeEventTypes[0];
      return firstEventType.scheduling_url;
    },
  },
});
</script>

<template src="./signup-ar.html" />

<style scoped src="./signup-ar.css" />