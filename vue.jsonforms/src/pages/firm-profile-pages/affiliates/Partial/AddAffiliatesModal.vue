<script lang="ts">
import { defineAsyncComponent, defineComponent, inject } from "vue";
import { Affiliate } from "@/entities/Affiliate/Affiliate";
import { AppConstants } from "@/infra/AppConstants";
import { FirmBasicInfo } from "@/entities/FirmBasicInfo";
import {
  IFcaService,
  IFcaServiceInfo,
} from "@/infra/dependency-services/rest/fca/IFcaService";
import { FcaAddressDetail } from "@/entities/FcaAddressDetail";
import { ContactNumber } from "@/entities/ContactNumber";
import { container } from "tsyringe";
import { AlertType, useAlert } from "@/composables/useAlert";
import {
  NotificationType,
  useNotification,
} from "@/composables/useNotification";
import { Emitter, EventType } from "mitt";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";

export default defineComponent({
  name: "AddAffiliatesModal",
  components: {
    KendoFirmFinderComponentAsync: defineAsyncComponent(
      () => import("@/components/form-fields/KendoFirmFinderComponent.vue"),
    ),
  },
  props: {
    saving: Boolean,
    isAdd: {
      type: Boolean,
      default: true,
    },
    affiliate: Affiliate,
  },
  data() {
    return {
      affiliateInputFirm: new FirmBasicInfo(),
      affiliateMarketingProviderInputFirm: new FirmBasicInfo(),
      fcaService: container.resolve<IFcaService>(IFcaServiceInfo.name),
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      eventBusControlElement: inject("$eventBusService") as Emitter<
        Record<EventType, string>
      >,
      isPRAAuthorised: false,
      isInitializing: true,
      affiliateName: "<Marketing Provider Name>",
      isTradingSameAsRegisteredAddress: false,
      isTradingSameAsRegisteredAddressForMarketing: false,
      affiliateContactNumber: new ContactNumber(),
      websiteAddress: "",
      isAffiliateMarketingProvider: undefined as boolean | undefined,
      isLoading: true,
      isShowSavingText: false,
      isShowAffiliateForm: false,
      isSwitchAdjustMarginTop: false,
      affiliateInternal: this.affiliate ?? new Affiliate(),
      isTradingAddressChanged: false, // To remove later after api deployment
      isMarketingTradingAddressChanged: false, // To remove later after api deployment
      isTradingAddressChangedAlertOpened: false,
    };
  },

  computed: {
    ContactNumber() {
      return ContactNumber;
    },
    AppConstants() {
      return AppConstants;
    },
    minimumRequiredFieldsAreNotEmpty() {
      return (
        this.affiliateInternal.details.name &&
        this.affiliateInternal.details.companyNumber &&
        this.affiliateInternal.details.firmReferenceNumber &&
        this.affiliateInternal.details.emailAddress
      );
    },
    allRequiredFieldsAreNotEmpty() {
      return (
        this.affiliateInternal.details.name &&
        this.affiliateInternal.details.companyNumber &&
        this.affiliateInternal.details.firmReferenceNumber &&
        this.affiliateInternal.details.registeredAddress &&
        this.affiliateInternal.details.tradingAddress &&
        this.affiliateInternal.details.emailAddress &&
        this.affiliateInternal.details.contactNumber &&
        this.affiliateInternal.representative.forename &&
        this.affiliateInternal.representative.surname &&
        this.affiliateInternal.representative.emailAddress &&
        this.affiliateInternal.representative.contactNumber &&
        this.affiliateInternal.representative.jobTitle
      );
    },  
    isEmailValid() {
      if (!this.affiliateInternal.details.emailAddress) {
        return false;
      }

      return this.helperService.checkIfEmailFormatIsValid(
        this.affiliateInternal.details.emailAddress,
      );
    },       
  },

  mounted() {
    this.affiliateInternal = this.affiliate ?? new Affiliate();
    this.affiliateInputFirm.firmName = this.affiliateInternal?.details.name;
    this.affiliateInputFirm.companyNumber =
      this.affiliateInternal?.details.companyNumber;
    this.affiliateInputFirm.firmReferenceNumber =
      this.affiliateInternal?.details.firmReferenceNumber;
    this.affiliateInputFirm.tradingAddress =
      this.affiliateInternal?.details.tradingAddress;
    this.affiliateMarketingProviderInputFirm.firmName =
      this.affiliateInternal?.marketingProviderDetails.name;
    this.affiliateMarketingProviderInputFirm.companyNumber =
      this.affiliateInternal?.marketingProviderDetails.companyNumber;
    this.affiliateMarketingProviderInputFirm.firmReferenceNumber =
      this.affiliateInternal?.marketingProviderDetails.firmReferenceNumber;
    this.affiliateMarketingProviderInputFirm.tradingAddress =
      this.affiliateInternal?.marketingProviderDetails.tradingAddress;
  },

  methods: {
    setUniqueIdentifier(value: string): string {
      const identifier = `${AppConstants.affiliatesRoute}${value}`;
      return identifier.replace(/\s+/g, "").replace("/", "");
    },

    async onAffiliatesDetailUpdated(affiliateParam: FirmBasicInfo) {
      if (this.affiliateInternal && this.affiliateInternal.details) {
        this.affiliateInternal.details.name = affiliateParam.firmName;
        this.affiliateInternal.details.companyNumber =
          affiliateParam.companyNumber;
        this.affiliateInternal.details.firmReferenceNumber =
          affiliateParam.firmReferenceNumber;
        this.affiliateInternal.details.registeredAddress =
          affiliateParam.address;
        this.affiliateInternal.details.tradingAddress =
          affiliateParam.tradingAddress;

        await this.isPraAuthorised(
          affiliateParam.firmReferenceNumber as string,
        );
        let addressFromFca = await this.getFCADetailsAsync(
          affiliateParam.firmReferenceNumber as string,
        );
        this.isTradingSameAsRegisteredAddress = this.isSameAsRegisteredAddress(
          this.affiliateInternal.details.registeredAddress as string,
          addressFromFca,
        );
        this.isSwitchAdjustMarginTop = true;
      }
    },

    async onAffiliatesMarketingProviderDetailUpdated(
      affiliateParam: FirmBasicInfo,
    ) {
      if (
        this.affiliateInternal &&
        this.affiliateInternal.marketingProviderDetails
      ) {
        this.affiliateInternal.marketingProviderDetails.name =
          affiliateParam.firmName;
        this.affiliateInternal.marketingProviderDetails.companyNumber =
          affiliateParam.companyNumber;
        this.affiliateInternal.marketingProviderDetails.firmReferenceNumber =
          affiliateParam.firmReferenceNumber;
        this.affiliateInternal.marketingProviderDetails.registeredAddress =
          affiliateParam.address;
        this.affiliateInternal.marketingProviderDetails.tradingAddress =
          affiliateParam.tradingAddress;

        let addressFromFca = await this.getFCADetailsAsync(
          affiliateParam.firmReferenceNumber as string,
        );
        this.isTradingSameAsRegisteredAddressForMarketing =
          this.isSameAsRegisteredAddress(
            this.affiliateInternal.marketingProviderDetails
              .registeredAddress as string,
            addressFromFca,
          );
      }
    },

    handleSubmit(values: Partial<Affiliate>) {
      this.$emit("submit", {
        ...(this.affiliate || {}),
        ...this.affiliateInternal,
      });
      //this.formElement.value.reset();
      return values;
    },

    handleRequestToComplete(affiliate: Partial<Affiliate>) {
      this.$emit("requestToComplete", affiliate);
    },

    async isPraAuthorised(firmReferenceNumber: string) {
      const isAuthorised =
        await this.fcaService.getFirmPraStatusAsync(firmReferenceNumber);
      this.isPRAAuthorised = isAuthorised;
      this.affiliateInternal.details.isPRAAuthorised = isAuthorised;
    },

    onAffiliateTradingAddressDoneTyping(
      elementId: string,
      hasChanged: boolean,
    ) {
      if (!hasChanged || this.isTradingAddressChangedAlertOpened) {
        return;
      }

      if (!this.affiliateInputFirm?.tradingAddress) {
        this.isTradingAddressChanged = false;
        this.affiliateInternal.details.isTradingAddressChanged =
          this.isTradingAddressChanged;
        return;
      }

      if (!this.isAdd) {
        this.handleTradingAddressChange(
          elementId,
          this.affiliateInternal.details.tradingAddress,
          this.affiliateInputFirm.tradingAddress,
          (value: boolean) => {
            this.isTradingAddressChanged = value;

            if (!this.affiliateInternal.details.isTradingAddressChanged) {
              this.affiliateInternal.details.isTradingAddressChanged = value;
            }
          },
          (value: string) => {
            this.affiliateInternal.details.tradingAddress = value;

            if (!this.affiliateInternal.details.isTradingAddressChanged) {
              this.affiliateInternal.details.isTradingAddressChanged = false;
            }
          },
        );
      }
    },

    onAffliateRegisteredAddressChanged(value: string) {
      if (this.affiliateInternal && this.affiliateInternal.details) {
        this.affiliateInternal.details.registeredAddress = value;
        this.isTradingSameAsRegisteredAddress =
          this.affiliateInternal.details.registeredAddress?.toLowerCase() ===
          this.affiliateInternal.details.tradingAddress?.toLowerCase();
      }
    },

    onAffliateTradingAddressChanged(value: string) {
      if (this.affiliateInternal && this.affiliateInternal.details) {
        this.affiliateInternal.details.tradingAddress = value;
        this.isTradingSameAsRegisteredAddress =
          this.affiliateInternal.details.registeredAddress?.toLowerCase() ===
          this.affiliateInternal.details.tradingAddress?.toLowerCase();
      }
    },

    onMarketingTradingAddressDoneTyping(
      elementId: string,
      hasChanged: boolean,
    ) {
      if (!hasChanged || this.isTradingAddressChangedAlertOpened) {
        return;
      }

      if (!this.affiliateMarketingProviderInputFirm?.tradingAddress) {
        this.isMarketingTradingAddressChanged = false;
        return;
      }

      if (!this.isAdd) {
        this.handleTradingAddressChange(
          elementId,
          this.affiliateInternal.marketingProviderDetails.tradingAddress,
          this.affiliateMarketingProviderInputFirm.tradingAddress,
          (value: boolean) => (this.isMarketingTradingAddressChanged = value),
          (value: string) =>
            (this.affiliateInternal.marketingProviderDetails.tradingAddress =
              value),
        );
      }
    },

    onMarketingTradingAddressChanged(value: string) {
      if (this.affiliateInternal) {
        this.affiliateInternal.marketingProviderDetails.tradingAddress = value;
      }
    },

    handleTradingAddressChange(
      elementId: string,
      internalTradingAddress: string | undefined,
      inputFirmTradingAddress: string,
      setIsTradingAddressChanged: (value: boolean) => void,
      clearTradingAddressChange: (value: string) => void,
    ) {
      useAlert({
        title: this.$t("common-alert-title"),
        content: "Are you sure you wish to update your Trading Address?",
        additionalContent: `Current Trading Address with the FCA is ${internalTradingAddress}.`,
        confirmButtonText: "Confirm & Save",
        type: AlertType.ALERT,
        onConfirm: async () => {
          useNotification({
            type: NotificationType.SUCCESS,
            content: "Trading Address updated.",
            interval: AppConstants.notificationPopupTimeOut,
          });

          this.isTradingAddressChangedAlertOpened = false;
          setIsTradingAddressChanged(
            internalTradingAddress !== inputFirmTradingAddress,
          );
          this.eventBusControlElement.emit(
            AppConstants.updateTradingAddressControlStateEvent,
            elementId,
          );
        },
        onClose: () => {
          clearTradingAddressChange(inputFirmTradingAddress);
          setIsTradingAddressChanged(false);
          this.isTradingAddressChangedAlertOpened = false;
        },
      });
    },

    onToggleTradingAddress() {
      this.isTradingSameAsRegisteredAddress =
        !this.isTradingSameAsRegisteredAddress;
      this.affiliate!.details.tradingAddress = this
        .isTradingSameAsRegisteredAddress
        ? this.affiliate?.details.registeredAddress
        : "";
    },

    onToggleTradingAddressForMarketing() {
      this.isTradingSameAsRegisteredAddressForMarketing =
        !this.isTradingSameAsRegisteredAddressForMarketing;
      this.affiliate!.marketingProviderDetails.tradingAddress = this
        .isTradingSameAsRegisteredAddressForMarketing
        ? this.affiliate?.marketingProviderDetails.registeredAddress
        : "";
    },

    isSameAsRegisteredAddress(
      address: string,
      details: FcaAddressDetail,
    ): boolean {
      if (details.country && details.postcode && details.town)
        if (
          address?.toUpperCase().includes(details?.country) &&
          address?.includes(details?.postcode) &&
          address?.includes(details?.town)
        ) {
          return true;
        }

      return false;
    },

    extractAddress(addressFromFca: FcaAddressDetail): string {
      const selectedAddress = addressFromFca;
      const obj = JSON.parse(JSON.stringify(selectedAddress));
      this.affiliateContactNumber = {
        dialCode: obj["dialCode"],
        country: obj["country"],
        countryCode: obj["countryCode"],
        number: obj["Phone Number"],
      };

      // build address
      let address = "";
      address += `${obj["Address Line 1"] ? obj["Address Line 1"] : ""}`;
      address += `${obj["Address Line 2"] ? `, ${obj["Address Line 2"]}` : ""}`;
      address += `${selectedAddress.town ? `, ${selectedAddress.town}` : ""}`;
      address += `${
        selectedAddress.country ? `, ${selectedAddress.country}` : ""
      }`;
      address += `${
        selectedAddress.postcode ? `, ${selectedAddress.postcode}` : ""
      }`;

      return address;
    },

    async getFCADetailsAsync(firmRefNo: string): Promise<FcaAddressDetail> {
      const addressFromFca = await this.fcaService.getFirmAddressesDetailsAsync(
        firmRefNo,
        "PPOB",
      );
      if (addressFromFca?.length > 0) {
        const selectedAddress = addressFromFca;
        const obj = JSON.parse(JSON.stringify(selectedAddress));
        this.websiteAddress = obj["Website Address"];
        //const formattedNumber = obj["Phone Number"].substring(3);
        //this.affiliateContactNumber = formattedNumber;
      }
      return addressFromFca[0];
    },

    closeForm() {
      this.isShowAffiliateForm = false;
    },

    triggerSubmit(): void {
      (this.$refs["affiliateFormElement"] as any)?.$el?.requestSubmit?.();
    },
  },
});
</script>

