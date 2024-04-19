<script lang="ts">
import { defineComponent } from "vue";
import { ComboBoxAll } from "@progress/kendo-vue-dropdowns/dist/npm/ComboBox/ComboBox";
import { container } from "tsyringe";
import {
  IFormValidatorService,
  IFormValidatorServiceInfo,
} from "@/infra/dependency-services/form-validator/IFormValidatorService";
import { AppConstants } from "@/infra/AppConstants";
import axios from "axios";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";

export default defineComponent({
  name: "KendoCountryComponent",
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
      default: "United Kingdom",
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
    loading: {
      type: Boolean,
      default: false,
    },
    value: {
      type: String,
      default: AppConstants.DefaultCountryCode.toUpperCase(),
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
      isLoading: false,
      items: [] as { label: string; value: string }[],
      formValidatorService: container.resolve<IFormValidatorService>(
        IFormValidatorServiceInfo.name,
      ),
      defaultCountryCode: AppConstants.DefaultCountryCode.toUpperCase(),
      isFieldValidOnFirstLoad: false,
      isUnmounted: false,
      errorMessage: "",
    };
  },
  watch: {
    value(newVal) {
      if (newVal) {
        this.defaultCountryCode = newVal.toUpperCase();
      }
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
    this.setupElement();
    this.setupList();
    this.isUnmounted = false;

    if (this.isValueReactive && this.isDataLoadedCompletely) {
      this.isFieldValidOnFirstLoad = this.validate(this.value) == "";
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
    setupElement() {
      const dropdownElement = this.$refs["dropdownElement"] as ComboBoxAll;
      const flagElement = this.$refs["flagElement"] as HTMLSpanElement;

      if (!dropdownElement?.element?.querySelector(".k-button") || !flagElement) {
        return;
      }

      const button = dropdownElement?.element?.querySelector("button");
      button.innerHTML = "";
      button.appendChild(flagElement);
    },

    setupList() {
      this.isLoading = true;
      axios.get("/api/countries.json").then(({ data }) => {
        this.items = data.map(({ name, code }: any) => ({
          label: name,
          value: code,
        }));

        this.isLoading = false;
      });
    },

    validate(currentValue: string) {
      //note, even if you remove the component in the DOM 'using v-if', the vue still preserves it in memory
      //that's why we need this variable
      if (this.isUnmounted) {
        return "";
      }

      this.formValidatorService.setFieldName(this.label || this.name);
      const errorMessage = this.formValidatorService.validate<string>(
        currentValue,
        {
          [AppConstants.RequiredKey]: this.isRequired,
        },
      );
      this.addIdKeyAndErrorValue(errorMessage);
      this.errorMessage = errorMessage;
      return errorMessage;
    },

    handleChange(event: any, onChange: (event: any) => void) {
      onChange(event);
      this.$emit("onValueChange", event.target.value);
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
  <KendoField :id="id" :name="name" component="template" :validator="validate">
    <template v-slot:template="{ props }">
      <KendoFieldWrapper :class="props.class">
        <KendoLabel
          v-if="label"
          :editor-id="id"
          :disabled="disabled"
          :valid="!(props.touched && errorMessage)"
          style="margin-bottom: 6px"
          class="control-label"
        >
          {{ label }}
          <span v-if="!isRequired" class="fineprint ms-1"> (Optional) </span>
        </KendoLabel>

        <KendoComboBox
          ref="dropdownElement"
          class="CountryList"
          :data-items="items"
          data-item-key="value"
          text-field="label"
          value-field="value"
          :value-primitive="true"
          size="large"
          item-render="item"
          :loading="loading || isLoading"
          :placeholder="placeholder"
          :value="value?.toUpperCase() || null"
          @change="handleChange($event, props.onChange)"
          :style="{
            borderColor: !(props.touched && errorMessage) ? '' : '#f31700',
          }"
          style="border-radius: 8px"
        >
          <template #item="{ props }">
            <li :class="['k-item', props.class]" @click="props.onClick">
              <IconComponent
                type="flag"
                :symbol="props.dataItem.value?.toUpperCase()"
                size="20"
              />
              {{ props.dataItem.label }}
            </li>
          </template>
        </KendoComboBox>

        <span
          ref="flagElement"
          class="CountryList-flag"
          style="margin-left: 8px"
        >
          <IconComponent
            type="flag"
            :symbol="value ?? defaultCountryCode"
            size="20"
            style="margin-right: 2px"
          />
          <svg
            style="margin-right: 6px"
            width="10"
            height="6"
            viewBox="0 0 10 6"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M9.0835 1.24992L5.70727 4.62614C5.31675 5.01667 4.68358 5.01667 4.29306 4.62614L0.916829 1.24992"
              stroke="black"
              stroke-width="1.5"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
          </svg>
          <div
            style="
              position: absolute;
              height: 100%;
              right: 0%;
              border-right: 1px solid var(--content-content-07);
            "
          />
        </span>

        <KendoError
          v-if="props.touched && errorMessage"
          style="margin-top: 6px"
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
.CountryList {
  display: flex;

  :deep(> *) {
    order: 2;
  }

  :deep(.k-button) {
    order: 1;
    border-left-width: 0;
    border-right-width: 1px;
    width: auto;
  }

  &-flag {
    display: flex;
    gap: 5px;
    align-items: center;
  }
}
</style>