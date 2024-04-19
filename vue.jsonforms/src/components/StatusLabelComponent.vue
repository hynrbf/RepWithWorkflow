<script setup lang="ts">
import { toRefs, computed } from "vue";

const props = withDefaults(
  defineProps<{
    status: string | number;
    text?: string;
    icon?: string;
    iconSize?: number | string;
    noIcon?: boolean;
  }>(),
  {
    iconSize: "12",
  },
);
const { status } = toRefs(props);
const properties = computed(() => {
  switch (status.value) {
    case "approved":
    case 0:
      return {
        text: "Approved",
        themeColor: "success-tint",
        icon: "check-circle-flat-corner",
      };
    case "rejected":
    case 1:
      return {
        text: "Rejected",
        themeColor: "error-tint",
        icon: "cross-circle-flat-corner",
      };
    case "pending":
    case 2:
      return {
        text: "Pending",
        themeColor: "warning-tint",
        icon: "clock-five-flat",
      };
    default:
      return { text: "", themeColor: "" };
  }
});
</script>

<template>
  <PillComponent :theme-color="properties.themeColor">
    <IconComponent
      v-if="!noIcon || !properties.icon"
      :symbol="icon || properties.icon"
      :size="iconSize"
    />
    <slot>{{ text || properties.text }}</slot>
  </PillComponent>
</template>