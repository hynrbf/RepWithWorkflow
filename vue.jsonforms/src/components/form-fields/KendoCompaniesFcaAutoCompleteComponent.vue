<script lang="ts">
import { defineComponent, inject } from "vue";
import {
  AutoCompleteCloseEvent,
  AutoCompleteOpenEvent,
} from "@progress/kendo-vue-dropdowns";
import { container } from "tsyringe";
import {
  ICompaniesHouseService,
  ICompaniesHouseServiceInfo,
} from "@/infra/dependency-services/rest/company-house/ICompaniesHouseService";
import { CompanyEntity } from "@/entities/CompanyEntity";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import { Emitter, EventType } from "mitt";
import { AppConstants } from "@/infra/AppConstants";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";
import {
  IFcaService,
  IFcaServiceInfo,
} from "@/infra/dependency-services/rest/fca/IFcaService";
import { FirmDetailsBase } from "@/entities/FirmDetailsBase";

export default defineComponent({
  props: {
    id: String,
    name: {
      type: String,
      default: "",
    },
    label: {
      type: String,
      default: "Company",
    },
    value: {
      type: String,
      default: "",
    },
    placeholder: {
      type: String,
      default: "Type...",
    },
    isRequired: {
      type: Boolean,
      default: true,
    },
    isSoleTrader: {
      type: Boolean,
      default: false,
    },
    isShowBottomResults: {
      type: Boolean,
      default: true,
    },
    isEditable: {
      type: Boolean,
      default: true,
    },
    excludedFirm: {
      type: Object as () => FirmDetailsBase | undefined,
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
  data() {
    return {
      isFieldValidOnFirstLoad: false,
      isLoadingCompanyHouseDetails: false,
      companiesHouseService: container.resolve<ICompaniesHouseService>(
        ICompaniesHouseServiceInfo.name,
      ),
      fcaService: container.resolve<IFcaService>(IFcaServiceInfo.name),
      firms: [] as CompanyEntity[],
      debounceTimer: null as NodeJS.Timeout | null,
      debounceTimerInMs: 500,
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      postCodeKeyword: "(Postcode: ",
      regionCodeKeyword: "(Region: ",
      isOpened: false,
      eventBusNotFoundCompany: inject("$eventBusService") as Emitter<
        Record<EventType, { msg: string; company: string }>
      >,
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      formatFirms: [] as string[],
      keywordFirmSearch: "",
      currentlySelectedCompany: undefined as CompanyEntity | undefined,
      inputClass: "default-firm",
      controlContainerId: "company-autocomplete-container",
      isCompanyHasMatch: false,
      searchIconWidth: 20,
      controlContainerLeftPadding: 12,
      isUnmounted: false,
      errorMessage: "",
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
  updated() {
    this.isFieldValidOnFirstLoad = this.validate(this.value) == "";
  },
  unmounted() {
    if (this.id) {
      this.removeValidationValueByFieldId(this.id);
    }
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
  methods: {
    async onValueChangeAsync(event: any) {
      if (!this.isEditable) {
        this.$emit("onAttemptEdit");
        return;
      }

      if (this.debounceTimer) {
        clearTimeout(this.debounceTimer);
      }

      if (!event.value && event?.type === "change") {
        // prevent changing the value except for selection and input
        return;
      }

      this.keywordFirmSearch = event.value ?? "";
      this.isFieldValidOnFirstLoad = false;
      this.$emit("onInput", this.keywordFirmSearch);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);

      let localKeyword = "";
      this.debounceTimer = setTimeout(async () => {
        if (!this.keywordFirmSearch || this.keywordFirmSearch.length < 3) {
          this.resetAutoCompleteEnum();
          return;
        }

        this.formatFirms = [];
        localKeyword = this.keywordFirmSearch;
        this.firms = await this.fcaService.searchFirmsByFirmNameKeywordAsync(
          encodeURIComponent(localKeyword),
        );

        if (!(this.firms && this.firms.length > 0)) {
          this.resetAutoCompleteEnum();
          return;
        }

        // show only companies that starts with the localKeyword
        if (this.helperService.isStringNumber(localKeyword.trim())) {
          this.firms = this.firms.filter(
            (c) =>
              c.companyNumber
                ?.toLowerCase()
                .startsWith(localKeyword.toLowerCase()),
          );
        } else {
          this.firms = this.firms.filter(
            (c) =>
              c.companyName
                ?.toLowerCase()
                .startsWith(localKeyword.toLowerCase()),
          );
        }

        this.firms.forEach((firm) => {
          const firmName = firm.companyName.trim();

          if (firm.postcode) {
            this.formatFirms.push(
              `${firmName} ${this.postCodeKeyword}${firm.postcode})`,
            );
          } else if (firm.region) {
            this.formatFirms.push(
              `${firmName} ${this.regionCodeKeyword}${firm.region})`,
            );
          } else {
            this.formatFirms.push(`${firmName}`);
          }
        });

        this.formatFirms = [...new Set(this.formatFirms)];
        this.isCompanyHasMatch = true;

        if (this.formatFirms.length <= 0) {
          this.resetAutoCompleteEnum();
        }
      }, this.debounceTimerInMs);

      await this.helperService.delayAsync(500);

      if (this.formatFirms.length <= 0) {
        return;
      }

      //avoid duplicate in the list
      this.formatFirms = [...new Set(this.formatFirms)];
    },

    onOpen(_event: AutoCompleteOpenEvent) {
      if (!this.isEditable) {
        return;
      }

      // wait until autocomplete container is displayed before aligning and correcting dropdown
      setTimeout(() => {
        this.alignAutocompleteResultDropdown();
        this.setAutoCompleteListTopMargin();
      }, 100);
      this.isOpened = true;
    },

    onClose(event: AutoCompleteCloseEvent) {
      let selected = event.target.value as string;

      if (selected) {
        this.displayNoCompanyPopupIfHasNoCompany();
      }

      this.isOpened = false;

      if (event.event.type !== "click") {
        return;
      }

      let selectedFirmFromFca = this.extractSelectedFirm(selected);

      if (!selectedFirmFromFca) {
        let isClearAction =
          event.event.currentTarget?.className === "k-clear-value";

        if (isClearAction) {
          return;
        }

        //no more this already because we clear this field already, when select sole trader
        //throw "selectedCompanyFromHouse is impossible to be null";
      }
    },

    async onSelectAsync(event: any) {
      try {
        this.isLoadingCompanyHouseDetails = true;
        this.currentlySelectedCompany = undefined;
        let companyName = event.target.value as string;

        let selectedFirmFromFca = this.extractSelectedFirm(companyName);

        if (!selectedFirmFromFca) {
          return;
        }

        if (
          this.excludedFirm &&
          selectedFirmFromFca.firmReferenceNo ===
            this.excludedFirm.firmReferenceNumber
        ) {
          this.$emit("onFirmSelectionNotAllowed");
          const emptyCompany: CompanyEntity = {
            companyName: "",
            address: "",
            tradingAddress: "",
            postcode: "",
            companyNumber: "",
            firmReferenceNo: "",
            status: "",
            companyHouseStatus: "",
            type: "",
            isAuthorized: false,
            isConfirmedFirmDetails: false,
            isVariedFirmPermissions: false,
            isSelected: false,
            region: "",
            isSoleTrader: false,
            appointedRepresentatives: [],
            contactNumber: undefined,
            website: undefined,
            sicCode: "",
            countryCode: "",
          };

          this.$emit("onValueChange", emptyCompany);
          return;
        }

        let cleanedCompanyName = this.cleanCompanyName(companyName);
        const foundCompanyFromCH =
          await this.companiesHouseService.searchCompaniesAsync(
            cleanedCompanyName,
          );

        if (!foundCompanyFromCH || foundCompanyFromCH.length < 1) {
          return;
        }

        let matchedCompanyFromCH = foundCompanyFromCH.find((c) =>
          this.compareIfMatched(c, cleanedCompanyName),
        );

        if (!matchedCompanyFromCH) {
          return;
        }

        selectedFirmFromFca.companyNumber = matchedCompanyFromCH.companyNumber;
        selectedFirmFromFca.address = matchedCompanyFromCH.address;
        selectedFirmFromFca.companyHouseStatus = matchedCompanyFromCH.status;
        this.currentlySelectedCompany = selectedFirmFromFca;

        const promise1 = this.fcaService.extractTradingAddressDetailsAsync(
          this.currentlySelectedCompany.firmReferenceNo,
        );
        const promise2 = this.companiesHouseService.getCompanyProfileAsync(
          selectedFirmFromFca.companyNumber,
        );
        const [result1, result2] = await Promise.all([promise1, promise2]);

        // get the address of the specific selected firm
        const addressDetails = result1;
        this.currentlySelectedCompany.tradingAddress = addressDetails.address;
        this.currentlySelectedCompany.website = addressDetails.website;
        this.currentlySelectedCompany.contactNumber =
          addressDetails.phoneNumber;

        // get company profile
        const profile = result2;

        if (profile?.sic_codes && profile.sic_codes.length > 0) {
          this.currentlySelectedCompany.sicCode = profile.sic_codes[0];
        }

        if (profile?.registered_office_address?.country) {
          this.currentlySelectedCompany.countryCode =
            await this.helperService.getCountryCodeAsync(
              profile.registered_office_address.country,
            );
        }

        this.adjustUiSelectedCompany();
        this.$emit("onValueChange", this.currentlySelectedCompany);
      } finally {
        this.isLoadingCompanyHouseDetails = false;
      }
    },

    adjustUiSelectedCompany(): void {
      if (!this.currentlySelectedCompany) {
        return;
      }

      // we intentionally blur the input to remove the focus once the user selected a company
      // this is to avoid the issue upon collapsing in the close-links
      const autoCompleteInput = document.querySelector(
        ".k-autocomplete.k-input .k-input-inner",
      ) as HTMLInputElement;

      if (autoCompleteInput) {
        autoCompleteInput.blur();
      }

      this.currentlySelectedCompany.isAuthorized =
        this.currentlySelectedCompany.status === AppConstants.authorised;
      this.currentlySelectedCompany.isSelected = true;

      //when searching fca, we don't need to hover and with green like in signup
      //this shows only in firm profile pages
      this.controlContainerId = "company-autocomplete-container-non-authorised";
      this.inputClass = "non-authorised-firm";

      this.setClearIconToCheckIfAuthorised(
        this.currentlySelectedCompany.isAuthorized,
      );
    },

    extractSelectedFirm(selected: string): CompanyEntity | undefined {
      let companyName = "";
      let postCode = "";
      let region = "";

      if (selected.includes(this.postCodeKeyword)) {
        let fIndex = selected.indexOf(this.postCodeKeyword);
        companyName = selected.substring(0, fIndex - 1).trim();

        let lIndex = selected.indexOf(")");
        let postCodeTextLength = this.postCodeKeyword.length;
        postCode = selected
          .substring(fIndex + postCodeTextLength, lIndex)
          .trim();
      } else if (selected.includes(this.regionCodeKeyword)) {
        let fIndex = selected.indexOf(this.regionCodeKeyword);
        companyName = selected.substring(0, fIndex - 1).trim();

        let lIndex = selected.indexOf(")");
        let regionTextLength = this.regionCodeKeyword.length;
        region = selected.substring(fIndex + regionTextLength, lIndex).trim();
      } else {
        companyName = selected.toString();
      }

      let selectedCompanies = this.firms?.filter(
        (c) => c.companyName.trim() === companyName,
      );
      let selectedCompanyFromHouse: CompanyEntity | null = null;

      if (postCode) {
        selectedCompanyFromHouse = selectedCompanies.find(
          (x) => x.postcode === postCode,
        ) as CompanyEntity;
      } else if (region) {
        selectedCompanyFromHouse = selectedCompanies.find(
          (x) => x.region === region,
        ) as CompanyEntity;
      } else {
        selectedCompanyFromHouse = selectedCompanies[0] as CompanyEntity;
      }

      return selectedCompanyFromHouse;
    },

    alignAutocompleteResultDropdown() {
      let dropdown = document.querySelector(
        ".k-list-container.k-popup.k-child-animation-container",
      );

      if (!(dropdown instanceof HTMLElement)) {
        return;
      }

      const width = dropdown.offsetWidth;
      const totalOffset =
        this.searchIconWidth + this.controlContainerLeftPadding;

      dropdown.style.position = "relative";
      dropdown.style.left = `-${totalOffset}px`;
      dropdown.style.width = `${width + totalOffset}px`;
    },

    setAutoCompleteListTopMargin() {
      let autoCompleteList = document.querySelector(
        ".k-animation-container.k-animation-container-relative",
      );

      if (autoCompleteList instanceof HTMLElement) {
        autoCompleteList.style.marginTop = "25px";
      }
    },

    resetAutoCompleteEnum() {
      this.isCompanyHasMatch = false;
      this.formatFirms = [this.keywordFirmSearch];
    },

    displayNoCompanyPopupIfHasNoCompany() {
      if (this.isCompanyHasMatch) {
        return;
      }

      this.eventBusNotFoundCompany.emit(AppConstants.firmNotFoundEvent, {
        msg: `Company '${this.keywordFirmSearch}' does not exists!`,
        company: this.keywordFirmSearch,
      });
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

      if (!currentValue) {
        this.currentlySelectedCompany = undefined;
        const errorMessage = `Please enter a valid ${this.label}`;
        this.addIdKeyAndErrorValue(errorMessage);
        this.errorMessage = errorMessage;
        return errorMessage;
      }

      this.addIdKeyAndErrorValue("");
      return "";
    },

    onMouseOver() {
      if (!this.currentlySelectedCompany) {
        this.inputClass = "default-firm";
        this.controlContainerId = "company-autocomplete-container";
        return;
      }
    },

    setClearIconToCheckIfAuthorised(isAuthorised: boolean) {
      let clearIcon = document.querySelector(
        "span.k-clear-value",
      ) as HTMLSpanElement;

      if (!clearIcon) {
        return;
      }

      this.setClearActionListener(clearIcon);

      if (!isAuthorised) {
        return;
      }

      let child = clearIcon.firstChild as HTMLSpanElement;
      clearIcon.removeChild(child);

      let customSpan = document.createElement("span");

      if (!customSpan) {
        return;
      }

      customSpan.classList.add("k-icon");
      customSpan.classList.add("k-i-check");
      clearIcon.appendChild(customSpan);
    },

    setClearActionListener(element: HTMLElement) {
      const event = () => {
        this.controlContainerId = "company-autocomplete-container";
        this.$emit("onClear");
      };

      element.removeEventListener("click", event);
      element.addEventListener("click", event);
    },

    cleanCompanyName(input: string): string {
      let cleanedCompanyName = input;

      if (cleanedCompanyName.includes("&")) {
        cleanedCompanyName = cleanedCompanyName.replaceAll("&", "and");
      }

      if (cleanedCompanyName.includes(this.postCodeKeyword)) {
        let fIndex = cleanedCompanyName.indexOf(this.postCodeKeyword);
        cleanedCompanyName = cleanedCompanyName.substring(0, fIndex - 1).trim();
      } else if (cleanedCompanyName.includes(this.regionCodeKeyword)) {
        let fIndex = cleanedCompanyName.indexOf(this.regionCodeKeyword);
        cleanedCompanyName = cleanedCompanyName.substring(0, fIndex - 1).trim();
      } else {
        cleanedCompanyName = cleanedCompanyName.toString();
      }

      cleanedCompanyName = cleanedCompanyName.endsWith(".")
        ? cleanedCompanyName.substring(0, cleanedCompanyName.length - 1)
        : cleanedCompanyName;

      return cleanedCompanyName;
    },

    compareIfMatched(
      fromCompanyHouse: CompanyEntity,
      selectedFromFca: string,
    ): CompanyEntity | undefined {
      let companyName = fromCompanyHouse.companyName;

      if (companyName.includes("&")) {
        companyName = companyName.replaceAll("&", "and");
      }

      const cleanedCompanyName = this.removeLimitedOrLtdIfAny(selectedFromFca);

      if (
        companyName.toLowerCase().startsWith(cleanedCompanyName.toLowerCase())
      ) {
        return fromCompanyHouse;
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

    removeLimitedOrLtdIfAny(companyName: string): string {
      const regexLimited = new RegExp("limited", "i");
      const regexLtd = new RegExp("ltd", "ig");

      if (regexLtd.test(companyName)) {
        return companyName.replace(regexLtd, "");
      }

      if (regexLimited.test(companyName)) {
        return companyName.replace(regexLimited, "");
      }

      return companyName;
    },
  },
});
</script>

<template>
  <div class="position-relative">
    <Field
      :name="name"
      :id="id"
      :validator="validate"
      :component="'myTemplate'"
    >
      <template v-slot:myTemplate="{ props }">
        <StackLayout orientation="vertical" style="gap: 6px">
          <StackLayout
            orientation="horizontal"
            :align="{ horizontal: 'start' }"
          >
            <Label
              v-if="label"
              :editor-id="props.id"
              class="control-label paragraph-s-figtree-medium flex-grow-1 mt-7px"
            >
              {{ label }}
              <span v-if="!isRequired" class="fineprint ms-1">
                (Optional)
              </span>
            </Label>
          </StackLayout>

          <StackLayout
            orientation="horizontal"
            :id="controlContainerId"
            class="control-container"
            :style="`padding-left: ${controlContainerLeftPadding}px; ${
              !(props.touched && errorMessage) ? '' : 'border-color: #f31700'
            }`"
            :class="inputClass"
            :align="{ horizontal: 'start', vertical: 'middle' }"
          >
            <IconComponent
              symbol="search-icon"
              :size="searchIconWidth.toString()"
              class="magnifier"
            />

            <AutoComplete
              v-bind="props"
              id="fca-autocomplete"
              :fillMode="null"
              class="control"
              :placeholder="placeholder"
              :opened="isOpened"
              :data-items="formatFirms"
              :value="value"
              @change="onValueChangeAsync"
              @open="onOpen"
              @close="onClose"
              @select="onSelectAsync"
              @mouseover="onMouseOver"
            />
          </StackLayout>

          <Label class="mt-2 paragraph-s-figtree-regular" v-if="isOpened">
            <span class="k-icon k-i-information" />
            If your firm doesn't appear in the dropdown list, please enter it
            manually.
          </Label>

          <StackLayout
            v-if="isLoadingCompanyHouseDetails"
            class="mt-2 caption-figtree-medium"
          >
            Searching...
          </StackLayout>

          <StackLayout
            class="mt-2"
            v-else-if="isShowBottomResults && currentlySelectedCompany"
            orientation="horizontal"
            :align="{ horizontal: 'start' }"
          >
            <Label class="caption-figtree-medium">
              Firm Reference Number:&nbsp;<span class="caption-frn"
                >{{ currentlySelectedCompany.firmReferenceNo }}
              </span></Label
            >

            <span class="mx-2 caption-figtree-medium">|</span>

            <Label class="caption-figtree-medium">
              Company Number:&nbsp;<span class="caption-figtree-semi-bold">{{
                currentlySelectedCompany.companyNumber
              }}</span>
            </Label>
          </StackLayout>

          <Error class="error" v-if="props.touched && errorMessage"
            >{{ errorMessage }}
          </Error>
        </StackLayout>
      </template>
    </Field>
  </div>
</template>

<style scoped>
.control-container {
  background-color: white;
  border: var(--content-content-07) solid thin;
  border-radius: 8px;
}

.control {
  color: var(--text-text-primary);
  /* font-family: Figtree; */
  font-size: var(--font-size-default);
  font-style: normal;
  font-weight: var(--font-weight-normal);
  line-height: 130%; /* 20.8px */
  border-width: 0;
}

.paragraph-s-figtree-regular {
  color: var(--text-text-tertiary);
  font-size: 14px;
  font-style: normal;
  font-weight: var(--font-weight-normal);
  line-height: 125%; /* 17.5px */
}

.caption-figtree-semi-bold {
  color: var(--text-text-primary);
  text-align: center;
  font-size: var(--font-size-sm);
  font-style: normal;
  font-weight: 600;
  line-height: 16px; /* 133.333% */
}

.authorised-firm {
  border-color: var(--color-success-600);
}

.non-authorised-firm {
  border-color: var(--content-content-06);
}

.default-firm {
  border-color: var(--content-content-07);
}

.status-label {
  justify-content: center;
  align-items: center;
  gap: 4px;
  border-radius: 150px;
  position: absolute;
  right: 0;
}

.status-label-authorised {
  background: var(--color-success-50);
}

.status-label-authorised > label {
  color: var(--color-success-700);
}

.status-label-non-authorised {
  background: var(--color-grey-classic-200);
}

.status-label-non-authorised > label {
  color: var(--text-text-tertiary);
}
</style>