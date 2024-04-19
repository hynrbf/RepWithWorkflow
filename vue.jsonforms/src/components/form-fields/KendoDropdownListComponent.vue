<script lang="ts">
import { defineComponent, PropType, inject } from "vue";
import { container } from "tsyringe";
import {
  IFormValidatorService,
  IFormValidatorServiceInfo,
} from "@/infra/dependency-services/form-validator/IFormValidatorService";
import { AppConstants } from "@/infra/AppConstants";
import { Emitter, EventType } from "mitt";
import isEqual from "lodash/isEqual";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";

export default defineComponent({
  name: "KendoDropdownListComponent",
  props: {
    name: {
      type: String,
      default: "",
    },
    id: String,
    label: {
      type: String,
      default: "",
    },
    placeholder: {
      type: String,
      default: "Please select",
    },
    hint: {
      type: String,
      default: "",
    },
    info: String,
    disabled: {
      type: Boolean,
      default: false,
    },
    isRequired: {
      type: Boolean,
      default: true,
    },
    dataItems: {
      type: Array,
      default: [] as { label: string; value: string; class?: string }[],
    },
    valuePrimitive: {
      type: Boolean,
      default: false,
    },
    loading: {
      type: Boolean,
      default: false,
    },
    disabledValues: {
      type: Array as PropType<[string, object][]>,
      default: () => [],
    },
    value: {
      type: [String, Object],
    },
    modelValue: {
      type: [String, Object],
    },
    addable: Boolean,
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
  watch: {
    isDataLoadedCompletely(newValue, oldValue): boolean {
      if (newValue == oldValue) {
        return newValue;
      }

      this.isFieldValidOnFirstLoad = this.validate(this.value ?? "") == "";
      return newValue;
    },
  },
  data() {
    return {
      isFieldValidOnFirstLoad: false,
      formValidatorService: container.resolve<IFormValidatorService>(
        IFormValidatorServiceInfo.name,
      ),
      customInputValue: "",
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      isUnmounted: false,
      errorMessage: "",
    };
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

    this.isFieldValidOnFirstLoad = this.validate(this.value ?? "") == "";
  },
  mounted() {
    this.$watch(
      (vm) => vm.disabled,
      () => {
        (this.kendoForm as any).errors[this.name] = this.validate(
          this.modelValue || this.value || "",
        );
      },
    );

    if (this.isValueReactive && this.isDataLoadedCompletely) {
      this.isFieldValidOnFirstLoad = this.validate(this.value ?? "") == "";
    }

    // Replacing the default dropdown icon
    let pickerButtonIcon = document.querySelector(
      "span.k-svg-icon.k-svg-i-caret-alt-down",
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
    pickerButtonIcon.style.color = "var(--content-content-05)";

    this.isUnmounted = false;
  },
  unmounted() {
    if (!this.id) {
      return;
    }

    this.removeValidationValueByFieldId(this.id);
    this.isUnmounted = true;
  },
  computed: {
    computedPlaceholder() {
      if (!this.$t("common-placeholder-text-dropdown")) {
        return this.placeholder;
      }

      return this.$t("common-placeholder-text-dropdown");
    },
  },
  methods: {
    validate(currentValue: string | Record<string, any>) {
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

      if (this.disabled) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      this.formValidatorService.setFieldName(this.label || this.name);
      const errorMessage = this.formValidatorService.validate<
        string | Record<string, any>
      >(currentValue, {
        [AppConstants.RequiredKey]: this.isRequired,
      });

      this.addIdKeyAndErrorValue(errorMessage);
      this.errorMessage = errorMessage;
      return errorMessage;
      //check profanity here
    },
    isValueDisabled(value: unknown) {
      return this.disabledValues.some((item) => isEqual(item, value));
    },
    onDropdownChanged(event: any) {
      (this.kendoForm as any)?.onChange?.(this.name, event);
      this.$emit("update:modelValue", event.value);
      this.$emit("onValueChange", event.value);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    },
    onCustomInputEnter() {
      // ToDo. Enhance in the future.
      const dropdownList = this.$refs.dropdownList as any;
      dropdownList._skipFocusEvent = true;
    },
    onCustomInputLeave() {
      // ToDo. Enhance in the future.
      const dropdownList = this.$refs.dropdownList as any;
      dropdownList._skipFocusEvent = false;
    },
    onAdd(event: string) {
      this.$emit("addCustom", event, (value = event) =>
        this.attachValue(value),
      );

      // ToDo. Enhance in the future.
      const dropdownList = this.$refs.dropdownList as any;
      dropdownList.currentFocused = false;
      dropdownList.currentOpened = false;
    },
    attachValue(value: any) {
      const option = this.dataItems.find((item: any) => item.value === value);
      if (!option) {
        return;
      }
      this.onDropdownChanged({ value: this.valuePrimitive ? value : option });
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
  inject: {
    kendoForm: { default: {} },
  },
});
</script>

<template>
  <KendoField :id="id" :name="name" component="template" :validator="validate">
    <template v-slot:template="{ props }">
      <KendoFieldWrapper :class="props.class">
        <KendoLabel
          v-if="label"
          :editor-id="id"
          :disabled="disabled"
          :valid="!(props.touched && errorMessage)"
          class="control-label"
        >
          {{ label }}
          <span v-if="!isRequired" class="fineprint ms-1"> (Optional) </span>
          <KendoTooltip v-if="info" anchor-element="target" position="top">
            <IconComponent
              :title="info"
              symbol="information-circle-18"
              class="ms-2 text-primary"
              size="17"
            />
          </KendoTooltip>
        </KendoLabel>

        <KendoDropDownList
          style="height: 38px"
          :style="{ 'margin-top': label !== '' ? '6px' : '0' }"
          ref="dropdownList"
          :valid="!(props.touched && errorMessage)"
          :value="modelValue || value || null"
          :data-items="dataItems"
          data-item-key="value"
          text-field="label"
          value-field="value"
          :value-primitive="valuePrimitive"
          value-render="value"
          item-render="item"
          :loading="loading"
          footer="footer"
          :disabled="disabled"
          @change="onDropdownChanged"
        >
          <template #value="{ props }">
            <span :class="[props.class]">
              <slot
                v-if="!props.value"
                name="placeholder"
                :placeholder="computedPlaceholder"
              >
                <text style="color: #808191">{{ computedPlaceholder }}</text>
              </slot>
              <slot v-else name="display" :value="props.value">
                {{ props.value?.label }}
              </slot>
            </span>
          </template>
          <template #item="{ props }">
            <li
              :class="[
                'k-item',
                isValueDisabled(props.dataItem.value) && 'is-disabled',
                props.class,
                props.dataItem.class && props.dataItem.class,
              ]"
              @click="
                ($event) => {
                  if (!isValueDisabled(props.dataItem.value)) {
                    props.onClick($event);
                  }
                }
              "
            >
              <slot name="display" :value="props.dataItem">
                {{ props.dataItem.label }}
              </slot>
            </li>
          </template>
          <template v-if="$slots.footer || addable" #footer>
            <slot name="footer">
              <div
                v-if="addable"
                class="p-3 p-2"
                @mouseenter="onCustomInputEnter"
                @mouseleave="onCustomInputLeave"
              >
                <InputWithAddComponent
                  v-model="customInputValue"
                  placeholder="Others"
                  @add="onAdd"
                />
              </div>
            </slot>
          </template>
        </KendoDropDownList>

        <KendoError class="error mt-6px" v-if="props.touched && errorMessage">
          {{ errorMessage }}
        </KendoError>

        <KendoHint v-else>
          {{ hint }}
        </KendoHint>
      </KendoFieldWrapper>
    </template>
  </KendoField>
</template>

<style scoped lang="scss">
.is-disabled {
  opacity: 0.5;
  cursor: not-allowed;
}
</style>