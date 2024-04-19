<script lang="ts">
import {defineComponent, inject} from "vue";
import {AppConstants} from "@/infra/AppConstants";
import {Emitter, EventType} from "mitt";

export default defineComponent({
  name: "KendoChooseOneQuestionInputComponent",
  props: {
    id: String,
    name: {
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
    firstOptionText: {
      type: String,
      default: "",
    },
    secondOptionText: {
      type: String,
      default: "",
    },
    width: {
      type: String,
      default: "127px",
    },
  },
  data() {
    return {
      eventBus: inject("$eventBusService") as Emitter<
          Record<EventType, unknown>
      >,
      valueInternal: undefined as boolean | undefined,
      errorMessage: "",
    };
  },
  created() {
    this.setValue();
  },
  updated() {
    this.setValue();
  },
  methods: {
    setValue() {
      this.valueInternal = this.value;
      this.errorMessage = this.validate(this.valueInternal);
    },
    onToggle(selected: boolean) {
      this.valueInternal = selected;
      this.errorMessage = this.validate(this.valueInternal);
      this.$emit("onValueChange", this.valueInternal);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    },

    validate(value: boolean | undefined): string {
      let errorMessage = "";

      if (typeof(value) == "undefined") {
        errorMessage = "Please select an option";
      }

      return errorMessage;
    },
  },
});
</script>

<template>
  <Field
    :id="id"
    :name="name"
    component="myTemplate"
    :value="valueInternal"
    :validator="validate"
  >
    <template v-slot:myTemplate="{ props }">
      <KendoFieldWrapper :class="props.class">
        <div class="d-flex flex-column">
          <div
            class="d-flex align-items-center kendo-yes-no-button"
            style="gap: 15px"
          >
            <Label v-if="questionText" class="flex-grow-1 question-text">
              {{ questionText }}
            </Label>

            <ButtonGroup class="button-group">
              <Button
                :style="`border-color: ${
                  props.touched && props.errorMessage && valueInternal === undefined
                    ? 'red'
                    : 'var(--brand-color-brand-primary)'
                }; width: ${width}`"
                type="button"
                class="btn-grp-item"
                size="small"
                :togglable="true"
                :selected="valueInternal === true"
                @click="onToggle(true)"
              >
                {{ firstOptionText }}
              </Button>

              <Button
                :style="`border-color: ${
                  props.touched && props.errorMessage && valueInternal === undefined
                    ? 'red'
                    : 'var(--brand-color-brand-primary)'
                }; width: ${width}`"
                type="button"
                class="btn-grp-item"
                size="small"
                :togglable="true"
                :selected="valueInternal === false"
                @click="onToggle(false)"
              >
                {{ secondOptionText }}
              </Button>
            </ButtonGroup>
          </div>

          <Error v-if="props.touched && errorMessage" style="margin-top: 6px"
            >{{ errorMessage }}
          </Error>
        </div>
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
  height: 45px;
  font-weight: var(--font-weight-semi-bold);
  color: var(--brand-color-brand-primary);
}

.question-text {
  font-size: var(--font-size-default);
  font-weight: var(--font-weight-medium);
  font-style: normal;
  line-height: 130%; /* 20.8px */
  color: var(--color-black);
}
</style>