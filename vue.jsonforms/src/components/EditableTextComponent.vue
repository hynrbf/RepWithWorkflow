<script lang="ts">
import { defineComponent } from "vue";
import { TINY_MCE_API_KEY } from "@/config";
import Editor from "@tinymce/tinymce-vue";
import { useNotification } from "@/composables/useNotification";
import { useClipboard } from "@vueuse/core";

export default defineComponent({
  name: "EditableTextComponent",
  components: { TinyMCEEditor: Editor },
  props: {
    modelValue: {
      type: String,
      default: "",
    },
  },
  data() {
    return {
      tinyMCEApiKey: TINY_MCE_API_KEY,
      isActive: false,
      isFocused: false,
      editableText: "",
    };
  },
  computed: {
    savable() {
      return this.editableText !== this.modelValue;
    },
  },
  methods: {
    setActive(value: boolean) {
      this.isActive = value;

      this.$nextTick(() => {
        if (value) this.$emit("activated");
        else this.$emit("deactivated");
      });
    },
    save() {
      this.$emit("update:modelValue", this.editableText);
      this.$emit("saved", this.editableText);
      this.setActive(false);
    },
    copy() {
      useNotification({
        content: "Copied to clipboard.",
        interval: 2e3,
      });

      const { isSupported, copy } = useClipboard();
      isSupported && copy(this.editableText);
    },
    cancel() {
      this.editableText = this.modelValue;
      this.setActive(false);
    },
  },
  mounted() {
    this.editableText = this.modelValue;
  },
  expose: ["setActive"],
});
</script>

<template>
  <div :class="['EditableText', isActive && 'EditableText-is-active']">
    <div
      v-if="!isActive"
      class="EditableText-content"
      @click="isFocused = !isFocused"
    >
      <div class="EditableText-text" ref="text" v-html="editableText"></div>
      <div v-if="isFocused" class="EditableText-actions">
        <KendoButtonGroup>
          <KendoButton @click.prevent="setActive(true)">Edit</KendoButton>
          <KendoButton @click.prevent="copy">Copy</KendoButton>
        </KendoButtonGroup>
      </div>
    </div>
    <div v-else>
      <slot
        name="editor"
        :editable-text="editableText"
        :update-editable-text="(value: string) => (editableText = value)"
        :save="save"
        :cancel="cancel"
        :activate="() => setActive(true)"
        :deactivate="() => setActive(false)"
      >
        <TinyMCEEditor :api-key="tinyMCEApiKey" v-model="editableText" />

        <div class="mt-4 hstack gap-2 justify-content-end">
          <KendoButton theme-color="primary" :disabled="!savable" @click="save">
            Save Changes
          </KendoButton>
          <KendoButton theme-color="error" @click="cancel">Cancel</KendoButton>
        </div>
      </slot>
    </div>
  </div>
</template>

<style scoped lang="scss">
.EditableText {
  outline: none;
  border: 1px solid transparent;
  transition: 0.2s ease border;
  border-radius: 5px;
  overflow: hidden;

  &:focus,
  &:hover {
    border-color: var(--color-primary);
  }

  &-is-active {
    border: none;
    overflow: visible;
  }

  &-content {
    padding: 10px;
    position: relative;
  }

  &-actions {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    background-color: rgba(0, 0, 0, 0.5);
    width: 100%;
    height: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
  }
}
</style>