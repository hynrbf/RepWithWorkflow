<script setup lang="ts">
import { ref, toRefs } from "vue";
import { MediaMarketingOutlet } from "@/entities/media-marketing-outlet/MediaMarketingOutlet";
import ARMediaDropdown from "./ARMediaDropdown.vue";
import {AppConstants} from "@/infra/AppConstants";

const props = withDefaults(
  defineProps<{
    initialValues?: Partial<MediaMarketingOutlet>;
    saving: boolean;
  }>(),
  {}
);
const emits = defineEmits<{
  (event: "submit", value: Partial<MediaMarketingOutlet>): void;
}>();
const { initialValues } = toRefs(props);

const modalElement = ref();
const formElement = ref();

const handleSubmit = (values: Partial<MediaMarketingOutlet>) => {
  emits("submit", {
    ...(initialValues?.value || {}),
    ...values,
  });
};

const setUniqueIdentifier = (value: string): string => {
  const identifier = `${AppConstants.arMarketingAndFinancialPromotionsRoute}-addOutletModal${value}`;
  return identifier.replace(/\s+/g, "").replace("/", "");
};
</script>

<template>
  <ModalComponent
    ref="modalElement"
    :title="$t('marketingFinancialPage-addMediaMarketingOutlet')"
    @close="formElement.reset()"
  >
    <KendoForm :initialValues="initialValues" @submit="handleSubmit">
      <KendoFormElementComponent ref="formElement" v-slot="{ form }">
        <OverlayLoader :loading="saving">
          <ARMediaDropdown
           :id="setUniqueIdentifier('-mediaOutlet')"
            name="platform"
            :value="form.values?.platform"
            class="mb-3"
          />

          <KendoGenericInputComponent
           :id="setUniqueIdentifier('-accountName')"
            name="name"
            label="Account Name"
            :value="form.values.name"
            class="mb-3"
          />

          <KendoGenericInputComponent
            :id="setUniqueIdentifier('-accountUrl')"
            type="url"
            name="url"
            label="Account URL"
            :value="form.values.url"
            class="mb-3"
          />

          <KendoOfficerSelectComponent
            :id="setUniqueIdentifier('-primaryOwner')"
            name="owner"
            label="Primary Content Owner"
            :value="form.values.owner"
          />
        </OverlayLoader>
      </KendoFormElementComponent>
    </KendoForm>

    <template #footer>
      <div class="text-right">
        <KendoButton
          type="button"
          theme-color="primary"
          :disabled="saving"
          @click="formElement.submit()"
        >
          Save & Add
        </KendoButton>
      </div>
    </template>
  </ModalComponent>
</template>
