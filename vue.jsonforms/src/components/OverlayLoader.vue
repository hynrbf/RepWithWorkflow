<script setup lang="ts">
defineProps<{ loading: boolean }>();
</script>

<template>
  <div :class="['OverlayLoader', loading && 'OverlayLoader--loading']">
    <slot></slot>

    <div v-if="loading" class="OverlayLoader-loader">
      <KendoLoader />
    </div>
  </div>
</template>

<style scoped lang="scss">
.OverlayLoader {
  position: relative;
  z-index: 1;

  &:before {
    content: "";
    display: none;
    width: 100%;
    height: 100%;
    position: absolute;
    top: 0;
    left: 0;
    z-index: 4;
    background-color: var(--color-white);
    opacity: 0.8;
  }

  &-loader {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    z-index: 5;
  }

  &--loading {
    &:before {
      display: block;
    }
  }
}
</style>