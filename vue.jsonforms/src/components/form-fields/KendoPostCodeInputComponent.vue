<script lang="ts">
import { defineComponent, inject } from "vue";
import {
  AutoCompleteChangeEvent,
  AutoCompleteCloseEvent,
} from "@progress/kendo-vue-dropdowns";
import { container } from "tsyringe";
import {
  IGetAddressService,
  IGetAddressServiceInfo,
} from "@/infra/dependency-services/rest/get-address/IGetAddressService";
import { GetAddressItem } from "@/entities/GetAddressItem";
import { AppConstants } from "@/infra/AppConstants";
import { Emitter, EventType } from "mitt";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";

export default defineComponent({
  name: "KendoPostCodeInputComponent",
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
      default: "Postcode",
    },
    searchModeText: {
      type: String,
      default: "+Add Manually",
    },
    manualModelText: {
      type: String,
      default: "-Remove",
    },
    modelValue: {
      type: String,
      default: "",
    },
    isUserModified: {
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
      getAddressService: container.resolve<IGetAddressService>(
        IGetAddressServiceInfo.name,
      ),
      postCodeList: ["No Data"],
      debounceTimer: null as NodeJS.Timeout | null,
      debounceTimerInMs: 500,
      // for add manually
      isAddManually: false,
      lineOne: "",
      lineTwo: "",
      city: "",
      postcode: "",
      country: "",
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      eventBusControlElement: inject("$eventBusService") as Emitter<
        Record<EventType, string>
      >,
      homeAddress: "",
      inputType: this.$t("common-placeholder-text"),
      isUnmounted: false,
      errorMessage: "",
      originalValue: "",
    };
  },
  watch: {
    isDataLoadedCompletely(newValue, oldValue): boolean {
      if (newValue == oldValue) {
        return newValue;
      }

      this.originalValue = this.value;
      this.isFieldValidOnFirstLoad = this.validate(this.value) == "";
      return newValue;
    },
  },
  computed: {
    showValidationErrorManual(): boolean {
      return (
        this.isAddManually &&
        !(
          this.lineOne?.trim() &&
          this.city?.trim() &&
          this.postcode?.trim() &&
          this.country?.trim()
        )
      );
    },
    computedPlaceholder() {
      if (!this.$t("common-postcode-text")) {
        return this.placeholder;
      }

      return this.$t("common-postcode-text");
    },
  },
  setup() {
    const pageComponentValidationValueStore =
      usePageComponentValidationValueStore();
    const { addComponentValidationValue, removeValidationValueByFieldId } =
      pageComponentValidationValueStore;

    return {
      removeValidationValueByFieldId,
      addComponentValidationValue,
    };
  },
  created() {
    this.originalValue = this.value;

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

    this.eventBusControlElement.on(
      AppConstants.updateTradingAddressControlStateEvent,
      (id) => {
        if (id === this.id) {
          this.originalValue = this.value;
        }
      },
    );
  },
  unmounted() {
    if (!this.id) {
      return;
    }

    this.removeValidationValueByFieldId(this.id);
    this.isUnmounted = true;
    this.eventBusControlElement.off(
      AppConstants.updateTradingAddressControlStateEvent,
    );
  },
  updated() {
    if (this.isAddManually) {
      if (
        !(
          this.lineOne?.trim() &&
          this.city?.trim() &&
          this.postcode?.trim() &&
          this.country?.trim()
        )
      ) {
        this.$emit("onValueChange", "");
        this.$emit("update:modelValue", "");
        this.eventBus.emit(AppConstants.formFieldChangedEvent);
        this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
        return;
      }

      this.buildHomeAddress();
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
      return;
    }

    this.isFieldValidOnFirstLoad = this.validate(this.value) == "";
  },
  methods: {
    async autoSearchAsync(event: AutoCompleteChangeEvent) {
      if (event.value == undefined) {
        return;
      }

      if (this.debounceTimer) {
        clearTimeout(this.debounceTimer);
      }

      this.isFieldValidOnFirstLoad = false;
      let localKeyword = event.value;
      this.$emit("onValueChange", localKeyword);
      this.$emit("update:modelValue", localKeyword);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);

      this.debounceTimer = setTimeout(async () => {
        if (!localKeyword || localKeyword.length < 3) {
          this.postCodeList = ["No Data"];
          return;
        }

        let addresses: GetAddressItem[] =
          await this.getAddressService.getFilteredAddressAsync(localKeyword);

        if (addresses && addresses.length > 0) {
          this.postCodeList = addresses.map((obj) => obj.address) as string[];
        } else {
          this.postCodeList = ["No Data"];
        }
      }, this.debounceTimerInMs);
    },

    onClosePostCode(event: AutoCompleteCloseEvent) {
      if (event.event.type !== "click") {
        return;
      }

      let isClearAction =
        event.event.currentTarget?.className === "k-clear-value";

      if (isClearAction) {
        this.$emit("onValueChange", "");
        this.$emit("update:modelValue", "");
        this.eventBus.emit(AppConstants.formFieldChangedEvent);
        this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
        return;
      }

      let local = event.target.value as string;
      this.$emit("onValueChange", local);
      this.$emit("update:modelValue", local);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    },

    toggleAddManually() {
      this.isAddManually = !this.isAddManually;

      if (this.isAddManually) {
        // Set to UK by default
        this.country = AppConstants.DefaultCountryCode.toUpperCase(); // "GB"
        return;
      }

      this.$emit("onValueChange", this.value);
      this.$emit("update:modelValue", this.value);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    },

    validate(currentValue: string): string {
      this.errorMessage = "";
      //note, even if you remove the component in the DOM 'using v-if', the vue still preserves it in memory
      //that's why we need this variable
      if (this.isUnmounted) {
        return "";
      }

      if (this.isAddManually) {
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

      if (this.isValueReactive && !currentValue && this.value) {
        currentValue = this.value;
      }

      if (this.isRequired && !currentValue) {
        const errorMessage = `Please enter a valid ${this.label}`;
        this.addIdKeyAndErrorValue(errorMessage);
        this.errorMessage = errorMessage;
        return errorMessage;
      }

      this.addIdKeyAndErrorValue("");
      return "";
    },

    validateLineOne(currentValue: string): string {
      if (!this.isAddManually) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (!currentValue) {
        const errorMessage = "Please enter a valid Line 1";
        this.addIdKeyAndErrorValue(errorMessage);
        return errorMessage;
      }

      this.addIdKeyAndErrorValue("");
      return "";
    },

    validateCity(currentValue: string): string {
      if (!this.isAddManually) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (!currentValue) {
        const errorMessage = "Please enter a valid City";
        this.addIdKeyAndErrorValue(errorMessage);
        return errorMessage;
      }

      this.addIdKeyAndErrorValue("");
      return "";
    },

    validateCountry(currentValue: string): string {
      if (!this.isAddManually) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (!currentValue) {
        const errorMessage = "Please enter a valid Country";
        this.addIdKeyAndErrorValue(errorMessage);
        return errorMessage;
      }

      this.addIdKeyAndErrorValue("");
      return "";
    },

    validatePostcode(currentValue: string): string {
      if (!this.isAddManually) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (!currentValue) {
        const errorMessage = "Please enter a valid Postcode";
        this.addIdKeyAndErrorValue(errorMessage);
        return errorMessage;
      }

      this.addIdKeyAndErrorValue("");
      return "";
    },

    onLineOneChange(e: any) {
      this.lineOne = e.target.value;
      this.buildHomeAddress();
    },

    onCityChange(e: any) {
      this.city = e.target.value;
      this.buildHomeAddress();
    },

    onPostcodeChange(e: any) {
      this.postcode = e.target.value;
      this.buildHomeAddress();
    },

    onCountryChange(value: string) {
      this.country = value;
      this.buildHomeAddress();
    },

    buildHomeAddress() {
      this.homeAddress = this.lineOne;

      // Need this one to check because this is not required.
      if (this.lineTwo && this.lineTwo.trim().length > 1) {
        this.homeAddress += `, ${this.lineTwo}`;
      }

      this.homeAddress += `, ${this.city}, ${this.country}, ${this.postcode}`;
      this.$emit("onValueChange", this.homeAddress);
      this.$emit("update:modelValue", this.homeAddress);
    },

    getBorderStyle(isValid: boolean): string {
      if (!isValid) {
        return "control-container-error";
      }

      return this.isUserModified
        ? "control-container-changed"
        : "control-container";
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

    onLostFocus() {
      this.$emit("onLostFocus", this.id, this.originalValue !== this.value);
    },

    handleBlur() {
      if ((
        this.lineOne?.trim() &&
       this.city?.trim() &&
       this.postcode?.trim() &&
       this.country?.trim()
      )) {
        this.$emit("onValueChange", this.value);
      }
    }
  },
});
</script>

