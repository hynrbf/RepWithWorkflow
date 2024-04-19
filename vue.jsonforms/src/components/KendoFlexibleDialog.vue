<script lang="ts">
import { defineComponent } from "vue";

export interface KendoFlexibleDialogComponent {
  showMessageAndContent(message: string, title?: string): void;

  closeActionDialog(): void;
}

export default defineComponent({
  name: "KendoFlexibleDialog",
  props: {
    //the popup dialog is default to 90% of your screen height,
    //but when you have few items in the popup you can pass height auto as a whole style object
    //e.g. :style="{height: 'auto', overflow: 'auto', padding: '1.143em', margin: '-1.143em'}"
    style: {
      type: Object,
      default: {
        height: "90vh",
        overflow: "auto",
        padding: "1.143em",
        margin: "-1.143em",
      },
    },
    isWithParagraphDialogBody: {
      type: Boolean,
      default: true,
    },
  },
  data() {
    return {
      isVisibleInternal: false,
      titleInternal: "",
      messageInternal: "",
    };
  },
  methods: {
    closeActionDialog() {
      this.isVisibleInternal = false;
    },

    showMessageAndContent(message: string, title: string = "") {
      if (title) {
        this.titleInternal = title;
      }

      if (message) {
        this.messageInternal = message;
      }

      if (!this.isVisibleInternal) {
        this.isVisibleInternal = true;
      }
    },
  },
});
</script>
<!-- TODO: need to revisit the @close to @click code because the code is applicable to some but breaks on other pages. 
     Reverting to @close for now because it is needed in Introducers page
-->
<template>
  <Dialog
    v-if="isVisibleInternal"
    :title="titleInternal"
    @close="closeActionDialog"
    :theme-color="'primary'"
  >
    <p v-if="isWithParagraphDialogBody" class="kendo-dialog-body">
      {{ messageInternal }}
    </p>

    <div v-else>
      {{ messageInternal }}
    </div>

    <div :style="style">
      <slot></slot>
    </div>
  </Dialog>
</template>

<style />