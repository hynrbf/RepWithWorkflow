<script lang="ts">
import { defineComponent, inject } from "vue";
import { container } from "tsyringe";
import {
  IProfanityService,
  IProfanityServiceInfo,
} from "@/infra/dependency-services/rest/profanity/IProfanityService";
import { Emitter, EventType } from "mitt";
import { AppConstants } from "@/infra/AppConstants";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";
import { usePageFieldsInvalidHandlerStore } from "@/stores/usePageFieldsInvalidHandlerStore";

export default defineComponent({
  name: "KendoGenericInputComponent",
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
    isCheckForProfanity: {
      type: Boolean,
      default: true,
    },
    isCapitalizeFirstLetter: {
      type: Boolean,
      default: false,
    },
    maxLength: {
      type: Number,
      default: 100,
    },
    type: {
      type: String,
      default: "text",
    },
    modelValue: {
      type: String,
      default: "",
    },
    customStyle: {
      type: Object,
      default: null,
    },
    optionalText: {
      type: String,
      default: "(Optional)",
    },
    isByPassGenericPlaceHolder: {
      type: Boolean,
      default: false,
    },
    inputClass: {
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
      profanityService: container.resolve<IProfanityService>(
        IProfanityServiceInfo.name,
      ),
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      isUnmounted: false,
      errorMessage: "",
    };
  },
  computed: {
    computedPlaceholder() {
      if (
        (this.isByPassGenericPlaceHolder && this.placeholder) ||
        !this.$t("common-placeholder-text")
      ) {
        return this.placeholder;
      }

      return this.$t("common-placeholder-text");
    },
  },
  watch: {
    isRequired(newValue, _oldValue): boolean {
      if (this.isFieldValidOnFirstLoad) {
        this.isFieldValidOnFirstLoad = false;
      }

      return newValue;
    },

    isDataLoadedCompletely(newValue, oldValue): boolean {
      if (newValue == oldValue) {
        return newValue;
      }

      this.isFieldValidOnFirstLoad = this.validate(this.value) == "";
      return newValue;
    },
  },
  created() {
    // below is not working in 'mounted' thus putting here
    if (this.type === "url" && this.id) {
      this.eventBus.on(
        `${AppConstants.pageFieldInvalidEvent}-${this.id}`,
        () => {
          // Replace invalid value with empty string
          this.emitValue("");
          this.clearInvalidEntries(this.id);
        },
      );
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
  updated() {
    this.isFieldValidOnFirstLoad = this.validate(this.value) == "";
  },
  unmounted() {
    if (!this.id) {
      return;
    }

    if (this.type === "url") {
      this.eventBus.off(`${AppConstants.pageFieldInvalidEvent}-${this.id}`);
    }

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
      addInvalidFieldEntry,
      clearInvalidEntries,
      removeValidationValueByFieldId,
    };
  },
  methods: {
    onInput(event: any) {
      this.isFieldValidOnFirstLoad = false;
      this.emitValue(event.target.value);
    },

    capitalizeFirstLetter(word: string): string {
      if (!word || word.length < 1) {
        return "";
      }

      if (!this.isCapitalizeFirstLetter) {
        return word;
      }

      // will work only for firstName and lastName only
      return `${word[0].toUpperCase()}${word.substring(1)}`;
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

      let errorMessage = `Please enter a valid ${this.label || "input"}`;

      if (this.isFieldValidOnFirstLoad) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (this.isValueReactive && !currentValue && this.value) {
        currentValue = this.value;
      }

      if (
        currentValue &&
        this.isCheckForProfanity &&
        this.profanityService.hasOffensiveWords(currentValue)
      ) {
        this.addIdKeyAndErrorValue(errorMessage);
        this.errorMessage = errorMessage;
        return errorMessage;
      }

      if (!this.isRequired) {
        if (!currentValue) {
          this.clearInvalidEntries(this.id);
        }

        if (currentValue && this.type === "url") {
          const urlErrorMessage = this.validateUrl(currentValue);

          if (!urlErrorMessage) {
            this.clearInvalidEntries(this.id);
          } else {
            this.addInvalidFieldEntry(this.id);
          }

          this.addIdKeyAndErrorValue(urlErrorMessage);
          this.errorMessage = urlErrorMessage;
          return urlErrorMessage;
        }

        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (!currentValue) {
        this.addIdKeyAndErrorValue(errorMessage);
        this.errorMessage = errorMessage;
        this.clearInvalidEntries(this.id);
        return errorMessage;
      }

      if (this.type === "url") {
        const urlErrorMessage = this.validateUrl(currentValue);
        this.addIdKeyAndErrorValue(urlErrorMessage);

        if (!urlErrorMessage) {
          // If there is no error message, then the url is valid
          this.clearInvalidEntries(this.id);
        } else {
          this.addInvalidFieldEntry(this.id);
        }

        this.errorMessage = urlErrorMessage;
        return urlErrorMessage;
      }

      this.addIdKeyAndErrorValue("");
      return "";
    },

    validateUrl(currentValue: string): string {
      let errorMessage = `Please enter a valid ${this.label || "input"}`;

      if (currentValue.includes("https") || currentValue.includes("http")) {
        return this.isUrlGood(currentValue) ? "" : errorMessage;
      }

      //1st attempt. Try 'https'
      const currentValueWithHttps = `https://${currentValue}`;

      if (this.isUrlGood(currentValueWithHttps)) {
        return "";
      }

      //2nd attempt.  Try 'http'
      const currentValueWithHttp = `http://${currentValue}`;

      if (this.isUrlGood(currentValueWithHttp)) {
        return "";
      }

      return errorMessage;
    },

    isUrlGood(url: string): boolean {
      return this.isValidUrlFormatViaRegex(url);
      // NOTE. 'new URL(url)' only checks if the url parameter is a valid url BUT do not check if the
      // url is working or existing.
      // try {
      //   new URL(url);
      //   return true;
      // } catch {
      //   return false;
      // }

      // Tried also to validate from the back-end, which doen't make sense because
      // validating using the back-end violates the very purpose of validation
      // which is to prevent invalid data to be sent to the back-end
    },

    isValidUrlFormatViaRegex(currentValue: string): boolean {
      const urlPattern = new RegExp(
        "^(https?:\\/\\/)?" + // protocol
          "((([a-z\\d]([a-z\\d-]*[a-z\\d])*)\\.)+[a-z]{2,}|" + // domain name
          "((\\d{1,3}\\.){3}\\d{1,3}))" + // OR IP (v4) address
          "(\\:\\d+)?(\\/[-a-z\\d%_.~+]*)*" + // port and path
          "(\\?[;&a-z\\d%_.~+=-]*)?" + // query string
          "(\\#[-a-z\\d_]*)?$", // fragment locator
        "i",
      );
      return urlPattern.test(currentValue);
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
          <Label v-if="label" :editor-id="props.id" class="control-label">
            {{ label }}
            <span v-if="!isRequired" class="fineprint ms-1">
              {{ optionalText }}
            </span>

            <slot name="after-label-text"></slot>
          </Label>
          <!-- Do not put :type="type", e.g :type="url", this cause headache validation url -->
          <Input
            v-bind="props"
            :class="['control', inputClass]"
            :id="props.id"
            :name="props.name"
            :placeholder="computedPlaceholder"
            :maxlength="maxLength.toString()"
            :valid="!(props.touched && errorMessage)"
            :value="capitalizeFirstLetter(modelValue || value)"
            :fillMode="customStyle ? null : 'solid'"
            :style="customStyle ?? ''"
            style="border-radius: 8px"
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

<style scoped />