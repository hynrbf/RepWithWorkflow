<script lang="ts">
import { defineComponent, inject } from "vue";
import { container } from "tsyringe";
import intlTelInput from "intl-tel-input";
import { MaskedTextBoxAll } from "@progress/kendo-vue-inputs";
import { ComboBoxAll } from "@progress/kendo-vue-dropdowns/dist/npm/ComboBox/ComboBox";
import {
  IFormValidatorService,
  IFormValidatorServiceInfo,
} from "@/infra/dependency-services/form-validator/IFormValidatorService";
import { AppConstants } from "@/infra/AppConstants";
import { Emitter, EventType } from "mitt";
import { ContactNumber } from "@/entities/ContactNumber";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";
import { usePageFieldsInvalidHandlerStore } from "@/stores/usePageFieldsInvalidHandlerStore";

export default defineComponent({
  name: "KendoTelInputComponent",
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
    value: {
      type: Object as () => ContactNumber,
      default: new ContactNumber(),
    },
    modelValue: {
      type: Object as () => ContactNumber,
      default: new ContactNumber(),
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
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      formValidatorService: container.resolve<IFormValidatorService>(
        IFormValidatorServiceInfo.name,
      ),
      options: [],
      iti: null as intlTelInput.Plugin | null,
      currentCountry: "",
      currentCountryCode: "",
      currentDial: "",
      mask: "",
      isFieldValidOnFirstLoad: false,
      isUserDidManualChange: false,
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
        this.validate(this.value?.number ?? "") == "";
      return newValue;
    },

    iti(newValue, _oldValue): intlTelInput.Plugin | null {
      if (this.iti && this.value?.countryCode) {
        this.iti.setCountry(this.value.countryCode);
      }

      return newValue;
    },
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
      removeValidationValueByFieldId,
      addInvalidFieldEntry,
      clearInvalidEntries,
    };
  },
  created() {
    if (this.isValueReactive) {
      return;
    }

    this.isFieldValidOnFirstLoad =
      this.validate(this.value?.number ?? "") == "";
  },
  mounted() {
    this.isUnmounted = false;
    this.$nextTick(() => {
      this.setupInput();
      this.setupInternationalTelInput();

      if (!this.id) {
        return;
      }

      this.eventBus.on(
        `${AppConstants.pageFieldInvalidEvent}-${this.id}`,
        () => {
          // Replace invalid value with empty string
          this.value.number = "";
          this.$emit("onValueChange", this.value);
          this.$emit("update:modelValue", this.value);
          this.eventBus.emit(AppConstants.formFieldChangedEvent);
          this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
          this.clearInvalidEntries(this.id);
        },
      );
    });

    const onCollapsiblePanelExpand = inject<(callback: () => void) => void>(
      "collapsiblePanel:expanded",
    );

    onCollapsiblePanelExpand?.(() => {
      this.$nextTick(() => {
        this.setupInput();
        this.setupInternationalTelInput();
      });
    });

    this.$watch(
      (vm) => [vm.modelValue, vm.value],
      ([modelValue, value]) => {
        (this.kendoForm as any).onChange(this.name, {
          value: modelValue?.number || value?.number || "",
        });
      },
    );

    if (this.isValueReactive && this.isDataLoadedCompletely) {
      this.isFieldValidOnFirstLoad =
        this.validate(this.value?.number ?? "") == "";
    }

    if (this.isValueReactive && this.isDataLoadedCompletely) {
      this.isFieldValidOnFirstLoad =
        this.validate(this.value?.number ?? "") == "";
    }
  },
  updated() {
    this.isFieldValidOnFirstLoad =
      this.validate(this.value?.number ?? "") == "";

    if (!this.iti || !this.value?.countryCode) {
      return;
    }

    this.iti.setCountry(this.value.countryCode);
  },
  unmounted() {
    if (!this.id) {
      return;
    }

    this.eventBus.off(`${AppConstants.pageFieldInvalidEvent}-${this.id}`);
    this.removeValidationValueByFieldId(this.id);
    this.isUnmounted = true;
  },
  methods: {
    setupInput() {
      const inputElement = this.$refs["inputElement"] as MaskedTextBoxAll;
      const dropdownElement = this.$refs["dropdownElement"] as ComboBoxAll;
      const flagElement = this.$refs["flagElement"] as HTMLSpanElement;

      if (!inputElement || !dropdownElement?.input?.offsetParent) return;

      dropdownElement.input.offsetParent.appendChild(
        inputElement.element?.offsetParent,
      );

      const button = dropdownElement.input.offsetParent.querySelector("button");
      if (button && flagElement) {
        button.innerHTML = "";
        button.appendChild(flagElement);
      }

      dropdownElement.input.remove();
    },

    setupInternationalTelInput() {
      const inputElement = this.$refs["inputElement"] as MaskedTextBoxAll;
      const input = inputElement?.input;

      if (!input) return;

      const iti = intlTelInput(input, {
        allowDropdown: false,
        // @ts-ignore
        showFlags: false,
        initialCountry: AppConstants.DefaultCountryCode,

        // ToDo. Not needed for now.
        // geoIpLookup: function (callback: (payload: string) => void) {
        //   fetch("https://ipapi.co/json")
        //     .then(function (res) {
        //       return res.json();
        //     })
        //     .then(function (data) {
        //       callback(data.country_code);
        //     })
        //     .catch(function () {
        //       callback(AppConstants.DefaultCountryCode);
        //     });
        // },
        utilsScript:
          // https://github.com/jackocnr/intl-tel-input#recommended-usage
          // We highly recommend you (lazy) load the included utils.js using the
          // utilsScript option. Then the plugin is built to always deal with numbers in
          // the full international format (e.g. "+17024181234") and convert them
          // accordingly - even when nationalMode or separateDialCode is enabled. We
          // recommend you get, store, and set numbers exclusively in this format for
          // simplicity - then you don't have to deal with handling the country code
          // separately, as full international numbers include the country code
          // information.
          "https://cdn.jsdelivr.net/npm/intl-tel-input@18.2.1/build/js/utils.js",
      });

      this.iti = iti;

      iti.promise.then(() => {
        this.setOptions();
        this.setMask(input);
        const selected = iti.getSelectedCountryData();
        this.currentCountry = selected.name;
        this.currentCountryCode = selected.iso2;
        this.currentDial = selected.dialCode;
      });

      input.addEventListener("countrychange", () => {
        this.$nextTick(() => {
          this.setMask(input);
          const selected = iti.getSelectedCountryData();
          this.currentCountry = selected.name;
          this.currentCountryCode = selected.iso2;
          this.currentDial = selected.dialCode;
        });
      });
    },

    setOptions() {
      if (!this.iti) return;

      this.options = ((this.iti as any).countries || []).map((country: any) => {
        return {
          name: country.name,
          code: country.iso2,
          dial: country.dialCode,
        };
      });
    },

    setMask(input: HTMLInputElement) {
      this.mask = input.placeholder.replace(/[0-9]/g, "0");
      input.setAttribute("placeholder", this.mask);
    },

    changeCountry(event: any) {
      if (!this.iti) {
        return;
      }

      // Clear selection
      this.value.number = "";
      this.value.country = "";
      this.value.dialCode = "";
      this.value.countryCode = "";

      this.iti.setCountry(event.value.code);
      (this.kendoForm as any).onChange(this.name, { value: "" });
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    },

    validate(currentValue: string) {
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

      if (!this.isRequired) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (!currentValue) {
        const errorText = this.getErrorMessage();
        this.addIdKeyAndErrorValue(errorText);
        this.errorMessage = errorText;
        return errorText;
      }

      //when first load this package CAN NOT get this.iti.getNumber(), which should have value of +442035679100
      //even if I force change it via, this.iti.setNumber("+442035679100");
      //so, we don't validate it on first load, only when user did manual change
      if (this.iti && this.isUserDidManualChange) {
        const isNumberValid = this.iti.isValidNumber();
        const errorText = this.getErrorMessage(isNumberValid);
        this.addIdKeyAndErrorValue(errorText);
        this.$emit("onValidPhoneNumberFormat", isNumberValid);
        this.errorMessage = errorText;

        if (isNumberValid) {
          this.clearInvalidEntries(this.id);
        } else {
          // add to invalid only if INDEED invalid and not empty
          if (currentValue !== "") {
            this.addInvalidFieldEntry(this.id);
          }
        }

        return errorText;
      }

      //we don't check like this here this.isValueReactive && !currentValue && this.value because
      //this validate is called when other components are changed, couldn't figure it out who trigger it
      if (!currentValue && this.value) {
        currentValue = this.value?.number ?? "";
      }

      this.formValidatorService.setFieldName(this.label || this.name);
      const errorMessage = this.formValidatorService.validate<string>(
        currentValue,
        {
          required: this.isRequired,
        },
      );
      this.addIdKeyAndErrorValue(errorMessage);
      this.errorMessage = errorMessage;

      if (!errorMessage) {
        this.clearInvalidEntries(this.id);
      }

      return errorMessage;
    },

    handleChange(event: any, onChange: (event: any) => void) {
      this.isUserDidManualChange = true;
      onChange(event);
      const contactNumber: ContactNumber = {
        number: event.target.value,
        dialCode: `+${this.currentDial}`,
        country: this.currentCountry,
        countryCode: this.currentCountryCode,
      };
      this.$emit("onValueChange", contactNumber);
      this.$emit("update:modelValue", contactNumber);
    },

    handleBlur() {
      if (this.isRequired) {
        this.validate(this.value?.number ?? "");
      }
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

    getErrorMessage(isValidNumber: boolean = false) {
      return !isValidNumber
        ? `Please enter a valid ${this.label || this.name}`
        : "";
    },
  },
  inject: {
    kendoForm: { default: {} },
  },
});
</script>

