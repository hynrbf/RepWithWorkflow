<script lang="ts">
import { defineComponent, defineAsyncComponent } from "vue";
import { container } from "tsyringe";
import {
  IEncodingService,
  IEncodingServiceInfo,
} from "@/infra/dependency-services/encoding/IEncodingService";
import {
  ICustomerService,
  ICustomerServiceInfo,
} from "@/infra/dependency-services/rest/forms-compliance/ICustomerService";
import {
  INavigationService,
  INavigationServiceInfo,
} from "@/infra/dependency-services/navigation/INavigationService";
import IconComponent from "@/components/IconComponent.vue";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import {
  IAuth0Service,
  IAuth0ServiceInfo,
} from "@/infra/dependency-services/rest/auth0/IAuth0Service";
import KendoFlexibleDialog, {
  KendoFlexibleDialogComponent,
} from "@/components/KendoFlexibleDialog.vue";
import { AppConstants } from "@/infra/AppConstants";
import { OnboardingType } from "@/infra/base";
import { OnBoardingUserBase } from "@/entities/OnBoardingUserBase";
import {
  IAppointedRepresentativeService,
  IAppointedRepresentativeServiceInfo,
} from "@/infra/dependency-services/rest/appointed-representative/IAppointedRepresentativeService";
import {
  IOrganizationalStructureService,
  IOrganizationalStructureServiceInfo,
} from "@/infra/dependency-services/rest/organizational-structure/IOrganizationalStructureService";
import {IProvidersServiceInfo, IProvidersService} from "@/infra/dependency-services/rest/providers/IProvidersService";
import {IIntroducersServiceInfo, IIntroducersService} from "@/infra/dependency-services/rest/introducers/IIntroducersService";

export default defineComponent({
  name: "ChangePassword",
  components: {
    IconComponent,
    KendoFlexibleDialog,
    TopBar: defineAsyncComponent(
      () => import("../../common-pages/portal/partials/TopBar.vue"),
    ),
  },
  data() {
    return {
      isLoading: true,
      authService: container.resolve<IAuth0Service>(IAuth0ServiceInfo.name),
      base64EncodingService: container.resolve<IEncodingService>(
        IEncodingServiceInfo.name,
      ),
      customerService: container.resolve<ICustomerService>(
        ICustomerServiceInfo.name,
      ),
      customerAppointedRepresentativeService:
        container.resolve<IAppointedRepresentativeService>(
          IAppointedRepresentativeServiceInfo.name,
        ),
      organisationalStructureService:
        container.resolve<IOrganizationalStructureService>(
          IOrganizationalStructureServiceInfo.name,
        ),
      providerService:
          container.resolve<IProvidersService>(
              IProvidersServiceInfo.name,
          ),
      introducerService:
          container.resolve<IIntroducersService>(
              IIntroducersServiceInfo.name,
          ),
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      navigationService: container.resolve<INavigationService>(
        INavigationServiceInfo.name,
      ),
      kendoCalendarFlexibleDialogInstance:
        null as KendoFlexibleDialogComponent | null,
      isPasswordCorrectAndValid: false,
      newPassword: "",
      confirmPassword: "",
      currentUser: undefined as OnBoardingUserBase | undefined,
      onboardingType: OnboardingType.Firm.toString(),
    };
  },
  computed: {
    isPasswordAtLeast8Chars(): boolean {
      if (!this.newPassword) {
        return false;
      }

      return this.newPassword.length >= 8;
    },

    isPasswordHaveAtLeastOneLetter(): boolean {
      if (!this.newPassword) {
        return false;
      }

      return /[a-zA-Z]/.test(this.newPassword);
    },

    isPasswordHaveAtLeastOneNumberOrSpecialCase() {
      if (!this.newPassword) {
        return false;
      }

      const hasNumber: boolean = /\d/.test(this.newPassword);
      const hasSpecialCharacter: boolean = /[!@#$%^&*(),.?":{}|<>_]/.test(
        this.newPassword,
      );

      return hasNumber || hasSpecialCharacter;
    },

    isEnableButton(): boolean {
      if (!(this.newPassword && this.confirmPassword)) {
        return false;
      }

      const currentPasswordIsStrong =
        this.isPasswordAtLeast8Chars &&
        this.isPasswordHaveAtLeastOneLetter &&
        this.isPasswordHaveAtLeastOneNumberOrSpecialCase;
      const passwordsMatched = this.newPassword === this.confirmPassword;
      return currentPasswordIsStrong && passwordsMatched;
    },
  },
  async created() {
    const email = this.extractEmailFromEncodedUrl();
    await this.setCurrentUserAsync(email);

    if (!this.currentUser || this.currentUser.isUserPasswordSet) {
      await this.navigationService.navigateRootAsync(
        AppConstants.forceLoginRoute,
      );
    }

    this.isLoading = false;
  },
  mounted() {
    if (this.$refs.kendoCalendarFlexibleDialog) {
      this.kendoCalendarFlexibleDialogInstance = this.$refs
        .kendoCalendarFlexibleDialog as KendoFlexibleDialogComponent;
    }
  },
  methods: {
    extractEmailFromEncodedUrl(): string {
      let indexOfQuestionMark = this.$route.fullPath.indexOf("?");

      if (indexOfQuestionMark < 1) {
        return "";
      }

      const queryPart = this.$route.fullPath.substring(indexOfQuestionMark + 1);
      const queryString = this.base64EncodingService.decode(queryPart);
      this.onboardingType =
        localStorage.getItem(AppConstants.onboardingTypeKey) ??
        OnboardingType.Firm.toString();
      return queryString.replace("email=", "");
    },

    async setCurrentUserAsync(email: string) {
      if (this.onboardingType === OnboardingType.Ar.toString()) {
        this.currentUser =
          await this.customerAppointedRepresentativeService.getAppointedRepresentativesByEmailAsync(
            email,
          );
      } else if (this.onboardingType === OnboardingType.Employee.toString()) {
        this.currentUser =
          await this.organisationalStructureService.getEmployeeByEmailAsync(email);
      } else if (this.onboardingType === OnboardingType.Provider.toString()) {
        this.currentUser =
            await this.providerService.getProviderByEmailAsync(email);
      } else if (this.onboardingType === OnboardingType.Introducer.toString()) {
        this.currentUser =
            await this.introducerService.getIntroducerByEmailAsync(email);
      } else {
        this.currentUser =
          await this.customerService.getCustomerByEmailAsync(email);
      }
    },

    async changePasswordAsync() {
      if (!(this.currentUser && this.currentUser.email)) {
        throw new Error(
          "ChangePassword.changePasswordAsync: User should not be null here.",
        );
      }

      await this.authService
        .changePasswordAsync(
          this.currentUser.email,
          this.newPassword,
          this.onboardingType,
        )
        .catch(() => {})
        .finally(async () => {
          this.kendoCalendarFlexibleDialogInstance?.showMessageAndContent(
            "",
            "",
          );
          await this.helperService.delayAsync(3000);
          this.kendoCalendarFlexibleDialogInstance?.closeActionDialog();
          await this.navigationService.navigateRootAsync(
            AppConstants.forceLoginRoute,
          );
        });
    },
  },
});
</script>

<template src="./change-password.html" />

<style scoped src="./change-password.css" />