<script lang="ts">
import { defineComponent } from "vue";
import { container } from "tsyringe";
import {
  IAppService,
  IAppServiceInfo,
} from "@/infra/dependency-services/app/IAppService";
import {
  INavigationService,
  INavigationServiceInfo,
} from "@/infra/dependency-services/navigation/INavigationService";
import { AppConstants } from "@/infra/AppConstants";
import { OnboardingType } from "@/infra/base";

export default defineComponent({
  name: "ForceLogin",
  data() {
    return {
      appService: container.resolve<IAppService>(IAppServiceInfo.name),
      navigationService: container.resolve<INavigationService>(
        INavigationServiceInfo.name,
      ),
      onboardingType: OnboardingType.Firm.toString(),
    };
  },
  async created() {
    const cachedOnboardingType = localStorage.getItem(
      AppConstants.onboardingTypeKey,
    );

    if (cachedOnboardingType) {
      this.onboardingType = cachedOnboardingType;
    }

    this.appService.clearAllLocalCacheWithToken();
    const redirectUrl =
      this.onboardingType === OnboardingType.Firm.toString()
        ? "/"
        : `/?onboarding_type=${this.onboardingType}`;
    await this.navigationService.navigateAsync(redirectUrl);
  },
});
</script>

<template></template>

<style scoped></style>
