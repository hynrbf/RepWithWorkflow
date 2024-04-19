<script setup lang="ts">
import { toRefs, computed } from "vue";

const props = withDefaults(
  defineProps<{
    icon?: string;
    text?: string;
    gap?: number;
    rtl?: boolean;
    dynamicUrl?: string;
  }>(),
  {
    text: "",
    gap: 2,
    rtl: false,
  },
);
const { text, gap, dynamicUrl } = toRefs(props);
const classes = computed(() => ["hstack", gap.value && `gap-${gap.value}`]);
</script>

<template>
  <div :class="classes">
    <img :src="dynamicUrl" />
    <div>
      <slot name="text">{{ text }}</slot>
    </div>
  </div>
</template>