<script lang="ts">
import { defineComponent, PropType } from "vue";
import { container } from "tsyringe";
import {
  IFormValidatorService,
  IFormValidatorServiceInfo,
} from "@/infra/dependency-services/form-validator/IFormValidatorService";
import { AppConstants } from "@/infra/AppConstants";
import isEmpty from "lodash/isEmpty";
import xor from "lodash/xor";
import omit from "lodash/omit";
import { getter } from "@progress/kendo-vue-common";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";
import DropdownListItemModel from "@/components/models/DropdownListItemModel";
import DataItemModel from "@/components/models/DataItemModel";

export default defineComponent({
  name: "KendoMultiSelectTreeComponent",
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
      default: "Please Type or Select",
    },
    hint: {
      type: String,
      default: "",
    },
    disabled: {
      type: Boolean,
      default: false,
    },
    isRequired: {
      type: Boolean,
      default: true,
    },
    dataItems: {
      type: Array as PropType<DataItemModel[]>,
      default: [],
    },
    valuePrimitive: {
      type: Boolean,
      default: false,
    },
    loading: {
      type: Boolean,
      default: false,
    },
    value: {
      type: Array as () => DropdownListItemModel[],
      default: [],
    },
    modelValue: {
      type: Array as () => DropdownListItemModel[],
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
      isFieldValidOnFirstLoad: false,
      subItemsField: "items",
      expandField: "expanded",
      checkField: "checked",
      checkIndeterminateField: "indeterminateChecked",
      expandedItems: [] as Array<number | string>,
      checkedItems: [] as DropdownListItemModel[],
      customInputValue: "",
      errorMessage: ""
    };
  },
  computed: {
    processedDataItems() {
      const [result] = this.processDataItems(this.dataItems);
      return result;
    },

    computedPlaceholder() {
      if (!this.$t("common-placeholder-text-multiselect-tree")) {
        return this.placeholder;
      }

      return this.$t("common-placeholder-text-multiselect-tree");
    },
  },
  watch: {
    disabled: {
      handler(value) {
        if (value) {
          this.checkedItems = [];
        }
      },
      immediate: true,
    },

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
    const { addComponentValidationValue } = pageComponentValidationValueStore;

    return {
      addComponentValidationValue,
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
      (vm) => [vm.modelValue, vm.value],
      ([a, b]) => {
        const value = a || b;
        if (Array.isArray(value)) {
          this.checkedItems = value;
        }
      },
      { immediate: true, deep: true },
    );
    this.$watch(
      (vm) => vm.disabled,
      () => {
        (this.kendoForm as any).errors[this.name] = this.validate(
          this.modelValue || this.value || [],
        );
      },
    );
  },
  methods: {
    validate(currentValue: DropdownListItemModel[]) {
      this.errorMessage = "";

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
      const errorMessage = this.formValidatorService.validate<
        DropdownListItemModel[]
      >(currentValue, {
        [AppConstants.RequiredKey]: this.isRequired,
      });
      this.addIdKeyAndErrorValue(errorMessage);
      this.errorMessage = errorMessage;
      return errorMessage;
    },

    processDataItems(items: any[]) {
      if (isEmpty(items)) {
        return [[], false];
      }

      let hasChecked = false;
      const processedItems = [...items].map((item) => {
        const [childItems, hasCheckedChildItem] = this.processDataItems(
          getter(this.subItemsField)(item),
        );

        const isChecked = this.checkedItems.find((checkedItem: any) => {
          return checkedItem.value === item.value;
        });
        if (isChecked || hasCheckedChildItem) {
          hasChecked = true;
        }

        item[this.expandField] = this.expandedItems.includes(item.value);
        item[this.subItemsField] = childItems;
        item[this.checkField] = isChecked;
        item[this.checkIndeterminateField] = !isChecked && hasCheckedChildItem;

        return item;
      });

      return [processedItems, hasChecked];
    },

    cleanItems(items: DropdownListItemModel[]) {
      this.isFieldValidOnFirstLoad = false;
      return items.map((item) => {
        return omit(item, [
          this.checkField,
          this.checkIndeterminateField,
          this.expandField,
          this.subItemsField,
        ]);
      });
    },

    removeCheckedItem(item: DropdownListItemModel, props: any) {
      this.isFieldValidOnFirstLoad = false;
      this.checkedItems = this.checkedItems.filter(
        (checkedItem) => item.value !== checkedItem.value,
      );
      const cleanedValue = this.cleanItems(this.checkedItems);
      this.$emit("update:modelValue", cleanedValue);
      this.$emit("onValueChange", cleanedValue);
      props.onChange({ value: cleanedValue });
    },

    onChange(event: any, props: any) {
      const eventType = event?.event?.type;
      const isClearing = event?.operation === "clear";
      this.isFieldValidOnFirstLoad = false;

      if (
        !isClearing &&
        eventType === "click" &&
        this.bypassDefaultOnSelectEvent
      ) {
        return;
      }

      this.checkedItems = event.value as DropdownListItemModel[];
      const cleanedValue = this.cleanItems(this.checkedItems);
      this.$emit("update:modelValue", cleanedValue);
      this.$emit("onValueChange", cleanedValue);
      props.onChange({ value: cleanedValue });
    },

    onExpanded(event: any) {
      this.expandedItems = xor(this.expandedItems, [
        (event.item as DropdownListItemModel)?.value ?? "",
      ]);
    },

    onDropdownChanged(event: any) {
      this.isFieldValidOnFirstLoad = false;
      (this.kendoForm as any)?.onChange?.(this.name, event);
      this.$emit("update:modelValue", event.value);
      this.$emit("onValueChange", event.value);
    },

    onCustomInputEnter() {
      // ToDo. Enhance in the future.
      const multiSelectTree = this.$refs.multiSelectTree as any;
      multiSelectTree._skipFocusEvent = true;
    },

    onCustomInputLeave() {
      // ToDo. Enhance in the future.
      const multiSelectTree = this.$refs.multiSelectTree as any;
      multiSelectTree._skipFocusEvent = false;
    },

    onAdd(event: string) {
      this.isFieldValidOnFirstLoad = false;
      this.$emit("addCustom", event, (value = event) =>
        this.attachValue(value),
      );

      // ToDo. Enhance in the future.
      const multiSelectTree = this.$refs.multiSelectTree as any;
      multiSelectTree.currentFocused = false;
      multiSelectTree.currentOpened = false;
    },

    removeCheckedFromExternal(item: DropdownListItemModel, itemName: string) {
      this.removeCheckedItem(item, itemName);
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
          class="control-label mb-2"
        >
          {{ label }}
          <span v-if="!isRequired" class="fineprint ms-1"> (Optional) </span>
        </KendoLabel>

        <MultiSelectTree
          ref="multiSelectTree"
          class-name="MultiSelectTree"
          :popup-settings="{ className: 'MultiSelectTree-popup' }"
          :value="checkedItems"
          :data-items="processedDataItems"
          :data-item-key="'value'"
          :text-field="'label'"
          :sub-items-field="subItemsField"
          :expanded-field="expandField"
          :check-field="checkField"
          :check-indeterminate-field="checkIndeterminateField"
          :placeholder="computedPlaceholder"
          :valid="!(props.touched && errorMessage)"
          :disabled="disabled"
          item="item"
          tag="tag"
          header="header"
          footer="footer"
          @expandchange="onExpanded"
          @change="onChange($event, props)"
        >
          <template #header>
            <slot name="header"></slot>
          </template>

          <template #item="{ props: props }">
            <li
              @click="
                ($event) => {
                  if (bypassDefaultOnSelectEvent) {
                    return;
                  }

                  props.onClick?.($event);
                }
              "
              class="col"
            >
              <slot
                name="display"
                :item="props.item"
                :clickEvent="props.onClick"
              >
                {{ props.item?.label }}
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

          <template #tag="{ props: tagProps }">
            <slot
              name="tagTemplate"
              :tagProps="tagProps"
              :item="tagProps.tagData.data[0]"
              :onClose="(item: any) => removeCheckedFromExternal(item, props)"
            >
              <KendoTooltip anchor-element="target" position="top">
                <PillComponent
                  theme-color="white"
                  size="lg"
                  class="font-weight-semi-bold"
                  text-class="is-truncated"
                  closeable
                  @close="removeCheckedItem(tagProps.tagData.data[0], props)"
                >
                  <span class="is-truncated" :title="tagProps.tagData.text">
                    {{ tagProps.tagData.text }}
                  </span>
                </PillComponent>
              </KendoTooltip>
              <a
                href="#"
                v-if="
                  tagProps.dataItems.length > 4 &&
                  tagProps.dataItems.length - 1 === tagProps.index
                "
              >
                <b>+{{ tagProps.dataItems.length - 4 }}</b>
              </a>
            </slot>
          </template>
        </MultiSelectTree>

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

<style lang="scss">
.MultiSelectTree {
  min-height: 40px;

  .k-chip-list[role="listbox"] {
    display: flex;
    flex-wrap: nowrap;
    width: 100%;

    & > div {
      max-width: 23%;
      flex: 0 0 23%;
      display: none;

      &:nth-child(1),
      &:nth-child(2),
      &:nth-child(3),
      &:nth-child(4) {
        display: block;
      }
    }

    .Pill {
      width: 100%;

      svg {
        display: block;
      }
    }
  }

  .k-input-value-text {
    color: #808191;
  }

  .k-clear-value {
    padding: 0 !important;
  }

  &-popup {
    .k-treeview {
      max-height: 300px;
    }

    .k-treeview-lines {
      & > .k-treeview-item {
        padding-left: 0;
      }
    }

    .k-treeview-mid {
      position: relative;
    }

    .k-treeview-toggle {
      margin: 0;
      position: absolute;
      right: 0;
      top: 50%;
      transform: translateY(-50%);

      & ~ .k-checkbox-wrap {
        display: none;

        & ~ .k-treeview-leaf {
          font-weight: var(--font-weight-semi-bold);
          pointer-events: none;
        }
      }
    }
  }
}

.k-treeview-mid,
.k-treeview-leaf,
.k-treeview-leaf-text {
  flex-grow: 1;
}
</style>