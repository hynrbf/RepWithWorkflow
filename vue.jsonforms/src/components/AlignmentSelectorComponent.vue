<script setup lang="ts">
defineProps<{ modelValue: string }>();
const emits = defineEmits<{
  (event: "update:modelValue", value: string): void;
}>();

const items = [
  {
    title: "Align Left",
    value: "left",
    icon: "align-left-3",
  },
  {
    title: "Align Right",
    value: "center",
    icon: "align-center-85",
  },
  {
    title: "Align Center",
    value: "right",
    icon: "align-right-23",
  },
  {
    title: "Align Justify",
    value: "justify",
    icon: "justified-alignment-5",
  },
];
</script>

<template>
  <div class="Selector">
    <KendoButton
      v-for="item in items"
      :key="`align-${item.value}`"
      type="button"
      shape="square"
      rounded="small"
      fill-mode="flat"
      size="small"
      :selected="modelValue === item.value"
      :title="item.title"
      :aria-label="item.title"
      @click="emits('update:modelValue', item.value)"
    >
      <IconComponent :symbol="item.icon" size="20" />
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