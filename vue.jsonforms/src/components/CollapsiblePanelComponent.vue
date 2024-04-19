<script setup lang="ts">
import { ref, toRefs, provide } from "vue";
import { createEventHook } from "@vueuse/core";

export interface CollapsiblePanelItem {
  id: string;
  title: string;
  content: string;
  active: boolean;
  noHeader?: boolean;
  contentClass?: string;
}

const props = withDefaults(
  defineProps<{
    items: CollapsiblePanelItem[];
    accordion: boolean;
    hiddenItems?: string[];
    panelClass?: string;
  }>(),
  {
    items: () => [],
    accordion: true,
    hiddenItems: () => [],
    panelClass: "",
  }
);
const emits = defineEmits<{
  (event: "collapse", item?: CollapsiblePanelItem): void;
  (event: "collapsed", item?: CollapsiblePanelItem): void;
  (event: "expand", item?: CollapsiblePanelItem): void;
  (event: "expanded", item?: CollapsiblePanelItem): void;
}>();

const { items, accordion } = toRefs(props);

const mainElement = ref();
const duration = 200;

const collapsedHook = createEventHook<string>();
const expandedHook = createEventHook<string>();

const collapse = (id: string) => {
  const panelItem = items.value.find((item) => item.id === id);
  const item = mainElement.value.querySelector(`#panel-${id}`);
  const body = item?.querySelector(".CollapsiblePanel-body") as HTMLElement;

  if (
    !item ||
    !body ||
    !item.classList.contains("is-active") ||
    item.classList.contains("is-transitioning")
  ) {
    return;
  }

  emits("collapse", panelItem);
  item.classList.remove("is-active");
  item.classList.remove("is-expanded");
  item.classList.add("is-transitioning");
  animateBodyHeight(body, body.scrollHeight, 0, duration, () => {
    body.style.height = "";
    item.classList.remove("is-transitioning");
    emits("collapsed", panelItem);
    collapsedHook.trigger(id);
  });
};

const expand = (id: string) => {
  const panelItem = items.value.find((item) => item.id === id);
  const item = mainElement.value.querySelector(`#panel-${id}`);
  const body = item?.querySelector(".CollapsiblePanel-body") as HTMLElement;
  if (
    !item ||
    !body ||
    item.classList.contains("is-active") ||
    item.classList.contains("is-transitioning")
  ) {
    return;
  }

  if (accordion.value) {
    items.value.forEach(({ id }) => collapse(id));
  }

  emits("expand", panelItem);
  item.classList.add("is-active");
  item.classList.add("is-transitioning");
  animateBodyHeight(body, 0, body.scrollHeight, duration, () => {
    body.style.height = "";
    item.classList.remove("is-transitioning");
    item.classList.add("is-expanded");
    emits("expanded", panelItem);
    expandedHook.trigger(id);
  });
};

const animateBodyHeight = (
  body: HTMLElement,
  from: number,
  to: number,
  duration: number,
  callback: () => void
) => {
  const start = performance.now();
  let frameId = 0;

  body.style.height = `${from}px`;

  const step = (timeStamp: number) => {
    const elapsed = timeStamp - start;
    const progress = Math.min(elapsed / duration, 1);
    const value = from + progress * (to - from);
    body.style.height = `${value}px`;

    if (progress < 1) {
      frameId = requestAnimationFrame(step);
    } else {
      cancelAnimationFrame(frameId);
      callback();
    }
  };

  requestAnimationFrame(step);
};

const handleClick = (event: Event) => {
  const header = (event.target as Element)?.closest(".CollapsiblePanel-header");
  if (!header) {
    return;
  }

  const item = header.closest(".CollapsiblePanel-item");
  const id = item?.id?.replace("panel-", "");
  const isActive = item?.classList.contains("is-active");

  if (!id) {
    return;
  }

  if (isActive) {
    collapse(id);
  } else {
    expand(id);
  }
};

defineExpose({
  collapse,
  expand,
});

provide("collapsiblePanel:collapsed", collapsedHook.on);
provide("collapsiblePanel:expanded", expandedHook.on);
</script>

<template>
  <div ref="mainElement" class="CollapsiblePanel" @click="handleClick">
    <template v-for="(item, index) in items" :key="`panel-${item.id}`">
      <div
        v-if="!hiddenItems.includes(item.id)"
        :id="`panel-${item.id}`"
        :class="[
          'CollapsiblePanel-item',
          item.active && 'is-active is-expanded',
          panelClass,
        ]"
      >
        <div v-if="!item.noHeader" class="CollapsiblePanel-header">
          <slot name="before-title" :item="item" :index="index"></slot>

          <div class="CollapsiblePanel-title">
            <slot :name="`title-${item.id}`" :item="item" :index="index">
              <slot name="title" :item="item" :index="index"
                >{{ item.title }}
              </slot>
            </slot>
          </div>

          <slot name="after-title" :item="item" :index="index"></slot>

          <button class="CollapsiblePanel-close" type="button">
            <IconComponent symbol="arrow-up-3-10" size="17" />
          </button>
        </div>
        <div v-else class="CollapsiblePanel-headerPlaceholder" />

        <div class="CollapsiblePanel-body">
          <slot name="before-content" :item="item" :index="index"></slot>

          <div :class="['CollapsiblePanel-content', item.contentClass ?? '']">
            <slot :name="`content-${item.id}`" :item="item" :index="index">
              <slot name="content" :item="item" :index="index">
                {{ item.content }}
              </slot>
            </slot>
          </div>

          <slot name="after-content" :item="item" :index="index"></slot>
        </div>
      </div>
    </template>
  </div>
</template>

<style scoped lang="scss">
// ToDo. Move to global scss
.CollapsiblePanel {
  $root: &;

  &-item {
    border: 0.3px solid var(--color-neutral);
    background-color: var(--color-white);
    border-radius: 8px;
    margin-bottom: 20px;

    &.is-active {
      & > #{$root}-header {
        #{$root}-close {
          transform: rotate(0deg);
        }
      }
    }

    &.is-transitioning {
      & > #{$root}-body {
        display: block;
        overflow: hidden;
      }
    }

    &.is-expanded {
      & > #{$root}-body {
        display: block;
      }
    }
  }

  &-header {
    padding: 20px;
    display: flex;
    gap: 15px;
    flex-wrap: nowrap;
    cursor: pointer;
  }

  &-headerPlaceholder {
    padding: 5px;
  }

  &-title {
    font-size: var(--font-size-xl);
    font-weight: var(--font-weight-bold);
    color: var(--color-primary);
    flex: 1;
  }

  &-close {
    background: none;
    border: none;
    padding: 0;
    margin: 0;
    transition: 0.2s ease all;
    transform: rotate(180deg);
  }

  &-body {
    position: relative;
    display: none;
  }

  &-content {
    padding: 10px 20px 20px;
  }
}
</style>
