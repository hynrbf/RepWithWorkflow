<script lang="ts">
// ToDo. Replace the TinyMce with CKEditor
import { defineComponent, inject } from "vue";
import { container } from "tsyringe";
import Editor from "@tinymce/tinymce-vue";
import {
  IFormValidatorService,
  IFormValidatorServiceInfo,
} from "@/infra/dependency-services/form-validator/IFormValidatorService";
import { TINY_MCE_API_KEY } from "@/config";
import { useKendoHelpers } from "@/composables/useKendoHelpers";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";

const { formValueToObject } = useKendoHelpers();

export default defineComponent({
  name: "KendoRichTextComponent",
  components: { TinyMCEEditor: Editor },
  props: {
    name: {
      type: String,
      default: "",
    },
    id: String,
    label: {
      type: String,
      default: "",
    },
    placeholder: {
      type: String,
      default: "Select ...",
    },
    hint: {
      type: String,
      default: "",
    },
    disabled: {
      type: Boolean,
      default: false,
    },
    isRequired: {
      type: Boolean,
      default: true,
    },
    dataItems: {
      type: Array,
      default: [] as { label: string; value: string }[],
    },
    valuePrimitive: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      tinyMCEApiKey: TINY_MCE_API_KEY,
      formValidatorService: container.resolve<IFormValidatorService>(
        IFormValidatorServiceInfo.name,
      ),
      errorMessage: ""
    };
  },
  methods: {
    validate(currentValue: string) {
      this.errorMessage = "";
      this.formValidatorService.setFieldName(this.label || this.name);
      const errorMessage = this.formValidatorService.validate<string>(
        currentValue,
        {
          required: this.isRequired,
        },
      );
      this.addIdKeyAndErrorValue(errorMessage);
      this.errorMessage = errorMessage;
      return errorMessage;
    },
    handleChange(value: string) {
      this.kendoForm.onChange(this.name, { value });
      formValueToObject(this.kendoForm, this.name);
    },
    addIdKeyAndErrorValue(value: string) {
      if (!this.id) {
        return;
      }

      const key = this.id;
      let errorItem: Record<string, string> = {};
      errorItem[key] = value;
      this.addComponentValidationValue(key, errorItem);
    },
  },
  watch: {
    "kendoForm.values": {
      handler(value) {
        const editor = this.$refs["editor"] as any;
        !value[this.name] && editor.getEditor().setContent("");
      },
      deep: true,
    },
  },
  setup() {
    const pageComponentValidationValueStore =
      usePageComponentValidationValueStore();
    const { addComponentValidationValue } = pageComponentValidationValueStore;

    return {
      kendoForm: inject<any>("kendoForm", { default: {} }),
      addComponentValidationValue,
    };
  },
});
</script>

<template>
  <KendoField :id="id" :name="name" component="template" :validator="validate">
    <template v-slot:template="{ props }">
      <KendoFieldWrapper :class="props.class">
        <KendoLabel
          v-if="label"
          :editor-id="id"
          :disabled="disabled"
          :valid="!(props.touched && errorMessage)"
          class="control-label mb-2"
        >
          {{ label }}
          <span v-if="!isRequired" class="fineprint ms-1"> (Optional) </span>
        </KendoLabel>

        <TinyMCEEditor
          ref="editor"
          :api-key="tinyMCEApiKey"
          :model-value="props.value"
          @update:model-value="handleChange"
        />

        <KendoError v-if="props.touched && errorMessage">
          {{ errorMessage }}
        </KendoError>

        <KendoHint v-else>
          {{ hint }}
        </KendoHint>
      </KendoFieldWrapper>
    </template>
  </KendoField>
</template>