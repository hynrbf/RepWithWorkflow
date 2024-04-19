<script lang="ts">
import { defineComponent, inject } from "vue";
import {
  AutoCompleteChangeEvent,
  AutoCompleteCloseEvent,
} from "@progress/kendo-vue-dropdowns";
import { GetAddressItem } from "@/entities/GetAddressItem";
import { container } from "tsyringe";
import {
  IGetAddressService,
  IGetAddressServiceInfo,
} from "@/infra/dependency-services/rest/get-address/IGetAddressService";
import { Emitter, EventType } from "mitt";
import { AppConstants } from "@/infra/AppConstants";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";

export default defineComponent({
  name: "KendoPostCodeInputWithSameAsComponent",
  props: {
    label: {
      type: String,
      default: "",
    },
    sameAsLabel: {
      type: String,
      default: "",
    },
    isRequired: {
      type: Boolean,
      default: true,
    },
    isChecked: {
      type: Boolean,
      default: false,
    },
    value: {
      type: String,
      default: "",
    },
    isShowCheckBox: {
      type: Boolean,
      default: true,
    },
    placeholder: {
      type: String,
      default: "Postcode",
    },
    id: String,
    name: {
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
      inputType: this.$t("common-placeholder-text"),
      isToggleDisabled: false,
      isUnmounted: false,
      originalValue: "",
      errorMessage: "",
    };
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
        this.eventBus.emit(AppConstants.formFieldChangedEvent);
        this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
        return;
      }
      this.$emit("onValueChange", this.getHomeAddress());
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    }

    this.isFieldValidOnFirstLoad = this.validate(this.value) == "";
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
  methods: {
    async autoSearchAsync(event: AutoCompleteChangeEvent) {
      if (event.value === undefined) {
        return;
      }

      if (event.value === "") {
        this.lineOne = "";
        this.lineTwo = "";
        this.city = "";
        this.country = "";
        this.postcode = "";
      }

      if (this.debounceTimer) {
        clearTimeout(this.debounceTimer);
      }

      this.isFieldValidOnFirstLoad = false;
      let localKeyword = event.value;
      this.$emit("OnValueChange", localKeyword);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);

      this.debounceTimer = setTimeout(async () => {
        if (!localKeyword || localKeyword.length < 3) {
          this.resetAutoCompleteEnum();
          return;
        }

        let addresses: GetAddressItem[] =
          await this.getAddressService.getFilteredAddressAsync(localKeyword);

        if (addresses && addresses.length > 0) {
          this.postCodeList = addresses.map((obj) => obj.address) as string[];
        } else {
          this.resetAutoCompleteEnum();
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
        this.eventBus.emit(AppConstants.formFieldChangedEvent);
        this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
        return;
      }

      this.$emit("onValueChange", event.target.value as string);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    },

    getHomeAddress() {
      let homeAddress = this.lineOne;

      // Need this one to check because this is not required.
      if (this.lineTwo && this.lineTwo.trim().length > 1) {
        homeAddress += `, ${this.lineTwo}`;
      }

      homeAddress += `, ${this.city}, ${this.country}, ${this.postcode}`;
      return homeAddress;
    },

    resetAutoCompleteEnum() {
      this.postCodeList = ["No Data"];
    },

    onToggle(_: boolean) {
      this.$emit("onToggle", !this.isChecked);
    },

    toggleAddManually() {
      this.isAddManually = !this.isAddManually;
      this.isToggleDisabled = this.isAddManually;

      if (this.isAddManually) {
        // Set to UK by default
        this.country = AppConstants.DefaultCountryCode.toUpperCase();
      }

      // this.lineOne = "";
      // this.lineTwo = "";
      // this.city = "";
      // this.country = "";
      // this.postcode = "";
      if (
        this.lineOne?.trim() &&
        this.city?.trim() &&
        this.postcode?.trim() &&
        this.country?.trim()
      ) {
        this.$emit("OnValueChange", this.getHomeAddress());
      }
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
      if (!currentValue) {
        const errorMessage = "Please enter a valid Line 1";
        this.addIdKeyAndErrorValue(errorMessage);
        return errorMessage;
      }

      this.addIdKeyAndErrorValue("");
      return "";
    },

    validateCity(currentValue: string): string {
      if (!currentValue) {
        const errorMessage = "Please enter a valid City";
        this.addIdKeyAndErrorValue(errorMessage);
        return errorMessage;
      }

      this.addIdKeyAndErrorValue("");
      return "";
    },

    validateCountry(currentValue: string): string {
      if (!currentValue) {
        const errorMessage = "Please enter a valid Country";
        this.addIdKeyAndErrorValue(errorMessage);
        return errorMessage;
      }

      this.addIdKeyAndErrorValue("");
      return "";
    },

    validatePostcode(currentValue: string): string {
      if (!currentValue) {
        const errorMessage = "Please enter a valid Postcode";
        this.addIdKeyAndErrorValue(errorMessage);
        return errorMessage;
      }

      this.addIdKeyAndErrorValue("");
      return "";
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

    handleBlur() {
      if (
        this.lineOne?.trim() &&
        this.city?.trim() &&
        this.postcode?.trim() &&
        this.country?.trim()
      ) {
        this.$emit("OnValueChange", this.getHomeAddress());
      } else {
        this.$emit("OnValueChange", this.value);
      }
    },

    onLostFocus() {
      this.$emit("onLostFocus", this.id, this.originalValue !== this.value);
    },
  },
});
</script>

<template>
  <StackLayout :gap="5" orientation="vertical">
    <StackLayout
      orientation="horizontal"
      :align="{ horizontal: 'start', vertical: 'middle' }"
      v-if="isShowCheckBox"
      :gap="8"
    >
      <Label class="control-label">
        {{ label }}
        <span v-if="!isRequired" class="fineprint ms-1"> (Optional) </span>
      </Label>

      <!-- :valid="true" => this is needed to bypass validation on switch. We don't validate this -->
      <div class="Switch">
        <Switch
          size="small"
          :valid="true"
          :checked="isChecked"
          off-label=""
          on-label=""
          @click="onToggle"
          :disabled="isToggleDisabled"
        />
      </div>

      <Label
        class="control-label paragraph-s-figtree-medium"
        style="margin-left: -2px"
        >{{ sameAsLabel }}</Label
      >

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
        >+Add Manually
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
        <StackLayout
          class="position-relative"
          orientation="horizontal"
          :align="{ horizontal: 'start', vertical: 'middle' }"
          :class="getBorderStyle(!(props.touched && errorMessage))"
        >
          <IconComponent symbol="search-icon" size="20" class="magnifier position-absolute l-12px is-zindex-base" />

          <AutoComplete
            v-bind="props"
            :id="props.id"
            :name="props.name"
            :valid="!(props.touched && errorMessage)"
            :placeholder="computedPlaceholder"
            :value="value"
            :data-items="postCodeList"
            :fillMode="null"
            class="input pl-32px"
            @change="autoSearchAsync"
            @close="onClosePostCode"
            @blur="onLostFocus"
          />
        </StackLayout>

        <Error v-if="props.touched && errorMessage">{{ errorMessage }}</Error>
      </template>
    </Field>

    <StackLayout v-else orientation="vertical" :gap="20">
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
              <StackLayout
                orientation="horizontal"
                class="justify-content-between"
                :align="{ horizontal: 'start' }"
              >
                <Label class="control-label"> Line 1 </Label>
              </StackLayout>

              <Input
                v-bind="props"
                :id="props.id"
                :name="props.name"
                :valid="lineOne?.trim().length > 0"
                :placeholder="inputType"
                style="border-radius: 8px"
                maxlength="100"
                :value="lineOne"
                @input="(e: any) => (lineOne = e.target.value)"
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
            @blur="handleBlur"
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
                :valid="city?.trim().length > 0"
                :placeholder="inputType"
                style="border-radius: 8px"
                maxlength="100"
                :value="city"
                @input="(e: any) => (city = e.target.value)"
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
              class="col"
              orientation="vertical"
              :gap="6"
              :align="{ vertical: 'top' }"
            >
              <Label class="control-label">Postcode</Label>

              <Input
                v-bind="props"
                :id="props.id"
                :name="props.name"
                :valid="postcode?.trim().length > 0"
                style="border-radius: 8px"
                placeholder="Postcode"
                maxlength="100"
                :value="postcode"
                @input="(e: any) => (postcode = e.target.value)"
                @blur="handleBlur"
              />
            </StackLayout>
          </template>
        </Field>

        <!-- TODO. to remove if country component is done -->
        <!--      <Field-->
        <!--          :id="'country'"-->
        <!--          :name="'country'"-->
        <!--          :component="'country'"-->
        <!--          :validator="validateCountry"-->
        <!--      >-->
        <!--        <template v-slot:country="{ props }">-->
        <!--          <StackLayout-->
        <!--              class="col"-->
        <!--              orientation="vertical"-->
        <!--              :gap="8"-->
        <!--              :align="{ vertical: 'top' }"-->
        <!--          >-->
        <!--            <Label class="control-label">Country</Label>-->

        <!--            <Input-->
        <!--                v-bind="props"-->
        <!--                :id="props.id"-->
        <!--                :name="props.name"-->
        <!--                :valid="country?.trim().length > 0"-->
        <!--                placeholder="Type ..."-->
        <!--                maxlength="100"-->
        <!--                :value="country"-->
        <!--                @input="(e: any) => (country = e.target.value)"-->
        <!--            />-->
        <!--          </StackLayout>-->
        <!--        </template>-->
        <!--      </Field>-->

        <!-- TODO. to fix this component to return the full name -->
        <!-- Country -->
        <KendoCountryComponent
          id="country"
          class="mt-0"
          label="Country"
          name="country"
          :value="country"
          @onValueChange="(value: string) => (country = value)"
        />

        <div class="col" />
        <!-- empty space only -->
      </StackLayout>
    </StackLayout>

    <Error style="margin-top: 2px" v-if="showValidationErrorManual"
      >Please complete the details.
    </Error>
  </StackLayout>
</template>

<style scoped>
.input {
  border-width: 0px;
}
</style>