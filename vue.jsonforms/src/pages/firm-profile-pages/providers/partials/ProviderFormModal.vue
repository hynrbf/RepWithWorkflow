<script setup lang="ts">
import {
  computed,
  defineAsyncComponent,
  onBeforeMount,
  onMounted,
  ref,
  toRefs,
} from "vue";
import { container } from "tsyringe";
import { ProvidersEntity } from "@/entities/providers-and-introducers/ProvidersEntity";
import {
  IFcaService,
  IFcaServiceInfo,
} from "@/infra/dependency-services/rest/fca/IFcaService";
import { StackLayout } from "@progress/kendo-vue-layout";
import { ContactNumber } from "@/entities/ContactNumber";
import { FirmBasicInfo } from "@/entities/FirmBasicInfo";
import { AppConstants } from "@/infra/AppConstants";
import { usePageLifeCycleStore } from "@/stores/progress-bar/usePageLifeCycleStore";
import PanelComponent from "@/components/PanelComponent.vue";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import { ProductType } from "@/entities/org-structure/ProductType";

const fcaService = container.resolve<IFcaService>(IFcaServiceInfo.name);

const props = withDefaults(
  defineProps<{
    provider?: Partial<ProvidersEntity>;
    isEditMode: false;
    saving: boolean;
  }>(),
  {},
);

const emits = defineEmits<{
  (event: "submit", value: Partial<ProvidersEntity>): void;
  (event: "requestToComplete", value: Partial<ProvidersEntity>): void;
}>();
const { provider, isEditMode } = toRefs(props);
const isInitializing = false;
const formElement = ref();

let providerFirm = new FirmBasicInfo();
let isPRAAuthorised = ref(false);
let isSwitchAdjustMarginTop = ref(false);

const helperService = container.resolve<IHelperService>(
  IHelperServiceInfo.name,
);
const pageLifeCycleStore = usePageLifeCycleStore();
const { changeLifeCycleName } = pageLifeCycleStore;

onBeforeMount(() => {
  changeLifeCycleName(AppConstants.pageLifeCycleNameCreated);
  mapProviderToProviderFirm();
});

onMounted(() => {
  changeLifeCycleName(AppConstants.pageLifeCycleNameMounted);
});

const handleSubmit = () => {
  emits("submit", {
    ...(provider?.value || {}),
  });

  formElement?.value?.$el?.requestSubmit();
};

const handleRequestToComplete = () => {
  emits("requestToComplete", {
    ...(provider?.value || {}),
  });
};

const setUniqueIdentifier = (value: string): string => {
  const identifier = `${AppConstants.providersRoute}${value}`;
  return identifier.replace(/\s+/g, "").replace("/", "");
};

const onToggleTradingAddress = () => {
  if (!provider?.value?.details) {
    return;
  }

  provider.value.details.isTradingSameAsRegisteredAddress =
    !provider.value.details.isTradingSameAsRegisteredAddress;

  if (provider.value.details.isTradingSameAsRegisteredAddress) {
    provider.value.details.tradingAddress =
      provider.value.details.registeredAddress ?? "";
  } else {
    provider.value.details.tradingAddress = "";
  }
};

const isPraAuthorised = async (firmReferenceNumber: string) => {
  const isAuthorised =
    await fcaService.getFirmPraStatusAsync(firmReferenceNumber);
  isPRAAuthorised.value = isAuthorised;

  if (!provider?.value?.details) {
    return;
  }

  provider.value.details.praAuthorised = isAuthorised;
};

const onIsPraAuthorisedChanged = (value: boolean): void => {
  isPRAAuthorised.value = value;

  if (!provider?.value?.details) {
    return;
  }

  provider.value.details.praAuthorised = value;
};

const mapProviderToProviderFirm = () => {
  if (!provider?.value?.details) {
    return;
  }

  providerFirm.firmName = provider.value.details.name;
  providerFirm.companyNumber = provider.value.details.companyNumber;
  providerFirm.firmReferenceNumber = provider.value.details.fcaFirmRefNo;

  if (isEditMode.value) {
    isPRAAuthorised.value = provider.value.details.praAuthorised ?? false;
  }
};

const onCompanyDetailUpdatedAsync = async (company: FirmBasicInfo) => {
  if (!provider?.value?.details) {
    return;
  }

  provider.value.details.name = company.firmName;
  provider.value.details.companyNumber = company.companyNumber;
  provider.value.details.fcaFirmRefNo = company.firmReferenceNumber;
  provider.value.details.registeredAddress = company.address;
  provider.value.details.tradingAddress = company.tradingAddress;
  provider.value.details.website = company.website;

  await getFcsDetailsAsync(provider.value.details.fcaFirmRefNo);

  providerFirm = company;
  isSwitchAdjustMarginTop.value = true;

  await isPraAuthorised(provider.value.details.fcaFirmRefNo ?? "");
};

