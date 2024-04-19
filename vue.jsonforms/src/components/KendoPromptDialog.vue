<script lang="ts">
import { defineComponent } from "vue";
import { Button } from "@progress/kendo-vue-buttons";
import { StackLayout } from "@progress/kendo-vue-layout";
import IconComponent from "@/components/IconComponent.vue";

export interface KendoPromptDialogComponent {
  setActionDialogMessage(title?: string): void;

  closeActionDialog(): void;
}

export default defineComponent({
  name: "KendoPromptDialog",
  props: {
    primaryActionButtonLabel: {
      type: String,
      default: "",
    },
    dangerActionButtonLabel: {
      type: String,
      default: "",
    },
    primaryActionButtonIcon: {
      type: String,
      default: "",
    },
    dangerActionButtonIcon: {
      type: String,
      default: "",
    },
    isPromptError: {
      type: Boolean,
      default: false,
    },
  },
  components: { IconComponent, StackLayout, Button },
  data() {
    return {
      isVisibleInternal: false,
      titleInternal: "",
      primaryActionButtonLabelInternal: "",
      dangerActionButtonLabelInternal: "",
    };
  },
  created() {
    this.primaryActionButtonLabelInternal = this.primaryActionButtonLabel;
    this.dangerActionButtonLabelInternal = this.dangerActionButtonLabel;
  },
  methods: {
    closeActionDialog() {
      this.isVisibleInternal = false;
    },
    primaryAction() {
      this.$emit("onPrimaryActionClick");
    },
    dangerAction() {
      this.$emit("onDangerActionClick");
    },
    setActionDialogMessage(title: string) {
      if (title) {
        this.titleInternal = title;
      }

      if (!this.isVisibleInternal) {
        this.isVisibleInternal = true;
      }
    },
  },
});
</script>

<template>
  <Dialog class="app-dialog" v-if="isVisibleInternal" :theme-color="'primary'">
    <StackLayout orientation="vertical" class="mt-2 slot">
      <StackLayout
        class="mb-4 title-section"
        orientation="horizontal"
        :align="{ horizontal: 'start' }"
      >
        <Label
          class="flex-grow-1 subtitle-figtree-bold"
          style="text-align: start"
          :style="
            isPromptError ? 'color: var(--error-color-error-primary)' : ''
          "
        >
          {{ titleInternal }}
        </Label>

        <IconComponent
          symbol="close"
          style="cursor: pointer"
          @click="closeActionDialog"
        />
      </StackLayout>

      <slot></slot>
    </StackLayout>

    <div class="button-container">
      <Button
        v-if="primaryActionButtonLabelInternal"
        class="primary-btn"
        type="button"
        size="large"
        :image-url="primaryActionButtonIcon"
        @click="primaryAction"
        >{{ primaryActionButtonLabelInternal }}
      </Button>

      <Button
        v-if="dangerActionButtonLabelInternal"
        class="danger-btn"
        type="button"
        :image-url="dangerActionButtonIcon"
        @click="dangerAction"
        >{{ dangerActionButtonLabelInternal }}
      </Button>
    </div>
  </Dialog>
</template>

<style scoped>
.button-container * {
  border-radius: 100px;
  padding: 12px 20px;
  min-width: 100px;
  color: white;
}

.primary-btn {
  background: var(--brand-color-brand-primary);
  gap: 10px;
}

.danger-btn {
  background: var(--error-color-error-primary);
}

.button-container {
  display: flex;
  justify-content: center;
  gap: 20px;
  margin-bottom: 20px;
}

.slot {
  padding: 20px 20px 0px 20px;
  color: var(--text-text-primary);
  font-size: var(--font-size-default);
  font-style: normal;
  font-weight: var(--font-weight-normal);
  line-height: 130%; /* 20.8px */
}

.title-section {
  color: black;
}
</style>
