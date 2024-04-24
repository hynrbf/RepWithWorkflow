<script setup lang="ts">
import { toRefs, computed, ref } from "vue";
import { InsuranceProvider } from "@/entities/client-money/InsuranceProvider";
import InsuranceProviderDetails from "./InsuranceProviderDetails.vue";

const props = withDefaults(
  defineProps<{
    modelValue: InsuranceProvider[];
  }>(),
  {
    modelValue: () => [],
  }
);

const emits = defineEmits<{
  (event: "update:modelValue", values: InsuranceProvider[]): void;
}>();

const { modelValue } = toRefs(props);

const activeIndex = ref(0);

const tabs = computed({
  get() {
    return modelValue.value.map((item, key) => ({
      id: `insurance-provider-${
        item.providerName.toLowerCase().trim().replace(/ /g, "-") ||
        Math.random().toString(36).substring(2, 7)
      }`,
      title: item.providerName || "<Insurance Provider Name>",
      content: "",
      active: activeIndex.value === key,
    }));
  },
  set(tabs: any[]) {
    for (const index in tabs) {
      if (tabs[index].active) {
        activeIndex.value = +index;
        break;
      }
    }
  },
});

const addTab = (index: number) => {
  activeIndex.value = index;
  emits("update:modelValue", [...modelValue.value, new InsuranceProvider()]);
};

const removeTab = (index: number) => {
  if (activeIndex.value === index) {
    activeIndex.value = index + 1;
  }
  emits(
    "update:modelValue",
    modelValue.value.filter((_, i) => i !== index)
  );
};
</script>

<template>
  <ScrollableTabComponent
    v-model:items="tabs"
    :addable="true"
    :removable="true"
    @add="addTab"
    @remove="removeTab"
  >
    <template
      v-for="tab in tabs"
      :key="`tab-content-${tab.id}`"
      #[`content(${tab.id})`]="{ index }"
    >
      <CollapsiblePanelComponent
        :items="[
          { id: `insurance-provider-${tab.id}`, noHeader: true, active: true },
        ]"
      >
        <template #content>
          <InsuranceProviderDetails v-model="modelValue[index]" />
        </template>
      </CollapsiblePanelComponent>
    </template>
  </ScrollableTabComponent>
</template>
