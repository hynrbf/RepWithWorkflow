<script setup lang="ts">
import { MediaMarketingOutlet } from "@/entities/media-marketing-outlet/MediaMarketingOutlet";

withDefaults(
  defineProps<{ outlet: MediaMarketingOutlet; archiving: boolean; archiveItem?: string }>(),
  {}
);

const emits = defineEmits<{
  (event: "view", id: string): void;
  (event: "archive", id: string, value: boolean): void;
  (event: "remove", id: string): void;
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
      @click="emits('view', `${outlet.id}`)"
    >
      <IconComponent symbol="eye" size="20" />
    </KendoButton>
    <KendoButton
      type="button"
      size="small"
      rounded="full"
      shape="square"
      theme-color="light"
      title="Upload"
      class="ActionButton"
      @click.prevent="emits('archive', `${outlet.id}`, !outlet?.archived)"
    >
      <SpinnerComponent v-if="archiving && archiveItem === outlet.id" :size="20" class="mt-2" />
      <IconComponent
        v-else
        :symbol="outlet.archived ? 'browser-refresh-66' : 'archived'"
        size="18"
        :style="{
          color: outlet?.archived ? 'var(--color-success)' : null,
        }"
      />
    </KendoButton>
    <KendoButton
      v-dev-only
      type="button"
      size="small"
      rounded="full"
      shape="square"
      theme-color="light"
      title="Remove"
      class="ActionButton"
      @click="emits('remove', `${outlet.id}`)"
    >
      <IconComponent symbol="trash-bin-remove-6" size="20" />
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
