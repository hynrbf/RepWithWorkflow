<script setup lang="ts">
import { FinancialPromotion } from "@/entities/financial-promotion/FinancialPromotion";
import { FinancialPromotionStatus } from "@/entities/financial-promotion/FinancialPromotionStatus";

withDefaults(
  defineProps<{ promotion: FinancialPromotion; disabled?: boolean }>(),
  {}
);

const emits = defineEmits<{
  (event: "view", id: string): void;
  (event: "remove", id: string): void;
  (event: "approve", id: string): void;
  (event: "reject", id: string): void;
}>();
</script>

<template>
  <div class="d-inline-flex gap-2">
    <KendoButton
      type="button"
      size="small"
      rounded="full"
      shape="square"
      theme-color="light"
      title="View"
      class="ActionButton"
      :disabled="disabled"
      @click="emits('view', `${promotion.id}`)"
    >
      <IconComponent symbol="eye" size="20" />
    </KendoButton>
    <KendoButton
      v-if="promotion.approvalStatus === FinancialPromotionStatus.Pending"
      type="button"
      size="small"
      rounded="full"
      shape="square"
      theme-color="success"
      title="View"
      class="ActionButton"
      :disabled="disabled"
      @click="emits('approve', `${promotion.id}`)"
    >
      <IconComponent symbol="check-big" size="12" />
    </KendoButton>
    <KendoButton
      v-if="promotion.approvalStatus === FinancialPromotionStatus.Pending"
      type="button"
      size="small"
      rounded="full"
      shape="square"
      theme-color="error"
      title="View"
      class="ActionButton"
      :disabled="disabled"
      @click="emits('reject', `${promotion.id}`)"
    >
      <IconComponent symbol="clear" size="12" />
    </KendoButton>
    <KendoButton
      v-dev-only
      type="button"
      size="small"
      rounded="full"
      shape="square"
      theme-color="light"
      title="Remove"
      class="ActionButton"
      :disabled="disabled"
      @click="emits('remove', `${promotion.id}`)"
    >
      <IconComponent symbol="trash-bin-remove-6" size="20" />
    </KendoButton>
  </div>
</template>

<style scoped lang="scss">
.ActionButton {
  width: 30px;
  height: 30px;

  &.k-button-solid-light {
    color: var(--color-primary);
  }
}
</style>
