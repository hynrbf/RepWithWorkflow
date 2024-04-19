<script setup lang="ts">
import { computed, toRefs, ref, onMounted } from "vue";
import lightGallery from "lightgallery";
import lgThumbnail from "lightgallery/plugins/thumbnail";

const props = withDefaults(defineProps<{ items: any }>(), {
  items: () => [],
});

const { items } = toRefs(props);

const galleryItems = computed(() =>
  items.value.map((item: any) => ({
    src: item.uploadedUrl || item.url,
    thumb: item.uploadedUrl || item.url,
  }))
);

const mediaElement = ref();
const lightGalleryInstance = ref();

onMounted(() => {
  lightGalleryInstance.value = lightGallery(mediaElement.value, {
    plugins: [lgThumbnail],
    dynamic: true,
    dynamicEl: galleryItems.value,
  });
});
</script>

<template>
  <div ref="mediaElement" :class="['Media']">
    <div
      v-for="(item, key) in items"
      :key="`media-item-${key}`"
      class="Media-item"
      @click.prevent="lightGalleryInstance.openGallery(key)"
    >
      <div
        class="Media-thumb"
        :style="{
          backgroundImage: `url(${item.uploadedUrl || item.url})`,
        }"
        :alt="item.name"
      ></div>
      <button type="button" class="Media-button" @click.stop>
        <IconComponent symbol="trash" class="text-error" />
      </button>
    </div>
  </div>
</template>

<style lang="scss">
@import "lightgallery/css/lightgallery.css";
@import "lightgallery/css/lg-thumbnail.css";

.lg-container {
  .lg-backdrop {
    z-index: 10002;
  }
  .lg-outer {
    z-index: 10003;
  }
}

.Media {
  display: flex;
  flex-wrap: wrap;

  &-item {
    flex: 0 0 50%;
    max-width: 50%;
    padding: 5px;
    cursor: pointer;
    position: relative;
    z-index: 1;
  }

  &-thumb {
    background: transparent no-repeat center;
    background-size: cover;
    height: 140px;
  }

  &-button {
    margin: 0;
    padding: 4px;
    background-color: rgba(245, 248, 252, 0.8);
    border: none;
    border-radius: 8px;
    position: absolute;
    z-index: 2;
    top: 15px;
    right: 15px;
  }
}
</style>
