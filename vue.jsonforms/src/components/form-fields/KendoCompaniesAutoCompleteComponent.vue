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

export default defineComponent({
  props: {
    id: String,
    name: {
      type: String,
      default: "",
    },
    label: {
      type: String,
      default: "",
    },
    value: {
      type: String,
      default: "",
    },
    placeholder: {
      type: String,
      default: "Type",
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
      default: false,
    },
    isShowToolTipWithoutHover: {
      type: Boolean,
      default: true,
    },
    isEditable: {
      type: Boolean,
      default: true,
    },
    isShowStatus: {
      type: Boolean,
      default: false,
    },
    isShowHighLightColor: {
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
  },
  data() {
    return {
      isFieldValidOnFirstLoad: false,
      isLoadingFcaDetails: false,
      isLoadingCHDetails: false,
      companiesHouseService: container.resolve<ICompaniesHouseService>(
        ICompaniesHouseServiceInfo.name,
      ),
      fcaService: container.resolve<IFcaService>(IFcaServiceInfo.name),
      companies: [] as CompanyEntity[],
      debounceTimer: null as NodeJS.Timeout | null,
      debounceTimerInMs: 500,
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      postCodeKeyword: "(Postcode: ",
      regionCodeKeyword: "(Region: ",
      isInSearchWhileTyping: false,
      eventBusNotFoundCompany: inject("$eventBusService") as Emitter<
        Record<EventType, { msg: string; company: string }>
      >,
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      formatCompanies: [] as string[] | undefined,
      keywordCompanySearch: "",
      currentlySelectedCompany: undefined as CompanyEntity | undefined,
      inputClass: "default-firm",
      controlContainerId: "company-autocomplete-container",
      isShowToolTip: false,
      isCompanyHasMatch: false,
      searchIconWidth: 20,
      controlContainerLeftPadding: 0,
      isFirstLoad: true,
      companyNameAndPostCode: "",
      valueInternal: "",
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
  created() {
    if (this.isValueReactive) {
      return;
    }

    this.isFieldValidOnFirstLoad = this.validate(this.value) == "";
    this.isShowToolTip = this.isShowToolTipWithoutHover;
    this.valueInternal = this.value;
  },
  unmounted() {
    if (!this.id) {
      return;
    }

    this.removeValidationValueByFieldId(this.id);
    this.isUnmounted = true;
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

      this.isFirstLoad = false;
      this.keywordCompanySearch = event.value ?? "";
      this.isFieldValidOnFirstLoad = false;
      this.valueInternal = this.keywordCompanySearch;
      this.$emit("onInput", this.keywordCompanySearch);
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
      if (this.currentlySelectedCompany) {
        const isCompanyNamePresent = new RegExp(
          this.currentlySelectedCompany.companyName,
          "i",
        ).test(this.keywordCompanySearch);
        const isPostCodePresent = new RegExp(
          this.currentlySelectedCompany.postcode,
          "i",
        ).test(this.keywordCompanySearch);
        if (!isCompanyNamePresent || !isPostCodePresent) {
          this.setToIsAuthoriseStatusToDefault();
        }
      }

      let localKeyword = "";
      this.debounceTimer = setTimeout(async () => {
        this.isLoadingCHDetails = true;

        if (
          !this.keywordCompanySearch ||
          this.keywordCompanySearch.length < 3
        ) {
          this.resetAutoCompleteEnum();
          return;
        }

        this.formatCompanies = [];
        localKeyword = this.keywordCompanySearch;
        this.companies =
          await this.companiesHouseService.searchCompaniesAsync(localKeyword);

        // show only companies that starts with the localKeyword
        if (this.companies && this.companies.length > 0) {
          if (this.helperService.isStringNumber(localKeyword.trim())) {
            this.companies = this.companies.filter(
              (c) =>
                c.companyNumber
                  ?.toLowerCase()
                  .startsWith(localKeyword.toLowerCase()),
            );
          } else {
            this.companies = this.companies.filter(
              (c) =>
                c.companyName
                  ?.toLowerCase()
                  .startsWith(localKeyword.toLowerCase()),
            );
          }

          this.companies.forEach((company) => {
            if (company.postcode) {
              this.formatCompanies?.push(
                `${company.companyName} ${this.postCodeKeyword}${company.postcode})`,
              );
            } else if (company.region) {
              this.formatCompanies?.push(
                `${company.companyName} ${this.regionCodeKeyword}${company.region})`,
              );
            } else {
              this.formatCompanies?.push(`${company.companyName}`);
            }
          });

          this.formatCompanies = [...new Set(this.formatCompanies)];
          this.isCompanyHasMatch = true;

          if (this.formatCompanies.length <= 0) {
            this.resetAutoCompleteEnum();
          }
        } else {
          this.resetAutoCompleteEnum();
        }
      }, this.debounceTimerInMs);

      await this.helperService.delayAsync(500);

      if (!(this.formatCompanies && this.formatCompanies.length > 0)) {
        return;
      }

      //avoid duplicate in the list
      this.formatCompanies = [...new Set(this.formatCompanies)];
      this.isLoadingCHDetails = false;
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
      this.isInSearchWhileTyping = true;
    },

    // TODO. to delete soon since we hide the clear icon
    onClose(event: AutoCompleteCloseEvent) {
      let selected = event.target.value as string;

      this.isInSearchWhileTyping = false;

      if (event.event.type !== "click") {
        return;
      }

      let selectedCompanyFromHouse = this.extractSelectedCompanyHouse(selected);

      if (!selectedCompanyFromHouse) {
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
      let companyName = event.target.value as string;
      this.companyNameAndPostCode = companyName;
      this.isInSearchWhileTyping = false;
      let selectedCompanyFromHouse =
        this.extractSelectedCompanyHouse(companyName);
      if (!selectedCompanyFromHouse) {
        return;
      }

      this.isLoadingFcaDetails = true;
      let cleanedCompanyName = companyName.endsWith(".")
        ? companyName.substring(0, companyName.length - 1)
        : companyName;
      let matched = await this.getMatchedFirmsAsync(
        cleanedCompanyName,
        this.isSoleTrader,
        selectedCompanyFromHouse.companyNumber,
        selectedCompanyFromHouse.address,
      ).finally(() => (this.isLoadingFcaDetails = false));

      if (!matched || matched.length === 0) {
        if (!matched) {
          matched = [] as CompanyEntity[];
        }

        selectedCompanyFromHouse.isSelected = true;
        selectedCompanyFromHouse.status = AppConstants.notAuthorised;
        selectedCompanyFromHouse.type = "Firm";
        matched.push(selectedCompanyFromHouse);
      }

      //when the matched firms is only one
      if (!(matched && matched.length > 1)) {
        const firm = matched[0];
        firm.companyHouseStatus = selectedCompanyFromHouse.status;
        await this.mapSelectCompanyPropsOnly(firm);
        this.adjustUiSelectedCompany();
        this.$emit("onValueChange", this.currentlySelectedCompany);
        return;
      }

      //when the matched firms is more than one, always default to the one that is authorized
      const authCompany = matched.find((m) => m.isAuthorized);
      const nonAuthCompany = matched.find((m) => !m.isAuthorized);

      if (authCompany) {
        authCompany.companyHouseStatus = selectedCompanyFromHouse.status;
        await this.mapSelectCompanyPropsOnly(authCompany);
        this.adjustUiSelectedCompany();
        this.$emit("onValueChange", this.currentlySelectedCompany);
        return;
      }

      if (nonAuthCompany) {
        nonAuthCompany.companyHouseStatus = selectedCompanyFromHouse.status;
        await this.mapSelectCompanyPropsOnly(nonAuthCompany);
        this.adjustUiSelectedCompany();
        this.$emit("onValueChange", this.currentlySelectedCompany);
      }
    },

    async mapSelectCompanyPropsOnly(selectedCompany: CompanyEntity) {
      selectedCompany.companyName = selectedCompany.companyName.trim(); // ensure no extra space
      this.currentlySelectedCompany = selectedCompany;
      this.currentlySelectedCompany.isSoleTrader = this.isSoleTrader;
      this.currentlySelectedCompany.firmReferenceNo =
        selectedCompany.firmReferenceNo;
      this.currentlySelectedCompany.isAuthorized = selectedCompany.isAuthorized;
      this.currentlySelectedCompany.status = selectedCompany.status;
      this.currentlySelectedCompany.companyHouseStatus =
        selectedCompany.companyHouseStatus;
      this.currentlySelectedCompany.type = selectedCompany.type;
      this.currentlySelectedCompany.appointedRepresentatives =
        selectedCompany.appointedRepresentatives;
      this.currentlySelectedCompany.address = selectedCompany.address;

      // get the trading address of the specific selected firm
      const addressDetails =
        await this.fcaService.extractTradingAddressDetailsAsync(
          this.currentlySelectedCompany.firmReferenceNo,
        );
      this.currentlySelectedCompany.tradingAddress = addressDetails.address;
      this.currentlySelectedCompany.website = addressDetails.website;
      this.currentlySelectedCompany.contactNumber = addressDetails.phoneNumber;
    },

    adjustUiSelectedCompany(): void {
      if (!this.currentlySelectedCompany) {
        return;
      }

      this.currentlySelectedCompany.isSelected = true;
      this.controlContainerId = this.currentlySelectedCompany.isAuthorized
        ? "company-autocomplete-container-authorised"
        : "company-autocomplete-container-non-authorised";
      this.inputClass =
        this.currentlySelectedCompany.isAuthorized && this.isShowHighLightColor
          ? "authorised-firm"
          : "non-authorised-firm";
      this.setClearIconToCheckIfAuthorised(
        this.currentlySelectedCompany.isAuthorized,
      );
      this.isShowToolTip = true;
    },

    setToIsAuthoriseStatusToDefault(): void {
      if (this.currentlySelectedCompany) {
        this.currentlySelectedCompany.isAuthorized = false;
        this.currentlySelectedCompany.firmReferenceNo = "Not Applicable";
        this.currentlySelectedCompany.companyNumber = "Not Applicable";
        this.currentlySelectedCompany.status = "";
        this.adjustUiSelectedCompany();
        this.$forceUpdate();
      }
    },

    extractSelectedCompanyHouse(selected: string): CompanyEntity | undefined {
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

      let selectedCompanies = this.companies?.filter(
        (c) => c.companyName === companyName,
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
      this.formatCompanies = [];
      this.isCompanyHasMatch = false;
      this.isInSearchWhileTyping = false;
      this.isLoadingCHDetails = false;
      this.eventBusNotFoundCompany.emit(AppConstants.companyNotFoundEvent, {
        msg: "",
        company: this.keywordCompanySearch,
      });
    },

    async getMatchedFirmsAsync(
      keyword: string,
      isFirm: boolean,
      companyNumber: string = "",
      companyAddress = "",
      fromLocalCache: boolean = false,
    ): Promise<CompanyEntity[] | undefined> {
      if (keyword?.length < 4) {
        return;
      }

      if (keyword.includes("&")) {
        keyword = encodeURIComponent(keyword);
      }

      return await this.fcaService
        .getMatchedFirms(
          keyword,
          isFirm,
          companyNumber,
          companyAddress,
          fromLocalCache,
        )
        .catch((error) => {
          throw new Error(error.message);
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

    setClearIconToCheckIfAuthorised(isAuthorised: boolean) {
      let clearIcon = document.querySelector(
        "span.k-clear-value",
      ) as HTMLSpanElement;

      if (!clearIcon) {
        return;
      }

      this.setClearActionListener(clearIcon);
      let child = clearIcon.firstChild as HTMLSpanElement;
      clearIcon.removeChild(child);
      let customSpan = document.createElement("span");
      if (!customSpan) {
        return;
      }
      customSpan.classList.add("k-icon");
      if (!isAuthorised) {
        customSpan.classList.add("k-i-x");
      } else {
        customSpan.classList.add("k-i-check");
      }

      clearIcon.appendChild(customSpan);
    },

    setClearActionListener(element: HTMLElement) {
      const event = () => {
        this.$emit("onClear");
      };

      element.removeEventListener("click", event);
      element.addEventListener("click", event);
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

    getExternalUrl(type: string, value: string) {
      const { fcaURLFirmSearch, chURLCompanyNumberSearch } = AppConstants;

      return type === "fca"
        ? fcaURLFirmSearch(value)
        : chURLCompanyNumberSearch(value);
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
        <StackLayout
          orientation="vertical"
          :style="{ gap: label !== '' ? '6px' : '0px' }"
        >
          <StackLayout
            orientation="horizontal"
            :align="{ horizontal: 'start', vertical: 'middle' }"
            class="position-relative"
          >
            <Label
              v-if="label !== ''"
              :editor-id="props.id"
              class="control-label paragraph-s-figtree-medium flex-grow-1 mt-7px"
            >
              {{ label }}
              <span v-if="!isRequired" class="fineprint ms-1">
                (Optional)
              </span>
            </Label>

            <!-- this is only spacer -->
            <div v-if="isShowStatus" class="status-label">
              <Label class="paragraph-s-figtree-medium" style="display: none">
                Spacer
              </Label>
            </div>

            <div
              v-if="
                isShowStatus &&
                isShowToolTip &&
                currentlySelectedCompany?.status
              "
              class="status-label"
              :class="
                currentlySelectedCompany.isAuthorized
                  ? 'status-label-authorised'
                  : 'status-label-non-authorised'
              "
            >
              <Label class="paragraph-s-figtree-medium font-size-sm">{{
                currentlySelectedCompany.status
              }}</Label>
            </div>
          </StackLayout>
          <StackLayout
            orientation="horizontal"
            :id="controlContainerId"
            class="control-container k-pos-relative"
            :style="`padding-left: ${controlContainerLeftPadding}px;`"
            :class="[inputClass, !props.valid && 'control-invalid']"
            :align="{ horizontal: 'start', vertical: 'middle' }"
          >
            <IconComponent
              symbol="search-icon"
              :size="searchIconWidth.toString()"
              class="magnifier position-absolute l-12px is-zindex-base"
            />

            <AutoComplete
              v-bind="props"
              id="companyHouse"
              class="control pl-32px"
              list-no-data-render="noDataOrLoading"
              :fillMode="null"
              :placeholder="placeholder"
              :opened="isCompanyHasMatch && isInSearchWhileTyping"
              :data-items="formatCompanies"
              :value="valueInternal"
              :loading="
                (isLoadingCHDetails && !isCompanyHasMatch) ||
                isLoadingFcaDetails
              "
              :readonly="isLoadingFcaDetails"
              @change="onValueChangeAsync"
              @open="onOpen"
              @close="onClose"
              @select="onSelectAsync"
            >
              <template #noDataOrLoading>
                <div v-if="isLoadingCHDetails">
                  <StackLayout
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
                </div>
              </template>
            </AutoComplete>
          </StackLayout>

          <Label
            class="mt-2 paragraph-s-figtree-regular"
            v-if="
              !isFirstLoad &&
              !isLoadingFcaDetails &&
              isShowBottomResults &&
              (isInSearchWhileTyping || !currentlySelectedCompany)
            "
          >
            {{ $t("common-cantFindCompany") }}
          </Label>

          <StackLayout
            class="mt-2"
            v-if="
              !isInSearchWhileTyping &&
              !isLoadingFcaDetails &&
              valueInternal &&
              isShowBottomResults
            "
            orientation="horizontal"
            :align="{ horizontal: 'start' }"
          >
            <Label class="caption-figtree-medium">
              Firm Reference Number:&nbsp;
              <span
                v-if="
                  !currentlySelectedCompany ||
                  currentlySelectedCompany?.firmReferenceNo.toUpperCase() ===
                    'NOT APPLICABLE'
                "
                class="caption-figtree-semi-bold"
              >
                {{
                  currentlySelectedCompany?.firmReferenceNo || "Not Applicable"
                }}
              </span>
              <a
                v-else
                :href="
                  getExternalUrl(
                    'fca',
                    currentlySelectedCompany?.firmReferenceNo || '',
                  )
                "
                target="_blank"
                class="caption-figtree-semi-bold"
              >
                {{ currentlySelectedCompany?.firmReferenceNo }}
              </a>
            </Label>

            <span class="mx-2 caption-figtree-medium">|</span>

            <Label class="caption-figtree-medium">
              Company Number:&nbsp;
              <span
                v-if="
                  !currentlySelectedCompany ||
                  currentlySelectedCompany?.companyNumber.toUpperCase() ===
                    'NOT APPLICABLE'
                "
                class="caption-figtree-semi-bold"
              >
                {{
                  currentlySelectedCompany?.companyNumber || "Not Applicable"
                }}
              </span>
              <a
                v-else
                :href="
                  getExternalUrl(
                    'ch',
                    currentlySelectedCompany?.companyNumber || '',
                  )
                "
                target="_blank"
                class="caption-figtree-semi-bold"
              >
                {{ currentlySelectedCompany?.companyNumber }}
              </a>
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
  padding: 0px 5px;
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

.control-invalid {
  border-color: var(--color-error);
}

.mt-7px {
  margin-top: 7px;
}
</style>
