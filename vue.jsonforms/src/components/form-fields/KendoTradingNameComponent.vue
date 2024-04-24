<script lang="ts">
import { defineComponent, inject } from "vue";
import { AppConstants } from "@/infra/AppConstants";
import { Emitter, EventType } from "mitt";
import { StackLayout } from "@progress/kendo-vue-layout";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";

export default defineComponent({
  name: "KendoTradingNameComponent",
  computed: {
    AppConstants() {
      return AppConstants;
    },
  },
  components: { StackLayout },
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
    value: {
      type: Array as () => string[],
      default: [] as string[],
    },
    optionalText: {
      type: String,
      default: "Optional",
    },
    isUserModified: {
      type: Boolean,
      default: false,
    },
    isShowAllOnPopup: {
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
  data() {
    return {
      isFieldValidOnFirstLoad: false,
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      isDropdownOpened: false,
      additionalTradingAddress: undefined as string | undefined,
      valuesInternal: this.value,
      // TODO: This is for trading address implemenation. To continue
      // currentScrollHeight: 0,
      amberBorderColor: "var(--color-warning-600)",
      defaultBorderColor: "var(--content-content-07)",
      errorBorderColor: "#F31700",
      isUnmounted: false,
      errorMessage: "",
    };
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

    this.valuesInternal = this.value;
    this.isFieldValidOnFirstLoad =
      this.validate(this.valuesInternal as string[]) == "";
  },
  mounted() {
    this.isUnmounted = false;
  },
  updated() {
    this.valuesInternal = this.value;
    this.isFieldValidOnFirstLoad =
      this.validate(this.valuesInternal as string[]) == "";
  },
  unmounted() {
    if (!this.id) {
      return;
    }

    this.removeValidationValueByFieldId(this.id);
    this.isUnmounted = true;
  },
  methods: {
    onItemClear(selectedItem: string, index?: number) {
      this.isFieldValidOnFirstLoad = false;
      this.$emit("onRemoveItem", selectedItem, index);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);

      (this.kendoForm as any)?.onChange(this.name, {
        value: this.valuesInternal,
      });
    },

    onAddOther() {
      if (!this.additionalTradingAddress) {
        return;
      }

      if (this.additionalTradingAddress.trim().length === 0) {
        this.additionalTradingAddress = undefined;
        return;
      }

      this.valuesInternal.unshift(this.additionalTradingAddress);
      this.$emit("onValueChange", this.valuesInternal);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
      this.additionalTradingAddress = undefined;

      (this.kendoForm as any)?.onChange(this.name, {
        value: this.valuesInternal,
      });
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

      if (!(currentValue && currentValue.length)) {
        const errorMessage = "Please enter valid Trading Name(s)";
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

    getBorderColor(isValid: boolean): string {
      if (!isValid) {
        return this.errorBorderColor;
      }

      return this.isUserModified
        ? this.amberBorderColor
        : this.defaultBorderColor;
    },
  },
  inject: {
    kendoForm: { default: {} },
  },
});
</script>

<template>
  <Field :id="id" :name="name" :component="'myTemplate'" :validator="validate">
    <template v-slot:myTemplate="{ props }">
      <div class="d-flex flex-column">
        <div class="d-flex" style="gap: 15px">
          <div class="d-flex trading-container">
            <Label
              class="control-label paragraph-s-figtree-medium"
              style="margin-bottom: 6px"
              >Trading Name(s)</Label
            >

            <!-- TODO. add this for trading name impl. 'style="height: 40px"' -->
            <MultiSelect
              ref="multiselect"
              v-bind="props"
              :id="props.id"
              :name="props.name"
              :placeholder="placeholder"
              :value="valuesInternal.sort((a, b) => a.localeCompare(b))"
              :opened="isDropdownOpened"
              size="small"
              :tag-render="'myTag'"
              :style="{
                borderColor: getBorderColor(!(props.touched && errorMessage)),
              }"
              class="tags-select"
              :fillMode="null"
            >
              <template v-slot:myTag="{ props }">
                <StackLayout
                  class="trading-name-tag-item is-truncated"
                  orientation="horizontal"
                  :gap="8"
                  :align="{ horizontal: 'start', vertical: 'middle' }"
                >
                  <Label
                    style="color: var(--color-black); display: block"
                    class="is-truncated"
                    >{{ props.tagData.text }}</Label
                  >
                  <a
                    @click="
                      () => onItemClear(props.tagData.data[0], props.index)
                    "
                    style="cursor: pointer"
                  >
                    <img :src="'/icons/clear-cicle-solid.svg'" alt="arrow" />
                  </a>
                </StackLayout>

                <SnipItemsComponent
                  v-if="props.index === valuesInternal.length - 1"
                  :items="valuesInternal"
                  :limit="4"
                  :isShowAll="isShowAllOnPopup"
                  popup-title="Trading Names"
                >
                  <template #visible-items>
                    <span></span>
                  </template>
                  <template #popup-item="{ item, index }">
                    <div class="hidden-items">
                      <Label class="hidden-item-text">{{ item }}</Label>

                      <a
                        @click="
                          () => {
                            eventBus.emit(
                              AppConstants.closeSnipItemsPopupEvent,
                            );
                            onItemClear(<string>item, index);
                          }
                        "
                        style="cursor: pointer"
                      >
                        <img src="/icons/clear-cicle-solid.svg" alt="arrow" />
                      </a>
                    </div>
                  </template>
                </SnipItemsComponent>
              </template>
            </MultiSelect>
          </div>

          <div class="d-flex add-trading-container">
            <Label
              class="control-label paragraph-s-figtree-medium"
              style="margin-bottom: 6px"
              >Add Trading Name</Label
            >

            <div class="d-flex add-input-frame">
              <KendoGenericInputComponent
                id="add-textbox"
                class="flex-grow-1"
                :isRequired="false"
                :value="additionalTradingAddress"
                :customStyle="{
                  borderWidth: '0px',
                  paddingTop: '5px',
                  paddingBottom: '5px',
                  paddingRight: '20px',
                }"
                placeholder="Type"
                @onValueChange="(v: string) => (additionalTradingAddress = v)"
              />

              <a
                @click="onAddOther"
                class="p-2 flex-grow-0"
                style="cursor: pointer"
              >
                <img :src="'/icons/add-circle-27.svg'" alt="arrow" />
              </a>
            </div>
          </div>
        </div>

        <Error
          class="error"
          v-if="props.touched && errorMessage"
          style="margin-top: 4px"
          >{{ errorMessage }}
        </Error>
      </div>
    </template>
  </Field>
