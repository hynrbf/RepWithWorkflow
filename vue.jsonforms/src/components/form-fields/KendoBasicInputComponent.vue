<script lang="ts">
import { defineComponent } from "vue";
import { Field } from "@progress/kendo-vue-form";
import { Label, Error } from "@progress/kendo-vue-labels";
import { Input } from "@progress/kendo-vue-inputs";
import { StackLayout } from "@progress/kendo-vue-layout";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";

export default defineComponent({
  name: "KendoBasicInputComponent",
  //props are meant to be parent to child passing only, not vice versa
  props: {
    id: String,
    isRequired: {
      type: Boolean,
      default: true,
    },
    value: {
      type: String,
      default: "",
    },
    name: {
      type: String,
      default: "",
    },
    label: {
      type: String,
      default: "",
    },
    placeholder: {
      type: String,
      default: "Type",
    },

    //these both should be used for reactive data,
    //otherwise the validation of Kendo behaves incorrectly
    isValueReactive: {
      type: Boolean,
      default: false,
    },
    isDataLoadedCompletely: {
      type: Boolean,
      default: false,
    },
  },
  components: {
    Field,
    StackLayout,
    Label,
    Error,
    Input,
  },
  data() {
    return {
      isFieldValidOnFirstLoad: false,
      isUnmounted: false,
      errorMessage: "",
    };
  },
  computed: {
    computedPlaceholder() {
      if (!this.$t("common-placeholder-text")) {
        return this.placeholder;
      }

      return this.$t("common-placeholder-text");
    },
  },
  created() {
    this.isUnmounted = false;
    this.isFieldValidOnFirstLoad = this.validate(this.value) == "";
  },
  mounted() {
    this.isUnmounted = false;
  },
  beforeUnmount() {},
  unmounted() {
    this.isUnmounted = true;
  },
  setup() {
    const pageComponentValidationValueStore =
      usePageComponentValidationValueStore();
    const { addComponentValidationValue, removeValidationValueByFieldId } =
      pageComponentValidationValueStore;

    return {
      addComponentValidationValue,
      removeValidationValueByFieldId,
    };
  },
  methods: {
    validate(currentValue: string): string {
      this.errorMessage = "";
      //note, even if you remove the component in the DOM 'using v-if', the vue still preserves it in memory
      //that's why we need this variable
      if (this.isUnmounted) {
        return "";
      }

      if (this.isValueReactive && !this.isDataLoadedCompletely) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (this.isFieldValidOnFirstLoad) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (this.isValueReactive && !currentValue && this.value) {
        currentValue = this.value;
      }

      if (!this.isRequired) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (!currentValue) {
        const errorMessage = `Please enter a valid ${this.label}`;
        this.addIdKeyAndErrorValue(errorMessage);
        this.errorMessage = errorMessage;
        return errorMessage;
      }

      this.addIdKeyAndErrorValue("");
      return "";
    },

    onInput(event: any) {
      this.isFieldValidOnFirstLoad = false;
      this.$emit("onValueChange", event.target.value);
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
});
</script>

<template>
  <Field
    ref="fieldRef"
    :name="name"
    :id="id"
    :validator="validate"
    :component="'myTemplate'"
  >
    <template v-slot:myTemplate="{ props }">
      <StackLayout orientation="vertical" :gap="10">
        <!--        {{ props }}-->
        <Label :editor-id="props.id" v-if="isRequired" class="control-label"
          >{{ label
          }}<span class="isRequiredAsterisk" style="color: red">*</span>
        </Label>

        <Input
          v-bind="props"
          :id="props.id"
          :name="props.name"
          :placeholder="computedPlaceholder"
          :valid="!(props.touched && errorMessage)"
          :value="value"
          @input="onInput"
        />

        <Error v-if="props.touched && errorMessage">{{ errorMessage }}</Error>
      </StackLayout>
    </template>
  </Field>
</template>

<style scoped></style>
