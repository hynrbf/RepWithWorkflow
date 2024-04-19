<script lang="ts">
import { defineComponent } from "vue";
import { AppConstants } from "@/infra/AppConstants";
import { container } from "tsyringe";
import {
  IAuth0Service,
  IAuth0ServiceInfo,
} from "@/infra/dependency-services/rest/auth0/IAuth0Service";
import {
  IEncodingService,
  IEncodingServiceInfo,
} from "@/infra/dependency-services/encoding/IEncodingService";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import {
  INavigationService,
  INavigationServiceInfo,
} from "@/infra/dependency-services/navigation/INavigationService";
import {
  IAppointedRepresentativeService,
  IAppointedRepresentativeServiceInfo,
} from "@/infra/dependency-services/rest/appointed-representative/IAppointedRepresentativeService";
import { OnboardingType } from "@/infra/base";
import { AppointedRepresentative } from "@/entities/appointed-representatives/AppointedRepresentative";

export default defineComponent({
  name: "ar-verify",
  data() {
    return {
      authService: container.resolve<IAuth0Service>(IAuth0ServiceInfo.name),
      base64EncodingService: container.resolve<IEncodingService>(
        IEncodingServiceInfo.name,
      ),
      customerAppointedRepresentativeService:
        container.resolve<IAppointedRepresentativeService>(
          IAppointedRepresentativeServiceInfo.name,
        ),
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      navigationService: container.resolve<INavigationService>(
        INavigationServiceInfo.name,
      ),
      currentUser: undefined as AppointedRepresentative | undefined,
    };
  },
  async created() {
    await this.authService.getTokenForSignupAndSaveLocallyAsync();
    const base64EncodedEmail = this.$route.params.email.toString();
    const email = this.base64EncodingService.decode(base64EncodedEmail);
    this.currentUser =
      await this.customerAppointedRepresentativeService.getAppointedRepresentativesByEmailAsync(
        email,
      );

    if (!this.currentUser) {
      throw new Error(
        `Customer Appointed Representative with email '${email}' does not exists.`,
      );
    }

    localStorage.setItem(AppConstants.onboardingTypeKey, OnboardingType.Ar);

    if (!this.currentUser.isUserPasswordSet) {
      const queryParam = this.base64EncodingService.encode(`email=${email}`);
      const changePasswordUrl = `${AppConstants.changePasswordRoute}?${queryParam}`;
      await this.navigationService.navigateRootAsync(changePasswordUrl);
    } else {
      await this.navigationService.navigateRootAsync(
        AppConstants.forceLoginRoute,
      );
    }
  },
});
</script>

<template />

<style scoped />