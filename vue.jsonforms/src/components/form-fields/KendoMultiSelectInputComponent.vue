<script lang="ts">
import { defineComponent, inject } from "vue";
import { MultiSelectChangeEvent } from "@progress/kendo-vue-dropdowns";
import { AppConstants } from "@/infra/AppConstants";
import { Emitter, EventType } from "mitt";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";

export default defineComponent({
  name: "KendoMultiSelectInputComponent",
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
      default: "",
    },
    isRequired: {
      type: Boolean,
      default: true,
    },
    dataItems: {
      type: Array,
      default: [] as string[],
    },
    value: {
      type: Array,
      default: [] as string[],
    },
    optionalText: {
      type: String,
      default: "Optional",
    },
    checkbox: Boolean,
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

      this.isFieldValidOnFirstLoad =
        this.validate(this.value as string[]) == "";
      return newValue;
    },
  },
  data() {
    return {
      isFieldValidOnFirstLoad: false,
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      isUnmounted: false,
      errorMessage: ""
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

    this.isFieldValidOnFirstLoad = this.validate(this.value as string[]) == "";
  },
  mounted() {
    this.isUnmounted = false;

    if (this.isValueReactive && this.isDataLoadedCompletely) {
      this.isFieldValidOnFirstLoad =
        this.validate(this.value as string[]) == "";
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
    onSelect(event: MultiSelectChangeEvent) {
      this.isFieldValidOnFirstLoad = false;
      this.$emit("onValueChange", event.value as string[] as string[]);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    },

    onRemove(indexToRemove: number) {
      this.isFieldValidOnFirstLoad = false;
      (this.value as string[]).splice(indexToRemove, 1);
      this.$emit("onValueChange", this.value as string[]);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    },

    onBlur() {
      // For some reason, validate is not called when removing
      // item, even when unfocused, thus using the onBlur method to
      // manually validate the value
      this.isFieldValidOnFirstLoad =
        this.validate(this.value as string[]) == "";
    },

    validate(currentValue: string[]) {
      this.errorMessage = "";
      //note, even if you remove the component in the DOM 'using v-if', the vue still preserves it in memory
      //that's why we need this variable
      if (this.isUnmounted) {
        return "";
      }

      if (!this.isRequired) {
        this.addIdKeyAndErrorValue("");
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

      if (!currentValue || currentValue.length < 1) {
        const errorMessage = `Please enter a valid ${this.label}`;
        this.addIdKeyAndErrorValue(errorMessage);
        this.errorMessage = errorMessage;
        return errorMessage;
      }

      this.addIdKeyAndErrorValue("");
      return "";
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
      :id="id"
      :name="name"
      :component="'myTemplate'"
      :validator="validate"
    >
      <template v-slot:myTemplate="{ props }">
        <StackLayout class="mb-2" orientation="vertical" :gap="8">
          <Label :editor-id="props.id" class="control-label">
            {{ label }}
            <span v-if="!isRequired" class="fineprint ms-1">
              ({{ optionalText }})
            </span>
          </Label>

          <MultiSelect
            v-bind="props"
            :id="props.id"
            :name="props.name"
            :valid="!(props.touched && errorMessage)"
            :placeholder="placeholder"
            :value="value"
            size="small"
            @change="onSelect"
            @focus="props.focus"
            @blur="onBlur"
            :data-items="dataItems"
            tag-render="tag"
            item-render="item"
          >
            <template #tag="{ props: tagProps }">
              <KendoTooltip anchor-element="target" position="top">
                <PillComponent
                  theme-color="white"
                  size="lg"
                  class="font-weight-semi-bold"
                  text-class="is-truncated"
                  closeable
                  @close="onRemove(tagProps.index)"
                >
                  <span class="is-truncated" :title="tagProps.tagData.text">
                    {{ tagProps.tagData.text }}
                  </span>
                </PillComponent>
              </KendoTooltip>
            </template>

            <template #item="{ props: itemProps }">
              <li
                v-bind="itemProps"
                :class="[itemProps.selected && checkbox ? 'is-checked' : '']"
              >
                <KendoCheckbox v-if="checkbox" :checked="itemProps.selected" />
                &nbsp;
                {{ itemProps.dataItem }}
              </li>
            </template>
          </MultiSelect>

          <Error v-if="props.touched && errorMessage"
            >{{ errorMessage }}
          </Error>
        </StackLayout>
      </template>
    </Field>
  </div>
</template>

<style scoped>
.is-checked {
  background-color: transparent !important;
}
</style>
