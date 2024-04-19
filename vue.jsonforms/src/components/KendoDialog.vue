<script lang="ts">
import { defineComponent } from "vue";
import { Dialog, DialogCloseEvent } from "@progress/kendo-vue-dialogs";

export interface KendoDialogComponent {
  setDialogMessage(message: string, title?: string): void;
}

export default defineComponent({
  name: "KendoDialog",
  props: {
    isVisible: {
      type: Boolean,
      default: false,
    },
    title: {
      type: String,
      default: "Error",
    },
    message: {
      type: String,
      default: "",
    },
  },
  components: { Dialog },
  data() {
    return {
      isVisibleInternal: false,
      titleInternal: "",
      messageInternal: "",
    };
  },
  created() {
    this.isVisibleInternal = this.isVisible;
    this.titleInternal = this.title;
    this.messageInternal = this.message;
  },
  methods: {
    onDialogClose(_event: DialogCloseEvent) {
      this.isVisibleInternal = false;
      this.titleInternal = "";
      this.messageInternal = "";
    },

    setDialogMessage(message: string, title: string = "Error") {
      if (title) {
        this.titleInternal = title;
      }

      // check and remove duplicate error message
      const errors = message.split("\n");
      const uniqueError: string[] = [...new Set(errors)];
      message = uniqueError
        .filter((item) => item.trim() !== "")
        .sort()
        .join("\n");

      if (message) {
        // Replace '\n' to '<br />' if any
        message = message.replaceAll("\n", "<br />");
        this.messageInternal = message;
      }

      if (!this.isVisibleInternal) {
        this.isVisibleInternal = true;
      }
    },
  },
});
</script>

<template>
  <div v-if="isVisibleInternal">
    <Dialog
      :title="titleInternal"
      min-width="250"
      @close="onDialogClose"
      :theme-color="'primary'"
    >
      <div class="kendo-dialog-body" v-html="messageInternal" />
    </Dialog>
  </div>
</template>

<style scoped />