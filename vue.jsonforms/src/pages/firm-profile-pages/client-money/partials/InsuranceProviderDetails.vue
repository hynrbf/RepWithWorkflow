<script setup lang="ts">
import { toRefs, computed } from "vue";
import { InsuranceProvider } from "@/entities/client-money/InsuranceProvider";

import InsurerRating from "./InsurerRating.vue";
import {AppConstants} from "@/infra/AppConstants";

const props = withDefaults(defineProps<{ modelValue: InsuranceProvider }>(), {
  modelValue: () => <InsuranceProvider>{},
});
const emits = defineEmits<{
  (event: "update:modelValue", value: InsuranceProvider): void;
}>();
const { modelValue } = toRefs(props);

const provider = computed({
  get() {
    return modelValue.value;
  },
  set(value) {
    emits("update:modelValue", value);
  },
});

const id = `insurance-provider-${Math.random().toString(36).slice(2, 7)}`;

// //ToDo. incorrect types
// const populateRelatedFieldsAsync = inject<any>("populateRelatedFieldsAsync");
//
// const KendoFirmFinderComponentAsync = defineAsyncComponent(
//   () => import("@/components/form-fields/KendoFirmFinderComponent.vue"),
// );

const setUniqueIdentifier = (value: string) : string => {
  const identifier = `${AppConstants.clientMoneyRoute}-insuranceProvider${value}`;
  return identifier.replace(/\s+/g, "").replace("/", "");
};

</script>

<template>
  <div class="row gy-3">
    <div class="col-lg-10">
      <!-- KendoFirmFinderComponentAsync
        :id="setUniqueIdentifier(`-companyInfo-${ID}`)"
        :company="{}"
        companyNameLabel="Provider Name"
        :companyName="provider.providerName"
        :companyNumber="provider.companyNumber"
        :firmReferenceNumber="provider.firmReferenceNumber"
        @onCompanyDetailUpdated="
          (value: any) => {
            provider.providerName = value.firmName;
            provider.companyNumber = value.companyNumber;
            provider.firmReferenceNumber = value.firmReferenceNumber;
            populateRelatedFieldsAsync(provider, value.firmReferenceNumber);
          }
        "
        :isInitializing="false"
      /-->
    </div>
    <div class="col-lg-2">
      <KendoSwitchToggleComponent
        v-model="provider.isPraAuthorized"
        :id="setUniqueIdentifier(`-praAuthorized-${id}`)"
        :name="`praAuthorized-${id}`"
        label="PRA Authorized"
        no-text
      />
    </div>
    <div class="col-lg-12">
      <InsurerRating
        v-model="provider.insurerRating"
        :id="setUniqueIdentifier(`-insurerRating-${id}`)"
        :name="`InsurerRating-${id}`"
        label="Insurer Rating"
        :max-width="480"
        :unrated="!provider.insurerRating"
      />
    </div>
    <div class="col-lg-12">
      <KendoPostCodeInputComponent
        v-model="provider.registeredAddress"
        :id="setUniqueIdentifier(`-registeredAddress-${id}`)"
        :name="`registeredAddress-${id}`"
        label="Registered Address"
      />
    </div>
    <div class="col-lg-12">
      <KendoPostCodeInputComponent
        v-model="provider.tradingAddress"
        :id="setUniqueIdentifier(`-tradingAddress-${id}`)"
        :name="`tradingAddress-${id}`"
        label="Trading Address"
      />
    </div>
    <div class="col-lg-4">
      <KendoEmailAddressInputComponent
        v-model="provider.emailAddress"
        :id="setUniqueIdentifier(`-emailAddress-${id}`)"
        name="emailAddress"
        label="Email Address"
      />
    </div>
    <div class="col-lg-4">
      <KendoTelInputComponent
        v-model="provider.contactNumber"
        :id="setUniqueIdentifier(`-contactNumber-${id}`)"
        name="contactNumber"
        label="Contact Number"
        :isDataLoadedCompletely="true"
      />
    </div>
    <div class="col-lg-4">
      <KendoGenericInputComponent
        v-model="provider.website"
        :id="setUniqueIdentifier(`-website-${id}`)"
        name="website"
        type="url"
        label="Website"
      />
    </div>
  </div>
</template>
