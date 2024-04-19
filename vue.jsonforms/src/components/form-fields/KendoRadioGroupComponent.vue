<script lang="ts">
import { defineComponent, inject } from "vue";
import { Emitter, EventType } from "mitt";
import { AppConstants } from "@/infra/AppConstants";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";

export default defineComponent({
  name: "KendoRadioGroupComponent",
  props: {
    id: String,
    value: {
      type: String,
      default: "",
    },
    dataItems: {
      type: Array,
      default: [],
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
      default: "",
    },
    layout: {
      type: String,
      default: "horizontal",
    },
    isValueReactive: {
      type: Boolean,
      default: false,
    },
    isDataLoadedCompletely: {
      type: Boolean,
      default: false,
    },
    labelForErrorMessageInternal: {
      type: String,
      default: "Selection",
    },
    isRequired: {
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
      isUnmounted: false,
      errorMessage: "",
    };
  },
  methods: {
    onRadioChange(event: any) {
      this.isFieldValidOnFirstLoad = false;
      if (event) {
        this.$emit("onValueChange", event.value);
      }
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
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

      if (this.isValueReactive && !currentValue && this.value) {
        setTimeout(() => (currentValue = this.value), 0);
      }

      if (!this.isRequired) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (!currentValue) {
        let errorMessage = `Please enter a valid ${this.labelForErrorMessageInternal}`;
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
});
</script>
<template>
  <div>
    <Field :name="name" :id="id" :component="'myTemplate'">
      <template v-slot:myTemplate="{ props }">
        <StackLayout orientation="vertical" style="gap: 6px">
          <Label
            v-if="label !== ''"
            :editor-id="props.id"
            class="control-label"
          >
            {{ label }}
            <span v-if="!isRequired" class="fineprint ms-1"> (Optional) </span>
          </Label>

          <RadioGroup
            :id="props.id"
            v-bind="props"
            :data-items="dataItems"
            :value="value"
            :layout="layout"
            @change="onRadioChange($event)"
          />
          <Error style="margin-top: 4px" v-if="props.touched && errorMessage">
            {{ errorMessage }}
          </Error>
        </StackLayout>
      </template>
    </Field>
  </div>
</template>
