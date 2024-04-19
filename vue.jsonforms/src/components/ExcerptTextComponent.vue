<script setup lang="ts">
import { toRefs, computed, ref } from "vue";
import { container } from "tsyringe";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";

const props = withDefaults(defineProps<{ text: string; length?: number }>(), {
  text: "",
  length: 100,
});

const helperService = container.resolve<IHelperService>(
  IHelperServiceInfo.name,
);

const { text, length } = toRefs(props);

const cleanText = computed(() =>
  helperService.stripHTMLTags(text.value).trim(),
);

const excerptedText = computed(
  () => cleanText.value.substring(0, length.value) + "...",
);

const isExcerptable = computed(() => cleanText.value.length > length.value);

const isCollapsed = ref(true);
</script>

<template>
  <div v-if="!isExcerptable" v-html="text" class="is-text-tight"></div>
  <template v-else>
    <div v-if="isCollapsed" class="is-text-tight">
      <p>
        {{ excerptedText }}
        <a href="javascript:;" @click.prevent="isCollapsed = false">
          <strong>[Read More]</strong>
        </a>
      </p>
    </div>
    <div v-else v-html="text" class="is-text-tight"></div>
  </template>
</template>