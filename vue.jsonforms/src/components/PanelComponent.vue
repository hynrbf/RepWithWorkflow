<script setup lang="ts">
import { useSlots } from "vue";

withDefaults(
  defineProps<{
    title?: string;
    description?: string;
    noHead?: boolean;
    bodyClass?: string;
  }>(),
  {
    bodyClass: "",
  },
);
const slots = useSlots();
</script>

<template>
  <div class="Panel">
    <div
      v-if="
        noHead ||
        title ||
        slots?.title ||
        slots?.head ||
        description ||
        slots.description
      "
      class="Panel-head"
    >
      <slot name="head">
        <h4 v-if="title || slots?.title" class="Panel-title">
          <slot name="title">
            {{ title }}
          </slot>
        </h4>
        <div v-if="description || slots.description" class="Panel-description">
          <slot name="description">{{ description }}</slot>
        </div>
      </slot>
    </div>
    <div :class="['Panel-body', bodyClass]">
      <slot></slot>
    </div>
  </div>
</template>