<script lang="ts">
import { defineAsyncComponent, defineComponent, inject } from "vue";
import { v4 as uuidv4 } from "uuid";
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
import { CustomerEntity } from "@/entities/CustomerEntity";
import {
  ICustomerService,
  ICustomerServiceInfo,
} from "@/infra/dependency-services/rest/forms-compliance/ICustomerService";
import { FirmProfileStatus } from "@/entities/enums/FirmProfileStatus";
import { KendoDialogComponent } from "@/components/KendoDialog.vue";
import {
  ICalendlyService,
  ICalendlyServiceInfo,
} from "@/infra/dependency-services/rest/calendly/ICalendlyService";
import KendoFlexibleDialog, {
  KendoFlexibleDialogComponent,
} from "@/components/KendoFlexibleDialog.vue";
import { Emitter, EventType } from "mitt";
import { AppointedRepresentative } from "@/entities/appointed-representatives/AppointedRepresentative";
import { ProfileModel } from "@/pages/sign-up-pages/signup/models/ProfileModel";
import { SignUpEventTypeModel } from "@/pages/sign-up-pages/signup/models/SignUpEventTypeModel";
import { APP_VERSION } from "@/config";

export default defineComponent({
  name: "Signup",
  components: {
    KendoFlexibleDialog,
    TopBar: defineAsyncComponent(
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
      customerService: container.resolve<ICustomerService>(
        ICustomerServiceInfo.name,
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
    };
  },
  computed: {
    AppVersion() {
      return APP_VERSION;
    },
    AppConstants() {
      return AppConstants;
    },
    isSoleTrader(): boolean {
      return this.selectedSigningType !== "company";
    },
    isGenerateProposalButtonDisabled(): boolean {
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
  },
  unmounted() {
    this.eventBus.off(AppConstants.companyNotFoundEvent);
  },
  methods: {
    setSigningType(type: string) {
      this.selectedSigningType = type;

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

    convertStringToDate(inputDate: string) {
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

    async signupAsync() {
      this.isProcessingProposal = true;

      try {
        // check if company has existing signed proposal
        const hasSignedProposal =
          await this.customerService.checkIfCompanyHasExistingSignedProposal(
            this.profile.selectedCompany.companyNumber,
          );

        if (hasSignedProposal) {
          this.kendoDialogInstance?.setDialogMessage(
            this.$t("signUpPage-companyHasAlreadySignedAProposal"),
          );
          return;
        }

        let registeredCustomer =
          await this.customerService.getCustomerByEmailAsync(
            this.profile.email,
          );

        if (registeredCustomer && registeredCustomer.isFinishedSignUp) {
          this.kendoDialogInstance?.setDialogMessage(
            this.$t("signUpPage-emailAlreadyRegistered"),
          );
          return;
        }

        let customer = new CustomerEntity();

        if (this.isSoleTrader) {
          customer.soleTraderDetails = {
            homeAddress: this.profile.homeAddress,
            dateOfBirth: this.profile.dateOfBirth,
          };

          if (this.profile.selectedCompany) {
            this.profile.selectedCompany.address = this.profile.homeAddress;
          }
        }

        customer.id = uuidv4();
        customer.firstName = this.profile.foreName;
        customer.lastName = this.profile.lastName;
        customer.email = this.profile.email;
        customer.contactNumber = this.profile.contactNumber;
        customer.isFinishedSignUp = this.isFinishedSignup;
        customer.isCompanyNotApplicable = this.isSoleTrader;
        customer.dateCreated = this.helperService.getCurrentDateTimeInEpoch();
        customer.firmProfileEditStatus =
          FirmProfileStatus.Incomplete.toString();

        customer.firmRepresentativeDetail = {
          forename: customer.firstName,
          surname: customer.lastName,
          emailAddress: customer.email,
          contactNumber: customer.contactNumber,
          title: "",
          jobTitle: "",
        };
        customer.isFinishedSignUp = true;

        if (this.profile.selectedCompany) {
          customer.selectedCompany = this.profile.selectedCompany;
          customer.companyName = this.profile.selectedCompany.companyName;
          customer.companyNumber = this.profile.selectedCompany.companyNumber;
          customer.firmReferenceNumber =
            this.profile.selectedCompany.firmReferenceNo;
          customer.companyAddress = this.profile.selectedCompany.address;
          customer.isAuthorised = this.profile.selectedCompany.isAuthorized;
        }

        if (
          this.isFirmIsNotCompanyHouseRegistered &&
          this.companyNameToProceed &&
          !customer.companyNumber
        ) {
          this.profile.selectedCompany = {
            companyName: this.companyNameToProceed,
            address: "",
            tradingAddress: "",
            companyNumber: "Not Applicable",
            firmReferenceNo: "Not Applicable",
            isAuthorized: false,
            isConfirmedFirmDetails: false,
            isSelected: true,
            isVariedFirmPermissions: false,
            postcode: "Not Available",
            status: "Not Authorised",
            companyHouseStatus: "",
            type: "Firm",
            region: "",
            isSoleTrader: false,
            appointedRepresentatives: [] as AppointedRepresentative[],
            contactNumber: "",
            website: "",
            sicCode: "",
            countryCode: "",
          };
        }

        await this.customerService
          .saveCustomerAsync(JSON.stringify(customer))
          .finally(() => (this.isProposalReady = true));
        localStorage.removeItem(AppConstants.signingTypeKey);
        localStorage.removeItem(AppConstants.profileKey);
      } finally {
        this.isProcessingProposal = false;
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
      localStorage.removeItem(AppConstants.signingTypeKey);
      this.selectedSigningType = "";
    },
  },
});
</script>

<template src="./signup.html" />

<style scoped src="./signup.css" />
