<script lang="ts">
import { defineComponent, inject } from "vue";
import { Field } from "@progress/kendo-vue-form";
import { Label, Error } from "@progress/kendo-vue-labels";
import { Input } from "@progress/kendo-vue-inputs";
import UkFlagComponent from "../../components/UkFlagComponent.vue";
import { StackLayout } from "@progress/kendo-vue-layout";
import { Emitter, EventType } from "mitt";
import { AppConstants } from "@/infra/AppConstants";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";

export default defineComponent({
  name: "KendoContactNumberInputComponent",
  components: { Field, StackLayout, Label, Error, Input, UkFlagComponent },
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
      default: "Contact Number",
    },
    placeholder: {
      type: String,
      default: "00000000000",
    },
    maxLength: {
      type: Number,
      default: 11,
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
      valueInternal: "",
      maxLengthInternal: 0,
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      errorMessage: "",
    };
  },
  computed: {
    computedPlaceholder() {
      if (!this.$t("common-contactnumber-text")) {
        return this.placeholder;
      }

      return this.$t("common-contactnumber-text");
    },
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
  updated() {
    this.valueInternal = this.value;
  },
  created() {
    this.maxLengthInternal = this.maxLength;
    this.calculateMaxLength(this.value);
    this.onAutoUkNumberFormat(this.value);

    if (this.isValueReactive) {
      return;
    }

    this.isFieldValidOnFirstLoad = this.validate(this.valueInternal) == "";
  },
  setup() {
    const pageComponentValidationValueStore =
      usePageComponentValidationValueStore();
    const { addComponentValidationValue } = pageComponentValidationValueStore;

    return {
      addComponentValidationValue,
    };
  },
  methods: {
    validate(currentValue: string): string {
      this.errorMessage = "";

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

      if (!currentValue || currentValue.length < 10) {
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
      let value = event.target.value;
      this.calculateMaxLength(value);
      this.onAutoUkNumberFormat(value);
    },

    calculateMaxLength(value: string) {
      if (!value) {
        return;
      }

      let firstChar: string = value[0];

      if (firstChar == "0" && this.maxLengthInternal != 11) {
        this.maxLengthInternal = 11;
      }

      if (firstChar != "0" && this.maxLength != 10) {
        this.maxLengthInternal = 10;
      }
    },

    // ref. https://grantify.io/eic-book-a-call/
    onAutoUkNumberFormat(value: string) {
      if (value.length === this.maxLengthInternal) {
        // the last digit is supplied
        if (value[0] === "0") {
          // remove the first zero
          value = value.substring(1);
        }

        value = value.replace(/(\d{4})(\d{6})/, "$1 $2");
      } else {
        value = value.replaceAll(" ", "");
      }

      this.valueInternal = value;
      this.$emit("onValueChange", this.valueInternal);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
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
  <!--needs to enclose in div otherwise bootstrap style outside, wont applied correctly -->
  <div>
    <Field
      :name="name"
      :id="id"
      :validator="validate"
      :component="'myTemplate'"
    >
      <template v-slot:myTemplate="{ props }">
        <StackLayout orientation="vertical" :gap="10">
          <Label :editor-valid="props.valid" class="control-label">
            {{ label }}
            <span v-if="!isRequired" class="fineprint ms-1"> (Optional) </span>
          </Label>

          <StackLayout
            orientation="horizontal"
            class="px-3 input-container"
            :class="props.touched && errorMessage ? 'input-error' : ''"
            :align="{ horizontal: 'start', vertical: 'middle' }"
            :gap="4"
          >
            <UkFlagComponent />

            <div class="col-auto">
              <Label>+44 (0)</Label>
            </div>

            <Input
              v-bind="props"
              :id="props.id"
              :name="props.name"
              :placeholder="computedPlaceholder"
              :valid="!(props.touched && errorMessage)"
              :value="valueInternal"
              @input="onInput"
              fill-mode="none"
              class="input"
              size="small"
              :maxlength="maxLengthInternal.toString()"
            />
          </StackLayout>

          <Error class="error" v-if="props.touched && errorMessage"
            >{{ errorMessage }}
          </Error>
        </StackLayout>
      </template>
    </Field>
  </div>
</template>

<style scoped>
.input {
  border-color: transparent;
  padding-left: 0;
  background-color: white;
  color: var(--text-text-primary);
  /* font-family: Figtree; */
  font-size: var(--font-size-default);
  font-style: normal;
  font-weight: var(--font-weight-normal);
  line-height: 130%; /* 20.8px */
}

.input-error {
  border-color: var(--error-color-error-primary);
  padding-left: 0;
}

.error {
  color: var(--error-color-error-primary);
  /* font-family: Figtree; */
  font-size: var(--font-size-sm);
  font-style: normal;
  font-weight: var(--font-weight-normal);
  line-height: 125%; /* 17.5px */
}
</style>