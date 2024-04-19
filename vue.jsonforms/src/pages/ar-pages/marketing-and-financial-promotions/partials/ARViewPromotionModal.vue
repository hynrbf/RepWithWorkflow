<script setup lang="ts">
import { toRefs, computed, ref } from "vue";
import moment from "moment";
import { storeToRefs } from "pinia";
import { AppConstants } from "@/infra/AppConstants";
import { FinancialPromotion } from "@/entities/financial-promotion/FinancialPromotion";
import { useArMediaMarketingOutletStore } from "@/stores/media-marketing-outlet/useArMediaMarketingOutletStore";
import { useFinancialPromotionStore } from "@/stores/financial-promotion/useFinancialPromotionStore";
import { useArStaffStore } from "@/stores/useArStaffStore";
import { useArCustomerStore } from "@/stores/useArCustomerStore";
import { useAlert } from "@/composables/useAlert";
import isEmpty from "lodash/isEmpty";
import { useCommentStore } from "@/stores/useCommentStore";
import { CommentType } from "@/entities/comment/CommentType";
import { FinancialPromotionStatus } from "@/entities/financial-promotion/FinancialPromotionStatus";

import ARPromotionContent from "./ARPromotionContent.vue";
import ARPromotionMedia from "./ARPromotionMedia.vue";
import ARPromotionLogs from "./ARPromotionLogs.vue";
import ARPromotionCommentInput from "./ARPromotionCommentInput.vue";
import ARPromotionDisclosures from "./ARPromotionDisclosures.vue";
import ARPromotionDetails from "./ARPromotionDetails.vue";
import ARPromotionAttachments from "./ARPromotionAttachments.vue";
import {
  NotificationType,
  useNotification,
} from "@/composables/useNotification";

const props = withDefaults(
  defineProps<{ promotion: FinancialPromotion; loading?: boolean }>(),
  { promotion: () => ({}) as FinancialPromotion }
);
const emits = defineEmits<{
  (event: "edit", values: FinancialPromotion): void;
}>();

const { promotion } = toRefs(props);

const isSaving = ref(false);

const mediaMarketingOutletStore = useArMediaMarketingOutletStore();
const { getMediaMarketingOutlet } = mediaMarketingOutletStore;

const financialPromotionStore = useFinancialPromotionStore();
const { saveOrUpdateFinancialPromotionAsync } = financialPromotionStore;

const commentStore = useCommentStore();
const { addOrEditCommentAsync } = commentStore;

const outlet = computed(() =>
  getMediaMarketingOutlet(`${promotion.value.mediaOutlet}`, false)
);

const { currentArCustomer } = storeToRefs(useArCustomerStore());

const staffStore = useArStaffStore();
const { staffs } = storeToRefs(staffStore);
const { fetchStaffsAsync } = staffStore;
fetchStaffsAsync();

const isModirator = computed(
  () => currentArCustomer.value.id === promotion.value.moderator
);

const users = computed(() =>
  staffs.value.map((staff) => ({
    ...staff,
    // @TODO Remove when real avatar is available.
    avatar: `https://api.dicebear.com/7.x/avataaars/svg?backgroundColor=b6e3f4,c0aede,d1d4f9&seed=${staff.name}`,
  }))
);

const contentElement = ref();

const isEdittingContent = ref(false);

const applyChanges = () => {
  contentElement.value.apply();
  isEdittingContent.value = false;
};

const rejectContentChanges = () => {
  useAlert({
    content: "Do you wish to reject the text change(s)?",
    confirmButtonText: "Confirm",
    onConfirm() {
      contentElement.value.discardChanges();
    },
  });
};

const acceptContentChanges = () => {
  useAlert({
    content: "Do you wish to approve the text change(s)?",
    confirmButtonText: "Confirm & Approve",
    onConfirm() {
      contentElement.value.acceptChanges();
    },
  });
};

