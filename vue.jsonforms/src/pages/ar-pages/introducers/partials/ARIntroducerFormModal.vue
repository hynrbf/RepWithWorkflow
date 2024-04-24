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
import { OnboardingType } from "@/infra/base";
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
      isIntroducerAddressesTheSame: false,
      isCompanySelected: this.isCompany,
      firmReferenceNumber: "",
      introducerInternal: this.introducer ?? new IntroducersEntity(),
    };
  },
  computed: {
    OnboardingType() {
      return OnboardingType;
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
        this.introducerInternal.details.emailAddress
      );
    },
    minimumRequiredFieldsAreNotEmptyForSoleTrader() {
      return (
        this.introducerInternal.details.foreName &&
        this.introducerInternal.details.emailAddress
      );
    },
    allRequiredFieldsAreNotEmptyForCompany() {
      return (
        this.introducerInternal.details.name &&
        this.introducerInternal.details.companyNumber &&
        this.introducerInternal.details.fcaFirmRefNo &&
        this.introducerInternal.details.registeredAddress &&
        this.introducerInternal.details.tradingAddress &&
        this.introducerInternal.details.emailAddress &&
        this.introducerInternal.details.contactNumber &&
        this.introducerInternal.representative.forename &&
        this.introducerInternal.representative.surname &&
        this.introducerInternal.representative.emailAddress &&
        this.introducerInternal.representative.contactNumber &&
        this.introducerInternal.representative.jobTitle
      );
    },
    allRequiredFieldsAreNotEmptyForSoleTrader() {
      return (
        this.introducerInternal.details.foreName &&
        this.introducerInternal.details.fcaFirmRefNo &&
        this.introducerInternal.details.homeAddress &&
        this.introducerInternal.details.tradingAddress &&
        this.introducerInternal.details.emailAddress &&
        this.introducerInternal.details.contactNumber
      );
    },
    isEmailValid() {
      if (!this.introducerInternal.details.emailAddress) {
        return false;
      }

      return this.helperService.checkIfEmailFormatIsValid(
        this.introducerInternal.details.emailAddress,
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
  },
  methods: {
    setUniqueIdentifier(value: string): string {
      const identifier = `${AppConstants.arIntroducersRoute}${value}`;
      return this.helperService.removeStringSpacesThenSlash(identifier);
    },

    handleSubmit(values: Partial<IntroducersEntity>) {
      this.$emit("submit", {
        ...(this.introducer || {}),
        ...this.introducerInternal,
      });
      //this.formElement.value.reset();
      return values;
    },

    onIntroducerDetailUpdated(introducer: FirmBasicInfo) {
      this.introducerInternal.details.name = introducer.firmName;
      this.introducerInternal.details.companyNumber = introducer.companyNumber;
      this.introducerInternal.details.fcaFirmRefNo =
        introducer.firmReferenceNumber;
      this.introducerInputFirm = introducer;
      this.introducerInternal.details.registeredAddress = introducer.address;
      this.introducerInternal.details.isCompany = this
        .isCompanySelected as boolean;
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
        this.introducerContactNumber = {
          dialCode: obj["dialCode"],
          country: obj["country"],
          countryCode: obj["countryCode"],
          number: obj["Phone Number"],
        };
        const formattedNumber = obj["Phone Number"].substring(3);
        this.introducerInternal.details.contactNumber = formattedNumber;

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
            this.isIntroducerAddressesTheSame = true;
            return;
          }

          this.introducerInternal.details.isTradingSameAsRegisteredAddress =
            false;
          this.isIntroducerAddressesTheSame = false;
        }
      }
    },

    onToggleTradingAddress() {
      this.isIntroducerAddressesTheSame = !this.isIntroducerAddressesTheSame;

      if (this.isIntroducerAddressesTheSame) {
        this.introducer!.details.tradingAddress =
          this.introducer!.details.registeredAddress ?? "";
      } else {
        this.introducer!.details.tradingAddress = "";
      }
    },

    onFullNameValueChanged(names: string[]) {
      this.introducerInternal.details.foreName = names[0];
      this.introducerInternal.details.lastName = names[1];
      this.introducerInternal.details.isCompany = false;
    },

    onSearchSoleTraderResult(selectedCompany: CompanyEntity) {
      this.firmReferenceNumber = selectedCompany.firmReferenceNo;
      this.introducerInternal.details.fcaFirmRefNo =
        selectedCompany.firmReferenceNo;
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
              :isRequired="true"
              :isValueReactive="true"
              :isDataLoadedCompletely="!isInitializing"
              :value="isCompanySelected"
              @onValueChange="
                (value: boolean) => {
                  isCompanySelected = value as boolean;
                  introducer!.details.isCompany = value;
                }
              "
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
                :onboardingType="OnboardingType.Ar.toString()"
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
                :align="{ horizontal: 'start' }"
              >
                <div class="col-sm-8">
                  <KendoSoleTraderNameInputComponent
                    :gap="20"
                    :id="setUniqueIdentifier('-introducer-soleTraderName')"
                    :isValueReactive="true"
                    :isDataLoadedCompletely="!isInitializing"
                    :firstName="introducer?.details?.foreName"
                    :lastName="introducer?.details?.lastName"
                    @onFullNameChange="onFullNameValueChanged"
                    @onFinishedSearch="onSearchSoleTraderResult"
                  />
                </div>

                <div class="col-sm-4">
                  <KendoFirmReferenceNumberInputComponent
                    :id="setUniqueIdentifier('-firmReferenceNumber')"
                    name="firmReferenceNumber"
                    label="Firm Reference Number"
                    :placeholder="$t('common-firmref-text')"
                    :isEditable="false"
                    :value="introducerInternal?.details?.fcaFirmRefNo"
                    :maxLength="7"
                    :isValueReactive="true"
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
                @onValueChange="
                  (value: string) =>
                    (introducerInternal!.details.registeredAddress = value)
                "
                :value="introducerInternal?.details?.registeredAddress"
                :isValueReactive="true"
                :isDataLoadedCompletely="!isInitializing"
              />
            </div>

            <div v-if="!isCompanySelected && isCompanySelected !== undefined">
              <KendoPostCodeInputComponent
                :id="setUniqueIdentifier('-introducer.details.homeAddress')"
                :name="'introducer.details.homeAddress'"
                class="mt-2"
                label="Home Address"
                @onValueChange="
                  (value: string) =>
                    (introducerInternal!.details.homeAddress = value)
                "
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
                :isChecked="isIntroducerAddressesTheSame"
                :sameAsLabel="$t('same-as-home-address')"
                @onValueChange="
                  (value: string) =>
                    (introducerInternal!.details.tradingAddress = value)
                "
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
                  (value: string) =>
                    (introducerInternal!.details.emailAddress = value)
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
                    (introducerInternal!.details.contactNumber =
                      value as ContactNumber)
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
                  (value: string) => (introducer!.details.website = value)
                "
                :value="introducerInternal?.details?.website"
                :isValueReactive="true"
                :isDataLoadedCompletely="!isInitializing"
              />
            </StackLayout>
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
                :value="introducer?.representative?.title"
                @onValueChange="
                  (value: string) =>
                    (introducerInternal!.representative.title = value)
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
                :isValueReactive="true"
                :isDataLoadedCompletely="!isInitializing"
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
                :isValueReactive="true"
                :isDataLoadedCompletely="!isInitializing"
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
                :isValueReactive="true"
                :isDataLoadedCompletely="!isInitializing"
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
                :isValueReactive="true"
                :isDataLoadedCompletely="!isInitializing"
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
                ? !allRequiredFieldsAreNotEmptyForCompany
                : !allRequiredFieldsAreNotEmptyForSoleTrader
            "
          >
            Request Introducer to Complete Details
          </KendoButton>

          <KendoButton
            type="submit"
            theme-color="primary"
            :disabled="((isCompanySelected ? !minimumRequiredFieldsAreNotEmptyForCompany : !minimumRequiredFieldsAreNotEmptyForSoleTrader) || saving) || !isEmailValid"
            @click="handleSubmit"
          >
            {{ isAdd ? "Save & Add Introducer" : "Save Changes" }}
          </KendoButton>
        </div>
      </KendoFormElementComponent>
    </KendoForm>
  </ModalComponentFlexible>
</template>
