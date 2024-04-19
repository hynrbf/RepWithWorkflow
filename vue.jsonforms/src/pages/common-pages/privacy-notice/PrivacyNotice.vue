<script lang="ts">
import { defineComponent } from "vue";
import { container } from "tsyringe";
import {
  INavigationService,
  INavigationServiceInfo,
} from "@/infra/dependency-services/navigation/INavigationService";

export default defineComponent({
  name: "PrivacyNotice",
  data() {
    return {
      navigationService: container.resolve<INavigationService>(
        INavigationServiceInfo.name,
      ),
    };
  },

  methods: {
    async showPrivacyNotice() {
      await this.navigationService.navigateAsync("/privacy-notice");
    },

    async goBack() {
      await this.navigationService.navigateAsync("/");
    },
  },
});
</script>

<template src="./privacy_notice.html" />

<style src="./privacy_notice.css" scoped />