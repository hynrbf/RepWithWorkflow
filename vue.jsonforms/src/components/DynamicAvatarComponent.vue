<script lang="ts">
import { BadgeThemeColor } from "@progress/kendo-vue-indicators";
import { PropType, defineComponent } from "vue";

export default defineComponent({
  name: "DynamicAvatar",
  props: {
    type: {
      type: String as PropType<"image" | "text" | "icon">,
      default: "text",
    },
    size: {
      type: String as PropType<"small" | "medium" | "large">,
      default: "medium",
    },
    rounded: {
      type: String as PropType<
        "small" | "medium" | "large" | "full" | undefined
      >,
      default: undefined,
    },
    color: {
      type: String as PropType<BadgeThemeColor>,
      default: "light",
    },
    text: {
      type: String,
      default: "",
    },
    subText: {
      type: String,
      default: "",
    },
    customStyle: {
      type: Object,
      default: () => ({}),
    },
    imageSrc: String,
    imageAlt: String,
    icon: String,
    avatarOnly: Boolean,
    textOnly: Boolean,
    textTruncated: Boolean,
  },
  computed: {
    avatarText() {
      return String(this.text || "Avatar")
        .match(/\b(\w)/g)
        ?.join("")
        .toUpperCase()
        .substring(0, 2);
    },
    avatarImage() {
      const text = this.text || "Avatar";

      return (
        this.imageSrc ||
        `https://api.dicebear.com/7.x/avataaars/svg?backgroundColor=b6e3f4,c0aede,d1d4f9&seed=${text}`
      );
    },
  },
});
</script>

<template>
  <div class="hstack gap-2">
    <KendoAvatar
      v-if="!textOnly"
      :type="type"
      :rounded="rounded"
      :theme-color="color"
      :size="size"
      :style="customStyle"
      class="me-0"
    >
      <slot name="avatar">
        <img
          v-if="type === 'image'"
          :src="avatarImage"
          :alt="imageAlt || 'Avatar image'"
        />

        <span v-else-if="type === 'text'">
          {{ avatarText }}
        </span>

        <span v-else-if="type === 'icon'">
          <i :class="['k-icon', icon]"></i>
        </span>
      </slot>
    </KendoAvatar>

    <div
      v-if="!avatarOnly"
      :class="[
        'vstack justify-content-center gap-1',
        textTruncated && 'is-truncated',
      ]"
    >
      <span :class="[textTruncated && 'is-truncated']">
        <slot name="text" :text="text">
          {{ text }}
        </slot>
      </span>

      <small v-if="subText || $slots.subtext">
        <slot name="subtext" :subText="subText">
          {{ subText }}
        </slot>
      </small>
    </div>
  </div>
</template>
