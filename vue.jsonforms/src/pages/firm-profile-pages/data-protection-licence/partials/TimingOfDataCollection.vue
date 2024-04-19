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
    id: "timingOfDataCollectionService",
    title: "Select Sample Text for Timing of Data Collection",
    initialItems: [],
  },
  {
    id: "timingOfDataCollectionServiceConfirmed",
    title: "Confirmed Text for Timing Data Collection",
    initialItems: [],
    isFeatured: true,
    noAdd: true,
    emptyText: "Confirmed Text",
    keepEditting: true,
  },
]);

const nonServiceCollectionBlocks = computed(() => [
  {
    id: "timingOfDataCollectionNonService",
    title: "Select Sample Text for Timing of Data Collection",
    initialItems: [],
  },
  {
    id: "timingOfDataCollectionNonServiceConfirmed",
    title: "Confirmed Text for Timing Data Collection",
    initialItems: [],
    isFeatured: true,
    noAdd: true,
    emptyText: "Confirmed Text",
    keepEditting: true,
  },
]);

const setUniqueIdentifier = (value: string): string => {
  const identifier = `${AppConstants.dataProtectionRoute}-timingOfDataCollection${value}`;
  return identifier.replace(/\s+/g, "").replace("/", "");
};
</script>

<template>
  <PanelComponent title="Service Dependent Data Collection">
    <template #description>
      Please specify at which point(s) {{ currentFirmName }} collects Personal
      Data which is required for the provision of Services.
    </template>
    <ContentListBoxComponent
      v-model="values"
      :blocks="serviceCollectionBlocks"
      add-title="Add New Text"
    >
      <template #create-form="{ getValue, setValue }">
        <KendoGenericInputComponent
          placeholder="Type"
          :id="setUniqueIdentifier('-service-timing-create')"
          :model-value="getValue('text', '')"
          @update:model-value="setValue('text', $event)"
        />
      </template>
      <template #item-edit-footer="{ getValue, setValue }">
        <KendoGenericInputComponent
          placeholder="Type"
          :id="setUniqueIdentifier('-service-timing-edit')"
          :model-value="getValue('text', '')"
          @update:model-value="setValue('text', $event)"
        />
      </template>
      <template #item-content="{ props }">
        {{ props.dataItem.text || "" }}
      </template>
    </ContentListBoxComponent>
  </PanelComponent>
  <PanelComponent title="Non-Service Dependent Data Collection">
    <template #description>
      Please specify at which point(s) {{ currentFirmName }} collects Personal
      Data which is required for the provision of Services.
    </template>
    <ContentListBoxComponent
      v-model="values"
      :blocks="nonServiceCollectionBlocks"
      add-title="Add New Text"
    >
      <template #create-form="{ getValue, setValue }">
        <KendoGenericInputComponent
          placeholder="Type"
          :id="setUniqueIdentifier('-nonService-timing-create')"
          :model-value="getValue('text', '')"
          @update:model-value="setValue('text', $event)"
        />
      </template>
      <template #item-edit-footer="{ getValue, setValue }">
        <KendoGenericInputComponent
          placeholder="Type"
          :id="setUniqueIdentifier('-nonService-timing-edit')"
          :model-value="getValue('text', '')"
          @update:model-value="setValue('text', $event)"
        />
      </template>
      <template #item-content="{ props }">
        {{ props.dataItem.text || "" }}
      </template>
    </ContentListBoxComponent>
  </PanelComponent>
</template>
