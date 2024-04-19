<script lang="ts">
import { defineComponent, inject } from "vue";
import { AppConstants } from "@/infra/AppConstants";
import { Emitter, EventType } from "mitt";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";

export default defineComponent({
  name: "KendoAutoCompleteInputComponent",
  props: {
    id: String,
    name: {
      type: String,
      default: "",
    },
    label: {
      type: String,
      default: "Company",
    },
    itemsSource: {
      type: Object as () => string[],
      default: [] as string[],
    },
    value: {
      type: String,
      default: "",
    },
    placeholder: {
      type: String,
      default: "Type",
    },
    optionalText: {
      type: String,
      default: "(Optional)",
    },
    isRequired: {
      type: Boolean,
      default: true,
    },
    hasDropdown:{
      type: Boolean,
      default: false
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
      debounceTimer: null as NodeJS.Timeout | null,
      debounceTimerInMs: 500,
      valueInternal: "",
      isOpened: false,
      filteredItems: [] as string[],
      isUnmounted: false,
      errorMessage: "",
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
    this.filteredItems = this.itemsSource;
    this.valueInternal = this.value;
  },
  mounted() {
    this.isUnmounted = false;

    if (this.isValueReactive && this.isDataLoadedCompletely) {
      this.isFieldValidOnFirstLoad = this.validate(this.value) == "";
    }
  },
  updated() {
    this.isFieldValidOnFirstLoad = this.validate(this.value) == "";
    this.filteredItems = this.itemsSource;
    this.valueInternal = this.value;
  },
  unmounted() {
    if (!this.id) {
      return;
    }

    this.removeValidationValueByFieldId(this.id);
    this.isUnmounted = true;
  },
  methods: {
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

    onInput(event: any) {
      if (this.debounceTimer) {
        clearTimeout(this.debounceTimer);
      }

      if (!event.value && event?.type === "change") {
        // prevent changing the value except for selection and input
        this.isOpened=false;
        return;
      }

      this.isOpened=true;
      this.valueInternal = event.target.value;
      this.$emit("onValueChange", this.valueInternal);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);

      this.debounceTimer = setTimeout(() => {
        if (!this.valueInternal || this.valueInternal.length < 3) {
          this.filteredItems = this.itemsSource;
        } else {
          this.filteredItems = this.itemsSource.filter((i) =>
            i.includes(this.valueInternal),
          );
        }
      }, this.debounceTimerInMs);
    },

    onSelect(event: any) {
      let selectedNatureOfBusiness = event.target.value as string;
      this.$emit("onValueChange", selectedNatureOfBusiness);
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
            <span v-if="!isRequired" class="fineprint ms-1">
              {{ optionalText }}
            </span>
          </Label>

          <StackLayout
            orientation="horizontal"
            style="padding-left: 12px; margin-right: 5px"
            :class="
              !(props.touched && errorMessage)
                ? 'control-container'
                : 'control-container-error'
            "
            :align="{ horizontal: 'start', vertical: 'middle' }"
          >
            <IconComponent symbol="search-icon" size="20" class="magnifier" />

            <AutoComplete
              v-bind="props"
              :valid="!(props.touched && errorMessage)"
              class="input"
              fill-mode="none"
              :placeholder="computedPlaceholder"
              :data-items="filteredItems"
              :value="valueInternal"
              :opened="isOpened"
              @change="onInput"
            />

            <IconComponent
                v-if="hasDropdown"
                :symbol="isOpened ?`arrow-up-3-10` : `arrow-down-3-79`"
                class="dropdown"
                size="15"
                @click="isOpened = !isOpened"/>
          </StackLayout>

          <Error v-if="props.touched && errorMessage"
            >{{ errorMessage }}
          </Error>
        </StackLayout>
      </template>
    </Field>
  </div>
</template>

<style scoped>
.input {
  background-color: white;
  border-width: 0;
}

.dropdown {
  z-index: 1;
  right: 12px;
}
</style>
