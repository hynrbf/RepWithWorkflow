<script setup lang="ts">
const emits = defineEmits<{ (event: "view"): void; (event: "edit"): void }>();
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
      @click="emits('view')"
    >
      <IconComponent symbol="eye" size="20" />
    </KendoButton>

    <KendoButton
      type="button"
      size="small"
      rounded="full"
      shape="square"
      theme-color="light"
      title="Edit"
      class="ActionButton"
      @click="emits('edit')"
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
