<script setup lang="ts">
import { toRefs } from "vue";
import moment from "moment";

const props = withDefaults(
  defineProps<{
    value: any;
  }>(),
  {
    value: new Date(),
  }
);

const { value } = toRefs(props);
</script>

<template>
  <span class="DateTime">
    <span class="DateTime-date">{{ moment(value).format("DD MMM YYYY") }}</span>
    <span class="DateTime-time">{{ moment(value).format("hh:mm A") }}</span>
  </span>
</template>

<style scoped lang="scss">
.DateTime {
  display: inline-flex;
  gap: 8px;

  &-time {
    color: #414e62;
  }
}
</style>
