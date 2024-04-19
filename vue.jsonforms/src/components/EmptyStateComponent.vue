<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "EmptyStateComponent",
  props: {
    label: {
      type: String,
      default: "",
    },
    height: {
      type: Number,
      default: 410,
    },
    icon: {
      type: String,
      default: "user-circle-single-49",
    },
  },
});
</script>

<template>
  <Card class="card">
    <StackLayout
      orientation="vertical"
      :style="`height: ${height}px; gap: 17px;`"
      :align="{ horizontal: 'center', vertical: 'middle' }"
    >
      <IconComponent size="50" :symbol="icon" style="color: #414e62" />

      <Label class="label">{{ label }}</Label>
    </StackLayout>
  </Card>
</template>

<style scoped>
.label {
  font-size: var(--font-size-2xl);
  color: var(--text-text-primary);
  text-align: center;
  font-style: normal;
  font-weight: var(--font-weight-bold);
  line-height: 32px; /* 133.333% */
}

.card {
  border-radius: 8px;
  border: 0.3px solid var(--text-text-disabled);
  background: var(--color-white);
}
</style>