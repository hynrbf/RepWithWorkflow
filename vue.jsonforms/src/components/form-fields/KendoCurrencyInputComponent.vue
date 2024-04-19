<script lang="ts">
import { defineComponent, inject } from "vue";
import { Field } from "@progress/kendo-vue-form";
import { AppConstants } from "@/infra/AppConstants";
import { Emitter, EventType } from "mitt";
import { Money } from "@/entities/Money";
import axios from "axios";
import { ComboBoxChangeEvent } from "@progress/kendo-vue-dropdowns";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";

export default defineComponent({
  name: "KendoCurrencyInputComponent",
  components: {
    Field,
  },
  props: {
    id: String,
    isRequired: {
      type: Boolean,
      default: true,
    },
    isFloorValueEnabled: {
      type: Boolean,
      default: false,
    },
    value: {
      type: Object as () => Money,
      default: new Money(),
    },
    name: {
      type: String,
      default: "",
    },
    label: {
      type: String,
      default: "Amount",
    },
    labelStyle: {
      type: String,
      default: "",
    },
    placeholder: {
      type: String,
      default: "Enter Amount",
    },
    minimumValue: {
      type: Object as () => Money,
      default: new Money(),
    },
    customStyle: {
      type: Object,
      default: () => {
        return { borderWidth: 0 };
      },
    },
    optionalText: {
      type: String,
      default: "(Optional)",
    },
    hasLabel: {
      type: Boolean,
      default: true,
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
    disabled: {
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
      currenciesObj: [] as { name: string; symbol: string }[],
      selectedCurrency: { name: "", symbol: "" } as {
        name: string;
        symbol: string;
      },
      amount: "",
      currency: "GBP",
      symbol: "£",
      isUnmounted: false,
      errorMessage: "",
    };
  },
  watch: {
    isDataLoadedCompletely(newValue, oldValue): boolean {
      if (newValue == oldValue) {
        return newValue;
      }

      this.isFieldValidOnFirstLoad = this.validate(this.amount) == "";
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
  async created() {
    await this.setupCurrencies();
    this.updateValue();

    if (this.isValueReactive) {
      return;
    }

    this.isFieldValidOnFirstLoad = this.validate(this.amount) == "";
  },
  mounted() {
    // Replacing the default dropdown icon
    let pickerButtonIcon = document.querySelector(
      ".currency-drop-down span.k-svg-icon.k-svg-i-caret-alt-down",
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
    this.isUnmounted = false;

    if (this.isValueReactive && this.isDataLoadedCompletely) {
      this.isFieldValidOnFirstLoad = this.validate(this.amount) == "";
    }
  },
  updated() {
    this.updateValue();
    this.isFieldValidOnFirstLoad = this.validate(this.amount) == "";
  },
  unmounted() {
    if (!this.id) {
      return;
    }

    this.removeValidationValueByFieldId(this.id);
    this.isUnmounted = true;
  },
  methods: {
    updateValue() {
      this.amount = this.value?.amount?.toString() ?? "";
      this.currency = this.value?.currency ?? "GBP";
      this.symbol = this.value?.symbol ?? "£";
      this.selectedCurrency = {
        name: this.currency,
        symbol: this.symbol,
      };
    },

    onInput(event: any) {
      this.amount = this.formatMoneyWithComma(event.target.value.toString());
    },

    onChange(event: any) {
      this.isFieldValidOnFirstLoad = false;
      const amountWithComma = event.target.value;

      if (!amountWithComma) {
        return;
      }

      let actualAmount = parseFloat(amountWithComma.replace(/,/g, ""));

      if (isNaN(actualAmount)) {
        return;
      }

      const money: Money = {
        currency: this.currency,
        amount: actualAmount,
        symbol: this.symbol,
      };

      this.$emit("onValueChange", money);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    },

    onCurrencySelect(event: ComboBoxChangeEvent) {
      this.selectedCurrency = event.value as { name: string; symbol: string };
      this.currency = this.selectedCurrency.name;
      this.symbol = this.selectedCurrency.symbol;

      // Todo. supported only for euro and pound
      if (
        this.isFloorValueEnabled &&
        this.currency != "GBP" &&
        this.currency != "EUR"
      ) {
        this.selectedCurrency = { name: "GBP", symbol: "£" };
        this.currency = this.selectedCurrency.name;
        this.symbol = this.selectedCurrency.symbol;
      }
    },

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

      const errorMessage = `Please enter a valid ${this.label}`;

      if (this.isFieldValidOnFirstLoad) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (this.isValueReactive && !currentValue && this.value) {
        currentValue = this.value?.amount?.toString() ?? "";
      }

      if (!this.isRequired) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (!currentValue) {
        this.addIdKeyAndErrorValue(errorMessage);
        this.errorMessage = errorMessage;
        return errorMessage;
      }

      if (!this.isFloorValueEnabled) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (
        !(
          this.minimumValue.currency === "GBP" && this.value?.currency === "GBP"
        )
      ) {
        this.addIdKeyAndErrorValue(errorMessage);
        this.errorMessage = errorMessage;
        return errorMessage;
      }

      currentValue = currentValue.replace(/,/g, "");
      const numericValue = parseFloat(currentValue);
      const euroAmount = this.convertPoundToEuro(numericValue);

      if (!this.minimumValue.amount || euroAmount < this.minimumValue.amount) {
        this.addIdKeyAndErrorValue(errorMessage);
        this.errorMessage = errorMessage;
        return errorMessage;
      }

      this.addIdKeyAndErrorValue("");
      return "";
    },

    formatMoneyWithComma(data: string) {
      if (!data) {
        return "";
      }

      data = data.replace(/[^0-9.]/g, "");
      return data.replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    },

    async setupCurrencies() {
      axios.get("/api/countries-all.json").then(({ data }) => {
        const currencies = data.map(({ currencies }: any) => currencies);
        // Extract values and create the new structure

        for (const currency of currencies) {
          if (!currency) {
            continue;
          }

          const name = Object.keys(currency)[0];
          const symbol = currency[name].symbol;

          if (name && symbol) {
            const currencyObj = { name: name, symbol: symbol };
            // Convert the list to a set to ensure uniqueness
            const objectSet = new Set(
              this.currenciesObj.map((obj) => obj.name),
            );

            // Check if the new object's name is not already in the set
            if (!objectSet.has(currencyObj.name)) {
              // Add the new object to the list
              this.currenciesObj.push(currencyObj);
            }
          }
        }

        const compareByName = (a: { name: string }, b: { name: string }) => {
          return a.name.localeCompare(b.name);
        };

        this.currenciesObj.sort(compareByName);
      });
    },

    // since the exchange rate are changing every minute, we need to use the latest exchange rate API
    // but for now, we are using the static exchange rate
    convertPoundToEuro(amount: number): number {
      return amount * 1.16;
    },

    //will be used soon...
    convertEuroToPound(amount: number): number {
      return amount / 1.16;
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
        <StackLayout class="pb-2" orientation="vertical">
          <Label
            v-if="hasLabel"
            :editor-id="props.id"
            class="pb-2 control-label"
            :style="labelStyle"
          >
            {{ label }}
            <span v-if="!isRequired" class="fineprint ms-1">
              {{ optionalText }}
            </span>
          </Label>

          <StackLayout
            class="control-frame kendo-currency-input-component"
            :class="props.touched && errorMessage ? 'currency-error' : ''"
            orientation="horizontal"
            :align="{ horizontal: 'start', vertical: 'stretch' }"
          >
            <span
              class="currency-symbol d-flex align-items-center justify-content-center"
              >{{ symbol }}</span
            >

            <Input
              v-bind="props"
              :id="props.id"
              :name="props.name"
              :placeholder="placeholder"
              :valid="!(props.touched && errorMessage)"
              :value="formatMoneyWithComma(amount)"
              :fillMode="customStyle ? null : 'solid'"
              :style="customStyle ?? ''"
              :disabled="disabled"
              size="small"
              @input="onInput"
              @change="onChange"
            />

            <KendoComboBox
              class="currency-drop-down"
              placeholder="GBP"
              :data-items="currenciesObj"
              :text-field="'name'"
              :data-item-key="'name'"
              :value="selectedCurrency"
              :clearButton="false"
              :allow-custom="true"
              :fillMode="null"
              :disabled="disabled"
              @change="onCurrencySelect"
              style="border-width: 0"
            />
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
.control-frame {
  border: var(--content-content-07) solid thin;
  border-radius: 8px;
}

.currency-symbol {
  background: var(--content-content-10);
  border-radius: 8px 0 0 8px;
  color: var(--text-text-disabled);
  padding: 0 12px;
  border-right: var(--content-content-07) solid thin;
}

.currency-error {
  border-color: red;
}
</style>