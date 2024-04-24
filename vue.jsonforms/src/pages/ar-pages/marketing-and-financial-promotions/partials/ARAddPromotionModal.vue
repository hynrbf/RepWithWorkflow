<script setup lang="ts">
import { ref, toRefs, computed } from "vue";
import { FinancialPromotion } from "@/entities/financial-promotion/FinancialPromotion";
import { FinancialPromotionType } from "@/entities/financial-promotion/FinancialPromotionType";
import { MediaMarketingOutlet } from "@/entities/media-marketing-outlet/MediaMarketingOutlet";
import StaticList from "@/infra/StaticListService";
import AROutletDropdown from "./AROutletDropdown.vue";
import isEmpty from "lodash/isEmpty";
import {AppConstants} from "@/infra/AppConstants";

const props = withDefaults(
  defineProps<{
    initialValues?: Partial<FinancialPromotion>;
    outlets?: MediaMarketingOutlet[];
    saving?: boolean;
    promotionTypeOptions?: Array<{
      label: string;
      value: FinancialPromotionType;
    }>;
  }>(),
  {
    outlets: () => [],
    promotionTypeOptions: () => [],
  }
);
const emits = defineEmits<{
  (event: "submit", value: Partial<FinancialPromotion>): void;
}>();
const { initialValues } = toRefs(props);

const modalElement = ref();
const formElement = ref();

// ToDo. Get providers from API
const providerNameOptions = ref<any[]>([]);

const addProviderName = (value: string) => {
  providerNameOptions.value = [
    ...providerNameOptions.value,
    {
      label: value,
      value,
    },
  ];
};

const isNot3rdParty = (type: string) => {
  return ["authorised-3rd-party", "non-authorised-3rd-party"].includes(type);
};

const remunerationMethodOptions = computed(() =>
  StaticList.getRemunerationMethods().map((item) => ({
    label: item,
    value: item,
  }))
);

const isUploadContent = ref(false);
const isConfirmed = ref(false);

const handleClose = () => {
  formElement.value.reset();
  isUploadContent.value = false;
};

const isEdit = computed(() => !isEmpty(initialValues?.value));

const handleSubmit = (values: Record<string, unknown>) => {
  // transform media
  values.media = ((values.media || []) as any).map((file: any) => ({
    id: file.uid,
    name: file.name,
    extension: file.extension,
    size: file.size,
    url: file.uploadedUrl,
  }));

  if (isEdit) {
    emits("submit", { ...(initialValues?.value || {}), ...values });
  } else {
    emits("submit", { ...values });
  }
};

const setUniqueIdentifier = (value: string): string => {
  const identifier = `${AppConstants.arMarketingAndFinancialPromotionsRoute}-addPromotionalModal${value}`;
  return identifier.replace(/\s+/g, "").replace("/", "");
};
</script>

