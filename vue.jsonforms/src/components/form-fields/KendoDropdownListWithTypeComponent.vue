<script lang="ts">
import { defineComponent, PropType } from "vue";
import DropdownListItemModel from "@/components/models/DropdownListItemModel";
import { container } from "tsyringe";
import {
  IFormValidatorService,
  IFormValidatorServiceInfo,
} from "@/infra/dependency-services/form-validator/IFormValidatorService";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";
import { AppConstants } from "@/infra/AppConstants";
import isEqual from "lodash/isEqual";

// This component will be cleaned up soon, this is dropdown with strongly typed value 'IDropdownListItemModel'
// with simple label and value properties
export default defineComponent({
  name: "KendoDropdownListWithTypeComponent",
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
      type: Array as () => DropdownListItemModel[],
      default: [] as DropdownListItemModel[],
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
      type: Array as PropType<DropdownListItemModel[]>,
      default: [],
    },
    value: {
      type: Object as () => DropdownListItemModel | null,
      default: null,
    },
    modelValue: {
      type: Object as () => DropdownListItemModel | null,
      default: null,
    },
    addable: Boolean,
    bypassDefaultOnSelectEvent: Boolean,
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
      formValidatorService: container.resolve<IFormValidatorService>(
        IFormValidatorServiceInfo.name,
      ),
      customInputValue: "",
      isFieldValidOnFirstLoad: false,
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
    this.$watch(
      (vm) => vm.disabled,
      () => {
        (this.kendoForm as any).errors[this.name] = this.validate(
          this.modelValue || this.value,
        );
      },
    );

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
  methods: {
    validate(currentValue: DropdownListItemModel | null) {
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

      if (this.isFieldValidOnFirstLoad || this.disabled) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (this.isValueReactive && !currentValue && this.value) {
        currentValue = this.value;
      }

      this.formValidatorService.setFieldName(this.label || this.name);
      const errorMessage = this.formValidatorService.validate<DropdownListItemModel | null>(
        currentValue,
        {
          [AppConstants.RequiredKey]: this.isRequired,
        },
      );

      this.addIdKeyAndErrorValue(errorMessage);
      this.errorMessage = errorMessage;
      return errorMessage;
      //check profanity here
    },
    isValueDisabled(value: unknown) {
      return this.disabledValues.some((item) => isEqual(item, value));
    },
    onDropdownChanged(event: any) {
      this.isFieldValidOnFirstLoad = false;
      (this.kendoForm as any)?.onChange?.(this.name, event);
      this.$emit("update:modelValue", event.value);
      this.$emit("onValueChange", event.value);
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
      const option = this.dataItems?.find((item: any) => item.value === value);
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
          style="margin-top: 6px; height: 38px"
          ref="dropdownList"
          :valid="!(props.touched && errorMessage)"
          :value="value"
          :data-items="dataItems"
          value-render="value"
          item-render="item"
          :loading="loading"
          footer="footer"
          header="header"
          :disabled="disabled"
          @change="onDropdownChanged"
        >
          <template #header>
            <slot name="header"></slot>
          </template>

          <template #value="{ props }">
            <span :class="[props.class]">
              <slot
                v-if="!props.value"
                name="placeholder"
                :placeholder="computedPlaceholder"
              >
                <text style="color: #808191">{{ computedPlaceholder }}</text>
              </slot>

              <slot
                v-else-if="$slots.valueDisplayTemplate"
                name="valueDisplayTemplate"
                :value="props.value"
              >
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
                  if (bypassDefaultOnSelectEvent) {
                    return;
                  }

                  if (!isValueDisabled(props.dataItem.value)) {
                    props.onClick($event);
                  }
                }
              "
            >
              <slot
                name="display"
                :value="props.dataItem"
                :clickEvent="props.onClick"
              >
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
                <slot name="footerTemplate" :onAdd="onAdd">
                  <InputWithAddComponent
                    v-model="customInputValue"
                    placeholder="Others"
                    @add="onAdd"
                  />
                </slot>
              </div>
            </slot>
          </template>
        </KendoDropDownList>

        <KendoError
          v-if="props.touched && errorMessage"
          style="margin-top: 8px"
        >
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