<script setup lang="ts">
import { ref, toRefs, computed } from "vue";
import { FinancialPromotion } from "@/entities/financial-promotion/FinancialPromotion";
import { AppConstants } from "@/infra/AppConstants";
import { isEmpty } from "lodash";
import { container } from "tsyringe";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import { DisclosureEntity } from "@/entities/stationery/DisclosureEntity";

const props = withDefaults(
  defineProps<{
    promotion: FinancialPromotion;
    disclosures?: DisclosureEntity;
  }>(),
  {
    disclosures: () => ({}) as DisclosureEntity,
  }
);

const emits = defineEmits<{ (event: "confirm"): void }>();

const helperService = container.resolve<IHelperService>(
  IHelperServiceInfo.name
);

const { promotion, disclosures } = toRefs(props);

const promotionContent = computed(
  () => promotion.value.editorContent?.content || ""
);

// ToDo. Move this business rules to infra
const verifyDisclosure = (
  disclosure: (typeof AppConstants.disclosureTypes)[0]
) => {
  const cleanContent = helperService
    .stripHTMLTags(promotionContent.value)
    .trim();

  if (disclosure.id === "timePeriodDisclosure") {
    return true;
  }

  if (disclosure.id === "affiliateDisclosure") {
    if (
      helperService.isWordExist(cleanContent, "providers") ||
      helperService.isWordExist(cleanContent, "provider")
    ) {
      return true;
    }
  }

  if (disclosure.id === "taxDisclosure") {
    if (
      helperService.isWordExist(cleanContent, "taxes") ||
      helperService.isWordExist(cleanContent, "tax")
    ) {
      return true;
    }
  }

  return false;
};

const list = computed(() =>
  AppConstants.disclosureTypes
    .map((disclosure) => {
      return {
        ...disclosure,
        content:
          disclosures.value[
            `${disclosure.id}ConfirmedText` as keyof DisclosureEntity
          ] || [],
      };
    })
    .filter((disclosure) => {
      if (isEmpty(disclosure.content)) {
        return false;
      }
      return verifyDisclosure(disclosure);
    })
);

const isOpen = ref(false);
</script>

<template>
  <div class="PromotionDisclosure">
    <PanelComponent
      class="PromotionDisclosure-button"
      @click.prevent="isOpen = true"
    >
      <img src="/file-pdf.svg" alt="PDF" width="23" height="24" />
      <a href="javascript:;" class="PromotionDisclosure-link">
        View Disclosure
      </a>
      <KendoButton
        type="button"
        size="small"
        rounded="full"
        shape="square"
        theme-color="light"
        title="Download"
        class="PromotionDisclosure-download"
        @click.stop
      >
        <IconComponent
          symbol="download-box-2-19"
          size="18"
          class="text-primary"
        />
      </KendoButton>
    </PanelComponent>
    <div class="row align-items-center">
      <div class="col-lg-9 gx-3 PromotionDisclosure-text">
        I hereby confirm that all
        <a href="javascript:;">Disclosures</a> are included within the content,
        while maintaining adequate prominence of no less than the prominence
        afforded to the content itself.
      </div>
      <div class="col-lg-3">
        <KendoButton
          v-if="!promotion.isDisclosureConfirmed"
          theme-color="primary"
          class="w-100"
          @click.prevent="emits('confirm')"
        >
          Confirm
        </KendoButton>
        <!-- ToDo. Request API -->
        <span v-else class="font-size-sm">
          Jenny Berns<br />
          17 November 2023<br />
          08:09 AM
        </span>
      </div>
    </div>

    <ModalComponent v-model="isOpen" fixed-height width="1300">
      <template #before-close>
        <KendoButton
          type="button"
          size="small"
          rounded="full"
          shape="square"
          theme-color="light"
          title="Download"
          @click.stop
        >
          <IconComponent
            symbol="download-box-2-19"
            size="18"
            class="text-primary"
          />
        </KendoButton>
      </template>
      <div
        v-for="disclosure in list"
        :key="`disclosure-${disclosure.id}`"
        class="mb-5"
      >
        <h4 class="font-size-lg text-primary mb-3">
          {{ disclosure.title }}
        </h4>
        <template
          v-for="(content, index) in disclosure.content"
          :key="`disclosure-${disclosure.id}-${index}`"
        >
          <div :data-index="index" v-html="content"></div>
        </template>
      </div>
    </ModalComponent>
  </div>
</template>

<style scoped lang="scss">
.PromotionDisclosure {
  &-button {
    :deep(.Panel-body) {
      padding: 10px;
      display: flex;
      gap: 10px;
      align-items: center;
    }
  }

  &-link {
    flex: 1;
    text-decoration: none;
    font-weight: var(--font-weight-semi-bold);
  }

  &-download {
    width: 30px;
    height: 30px;
  }

  &-text {
    font-size: var(--font-size-sm);
    text-align: justify;
    font-weight: var(--font-weight-semi-bold);
    color: black;
  }
}
</style>
