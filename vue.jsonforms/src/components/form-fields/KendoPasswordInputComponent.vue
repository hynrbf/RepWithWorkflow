<script lang="ts">
import { defineComponent, inject } from "vue";
import { Emitter, EventType } from "mitt";
import { StackLayout } from "@progress/kendo-vue-layout";
import { AppConstants } from "@/infra/AppConstants";
import IconComponent from "@/components/IconComponent.vue";
import {usePageComponentValidationValueStore} from "@/stores/progress-bar/usePageComponentValidationValueStore";
import {usePageFieldsInvalidHandlerStore} from "@/stores/usePageFieldsInvalidHandlerStore";

export default defineComponent({
  name: "KendoPasswordInputComponent",
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
      default: "******",
    },
    maxLength: {
      type: Number,
      default: 100,
    },
    customValidationMessageError: {
      type: String,
      default: "",
    },
    customValidationMessageSuccess: {
      type: String,
      default: "",
    },
    isForConfirmPassword: {
      type: Boolean,
      default: false,
    },
    passwordToCompare: {
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
  components: {
    IconComponent,
    StackLayout,
  },
  data() {
    return {
      isFieldValidOnFirstLoad: false,
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      isPasswordShown: false,
      isShowSuccessValidationMessage: false,
      borderClass: "",
      errorMessage: "",
      isUnmounted: false,
    };
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

    isShowSuccessValidationMessage(newValue, _oldValue): boolean {
      this.borderClass = newValue
        ? "password-strong-container-border"
        : "password-weak-container-border";
      return newValue;
    },
  },
  created() {
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
      this.$emit("onValueChange", event.target.value);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    },

    togglePassword() {
      return (this.isPasswordShown = !this.isPasswordShown);
    },

    isPasswordAtLeast8Chars(input: string): boolean {
      if (!input) {
        return false;
      }

      return input.length >= 8;
    },

    isPasswordHaveAtLeastOneLetter(input: string): boolean {
      if (!input) {
        return false;
      }

      return /[a-zA-Z]/.test(input);
    },

    isPasswordHaveAtLeastOneNumberOrSpecialCase(input: string) {
      if (!input) {
        return false;
      }

      const hasNumber: boolean = /\d/.test(input);
      const hasSpecialCharacter: boolean = /[!@#$%^&*(),.?":{}|<>_]/.test(
        input,
      );

      return hasNumber || hasSpecialCharacter;
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

    validate(currentValue: string): string {
      this.errorMessage = "";

      if (this.isUnmounted){
        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (this.isValueReactive && !this.isDataLoadedCompletely) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      this.isShowSuccessValidationMessage = false;

      if (this.isFieldValidOnFirstLoad) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (this.isForConfirmPassword) {
        if (!(this.passwordToCompare && currentValue)) {
          this.errorMessage = this.customValidationMessageError;
          this.addIdKeyAndErrorValue(this.customValidationMessageError);
          return this.customValidationMessageError;
        }

        if (this.passwordToCompare === currentValue) {
          this.isShowSuccessValidationMessage = true;
          this.addIdKeyAndErrorValue("");
          return "";
        }

        this.errorMessage = this.customValidationMessageError;
        this.addIdKeyAndErrorValue(this.customValidationMessageError);
        return this.customValidationMessageError;
      }

      if (!currentValue && this.value) {
        currentValue = this.value;
      }

      if (!this.isRequired) {
        this.addIdKeyAndErrorValue("");
        this.clearInvalidEntries(this.id);
        return "";
      }

      if (!currentValue) {
        this.errorMessage = this.customValidationMessageError;
        this.addIdKeyAndErrorValue(this.customValidationMessageError);
        this.clearInvalidEntries(this.id);
        return this.customValidationMessageError;
      }

      if (
        !(
          this.isPasswordAtLeast8Chars(currentValue) &&
          this.isPasswordHaveAtLeastOneLetter(currentValue) &&
          this.isPasswordHaveAtLeastOneNumberOrSpecialCase(currentValue)
        )
      ) {
        this.errorMessage = this.customValidationMessageError;
        this.addIdKeyAndErrorValue(this.customValidationMessageError);
        this.addInvalidFieldEntry(this.id);
        return this.customValidationMessageError;
      } else {
        this.isShowSuccessValidationMessage = true;
        this.addIdKeyAndErrorValue("");
        this.clearInvalidEntries(this.id);
        return "";
      }
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
            v-if="label"
            :editor-id="props.id"
            class="control-label paragraph-s-figtree-medium"
          >
            {{ label }}
            <span v-if="!isRequired" class="fineprint ms-1"> (Optional) </span>
          </Label>

          <StackLayout
            orientation="horizontal"
            class="input-container"
            :class="borderClass"
            :align="{ horizontal: 'start', vertical: 'middle' }"
            :gap="4"
          >
            <Input
              v-bind="props"
              class="control"
              :id="props.id"
              :name="props.name"
              :placeholder="placeholder"
              :maxlength="maxLength.toString()"
              :valid="!(props.touched && errorMessage)"
              :value="value"
              :type="isPasswordShown ? 'text' : 'password'"
              @input="onInput"
              style="
                background-color: transparent;
                border-color: transparent;
                box-shadow: none;
              "
            />

            <IconComponent
              symbol="eye"
              size="30"
              @click="togglePassword"
              style="cursor: pointer; margin-right: 8px"
            />
          </StackLayout>

          <Error
            class="paragraph-s-figtree-semi-bold password-weak"
            v-if="props.touched && errorMessage"
            >{{ errorMessage }}
          </Error>

          <Label
            v-else-if="isShowSuccessValidationMessage"
            class="paragraph-s-figtree-semi-bold password-strong"
          >
            {{ customValidationMessageSuccess }}
          </Label>
        </StackLayout>
      </template>
    </Field>
  </div>
</template>

<style scoped>
.password-weak-container-border {
  border-color: var(--color-error-600);
}

.password-strong-container-border {
  border-color: var(--color-success-700);
}

.control {
  color: var(--text-text-primary);
  /* font-family: Figtree; */
  font-size: var(--font-size-default);
  font-style: normal;
  font-weight: var(--font-weight-normal);
  line-height: 130%; /* 20.8px */
}

.paragraph-s-figtree-semi-bold {
  /* font-family: Figtree; */
  font-size: 14px;
  font-style: normal;
  font-weight: 600;
  line-height: 125%; /* 17.5px */
}

.password-strong {
  color: var(--color-success-700);
}

.password-weak {
  color: var(--color-error-600);
}
</style>