const approveRejectPost = (isApprove: boolean) => {
  useAlert({
    title: "Confirm",
    content: isApprove
      ? "Do you wish to approve this post?"
      : "Do you wish to reject this post?",
    confirmButtonText: isApprove ? "Confirm & Approve" : "Confirm & Reject",
    confirmButtonThemeColor: isApprove ? "primary" : "error",
    async onConfirm() {
      try {
        isSaving.value = true;
        await saveOrUpdateFinancialPromotionAsync({
          ...promotion.value,
          approvalStatus: isApprove
            ? FinancialPromotionStatus.Approved
            : FinancialPromotionStatus.Rejected,
        });
        // ToDo. Check if need a comment
        // await addOrEditCommentAsync({
        //   contentId: promotion.value.id,
        //   contentType: AppConstants.CommentFinancialPromotion,
        //   commentType: CommentType.Modify,
        //   isPublic: true,
        // });
        useNotification({
          type: NotificationType.SUCCESS,
          content: isApprove ? "Post Approved." : "Post Rejected.",
        });
        isSaving.value = false;
      } catch (error) {
        // ToDo. Create error handler
      }
    },
  });
};

const content = computed(() => {
  const { editorContent, contentUrl, content } = promotion.value;

  if (contentUrl && content?.textContent && !editorContent?.content) {
    return content.textContent?.text || "";
  }

  return editorContent?.content || "";
});

const rawContent = computed(() => {
  const { editorContent, contentUrl, content } = promotion.value;

  if (contentUrl && content?.textContent && !editorContent?.rawContent) {
    return content.textContent?.text || "";
  }

  return editorContent?.rawContent || "";
});

const media = computed(() => {
  const { media, contentUrl, content } = promotion.value;

  if (contentUrl && content?.images) {
    return content.images;
  }

  return media?.filter((media) =>
    AppConstants.ImageExtensions.includes(media.extension ?? "")
  );
});

const attachments = computed(() => {
  const { media, contentUrl, content } = promotion.value;

  if (contentUrl && content?.documents) {
    return content.documents;
  }

  return media?.filter((media) =>
    AppConstants.DocumentExtensions.includes(media.extension ?? "")
  );
});

const updateContent = async (values: Record<string, unknown>) => {
  values = {
    editorContent: {
      content: values.content,
      rawContent: values.rawContent,
      suggestions: values.suggestions,
      commentThreads: values.commentThreads,
    } as FinancialPromotion["editorContent"],
  };

  try {
    isSaving.value = true;
    await saveOrUpdateFinancialPromotionAsync({
      ...promotion.value,
      ...values,
      isDisclosureConfirmed: false,
    });
    await addOrEditCommentAsync({
      contentId: promotion.value.id,
      contentType: AppConstants.CommentFinancialPromotion,
      commentType: CommentType.Modify,
      isPublic: true,
    });
    isSaving.value = false;
  } catch (error) {
    // ToDo. Create error handler
  }
};

const submitComment = (content: string, isPublic: boolean) => {
  addOrEditCommentAsync({
    contentId: promotion.value.id,
    contentType: AppConstants.CommentFinancialPromotion,
    commentType: CommentType.Comment,
    commentText: content,
    isPublic,
  });
};

const confirmDisclosure = async () => {
  isSaving.value = true;
  await saveOrUpdateFinancialPromotionAsync({
    ...promotion.value,
    isDisclosureConfirmed: true,
  });
  isSaving.value = false;
};

const lookupInvalidWords = (content: string) => {
  const words = ["crypto"];

  let newContent = content;

  for (const word of words) {
    const regex = new RegExp(word, "gi");
    newContent = newContent.replace(regex, `<span class="warn">${word}</span>`);
  }

  return newContent;
};
</script>

