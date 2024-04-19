<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  props: {
    title: String,
    subtitle: String,
    collapse: Boolean,
    expandIcon: String,
    collapseIcon: String,
    hideIndicator: Boolean
  },
  data() {
    return {
      isCollapsed: false,
    };
  },
  watch: {
    collapse: {
      handler(value) {
        this.isCollapsed = value;
      },
      immediate: true,
    },
  },
  methods: {
    toggleCollapse() {
      this.$emit("update:collapse", (this.isCollapsed = !this.isCollapsed));
      this.$emit("onToggleCollapse", this.isCollapsed);
    },
  },
});
</script>

<template>
  <KendoExpansionPanel
    ref="mainElement"
    :expanded="!isCollapsed"
    title-render="title"
    subtitle-render="subtitle"
    :class="{'k-card collapsible-panel': true, 'indicator-hidden': hideIndicator}"
    @action="toggleCollapse"
  >
    <template #title>
      <slot name="title">
        <h3 class="font-size-xl text-primary my-2">{{ title }}</h3>
      </slot>

      <slot v-if="isCollapsed && collapseIcon" name="collpaseIcon">
        <img :src="collapseIcon" alt="arrow" :class="{'k-pos-absolute k-right-5': hideIndicator}" />
      </slot>
      <slot v-else-if="!isCollapsed && expandIcon" name="expandIcon">
        <img :src="expandIcon" alt="arrow" :class="{'k-pos-absolute k-right-5': hideIndicator}" />
      </slot>
    </template>  
  
    <template #subtitle>
      <slot name="subtitle">{{ subtitle }}</slot>
    </template>

    <div v-show="!isCollapsed">
      <KendoExpansionPanelContent>
        <slot></slot>
      </KendoExpansionPanelContent>
    </div>
  </KendoExpansionPanel>
</template>