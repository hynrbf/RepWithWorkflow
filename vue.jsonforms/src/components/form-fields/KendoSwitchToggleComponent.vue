<script lang="ts">
import { container } from "tsyringe";
import { defineComponent } from "vue";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";
import {
  IFormValidatorService,
  IFormValidatorServiceInfo,
} from "@/infra/dependency-services/form-validator/IFormValidatorService";

export default defineComponent({
  name: "KendoSwitchToggleComponent",
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
    modelValue: {
      type: Boolean,
      default: false,
    },
    value: {
      type: Boolean,
      default: false,
    },
    noText: {
      type: Boolean,
      default: false,
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
  created() {
    if (this.isValueReactive) {
      return;
    }

    this.isFieldValidOnFirstLoad = this.validate(this.value) == "";
  },
  mounted() {
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
  data() {
    return {
      isFieldValidOnFirstLoad: false,
      formValidatorService: container.resolve<IFormValidatorService>(
        IFormValidatorServiceInfo.name,
      ),
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
  watch: {
    isDataLoadedCompletely(newValue, oldValue): boolean {
      if (newValue == oldValue) {
        return newValue;
      }

      this.isFieldValidOnFirstLoad = this.validate(this.value) == "";
      return newValue;
    },
  },
  methods: {
    validate: function (currentValue?: boolean) {
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

      //It started as string type, needs further checking why and retest
      if (this.isRequired && currentValue === undefined) {
        const errorMessage = `${this.label || this.name} is required.`;
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

        <KendoSwitch
          v-bind="props"
          :class="[noText && 'is-no-text']"
          size="small"
          :value="modelValue || value || false"
          :disabled="disabled"
          @change="
            ($event: any) => {
              props.onChange($event);
              $emit('update:modelValue', $event.value);
              $emit('onValueChange', $event.value);
            }
          "
        />

        <KendoError v-if="props.touched && errorMessage">
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
.is-no-text {
  :deep(.k-switch-label-on),
  :deep(.k-switch-label-off) {
    opacity: 0;
    visibility: hidden;
  }
}
</style>