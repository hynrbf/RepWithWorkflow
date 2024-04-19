<script setup lang="ts">
import { computed, toRefs, watchEffect } from "vue";

const props = defineProps<{ expanded?: boolean; title?: string }>();
const emits = defineEmits<{
  (event: "update:expanded", value?: boolean): void;
  (event: "expanded"): void;
  (event: "collapsed"): void;
}>();
const { expanded } = toRefs(props);
const isExpanded = computed({
  get() {
    return expanded?.value;
  },
  set(value) {
    emits("update:expanded", value);
  },
});

const getClass = (slot: string) => {
  const definedClass = ["p-4"];

  if (slot === "content") {
    definedClass.push("flex-grow-1");
  }

  return definedClass;
};

watchEffect(() => {
  if (expanded.value) {
    emits("expanded");
  } else {
    emits("collapsed");
  }
});

defineExpose({
  open() {
    isExpanded.value = true;
  },
  close() {
    isExpanded.value = false;
  },
});
</script>

<template>
  <KendoDrawer
    :expanded="isExpanded"
    :items="[{ slot: 'header' }, { slot: 'content' }, { slot: 'footer' }]"
    item="item"
    position="end"
    :width="600"
  >
    <template #item="{ props }">
      <li
        :class="[
          ...getClass(props.slot),
          props.slot === 'content' && 'is-scrollable-y',
        ]"
      >
        <slot :name="props.slot" :close="() => (isExpanded = false)">
          <div
            v-if="props.slot === 'header'"
            class="k-hstack justify-content-between align-items-center"
          >
            <h4 class="text-primary mb-0">{{ title }}</h4>
            <KendoButton fill-mode="flat" @click.prevent="isExpanded = false">
              <img src="/cross.svg" width="15" height="15" alt="close" />
            </KendoButton>
          </div>
        </slot>
      </li>
    </template>
  </KendoDrawer>
</template>

<style scoped lang="scss">
:deep(.k-drawer .k-drawer-wrapper),
:deep(.k-drawer .k-drawer-items) {
  height: 100%;
}
</style>