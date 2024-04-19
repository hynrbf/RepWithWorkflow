<script lang="ts">
import { defineComponent } from "vue";
import IconComponent from "@/components/IconComponent.vue";
import { Button } from "@progress/kendo-vue-buttons";
import { StackLayout } from "@progress/kendo-vue-layout";

export interface KendoConfirmDialogComponent {
  showConfirmDialogMessage(
    title?: string,
    text?: string,
    selectedItem?: string,
  ): void;
}

export default defineComponent({
  name: "KendoConfirmDialog",
  components: { StackLayout, IconComponent, Button },
  data() {
    return {
      isVisibleInternal: false,
      title: "Confirm",
      text: "Are you sure you want to delete this <Trading Address>?",
      selectedItem: "",
    };
  },
  methods: {
    showConfirmDialogMessage(
      title?: string,
      text?: string,
      selectedItem?: string,
    ) {
      if (title) {
        this.title = title;
      }

      if (text) {
        this.text = text;
      }

      if (selectedItem) {
        this.selectedItem = selectedItem;
      }

      this.isVisibleInternal = true;
    },

    close() {
      this.isVisibleInternal = false;
    },

    onConfirm() {
      this.$emit("onConfirm", this.selectedItem);
      this.close();
    },
  },
});
</script>

<template>
  <Dialog class="app-dialog" v-if="isVisibleInternal" :theme-color="'primary'">
    <StackLayout
      orientation="horizontal"
      style="padding: 20px 10px 0 10px"
      :align="{ horizontal: 'start', vertical: 'middle' }"
    >
      <Label class="title flex-grow-1">{{ title }}</Label>

      <IconComponent style="cursor: pointer" :symbol="'close'" @click="close" />
    </StackLayout>

    <StackLayout
      class="alert-body"
      orientation="vertical"
      :gap="20"
      :align="{ horizontal: 'start', vertical: 'middle' }"
    >
      <Label class="paragraph-m-figtree-medium">{{ text }}</Label>

      <Button class="danger-btn align-self-center" @click="onConfirm">
        Confirm & Delete
      </Button>
    </StackLayout>
  </Dialog>
</template>

<style scoped>
.alert-body {
  min-width: 350px;
  padding: 20px 0 20px 10px;
}

.danger-btn {
  display: flex;
  padding: 12px 20px;
  flex-direction: column;
  align-items: center;
  border-radius: 100px;
  color: white;
  background: var(--color-error-600);
}

.title {
  color: var(--text-text-primary, #000);
  text-align: start;
  font-size: 20px;
  font-style: normal;
  font-weight: 700;
  line-height: 28px; /* 140% */
}
</style>