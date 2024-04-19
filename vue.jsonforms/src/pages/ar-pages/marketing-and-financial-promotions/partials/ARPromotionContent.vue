<script setup lang="ts">
import { toRefs, shallowRef, computed, toRaw } from "vue";
import { useCKEditor } from "@/composables/ckeditor/useCKEditor";
import { container } from "tsyringe";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";

const helperService = container.resolve<IHelperService>(
  IHelperServiceInfo.name
);

const props = withDefaults(
  // ToDo. Update "any" to exact typing
  defineProps<{
    userId?: string;
    users?: any;
    content?: string;
    rawContent?: string;
    suggestions?: any;
    commentThreads?: any;
    editting?: boolean;
  }>(),
  {
    userId: "",
    users: () => [],
    content: "",
    rawContent: "",
    suggestions: () => [],
    commentThreads: () => [],
    editting: false,
  }
);

const emits = defineEmits<{
  (event: "update", values: Record<string, unknown>): void;
}>();

const { content, rawContent, suggestions, commentThreads, userId, users } =
  toRefs(props);

const editorInstance = shallowRef();
const editorConfig = computed(() => ({
  classicWithTrackChanges: {
    currentUser: userId.value,
    users: toRaw(users.value),
    suggestions: mapSuggestions(toRaw(suggestions.value), true),
    commentThreads: toRaw(commentThreads.value),
  },
  toolbar: [
    "heading",
    "|",
    "fontsize",
    "fontfamily",
    "|",
    "bold",
    "italic",
    "underline",
    "strikethrough",
    "removeFormat",
    "|",
    "numberedList",
    "bulletedList",
    "|",
    "undo",
    "redo",
    "|",
    "link",
  ],
}));

const { Editor, onInit } = useCKEditor("ClassicWithTrackChangesEditor");

onInit((editor) => {
  editor.execute("trackChanges");
  editor.commands.get("trackChanges")?.forceDisabled?.("suggestionsMode");
  // @TODO Evaluate if still needed
  // editor.commands
  //   .get("acceptSuggestion")
  //   ?.forceDisabled?.("suggestionsMode");
  // editor.commands
  //   .get("acceptAllSuggestions")
  //   ?.forceDisabled?.("suggestionsMode");
  // editor.commands
  //   .get("discardAllSuggestions")
  //   ?.forceDisabled?.("suggestionsMode");
  // editor.commands
  //   .get("discardSuggestion")
  //   ?.forceDisabled?.("suggestionsMode");

  editorInstance.value = editor;
});

const apply = () => {
  if (!editorInstance.value) {
    return;
  }

  const editor = editorInstance.value;
  const trackChanges = editor.plugins.get("TrackChanges");
  const commentsRepository = editor.plugins.get("CommentsRepository");

  const contentData = editor.data.get({ showSuggestionHighlights: true });
  const contentRawData = editor.data.get();
  const suggestionsData = trackChanges.getSuggestions({
    skipNotAttached: true,
    skipEmpty: true,
    toJSON: true,
  });
  const commentThreadsData = commentsRepository.getCommentThreads({
    skipNotAttached: true,
    skipEmpty: true,
    toJSON: true,
  });

  emits("update", {
    content: contentData,
    rawContent: contentRawData,
    suggestions: mapSuggestions(suggestionsData),
    commentThreads: commentThreadsData,
  });
};

const acceptOrDiscardChanges = async (isAccept: boolean) => {
  if (!editorInstance.value) {
    return;
  }

  const editor = editorInstance.value;
  if (isAccept) editor.execute("acceptAllSuggestions");
  else editor.execute("discardAllSuggestions");

  const trackChanges = editor.plugins.get("TrackChanges");
  const commentsRepository = editor.plugins.get("CommentsRepository");
  const contentData = editor.data.get();
  const suggestionsData = trackChanges.getSuggestions({
    skipNotAttached: true,
    skipEmpty: true,
    toJSON: true,
  });
  const commentThreadsData = commentsRepository.getCommentThreads({
    skipNotAttached: true,
    skipEmpty: true,
    toJSON: true,
  });

  emits("update", {
    content: contentData,
    rawContent: contentData,
    suggestions: mapSuggestions(suggestionsData),
    commentThreads: commentThreadsData,
  });
};

const mapSuggestions = (suggestions: any[], toEditor = false) => {
  if (!Array.isArray(suggestions)) {
    return suggestions;
  }

  return suggestions.map((suggestion) => ({
    ...suggestion,
    createdAt: (toEditor
      ? helperService.convertEpochToDateTime(suggestion.createdAt)
      : helperService.dateToEpoch(suggestion.createdAt)) as any,
  }));
};

defineExpose({
  apply,
  acceptChanges() {
    acceptOrDiscardChanges(true);
  },
  discardChanges() {
    acceptOrDiscardChanges(false);
  },
});
</script>

<template>
  <div class="PromotionContent">
    <div v-show="editting" class="PromotionContent-editor">
      <Editor :initial-data="rawContent || content" :config="editorConfig" />
    </div>
    <div v-show="!editting" class="PromotionContent-content is-scrollable-y">
      <slot name="content" :content="content">
        <div v-html="content"></div>
      </slot>
    </div>
  </div>
</template>

<style scoped lang="scss">
.PromotionContent {
  &-editor {
    margin-bottom: 20px;

    :deep(.ck-editor__editable_inline:not(.ck-comment__input *)) {
      height: 300px;
      overflow-y: auto;
    }
  }
  &-content {
    max-height: 325px;

    :deep(.ck-suggestion-marker) {
      &.ck-suggestion-marker-insertion {
        color: var(--color-success);
      }
      &.ck-suggestion-marker-deletion {
        color: var(--color-error);
        text-decoration: line-through;
      }
    }
  }
}
</style>
