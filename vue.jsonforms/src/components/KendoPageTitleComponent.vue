<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "KendoPageTitleComponent",
  props: {
    title: {
      type: String,
      default: "",
    },
    description: {
      type: String,
      default: "",
    },
    progressBar: {
      type: Boolean,
      default: false,
    },
    titleClass: {
      type: String,
      default: "",
    },
    routeName: {
      type: String,
      default: "",
    },
  },
});
</script>

<template>
  <div class="parent-container row gy-custom">
    <StackLayout orientation="horizontal">
      <h1 :class="['title-page text-primary mb-0', titleClass]">
        <slot name="title">{{ title }}</slot>
      </h1>

      <KendoProgressBarComponent v-if="progressBar" :routeName="routeName" />
    </StackLayout>

    <div class="Description">
      <slot name="description">
        <p v-if="description">{{ description }}</p>
      </slot>
    </div>
  </div>
</template>

<style scoped lang="scss">
.FormProgress {
  max-width: 360px;
  text-align: right;
  margin-right: 7px;

  &-progressBar {
    height: 4px;
  }

  &-label {
    font-weight: var(--font-weight-semi-bold);
  }
}

.Description {
  &, :deep(p), :deep(li) {
    color: var(--text-text-secondary);
    font-size: var(--font-size-lg);
    text-align: justify;
    font-style: normal;
    font-weight: var(--font-weight-medium);
    line-height: 130%; /* 20.8px */
  }
}

.title-page {
  font-size: var(--font-size-2xl);
  line-height: 133%; /* 32px*/
  margin: auto;
}

.parent-container {
  display: flex;
  flex-direction: column;
  padding-top: 0;
}

.gy-custom {
  --bs-gutter-y: 20px;
}
</style>