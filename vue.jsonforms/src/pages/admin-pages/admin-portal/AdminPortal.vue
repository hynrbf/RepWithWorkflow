<script lang="ts">
import { container } from "tsyringe";
import { defineComponent } from "vue";
import {
  INavigationService,
  INavigationServiceInfo,
} from "@/infra/dependency-services/navigation/INavigationService";

export default defineComponent({
  name: "AdminPortal",
  data() {
    return {
      navigationService: container.resolve<INavigationService>(
        INavigationServiceInfo.name,
      ),
    };
  },
  methods: {
    async showBusinessRules() {
      await this.navigationService.navigateAsync("/admin/business-rules");
    },

    async showAdminDocuments() {
      await this.navigationService.navigateAsync("/admin/documents");
    },

    async showAdminForms() {
      await this.navigationService.navigateAsync("/admin/forms");
    },

    async showDocumentTemplating() {
      await this.navigationService.navigateAsync("/admin/document-templates");
    },

    async showSettings() {
      await this.navigationService.navigateAsync("/admin/settings");
    },
  },
});
</script>

<template src="./admin_portal.html" />

<style src="./admin_portal.css" scoped />
