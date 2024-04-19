<script setup lang="ts">
import { ref, computed } from "vue";
import { container } from "tsyringe";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";

const emits = defineEmits<{
  (event: "submit", input: string, isPublic: boolean): void;
}>();

const helperService = container.resolve<IHelperService>(
  IHelperServiceInfo.name
);

const input = ref("");

const isEmpty = computed(
  () => !helperService.stripHTMLTags(input.value).trim()
);

const submit = (isPublic: boolean) => {
  emits('submit', input.value, isPublic);
  input.value = "";
}
</script>

<template>
  <div class="PromotionCommentInput">
    <KendoEditor
      :value="input"
      class="PromotionCommentInput-editor"
      :tools="[]"
      :content-style="{ height: '30px' }"
      placeholder="Add Comment"
      @change="input = $event.html"
    >
    </KendoEditor>
    <div class="PromotionCommentInput-buttons">
      <KendoButton
        :disabled="isEmpty"
        theme-color="tertiary"
        class="text-primary font-weight-semi-bold"
        @click.prevent="submit(true)"
      >
        <IconComponent size="15" symbol="earth-1-33" />
        Public
      </KendoButton>
      <KendoButton
        :disabled="isEmpty"
        theme-color="primary"
        @click.prevent="submit(false)"
      >
        <IconComponent size="15" symbol="user-multiple-circle-28" />
        Internal
      </KendoButton>
    </div>
  </div>
</template>

<style scoped lang="scss">
.PromotionCommentInput {
  display: flex;
  gap: 5px;
  background-color: #f2f4f7;
  padding: 5px 10px;
  border-radius: 20px;
  overflow: hidden;

  &-editor {
    background-color: transparent;
    border: none;
    flex: 1;
  }

  &-buttons {
    display: flex;
    gap: 10px;

    :deep(.k-button) {
      padding: 2px 15px;
      height: 30px;
      display: flex;
      align-items: center;
      gap: 5px;
    }
  }
}
</style>
