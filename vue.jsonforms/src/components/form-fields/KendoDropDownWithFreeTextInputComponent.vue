<script lang="ts">
import { defineComponent, inject } from "vue";
import { Field } from "@progress/kendo-vue-form";
import { Label, Error } from "@progress/kendo-vue-labels";
import { ComboBox, ComboBoxChangeEvent } from "@progress/kendo-vue-dropdowns";
import { StackLayout } from "@progress/kendo-vue-layout";
import { AppConstants } from "@/infra/AppConstants";
import { Emitter, EventType } from "mitt";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";

export default defineComponent({
  name: "KendoDropDownWithFreeTextInputComponent",
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
      default: "Please select",
    },
    dataItems: {
      type: Array,
      default: [] as string[],
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
    StackLayout,
    Field,
    Label,
    ComboBox,
    Error,
  },
  data() {
    return {
      isFieldValidOnFirstLoad: false,
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      other: "",
      isAddingOther: false,
      isDropdownOpened: false,
      isUnmounted: false,
      errorMessage: ""
    };
  },
  computed: {
    computedPlaceholder() {
      if (!this.$t("common-placeholder-text-dropdown")) {
        return this.placeholder;
      }

      return this.$t("common-placeholder-text-dropdown");
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
    if (this.isValueReactive) {
      return;
    }

    this.isFieldValidOnFirstLoad = this.validate(this.value) == "";
  },
  mounted() {
    // Replacing the default dropdown icon
    let pickerButtonIcon = document.querySelector(
      ".kendo-drop-down span.k-svg-icon.k-svg-i-caret-alt-down",
    ) as HTMLSpanElement;

    if (!pickerButtonIcon) {
      return;
    }

    if (pickerButtonIcon.classList?.length < 1 ?? true) {
      return;
    }

    pickerButtonIcon.classList.remove("k-svg-icon");
    pickerButtonIcon.classList.remove("k-svg-i-caret-alt-down");

    pickerButtonIcon.classList.add("k-icon");
    pickerButtonIcon.classList.add("k-i-chevron-down");
    pickerButtonIcon.style.scale = "1.25"; // width and height = 20px
    this.isUnmounted = false;
  },
  unmounted() {
    if (!this.id) {
      return;
    }

    this.removeValidationValueByFieldId(this.id);
    this.isUnmounted = true;
  },
  methods: {
    onSelect(event: ComboBoxChangeEvent) {
      this.isFieldValidOnFirstLoad = false;
      this.$emit("onValueChange", event.value);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
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

    onFocus() {
      // need delay to fully open the dropdown
      setTimeout(() => (this.isDropdownOpened = true), 100);
    },

    onBlur() {
      if (this.isAddingOther) {
        return;
      }

      this.isDropdownOpened = false;
    },

    onAddOtherClick(event: any) {
      this.isAddingOther = true;
      event.target?.focus();
    },

    outFocusAddOther() {
      this.isAddingOther = false;
      this.isDropdownOpened = false;
    },

    // click the add other icon
    onAddOther() {
      this.outFocusAddOther();
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
        <StackLayout orientation="vertical" style="gap: 6px">
          <Label :editor-id="props.id" class="control-label">
            {{ label }}
            <span v-if="!isRequired" class="fineprint ms-1"> (Optional) </span>
          </Label>

          <ComboBox
            class="kendo-drop-down"
            v-bind="props"
            :id="props.id"
            :valid="!(props.touched && errorMessage)"
            :placeholder="computedPlaceholder"
            :value="value"
            @change="onSelect"
            @blur="onBlur"
            @focus="onFocus"
            :data-items="dataItems"
            fill-mode="none"
            :clearButton="false"
            :footer="'myFooter'"
            style="
              border: var(--content-content-07) solid thin;
              color: var(--color-dark);
              border-radius: 8px;
            "
          >
            <template #myFooter>
              <StackLayout
                class="nationality-footer"
                orientation="horizontal"
                :gap="8"
                :align="{ horizontal: 'start', vertical: 'middle' }"
              >
                <KendoGenericInputComponent
                  class="flex-grow-1"
                  :isRequired="false"
                  :value="other"
                  :customStyle="{ borderWidth: 0 }"
                  placeholder="Other"
                  @click="onAddOtherClick"
                  @focusout="outFocusAddOther"
                  @onValueChange="(v: string) => (other = v)"
                />

                <IconComponent
                  class="add-other-icon"
                  symbol="add-circle-27"
                  size="20"
                  @click="onAddOther"
                />
              </StackLayout>
            </template>
          </ComboBox>

          <Error v-if="props.touched && errorMessage"
            >{{ errorMessage }}
          </Error>
        </StackLayout>
      </template>
    </Field>
  </div>
</template>

<style scoped>
.nationality-footer {
  padding: 4px 12px 4px 6px;
  margin: 12px;
  border: 1px solid var(--content-content-07);
  border-radius: 8px;
}

.add-other-icon {
  color: var(--brand-color-brand-primary);
  cursor: pointer;
}
</style>