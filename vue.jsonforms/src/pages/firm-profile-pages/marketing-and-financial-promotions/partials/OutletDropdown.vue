<script setup lang="ts">
import {computed, toRefs} from "vue";
import { useMediaMarketingOutletStore } from "@/stores/media-marketing-outlet/useMediaMarketingOutletStore";
import { storeToRefs } from "pinia";

const props = withDefaults(
    defineProps<{
      id?:string;
    }>(),
    {}
);

const { id } = toRefs(props);

const mediaMarketingOutletStore = useMediaMarketingOutletStore();

const { mediaMarketingOutlets } = storeToRefs(mediaMarketingOutletStore);

const outletsOptions = computed(() =>
  mediaMarketingOutlets.value
    .filter((outlet) => !outlet.archived)
    .map((outlet) => ({ label: outlet.name, value: outlet.id }))
);

const getOutlet = (id: string) =>
mediaMarketingOutletStore.getMediaMarketingOutlet(id);
</script>

<template>
  <KendoDropdownListComponent
    name="mediaOutlet"
    :id="id"
    label="Media Outlet"
    :data-items="outletsOptions"
    value-primitive
  >
    <template #display="{ value }">
      <MediaLabelComponent
        :id="`${getOutlet(value.value)?.platform}`"
        :text="value.label"
        :iconSize="32"
      />
    </template>
  </KendoDropdownListComponent>
</template>
