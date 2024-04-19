<script lang="ts">
import { defineComponent } from "vue";
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
import { AppConstants } from "@/infra/AppConstants";
import { OnboardingType } from "@/infra/base";
import {IProvidersService, IProvidersServiceInfo} from "@/infra/dependency-services/rest/providers/IProvidersService";
import {ProvidersEntity} from "@/entities/providers-and-introducers/ProvidersEntity";

export default defineComponent({
  name: "ProviderVerify",
  data() {
    return {
      authService: container.resolve<IAuth0Service>(IAuth0ServiceInfo.name),
      base64EncodingService: container.resolve<IEncodingService>(
          IEncodingServiceInfo.name,
      ),
      providerService:
          container.resolve<IProvidersService>(
              IProvidersServiceInfo.name,
          ),
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      navigationService: container.resolve<INavigationService>(
          INavigationServiceInfo.name,
      ),
      currentUser: undefined as ProvidersEntity | undefined,
    };
  },
  async created() {
    await this.authService.getTokenForSignupAndSaveLocallyAsync();
    const base64EncodedEmail = this.$route.params.email.toString();
    const email = this.base64EncodingService.decode(base64EncodedEmail);
    this.currentUser =
        await this.providerService.getProviderByEmailAsync(email);

    if (!this.currentUser) {
      throw new Error(
          `Customer Provider with email '${email}' does not exists.`,
      );
    }

    localStorage.setItem(
        AppConstants.onboardingTypeKey,
        OnboardingType.Provider,
    );

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

<style scoped></style>