</template>

<style scoped>
.trading-name-tag-item {
  padding: 2px 2px 2px 10px;
  border: 1px solid var(--neutral-gray-300);
  border-radius: 20px;
}

.tag-clear {
  color: var(--text-text-disabled);
  scale: 1.5;
  cursor: pointer;
}

.add-input-frame {
  border-radius: 8px;
  display: flex;
  flex-direction: row;
  justify-content: start;
  border: 1px solid var(--content-content-07);
  background: #fff;
}

.add-input-frame span {
  cursor: pointer;
}

.trading-container {
  width: 67%;
  flex-direction: column;
}

.add-trading-container {
  width: 33%;
  flex-direction: column;
}

.hidden-items {
  display: flex;
  padding: 2px 2px 2px 10px;
  margin-bottom: 4px;
  align-items: center;
  justify-content: space-between;
  gap: 4px;
  align-self: stretch;
  border-radius: 35px;
  border: 1px solid var(--content-content-07);
}

.hidden-item-text {
  color: var(--color-black);
  display: block;
  white-space: nowrap; /* Prevents the text from wrapping to the next line */
  overflow: hidden; /* Hides any content that overflows the container */
  text-overflow: ellipsis; /* Displays an ellipsis (...) for truncated text */
}

:global(.tags-select .k-chip-list) {
  display: flex !important;
  flex-wrap: nowrap;
  width: 100%;
}

:global(.tags-select .trading-name-tag-item) {
  max-width: 22%;
  display: none;
}

:global(.tags-select .trading-name-tag-item:nth-child(1)),
:global(.tags-select .trading-name-tag-item:nth-child(2)),
:global(.tags-select .trading-name-tag-item:nth-child(3)),
:global(.tags-select .trading-name-tag-item:nth-child(4)) {
  display: flex;
}
</style>
