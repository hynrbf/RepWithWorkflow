<script lang="ts">
import { IntroducersEntity } from "@/entities/providers-and-introducers/IntroducersEntity";
import { AppConstants } from "@/infra/AppConstants";
import { FirmBasicInfo } from "@/entities/FirmBasicInfo";
import {
  IFcaService,
  IFcaServiceInfo,
} from "@/infra/dependency-services/rest/fca/IFcaService";
import { container } from "tsyringe";
import { ContactNumber } from "@/entities/ContactNumber";
import { CompanyEntity } from "@/entities/CompanyEntity";
import { defineAsyncComponent, defineComponent } from "vue";
import { ProductType } from "@/entities/org-structure/ProductType";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";

export default defineComponent({
  name: "IntroducerFormModal",
  components: {
    KendoFirmFinderComponentAsync: defineAsyncComponent(
      () => import("@/components/form-fields/KendoFirmFinderComponent.vue"),
    ),
  },
  props: {
    introducer: IntroducersEntity,
    isAdd: Boolean,
    saving: Boolean,
    isCompany: {
      type: Boolean,
      default: undefined,
    },
  },
  data() {
    return {
      fcaService: container.resolve<IFcaService>(IFcaServiceInfo.name),
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      introducerInputFirm: new FirmBasicInfo(),
      isInitializing: true,
      introducerContactNumber: new ContactNumber(),
      isTradingSameAsRegisteredAddress: false,
      isCompanySelected: this.isCompany,
      firmReferenceNumber: "",
      introducerInternal: this.introducer ?? new IntroducersEntity(),
    };
  },
  computed: {
    ProductType() {
      return ProductType;
    },
    ContactNumber() {
      return ContactNumber;
    },
    AppConstants() {
      return AppConstants;
    },
    minimumRequiredFieldsAreNotEmptyForCompany() {
      return (
        this.introducerInternal.details.name &&
        this.introducerInternal.details.companyNumber &&
        this.introducerInternal.details.fcaFirmRefNo &&
        this.isEmailValid(this.introducerInternal.details.emailAddress)
      );
    },
    minimumRequiredFieldsAreNotEmptyForSoleTrader() {
      return (
        this.introducerInternal.details.foreName &&
        this.isEmailValid(this.introducerInternal.details.emailAddress)
      );
    },
    allRequiredFieldsAreNotEmptyForCompany() {
      return (
        this.introducerInternal.details.name &&
        this.introducerInternal.details.companyNumber &&
        this.introducerInternal.details.fcaFirmRefNo &&
        this.introducerInternal.details.registeredAddress &&
        this.introducerInternal.details.tradingAddress &&
        this.isEmailValid(this.introducerInternal.details.emailAddress) &&
        this.introducerInternal.details.contactNumber &&
        this.introducerInternal.representative.forename &&
        this.introducerInternal.representative.surname &&
        this.introducerInternal.representative.emailAddress &&
        this.introducerInternal.representative.contactNumber &&
        this.introducerInternal.representative.jobTitle &&
        this.introducerInternal.details.productType &&
        this.introducerInternal.details.productType.length > 0
      );
    },
    allRequiredFieldsAreNotEmptyForSoleTrader() {
      return (
        this.introducerInternal.details.foreName &&
        this.introducerInternal.details.fcaFirmRefNo &&
        this.introducerInternal.details.homeAddress &&
        this.introducerInternal.details.tradingAddress &&
        this.isEmailValid(this.introducerInternal.details.emailAddress) &&
        this.introducerInternal.details.contactNumber &&
        this.introducerInternal.details.productType &&
        this.introducerInternal.details.productType.length > 0
      );
    },
  },
  mounted() {
    this.introducerInternal = this.introducer ?? new IntroducersEntity();
    this.introducerInputFirm.firmName = this.introducer?.details.name;
    this.introducerInputFirm.companyNumber =
      this.introducer?.details.companyNumber;
    this.introducerInputFirm.firmReferenceNumber =
      this.introducer?.details.fcaFirmRefNo;
    this.isCompanySelected = this.isCompany;
  },
  methods: {
    isEmailValid(email?: string): boolean {
      if (!email) {
        return false;
      }

      return this.helperService.checkIfEmailFormatIsValid(email);
    },

    setUniqueIdentifier(value: string): string {
      const identifier = `${AppConstants.introducersRoute}${value}`;
      return identifier.replace(/\s+/g, "").replace("/", "");
    },

    handleSubmit(values: Partial<IntroducersEntity>) {
      this.$emit("submit", {
        ...(this.introducer || {}),
        ...this.introducerInternal,
      });
      return values;
    },

    handleRequest(values: Partial<IntroducersEntity>) {
      this.$emit("request", {
        ...(this.introducer || {}),
        ...this.introducerInternal,
      });
      return values;
    },

    onIntroducerDetailUpdated(introducer: FirmBasicInfo) {
      this.introducerInternal.details.name = introducer.firmName;
      this.introducerInternal.details.companyNumber = introducer.companyNumber;
      this.introducerInternal.details.fcaFirmRefNo =
        introducer.firmReferenceNumber;
      this.introducerInputFirm = introducer;
      this.introducerInternal.details.registeredAddress = introducer.address;
      this.introducerInternal.details.isCompany = true;
      this.introducerInternal.representative.notApplicable = false;

      this.getFCADetails();
    },

    async getFCADetails() {
      const addressFromFca = await this.fcaService.getFirmAddressesDetailsAsync(
        this.introducerInternal.details.fcaFirmRefNo as string,
        "PPOB",
      );

      if (addressFromFca?.length > 0) {
        const selectedAddress = addressFromFca[0];
        const obj = JSON.parse(JSON.stringify(selectedAddress));

        if (obj["Phone Number"] && obj["country"]) {
          this.introducerContactNumber =
            await this.helperService.convertToContactNoAsync(
              obj["Phone Number"],
              obj["country"],
            );
          this.introducerInternal.details.contactNumber =
            this.introducerContactNumber;
        }

        this.introducerInternal.details.website = obj["Website Address"];
        // build address
        let address = "";
        address += `${obj["Address Line 1"] ? obj["Address Line 1"] : ""}`;
        address += `${
          obj["Address Line 2"] ? `, ${obj["Address Line 2"]}` : ""
        }`;
        address += `${selectedAddress.town ? `, ${selectedAddress.town}` : ""}`;
        address += `${
          selectedAddress.country ? `, ${selectedAddress.country}` : ""
        }`;
        address += `${
          selectedAddress.postcode ? `, ${selectedAddress.postcode}` : ""
        }`;

        this.introducerInternal.details.tradingAddress = address;

        if (
          selectedAddress.country &&
          selectedAddress.postcode &&
          selectedAddress.town
        ) {
          if (
            this.introducerInternal.details.registeredAddress
              ?.toUpperCase()
              .includes(selectedAddress?.country) &&
            this.introducerInternal.details.registeredAddress?.includes(
              selectedAddress?.postcode,
            ) &&
            this.introducerInternal.details.registeredAddress?.includes(
              selectedAddress?.town,
            )
          ) {
            this.introducerInternal.details.isTradingSameAsRegisteredAddress =
              true;
            this.isTradingSameAsRegisteredAddress = true;
            return;
          }

          this.introducerInternal.details.isTradingSameAsRegisteredAddress =
            false;
          this.isTradingSameAsRegisteredAddress = false;
        }
      }
    },

    onIntroducerRegisteredAddressChanged(value: string) {
      if (this.introducerInternal && this.introducerInternal.details) {
        this.introducerInternal.details.registeredAddress = value;
        this.isTradingSameAsRegisteredAddress =
          this.introducerInternal.details.registeredAddress?.toLowerCase() ===
          this.introducerInternal.details.tradingAddress?.toLowerCase();
      }
    },

    onIntroducerHomeAddressChanged(value: string) {
      if (this.introducerInternal && this.introducerInternal.details) {
        this.introducerInternal.details.homeAddress = value;
        this.isTradingSameAsRegisteredAddress =
          this.introducerInternal.details.homeAddress?.toLowerCase() ===
          this.introducerInternal.details.tradingAddress?.toLowerCase();
      }
    },

    onIntroducerTradingAddressChanged(value: string) {
      if (this.introducerInternal && this.introducerInternal.details) {
        this.introducerInternal.details.tradingAddress = value;
        this.isTradingSameAsRegisteredAddress = this.isCompanySelected
          ? this.introducerInternal.details.registeredAddress?.toLowerCase() ===
            this.introducerInternal.details.tradingAddress?.toLowerCase()
          : this.introducerInternal.details.homeAddress?.toLowerCase() ===
            this.introducerInternal.details.tradingAddress?.toLowerCase();
      }
    },

    onToggleTradingAddress() {
      this.isTradingSameAsRegisteredAddress =
        !this.isTradingSameAsRegisteredAddress;
      this.introducerInternal!.details.tradingAddress = this
        .isTradingSameAsRegisteredAddress
        ? this.isCompanySelected
          ? this.introducerInternal?.details.registeredAddress
          : this.introducerInternal.details.homeAddress
        : "";
    },

    onFullNameValueChanged(names: string[]) {
      if (!names.length) {
        return;
      }

      this.introducerInternal.details.foreName = names[0];
      this.introducerInternal.firstName = names[0];

      if (names.length > 1) {
        this.introducerInternal.details.lastName = names[1];
        this.introducerInternal.lastName = names[1];
        this.introducerInternal.details.name = `${this.introducerInternal.firstName} ${this.introducerInternal.details.lastName}`;
      } else {
        this.introducerInternal.details.name =
          this.introducerInternal.firstName;
      }

      this.introducerInternal.details.isCompany = false;
      this.introducerInternal.representative.notApplicable = true;
    },

    onSearchSoleTraderResult(selectedCompany: CompanyEntity) {
      this.firmReferenceNumber = selectedCompany.firmReferenceNo;
      this.introducerInternal.details.fcaFirmRefNo =
        selectedCompany.firmReferenceNo;
    },

    toggleCompanySoleTraderButtonHandler(value: boolean) {
      this.isCompanySelected = value as boolean;
      this.introducerInternal = new IntroducersEntity();
      this.introducerInternal.details.isCompany = value;
    },
  },
});
</script>

