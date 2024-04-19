<script lang="ts">
import { defineAsyncComponent, defineComponent, inject } from "vue";
import { AppConstants } from "@/infra/AppConstants";
import { Emitter, EventType } from "mitt";
import { container } from "tsyringe";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";

export default defineComponent({
  name: "KendoNumericInputComponentWithIcon",
  props: {
    id: String,
    isRequired: {
      type: Boolean,
      default: true,
    },
    value: {
      type: Number,
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
    iconPath: {
      type: String,
      default: "",
    },
    min: {
      type: Number,
      default: 0,
    },
    hasIcon: {
      type: Boolean,
      default: false,
    },
    isInputDisabled: {
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
  components: {
    KendoNumericTextInputComponent: defineAsyncComponent(
      () =>
        import("@/components/form-fields/KendoNumericTextInputComponent.vue"),
    ),
  },
  data() {
    return {
      isFieldValidOnFirstLoad: false,
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      valueInternal: "",
      debounceTimer: null as NodeJS.Timeout | null,
      errorMessage: ""
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
  created() {
    if (this.isValueReactive) {
      return;
    }

    this.valueInternal = this.value?.toString();
    this.isFieldValidOnFirstLoad = this.validate(this.value) == "";
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
    validate(currentValue: number): string {
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

      let errorMessage = `Please enter a valid ${this.label}`;

      if (!this.isRequired) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (!currentValue) {
        this.addIdKeyAndErrorValue(errorMessage);
        this.errorMessage = errorMessage;
        return errorMessage;
      }

      if (currentValue <= 0) {
        this.addIdKeyAndErrorValue(errorMessage);
        this.errorMessage = errorMessage;
        return errorMessage;
      }

      this.addIdKeyAndErrorValue("");
      return "";
    },

    onValueChange(value: string) {
      if (this.debounceTimer) {
        clearTimeout(this.debounceTimer);
      }

      const oldValue = this.valueInternal;
      this.valueInternal = value;
      this.debounceTimer = setTimeout(() => {
        this.isFieldValidOnFirstLoad = false;
        let safeNo = this.helperService.convertToSafeNumber(this.valueInternal);

        if (safeNo < this.min) {
          this.valueInternal = oldValue;
          return;
        }

        this.$emit("onInputValueChange", safeNo);
        this.eventBus.emit(AppConstants.formFieldChangedEvent);
        this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
      }, 1000);
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
  <Field :name="name" :id="id" :validator="validate" :component="'myTemplate'">
    <template v-slot:myTemplate="{ props }">
      <StackLayout
        class="frame"
        orientation="horizontal"
        :align="{ horizontal: 'start' }"
      >
        <img v-if="hasIcon" :src="iconPath" alt="icon" />

        <Label :editor-id="props.id" class="description">
          {{ label }}
        </Label>

        <!--TODO. to confirm if validation error is needed here :minimum-value="min" -->
        <KendoNumericTextInputComponent
          v-bind="props"
          :id="props.id"
          :name="props.name"
          :placeholder="placeholder"
          :valid="!(props.touched && errorMessage)"
          :isInputDisabled="isInputDisabled"
          :value="valueInternal"
          @onValueChange="onValueChange"
        />
      </StackLayout>
    </template>
  </Field>
</template>

<style scoped>
.frame {
  gap: 15px;
  padding: 20px;
  border: 0.3px solid var(--text-text-disabled);
  border-radius: 8px;
  background: var(--screen-background-background-primary);
}

.description {
  flex-grow: 1;
  color: var(--text-text-primary);
  font-size: var(--font-size-default);
  text-align: justify;
  font-style: normal;
  font-weight: var(--font-weight-medium);
  line-height: 130%; /* 20.8px */
}
</style>