<template>
  <div>
    <KendoField
      :id="id"
      :name="name"
      :value="modelValue?.number || value?.number"
      component="template"
      :validator="validate"
    >
      <template v-slot:template="{ props }">
        <KendoFieldWrapper :class="props.class">
          <StackLayout orientation="vertical" style="gap: 6px">
            <KendoLabel
              v-if="label"
              :editor-id="id"
              :disabled="disabled"
              :valid="!(props.touched && errorMessage)"
              class="control-label"
            >
              {{ label }}
              <span v-if="!isRequired" class="fineprint ms-1">(Optional)</span>
            </KendoLabel>

            <div class="TelInput">
              <KendoComboBox
                ref="dropdownElement"
                :filterable="false"
                :suggest="false"
                style="border-radius: 8px"
                :clear-button="false"
                :data-items="options"
                item-render="item"
                :valid="!(props.touched && errorMessage)"
                showClearButton
                @change="changeCountry"
                :fillMode="'solid'"
              >
                <template #item="{ props }">
                  <li
                    :class="['k-list-item', props.class]"
                    @click="props.onClick"
                  >
                    <span class="k-list-item-text">
                      {{ props.dataItem.name }} {{ `+${props.dataItem.dial}` }}
                    </span>
                  </li>
                </template>
              </KendoComboBox>

              <div ref="flagElement" class="TelInput-flag">
                <!--
                In order to have flag,
                1. we need to rebuild our own https://intl-tel-input.com/examples/large-flags.html
                2. put into the css something like this:
                  .iti__flag {background-image: url("intl-tel-input/build/img/flags.png");}
  
                  @media (-webkit-min-device-pixel-ratio: 2), (min-resolution: 192dpi) {
                    .iti__flag {background-image: url("intl-tel-input/build/img/flags@2x.png");}
                  }
                -->
                <div class="TelInput-flagIcon pl-13px">
                  <IconComponent
                    type="flag"
                    :symbol="currentCountryCode.toUpperCase()"
                    size="20"
                  />
                  <IconComponent
                    symbol="chevron-right"
                    size="10"
                    class="is-rotate90"
                  />
                </div>
                <div class="TelInput-flagText" style="margin-top: 2px">
                  +{{ currentDial }}
                </div>
              </div>

              <KendoMaskedTextBox
                ref="inputElement"
                type="tel"
                :mask="mask"
                :isValueReactive="true"
                v-bind="props"
                @change="handleChange($event, props.onChange)"
                :fillMode="'solid'"
                @blur="handleBlur"
              />
            </div>
            <KendoError v-if="props.touched && errorMessage">
              {{ errorMessage }}
            </KendoError>

            <KendoHint v-else>
              {{ hint }}
            </KendoHint>
          </StackLayout>
        </KendoFieldWrapper>
      </template>
    </KendoField>
  </div>
</template>

<style scoped lang="scss">
@import url("intl-tel-input/build/css/intlTelInput.css");

:deep(.k-form-field) {
  margin-top: 0 !important;
}

.TelInput {
  .k-combobox {
    :deep(.k-button) {
      background: transparent;
      border: none;
      width: auto;
      padding: 0;
    }
  }

  .k-maskedtextbox {
    padding: 0;
    margin: 0;
    border: none;
    box-shadow: none;
    background: transparent;

    :deep(.iti) {
      width: 100%;
    }
  }

  &-flag {
    height: 100%;
    display: flex;
    align-items: center;

    &Icon {
      display: flex;
      align-items: center;
      height: 100%;
      padding: 10px;
      gap: 5px;
      background: #f9fafb;
      border-right: 1px solid var(--content-content-07);
      margin-right: 10px;

      :deep(.iti__flag) {
        display: inline-flex;
        align-items: center;
        gap: 5px;
      }
    }
  }
}
</style>
