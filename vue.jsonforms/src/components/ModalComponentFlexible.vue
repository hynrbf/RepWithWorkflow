<script setup lang="ts">
withDefaults(
  defineProps<{
    modelValue?: boolean;
    title?: string;
    isTitleCentered?: boolean;
    modalClass?: object
  }>(),
  {
    modelValue: true,
    title: "",
    isTitleCentered: false,
  },
);
const emits = defineEmits<{
  (event: "update:modelValue", value: boolean): void;
  (event: "close"): void;
}>();

const close = () => {
  emits("update:modelValue", false);
  emits("close");
};

defineExpose({
  close,
});
</script>

<template>
  <Teleport to="body">
    <Dialog
      v-if="modelValue"
      :class="{'ModalFlexible': true, ...modalClass}"
      title-render="title"
      :close-icon="false"
      @close="close"
    >
      <template #title>
        <h3
          class="Modal-title"
          :style="{ textAlign: isTitleCentered ? 'center' : 'left' }"
        >
          <slot name="title">{{ title }}</slot>
        </h3>

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