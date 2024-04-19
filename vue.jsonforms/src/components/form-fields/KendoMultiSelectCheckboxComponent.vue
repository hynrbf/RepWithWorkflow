<script lang="ts">
import {
  defineComponent,
  Ref,
  ref,
  computed,
  onMounted,
  watch,
  inject,
} from "vue";
import { Emitter, EventType } from "mitt";
import { container } from "tsyringe";
import { getter, setter } from "@progress/kendo-vue-common";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";
import {
  IFormValidatorService,
  IFormValidatorServiceInfo,
} from "@/infra/dependency-services/form-validator/IFormValidatorService";
import { AppConstants } from "@/infra/AppConstants";
import DataItemModel from "@/components/models/DataItemModel";

export interface TreeDataOptionsModel {
  subItemsField?: string;
  checkField?: string;
  checkIndeterminateField?: string;
  expandField?: string;
  dataItemKey: string;
  value: DataItemModel[];
  parent?: string;
  expanded: string[];
}

export interface ProcessedTreeDataOptionsModel extends TreeDataOptionsModel {
  valueMap: Record<string, boolean>;
  expandedMap: Record<string, boolean>;
  keyGetter: (item: DataItemModel) => string;
  subItemGetter: (item: DataItemModel) => DataItemModel[];
  subItemSetter: (item: DataItemModel, value: DataItemModel[]) => void;
  checkSetter: (item: DataItemModel, value: boolean) => void;
  expandedSetter: (item: DataItemModel, value: boolean) => void;
  checkIndeterminateSetter: (item: DataItemModel, value: boolean) => void;
}

