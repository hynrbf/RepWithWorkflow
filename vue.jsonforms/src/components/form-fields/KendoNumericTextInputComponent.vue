<script lang="ts">
import { defineComponent, inject } from "vue";
import { Emitter, EventType } from "mitt";
import { AppConstants } from "@/infra/AppConstants";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";

export default defineComponent({
  name: "KendoNumericTextInputComponent",
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
      default: "",
    },
    minimumValue: {
      type: Number,
      default: 0,
    },
    maskFormat: {
      type: String,
      default: "",
    },
    isInputDisabled: {
      type: Boolean,
      default: false,
    },
    acceptDash: {
      type: Boolean,
      default: false,
    },
    mask: {
      type: String,
      default: "",
    },
    maxlength: {
      type: String,
      default: "",
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
  data() {
    return {
      isFieldValidOnFirstLoad: false,
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      isUnmounted: false,
      errorMessage: "",
      validNumericInput: "[^0-9]",
      formattedValue: "",
    };
  },
  watch: {
    isDataLoadedCompletely(newValue, oldValue): boolean {
      if (newValue == oldValue) {
        return newValue;
      }

      this.isFieldValidOnFirstLoad = this.validate(this.value) == "";
      return newValue;
    },
    value(newValue) {
      this.formattedValue = newValue;
    },
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
  created() {
    if (this.isValueReactive) {
      return;
    }

    this.isFieldValidOnFirstLoad = this.validate(this.value) == "";
  },
  mounted() {
    this.isUnmounted = false;
  },
  unmounted() {
    if (!this.id) {
      return;
    }

    this.removeValidationValueByFieldId(this.id);
    this.isUnmounted = true;
  },
  methods: {
    onInput(event: any) {
      if (this.acceptDash) {
        this.validNumericInput = "[^0-9-]";
      }

      const regex = new RegExp(this.validNumericInput, "g");

      this.isFieldValidOnFirstLoad = false;
      let value = event.target.value;
      value = value.replace(regex, "");

      this.$emit("onValueChange", value);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    },

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

      let errorMessage = `Please enter a valid ${this.label}`;

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

      if (this.maskFormat.toLowerCase() == "money") {
        currentValue = this.unFormatMoney(currentValue);
      }

      let numericValue = parseFloat(currentValue);

      if (numericValue < this.minimumValue) {
        this.addIdKeyAndErrorValue(errorMessage);
        this.errorMessage = errorMessage;
        return errorMessage;
      }

      if (!currentValue) {
        this.addIdKeyAndErrorValue(errorMessage);
        this.errorMessage = errorMessage;
        return errorMessage;
      }

      if (this.mask != "" && !this.validateMaskInput(currentValue)) {
        this.addIdKeyAndErrorValue(errorMessage);
        this.errorMessage = `Please enter valid ${this.label}`;
        return errorMessage;
      }

      this.addIdKeyAndErrorValue("");
      return "";
    },

    formatMask(input: string): string {
      if (this.maskFormat.toLowerCase() == "money") {
        return this.formatMoney(input);
      }

      return input;
    },

    formatMoney(input: string): string {
      const numberValue = parseFloat(input);

      if (isNaN(numberValue)) {
        this.errorMessage = `Invalid number ${input} in numeric control`;
        return input;
      }

      const formattedMoney = new Intl.NumberFormat("en-GB", {
        style: "decimal",
        minimumFractionDigits: 0,
        maximumFractionDigits: 0,
      }).format(numberValue);

      return formattedMoney.toString();
    },

    unFormatMoney(formattedMoney: string): string {
      if (!formattedMoney) {
        return formattedMoney;
      }

      return formattedMoney.replace(/[^0-9.]/g, "");
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

    validateMaskInput(text: string): boolean {
      return /^\d{2}-\d{2}-\d{2}$/.test(text);
    },
  },
});
</script>

<template>
  <div>
    <Field
      :name="name"
      :id="id"
      :validator="validate"
      :component="'myTemplate'"
    >
      <template v-slot:myTemplate="{ props }">
        <StackLayout orientation="vertical" style="gap: 6px">
          <Label v-if="label" :editor-id="props.id" class="control-label">
            {{ label }}
            <span v-if="!isRequired" class="fineprint ms-1"> (Optional) </span>
          </Label>

          <KendoMaskedTextBox
            v-if="mask"
            v-bind="props"
            :fillMode="'solid'"
            :id="props.id"
            v-model="formattedValue"
            :placeholder="placeholder"
            :mask="mask"
            :valid="!(props.touched && errorMessage)"
            @input="onInput"
          />

          <Input
            v-else
            v-bind="props"
            :fillMode="'solid'"
            :id="props.id"
            :name="props.name"
            :placeholder="placeholder"
            :valid="!(props.touched && errorMessage)"
            :value="formatMask(value)"
            :disabled="isInputDisabled"
            :maxlength="maxlength"
            size="small"
            @input="onInput"
          />

          <Error class="error" v-if="props.touched && errorMessage"
            >{{ errorMessage }}
          </Error>
        </StackLayout>
      </template>
    </Field>
  </div>
</template>

<style scoped></style>
