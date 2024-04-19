<script setup lang="ts">
import { toRefs, ref, watch, onMounted, onUnmounted, nextTick } from "vue";
import { useElementHelpers } from "@/composables/useElementHelpers";
import { ScrollableTabItemModel } from "@/components/models/ScrollableTabItemModel";

const props = withDefaults(
  defineProps<{
    items: ScrollableTabItemModel[];
    addable?: boolean;
    removable?: boolean;
  }>(),
  {
    items: () => [],
  }
);

const emits = defineEmits<{
  (event: "update:items", value: ScrollableTabItemModel[]): void;
  (event: "add", index: number): void;
  (event: "remove", index: number): void;
}>();

const { items } = toRefs(props);

const { getPrevAll } = useElementHelpers();

const tabs = ref();
const tabsInner = ref();

const autoSlide = (id: string) => {
  if (!tabs.value) {
    return;
  }

  const tabElement = tabs.value.querySelector(`.AdvancedTab-tab-${id}`);

  if (!tabElement) {
    return;
  }

  let sum = 0;

  getPrevAll(tabElement).forEach((element) => {
    if (element) {
      sum += element.clientWidth;
    }
  });

  let dist = sum - (tabsInner.value.clientWidth - tabElement.clientWidth) / 2;

  if (dist < 0) {
    dist = 0;
  }

  if (!tabsInner?.value) {
    return;
  }

  tabsInner.value.scrollBy({
    left: dist - tabsInner.value.scrollLeft,
    behavior: "smooth",
  });
};

const prevCount = ref(0);
const nextCount = ref(0);

const countHiddenTabs = () => {
  if (!tabsInner?.value) {
    return;
  }

  const tabsInnerRect = tabsInner.value.getBoundingClientRect();
  const children = tabsInner.value.children;
  let countLeft = 0;
  let countRight = 0;

  for (let i = 0; i < children.length; i++) {
    const childRect = children[i].getBoundingClientRect();
    if (childRect.right < tabsInnerRect.left) {
      countLeft++;
    }
    if (childRect.left > tabsInnerRect.right) {
      countRight++;
    }
  }

  prevCount.value = countLeft;
  nextCount.value = countRight;
};

const selectTab = (id: string) => {
  emits(
    "update:items",
    items.value.map((item) => {
      item.active = item.id === id;
      return item;
    })
  );
  autoSlide(id);
};

const scrollTabs = (prev: boolean) => {
  tabsInner.value.scrollBy({
    left: prev ? -150 : 150,
    behavior: "smooth",
  });
};

watch(
  items,
  () => {
    nextTick(() => {
      countHiddenTabs();
    });
  },
  { immediate: true }
);

onMounted(() => {
  window.addEventListener("resize", countHiddenTabs);
});

onUnmounted(() => {
  window.removeEventListener("resize", countHiddenTabs);
});
</script>

<template>
  <div class="AdvancedTab">
    <div class="AdvancedTab-top">
      <div ref="tabs" class="AdvancedTab-tabs">
        <div
          ref="tabsInner"
          class="AdvancedTab-tabsInner"
          @scroll="countHiddenTabs"
        >
          <div
            v-for="(tab, index) in items"
            :key="`advancedtab-tab-${tab.id}`"
            :class="[
              'AdvancedTab-tab',
              `AdvancedTab-tab-${tab.id}`,
              tab.active && 'is-active',
            ]"
            @click.prevent="selectTab(tab.id)"
          >
            <span class="is-truncated">{{ tab.title }}</span>

            <button
              v-if="removable && items.length > 1"
              class="AdvancedTab-remove"
              @click.prevent="emits('remove', index)"
            >
              <img src="/clear-circle-close.svg" alt="close" />
            </button>
          </div>
        </div>

        <button
          v-if="prevCount > 0"
          class="AdvancedTab-arrow AdvancedTab-arrowPrev"
          @click.prevent="scrollTabs(true)"
        >
          {{ `<${prevCount}` }}
        </button>

        <button
          v-if="nextCount > 0"
          class="AdvancedTab-arrow AdvancedTab-arrowNext"
          @click.prevent="scrollTabs(false)"
        >
          {{ `${nextCount}>` }}
        </button>
      </div>

      <button
        v-if="addable"
        class="AdvancedTab-add pl-5px pr-16px"
        @click.prevent="emits('add', items.length)"
      >
        <IconComponent symbol="add-circle-27" size="20" />

        <strong>Add</strong>
      </button>
    </div>

    <div class="AdvancedTab-contents">
      <template
        v-for="(tab, index) in items"
        :key="`advancedtab-content-${tab.id}`"
      >
        <div
          v-show="tab.active"
          :class="['AdvancedTab-content', `AdvancedTab-content-${tab.id}`]"
        >
          <!--ToDo. does this use here :tab="tab" :index="index", to check to other pages -->
          <slot :name="`content(${tab.id})`" :tab="tab" :index="index">
            {{ tab.content }}
          </slot>
        </div>
      </template>
    </div>
  </div>
</template>

<style scoped lang="scss">
.AdvancedTab {
  &-top {
    display: flex;
    gap: 10px;
  }

  &-tabs {
    position: relative;
    z-index: 1;
    overflow-x: hidden;
  }

  &-tabsInner {
    position: relative;
    display: flex;
    gap: 5px;
    padding: 0 2px;
    overflow: hidden;
    flex-wrap: nowrap;
  }

  &-tab {
    border: 0.3px solid var(--color-neutral);
    background-color: var(--color-light);
    color: var(--color-dark);
    border-bottom-width: 0;
    border-top-left-radius: 8px;
    border-top-right-radius: 8px;
    padding: 8px 15px;
    font-weight: var(--font-weight-bold);
    font-size: var(--font-size-lg);
    display: flex;
    gap: 10px;
    align-items: center;
    justify-content: space-between;
    max-width: 200px;
    transition: 0.3s ease all;
    cursor: pointer;

    &.is-active {
      background-color: var(--color-white);
      color: var(--color-primary);
    }
  }

  &-remove,
  &-add,
  &-arrow {
    background: none;
    padding: 0;
    margin: 0;
    border: none;
  }

  &-remove {
    img {
      display: block;
    }
  }

  &-add {
    color: var(--color-primary);
    display: flex;
    gap: 5px;
    align-items: center;
  }

  &-arrow {
    position: absolute;
    z-index: 2;
    display: block;
    width: 30px;
    height: 30px;
    border: 2px solid var(--color-primary);
    background: var(--color-white);
    border-radius: 50%;
    color: var(--color-primary);
    font-size: var(--font-size-sm);
    font-weight: var(--font-weight-semi-bold);
    text-align: center;
    top: 50%;
    transform: translateY(-50%);

    &:hover {
      background-color: var(--color-primary);
      color: var(--color-white);
    }

    &Prev {
      left: 0;
    }

    &Next {
      right: 0;
    }
  }
}
</style>