<template>
  <ModalComponentFlexible
    ref="modalElement"
    :title="`${isAdd ? 'Add New Affiliate' : 'Edit'}`"
    :isTitleCentered="!isAdd"
  >
    <KendoForm :initialValues="affiliate">
      <KendoFormElementComponent ref="affiliateFormElement">
        <StackLayout
          class="my-3"
          orientation="vertical"
          :gap="20"
          :align="{ vertical: 'top' }"
        >
          <PanelComponent>
            <Label>{{ $t("affiliates-label") }}</Label>
            <StackLayout
              orientation="horizontal"
              :align="{ vertical: 'stretch' }"
              style="gap: 15px; display: flex"
            >
              <KendoFirmFinderComponentAsync
                :id="setUniqueIdentifier('-affiliatesDetails.firmFinder')"
                class="flex-grow-1"
                :company="affiliateInputFirm"
                :companyName="affiliateInternal?.details?.name"
                :companyNumber="affiliateInternal?.details?.companyNumber"
                :firmReferenceNumber="
                  affiliateInternal?.details?.firmReferenceNumber
                "
                companyNameLabel="Affiliate Name"
                @onCompanyDetailUpdated="onAffiliatesDetailUpdated"
                :maxLengthBeforeTruncate="14"
              />

              <KendoSwitchToggleComponent
                :id="setUniqueIdentifier('-details.praAuthorised')"
                class="flex-grow-0"
                :value="affiliateInternal?.details?.isPRAAuthorised"
                name="praAuthorised"
                label="PRA Authorized"
                :isSwitchAdjustMarginTop="isSwitchAdjustMarginTop"
              />
            </StackLayout>

            <KendoPostCodeInputComponent
              :id="setUniqueIdentifier('-details.registeredAddress')"
              :name="'registeredAddress'"
              class="mt-2"
              label="Registered Address"
              @onValueChange="onAffliateRegisteredAddressChanged"
              :value="affiliateInternal?.details?.registeredAddress"
              :isValueReactive="true"
            />

            <KendoPostCodeInputWithSameAsComponent
              :id="setUniqueIdentifier('-details.tradingAddress')"
              :name="'details.tradingAddress'"
              label="Trading Address"
              :isUserModified="
                affiliateInternal?.details?.isTradingAddressChanged ??
                isTradingAddressChanged
              "
              :value="affiliateInternal?.details?.tradingAddress"
              :isChecked="isTradingSameAsRegisteredAddress"
              @onToggle="onToggleTradingAddress"
              @onValueChange="onAffliateTradingAddressChanged"
              @onLostFocus="onAffiliateTradingAddressDoneTyping"
              :sameAsLabel="$t('same-as-registered-address')"
              isValueReactive
              :isDataLoadedCompletely="!isInitializing"
            />

            <StackLayout
              class="my-3"
              orientation="horizontal"
              :gap="15"
              :align="{ vertical: 'stretch' }"
            >
              <KendoEmailAddressInputComponent
                :id="setUniqueIdentifier('-details.emailAddress')"
                :value="affiliateInternal?.details?.emailAddress"
                @onValueChange="
                  (value: string) =>
                    (affiliateInternal!.details.emailAddress = value)
                "
                name="details.emailAddress"
                label="Email Address"
              />

              <KendoTelInputComponent
                :id="setUniqueIdentifier('-details.contactNumber')"
                style="margin-left: 15px"
                name="details.affiliateContactNumber"
                label="Contact Number"
                :value="affiliateInternal?.details?.contactNumber"
                @onValueChange="
                  (value: any) =>
                    (affiliateInternal!.details.contactNumber =
                      value as ContactNumber)
                "
                :isValueReactive="true"
                :isDataLoadedCompletely="!isInitializing"
              />
              <KendoGenericInputComponent
                :id="setUniqueIdentifier('-details.website')"
                style="margin-left: 15px"
                class="col"
                name="website"
                label="Website (Optional)"
                placeholder=""
                :isRequired="false"
                :value="affiliateInternal!.details.website"
                @onValueChange="
                  (value: string) =>
                    (affiliateInternal!.details.website = value)
                "
              />
            </StackLayout>
          </PanelComponent>

          <PanelComponent>
            <Label>{{ $t("marketing-label") }}</Label>
            <div>
              <ToggleButtonComponent
                text="Does Affiliate have a Marketing Provider"
                reverse
                wide
                :model-value="isAffiliateMarketingProvider"
                @update:model-value="
                  (value: boolean) => (isAffiliateMarketingProvider = value)
                "
              />
            </div>

            <div v-if="isAffiliateMarketingProvider">
              <StackLayout
                class="my-3"
                orientation="vertical"
                :gap="20"
                :align="{ vertical: 'top' }"
              >
                <KendoFirmFinderComponentAsync
                  :id="
                    setUniqueIdentifier(
                      '-affiliateMarketingProvider.firmFinder',
                    )
                  "
                  class="flex-grow-1"
                  :company="affiliateMarketingProviderInputFirm"
                  :companyName="
                    affiliateInternal?.marketingProviderDetails.name
                  "
                  :companyNumber="
                    affiliateInternal?.marketingProviderDetails?.companyNumber
                  "
                  :firmReferenceNumber="
                    affiliateInternal?.marketingProviderDetails
                      ?.firmReferenceNumber
                  "
                  companyNameLabel="Affiliate Marketing Provider Name"
                  @onCompanyDetailUpdated="
                    onAffiliatesMarketingProviderDetailUpdated
                  "
                  :maxLengthBeforeTruncate="18"
                />

                <KendoPostCodeInputComponent
                  :id="
                    setUniqueIdentifier(
                      '-affiliateMarketingProvider.registeredAddress',
                    )
                  "
                  class="mt-2"
                  label="Registered Address"
                  @onValueChange="
                    (value: string) =>
                      (affiliate!.marketingProviderDetails!.registeredAddress =
                        value)
                  "
                  :value="
                    affiliateInternal?.marketingProviderDetails
                      ?.registeredAddress
                  "
                  :isValueReactive="true"
                  :isDataLoadedCompletely="!isInitializing"
                />

                <KendoPostCodeInputWithSameAsComponent
                  :id="
                    setUniqueIdentifier(
                      '-affiliateMarketingProvider.tradingAddress',
                    )
                  "
                  label="Trading Address"
                  :isRequired="false"
                  :isUserModified="
                    affiliateInternal?.marketingProviderDetails
                      ?.isTradingAddressChanged ??
                    isMarketingTradingAddressChanged
                  "
                  @onToggle="onToggleTradingAddressForMarketing"
                  @onValueChange="onMarketingTradingAddressChanged"
                  @onLostFocus="onMarketingTradingAddressDoneTyping"
                  :isChecked="isTradingSameAsRegisteredAddressForMarketing"
                  :value="
                    affiliateInternal?.marketingProviderDetails?.tradingAddress
                  "
                  :sameAsLabel="$t('same-as-registered-address')"
                  :isValueReactive="true"
                  :isDataLoadedCompletely="!isInitializing"
                />

                <StackLayout
                  class="my-3"
                  orientation="horizontal"
                  :gap="15"
                  :align="{ vertical: 'stretch' }"
                >
                  <KendoEmailAddressInputComponent
                    :id="
                      setUniqueIdentifier(
                        '-affiliateMarketingProvider.emailAddress',
                      )
                    "
                    :value="
                      affiliateInternal?.marketingProviderDetails?.emailAddress
                    "
                    name="emailAddress"
                    label="Email Address"
                    @onValueChange="
                      (value: string) =>
                        (affiliateInternal!.marketingProviderDetails.emailAddress =
                          value)
                    "
                  />

                  <KendoTelInputComponent
                    :id="
                      setUniqueIdentifier(
                        '-affiliateMarketingProvider.affiliateContactNumber',
                      )
                    "
                    style="margin-left: 15px"
                    label="Contact Number"
                    :value="
                      affiliateInternal?.marketingProviderDetails?.contactNumber
                    "
                    @onValueChange="
                      (value: ContactNumber) =>
                        (affiliate!.marketingProviderDetails!.contactNumber =
                          value)
                    "
                    :isValueReactive="true"
                    :isDataLoadedCompletely="!isInitializing"
                  />
                  <KendoGenericInputComponent
                    :id="
                      setUniqueIdentifier('-affiliateMarketingProvider.website')
                    "
                    style="margin-left: 15px"
                    class="col"
                    name="website"
                    label="Website (Optional)"
                    placeholder=""
                    :isRequired="false"
                    :value="
                      affiliateInternal?.marketingProviderDetails?.website
                    "
                    @onValueChange="
                      (value: string) =>
                        (affiliateInternal!.marketingProviderDetails.website =
                          value)
                    "
                  />
                </StackLayout>
              </StackLayout>
            </div>
          </PanelComponent>

          <PanelComponent>
            <Label style="margin-top: 15px"
              >{{
                $t("affiliates-rep-label", {
                  AffiliateName:
                    affiliateInternal?.details?.name || affiliateName,
                })
              }}
            </Label>

            <div class="d-flex" style="margin-top: 15px">
              <KendoNameTitleComponent
                :name="'affiliate.title'"
                :id="'affiliate.title'"
                class="col"
                label="Title"
                :is-required="false"
                :value="affiliate?.representative.title"
                :isValueReactive="true"
                :isDataLoadedCompletely="!isInitializing"
                @onValueChange="
                  (value: string) =>
                    (affiliateInternal!.representative.title = value)
                "
              />
              <KendoGenericInputComponent
                :id="
                  setUniqueIdentifier('-affiliateMarketingProvider.forename')
                "
                style="margin-left: 15px"
                class="col"
                name="forename"
                label="Forename(s) "
                placeholder="12345678"
                :value="affiliate?.representative.forename"
                @onValueChange="
                  (value: string) =>
                    (affiliateInternal!.representative.forename = value)
                "
              />
              <KendoGenericInputComponent
                :id="setUniqueIdentifier('-affiliateMarketingProvider.surname')"
                style="margin-left: 15px"
                class="col"
                name="surname"
                label="Surname"
                placeholder="123456"
                :value="affiliate?.representative.surname"
                @onValueChange="
                  (value: string) =>
                    (affiliateInternal!.representative.surname = value)
                "
              />
            </div>

            <StackLayout
              class="my-3"
              orientation="horizontal"
              :gap="15"
              :align="{ vertical: 'stretch' }"
            >
              <KendoEmailAddressInputComponent
                :id="
                  setUniqueIdentifier(
                    '-affiliateMarketingProvider.values.emailAddress',
                  )
                "
                :value="affiliateInternal?.representative.emailAddress"
                name="representative.emailAddress"
                label="Email Address"
                @onValueChange="
                  (value: string) =>
                    (affiliateInternal!.representative.emailAddress = value)
                "
              />

              <KendoTelInputComponent
                :id="
                  setUniqueIdentifier(
                    '-affiliateMarketingProvider.contactNumber',
                  )
                "
                style="margin-left: 15px"
                name="representative.contactNumber"
                label="Contact Number"
                :value="affiliateInternal?.representative.contactNumber"
                @onValueChange="
                  (value: ContactNumber) =>
                    (affiliateInternal!.representative.contactNumber = value)
                "
                :isValueReactive="true"
                :isDataLoadedCompletely="!isInitializing"
              />
              <KendoGenericInputComponent
                :id="
                  setUniqueIdentifier('-affiliateMarketingProvider.jobTitle')
                "
                style="margin-left: 15px"
                class="col"
                name="jobTitle"
                label="Job Title (Optional)"
                :isRequired="false"
                :value="affiliateInternal?.representative.jobTitle"
                @onValueChange="
                  (value: string) =>
                    (affiliateInternal!.representative.jobTitle = value)
                "
              />
            </StackLayout>
          </PanelComponent>
        </StackLayout>
      </KendoFormElementComponent>
    </KendoForm>

    <template #footer>
      <div class="text-right" style="margin-top: 10px">
        <KendoButton
          v-if="isAdd"
          style="margin-right: 10px; font-weight: 600"
          type="button"
          fill-mode="outline"
          theme-color="primary"
          :disabled="!allRequiredFieldsAreNotEmpty"
          @click="handleRequestToComplete"
        >
          Request Affiliate to Complete Details
        </KendoButton>

        <KendoButton
          type="submit"
          theme-color="primary"
          :disabled="(!minimumRequiredFieldsAreNotEmpty || saving) || !isEmailValid"
          @click="handleSubmit"
        >
          {{ isAdd ? "Save & Add Affiliate" : "Save Changes" }}
        </KendoButton>
      </div>
    </template>
  </ModalComponentFlexible>
</template>

<style scoped>
:global(.Modal .k-dialog) {
  width: 1030px;
  overflow: auto;
}

div > .k-label {
  color: #003366;
  font-weight: 700;
  font-size: var(--font-size-xl);
}
</style>
@/entities/Affiliate