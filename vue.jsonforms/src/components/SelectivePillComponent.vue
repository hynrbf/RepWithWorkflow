<script setup lang="ts">
import { toRefs, computed } from "vue";

const props = withDefaults(defineProps<{ modelValue: unknown[] }>(), {
  modelValue: () => [],
});

const emits = defineEmits<{
  (event: "update:modelValue", value: unknown[]): void;
}>();

const { modelValue } = toRefs(props);

const DEFAULTS = 4;

const pills = computed(() => [...modelValue.value].splice(0, DEFAULTS));

const counts = computed(() => {
  const numbers = modelValue.value.length - DEFAULTS;
  return numbers <= 0 ? 0 : numbers;
});

const removePill = (index: number) => {
  emits(
    "update:modelValue",
    modelValue.value.filter((_value, key) => key !== index),
  );
};
</script>

<template>
  <div class="SelectivePill">
    <PillComponent
      v-for="(pill, index) in pills"
      :key="`pill-${index}`"
      theme-color="white"
      size="lg"
      closeable
      class="mb-2"
      @close="removePill(index)"
    >
      <template #default>
        <slot name="pill-text" :item="pill">
          {{ pill }}
        </slot>
      </template>
    </PillComponent>
    <a href="javascript:;" v-if="counts" class="mb-2">
      <strong>+{{ counts }}</strong>
    </a>
  </div>
</template>

<style scoped lang="scss">
.SelectivePill {
  display: flex;
  align-items: center;
  flex-wrap: nowrap;
  gap: 5px;

  :deep(.Pill) {
    max-width: 21%;

    .Pill-text {
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    svg {
      display: block;
    }
  }
}
</style>