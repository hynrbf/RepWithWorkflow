<script lang="ts">
import { defineComponent, inject, PropType } from "vue";
import { Emitter, EventType } from "mitt";
import { AppConstants } from "@/infra/AppConstants";
import { NumericTextBoxChangeEvent } from "@progress/kendo-vue-inputs";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";
import { usePageFieldsInvalidHandlerStore } from "@/stores/usePageFieldsInvalidHandlerStore";

export default defineComponent({
  name: "KendoNumericStepperWithSuffixComponent",
  props: {
    id: String,
    isRequired: {
      type: Boolean,
      default: true,
    },
    value: {
      type: Object as PropType<number | null>,
      default: null,
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
      default: "0",
    },
    suffix: {
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

      this.isFieldValidOnFirstLoad = this.validate(this.value) == "";
      return newValue;
    },
  },
  setup() {
    const pageComponentValidationValueStore =
      usePageComponentValidationValueStore();
    const { addComponentValidationValue, removeValidationValueByFieldId } =
      pageComponentValidationValueStore;
    const { addInvalidFieldEntry, clearInvalidEntries } =
      usePageFieldsInvalidHandlerStore();

    return {
      addComponentValidationValue,
      removeValidationValueByFieldId,
      addInvalidFieldEntry,
      clearInvalidEntries,
    };
  },
  created() {
    if (this.isValueReactive) {
      return;
    }

    this.isFieldValidOnFirstLoad = this.validate(this.value) == "";
  },
  mounted() {
    if (this.id) {
      this.eventBus.on(
        `${AppConstants.pageFieldInvalidEvent}-${this.id}`,
        () => {
          // Replace invalid value with null
          this.emitValue(null);
          this.clearInvalidEntries(this.id);
        },
      );
    }

    this.isUnmounted = false;
    this.replaceDefaultIcon("k-svg-i-caret-alt-up", "k-i-chevron-up");
    this.replaceDefaultIcon("k-svg-i-caret-alt-down", "k-i-chevron-down");

    if (this.isValueReactive && this.isDataLoadedCompletely) {
      this.isFieldValidOnFirstLoad = this.validate(this.value) == "";
    }
  },
  updated() {
    this.isFieldValidOnFirstLoad = this.validate(this.value) == "";
  },
  unmounted() {
    if (!this.id) {
      return;
    }

    this.removeValidationValueByFieldId(this.id);
    this.isUnmounted = true;
  },
  methods: {
    replaceDefaultIcon(iconToReplace: string, iconClass: string) {
      let spinnerIcon = document.querySelector(
        `.k-svg-icon.${iconToReplace}.k-button-icon`,
      ) as HTMLSpanElement;

      if (!spinnerIcon) {
        return;
      }

      if (spinnerIcon.classList?.length < 1 ?? true) {
        return;
      }

      spinnerIcon.classList.remove("k-svg-icon");
      spinnerIcon.classList.remove(iconToReplace);

      spinnerIcon.classList.add("k-icon");
      spinnerIcon.classList.add(iconClass);
      spinnerIcon.style.scale = "1.25"; // ESTIMATED width and height = 20px
    },

    onValueChange(event: NumericTextBoxChangeEvent) {
      this.emitValue(event.value);
      this.validate(this.value);
    },

    emitValue(value: number | null) {
      this.isFieldValidOnFirstLoad = false;
      this.$emit("onValueChange", value);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      // this.eventBus.emit(AppConstants.formFieldChangedEventForSaving);
    },

    validate(currentValue: number | null): string {
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

      if (!this.isRequired) {
        this.addIdKeyAndErrorValue("");
        this.clearInvalidEntries(this.id);
        return "";
      }

      if (currentValue === null || currentValue < this.minLimit) {
        this.addIdKeyAndErrorValue(errorMessage);
        this.addInvalidFieldEntry(this.id);
        this.errorMessage = errorMessage;
        return errorMessage;
      }

      if (this.maxLimit) {
        if (currentValue > this.maxLimit) {
          this.addIdKeyAndErrorValue(errorMessage);
          this.addInvalidFieldEntry(this.id);
          this.errorMessage = errorMessage;
          return errorMessage;
        }
      }

      this.addIdKeyAndErrorValue("");
      this.clearInvalidEntries(this.id);
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
            class="k-percentage-input-component flex-row-reverse"
            v-bind="props"
            :id="props.id"
            :name="props.name"
            :placeholder="placeholder"
            :valid="!(props.touched && errorMessage)"
            :value="value"
            :min="min"
            input-prefix="prefix"
            style="border-radius: 8px"
            :spinners="true"
            @change="onValueChange"
          >
            <template v-slot:prefix>
              <div>
                <span class="suffix-container d-flex justify-content-center">{{
                  suffix
                }}</span>
              </div>
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

<style scoped>
.suffix-container {
  background: var(--content-content-10);
  border-radius: 0 8px 8px 0;
  color: var(--text-text-disabled);
  padding: 0 4px;
}
</style>