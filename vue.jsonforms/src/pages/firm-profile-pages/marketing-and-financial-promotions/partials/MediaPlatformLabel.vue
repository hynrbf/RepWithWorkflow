<script setup lang="ts">
import { container } from "tsyringe";
import {
  IMediaPlatformService,
  IMediaPlatformServiceInfo,
} from "@/infra/dependency-services/media-platform/IMediaPlatformService";
import { computed } from "vue";

const mediaPlatformService = container.resolve<IMediaPlatformService>(
  IMediaPlatformServiceInfo.name
);

const props = defineProps<{ id: string, iconOnly?: boolean }>();

const platform = computed(() =>
  mediaPlatformService.getMediaPlatform(props.id)
);
</script>

<template>
  <div v-if="platform" class="hstack gap-2">
    <img :src="`/${platform.icon}`" width="20" />
    <span v-if="!iconOnly">{{ platform.name }}</span>
  </div>
</template>
