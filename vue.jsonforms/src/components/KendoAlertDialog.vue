<script lang="ts">
import { defineComponent } from "vue";
import { Button } from "@progress/kendo-vue-buttons";
import IconComponent from "@/components/IconComponent.vue";

export interface KendoAlertDialogComponent {
  showAlertDialogMessage(text?: string): void;
}

export default defineComponent({
  name: "KendoAlertDialog",
  components: { IconComponent, Button },
  props: {
    hasIcon: {
      type: Boolean,
      default: true,
    },
  },
  data() {
    return {
      isVisibleInternal: false,
      text: "",
    };
  },
  methods: {
    showAlertDialogMessage(text?: string) {
      if (text) {
        this.text = text;
      }

      this.isVisibleInternal = true;
    },

    close() {
      this.isVisibleInternal = false;
    },
  },
});
</script>

<template>
  <Dialog
    class="app-dialog"
    v-if="isVisibleInternal"
    @click="close"
    :theme-color="'primary'"
  >
    <StackLayout
      class="my-4 alert-body"
      orientation="vertical"
      :align="{ horizontal: 'center', vertical: 'middle' }"
    >
      <img
        v-if="hasIcon"
        src="/check-filled.svg"
        alt="check-filled.svg"
        style="height: 35px; margin-bottom: 20px"
      />

      <Label class="paragraph-m-figtree-medium">{{ text }}</Label>
    </StackLayout>
  </Dialog>
</template>

<style scoped>
.alert-body {
  min-width: 350px;
  padding: 20px;
}
</style>