<template>
  <ModalComponentFlexible
    ref="modalElement"
    :title="`${isAdd ? 'Add New Introducer' : 'Edit'}`"
    :isTitleCentered="!isAdd"
  >
    <KendoForm :initialValues="introducer" @submit="handleSubmit">
      <KendoFormElementComponent>
        <PanelComponent>
          <Label class="flex-grow-1 section-title k-color-primary">{{
            $t("introducersPage-formHeader")
          }}</Label>

          <StackLayout
            class="my-3"
            orientation="vertical"
            :gap="20"
            :align="{ vertical: 'top' }"
          >
            <KendoChooseOneQuestionInputComponent
              :id="
                setUniqueIdentifier(
                  '-introducerFormModal.chooseOneCompanyOrSoleTrader',
                )
              "
              name="introducerFormModal.chooseOneCompanyOrSoleTrader"
              :value="isCompanySelected"
              @onValueChange="toggleCompanySoleTraderButtonHandler"
              :questionText="`Is Introducer a Company or Sole Trader? `"
              firstOptionText="Company"
              secondOptionText="Sole Trader"
            />

            <div v-if="isCompanySelected">
              <KendoFirmFinderComponentAsync
                :id="setUniqueIdentifier('-introducerInputFirm')"
                companyNameLabel="Introducer Name"
                :company="introducerInputFirm"
                :companyName="introducerInternal?.details?.name"
                :companyNumber="introducerInternal?.details?.companyNumber"
                :firmReferenceNumber="introducerInternal?.details?.fcaFirmRefNo"
                :isInitializing="isInitializing"
                @onCompanyDetailUpdated="onIntroducerDetailUpdated"
              />
            </div>

            <div
              class="row"
              v-if="
                isCompanySelected === false && isCompanySelected !== undefined
              "
            >
              <StackLayout
                orientation="horizontal"
                :gap="20"
                :align="{ horizontal: 'stretch' }"
              >
                <div style="flex: 2">
                  <KendoSoleTraderNameInputComponent
                    :gap="20"
                    :id="setUniqueIdentifier('-introducer.soleTraderName')"
                    :isInitializing="isInitializing"
                    :firstName="introducerInternal?.details?.foreName ?? ''"
                    :lastName="introducerInternal?.details?.lastName"
                    @onFullNameChange="onFullNameValueChanged"
                    @onFinishedSearch="onSearchSoleTraderResult"
                  />
                </div>

                <div style="flex: 1">
                  <KendoFirmReferenceNumberInputComponent
                    :id="setUniqueIdentifier('-firmReferenceNumber')"
                    name="firmReferenceNumber"
                    label="Firm Reference Number"
                    :placeholder="$t('common-firmref-text')"
                    :isEditable="false"
                    :value="introducerInternal?.details?.fcaFirmRefNo"
                    :maxLength="7"
                    :isValueReactive="true"
                    :isDataLoadedCompletely="!isInitializing"
                  />
                </div>
              </StackLayout>
            </div>

            <div v-if="isCompanySelected">
              <KendoPostCodeInputComponent
                :id="
                  setUniqueIdentifier('-introducer.details.registeredAddress')
                "
                :name="'introducer.details.registeredAddress'"
                class="mt-2"
                label="Registered Address"
                @onValueChange="onIntroducerRegisteredAddressChanged"
                :value="introducerInternal?.details?.registeredAddress"
                :isValueReactive="true"
                :isDataLoadedCompletely="!isInitializing"
              />
            </div>

            <div v-if="isCompanySelected !== undefined && !isCompanySelected">
              <KendoPostCodeInputComponent
                :id="setUniqueIdentifier('-introducer.details.homeAddress')"
                :name="'introducer.details.homeAddress'"
                class="mt-2"
                label="Home Address"
                @onValueChange="onIntroducerHomeAddressChanged"
                :value="introducerInternal?.details?.homeAddress"
                :isValueReactive="true"
                :isDataLoadedCompletely="!isInitializing"
              />
            </div>

            <div v-if="isCompanySelected !== undefined">
              <KendoPostCodeInputWithSameAsComponent
                :id="setUniqueIdentifier('-introducer.details.tradingAddress')"
                :name="'introducer.details.tradingAddress'"
                class="mt-2"
                label="Trading Address"
                :value="introducerInternal?.details?.tradingAddress"
                :isChecked="isTradingSameAsRegisteredAddress"
                :sameAsLabel="
                  isCompanySelected
                    ? $t('same-as-trading-address')
                    : $t('same-as-home-address')
                "
                @onValueChange="onIntroducerTradingAddressChanged"
                @onToggle="onToggleTradingAddress"
                :isValueReactive="true"
                :isDataLoadedCompletely="!isInitializing"
              />
            </div>
          </StackLayout>

          <div v-if="isCompanySelected !== undefined">
            <StackLayout
              class="my-3"
              orientation="horizontal"
              :gap="20"
              :align="{ horizontal: 'start' }"
            >
              <KendoEmailAddressInputComponent
                :id="setUniqueIdentifier('-introducer.details.emailAddress')"
                :name="'introducer.details.emailAddress'"
                class="col"
                @onValueChange="
                  (value: string) => {
                    introducerInternal!.details.emailAddress = value;
                    introducerInternal!.email = value;
                  }
                "
                :value="introducerInternal?.details?.emailAddress"
                :isValueReactive="true"
                :isDataLoadedCompletely="!isInitializing"
              />

              <KendoTelInputComponent
                :id="setUniqueIdentifier('-introducer.details.contactNumber')"
                :name="'introducer.details.contactNumber'"
                label="Contact Number"
                class="col"
                :value="introducerInternal?.details?.contactNumber"
                @onValueChange="
                  (value: ContactNumber) =>
                    (introducerInternal.details.contactNumber = value)
                "
                :isValueReactive="true"
                :isDataLoadedCompletely="!isInitializing"
              />

              <KendoGenericInputComponent
                :id="setUniqueIdentifier('-introducer.details.website')"
                :name="'introducer.details.website'"
                class="col"
                label="Website"
                :isRequired="false"
                @onValueChange="
                  (value: string) =>
                    (introducerInternal.details.website = value)
                "
                :value="introducerInternal?.details?.website"
                :isValueReactive="true"
                :isDataLoadedCompletely="!isInitializing"
              />
            </StackLayout>
            <!--TODO: Placeholder-->
            <KendoProductTypeSelectComponent
              :id="setUniqueIdentifier('-introducer.details.productType')"
              name="introducer.details.productType"
              label="Product Type"
              pageName="Organisational Structure Chart"
              :value="introducerInternal?.details.productType"
              @onValueChange="
                (productTypes: ProductType[]) =>
                  (introducerInternal.details.productType = productTypes ?? [])
              "
              :isDataLoadedCompletely="!isInitializing"
              :isValueReactive="true"
            />
          </div>
        </PanelComponent>

        <div v-if="isCompanySelected">
          <PanelComponent>
            <Label class="flex-grow-1 section-title k-color-primary">{{
              $t("introducersPage-introducerRepresentativeText")
            }}</Label>
            <StackLayout
              class="my-3"
              orientation="horizontal"
              :gap="20"
              :align="{ horizontal: 'start' }"
            >
              <KendoNameTitleComponent
                :name="'introducer.representative.title'"
                class="col"
                label="Title"
                :id="setUniqueIdentifier('-introducer.representative.title')"
                :is-required="false"
                :value="introducerInternal?.representative?.title"
                @onValueChange="
                  (value: string) =>
                    (introducerInternal.representative.title = value)
                "
                :isValueReactive="true"
                :isDataLoadedCompletely="!isInitializing"
              />

              <KendoGenericInputComponent
                :id="setUniqueIdentifier('-introducer.representative.forename')"
                :name="'introducer.representative.forename'"
                class="col"
                label="Forename(s)"
                @onValueChange="
                  (value: string) =>
                    (introducerInternal!.representative.forename = value)
                "
                :isCapitalizeFirstLetter="true"
                :value="introducerInternal?.representative?.forename"
              />

              <KendoGenericInputComponent
                :id="setUniqueIdentifier('-introducer.representative.surname')"
                :name="'introducer.representative.surname'"
                class="col"
                label="Surname"
                @onValueChange="
                  (value: string) =>
                    (introducerInternal!.representative.surname = value)
                "
                :isCapitalizeFirstLetter="true"
                :value="introducerInternal?.representative?.surname"
              />
            </StackLayout>
            <StackLayout
              class="my-3"
              orientation="horizontal"
              :gap="20"
              :align="{ horizontal: 'start' }"
            >
              <KendoEmailAddressInputComponent
                :id="
                  setUniqueIdentifier('-introducer.representative.emailAddress')
                "
                :name="'introducer.representative.emailAddress'"
                class="col"
                @onValueChange="
                  (value: string) =>
                    (introducerInternal!.representative.emailAddress = value)
                "
                :value="introducerInternal?.representative?.emailAddress"
              />

              <KendoTelInputComponent
                :id="
                  setUniqueIdentifier(
                    '-introducer.representative.contactNumber',
                  )
                "
                :name="'introducer.representative.contactNumber'"
                label="Contact Number"
                class="col"
                :value="introducerInternal?.representative?.contactNumber"
                @onValueChange="
                  (value: ContactNumber) =>
                    (introducerInternal!.representative.contactNumber =
                      value as ContactNumber)
                "
                :isValueReactive="true"
                :isDataLoadedCompletely="!isInitializing"
              />

              <KendoGenericInputComponent
                :id="setUniqueIdentifier('-introducer.representative.jobTitle')"
                :name="'introducer.representative.jobTitle'"
                class="col"
                label="Job Title"
                @onValueChange="
                  (value: string) =>
                    (introducerInternal!.representative.jobTitle = value)
                "
                :value="introducerInternal?.representative?.jobTitle"
              />
            </StackLayout>
          </PanelComponent>
        </div>

        <div class="text-right">
          <KendoButton
            style="margin-right: 10px; font-weight: 600"
            type="button"
            theme-color="primary"
            fill-mode="outline"
            :disabled="
              isCompanySelected
                ? !minimumRequiredFieldsAreNotEmptyForCompany
                : !minimumRequiredFieldsAreNotEmptyForSoleTrader
            "
            @click="handleRequest"
          >
            {{ isAdd ? "Save & Add Introducer" : "Save Changes" }}
          </KendoButton>

          <KendoButton
            type="submit"
            theme-color="primary"
            :disabled="
              isCompanySelected
                ? !allRequiredFieldsAreNotEmptyForCompany
                : !allRequiredFieldsAreNotEmptyForSoleTrader
            "
            @click="handleSubmit"
          >
            Request Introducer to Complete Details
          </KendoButton>
        </div>
      </KendoFormElementComponent>
    </KendoForm>
  </ModalComponentFlexible>
</template>
