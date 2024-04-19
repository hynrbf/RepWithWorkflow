<script setup lang="ts">
import { toRefs, computed } from "vue";

const props = withDefaults(
  defineProps<{
    themeColor?: string;
    text?: string;
    closeable?: boolean;
    size?: "sm" | "lg";
    textClass?: string;
  }>(),
  {},
);
const emits = defineEmits<{ (event: "close"): void }>();
const { themeColor, closeable, size } = toRefs(props);
const classes = computed(() => [
  "Pill",
  themeColor?.value && `Pill--${themeColor.value}`,
  closeable?.value && "pe-1",
  size?.value && `Pill--${size.value}`,
]);
</script>

<template>
  <div :class="classes">
    <span :class="['Pill-text', textClass && textClass]">
      <slot>
        {{ text }}
      </slot>
    </span>
    <span
      v-if="closeable"
      class="text-neutral is-clickable"
      @click.prevent="emits('close')"
    >
      <IconComponent symbol="cross-circle-flat-corner" size="16" />
    </span>
  </div>
</template>