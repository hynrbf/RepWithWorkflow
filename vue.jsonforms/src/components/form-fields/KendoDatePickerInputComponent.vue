<script lang="ts">
import { defineComponent, inject } from "vue";
import { DatePickerChangeEvent } from "@progress/kendo-vue-dateinputs";
import { AppConstants } from "@/infra/AppConstants";
import { Emitter, EventType } from "mitt";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import { container } from "tsyringe";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";

export default defineComponent({
  name: "KendoDatePickerInputComponent",
  props: {
    id: String,
    isRequired: {
      type: Boolean,
      default: true,
    },
    value: {
      type: [Date, Number],
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
    placeholder: {
      type: String,
      default: "DD/MM/YYYY",
    },
    minDate: {
      type: Date,
      default: new Date(1970, 0, 1),
    },
    maxDate: {
      type: Date,
      default: new Date(3000, 0, 1),
    },
    modelValue: {
      type: [Date, Number],
      default: null,
    },
    epoch: {
      type: Boolean,
      default: false,
    },
    isShowPresentDate: {
      type: Boolean,
      default: false,
    },
    inputClass: {
      type: String,
      default: "",
    },
    origDateString: {
      type: String,
      default: undefined,
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
    internalModel: {
      type: String,
      default: "",
    },
  },
  data() {
    return {
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      isFieldValidOnFirstLoad: false,
      customFormatPlaceholder: {
        year: "YYYY",
        month: "MM",
        day: "DD",
      },
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      isChecked: false,
      isUnmounted: false,
      errorMessage: "",
    };
  },
  computed: {
    inputValue() {
      const value = this.modelValue || this.value;

      if (this.epoch) {
        return this.helperService.convertEpochToDateTime(+value);
      }

      return value;
    },
  },
  watch: {
    isDataLoadedCompletely(newValue, oldValue): boolean {
      if (newValue == oldValue) {
        return newValue;
      }

      this.isFieldValidOnFirstLoad =
        this.validate(this.inputValue as Date) == "";
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

    this.isFieldValidOnFirstLoad = this.validate(this.inputValue as Date) == "";
    this.updateIsChecked(this.inputValue);
  },
  mounted() {
    this.isUnmounted = false;

    if (this.isValueReactive && this.isDataLoadedCompletely) {
      this.isFieldValidOnFirstLoad =
        this.validate(this.inputValue as Date) == "";
    }
  },
  updated() {
    this.isFieldValidOnFirstLoad = this.validate(this.inputValue as Date) == "";
  },
  unmounted() {
    if (!this.id) {
      return;
    }

    this.isUnmounted = true;
    this.removeValidationValueByFieldId(this.id);
  },
  methods: {
    validate(currentValue: Date): string {
      this.errorMessage = "";
      //note, even if you remove the component in the DOM 'using v-if', the vue still preserves it in memory
      //that's why we need this variable
      if (this.isUnmounted || !this.isRequired) {
        return "";
      }

      if (this.isFieldValidOnFirstLoad) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (!currentValue) {
        const errorMessage = `Please enter a valid ${
          this.label || this.internalModel
        }`;
        this.addIdKeyAndErrorValue(errorMessage);
        this.errorMessage = errorMessage;
        return errorMessage;
      }

      this.addIdKeyAndErrorValue("");
      return "";
    },

    onDateChange(event: DatePickerChangeEvent) {
      this.isFieldValidOnFirstLoad = false;
      const value = event.value as Date;
      this.updateIsChecked(value);
      this.emitValue(value);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    },

    handleCheckBoxChange(): void {
      this.isChecked = !this.isChecked;

      if (this.origDateString && !this.isChecked) {
        const date = new Date(this.origDateString);
        this.emitValue(date);
        return;
      }

      this.$emit("onValueChange", null);
      this.$emit("update:modelValue", null);
    },

    //note: if is present is checked, the today's date value is passed
    //if you want it to be present word, to show in mounted hook, just handle it in your page
    //like ind controller, it pass -1 to db to tell it's present
    emitValue(value: Date): void {
      if (!value){
        if (this.epoch) {
          this.$emit("onValueChange", 0);
          this.$emit("update:modelValue", 0);
          return;
        }

        this.$emit("onValueChange", new Date(0));
        this.$emit("update:modelValue", new Date(0));
        return;
      }

      if (this.epoch) {
        const convertedValue = this.helperService.dateStringToEpoch(
          value.toDateString()
        );
        this.$emit("onValueChange", convertedValue);
        this.$emit("update:modelValue", convertedValue);
        return;
      }

      this.$emit("onValueChange", value);
      this.$emit("update:modelValue", value);
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

    updateIsChecked(inputValue: Date | number | undefined) {
      if (!this.isShowPresentDate) {
        return;
      }

      this.isChecked = !inputValue;
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
        <StackLayout
          orientation="vertical"
          :style="{ gap: label !== '' ? '6px' : '0' }"
        >
          <Label :editor-id="props.id" class="control-label">
            {{ label }}
            <span v-if="!isRequired" class="fineprint ms-1"> (Optional) </span>

            <Checkbox
              v-if="isShowPresentDate"
              :id="'presentCheckBox'"
              style="margin-left: auto"
              v-model="isChecked"
              @input="handleCheckBoxChange"
            >
              <label
                class="k-checkbox-label"
                :for="'presentCheckBox'"
                :style="{
                  marginLeft: '8px',
                  fontSize: 'var(--font-size-default)',
                  color: 'var(--text-text-secondary)',
                  fontWeight: 'var(--font-weight-medium)',
                }"
              >
                Present
              </label>
            </Checkbox>
          </Label>

          <div class="d-flex align-items-center" style="position: relative">
            <DatePicker
              :class="['k-date-picker-input-component', inputClass]"
              v-bind="props"
              size="small"
              :id="props.id"
              :name="props.name"
              :placeholder="placeholder"
              :valid="!(props.touched && errorMessage)"
              :value="inputValue"
              @change="onDateChange"
              style="padding: 0; border-radius: 8px"
              :max="maxDate"
              :min="minDate"
              :format-placeholder="customFormatPlaceholder"
              :format="'dd/MM/yyyy'"
              :toggle-button="'myTemplate'"
            >
              <template v-slot:myTemplate="{ props }">
                <toggle-button
                  style="cursor: pointer"
                  class="d-flex align-items-center date-toggle-button"
                  @click="props.onClick"
                >
                  <IconComponent
                    class="calendar-icon"
                    symbol="calendar"
                    size="20"
                  />
                </toggle-button>
              </template>
            </DatePicker>

            <span
              v-if="isShowPresentDate && isChecked"
              class="overlayed-present-text"
              >Present</span
            >
          </div>

          <Error class="error mt-6px" v-if="props.touched && errorMessage"
            >{{ errorMessage }}
          </Error>
        </StackLayout>
      </template>
    </Field>
  </div>
</template>

<style scoped lang="scss">
.calendar-icon {
  margin-right: 5px;
  color: var(--brand-color-brand-primary);
}

.error {
  color: var(--error-color-error-primary);
  /* font-family: Figtree; */
  font-size: var(--font-size-sm);
  font-style: normal;
  font-weight: var(--font-weight-normal);
  line-height: 125%; /* 17.5px */
}

.overlayed-present-text {
  position: absolute;
  margin-left: 12px;
  z-index: 2;
  background-color: white;
  width: 100px;
}
.date-toggle-button {
  padding: 0 !important;
  outline: none !important;
  border: none !important;
  border-radius: 0 !important;
}
.date-toggle-button:active,
.date-toggle-button:focus,
.date-toggle-button:visited {
  outline: none !important;
  border: none !important;
  box-shadow: none !important;
}

:global(.k-calendar .k-link),
:global(.k-calendar .k-calendar-td) {
  border-radius: 50% !important;
}

:global(.k-calendar-td) {
  width: 38px !important;
  height: 38px !important;
}
</style>
