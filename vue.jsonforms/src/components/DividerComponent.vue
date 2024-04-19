<script setup lang="ts">
import { computed } from "vue";

const props = withDefaults(
  defineProps<{
    width?: number;
    height?: number;
    spacing?: number;
  }>(),
  {
    width: 1,
    height: 12,
    spacing: 10,
  },
);

const UNIT = "px";
const width = computed(() => `${props.width}${UNIT}`);
const height = computed(() => `${props.height}${UNIT}`);
const spacing = computed(() => `${props.spacing}${UNIT}`);
</script>

<template>
  <div class="Divider"></div>
</template>

<style scoped lang="scss">
.Divider {
  display: inline-block;
  vertical-align: middle;

  &:before {
    content: "";
    display: block;
    background-color: var(--text-text-disabled);
    width: v-bind(width);
    height: v-bind(height);
    margin: 0 v-bind(spacing);
  }
}
</style>