<template>
  <StackLayout orientation="vertical">
    <StackLayout
      orientation="horizontal"
      :align="{ horizontal: 'start', vertical: 'top' }"
      :gap="8"
    >
      <Label class="control-label">
        {{ label }}
        <span v-if="!isRequired" class="fineprint ms-1"> (Optional) </span>
      </Label>

      <span
        v-if="!isAddManually"
        class="control-label align-right-and-stretch paragraph-s-figtree-medium"
        >Can’t find the address?</span
      >

      <div v-else class="flex-grow-1" />

      <Button
        v-if="!isAddManually"
        type="button"
        fill-mode="link"
        class="add-manually-btn"
        @click="toggleAddManually"
        size="small"
        >{{ searchModeText }}
      </Button>

      <Button
        v-else
        type="button"
        fill-mode="link"
        class="add-manually-btn"
        @click="toggleAddManually"
        size="small"
        >Back to Address Search
      </Button>
    </StackLayout>

    <Field
      v-if="!isAddManually"
      :id="id"
      :name="name"
      :component="'myTemplate'"
      :validator="validate"
    >
      <template v-slot:myTemplate="{ props }">
        <StackLayout orientation="vertical">
          <StackLayout
            orientation="horizontal"
            class="position-relative"
            :class="getBorderStyle(!(props.touched && errorMessage))"
            :align="{ horizontal: 'start', vertical: 'middle' }"
          >
            <IconComponent symbol="search-icon" size="20" class="magnifier position-absolute l-12px is-zindex-base" />

            <AutoComplete
              v-bind="props"
              :id="props.id"
              :name="props.name"
              :placeholder="computedPlaceholder"
              :valid="!(props.touched && errorMessage)"
              :value="modelValue || value"
              @close="onClosePostCode"
              @change="autoSearchAsync"
              :fillMode="null"
              @blur="onLostFocus"
              class="input pl-32px"
              :data-items="postCodeList"
            />
          </StackLayout>

          <Error class="error" v-if="props.touched && errorMessage"
            >{{ errorMessage }}
          </Error>
        </StackLayout>
      </template>
    </Field>

    <StackLayout v-else orientation="vertical" :gap="2">
      <StackLayout orientation="horizontal" :gap="15">
        <!-- Line 1 -->
        <Field
          :id="'lineOne'"
          :name="'lineOne'"
          :component="'lineOne'"
          :validator="validateLineOne"
        >
          <template v-slot:lineOne="{ props }">
            <StackLayout
              class="col"
              orientation="vertical"
              :gap="8"
              :align="{ vertical: 'top' }"
            >
              <Label class="control-label">Line 1</Label>

              <Input
                v-bind="props"
                :id="props.id"
                style="border-radius: 8px"
                :name="props.name"
                :valid="lineOne?.trim().length > 0"
                :placeholder="inputType"
                maxlength="100"
                :fillMode="null"
                :value="lineOne"
                @input="onLineOneChange"
                @blur="handleBlur"
              />
            </StackLayout>
          </template>
        </Field>

        <!-- Line 2 -->
        <StackLayout
          class="col"
          orientation="vertical"
          :gap="8"
          :align="{ vertical: 'top' }"
        >
          <Label class="control-label"
            >Line 2
            <span class="fineprint ms-1"> (Optional) </span>
          </Label>

          <Input
            :placeholder="inputType"
            maxlength="100"
            v-model="lineTwo"
            style="border-radius: 8px"
          />
        </StackLayout>

        <!-- City -->
        <Field
          :id="'city'"
          :name="'city'"
          :component="'city'"
          :validator="validateCity"
        >
          <template v-slot:city="{ props }">
            <StackLayout
              class="col"
              orientation="vertical"
              :gap="8"
              :align="{ vertical: 'top' }"
            >
              <Label class="control-label">City</Label>

              <Input
                v-bind="props"
                :id="props.id"
                :name="props.name"
                style="border-radius: 8px"
                :valid="city?.trim().length > 0"
                :placeholder="inputType"
                maxlength="100"
                :value="city"
                @input="onCityChange"
                @blur="handleBlur"
              />
            </StackLayout>
          </template>
        </Field>
      </StackLayout>

      <StackLayout orientation="horizontal" :gap="15">
        <!-- Postcode -->
        <Field
          :id="'postcode'"
          :name="'postcode'"
          :component="'postcode'"
          :validator="validatePostcode"
        >
          <template v-slot:postcode="{ props }">
            <StackLayout
              class="col mt-2"
              orientation="vertical"
              :gap="6"
              :align="{ vertical: 'top' }"
            >
              <Label class="control-label">Postcode</Label>

              <Input
                v-bind="props"
                :id="props.id"
                :name="props.name"
                style="border-radius: 8px"
                :valid="postcode?.trim().length > 0"
                placeholder="Postcode"
                maxlength="100"
                :value="postcode"
                @input="onPostcodeChange"
                @blur="handleBlur"
              />
            </StackLayout>
          </template>
        </Field>

        <!-- Country -->
        <!--      <Field-->
        <!--          :id="'country'"-->
        <!--          :name="'country'"-->
        <!--          :component="'country'"-->
        <!--          :validator="validateCountry"-->
        <!--      >-->
        <!--        <template v-slot:country="{ props }">-->
        <!--          <StackLayout class="col mt-2" orientation="vertical" :gap="8" :align="{vertical: 'top'}">-->
        <!--            <Label class="control-label">Country </Label>-->

        <!--            <Input-->
        <!--                v-bind="props"-->
        <!--                :id="props.id"-->
        <!--                :name="props.name"-->
        <!--                :valid="country?.trim().length > 0"-->
        <!--                placeholder="Type ..."-->
        <!--                maxlength="100"-->
        <!--                :value="country"-->
        <!--                @input="onCountryChange"-->
        <!--            />-->
        <!--          </StackLayout>-->
        <!--        </template>-->
        <!--      </Field>-->
        <!-- TODO. to fix this component to return the full name -->
        <KendoCountryComponent
          id="country"
          class="mt-2"
          label="Country"
          name="country"
          :value="country"
          @onValueChange="onCountryChange"
        />

        <div />
        <!-- empty space only -->
      </StackLayout>

      <Error
        class="error"
        style="margin-top: 6px"
        v-if="showValidationErrorManual"
        >Please complete the details.
      </Error>
    </StackLayout>
  </StackLayout>
</template>

<style scoped>
.input {
  background-color: white;
  border-width: 0px;
}

.error {
  color: var(--error-color-error-primary);
  /* font-family: Figtree; */
  font-size: var(--font-size-sm);
  font-style: normal;
  font-weight: var(--font-weight-normal);
  line-height: 125%; /* 17.5px */
}
</style>