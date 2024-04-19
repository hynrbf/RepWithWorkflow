<script lang="ts">
import { PropType } from "vue";
import { defineComponent } from "vue";
import { SvgSprite } from "vue-svg-sprite";

export default defineComponent({
  components: {
    SvgSprite,
  },
  props: {
    symbol: {
      type: String,
      default: "3d-coordinate-axis-35",
    },
    size: {
      type: String,
      default: "24",
    },
    type: {
      type: String as PropType<"icon" | "flag">,
      default: "icon",
    },
    dynamicUrl: {
      type: String,
    },
  },
  computed: {
    url() {
      if (this.dynamicUrl) {
        return this.dynamicUrl;
      }

      switch (this.type) {
        case "icon":
        default:
          return `${import.meta.env.VITE_LANDING_URL}/sprite/sprite-icons.svg`;
        case "flag":
          return `${import.meta.env.VITE_LANDING_URL}/sprite/sprite-flags.svg`;
      }
    },
    calculatedSize() {
      // Calculate flag size based on the original dimension (32x24)
      if (this.type === "flag") {
        const height = parseFloat(this.size) * (24 / 32);
        return `0 0 ${this.size} ${height}`;
      }

      return this.size;
    },
  },
});
</script>

<template>
  <SvgSprite :symbol="symbol" :size="calculatedSize" :url="url" class="Icon" />
</template>