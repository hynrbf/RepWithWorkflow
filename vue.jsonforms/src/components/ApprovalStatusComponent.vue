<script setup lang="ts">
import { computed, toRefs } from "vue";

const props = defineProps<{ status: "Approved" | "Rejected" | "Pending" }>();
const { status } = toRefs(props);

const imageSrc = computed(() => {
  switch (status.value) {
    case "Approved":
      return "/approved.svg";
    case "Rejected":
      return "/rejected.svg";
    case "Pending":
    default:
      return "/pending.svg";
  }
});
</script>

<template>
  <span class="hstack gap-2">
    <img :src="imageSrc" width="20" height="20" alt="Approved" />
    <slot>{{ status }}</slot>
  </span>
</template>