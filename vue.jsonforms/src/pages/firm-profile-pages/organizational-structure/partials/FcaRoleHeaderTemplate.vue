<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "FcaRoleHeaderTemplate",
  props: {
    isMultiSelect: {
      type: Boolean,
      default: false,
    },
  },
});
</script>

<template>
  <div
    class="primary-role-dropdown-header"
    :class="isMultiSelect ? 'adjust' : ''"
  >
    <div class="first-column">&nbsp;</div>

    <!-- if scrollbar is visible, right margin must be set to 20px; -->
    <div
      class="col d-flex align-items-center"
      :style="`margin-right: ${isMultiSelect ? '20px' : ''}`"
    >
      <div class="col">FCA Authorised</div>
      <div class="col">Add</div>
      <div class="col">Pending</div>
      <div class="col">Remove</div>
    </div>
  </div>
</template>

<style scoped>
.primary-role-dropdown-header {
  display: flex;
  height: 44px;
  padding-right: 35px; /* 25px vertical scrollbar width */
  align-items: center;
  gap: 0;
  flex-shrink: 0;
  background: #e4e7ec;

  color: #000;
  text-align: center;
  font-size: var(--font-size-xs);
  font-style: normal;
  font-weight: 600;
  line-height: 16px; /* 133.333% */
}

.adjust {
  padding-right: 16px !important;
  gap: 0 !important;
}

.adjust .first-column {
  margin-left: 38px;
}

.first-column {
  margin-left: 16px;
  width: 380px; /* 435px */
}
</style>
