<script setup lang="ts">
import { toRefs, computed, watch } from "vue";
import { storeToRefs } from "pinia";
import { AppConstants } from "@/infra/AppConstants";
import moment from "moment";
import { v4 as uuid } from "uuid";
import { FinancialPromotion } from "@/entities/financial-promotion/FinancialPromotion";
import { useCustomerStore } from "@/stores/useCustomerStore";
import { useCommentStore } from "@/stores/useCommentStore";
import { CommentType } from "@/entities/comment/CommentType";
import { CommentEntity } from "@/entities/comment/CommentEntity";

const props = defineProps<{
  promotion: FinancialPromotion;
}>();

const { promotion } = toRefs(props);

const { currentCustomerName } = storeToRefs(useCustomerStore());

const commentStore = useCommentStore();
const { comments } = storeToRefs(commentStore);
const { fetchCommentsAsync } = commentStore;

interface Log extends CommentEntity {
  isForDate?: boolean;
}

const logs = computed(() => {
  const logs: Log[] = [];
  comments.value
    .sort((a, b) => (Number(a.createdAt) < Number(b.createdAt) ? 1 : -1))
    .forEach((comment) => {
      const lastLog = logs[logs.length - 1];

      if (
        !lastLog ||
        moment(Number(comment.createdAt) * 1e3).format("L") !==
          moment(Number(lastLog.createdAt) * 1e3).format("L")
      ) {
        logs.push({
          ...comment,
          id: uuid(),
          isForDate: true,
          isPublic: true,
        });
      }

      logs.push(comment);
    });

  return logs;
});

const getIcon = (event?: string) => {
  switch (event) {
    case CommentType.Comment:
      return "comment-bubble";
    case CommentType.Approve:
      return "check-big";
    case CommentType.Reject:
      return "clear";
    case CommentType.Create:
    case CommentType.Modify:
    default:
      return "edit-pdf-50";
  }
};

const getTitle = (item: CommentEntity) => {
  switch (item.commentType) {
    case CommentType.Comment:
      return `left a comment.`;
    case CommentType.Approve:
      return `approved this post.`;
    case CommentType.RevokeApprove:
      return `revoke the approval.`;
    case CommentType.RevokeReject:
      return `revoke the rejection.`;
    case CommentType.Reject:
      return `rejected this post.`;
    case CommentType.Modify:
      return `modified the text.`;
    case CommentType.Create:
      return `created this post.`;
  }
};

watch(
  promotion,
  (value) => {
    if (!value.id) {
      return;
    }
    fetchCommentsAsync(
      promotion.value.id,
      AppConstants.CommentFinancialPromotion
    );
  },
  { immediate: true }
);
</script>

<!-- ToDo. Move timeline component in global -->
<template>
  <div class="Timeline">
    <div v-if="!logs.length" class="p-4 text-center">
      <em>No logs yet.</em>
    </div>
    <div v-else class="Timeline-body">
      <div
        v-for="log in logs"
        :key="`timeline-log-${log.id}`"
        :class="[`Timeline-${log.isForDate ? 'heading' : 'item'}`]"
      >
        <template v-if="log.isForDate">
          <strong>
            {{
              moment(Number(log.createdAt) * 1e3).calendar({
                sameDay: "[Today]",
                lastDay: "[Yesterday]",
                sameElse: "MMMM D, YYYY",
              })
            }}
          </strong>
        </template>
        <template v-else>
          <div class="Timeline-icon">
            <IconComponent size="18" :symbol="getIcon(log.commentType)" />
          </div>

          <PanelComponent class="Timeline-content">
            <div class="Timeline-contentTop">
              <div class="Timeline-title">
                <DynamicAvatarComponent
                  :text="currentCustomerName"
                  rounded="full"
                  size="small"
                >
                  <template #text>
                    <span class="font-weight-semi-bold">
                      {{ currentCustomerName }}
                    </span>
                    {{ getTitle(log) }}
                  </template>
                </DynamicAvatarComponent>
              </div>
              <div class="Timeline-time">
                {{ moment(Number(log.createdAt) * 1e3).fromNow() }}
              </div>
              <button class="Timeline-switchMode">
                <IconComponent
                  v-if="log.isPublic"
                  size="20"
                  symbol="earth-1-33"
                  class="text-tertiary"
                />
                <IconComponent
                  v-else
                  size="20"
                  symbol="user-multiple-circle-28"
                  class="text-primary"
                />
                <IconComponent size="12" symbol="arrow-down-3-79" />
              </button>
            </div>
            <div class="Timeline-contentBody">
              <div v-html="log.commentText" class="is-text-tight"></div>
            </div>
          </PanelComponent>
        </template>
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
.Timeline {
  $root: &;
  $offset: 12px;

  padding-left: $offset;

  &-body {
    border-left: 2px solid #eee;
  }

  &-item {
    display: flex;
    align-items: flex-start;
    gap: 10px;
    margin-bottom: 15px;
    margin-left: calc(#{$offset} * -1);
  }

  &-icon {
    width: 24px;
    height: 24px;
    flex: 0 0 24px;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 50%;
    background-color: var(--color-primary);
    color: var(--color-white);
  }

  &-content {
    flex-grow: 1;
    background-color: #f9fafb;
    color: var(--color-black);
    font-size: var(--font-size-sm);
    margin-bottom: 0;

    :deep(.Panel-body) {
      padding: 10px;
    }

    &Top {
      display: flex;
      gap: 10px;
      align-items: center;
    }

    &Body {
      padding-top: 15px;
    }
  }

  &-title {
    font-size: var(--font-size-sm);
  }

  &-time {
    color: var(--color-neutral);
    margin-left: auto;
  }

  &-switchMode {
    background: none;
    border: none;
    padding: 0;
    margin: 0;
    display: flex;
    align-items: center;
    gap: 5px;
    color: var(--color-primary);
  }

  &-heading {
    background-color: var(--color-white);
    padding-bottom: 15px;
    margin-left: calc(#{$offset} * -1);
  }

  &-commentor {
    #{$root}-icon {
      :deep(.k-avatar) {
        width: 45px;
        height: 45px;
        flex-basis: 45px;
      }
    }
    #{$root}-content {
      padding-top: 0;
    }
  }

  &-editor {
    flex-direction: column-reverse;
    :deep(.k-toolbar) {
      border-bottom-width: 0;
      border-top-width: 1px;
    }
  }
}
</style>
