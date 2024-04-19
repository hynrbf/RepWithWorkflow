<script setup lang="ts">
import { AppointedRepresentative } from "@/entities/appointed-representatives/AppointedRepresentative";

withDefaults(defineProps<{ item: AppointedRepresentative }>(), {});

const emits = defineEmits<{
  (event: "view", id: string): void;
  (event: "edit", id: string): void;
}>();
</script>

<template>
  <div class="d-inline-flex gap-2">
    <KendoButton
      type="button"
      size="small"
      rounded="full"
      shape="square"
      theme-color="light"
      title="View"
      class="ActionButton"
      @click="emits('view', `${item.id}`)"
    >
      <IconComponent symbol="eye" size="20" />
    </KendoButton>
    <KendoButton
      type="button"
      size="small"
      rounded="full"
      shape="square"
      theme-color="light"
      title="Remove"
      class="ActionButton"
      @click="emits('edit', `${item.id}`)"
    >
      <IconComponent symbol="edit-pen" size="20" />
    </KendoButton>
  </div>
</template>

<style scoped>
.ActionButton {
  width: 30px;
  height: 30px;
  color: var(--color-primary);
}
</style>
