<script setup lang="ts">
import { computed } from "vue";
import { container } from "tsyringe";
import {
  IMediaPlatformService,
  IMediaPlatformServiceInfo,
} from "@/infra/dependency-services/media-platform/IMediaPlatformService";

const mediaPlatformService = container.resolve<IMediaPlatformService>(
  IMediaPlatformServiceInfo.name
);

const props = withDefaults(
  defineProps<{
    id: string;
    iconOnly?: boolean;
    iconSize?: number;
    text?: string;
    textOnly?: boolean;
  }>(),
  { iconSize: 20 }
);

const platform = computed(() =>
  mediaPlatformService.getMediaPlatform(props.id)
);

const isAds = computed(() =>
  ["google-ads", "facebook-ads", "twitter-ads", "linkedin-ads", "instagram-ads"].includes(
    `${platform?.value?.id}`
  )
);
</script>

<template>
  <div v-if="platform" class="MediaLabel">
    <img
      v-if="!textOnly"
      :src="`/${platform.icon}`"
      :width="iconSize"
      :alt="platform.name"
    />
    <span v-if="!iconOnly">
      <slot name="text">{{ text || platform.name }}</slot>
      <strong v-if="isAds" class="MediaLabel-ads"> Ads</strong>
    </span>
  </div>
</template>

<style scoped lang="scss">
.MediaLabel {
  display: flex;
  align-items: center;
  gap: 8px;

  &-ads {
    margin-right: 5px;
  }
}
</style>
