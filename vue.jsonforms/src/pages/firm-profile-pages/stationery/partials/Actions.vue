<script setup lang="ts">
import { AppConstants } from "@/infra/AppConstants";
import { StationeryEntity } from "@/entities/stationery/StationeryEntity";

withDefaults(
  defineProps<{ stationery: Partial<StationeryEntity>; disabled?: boolean }>(),
  {}
);

const emits = defineEmits<{
  (event: "view", id: string, action: string): void;
  (event: "upload", id: string): void;
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
      @click="emits('view', `${stationery.id}`, AppConstants.viewAction)"
      :disabled="!stationery?.files?.length"
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
      @click="emits('upload', `${stationery.id}`)"
    >
      <IconComponent symbol="upload-file-28" size="20" />
    </KendoButton>
    <KendoButton
      v-if="
        stationery &&
        stationery.files &&
        (stationery.files.length === 0 || stationery.files.length > 1)
      "
      type="button"
      size="small"
      rounded="full"
      shape="square"
      theme-color="light"
      title="Download"
      class="ActionButton"
      :disabled="!stationery?.files?.length"
      @click="emits('view', `${stationery.id}`, AppConstants.downloadAction)"
    >
      <IconComponent symbol="download-box-2-19" size="20" />
    </KendoButton>
    <a
      v-else-if="
        stationery && stationery?.files && stationery?.files.length == 1
      "
      :href="`${stationery.files[0].url}`"
      target="_blank"
      class="k-button k-button-sm k-button-square k-rounded-full k-button-solid k-button-solid-light ActionButton disabled"
    >
      <span class="k-button-text">
        <IconComponent symbol="download-box-2-19" size="20" />
      </span>
    </a>
  </div>
</template>

<style scoped>
.ActionButton {
  width: 30px;
  height: 30px;
  color: var(--color-primary);
}
</style>