export default defineComponent({
  name: "KendoMultiSelectCheckboxComponent",
  props: {
    items: {
      type: Array as () => DataItemModel[],
      default: () => [],
    },
    value: {
      type: Array as () => DataItemModel[],
      default: () => [],
    },
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
    valuePrimitive: {
      type: Boolean,
      default: false,
    },
    loading: {
      type: Boolean,
      default: false,
    },
    isValueReactive: {
      type: Boolean,
      default: false,
    },
    isDataLoadedCompletely: {
      type: Boolean,
      default: false,
    },
  },
  setup(props, { emit }) {
    const textField: string = "label";
    const dataItemKey: string = "value";
    const checkField: string = "checkField";
    const checkIndeterminateField: string = "checkIndeterminateField";
    const subItemsField: string = "items";
    const expandField: string = "expanded";

    const formValidatorService = container.resolve<IFormValidatorService>(
      IFormValidatorServiceInfo.name
    );

    const pageComponentValidationValueStore =
      usePageComponentValidationValueStore();
    const { addComponentValidationValue } = pageComponentValidationValueStore;

    const kendoForm = inject<any>("kendoForm", { default: {} });
    const eventBus = inject("$eventBusService") as Emitter<
      Record<EventType, unknown>
    >;

    const errorMessage: Ref<string> = ref("");
    const isFieldValidOnFirstLoad = ref(false);
    const dataItems: Ref<DataItemModel[]> = ref(props.items);
    const value: Ref<DataItemModel[]> = ref(props.value);

    const expanded: Ref<string[]> = computed(
      () =>
        dataItems.value
          .map((item) => item.value)
          .filter((value) => value !== undefined) as string[]
    );

    const treeData = computed(() =>
      processMultiSelectTreeData(dataItems.value, {
        dataItemKey,
        subItemsField,
        checkField,
        checkIndeterminateField,
        expandField,
        value: value.value,
        expanded: expanded.value,
      })
    );

    const getValueMap = (
      value: any[],
      idGetter: (item: any) => any
    ): Record<string, boolean> => {
      const map: Record<string, boolean> = {};
      value.forEach((item) => {
        map[idGetter(item)] = true;
      });
      return map;
    };

    // Use this for expanded items if will the UI requires it
    /*const expandedState = (item: any, dataItemKey: string, expanded: number[]): number[] => {
      const nextExpanded = [...expanded];
      const keyGetter = getter(dataItemKey);
      const itemKey = keyGetter(item);
      const index = expanded.findIndex((currentKey) => currentKey === itemKey);
      index === -1 ? nextExpanded.push(itemKey) : nextExpanded.splice(index, 1);
      return nextExpanded;
    };*/

    const mapMultiSelectTreeData = (
      data: DataItemModel[],
      options: ProcessedTreeDataOptionsModel
    ): [DataItemModel[], boolean] => {
      const {
        keyGetter,
        subItemGetter,
        subItemSetter,
        checkSetter,
        expandedSetter,
        checkIndeterminateSetter,
        valueMap,
        expandedMap,
      } = options;

      if (!data || !data.length) {
        return [data, false];
      }

      let hasChecked = false;

      const newData = [...data].map((dataItem: DataItemModel) => {
        const [children, hasCheckedChildren] = mapMultiSelectTreeData(
          subItemGetter(dataItem),
          options
        );

        const isChecked = valueMap[keyGetter(dataItem)];
        if (isChecked || hasCheckedChildren) {
          hasChecked = true;
        }

        const newItem: DataItemModel = { ...dataItem };

        expandedSetter(newItem, expandedMap[keyGetter(newItem)]);
        subItemSetter(newItem, children);
        checkSetter(newItem, isChecked);
        checkIndeterminateSetter(newItem, !isChecked && hasCheckedChildren);

        return newItem;
      });

      return [newData, hasChecked];
    };

    const processMultiSelectTreeData = (
      tree: DataItemModel[],
      options: TreeDataOptionsModel
    ): DataItemModel[] => {
      const {
        subItemsField = "items",
        checkField = "checkField",
        checkIndeterminateField = "checkIndeterminateField",
        expandField = "expanded",
        dataItemKey,
        value,
        expanded,
      } = options;

      const keyGetter = getter(dataItemKey);
      const expandedMap: Record<string, boolean> = {};

      expanded.forEach((id) => (expandedMap[id.toString()] = true));

      const [result] = mapMultiSelectTreeData(tree, {
        valueMap: getValueMap(value, keyGetter),
        expandedMap: expandedMap,
        keyGetter,
        expandedSetter: setter(expandField),
        subItemGetter: getter(subItemsField),
        subItemSetter: setter(subItemsField),
        checkSetter: setter(checkField),
        checkIndeterminateSetter: setter(checkIndeterminateField),
        dataItemKey: "",
        value: [],
        expanded: [],
      });

      return result;
    };

    const onChange = (event: any) => {
      value.value = event.value;
      emit("onValueChange", event.value);
      eventBus.emit(AppConstants.formFieldChangedEvent);
      eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    };

    const removeItem = (index: number) => {
      value.value.splice(index, 1);
      emit("onValueChange", value.value);
      eventBus.emit(AppConstants.formFieldChangedEvent);
      eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    };

    //ToDo. to check if the validation will work as simple as this in composition API
    onMounted(() => {
      isFieldValidOnFirstLoad.value = validate(value.value) == "";
    });

    const addEllipsis = (word: string, maxLength: number) => {
      if (word.length > maxLength) {
        return word.substring(0, maxLength - 3) + "...";
      }

      return word;
    };

    const watchValueLength = computed(() => value.value.length === 1);

    const validate = (currentValue: DataItemModel[]) => {
      errorMessage.value = "";

      if (props.isValueReactive && !currentValue && props.value) {
        currentValue = props.value;
      }

      formValidatorService.setFieldName(props.label || props.name);

      const errorMessageValue = formValidatorService.validate<DataItemModel[]>(
        currentValue,
        {
          [AppConstants.RequiredKey]: props.isRequired,
        }
      );

      addIdKeyAndErrorValue(errorMessageValue);
      errorMessage.value = errorMessageValue;
      return errorMessageValue;
    };

    const addIdKeyAndErrorValue = (value: string) => {
      if (!props.id) {
        return;
      }

      const key = props.id;
      let errorItem: Record<string, string> = {};

      errorItem[key] = value;
      addComponentValidationValue(key, errorItem);
    };

    watch(
      value,
      (newValue) => {
        if (Array.isArray(newValue)) {
          value.value = newValue;
        }
        kendoForm.errors[props.name] = validate(newValue);
      },
      { immediate: true, deep: true }
    );

    return {
      textField,
      dataItemKey,
      checkField,
      checkIndeterminateField,
      subItemsField,
      expandField,
      dataItems,
      expanded,
      processMultiSelectTreeData,
      treeData,
      onChange,
      addEllipsis,
      removeItem,
      watchValueLength,
      errorMessage,
      validate,
      addIdKeyAndErrorValue,
      kendoForm,
    };
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
          class="multi-select-checkbox"
          :class="{ 'has-selected': value.length > 0 }"
          :dataItems="treeData"
          :value="value"
          :placeholder="'Please Select'"
          :textField="textField"
          :dataItemKey="dataItemKey"
          :checkField="checkField"
          :checkIndeterminateField="checkIndeterminateField"
          :subItemsField="subItemsField"
          :expandField="expandField"
          :valid="!(props.touched && errorMessage)"
          :tag="'tag'"
          :popupOpen="watchValueLength"
          :keepPopupOpen="true"
          @change="onChange($event)"
        >
          <template v-slot:item="{ props }">
            <span>
              {{ props.item.text }}
            </span>
          </template>
          <template v-slot:tag="{ props: tagProps }">
            <li
              v-if="
                tagProps.index === 0 ||
                (tagProps.index === 1 && value.length > 1)
              "
              class="k-chip"
            >
              <KendoTooltip anchor-element="target" position="top">
                <PillComponent
                  theme-color="white"
                  size="lg"
                  class="font-weight-semi-bold"
                  text-class="is-truncated"
                  closeable
                  @close="removeItem(tagProps.index)"
                >
                  <span class="is-truncated" :title="tagProps.tagData.text">
                    {{ addEllipsis(tagProps.tagData.text, 14) }}
                  </span>
                </PillComponent>
              </KendoTooltip>
            </li>
            <li v-else-if="tagProps.index > 1">+{{ value.length - 2 }}</li>
          </template>
        </MultiSelectTree>

        <KendoError
          v-if="props.touched && errorMessage"
          style="margin-top: 8px"
        >
          {{ errorMessage }}
        </KendoError>
      </KendoFieldWrapper>
    </template>
  </KendoField>
</template>
<style lang="scss">
.multi-select-checkbox {
  width: 100%;
}

.k-treeview-toggle {
  display: none !important;
}

.k-treeview-item[aria-level="1"] {
  padding-left: 0;
}

.k-treeview-item[aria-level="2"] {
  padding-left: 16px;
}

.k-chip-list li:not(:first-child) {
  display: none;
}

.k-chip-list li:nth-child(2) {
  display: inline;
}

.k-chip-list li:nth-child(3) {
  display: inline;
  position: absolute;
  right: 0;
}

.k-clear-value {
  width: 30px !important;
  height: 30px !important;
}

.tag-custom-button {
  border: 1px solid #c1c1c1;
  background: #f0f1f5;
  border-radius: 50%;
  font-size: 12px;
  width: 16px;
  height: 16px;
  padding: 0;
}

.k-chip {
  border: none !important;
  padding-inline: 0 !important;
  padding-block: 0 !important;
}

.k-multiselecttree:before {
  content: url('data:image/svg+xml;utf8,<svg aria-hidden="true" focusable="false" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"><path d="M256 352 128 160h256L256 352z"></path></svg>');
  width: 16px;
  height: 16px;
  position: absolute;
  top: 30%;
  right: 8px;
}

.k-multiselecttree.has-selected:before {
  display: none;
}

.k-input-value-text {
  color: rgb(128, 129, 145);
}
</style>
