<script setup lang="ts">
import { ref } from "vue";

withDefaults(
  defineProps<{
    width: number;
    title: string;
  }>(),
  {
    width: 330,
    title: "",
  },
);

const modalElement = ref();
</script>

<template>
  <ModalComponent ref="modalElement" :width="width">
    <template #title>
      <Label class="title">{{ title }}</Label>
    </template>

    <slot name="content"></slot>
  </ModalComponent>
</template>

<style scoped>
.title {
  font-size: 20px;
  font-weight: 700;
  line-height: 28px;
  letter-spacing: 0;
  text-align: center;
  color: var(--color-black);
}
</style>