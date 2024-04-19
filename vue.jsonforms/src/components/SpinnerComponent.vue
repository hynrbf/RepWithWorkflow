<script setup lang="ts">
import { computed, toRefs } from "vue";

const props = withDefaults(defineProps<{ size: number }>(), { size: 40 });

const { size } = toRefs(props);

const sizeInPx = computed(() => `${size.value}px`);
const borderSizeInPx = computed(() => `${Math.abs(size.value / 10)}px`);
</script>

<template>
  <div class="Spinner">
    <div></div>
    <div></div>
    <div></div>
    <div></div>
  </div>
</template>

<style scoped lang="scss">
.Spinner {
  display: inline-block;
  position: relative;
  width: v-bind(sizeInPx);
  height: v-bind(sizeInPx);

  div {
    box-sizing: border-box;
    display: block;
    position: absolute;
    width: v-bind(sizeInPx);
    height: v-bind(sizeInPx);
    margin: 0;
    border: v-bind(borderSizeInPx) solid var(--color-primary);
    border-radius: 50%;
    animation: spinner 1.2s cubic-bezier(0.5, 0, 0.5, 1) infinite;
    border-color: var(--color-primary) transparent transparent transparent;

    &:nth-child(1) {
      animation-delay: -0.45s;
    }

    &:nth-child(2) {
      animation-delay: -0.3s;
    }

    &:nth-child(3) {
      animation-delay: -0.15s;
    }
  }
}

@keyframes spinner {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}
</style>