<script lang="ts">
import { defineComponent, inject } from "vue";
import { Dialog, DialogActionsBar } from "@progress/kendo-vue-dialogs";
import { Button } from "@progress/kendo-vue-buttons";
import { Emitter, EventType } from "mitt";

export interface KendoActionDialogComponent {
  setActionDialogMessage(message: string, title?: string): void;

  closeActionDialog(): void;
}

export default defineComponent({
  name: "KendoActionDialog",
  components: { Dialog, DialogActionsBar, Button },
  props: {
    leftActionButtonLabel: {
      type: String,
      default: "",
    },
    rightActionButtonLabel: {
      type: String,
      default: "",
    },
  },
  data() {
    return {
      isVisibleInternal: false,
      titleInternal: "",
      leftActionButtonLabelInternal: "",
      rightActionButtonLabelInternal: "",
      messageInternal: "Company does not exist!",
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
    };
  },
  created() {
    this.leftActionButtonLabelInternal = this.leftActionButtonLabel;
    this.rightActionButtonLabelInternal = this.rightActionButtonLabel;
  },
  methods: {
    closeActionDialog() {
      this.isVisibleInternal = false;
    },

    proceedAsSoleTrader() {
      this.$emit("onLeftButtonClicked");
    },

    proceedAnyway() {
      this.$emit("onRightButtonClicked");
    },

    setActionDialogMessage(
      message: string,
      title: string = "Company Not Found!",
    ) {
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

<template>
  <Dialog
    v-if="isVisibleInternal"
    :title="titleInternal"
    @close="closeActionDialog"
    :theme-color="'primary'"
  >
    <p class="kendo-dialog-body">
      {{ messageInternal }}
    </p>

    <DialogActionsBar>
      <Button
        v-if="leftActionButtonLabelInternal"
        type="button"
        @click="proceedAsSoleTrader"
        >{{ leftActionButtonLabelInternal }}
      </Button>

      <Button
        v-if="rightActionButtonLabelInternal"
        type="button"
        @click="proceedAnyway"
        >{{ rightActionButtonLabelInternal }}
      </Button>
    </DialogActionsBar>
  </Dialog>
</template>

<style />