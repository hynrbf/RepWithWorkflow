<script setup lang="ts">
import { useAttrs } from "vue";

withDefaults(
  defineProps<{
    modelValue?: boolean;
    title?: string;
    autoScrollContent?: boolean;
    fixedHeight?: boolean;
    isShowDownloadPDF?: boolean;
    hasIconTitle?: boolean;
    iconTitle?: string;
  }>(),
  {
    modelValue: true,
    title: "",
  }
);
const emits = defineEmits<{
  (event: "update:modelValue", value: boolean): void;
  (event: "close"): void;
}>();

const attrs = useAttrs();

const close = () => {
  emits("update:modelValue", false);
  emits("close");
};

const downloadPDF = () => {};

defineExpose({
  close,
});
</script>

<script lang="ts">
export default {
  inheritAttrs: false,
};
</script>

<template>
  <Teleport to="body">
    <Dialog
      v-if="modelValue"
      v-bind="attrs"
      :class="[
        'Modal',
        autoScrollContent && 'Modal--autoScrollContent',
        fixedHeight && 'Modal--fixedHeight',
      ]"
      title-render="title"
      :close-icon="false"
      @close="close"
    >
      <template #title>
        <IconTextComponent
          v-if="hasIconTitle"
          :icon="iconTitle"
          class="Modal-icon-title"
        >
          <template #text>
            <span>{{ title }}</span>
          </template>
        </IconTextComponent>
        <h3 v-else class="Modal-title">
          <slot name="title">{{ title }}</slot>
        </h3>

        <KendoButton
          v-if="isShowDownloadPDF"
          type="button"
          theme-color="primary"
          style="box-shadow: none"
          @click="downloadPDF"
        >
          Download PDF
        </KendoButton>

        <slot name="before-close"></slot>
        <KendoButton
          type="button"
          rounded="full"
          shape="square"
          size="small"
          fill-mode="flat"
          class="Modal-close"
          @click="close"
        >
          <IconComponent symbol="close" />
        </KendoButton>
      </template>

      <slot></slot>

      <DialogActionsBar>
        <slot name="footer"></slot>
      </DialogActionsBar>
    </Dialog>
  </Teleport>
</template>
