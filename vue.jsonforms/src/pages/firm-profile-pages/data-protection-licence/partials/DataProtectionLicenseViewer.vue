<script setup lang="ts">
import { toRefs } from "vue";
const props = defineProps<{
  documentUrl?: string;
}>();

const { documentUrl } = toRefs(props);
</script>

<template>
  <ModalComponent width="1200" fixed-height>
    <iframe
      v-if="documentUrl"
      :src="documentUrl"
      style="width: 100%; height: 70vh; border: none"
    >
      There was an error loading your document.
    </iframe>
    <div>No Document Available</div>
  </ModalComponent>
</template>

<style scoped></style>