<template>
  <ModalComponent
    ref="modalElement"
    :title="
      $t(
        isEdit
          ? 'marketingFinancialPage-editPost'
          : 'marketingFinancialPage-newPost'
      )
    "
    width="960"
    auto-scroll-content
    @close="handleClose"
  >
    <OverlayLoader :loading="saving">
      <KendoForm :initialValues="initialValues" @submit="handleSubmit">
        <KendoFormElementComponent
          ref="formElement"
          v-slot="{ form, updateValue }"
        >
          <div class="row gy-1">
            <div class="col-lg-6">
              <AROutletDropdown :id="setUniqueIdentifier('outletDropdown-mediaOutlet')" :value="form.values.mediaOutlet" />
            </div>

            <div class="col-lg-6">
              <KendoLabel>&nbsp;</KendoLabel>
              <ToggleButtonComponent
                text="Is this post currently live?"
                reverse
                wide
                :model-value="form.values.isLive"
                @update:model-value="
                  (value: any) => updateValue('isLive', value)
                "
              />
            </div>

            <div class="col-lg-6">
              <KendoOfficerSelectComponent
                name="contentOwner"
                :id="setUniqueIdentifier('-contentOwner')"
                label="Content Owner"
                :value="form.values.owner"
              />
            </div>

            <div class="col-lg-6">
              <KendoOfficerSelectComponent
                name="moderator"
                :id="setUniqueIdentifier('-moderator')"
                label="Line Manager of Content Owner"
                :value="form.values.moderator"
              />
            </div>

            <div class="col-lg-6">
              <KendoDropdownListComponent
                name="promotion-type"
                :id="setUniqueIdentifier('-promotion-type')"
                label="Financial Promotion Type"
                :data-items="promotionTypeOptions"
                :value="form.values.type"
                value-primitive
                info="Please specify whose products/services you will be promoting."
              />
            </div>

            <div class="col-lg-6">
              <KendoDropdownListComponent
                name="provider"
                :id="setUniqueIdentifier('-provider')"
                label="Provider Name"
                :data-items="providerNameOptions"
                :value="form.values.provider"
                :disabled="!isNot3rdParty(form.values.type)"
                value-primitive
                :addable="true"
                @add-custom="addProviderName"
              />
            </div>

            <div class="col-lg-6">
              <KendoProductTypeSelectComponent
                name="productType"
                :id="setUniqueIdentifier('-productType')"
                label="Product Type"
                pageName="Marketing and Financial Promotions"
                :disabled="!isNot3rdParty(form.values.type)"
                :value="form.values.productType"
              />
            </div>

            <div class="col-lg-6">
              <KendoDropdownListComponent
                name="remunerationMethod"
                :id="setUniqueIdentifier('-remunerationMethod')"
                label="Remuneration Method"
                :data-items="remunerationMethodOptions"
                :value="form.values.remunerationMethod"
                :disabled="!isNot3rdParty(form.values.type)"
                value-primitive
              />
            </div>

            <div class="col-lg-6">
              <KendoNumericInputComponent
                class="NumberInput"
                name="approvalDays"
                :id="setUniqueIdentifier('-approvalDays')"
                label="Timeline Required for Approval"
                :value="form.values.approvalDays"
                :default-value="7"
                suffix-text="days"
              />
            </div>

            <div class="col-lg-12">
              <KendoGenericInputComponent
                name="contentUrl"
                :id="setUniqueIdentifier('-contentUrl')"
                label="Enter URL (If Applicable. Upload content if none)"
                type="url"
                :is-required="false"
                :value="form.values.contentUrl"
              >
                <template #after-label-text>
                  <div class="ms-auto">
                    No URL?
                    <a href="#" @click.prevent="isUploadContent = true">
                      <b>+ Upload Content</b>
                    </a>
                  </div>
                </template>
              </KendoGenericInputComponent>
            </div>

            <div v-if="isUploadContent" class="col-lg-12">
              <p class="font-size-sm mb-2">
                Upload Files (Accepted file types: jpg, png, mp4, mov, doc, pdf.
                Maximum 25 MB file size)
              </p>

              <KendoUploadInputComponent
                name="media"
                stretched
                multiple
                dropzone
                :allowed-extensions="[
                  '.jpg',
                  '.png',
                  '.mp4',
                  '.mov',
                  '.doc',
                  '.docx',
                  '.pdf',
                ]"
                :disabled="false"
              />
            </div>

            <div v-if="isUploadContent" class="col-lg-12">
              <KendoRichTextComponent
                label="Content Description"
                :id="setUniqueIdentifier('-contentDescription')"
                :is-required="false"
                name="editorContent.content"
              />
            </div>
          </div>

          <KendoCheckbox
            class="ConfirmCheck"
            v-model="isConfirmed"
            :label="$t('marketingFinancialPage-iHerebyConfirm')"
          />
        </KendoFormElementComponent>
      </KendoForm>
    </OverlayLoader>

    <template #footer>
      <div class="text-right">
        <KendoButton
          type="button"
          theme-color="primary"
          :disabled="!isConfirmed || saving"
          @click="formElement.submit()"
        >
          {{ isEdit ? "Save Changes" : "Save & Add Post" }}
        </KendoButton>
      </div>
    </template>
  </ModalComponent>
</template>

<style scoped lang="scss">
.ConfirmCheck {
  margin-top: 20px;
  align-items: flex-start;
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semi-bold);

  :deep(.k-checkbox) {
    margin-right: 10px;
    margin-top: 4px;
  }
}
</style>
