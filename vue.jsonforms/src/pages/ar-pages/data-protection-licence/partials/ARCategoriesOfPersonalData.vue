<script setup lang="ts">
import { computed, toRefs } from "vue";
import { useArCustomerStore } from "@/stores/useArCustomerStore";
import { storeToRefs } from "pinia";
import { ArDataProtectionLicenceEntity } from "@/entities/data-protection-license/ARDataProtectionLicenceEntity";
import { AppConstants } from "@/infra/AppConstants";

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
    id: "categoriesOfPersonalDataService",
    title: "Select the Applicable Categories",
    initialItems: [],
  },
  {
    id: "categoriesOfPersonalDataServiceConfirmed",
    title: "Confirmed Categories of Personal Data",
    initialItems: [],
    isFeatured: true,
    noAdd: true,
    emptyText: "Confirmed Categories",
    keepEditting: true,
  },
]);

const nonServiceCollectionBlocks = computed(() => [
  {
    id: "categoriesOfPersonalDataNonService",
    title: "Select the Applicable Categories",
    initialItems: [],
  },
  {
    id: "categoriesOfPersonalDataNonServiceConfirmed",
    title: "Confirmed Categories of Personal Data",
    initialItems: [],
    isFeatured: true,
    noAdd: true,
    emptyText: "Confirmed Categories",
    keepEditting: true,
  },
]);

const setUniqueIdentifier = (value: string): string => {
  const identifier = `${AppConstants.arDataProtectionRoute}-categoriesOfPersonalData${value}`;
  return identifier.replace(/\s+/g, "").replace("/", "");
};
</script>

<template>
  <PanelComponent title="Service Dependent Data Collection">
    <template #description>
      <div class="alert alert-primary border-0">
        {{
          $t("dataProtectionLicencePage-sectionRequiredDescription", {
            currentFirmName: currentArFirmName,
          })
        }}
        <span class="text-decoration-underline">required</span>
        {{
          $t(
            "dataProtectionLicencePage-sectionRequiredContinuationDescription",
            { currentFirmName: currentArFirmName }
          )
        }}
      </div>
    </template>
    <ContentListBoxComponent
      v-model="values"
      :blocks="serviceCollectionBlocks"
      add-title="Add New Category"
    >
      <template #create-form="{ getValue, setValue }">
        <KendoGenericInputComponent
          class="mb-2"
          :id="setUniqueIdentifier('-service-category-title-create')"
          placeholder="Category Title"
          :model-value="getValue('title', '')"
          :isValueReactive="true"
          @update:model-value="setValue('title', $event)"
        />

        <SelectivePillComponent
          :model-value="getValue('details', [])"
          @update:model-value="setValue('details', $event)"
        />

        <InputWithAddComponent
          placeholder="Category Details"
          @add="
            ($event: string) =>
              setValue('details', [...getValue('details', []), $event])
          "
        />
      </template>

      <template #item-edit-footer="{ getValue, setValue }">
        <KendoGenericInputComponent
          class="mb-2"
          :id="setUniqueIdentifier('-service-category-title-edit')"
          placeholder="Category Title"
          :model-value="getValue('title', '')"
          :isValueReactive="true"
          @update:model-value="setValue('title', $event)"
        />

        <SelectivePillComponent
          :model-value="getValue('details', [])"
          @update:model-value="setValue('details', $event)"
        />

        <InputWithAddComponent
          placeholder="Category Details"
          @add="
            ($event: string) =>
              setValue('details', [...getValue('details', []), $event])
          "
        />
      </template>

      <template #item-content="{ props }">
        <span class="font-weight-semi-bold">
          {{ props.dataItem?.title || "" }}
        </span>
      </template>

      <template #item-footer="{ props }">
        <span>
          {{ (props.dataItem.details || []).join(", ").trim() }}
        </span>
      </template>
    </ContentListBoxComponent>
  </PanelComponent>

  <PanelComponent title="Non-Service Dependent Data Collection">
    <template #description>
      <div class="alert alert-primary border-0">
        {{
          $t("dataProtectionLicencePage-sectionRequiredDescription", {
            currentFirmName: currentArFirmName,
          })
        }}
        <span class="text-decoration-underline">not required</span>
        {{
          $t(
            "dataProtectionLicencePage-sectionRequiredContinuationDescription",
            { currentFirmName: currentArFirmName }
          )
        }}
      </div>
    </template>
    <ContentListBoxComponent
      v-model="values"
      :blocks="nonServiceCollectionBlocks"
      add-title="Add New Category"
    >
      <template #create-form="{ getValue, setValue }">
        <KendoGenericInputComponent
          class="mb-2"
          :id="setUniqueIdentifier('-nonService-category-title-create')"
          placeholder="Category Title"
          :model-value="getValue('title', '')"
          :isValueReactive="true"
          @update:model-value="setValue('title', $event)"
        />

        <SelectivePillComponent
          :model-value="getValue('details', [])"
          @update:model-value="setValue('details', $event)"
        />

        <InputWithAddComponent
          placeholder="Category Details"
          @add="
            ($event: string) =>
              setValue('details', [...getValue('details', []), $event])
          "
        />
      </template>

      <template #item-edit-footer="{ getValue, setValue }">
        <KendoGenericInputComponent
          class="mb-2"
          :id="setUniqueIdentifier('-nonService-category-title-edit')"
          placeholder="Category Title"
          :model-value="getValue('title', '')"
          :isValueReactive="true"
          @update:model-value="setValue('title', $event)"
        />

        <SelectivePillComponent
          :model-value="getValue('details', [])"
          @update:model-value="setValue('details', $event)"
        />

        <InputWithAddComponent
          placeholder="Category Details"
          @add="
            ($event: string) =>
              setValue('details', [...getValue('details', []), $event])
          "
        />
      </template>

      <template #item-content="{ props }">
        <span class="font-weight-semi-bold">
          {{ props.dataItem?.title || "" }}
        </span>
      </template>
      <template #item-footer="{ props }">
        <span>
          {{ (props.dataItem.details || []).join(", ").trim() }}
        </span>
      </template>
    </ContentListBoxComponent>
  </PanelComponent>
</template>
