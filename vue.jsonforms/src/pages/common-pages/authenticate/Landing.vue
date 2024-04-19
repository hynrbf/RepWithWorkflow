<script lang="ts">
import { defineComponent } from "vue";
import { container } from "tsyringe";
import { AppConstants } from "@/infra/AppConstants";
import { SECURE_APP_ENABLE } from "@/config";
import { AuthUser } from "@/entities/AuthUser";
import {
  ICustomerService,
  ICustomerServiceInfo,
} from "@/infra/dependency-services/rest/forms-compliance/ICustomerService";
import {
  IAuth0Service,
  IAuth0ServiceInfo,
} from "@/infra/dependency-services/rest/auth0/IAuth0Service";
import {
  IAppService,
  IAppServiceInfo,
} from "@/infra/dependency-services/app/IAppService";
import {
  ISettingService,
  ISettingServiceInfo,
} from "@/infra/dependency-services/rest/settings/ISettingService";
import {
  INavigationService,
  INavigationServiceInfo,
} from "@/infra/dependency-services/navigation/INavigationService";
import { OnboardingType } from "@/infra/base";
import {
  IFcaService,
  IFcaServiceInfo,
} from "@/infra/dependency-services/rest/fca/IFcaService";

export default defineComponent({
  name: "Landing",
  data() {
    return {
      authService: container.resolve<IAuth0Service>(IAuth0ServiceInfo.name),
      fcaService: container.resolve<IFcaService>(IFcaServiceInfo.name),
      customerService: container.resolve<ICustomerService>(
        ICustomerServiceInfo.name,
      ),
      settingService: container.resolve<ISettingService>(
        ISettingServiceInfo.name,
      ),
      savedToken: "" as string | null,
      appService: container.resolve<IAppService>(IAppServiceInfo.name),
      userEmail: "",
      isAuthenticated: false,
      onboardingType: OnboardingType.Firm.toString(),
    };
  },
  async created() {
    if (localStorage.getItem(AppConstants.onboardingTypeKey)) {
      this.onboardingType =
        localStorage.getItem(AppConstants.onboardingTypeKey) ??
        OnboardingType.Firm.toString();
    }

    if (this.$route.fullPath.includes("?onboarding_type")) {
      const onboardingType = this.extractOnBoardingType();
      localStorage.setItem(
        AppConstants.onboardingTypeKey,
        onboardingType ?? OnboardingType.Firm.toString(),
      );
      this.onboardingType = onboardingType;
    }

    const navigationService = container.resolve<INavigationService>(
      INavigationServiceInfo.name,
    );

    if (!SECURE_APP_ENABLE) {
      //when security not required for dev only, then go straight to default page
      await this.initializeRemoteDataAsync();
      await navigationService.navigateAsync("/signup");
      return;
    }

    this.savedToken = localStorage.getItem(AppConstants.authTokenCacheKey);

    if (!this.savedToken) {
      return;
    }

    switch (this.onboardingType) {
      case OnboardingType.Ar.toString(): {
        if (
          await this.appService.checkCustomerAppointedRepresentativeSignedUpAlreadyAsync()
        ) {
          await navigationService.navigateAsync("/portal");
          return;
        }

        break;
      }
      case OnboardingType.Employee.toString(): {
        if (await this.appService.checkEmployeeSignedUpAlreadyAsync()) {
          await navigationService.navigateAsync("/portal");
          return;
        }
        break;
      }
      case OnboardingType.Provider.toString(): {
        if (await this.appService.checkProviderSignedUpAlreadyAsync()) {
          await navigationService.navigateAsync("/portal");
          return;
        }
        break;
      }
      case OnboardingType.Introducer.toString(): {
        if (await this.appService.checkIntroducerSignedUpAlreadyAsync()) {
          await navigationService.navigateAsync("/portal");
          return;
        }
        break;
      }
      default: {
        if (await this.appService.checkCustomerSignedUpAlreadyAsync()) {
          await navigationService.navigateAsync("/portal");
          return;
        }
      }
    }

    await navigationService.navigateAsync("/signup");
  },
  async mounted() {
    if (!SECURE_APP_ENABLE) {
      //when security not required for dev only, then do not execute below
      return;
    }

    if (this.savedToken) {
      return;
    }

    // according to doc of Auth0 in quickstart, the 'const authToken = await this.$auth0.getAccessTokenSilently()'
    // should work, but it doesn't
    // const authToken = await this.$auth0.getAccessTokenSilently();
    await this.$auth0.getAccessTokenSilently().then(
      async (value) => {
        //do not process when either is null, cause of callback
        //the other callback could have the value already
        if (!this.$auth0.idTokenClaims.value || !this.$auth0.user.value) {
          return;
        }

        let idToken = this.$auth0.idTokenClaims.value.__raw;
        this.savedToken = await this.authService.AuthenticateAsync(
          value,
          idToken,
        );

        if (!this.savedToken) {
          alert(
            "Login is not success getting authentication token = id + access token.",
          );
          return;
        }

        let actualExpiry = this.$auth0.idTokenClaims.value.exp;

        if (!actualExpiry) {
          throw new Error(
            "There seems to be a problem. Token should have expiry.",
          );
        }

        let oneHourAdvanceExpiry = actualExpiry - 3600;
        localStorage.setItem(AppConstants.authTokenCacheKey, this.savedToken);
        this.isAuthenticated = this.$auth0.isAuthenticated.value;

        if (this.isAuthenticated) {
          let email = this.$auth0.user.value.email;

          if (email) {
            this.userEmail = email;
          }
        }

        let authUser: AuthUser = {
          token: this.savedToken,
          tokenExpiry: oneHourAdvanceExpiry,
          isAuthenticated: this.isAuthenticated,
          email: this.userEmail,
        };
        this.appService.saveAuthUserToLocal(authUser);
        await this.initializeRemoteDataAsync();
      },
      (reason) => {
        return reason;
      },
    );
    await this.$auth0.loginWithRedirect();
  },
  methods: {
    async initializeRemoteDataAsync() {
      await this.fcaService.initializeFcaDefinedPermissionsRemoteDataAsync();
      await this.fcaService.initializeFcaStatusesRemoteDataAsync();
      await this.settingService.initializeSettingsAsync();
    },

    extractOnBoardingType(): string {
      const indexOfQuestionMark = this.$route.fullPath.indexOf("?");

      if (indexOfQuestionMark < 1) {
        return OnboardingType.Firm.toString();
      }

      const queryPart = this.$route.fullPath.substring(indexOfQuestionMark + 1);
      return queryPart.replace("onboarding_type=", "");
    },
  },
});
</script>

<template>
  <div class="parent-container">
    <KendoLoadingComponent :isLoading="true" />
  </div>
</template>

<style scoped>
.parent-container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  width: 100vw;
  margin: 0;
}
</style>