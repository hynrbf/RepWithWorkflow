<script setup lang="ts">
import { useSlots } from "vue";

withDefaults(
  defineProps<{ items: string[]; alignTop?: boolean; hiddenItems?: number[]; disabledItems?: number[] }>(),
  {
    items: () => [],
    hiddenItems: () => [],
    disabledItems: () => [],
  },
);

const slots = useSlots();
</script>

<template>
  <ul :class="['NumberList', alignTop && 'NumberList--alignTop']">
    <template v-for="(item, index) in items" :key="`item-${index}`">
      <li
        :class="[
          'NumberList-item',
          disabledItems.includes(index + 1) && 'is-disabled',
        ]"
        v-if="!hiddenItems.includes(index + 1)"
      >
        <span class="NumberList-bullet">
          {{ index + 1 }}
        </span>
        <span class="NumberList-text">
          <slot :name="`item-${index + 1}`" :item="item">{{ item }}</slot>
        </span>
      </li>
      <li v-if="slots[`item-extend-${index + 1}`]" class="NumberList-extend">
        <slot :name="`item-extend-${index + 1}`" :item="item">{{ item }}</slot>
      </li>
    </template>
  </ul>
</template>

<style scoped lang="scss">
.NumberList {
  list-style-type: none;
  margin: 0;
  padding: 0;

  &-item {
    margin-bottom: 15px;
    display: flex;
    align-items: center;

    &.is-disabled {
      opacity: 0.4;
    }
  }

  &-bullet {
    margin-right: 10px;
    width: 24px;
    height: 24px;
    line-height: 24px;
    display: block;
    background-color: var(--color-primary);
    color: var(--color-white);
    border-radius: 18px;
    text-align: center;
    font-size: var(--font-size-sm);
  }

  &-text {
    flex: 1;
  }

  &-extend {
    margin-bottom: 15px;
  }

  &--alignTop {
    .NumberList-item {
      align-items: start;
    }
  }
}
</style>