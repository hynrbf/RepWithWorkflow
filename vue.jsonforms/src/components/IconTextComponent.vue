<script setup lang="ts">
import { toRefs, computed } from "vue";

const props = withDefaults(
  defineProps<{
    icon?: string;
    text?: string;
    gap?: number;
    rtl?: boolean;
  }>(),
  {
    text: "",
    gap: 2,
    rtl: false,
  },
);
const { icon, text, gap, rtl } = toRefs(props);
const classes = computed(() => ["hstack", gap.value && `gap-${gap.value}`]);
</script>

<template>
  <div :class="classes">
    <IconComponent v-if="!rtl" :symbol="icon" />
    <div>
      <slot name="text">{{ text }}</slot>
    </div>
    <IconComponent v-if="rtl" :symbol="icon" />
  </div>
</template>