const getFcsDetailsAsync = async (fcaFirmRefNo: string | undefined) => {
  if (!fcaFirmRefNo) {
    return;
  }

  const addressFromFca = await fcaService.getFirmAddressesDetailsAsync(
    fcaFirmRefNo,
    "PPOB",
  );

  if (addressFromFca?.length > 0) {
    const selectedAddress = addressFromFca[0];
    const obj = JSON.parse(JSON.stringify(selectedAddress));

    if (obj["Phone Number"] && obj["country"] && provider?.value?.details) {
      provider.value.details.contactNumber =
        await helperService.convertToContactNoAsync(
          obj["Phone Number"],
          obj["country"],
        );
    }
  }
};

const KendoFirmFinderComponentAsync = defineAsyncComponent(
  () => import("@/components/form-fields/KendoFirmFinderComponent.vue"),
);

const minimumRequiredFieldsAreNotEmpty = computed(() => {
  if (provider?.value?.details) {
    return (
      provider.value.details.name &&
      provider.value.details.companyNumber &&
      provider.value.details.fcaFirmRefNo &&
      provider.value.details.emailAddress &&
      helperService.checkIfEmailFormatIsValid(
        provider.value.details.emailAddress,
      )
    );
  }

  return false;
});

const allRequiredFieldsAreNotEmpty = computed(() => {
  if (provider?.value?.details && provider?.value?.representative) {
    return (
      provider.value.details.name &&
      provider.value.details.companyNumber &&
      provider.value.details.fcaFirmRefNo &&
      provider.value.details.registeredAddress &&
      provider.value.details.tradingAddress &&
      provider.value.details.emailAddress &&
      provider.value.details.contactNumber &&
      provider.value.representative.forename &&
      provider.value.representative.surname &&
      provider.value.representative.emailAddress &&
      provider.value.representative.contactNumber &&
      provider.value.representative.jobTitle &&
      provider.value.details.productType &&
      provider.value.details.productType.length > 0
    );
  }

  return false;
});
</script>

