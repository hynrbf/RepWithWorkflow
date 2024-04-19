<script lang="ts">
import { defineComponent, inject } from "vue";
import { Emitter, EventType } from "mitt";
import { AppConstants } from "@/infra/AppConstants";
import {usePageComponentValidationValueStore} from "@/stores/progress-bar/usePageComponentValidationValueStore";

export default defineComponent({
  name: "KendoYesOrNoQuestionInputComponent",
  props: {
    id: String,
    numberText: {
      type: String,
      default: "",
    },
    questionText: {
      type: String,
      default: "",
    },
    value: {
      type: Boolean,
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
  },
  data() {
    return {
      isFieldValidOnFirstLoad: false,
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      valueInternal: undefined as boolean | undefined,
      isUnmounted: false,
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
    this.bindValue();

    if (this.isValueReactive) {
      return;
    }

    this.isFieldValidOnFirstLoad = this.validate(this.value) == "";
  },
  updated() {
    this.bindValue();
  },
  mounted() {
    this.isUnmounted = false;

    if (this.isValueReactive && this.isDataLoadedCompletely) {
      this.isFieldValidOnFirstLoad = this.validate(this.valueInternal) == "";
    }
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

    return {
      addComponentValidationValue,
      removeValidationValueByFieldId,
    };
  },
  methods: {
    bindValue() {
      this.valueInternal = this.value;
    },

    onToggle(isYes: boolean) {
      this.valueInternal = isYes;
      this.validate(this.valueInternal);
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

    validate(value?: boolean): string {
      if (this.isUnmounted) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (this.isValueReactive && !this.isDataLoadedCompletely) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      let errorMessage = "Please select an option";
      if (value === undefined) {
        this.addIdKeyAndErrorValue(errorMessage);
        return errorMessage;
      }

      this.addIdKeyAndErrorValue("");
      return "";
    },

    getButtonStyle(
      buttonText: string,
      isTouched: boolean,
      validationMessage: string,
    ): {
      borderColor: string;
      color: string;
    } {
      const primaryColor = "var(--brand-color-brand-primary)";
      const whiteColor = "white";
      const errorRed = "var(--color-error-600)";

      const borderColor =
        this.valueInternal === undefined
          ? isTouched && validationMessage
            ? errorRed
            : primaryColor
          : primaryColor;

      const color =
        this.valueInternal === undefined
          ? borderColor
          : buttonText === "Yes"
          ? this.valueInternal
            ? whiteColor
            : primaryColor
          : this.valueInternal
          ? primaryColor
          : whiteColor;

      return { borderColor, color };
    },
  },
});
</script>

<template>
  <Field
    :id="'yesOrNoQuestion-id'"
    :name="'yesOrNoQuestion-name'"
    component="myTemplate"
    :value="valueInternal"
    :validator="validate"
  >
    <template v-slot:myTemplate="{ props }">
      <KendoFieldWrapper :class="props.class">
        <div
          class="d-flex align-items-center kendo-yes-no-button"
          style="gap: 15px"
        >
          <div v-if="numberText" class="number-text-circle">
            <Label class="number-text">{{ numberText }}</Label>
          </div>

          <Label class="flex-grow-1 question-text">
            {{ questionText }}
          </Label>

          <ButtonGroup class="button-group">
            <Button
              :style="
                getButtonStyle('Yes', props.touched, props.validationMessage)
              "
              type="button"
              class="btn-grp-item"
              size="small"
              :togglable="true"
              :selected="valueInternal === undefined ? false : valueInternal"
              @click="onToggle(true)"
            >
              Yes
            </Button>

            <Button
              :style="
                getButtonStyle('No', props.touched, props.validationMessage)
              "
              type="button"
              class="btn-grp-item"
              size="small"
              :togglable="true"
              :selected="valueInternal === undefined ? false : !valueInternal"
              @click="onToggle(false)"
            >
              No
            </Button>
          </ButtonGroup>
        </div>

        <Error
          style="float: right; margin-right: 20px; margin-top: 6px"
          v-if="
            props.touched &&
            props.validationMessage &&
            valueInternal === undefined
          "
          >{{ props.validationMessage }}
        </Error>
      </KendoFieldWrapper>
    </template>
  </Field>
</template>

<style scoped>
.button-group button {
  display: flex;
  padding: 12px 20px;
  flex-direction: column;
  align-items: center;
}

.btn-grp-item {
  border: 1px solid var(--brand-color-brand-primary);
  background-color: white;
  width: 82px;
  height: 45px;
  font-weight: var(--font-weight-semi-bold);
}

.number-text {
  color: white;
  font-size: 14px;
  font-style: normal;
  font-weight: 600;
  line-height: 125%; /* 17.5px */
}

.number-text-circle {
  display: flex;
  width: 24px;
  height: 24px;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  gap: 10px;
  flex-shrink: 0;

  border-radius: 18px;
  background: var(--brand-color-brand-primary);
}

.question-text {
  font-size: var(--font-size-default);
  font-weight: var(--font-weight-medium);
  font-style: normal;
  line-height: 130%; /* 20.8px */
  color: var(--color-black);
}
</style>