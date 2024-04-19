<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "CollapseExpandToggleComponent",
  props: {
    isExpandedAll: {
      type: Boolean,
      default: true,
    },
  },
  methods: {
    collapseAll() {
      this.$emit("onCollapseAll");
    },
    expandAll() {
      this.$emit("onExpandAll");
    },
  },
});
</script>

<template>
  <div>
    <a href="#" @click="isExpandedAll ? collapseAll() : expandAll()">
      {{ isExpandedAll ? "Collapse all" : "Expand all" }}
    </a>
  </div>
</template>

<style />