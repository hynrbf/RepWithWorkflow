<script lang="ts">
import { defineComponent, inject } from "vue";
import StaticList from "@/infra/StaticListService";
import { Emitter, EventType } from "mitt";
import BusinessIncorporation from "@/pages/models/owners-and-controllers/BusinessIncorporation";
import { AppConstants } from "@/infra/AppConstants";

export default defineComponent({
  name: "KendoBusinessIncorporationComponent",
  props: {
    id: String,
    value: {
      type: Object as () => BusinessIncorporation,
      default: new BusinessIncorporation(),
    },
    isInitializing: {
      type: Boolean,
      default: true,
    },
  },
  data() {
    return {
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      natureOfBusiness: [] as string[],
      valueInternal: new BusinessIncorporation(),
      isInitializingInternal: false,
      isUk: false,
    };
  },
  created() {
    if (!this.id) {
      throw new Error("id is required in KendoBusinessIncorporationComponent");
    }

    this.natureOfBusiness = StaticList.getNatureOfBusinesses();
    this.setBinding();
  },
  updated() {
    this.setBinding();
  },
  methods: {
    setBinding() {
      //what is saved in db is only number 59140
      //so here we map number, and it's description 59140 Motion picture projection activities to display to user
      const businessNatureItem = this.natureOfBusiness.find((item) => {
        const matchedNumber = item.match(/\d+/);
        return matchedNumber && matchedNumber[0] === this.value.businessNature;
      });

      this.valueInternal = businessNatureItem
        ? { ...this.value, businessNature: businessNatureItem }
        : this.value;

      if (!this.valueInternal.country) {
        this.valueInternal.country =
          AppConstants.DefaultCountryCode.toUpperCase();
      }

      this.isInitializingInternal = this.isInitializing;
      this.isUk = this.valueInternal.country === "GB";
      this.onBusinessNatureChange(this.valueInternal.businessNature);
    },

    onCountryChange(country: string) {
      if (this.valueInternal.country === country) {
        return;
      }

      this.valueInternal.country = country;
      this.valueInternal.businessNature = "";
      this.onValueChange();
    },

    onBusinessNatureChange(businessNature: string) {
      if (this.valueInternal.businessNature === businessNature) {
        return;
      }

      this.valueInternal.businessNature = businessNature;
      this.onValueChange();
    },

    onValueChange() {
      //we need to put to consumer or the pages to save only the number part, excluding the description
      const modifiedItem = { ...this.valueInternal };
      modifiedItem.businessNature = modifiedItem.businessNature.replace(
        /\D/g,
        "",
      );

      this.$emit("onBusinessIncorporationChange", modifiedItem);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    },
  },
});
</script>

<template>
  <StackLayout
    orientation="horizontal"
    :align="{ horizontal: 'start' }"
    style="gap: 15px"
  >
    <KendoCountryComponent
      :id="`${id}-countryOfIncorporation`"
      :name="`${id}-countryOfIncorporation`"
      class="col"
      label="Country of Incorporation"
      :value="valueInternal?.country"
      @onValueChange="onCountryChange"
    />

    <div style="margin-left: -15px" />

    <KendoAutoCompleteInputComponent
      style="margin-right: -5px"
      v-if="isUk"
      :id="`${id}-natureOfBusiness`"
      :name="`${id}-natureOfBusiness`"
      class="col-8"
      label="Nature of Business (SIC)"
      hasDropdown
      :itemsSource="natureOfBusiness"
      :value="valueInternal?.businessNature"
      :isDataLoadedCompletely="!isInitializingInternal"
      @onValueChange="onBusinessNatureChange"
    />

    <KendoGenericInputComponent
      v-else
      :id="`${id}-natureOfBusiness`"
      :name="`${id}-natureOfBusiness`"
      class="col-8"
      label="Nature of Business"
      :value="valueInternal?.businessNature"
      :isDataLoadedCompletely="!isInitializingInternal"
      @onValueChange="onBusinessNatureChange"
    />
  </StackLayout>
</template>

<style scoped />