<template>
  <ModalComponentFlexible
    class="ModalFlexible"
    :title="`${isEditMode ? 'Edit' : 'Add New Provider'}`"
    :close-icon="false"
  >
    <Form>
      <FormElement ref="formElement">
        <StackLayout orientation="vertical" :gap="15" class="col-md-12">
          <PanelComponent>
            <!-- Provider Details Section -->
            <Label class="flex-grow-1 section-title k-color-primary"
              >Provider Details</Label
            >

            <StackLayout
              orientation="horizontal"
              :align="{ horizontal: 'stretch' }"
              style="gap: 15px; display: flex"
            >
              <KendoFirmFinderComponentAsync
                :id="setUniqueIdentifier('-provider.firmFinder')"
                companyNameLabel="Provider Name"
                :company="providerFirm"
                :companyName="provider?.details?.name"
                :companyNumber="provider?.details?.companyNumber"
                :firmReferenceNumber="provider?.details?.fcaFirmRefNo"
                @onCompanyDetailUpdated="onCompanyDetailUpdatedAsync"
              />

              <KendoSwitchToggleComponent
                :id="
                  setUniqueIdentifier(
                    '-providerIntroducerDetails.praAuthorised',
                  )
                "
                label="PRA Authorised"
                class="flex-grow-0"
                :value="isPRAAuthorised"
                :isSwitchAdjustMarginTop="isSwitchAdjustMarginTop"
                :name="`providerIntroducerDetails.praAuthorised`"
                :isRequired="true"
                @onValueChange="onIsPraAuthorisedChanged"
              />
            </StackLayout>

            <KendoPostCodeInputComponent
              :id="setUniqueIdentifier('-provider.details.registeredAddress')"
              :name="'provider.details.registeredAddress'"
              class="mt-2"
              label="Registered Address"
              @onValueChange="
                (value: string) => {
                  if (provider?.details) {
                    provider.details.registeredAddress = value;
                  }
                }
              "
              :value="provider?.details?.registeredAddress"
              :isValueReactive="true"
              :isDataLoadedCompletely="!isInitializing"
            />

            <KendoPostCodeInputWithSameAsComponent
              :id="setUniqueIdentifier('-provider.details.tradingAddress')"
              :name="'provider.details.tradingAddress'"
              label="Trading Address"
              :isUserModified="
                provider?.details?.isTradingSameAsRegisteredAddress
              "
              :value="provider?.details?.tradingAddress"
              :isChecked="provider?.details?.isTradingSameAsRegisteredAddress"
              @onValueChange="
                (value: string) => {
                  if (provider?.details) {
                    provider.details.tradingAddress = value;
                  }
                }
              "
              @onToggle="onToggleTradingAddress()"
              :sameAsLabel="$t('same-as-registered-address')"
              :isValueReactive="true"
              :isDataLoadedCompletely="!isInitializing"
            />

            <StackLayout
              orientation="horizontal"
              :align="{ horizontal: 'stretch' }"
              style="gap: 15px"
            >
              <KendoEmailAddressInputComponent
                :id="setUniqueIdentifier('-provider.emailAddress')"
                label="Email Address"
                name="details.emailAddress"
                :value="provider?.details?.emailAddress"
                @onValueChange="
                  (value: string) => {
                    if (provider?.details) {
                      provider.details.emailAddress = value;
                      provider.email = value;
                    }
                  }
                "
              />

              <KendoTelInputComponent
                :id="setUniqueIdentifier('-provider.contactNumber')"
                label="Contact Number"
                name="details.contactNumber"
                :value="provider?.details?.contactNumber"
                @onValueChange="
                  (value: ContactNumber) => {
                    if (provider?.details) {
                      provider.details.contactNumber = value;
                    }
                  }
                "
              />

              <KendoGenericInputComponent
                :id="setUniqueIdentifier('-provider.website')"
                label="Website"
                name="details.website"
                placeholder="www.domain.com"
                :isRequired="false"
                :value="provider?.details?.website"
                @onValueChange="
                  (value: any) => {
                    if (provider?.details) {
                      provider.details.website = value;
                    }
                  }
                "
              />
            </StackLayout>
            <!--TODO: Placeholder-->
            <KendoProductTypeSelectComponent
              :id="setUniqueIdentifier('-provider.details.productType')"
              name="provider.details.productType"
              label="Product Type"
              pageName="Organisational Structure Chart"
              :value="provider?.details?.productType"
              @onValueChange="
                (productTypes: ProductType[]) => {
                  if (provider?.details) {
                    provider.details.productType = productTypes ?? [];
                  }
                }
              "
              :isDataLoadedCompletely="!isInitializing"
              :isValueReactive="true"
            />
          </PanelComponent>

          <PanelComponent>
            <!-- Representative Details Section -->
            <Label class="flex-grow-1 section-title k-color-primary"
              >{{ provider?.details?.name }} Representative Details</Label
            >

            <StackLayout
              class="my-3"
              orientation="horizontal"
              :align="{ horizontal: 'stretch' }"
              style="gap: 15px"
            >
              <KendoNameTitleComponent
                :id="setUniqueIdentifier('-provider.title')"
                label="Title"
                name="representative.title"
                :isRequired="false"
                :value="provider?.representative?.title"
                @onValueChange="
                  (value: string) => {
                    if (provider?.representative) {
                      provider.representative.title = value;
                    }
                  }
                "
              />

              <KendoGenericInputComponent
                :id="setUniqueIdentifier('-representative.forename')"
                label="Forename(s)"
                name="representative.forename"
                placeholder="John"
                :value="provider?.representative?.forename"
                @onValueChange="
                  (value: string) => {
                    if (provider?.representative) {
                      provider.representative.forename = value;
                    }
                  }
                "
              />

              <KendoGenericInputComponent
                :id="setUniqueIdentifier('-representative.surname')"
                label="Surname"
                name="representative.surname"
                placeholder="Doe"
                :value="provider?.representative?.surname"
                @onValueChange="
                  (value: string) => {
                    if (provider?.representative) {
                      provider.representative.surname = value;
                    }
                  }
                "
              />
            </StackLayout>

            <StackLayout
              orientation="horizontal"
              :align="{ horizontal: 'stretch' }"
              style="gap: 15px"
            >
              <KendoEmailAddressInputComponent
                :id="setUniqueIdentifier('-representative.emailAddress')"
                label="Email Address"
                name="representative.emailAddress"
                :value="provider?.representative?.emailAddress"
                @onValueChange="
                  (value: string) => {
                    if (provider?.representative) {
                      provider.representative.emailAddress = value;
                    }
                  }
                "
              />

              <KendoTelInputComponent
                :id="setUniqueIdentifier('-representative.contactNumber')"
                label="Contact Number"
                name="representative.contactNumber"
                :value="provider?.representative?.contactNumber"
                @onValueChange="
                  (value: ContactNumber) => {
                    if (provider?.representative) {
                      provider.representative.contactNumber = value;
                    }
                  }
                "
              />

              <KendoGenericInputComponent
                :id="setUniqueIdentifier('-representative.jobTitle')"
                label="Job Title"
                name="representative.jobTitle"
                placeholder="Job Title"
                :value="provider?.representative?.jobTitle"
                @onValueChange="
                  (value: string) => {
                    if (provider?.representative) {
                      provider.representative.jobTitle = value;
                    }
                  }
                "
              />
            </StackLayout>
          </PanelComponent>
          <div class="text-right">
            <KendoButton
              style="margin-right: 10px; font-weight: 600"
              type="button"
              theme-color="primary"
              @click="handleRequestToComplete"
              :disabled="!minimumRequiredFieldsAreNotEmpty"
              fill-mode="outline"
            >
              {{ isEditMode ? "Save Changes" : "Save & Add Provider" }}
            </KendoButton>

            <KendoButton
              type="submit"
              theme-color="primary"
              @click="handleSubmit"
              :disabled="!allRequiredFieldsAreNotEmpty"
            >
              Request Provider to Complete Details
            </KendoButton>
          </div>
        </StackLayout>
      </FormElement>
    </Form>
  </ModalComponentFlexible>
</template>

<style scoped lang="scss" />
