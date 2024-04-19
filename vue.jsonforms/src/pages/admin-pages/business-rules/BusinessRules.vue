<script lang="ts">
import { container } from "tsyringe";
import {defineComponent} from "vue";
import { INavigationService, INavigationServiceInfo } from "@/infra/dependency-services/navigation/INavigationService";

export default defineComponent({
  name: "BusinessRules",
  data () {
    return {
      navigationService: container.resolve<INavigationService>(INavigationServiceInfo.name)
    }
  },
  methods: {
    async showEditPermission() {
      await this.navigationService.navigateAsync("/admin/business-rules/edit-permission");
    },
    async showEditStatus() {      
      await this.navigationService.navigateAsync("/admin/business-rules/edit-status");
    }
  }
})

</script>

<template src="./business_rules.html"/>

<style src="./business_rules.css" scoped/>