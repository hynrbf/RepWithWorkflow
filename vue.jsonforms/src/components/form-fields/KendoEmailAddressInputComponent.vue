<script lang="ts">
import { defineComponent, inject } from "vue";
import { AppConstants } from "@/infra/AppConstants";
import { Emitter, EventType } from "mitt";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";
import { usePageFieldsInvalidHandlerStore } from "@/stores/usePageFieldsInvalidHandlerStore";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import { container } from "tsyringe";

export default defineComponent({
  name: "KendoEmailAddressInputComponent",
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
      default: "Email Address",
    },
    placeholder: {
      type: String,
      default: "Type",
    },
    maxLength: {
      type: Number,
      default: 100,
    },
    modelValue: {
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
    isEditable: {
      type: Boolean,
      default: true,
    },
  },
  data() {
    return {
      isFieldValidOnFirstLoad: false,
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      isUnmounted: false,
      errorMessage: "",
    };
  },
  computed: {
    computedPlaceholder() {
      if (!this.$t("common-email-text")) {
        return this.placeholder;
      }

      return this.$t("common-email-text");
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
          // Replace invalid value with empty string
          this.emitValue("");
          this.clearInvalidEntries(this.id);
        },
      );
    }

    this.isUnmounted = false;

    if (this.isValueReactive && this.isDataLoadedCompletely) {
      this.isFieldValidOnFirstLoad = this.validate(this.value) == "";
    }
  },
  unmounted() {
    if (!this.id) {
      return;
    }

    this.eventBus.off(`${AppConstants.pageFieldInvalidEvent}-${this.id}`);
    this.removeValidationValueByFieldId(this.id);
    this.isUnmounted = true;
  },
  setup() {
    const pageComponentValidationValueStore =
      usePageComponentValidationValueStore();
    const { addComponentValidationValue, removeValidationValueByFieldId } =
      pageComponentValidationValueStore;

    const pageFieldsInvalidHandlerStore = usePageFieldsInvalidHandlerStore();
    const { addInvalidFieldEntry, clearInvalidEntries } =
      pageFieldsInvalidHandlerStore;

    return {
      addComponentValidationValue,
      removeValidationValueByFieldId,
      addInvalidFieldEntry,
      clearInvalidEntries,
    };
  },
  methods: {
    onInput(event: any) {
      if (!this.isEditable) {
        this.$emit("onAttemptEdit");
        return;
      }

      this.emitValue(event.target.value);
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

      if (this.isFieldValidOnFirstLoad) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (this.isValueReactive && !currentValue && this.value) {
        currentValue = this.value;
      }

      let curValue = currentValue;

      if (!this.isRequired) {
        this.addIdKeyAndErrorValue("");
        this.clearInvalidEntries(this.id);
        return "";
      }

      if (!curValue) {
        const errorText = `Please enter a valid ${this.label}`;
        this.addIdKeyAndErrorValue(errorText);
        this.clearInvalidEntries(this.id);
        this.errorMessage = errorText;
        return errorText;
      }

      const isValidEmailFormat =
        this.helperService.checkIfEmailFormatIsValid(curValue);
      const errorMessage = isValidEmailFormat
        ? ""
        : `Please enter a valid ${this.label}`;
      this.addIdKeyAndErrorValue(errorMessage);

      if (isValidEmailFormat) {
        this.clearInvalidEntries(this.id);
      } else {
        this.errorMessage = errorMessage;
        this.addInvalidFieldEntry(this.id);
      }

      return errorMessage;
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

    emitValue(updatedValue: string) {
      this.isFieldValidOnFirstLoad = false;
      this.$emit("onValueChange", updatedValue);
      this.$emit("update:modelValue", updatedValue);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
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
          <Label :editor-id="props.id" class="control-label">
            {{ label }}
            <span v-if="!isRequired" class="fineprint ms-1"> (Optional) </span>
          </Label>

          <Input
            v-bind="props"
            class="control"
            :id="props.id"
            style="border-radius: 8px"
            :name="props.name"
            :placeholder="computedPlaceholder"
            :valid="!(props.touched && errorMessage)"
            :value="modelValue || value"
            @input="onInput"
            type="email"
            size="small"
            :maxlength="maxLength.toString()"
            :fillMode="'solid'"
          />

          <Error class="error" v-if="props.touched && errorMessage"
            >{{ errorMessage }}
          </Error>
        </StackLayout>
      </template>
    </Field>
  </div>
</template>

<style scoped />