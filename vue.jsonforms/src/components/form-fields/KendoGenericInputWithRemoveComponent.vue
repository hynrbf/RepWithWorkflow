<script lang="ts">
import { defineComponent, inject } from "vue";
import { container } from "tsyringe";
import {
  IProfanityService,
  IProfanityServiceInfo,
} from "@/infra/dependency-services/rest/profanity/IProfanityService";
import { StackLayout } from "@progress/kendo-vue-layout";
import { AppConstants } from "@/infra/AppConstants";
import { Emitter, EventType } from "mitt";
import {usePageComponentValidationValueStore} from "@/stores/progress-bar/usePageComponentValidationValueStore";
import {usePageFieldsInvalidHandlerStore} from "@/stores/usePageFieldsInvalidHandlerStore";

// TODO. to get back. Is this still needed? this is currently not in used.
export default defineComponent({
  name: "KendoGenericInputWithRemoveComponent",
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
      default: "Type",
    },
    isCheckForProfanity: {
      type: Boolean,
      default: false,
    },
    isCapitalizeFirstLetter: {
      type: Boolean,
      default: false,
    },
    maxLength: {
      type: Number,
      default: 100,
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
  components: { StackLayout },
  data() {
    return {
      isFieldValidOnFirstLoad: false,
      profanityService: container.resolve<IProfanityService>(
        IProfanityServiceInfo.name,
      ),
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      isUnmounted: false,
      errorMessage: ""
    };
  },
  computed: {
    computedPlaceholder() {
      if (!this.$t("common-placeholder-text")) {
        return this.placeholder;
      }

      return this.$t("common-placeholder-text");
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
  created() {
    if (this.isValueReactive) {
      return;
    }

    this.isFieldValidOnFirstLoad = this.validate(this.value) == "";
  },
  mounted() {
    this.isUnmounted = false;
  },
  unmounted() {
    if (!this.id) {
      return;
    }

    this.removeValidationValueByFieldId(this.id);
    this.isUnmounted = true;
  },

  setup() {
    const pageComponentValidationValueStore =
        usePageComponentValidationValueStore();
    const { addComponentValidationValue, removeValidationValueByFieldId } =
        pageComponentValidationValueStore;

    const pageFieldsInvalidHandlerStore = usePageFieldsInvalidHandlerStore();
    const { addInvalidFieldEntry, clearInvalidEntries } =
        pageFieldsInvalidHandlerStore;

    return {
      addComponentValidationValue,
      addInvalidFieldEntry,
      clearInvalidEntries,
      removeValidationValueByFieldId,
    };
  },

  methods: {
    onInput(event: any) {
      this.isFieldValidOnFirstLoad = false;
      this.$emit("onValueChange", event.target.value);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    },

    capitalizeFirstLetter(word: string): string {
      if (this.isFieldValidOnFirstLoad) {
        return "";
      }

      if (!word || word.length < 1) {
        return "";
      }

      if (!this.isCapitalizeFirstLetter) {
        return word;
      }
      // will work only for firstName and lastName only
      return `${word[0].toUpperCase()}${word.substring(1)}`;
    },

    onRemove() {
      this.$emit("onRemove");
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


    validate(currentValue: string): string {
      this.errorMessage = "";

      if (this.isUnmounted){
        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (this.isValueReactive && !this.isDataLoadedCompletely) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      let errorMessage = `Please enter a valid ${this.label}`;

      if (this.isFieldValidOnFirstLoad) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (this.isValueReactive && !currentValue && this.value) {
        currentValue = this.value;
      }

      if (!this.isRequired) {
        this.addIdKeyAndErrorValue("");
        this.clearInvalidEntries(this.id);
        return "";
      }

      if (!currentValue) {
        this.errorMessage = errorMessage;
        this.addIdKeyAndErrorValue(errorMessage);
        this.clearInvalidEntries(this.id);
        return errorMessage;
      }

      if (
        currentValue &&
        this.isCheckForProfanity &&
        this.profanityService.hasOffensiveWords(currentValue)
      ) {
        this.errorMessage = errorMessage;
        this.addIdKeyAndErrorValue(errorMessage);
        this.addInvalidFieldEntry(this.id);
        return errorMessage;
      }

      this.addIdKeyAndErrorValue("");
      this.clearInvalidEntries(this.id);
      return "";
    },
  },
});
</script>

<template>
  <div>
    <Field
      :id="id"
      :name="name"
      :validator="validate"
      :component="'myTemplate'"
    >
      <template v-slot:myTemplate="{ props }">
        <StackLayout orientation="vertical" :gap="8">
          <StackLayout
            class="justify-content-between"
            orientation="horizontal"
            :align="{ horizontal: 'start', vertical: 'middle' }"
          >
            <Label
              :editor-id="props.id"
              v-if="isRequired"
              class="control-label"
            >
              {{ label }}
              <span v-if="!isRequired" class="fineprint ms-1">
                (Optional)
              </span>
            </Label>

            <Button
              type="button"
              fill-mode="link"
              class="add-manually-btn"
              size="small"
              @click="onRemove"
              >-Remove
            </Button>
          </StackLayout>

          <Input
            v-bind="props"
            :id="props.id"
            :name="props.name"
            :placeholder="computedPlaceholder"
            :valid="!(props.touched && errorMessage)"
            :value="capitalizeFirstLetter(value)"
            @input="onInput"
            :maxlength="maxLength.toString()"
          />

          <Error v-if="props.touched && errorMessage"
            >{{ errorMessage }}
          </Error>
        </StackLayout>
      </template>
    </Field>
  </div>
</template>

<style scoped />