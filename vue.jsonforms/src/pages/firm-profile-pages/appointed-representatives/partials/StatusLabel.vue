<script setup lang="ts">
import { AppointedRepresentative } from "@/entities/appointed-representatives/AppointedRepresentative";
import { computed, toRefs } from "vue";
import StaticList from "@/infra/StaticListService";

const props = defineProps<{ item: AppointedRepresentative }>();

const { item } = toRefs(props);

const text = computed(
  () => StaticList.getARStatuses().map(({ label }) => label)[item.value.status ?? 1]
);

const status = computed(() => [0, 2, 1, 1][item.value.status ?? 1])
</script>

<template>
  <StatusLabelComponent no-icon :status="status" :text="text" />
</template>
