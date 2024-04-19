<script lang="ts">
import { defineComponent, inject } from "vue";
import { AppConstants } from "@/infra/AppConstants";
import { Emitter, EventType } from "mitt";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";

export default defineComponent({
  name: "KendoFirmReferenceNumberInputComponent",
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
    indicatorLabel: {
      type: String,
      default: "",
    },
    placeholder: {
      type: String,
      default: "Type",
    },
    iconPath: {
      type: String,
      default: "",
    },
    iconPathRight: {
      type: String,
      default: "",
    },
    maxLength: {
      type: Number,
      default: 100,
    },
    isCapitalizeFirstLetter: {
      type: Boolean,
      default: false,
    },
    isEditable: {
      type: Boolean,
      default: true,
    },
    isOverrideCommonPlaceHolderText: {
      type: Boolean,
      default: false,
    },
    statusLabelColor: {
      type: String,
      default: "",
    },
    maxLengthBeforeTruncate: {
      type: Number,
      default: 23,
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
      valueInternal: "",
      isUnmounted: false,
      errorMessage: "",
      showFullText: false,
    };
  },
  computed: {
    computedPlaceholder() {
      if (!this.$t("common-placeholder-text")) {
        return this.placeholder;
      }

      if (this.isOverrideCommonPlaceHolderText && this.placeholder) {
        return this.placeholder;
      }

      return this.$t("common-placeholder-text");
    },
  },
  watch: {
    isDataLoadedCompletely(newValue, oldValue): boolean {
      if (newValue == oldValue) {
        return newValue;
      }

      this.isFieldValidOnFirstLoad = this.validate(this.valueInternal) == "";
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
    this.valueInternal = this.value;

    if (this.isValueReactive) {
      return;
    }

    this.isFieldValidOnFirstLoad = this.validate(this.valueInternal) == "";
  },
  mounted() {
    this.isUnmounted = false;

    if (this.isValueReactive && this.isDataLoadedCompletely) {
      this.isFieldValidOnFirstLoad = this.validate(this.valueInternal) == "";
    }
  },
  updated() {
    this.valueInternal = this.value;

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
  methods: {
    onInput(event: any) {
      if (!this.isEditable) {
        this.$emit("onAttemptEdit");
        return;
      }

      this.isFieldValidOnFirstLoad = false;
      this.valueInternal = event.target.value;
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

      if (this.isValueReactive && !currentValue && this.valueInternal) {
        currentValue = this.valueInternal;
      }

      if (!this.isRequired) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (!currentValue) {
        const errorMessage = `Please enter a valid ${this.label}`;
        this.addIdKeyAndErrorValue(errorMessage);
        this.errorMessage = errorMessage;
        return errorMessage;
      }

      this.addIdKeyAndErrorValue("");
      return "";
    },

    internalChange(event: any) {
      if (!this.isEditable) {
        return;
      }

      this.isFieldValidOnFirstLoad = false;
      this.valueInternal = event.target.value;
      this.$emit("onValueChange", this.valueInternal);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    },

    addIdKeyAndErrorValue(value: string) {
      if (!this.id) {
        console.log(
          "Component id is a must. Your progress bar value will be erratic."
        );
        return;
      }

      const key = this.id;
      let errorItem: Record<string, string> = {};
      errorItem[key] = value;
      this.addComponentValidationValue(key, errorItem);
    },

    onToolTipHover() {
      const tooltipPill = this.$refs.tooltipPill as HTMLElement;
      const toolTip = this.$refs.customTooltip as HTMLElement;

      const toolTipBCR = tooltipPill.getBoundingClientRect();

      if (toolTip != undefined) {
        toolTip.style.left =
          (
            toolTipBCR.right +
            tooltipPill.clientWidth -
            (toolTip.clientWidth + toolTipBCR.width)
          ).toString() + "px";

        toolTip.style.top =
          (toolTipBCR.top - (tooltipPill.clientHeight + 3)).toString() + "px";        
      }
    },
  },
});
</script>

<template>
  <div class="position-relative">
    <Field
      :name="name"
      :id="id"
      :component="'myTemplate'"
      :validator="validate"
    >
      <template v-slot:myTemplate="{ props }">
        <StackLayout
          orientation="vertical"
          class="position-relative"
          style="gap: 6px"
        >
          <div v-if="label !== ''" class="d-flex align-items-center">
            <Label
              :editor-id="props.id"
              class="control-label align-self-end w-60 mt-5px"
            >
              {{ label }}
            </Label>
            <div class="flex-grow-1" />
            <!-- Spacer only -->
            <div
              v-if="indicatorLabel"
              :class="
                statusLabelColor === 'Green'
                  ? 'status-label-authorised'
                  : 'status-label-non-authorised'
              "
            >
              <!-- Custom Tooltip -->
              <div
                v-if="
                  indicatorLabel &&
                  indicatorLabel.length > maxLengthBeforeTruncate
                "
                ref="customTooltip"
                class="firm-status-tooltip"
                :class="{
                  'status-label-authorised': statusLabelColor === 'Green',
                  'status-label-non-authorised': statusLabelColor === 'Red',
                  'long-indicator-label': indicatorLabel.length >= 80,
                }"
              >
                <Label class="paragraph-s-figtree-medium">
                  <PillComponent
                    v-if="indicatorLabel"
                    :class="
                      statusLabelColor === 'Green'
                        ? 'status-label-authorised'
                        : 'status-label-non-authorised'
                    "
                  >
                    <span :title="indicatorLabel">
                      {{ indicatorLabel }}
                    </span>
                  </PillComponent>
                </Label>
              </div>
              <!-- Pll/Badge Indicator Label -->
              <Label
                @mouseover="onToolTipHover"
                class="paragraph-s-figtree-medium firm-status-trigger"
              >
                <span
                  ref="tooltipPill"
                  v-if="indicatorLabel.length <= maxLengthBeforeTruncate"
                >
                  <PillComponent
                    v-if="indicatorLabel"
                    :class="
                      statusLabelColor === 'Green'
                        ? 'status-label-authorised'
                        : 'status-label-non-authorised'
                    "
                  >
                    <span>
                      {{ indicatorLabel }}
                    </span>
                  </PillComponent>
                </span>
                <span ref="tooltipPill" v-else>
                  <PillComponent
                    hasTooltip
                    :maxlengthBeforeTruncate="23"
                    v-if="indicatorLabel"
                    :class="
                      statusLabelColor === 'Green'
                        ? 'status-label-authorised'
                        : 'status-label-non-authorised'
                    "
                  >
                    <span>
                      {{
                        indicatorLabel === ""
                          ? ""
                          : indicatorLabel.substring(
                              0,
                              maxLengthBeforeTruncate - 3
                            ) + "..."
                      }}
                    </span>
                  </PillComponent>
                </span>
              </Label>
            </div>
            <!-- Add space by default to prevent alignment issue if there is no status -->
            <div v-else class="status-label" style="background: transparent">
              <Label class="paragraph-s-figtree-medium">&nbsp;</Label>
            </div>
          </div>
          <span v-if="!isRequired" class="optionalLabel"> (Optional) </span>
          <Input
            class="input-with-icon"
            style="border-radius: 8px"
            v-bind="props"
            :id="props.id"
            :name="props.name"
            :placeholder="computedPlaceholder"
            :valid="!(props.touched && errorMessage)"
            :value="capitalizeFirstLetter(valueInternal)"
            @input="onInput"
            @change="internalChange"
            :maxLength="maxLength.toString()"
            :inputPrefix="iconPath ? 'prefix' : ''"
            :inputSuffix="iconPathRight ? 'suffix' : ''"
            :fillMode="'solid'"
          >
            <!--These are fca and company house logo before, but Figma always change? just in case it puts back again -->
            <template v-slot:prefix>
              <img
                class="mx-1"
                :src="iconPath"
                style="height: 20px"
                alt="icon"
              />
            </template>

            <template v-slot:suffix>
              <img
                class="mx-1"
                :src="iconPathRight"
                style="height: 20px"
                alt="icon"
              />
            </template>
          </Input>

          <Error class="error" v-if="props.touched && errorMessage"
            >{{ errorMessage }}
          </Error>
        </StackLayout>
      </template>
    </Field>
  </div>
</template>

<style scoped>
.status-label {
  justify-content: center;
  align-items: center;
  gap: 4px;
  border-radius: 150px;
  background: var(--color-grey-classic-200);
}

.status-label-authorised {
  justify-content: center;
  align-items: center;
  gap: 4px;
  border-radius: 150px;
  background: var(--color-success-50);
}

.status-label-authorised > label {
  color: var(--color-success-700);
}

.status-label-non-authorised {
  justify-content: center;
  align-items: center;
  gap: 4px;
  border-radius: 150px;
  background: var(--Error-50, #fee7e7);
  color: var(--Error-700, #c20a0a);
}

.control-invalid {
  border-color: var(--color-error);
}

.firm-status-tooltip {
  position: fixed;
  display: none;
  z-index: 2;
}

.status-label-non-authorised:hover .firm-status-tooltip,
.status-label-authorised:hover .firm-status-tooltip {
  display: block;
}

.long-indicator-label {
  width: 156%;
}

.optionalLabel {
  position: absolute;
  bottom: 9px;
  z-index: 10;
  right: 5px;
}
</style>
