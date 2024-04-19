<script setup lang="ts">
import { toRefs, computed } from "vue";

const props = withDefaults(defineProps<{ modelValue: string }>(), {
  modelValue: "",
});

const emits = defineEmits<{
  (event: "update:modelValue", value: string): void;
}>();

const { modelValue } = toRefs(props);

const tools = computed(() => [
  ["Bold", "Italic", "Underline", "Strikethrough"],
  ["Subscript", "Superscript"],
  ["AlignLeft", "AlignCenter", "AlignRight", "AlignJustify"],
  ["Indent", "Outdent"],
  ["OrderedList", "UnorderedList"],
  // "FontSize",
  // "FontName",
  // "FormatBlock",
  ["Undo", "Redo"],
]);
</script>

<template>
  <KendoEditor
    :value="modelValue"
    class="RichText"
    :tools="tools"
    :content-style="{ height: '200px' }"
    placeholder="Add Comment"
    default-edit-mode="div"
    @change="emits('update:modelValue', $event.html)"
  >
  </KendoEditor>
</template>

<style scoped lang="scss">
.RichText {
  :deep(.k-button) {
    padding: 5px;
  }

  :deep(.k-dropdownlist) {
    .k-input-inner {
      padding: 5px;
    }
  }

  :deep(.k-editor-content) {
    cursor: text;
  }
}
</style>