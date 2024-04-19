<script setup lang="ts">
import { computed, toRefs } from "vue";
import { useArCustomerStore } from "@/stores/useArCustomerStore";
import { storeToRefs } from "pinia";
import { ArDataProtectionLicenceEntity } from "@/entities/data-protection-license/ARDataProtectionLicenceEntity";
import {AppConstants} from "@/infra/AppConstants";

const props = withDefaults(
  defineProps<{ modelValue: ArDataProtectionLicenceEntity }>(),
  {
    modelValue: () => ({}) as ArDataProtectionLicenceEntity,
  }
);

const emits = defineEmits<{
  (event: "update:modelValue", value: ArDataProtectionLicenceEntity): void;
}>();

const { modelValue } = toRefs(props);

const values = computed({
  get() {
    return modelValue.value;
  },
  set(value) {
    emits("update:modelValue", value);
  },
});

const customerArStore = useArCustomerStore();
const { currentArFirmName } = storeToRefs(customerArStore);

const serviceCollectionBlocks = computed(() => [
  {
    id: "methodsOfDataCollectionService",
    title: "Select Sample Type for Methods of Data Collection",
    initialItems: [],
  },
  {
    id: "methodsOfDataCollectionServiceConfirmed",
    title: "Confirmed Method Types",
    initialItems: [],
    isFeatured: true,
    noAdd: true,
    emptyText: "Confirmed Method Types",
    keepEditting: true,
  },
]);

const nonServiceCollectionBlocks = computed(() => [
  {
    id: "methodsOfDataCollectionNonService",
    title: "Select Sample Type for Methods of Data Collection",
    initialItems: [],
  },
  {
    id: "methodsOfDataCollectionNonServiceConfirmed",
    title: "Confirmed Method Types",
    initialItems: [],
    isFeatured: true,
    noAdd: true,
    emptyText: "Confirmed Method Types",
    keepEditting: true,
  },
]);

const setUniqueIdentifier = (value: string): string => {
  const identifier = `${AppConstants.arDataProtectionRoute}-methodsOfDataProtection${value}`;
  return identifier.replace(/\s+/g, "").replace("/", "");
};
</script>

<template>
  <PanelComponent title="Service Dependent Data Collection">
    <template #description>
      <div class="alert alert-primary border-0">
        {{ $t('dataProtectionLicencePage-sectionServiceDependentDescription', { currentFirmName: currentArFirmName }) }} <span class="text-decoration-underline">required</span>
        {{ $t('dataProtectionLicencePage-sectionForTheProvision', { currentFirmName: currentArFirmName }) }}
      </div>
    </template>
    <ContentListBoxComponent
      v-model="values"
      :blocks="serviceCollectionBlocks"
      add-title="Add New Method Type"
    >
      <template #create-form="{ getValue, setValue }">
        <KendoGenericInputComponent
          class="mb-2"
          :id="setUniqueIdentifier('-service-method-title-create')"
          placeholder="Method Type Title"
          :model-value="getValue('title', '')"
          @update:model-value="setValue('title', $event)"
        />

        <KendoGenericInputComponent
          placeholder="Method Type Details"
          :id="setUniqueIdentifier('service-method-details-create')"
          :model-value="getValue('text', '')"
          @update:model-value="setValue('text', $event)"
        />
      </template>

      <template #item-edit-footer="{ getValue, setValue }">
        <KendoGenericInputComponent
          class="mb-2"
          :id="setUniqueIdentifier('service-method-title-edit')"
          placeholder="Method Type Title"
          :model-value="getValue('title', '')"
          @update:model-value="setValue('title', $event)"
        />

        <KendoGenericInputComponent
          placeholder="Method Type Details"
          :id="setUniqueIdentifier('service-method-details-edit')"
          :model-value="getValue('text', '')"
          @update:model-value="setValue('text', $event)"
        />
      </template>

      <template #item-content="{ props }">
        <span class="font-weight-semi-bold">
          {{ props.dataItem?.title || "" }}
        </span>
      </template>
      <template #item-footer="{ props }">
        {{ props.dataItem.text || "" }}
      </template>
    </ContentListBoxComponent>
  </PanelComponent>

  <PanelComponent title="Non-Service Dependent Data Collection">
    <template #description>
      <div class="alert alert-primary border-0">
        {{ $t('dataProtectionLicencePage-sectionServiceDependentDescription', { currentFirmName: currentArFirmName }) }} <span class="text-decoration-underline">not required</span>
        {{ $t('dataProtectionLicencePage-sectionForTheProvision', { currentFirmName: currentArFirmName }) }}
      </div>
    </template>
    <ContentListBoxComponent
      v-model="values"
      :blocks="nonServiceCollectionBlocks"
      add-title="Add New Method Type"
    >
      <template #create-form="{ getValue, setValue }">
        <KendoGenericInputComponent
          class="mb-2"
          :id="setUniqueIdentifier('nonService-method-title-create')"
          placeholder="Method Type Title"
          :model-value="getValue('title', '')"
          @update:model-value="setValue('title', $event)"
        />

        <KendoGenericInputComponent
          placeholder="Method Type Details"
          :id="setUniqueIdentifier('nonService-method-details-create')"
          :model-value="getValue('text', '')"
          @update:model-value="setValue('text', $event)"
        />
      </template>

      <template #item-edit-footer="{ getValue, setValue }">
        <KendoGenericInputComponent
          class="mb-2"
          :id="setUniqueIdentifier('nonService-method-title-edit')"
          placeholder="Method Type Title"
          :model-value="getValue('title', '')"
          @update:model-value="setValue('title', $event)"
        />

        <KendoGenericInputComponent
          placeholder="Method Type Details"
          :id="setUniqueIdentifier('nonService-method-details-edit')"
          :model-value="getValue('text', '')"
          @update:model-value="setValue('text', $event)"
        />
      </template>

      <template #item-content="{ props }">
        <span class="font-weight-semi-bold">
          {{ props.dataItem?.title || "" }}
        </span>
      </template>

      <template #item-footer="{ props }">
        {{ props.dataItem.text || "" }}
      </template>
    </ContentListBoxComponent>
  </PanelComponent>
</template>
