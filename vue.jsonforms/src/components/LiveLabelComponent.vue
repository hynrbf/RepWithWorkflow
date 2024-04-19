<script setup lang="ts">
defineProps<{ live: boolean }>();
</script>

<template>
  <StatusLabelComponent
    icon="dot"
    icon-size="8"
    :status="live ? 'approved' : 'rejected'"
    class="gap-1"
  >
    {{ live ? "Live" : "Not Live" }}
  </StatusLabelComponent>
</template>