<script lang="ts">
import { defineComponent, inject } from "vue";
import {
  AutoComplete,
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
  name: "KendoPostCodeInputComponentForSignup",
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
      default: "B17 8RP",
    },
    searchModeText: {
      type: String,
      default: "+Add Manually",
    },
    manualModelText: {
      type: String,
      default: "-Remove",
    },
    autoCompleteClass: {
      type: [Object, String],
      default: undefined,
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
  components: { AutoComplete },
  data() {
    return {
      isLoadingPCData: false,
      postcodeHasMatch: false,
      isInSearchWhileTyping: false,
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
      homeAddress: "",
      selectedAddress: "",
      inputType: this.$t("common-placeholder-text"),
      errorMessage: "",
      isUnmounted: false,
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
        return;
      }

      this.buildHomeAddress();
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    }
  },
  methods: {
    async autoSearchAsync(event: AutoCompleteChangeEvent) {
      if (this.debounceTimer) {
        clearTimeout(this.debounceTimer);
      }

      if (!event.value) {
        this.onClosePostCode(event as AutoCompleteCloseEvent);
        return;
      }

      this.isFieldValidOnFirstLoad = false;
      let localKeyword = event.value;
      this.$emit("onValueChange", localKeyword);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);

      this.debounceTimer = setTimeout(async () => {
        if (!localKeyword || localKeyword.length < 3) {
          this.resetAutocompleteEnum();
          return;
        }

        if (this.selectedAddress) {
          const replacedStr = this.selectedAddress.replace(
            /[.*+?^${}()|[\]\\]/g,
            "\\$&",
          );
          const isSameAddress = new RegExp(`^${replacedStr}$`).test(
            localKeyword,
          );
          if (isSameAddress) return;
        }

        this.postCodeList = [];
        this.isLoadingPCData = true;
        let addresses: GetAddressItem[] =
          await this.getAddressService.getFilteredAddressAsync(localKeyword);

        if (addresses && addresses.length > 0) {
          let addressNames = addresses.map((obj) => obj.address) as string[];
          this.postCodeList = addressNames;
          this.postcodeHasMatch = true;
          this.isInSearchWhileTyping = true;
        } else {
          this.resetAutocompleteEnum();
        }

        this.isLoadingPCData = false;
      }, this.debounceTimerInMs);
    },

    resetAutocompleteEnum() {
      this.postcodeHasMatch = false;
      this.isInSearchWhileTyping = false;
      this.isLoadingPCData = false;
      this.selectedAddress = "";
    },

    onClosePostCode(event: AutoCompleteCloseEvent) {
      this.isInSearchWhileTyping = false;

      let isClearAction =
        event?.event?.currentTarget?.className === "k-clear-value";

      if (isClearAction) {
        this.resetAutocompleteEnum();
        this.$emit("onValueChange", "");
        this.eventBus.emit(AppConstants.formFieldChangedEvent);
        this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
        return;
      }

      let local = event.target.value as string;

      this.selectedAddress = local;

      if (!this.selectedAddress) {
        this.resetAutocompleteEnum();
      }

      this.$emit("onValueChange", local);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    },

    toggleAddManually() {
      this.isAddManually = !this.isAddManually;

      if (!this.isAddManually) {
        this.lineOne = "";
        this.lineTwo = "";
        this.city = "";
        this.country = "";
        this.postcode = "";
        this.$emit("OnValueChange", "");
        this.eventBus.emit(AppConstants.formFieldChangedEvent);
        this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
      }
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

    onLineOneChange(e: { target: HTMLInputElement }) {
      this.lineOne = e.target.value;
      this.buildHomeAddress();
    },

    onCityChange(e: { target: HTMLInputElement }) {
      this.city = e.target.value;
      this.buildHomeAddress();
    },

    onPostcodeChange(e: { target: HTMLInputElement }) {
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
      v-if="!isAddManually"
      :id="id"
      :name="name"
      :component="'myTemplate'"
      :validator="validate"
    >
      <template v-slot:myTemplate="{ props }">
        <StackLayout orientation="vertical" :gap="8">
          <StackLayout
            orientation="horizontal"
            class="justify-content-between"
            :align="{ horizontal: 'start', vertical: 'middle' }"
          >
            <Label
              :editor-id="props.id"
              class="control-label paragraph-s-figtree-medium"
              >{{ label }}
            </Label>

            <div>
              <span
                style="margin-right: 2px"
                class="control-label paragraph-s-figtree-medium"
              >
                {{ $t("signUpPage-postCodeCantFindAddress") }}
              </span>

              <span class="link-text" @click="toggleAddManually">
                {{ searchModeText }}
              </span>
            </div>
          </StackLayout>

          <StackLayout
            orientation="horizontal"
            class="position-relative"
            :class="{
              'control-container': !props.touched || !errorMessage,
              'control-container-error': props.touched && errorMessage,
            }"
            :align="{ horizontal: 'start', vertical: 'middle' }"
          >
            <IconComponent
              symbol="search-icon"
              size="20"
              class="magnifier position-absolute l-12px is-zindex-base"
            />

            <AutoComplete
              v-bind="props"
              list-no-data-render="noDataOrLoading"
              :id="props.id"
              :name="props.name"
              :placeholder="placeholder"
              :valid="!(props.touched && errorMessage)"
              :value="value"
              fillMode="null"
              :class="['input pl-32px', autoCompleteClass]"
              :data-items="postCodeList"
              :loading="
                (isLoadingPCData && !postcodeHasMatch) ||
                (isLoadingPCData && !isInSearchWhileTyping)
              "
              :opened="postcodeHasMatch && isInSearchWhileTyping"
              @close="onClosePostCode"
              @change="autoSearchAsync"
            >
              <template #noDataOrLoading>
                <StackLayout
                  v-if="isLoadingPCData"
                  class="pt-50px pb-50px"
                  orientation="vertical"
                  :align="{ horizontal: 'center' }"
                  :gap="15"
                >
                  <KendoLoadingComponent
                    loaderType="component"
                    loadingText="Searching..."
                    wrapperClass="gap-3"
                    :isLoading="true"
                  />
                </StackLayout>
              </template>
            </AutoComplete>
          </StackLayout>

          <Error class="error" v-if="props.touched && errorMessage"
            >{{ errorMessage }}
          </Error>
        </StackLayout>
      </template>
    </Field>

    <StackLayout v-else orientation="vertical" :gap="2">
      <StackLayout
        orientation="horizontal"
        class="justify-content-between"
        :align="{ horizontal: 'start', vertical: 'middle' }"
      >
        <Label class="control-label">{{ label }} </Label>

        <Button
          type="button"
          fill-mode="link"
          class="add-manually-btn paragraph-s-figtree-medium"
          @click="toggleAddManually"
          size="small"
          >{{ manualModelText }}
        </Button>
      </StackLayout>

      <StackLayout orientation="horizontal" :gap="8">
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
              <!-- Line 1 -->
              <Label class="control-label">Line 1 </Label>

              <Input
                v-bind="props"
                :id="props.id"
                :name="props.name"
                :valid="lineOne?.trim().length > 0"
                :placeholder="inputType"
                maxlength="100"
                :value="lineOne"
                @input="onLineOneChange"
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
          <Label class="control-label">Line 2</Label>

          <Input :placeholder="inputType" maxlength="100" v-model="lineTwo" />
        </StackLayout>
      </StackLayout>

      <StackLayout class="my-2" orientation="horizontal" :gap="8">
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
              <Label class="control-label">City </Label>

              <Input
                v-bind="props"
                :id="props.id"
                :name="props.name"
                :valid="city?.trim().length > 0"
                :placeholder="inputType"
                maxlength="100"
                :value="city"
                @input="onCityChange"
              />
            </StackLayout>
          </template>
        </Field>

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
              :gap="8"
              :align="{ vertical: 'top' }"
            >
              <Label class="control-label">Postcode </Label>

              <Input
                v-bind="props"
                :id="props.id"
                :name="props.name"
                :valid="postcode?.trim().length > 0"
                :placeholder="inputType"
                maxlength="100"
                :value="postcode"
                @input="onPostcodeChange"
              />
            </StackLayout>
          </template>
        </Field>
      </StackLayout>

      <StackLayout orientation="horizontal" :gap="8">
        <!-- Country -->
        <!--        <Field-->
        <!--            :id="'country'"-->
        <!--            :name="'country'"-->
        <!--            :component="'country'"-->
        <!--            :validator="validateCountry"-->
        <!--        >-->
        <!--          <template v-slot:country="{ props }">-->
        <!--            <StackLayout class="col" orientation="vertical" :gap="8" :align="{vertical: 'top'}">-->
        <!--              <Label class="control-label"-->
        <!--              >Country-->
        <!--              </Label>-->

        <!--              <Input-->
        <!--                  v-bind="props"-->
        <!--                  :id="props.id"-->
        <!--                  :name="props.name"-->
        <!--                  :valid="country?.trim().length > 0"-->
        <!--                  placeholder="Type ..."-->
        <!--                  maxlength="100"-->
        <!--                  :value="country"-->
        <!--                  @input="onCountryChange"-->
        <!--              />-->
        <!--            </StackLayout>-->
        <!--          </template>-->
        <!--        </Field>-->
        <!-- TODO. to fix this component to return the full name -->
        <KendoCountryComponent
          id="country"
          class="mt-0"
          label="Country"
          name="country"
          :value="country"
          @onValueChange="onCountryChange"
        />

        <div />
        <!-- empty space only -->
      </StackLayout>

      <Error class="my-2 error" v-if="showValidationErrorManual"
        >Please complete the details.
      </Error>
    </StackLayout>
  </div>
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

.link-text {
  color: var(--color-primary);
  font-size: 14px;
  font-style: normal;
  font-weight: 700;
  line-height: 17.5px;
  text-decoration-line: underline;
  cursor: pointer;
}
</style>
