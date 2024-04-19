<script setup lang="ts">
import { toRefs, ref } from "vue";

const props = withDefaults(defineProps<{ items: any }>(), {
  items: () => [],
});

const { items } = toRefs(props);

const mediaElement = ref();
</script>

<template>
  <div ref="mediaElement" :class="['Attachment']">
    <div
      v-for="(item, key) in items"
      :key="`attachment-item-${key}`"
      class="Attachment-item"
    >
      <div class="Attachment-block">
        <div
          class="Attachment-thumb"
          :style="{
            backgroundImage: `url(${item.uploadedUrl || item.url})`,
          }"
          :alt="item.name"
        ></div>
        <div class="Attachment-info">
          <span class="Attachment-name">
            <img src="/file-xls.svg" width="24" height="24" />
            <a href="#" class="is-truncated" title="filenamesdfdfsdfsdf.pdf">
              filenamesdfdfsdfsdf.pdf
            </a>
          </span>
          <span class="Attachment-date is-truncated">
            Sep 19, 2020 | 10:52 AM
          </span>
        </div>
        <div class="Attachment-buttons">
          <button type="button" class="Attachment-button" @click.stop>
            <IconComponent symbol="eye" class="text-primary" />
          </button>
          <button type="button" class="Attachment-button" @click.stop>
            <IconComponent symbol="trash" class="text-error" />
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
.Attachment {
  display: flex;
  flex-wrap: wrap;

  &-item {
    flex: 0 0 33.33%;
    max-width: 33.33%;
    padding: 5px;
  }

  &-block {
    cursor: pointer;
    position: relative;
    z-index: 1;
    border: 1px solid var(--color-neutral);
    border-radius: 15px;
    overflow: hidden;
  }

  &-thumb {
    background: transparent no-repeat center;
    background-color: var(--color-neutral);
    background-size: cover;
    height: 100px;
  }

  &-info {
    padding: 10px;
  }

  &-name {
    display: flex;
    gap: 10px;
    align-items: center;
    margin-bottom: 5px;
  }

  &-date {
    font-size: var(--font-size-xs);
  }

  &-buttons {
    position: absolute;
    z-index: 2;
    top: 10px;
    right: 10px;
    display: flex;
    gap: 5px;
  }

  &-button {
    margin: 0;
    padding: 4px;
    background-color: rgba(245, 248, 252, 0.8);
    border: none;
    border-radius: 8px;
  }
}
</style>
