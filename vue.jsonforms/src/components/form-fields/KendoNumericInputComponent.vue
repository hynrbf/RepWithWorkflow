<script lang="ts">
import { defineComponent, inject, PropType } from "vue";
import { NumericTextBoxChangeEvent } from "@progress/kendo-vue-inputs";
import { AppConstants } from "@/infra/AppConstants";
import { Emitter, EventType } from "mitt";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";

export default defineComponent({
  name: "KendoNumericInputComponent",
  props: {
    id: String,
    isRequired: {
      type: Boolean,
      default: true,
    },
    value: Number,
    defaultValue: {
      type: Number,
      default: undefined,
    },
    name: {
      type: String,
      default: "",
    },
    label: {
      type: String,
      default: "",
    },
    labelStyle: {
      type: String,
      default: "",
    },
    placeholder: {
      type: String,
      default: "",
    },
    min: {
      type: Number,
      default: 0,
    },
    minLimit: {
      type: Number,
      default: 0,
    },
    maxLimit: {
      type: Object as PropType<number | undefined>,
      default: undefined,
      validator(value: unknown): boolean {
        return value === undefined || typeof value === "number";
      },
    },
    suffixText: String,
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
    };
  },
  watch: {
    isDataLoadedCompletely(newValue, oldValue): boolean {
      if (newValue == oldValue) {
        return newValue;
      }

      this.isFieldValidOnFirstLoad =
        this.validate(this.value || this.defaultValue) == "";
      return newValue;
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

    this.isFieldValidOnFirstLoad =
      this.validate(this.value || this.defaultValue) == "";
  },
  mounted() {
    this.isUnmounted = false;

    if (this.isValueReactive && this.isDataLoadedCompletely) {
      this.isFieldValidOnFirstLoad =
        this.validate(this.value || this.defaultValue) == "";
    }
  },
  updated() {
    this.isFieldValidOnFirstLoad =
      this.validate(this.value || this.defaultValue) == "";
  },
  unmounted() {
    if (!this.id) {
      return;
    }

    this.removeValidationValueByFieldId(this.id);
    this.isUnmounted = true;
  },
  methods: {
    onValueChange(event: NumericTextBoxChangeEvent) {
      this.isFieldValidOnFirstLoad = false;
      this.$emit("onValueChange", event.value);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    },

    validate(currentValue?: number): string {
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

      if (!currentValue || currentValue < this.minLimit) {
        this.addIdKeyAndErrorValue(errorMessage);
        this.errorMessage = errorMessage;
        return errorMessage;
      }

      if (this.maxLimit) {
        if (currentValue > this.maxLimit) {
          this.addIdKeyAndErrorValue(errorMessage);
          this.errorMessage = errorMessage;
          return errorMessage;
        }
      }

      this.addIdKeyAndErrorValue("");
      return "";
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
  <div>
    <Field
      :name="name"
      :id="id"
      :validator="validate"
      :component="'myTemplate'"
    >
      <template v-slot:myTemplate="{ props }">
        <StackLayout orientation="vertical" :gap="8">
          <Label
            :editor-id="props.id"
            class="control-label"
            :style="labelStyle"
          >
            {{ label }}
            <span v-if="!isRequired" class="fineprint ms-1"> (Optional) </span>
          </Label>
          <NumericTextBox
            v-bind="props"
            :id="props.id"
            :name="props.name"
            :placeholder="placeholder"
            :valid="!(props.touched && errorMessage)"
            :default-value="defaultValue"
            :value="value"
            :min="min"
            :input-suffix="suffixText && 'suffix'"
            @change="onValueChange"
            :fillMode="null"
          >
            <template v-if="suffixText" #suffix>
              {{ suffixText }}
            </template>
          </NumericTextBox>

          <Error v-if="props.touched && errorMessage"
            >{{ errorMessage }}
          </Error>
        </StackLayout>
      </template>
    </Field>
  </div>
</template>

<style scoped></style>