<template>
  <ModalComponent width="1300" fixed-height class="ViewPromotionModal">
    <template #title>
      {{ $t("marketingFinancialPage-previewFinancialPromotion") }}
      <KendoButton
        type="button"
        fill-mode="flat"
        shape="square"
        size="small"
        @click.prevent="emits('edit', promotion)"
      >
        <IconComponent symbol="edit-pen" size="20" />
      </KendoButton>
    </template>
    <OverlayLoader :loading="loading || isSaving" class="is-vertical-content">
      <PanelComponent v-if="promotion" class="PromotionInfo">
        <div class="row">
          <div class="col-lg-2">
            <label>Source</label>
            <MediaLabelComponent
              :id="outlet?.platform"
              :icon-size="25"
              :text="outlet?.name"
              class="gap-1"
            />
          </div>
          <div class="col-lg-2">
            <label>Content Status</label>
            <LiveLabelComponent :live="promotion.isLive" />
          </div>
          <div class="col-lg-2">
            <label>Content Owner</label>
            <OfficerAvatarComponent
              :id="promotion.owner"
              size="small"
              text-only
              text-truncated
            />
          </div>
          <div class="col-lg-2">
            <label>Moderator</label>
            <OfficerAvatarComponent
              :id="promotion.moderator"
              size="small"
              rounded="full"
              text-truncated
            />
          </div>
          <div class="col-lg-2">
            <label>Approval Type</label>
            <span>Existing Post</span>
          </div>
          <div class="col-lg-2">
            <label>Approval Required Date</label>
            <span>
              {{
                moment((promotion.createdAt || 0) * 1e3)
                  .add(promotion.approvalDays || 0, "days")
                  .format("LL")
              }}
            </span>
          </div>
          <div class="col-lg-2">
            <label>Financial Promotion Type</label>
            <span>{{ promotion.type }}</span>
          </div>
          <div class="col-lg-2">
            <label>Provider Name</label>
            <span>{{ promotion.provider }}</span>
          </div>
          <div class="col-lg-2">
            <label>Product Type</label>
            <SnipItemsComponent
              :items="promotion.productType"
              popup-title="Product Type"
            >
              <template #visible-items="{ items }">
                <span class="is-truncated d-inline-block w-100">
                  {{ (items || []).join(",") }}
                </span>
              </template>
            </SnipItemsComponent>
          </div>
          <div class="col-lg-2">
            <label>Remuneration Method</label>
            <span>{{ promotion.remunerationMethod }}</span>
          </div>
          <div class="col-lg-2">
            <KendoButton
              theme-color="error"
              class="w-100"
              :disabled="
                !isModirator ||
                promotion.approvalStatus !== FinancialPromotionStatus.Pending
              "
              @click.prevent="approveRejectPost(false)"
            >
              <div class="d-flex align-items-center gap-2">
                <IconComponent symbol="clear" size="12" />
                Reject
              </div>
            </KendoButton>
          </div>
          <div class="col-lg-2">
            <KendoButton
              theme-color="success"
              class="w-100"
              :disabled="
                !isModirator ||
                promotion.approvalStatus !== FinancialPromotionStatus.Pending
              "
              @click.prevent="approveRejectPost(true)"
            >
              <div class="d-flex align-items-center gap-2">
                <IconComponent symbol="check-big" size="12" />
                Approve
              </div>
            </KendoButton>
          </div>
        </div>
      </PanelComponent>
      <div v-if="promotion" class="row is-stretched-content">
        <div class="col-lg-6">
          <PanelComponent class="mb-0 h-100">
            <template #head>
              <div class="row">
                <div class="col-md-6">
                  <a :href="promotion.contentUrl ?? '#'" target="_blank">
                    <MediaLabelComponent
                      :id="outlet?.platform"
                      :icon-size="30"
                      class="gap-1"
                    >
                      <template #text>
                        <b>{{ outlet?.name }}</b> (Source)
                      </template>
                    </MediaLabelComponent>
                  </a>
                </div>
                <div class="col-md-6 text-right">
                  <div v-if="isEdittingContent" class="d-inline-flex gap-2">
                    <KendoButton
                      theme-color="primary"
                      size="small"
                      fill-mode="flat"
                      @click.prevent="isEdittingContent = false"
                    >
                      <strong class="font-weight-semi-bold">Cancel</strong>
                    </KendoButton>
                    <KendoButton
                      theme-color="primary"
                      size="small"
                      @click.prevent="applyChanges"
                    >
                      <strong>Done</strong>
                    </KendoButton>
                  </div>
                  <KendoButton
                    v-else
                    theme-color="warning"
                    size="small"
                    @click.prevent="isEdittingContent = true"
                  >
                    <div class="d-flex align-items-center gap-2 px-1">
                      <IconComponent symbol="pencil-7" size="12" />
                      <strong>Edit to Approve</strong>
                    </div>
                  </KendoButton>
                </div>
              </div>
            </template>
            <ARPromotionDetails>
              <template v-if="promotion.editorContent" #text-content>
                <ARPromotionContent
                  ref="contentElement"
                  :editting="isEdittingContent"
                  :user-id="currentArCustomer.id"
                  :users="users"
                  :content="String(content)"
                  :raw-content="String(rawContent)"
                  :suggestions="promotion.editorContent?.suggestions"
                  :comment-threads="promotion.editorContent?.commentThreads"
                  @update="updateContent"
                >
                  <template #content="{ content }">
                    <div v-html="lookupInvalidWords(content)"></div>
                  </template>
                </ARPromotionContent>
                <div
                  class="RejectApproveButtons"
                  v-if="!isEmpty(promotion.editorContent?.suggestions)"
                >
                  <KendoButton
                    size="small"
                    fill-mode="outline"
                    theme-color="error"
                    @click.prevent="rejectContentChanges"
                  >
                    Reject All Text Changes
                  </KendoButton>
                  <KendoButton
                    size="small"
                    theme-color="success"
                    @click.prevent="acceptContentChanges"
                  >
                    Approve All Text Changes
                  </KendoButton>
                </div>
              </template>
              <template v-if="media" #media-content>
                <ARPromotionMedia :items="media" />
              </template>
              <template #attachments-content>
                <ARPromotionAttachments :items="attachments" />
              </template>
            </ARPromotionDetails>
          </PanelComponent>
        </div>
        <div class="col-lg-6 is-vertical-content gap-4">
          <PanelComponent title="Disclosures" class="mb-0">
            <ARPromotionDisclosures
              :promotion="promotion"
              :disclosures="currentArCustomer.disclosure"
              @confirm="confirmDisclosure"
            />
          </PanelComponent>
          <PanelComponent
            title="Log History"
            class="mb-0 is-stretched-content LogHistory"
            body-class="is-scrollable-y"
          >
            <ARPromotionLogs :promotion="promotion" />
          </PanelComponent>
          <PanelComponent class="mb-0">
            <ARPromotionCommentInput @submit="submitComment" />
          </PanelComponent>
        </div>
      </div>
    </OverlayLoader>
  </ModalComponent>
</template>

<style lang="scss">
// ToDo. Reuse styles from scss
.ViewPromotionModal {
  .k-dialog {
    background-color: #f2f4f7;
  }

  .Panel-title {
    color: var(--color-black);
    font-weight: var(--font-weight-semi-bold);
  }
}

.PromotionInfo {
  label {
    display: block;
    margin-bottom: 5px;
    font-size: var(--font-size-xs);
    font-weight: var(--font-weight-semi-bold);
  }
}

.RejectApproveButtons {
  display: flex;
  gap: 10px;
  padding: 20px 0;

  & > * {
    flex: 1;
  }
}

.LogHistory {
  & > .Panel-head {
    padding-bottom: 20px;
  }
  & > .Panel-body {
    padding-top: 0;
    max-height: 400px;
  }
}
</style>
