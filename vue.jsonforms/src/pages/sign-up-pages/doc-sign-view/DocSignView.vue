<script lang="ts">
import { defineComponent } from "vue";
import { container } from "tsyringe";
import {
  ICustomerService,
  ICustomerServiceInfo,
} from "@/infra/dependency-services/rest/forms-compliance/ICustomerService";
import { AppConstants } from "@/infra/AppConstants";
import {
  IEncodingService,
  IEncodingServiceInfo,
} from "@/infra/dependency-services/encoding/IEncodingService";

export default defineComponent({
  name: "DocSignView",
  data() {
    return {
      customerService: container.resolve<ICustomerService>(
        ICustomerServiceInfo.name,
      ),
      base64EncodingService: container.resolve<IEncodingService>(
        IEncodingServiceInfo.name,
      ),
      proposalUrl: "",
    };
  },
  async created() {
    let savedToken = localStorage.getItem(AppConstants.authTokenCacheKey);

    if (!savedToken) {
      await this.$auth0.loginWithRedirect();
      return;
    }

    let currentEmail = this.extractEmailFromEncodedUrl();

    if (!currentEmail) {
      return;
    }

    let customer =
      await this.customerService.getCustomerByEmailAsync(currentEmail);
    this.proposalUrl = customer.embeddedSigning?.link as string;
  },
  methods: {
    extractEmailFromEncodedUrl(): string {
      let indexOfQuestionMark = this.$route.fullPath.indexOf("?");

      if (indexOfQuestionMark < 1) {
        return "";
      }

      let queryPart = this.$route.fullPath.substring(indexOfQuestionMark + 1);
      let queryString = this.base64EncodingService.decode(queryPart);
      return queryString.replace("email=", "");
    },
  },
});
</script>

<template>
  <div class="box">
    <iframe :src="proposalUrl" width="100%" height="100%" />
  </div>
</template>

<style src="./doc_sign_view.css" scoped />