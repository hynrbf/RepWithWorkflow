<script setup lang="ts">
import { useSlots } from 'vue';
const slots = useSlots();
</script>

<template>
  <CollapsiblePanelComponent
    class="PromotionDetails"
    :items="[
      {
        id: 'details-text',
        title: 'Text',
        content: '',
        active: true,
      },
      {
        id: 'details-media',
        title: 'Photos & Videos',
        content: '',
      },
      {
        id: 'details-attachments',
        title: 'Attachments',
        content: '',
      },
    ]"
  >
    <template v-if="slots['text-content']" #content-details-text>
      <slot name="text-content"></slot>
    </template>
    <template v-if="slots['media-content']" #content-details-media>
      <slot name="media-content"></slot>
    </template>
    <template v-if="slots['attachments-content']" #content-details-attachments>
      <slot name="attachments-content"></slot>
    </template>
  </CollapsiblePanelComponent>
</template>

<style scoped lang="scss">
.PromotionDetails {
  :deep(.CollapsiblePanel-item) {
    border: none;
    border-bottom: 1px solid #dfdfdf;
    border-radius: 0;
    margin-bottom: 10px;
  }
  :deep(.CollapsiblePanel-header) {
    padding: 0 0 10px;
  }
  :deep(.CollapsiblePanel-content) {
    padding: 10px 0;
  }
  :deep(.CollapsiblePanel-title) {
    font-size: var(--font-size-lg);
    color: var(--color-black);
    font-weight: var(--font-weight-semi-bold);
  }
}
</style>
