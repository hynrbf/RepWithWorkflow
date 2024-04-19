<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "KendoLoadingComponent",
  props: {
    isLoading: {
      type: Boolean,
      default: false,
    },
    isShowSavingText: {
      type: Boolean,
      default: false,
    },
    loaderType: {
      type: String,
      default: "overlay", // overlay | component
    },
    loadingText: {
      type: String,
      default: undefined,
    },
    overlayClass: {
      type: [Object, String],
      default: undefined,
    },
    wrapperClass: {
      type: [Object, String],
      default: undefined,
    },
  },
});
</script>

<template>
  <div
    v-if="isLoading"
    :class="[
      {
        'container-fluid k-animation-container h-100 parent-loader-container':
          loaderType === 'overlay',
        'cparent-loader-container': loaderType === 'component',
      },
      overlayClass,
    ]"
  >
    <div
      :class="[
        {
          'd-flex flex-column dialog-box child-loader-container':
            loaderType === 'overlay',
          'd-flex flex-column cchild-loader-container':
            loaderType === 'component',
        },
        wrapperClass,
      ]"
    >
      <img
        src="@/assets/img/loader.gif"
        class="custom-loader"
        style="color: var(--color-primary); height: -20px"
        alt="Loading..."
      />

      <Label v-if="isShowSavingText" class="loading-text">{{
        $t("common-saving-text")
      }}</Label>

      <Label v-else class="loading-text">{{
        loadingText || $t("common-loading-text")
      }}</Label>
    </div>
  </div>
</template>

<style scoped>
.parent-loader-container {
  top: 0;
  left: 0;
  z-index: 1050;
  background-color: transparent;
  position: fixed;
}

.child-loader-container {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
}

.dialog-box {
  background: var(--color-white);
  width: 120px;
  height: 120px;
  justify-content: center;
  align-items: center;
  gap: 20px;
  flex-shrink: 0;
  border-radius: 8px;
  box-shadow: 4px 4px 20px 0px rgba(0, 0, 0, 0.25);
}

.loading-text {
  color: var(--color-primary);
  text-align: center;
  font-size: var(--font-size-sm);
  font-style: normal;
  font-weight: var(--font-weight-semi-bold);
  line-height: 125%;
}

.custom-loader {
  max-width: 50px;
  max-height: 50px;
}
</style>