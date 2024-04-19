<script lang="ts">
import { defineComponent, inject } from "vue";
import { Emitter, EventType } from "mitt";
import { AppConstants } from "@/infra/AppConstants";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";
export default defineComponent({
  name: "KendoTextAreaInputComponent",
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
    labelForErrorMessage: {
      type: String,
      default: "",
    },
    placeholder: {
      type: String,
      default: "Type",
    },
    cornerRadius: {
      type: Number,
      default: 0,
    },
    maxLength: {
      type: Number,
      default: 200,
    },
    rows: {
      type: Number,
      default: 3,
    },
    isOverrideCommonPlaceHolderText: {
      type: Boolean,
      default: false,
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
      labelForErrorMessageInternal: "",
      isUnmounted: false,
      errorMessage: "",
    };
  },
  computed: {
    computedPlaceholder() {
      if (!this.$t("common-placeholder-text-area")) {
        return this.placeholder;
      }

      if (this.isOverrideCommonPlaceHolderText && this.placeholder) {
        return this.placeholder;
      }

      return this.$t("common-placeholder-text-area");
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
    if (this.labelForErrorMessage) {
      this.labelForErrorMessageInternal = this.labelForErrorMessage;
    } else {
      this.labelForErrorMessageInternal = this.label;
    }

    if (this.isValueReactive) {
      return;
    }

    this.isFieldValidOnFirstLoad = this.validate(this.value) == "";
  },
  mounted() {
    this.isUnmounted = false;

    if (this.isValueReactive && this.isDataLoadedCompletely) {
      this.isFieldValidOnFirstLoad = this.validate(this.value) == "";
    }
  },
  unmounted() {
    if (!this.id) {
      return;
    }

    this.removeValidationValueByFieldId(this.id);
    this.isUnmounted = true;
  },
  methods: {
    validate(currentValue: string): string {
      this.errorMessage = "";

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
        setTimeout(() => (currentValue = this.value), 0);
      }

      if (!this.isRequired) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (!currentValue) {
        let errorMessage = `Please enter a valid ${this.labelForErrorMessageInternal}`;
        this.addIdKeyAndErrorValue(errorMessage);
        this.errorMessage = errorMessage;
        return errorMessage;
      }

      this.addIdKeyAndErrorValue("");
      return "";
    },

    onInput(event: any) {
      this.isFieldValidOnFirstLoad = false;
      if (event) {
        this.$emit("onValueChange", event.value);
      }
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
  <div>
    <Field
      :name="name"
      :id="id"
      :validator="validate"
      :component="'myTemplate'"
    >
      <template v-slot:myTemplate="{ props }">
        <StackLayout orientation="vertical" style="gap: 6px;">
          <Label :editor-id="props.id" class="control-label">
            {{ label }}
            <span v-if="!isRequired" class="fineprint ms-1"> (Optional) </span>
          </Label>

          <!--ToDo. this has issue in getting data based from @change or @input.
              So making this required will tell progress computation that it has an error.-->
          <TextArea
            :id="props.id"
            v-bind="props"
            :name="props.name"
            :placeholder="computedPlaceholder"
            :valid="!(props.touched && errorMessage)"
            :value="value"
            @input="onInput($event)"
            :maxlength="maxLength.toString()"
            :class="props.touched && errorMessage ? 'text-field-invalid' : ''"
            :style="`border-radius: ${cornerRadius}px; padding: 8px 8px 8px 12px; box-shadow: none;`"
            :rows="rows"
          />

          <Error style="margin-top: 4px" v-if="props.touched && errorMessage"
            >{{ errorMessage }}
          </Error>
        </StackLayout>
      </template>
    </Field>
  </div>
</template>

<style scoped>
  .text-field-invalid {
    border-color: red;
  }
</style>