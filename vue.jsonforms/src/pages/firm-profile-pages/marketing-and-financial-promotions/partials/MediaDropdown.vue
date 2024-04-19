<script setup lang="ts">
import { container } from "tsyringe";
import {
  IMediaPlatformService,
  IMediaPlatformServiceInfo,
} from "@/infra/dependency-services/media-platform/IMediaPlatformService";

defineProps<{
  multiple?: boolean;
  noIcon?: boolean;
  id?: string;
}>();

const mediaPlatformService = container.resolve<IMediaPlatformService>(
  IMediaPlatformServiceInfo.name
);
const mediaPlatformsOptions = mediaPlatformService
  .getMediaPlatforms()
  .map((item) => ({ label: item.name, value: item.id }));
</script>

<template>
  <KendoDropdownListComponent
    :id="`${id}-remunerationMethod`"
    name="remunerationMethod"
    v-if="!multiple"
    label="Media Outlet"
    :data-items="mediaPlatformsOptions"
    value-primitive
  >
    <template #display="{ value }">
      <MediaLabelComponent :id="value?.value" :text-only="noIcon" />
    </template>
  </KendoDropdownListComponent>
  <KendoMultiSelectTreeComponent
    v-else
    :id="`${id}-media-platform-options`"
    name="media-platform-options"
    :data-items="mediaPlatformsOptions"
    value-primitive
  />
</template>
