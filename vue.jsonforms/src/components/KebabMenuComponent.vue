<script lang="ts">
import { MenuItemModel } from "@progress/kendo-vue-layout";
import { PropType, defineComponent } from "vue";

export default defineComponent({
  props: {
    items: {
      type: Array as PropType<MenuItemModel[]>,
      default: [],
    },
    referenceId: [String, Number],
  },
  data() {
    return {
      show: false,
    };
  },
  methods: {
    select(event: any) {
      this.show = false;
      if (this.referenceId) event.referenceId = this.referenceId;
      this.$emit("select", event);
    },
  },
});
</script>

<template>
  <div>
    <KendoButton
      ref="button"
      type="button"
      fill-mode="flat"
      @click.prevent="show = !show"
    >
      <span class="k-icon k-i-more-vertical" />
    </KendoButton>
    <KendoPopup
      anchor="button"
      :show="show"
      :anchor-align="{ horizontal: 'right', vertical: 'bottom' }"
      :popup-align="{ horizontal: 'right', vertical: 'top' }"
    >
      <KendoMenu
        :items="items"
        vertical
        :style="{ display: 'inline-block' }"
        @select="select"
      />
    </KendoPopup>
  </div>
</template>