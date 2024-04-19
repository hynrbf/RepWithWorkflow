<script setup lang="ts">
import { computed, toRefs } from "vue";
import { useCustomerStore } from "@/stores/useCustomerStore";
import { storeToRefs } from "pinia";
import { DataProtectionLicenceEntity } from "@/entities/data-protection-license/DataProtectionLicenceEntity";
import {AppConstants} from "@/infra/AppConstants";

const props = withDefaults(
  defineProps<{ modelValue: DataProtectionLicenceEntity }>(),
  {
    modelValue: () => ({}) as DataProtectionLicenceEntity,
  }
);

const emits = defineEmits<{
  (event: "update:modelValue", value: DataProtectionLicenceEntity): void;
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

const customerStore = useCustomerStore();
const { currentFirmName } = storeToRefs(customerStore);

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
  const identifier = `${AppConstants.dataProtectionRoute}-methodsOfDataCollection${value}`;
  return identifier.replace(/\s+/g, "").replace("/", "");
};
</script>

<template>
  <PanelComponent title="Service Dependent Data Collection">
    <template #description>
      Please specify how {{ currentFirmName }} collects Personal Data which is
      required for the provision of Services.
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
          :id="setUniqueIdentifier('-service-method-details-create')"
          :model-value="getValue('text', '')"
          @update:model-value="setValue('text', $event)"
        />
      </template>
      <template #item-edit-footer="{ getValue, setValue }">
        <KendoGenericInputComponent
          class="mb-2"
          :id="setUniqueIdentifier('-service-method-title-edit')"
          placeholder="Method Type Title"
          :model-value="getValue('title', '')"
          @update:model-value="setValue('title', $event)"
        />
        <KendoGenericInputComponent
          placeholder="Method Type Details"
          :id="setUniqueIdentifier('-service-method-details-edit')"
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
      Please specify how {{ currentFirmName }} collects Personal Data which is
      required for the provision of Services.
    </template>
    <ContentListBoxComponent
      v-model="values"
      :blocks="nonServiceCollectionBlocks"
      add-title="Add New Method Type"
    >
      <template #create-form="{ getValue, setValue }">
        <KendoGenericInputComponent
          class="mb-2"
          :id="setUniqueIdentifier('-nonService-method-title-create')"
          placeholder="Method Type Title"
          :model-value="getValue('title', '')"
          @update:model-value="setValue('title', $event)"
        />
        <KendoGenericInputComponent
          placeholder="Method Type Details"
          :id="setUniqueIdentifier('-nonService-method-details-create')"
          :model-value="getValue('text', '')"
          @update:model-value="setValue('text', $event)"
        />
      </template>
      <template #item-edit-footer="{ getValue, setValue }">
        <KendoGenericInputComponent
          class="mb-2"
          :id="setUniqueIdentifier('-nonService-method-title-edit')"
          placeholder="Method Type Title"
          :model-value="getValue('title', '')"
          @update:model-value="setValue('title', $event)"
        />
        <KendoGenericInputComponent
          placeholder="Method Type Details"
          :id="setUniqueIdentifier('-nonService-method-details-edit')"
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
