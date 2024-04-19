<script setup lang="ts">
defineProps<{ bold: boolean; italic: boolean; underline: boolean }>();
const emits = defineEmits<{
  (event: "update:bold", value: boolean): void;
  (event: "update:italic", value: boolean): void;
  (event: "update:underline", value: boolean): void;
}>();
</script>

<template>
  <div class="Selector">
    <KendoButton
      type="button"
      shape="square"
      rounded="small"
      fill-mode="flat"
      size="small"
      :selected="bold"
      @click="emits('update:bold', !bold)"
    >
      <IconComponent symbol="bold-text-31" size="20" />
    </KendoButton>
    <KendoButton
      type="button"
      shape="square"
      rounded="small"
      fill-mode="flat"
      size="small"
      :selected="italic"
      @click="emits('update:italic', !italic)"
    >
      <IconComponent symbol="italic-text-32" size="20" />
    </KendoButton>
    <KendoButton
      type="button"
      shape="square"
      rounded="small"
      fill-mode="flat"
      size="small"
      :selected="underline"
      @click="emits('update:underline', !underline)"
    >
      <IconComponent symbol="underline-text-1-32" size="20" />
    </KendoButton>
  </div>
</template>

<style scoped lang="scss">
// ToDo. Move to global scss
.Selector {
  display: flex;
  align-items: center;
  gap: 5px;

  :deep(.k-button) {
    width: 35px;
    height: 35px;
    border-radius: 8px;
    color: #e4e7ec;

    &.k-selected,
    &:hover,
    &.k-hover {
      &:before {
        opacity: 1;
      }
    }

    svg {
      color: black;
      display: block;
    }
  }

  :deep(.k-button-text) {
    z-index: 1;
  }
}
</style>