<script setup lang="ts">
import { toRefs, defineAsyncComponent, computed } from "vue";
import StaticList from "@/infra/StaticListService";
import { useArCustomerStore } from "@/stores/useArCustomerStore";
import { AppConstants } from "@/infra/AppConstants";

const MediaDropdown = defineAsyncComponent(() => import("./ARMediaDropdown.vue"));

const props = withDefaults(
  defineProps<{
    form: { values: Record<string, unknown> };
    type: "pending" | "approved" | "rejected" | "all";
  }>(),
  {
    form: () => ({ values: {} }),
    type: "all",
  }
);

const { form } = toRefs(props);

const { currentArFirmName } = useArCustomerStore();

const approvalTypeOptions = computed(() =>
  StaticList.getPostApprovalTypes().map((type) => ({
    label: type,
    value: type,
  }))
);

const contentStatusOptions = computed(() => [
  {
    label: "Live",
    value: "live",
  },
  {
    label: "Not Live",
    value: "not live",
  },
]);

const promotionTypeOptions = computed(() => [
  {
    label: `${currentArFirmName}`,
    value: "self",
  },
  {
    label: "Authorised Third Party",
    value: "authorised-3rd-party",
  },
  {
    label: "Non-Authorised Third Party",
    value: "non-authorised-3rd-party",
  },
]);

const setUniqueIdentifier = (value: string): string => {
  const identifier = `${AppConstants.arMarketingAndFinancialPromotionsRoute}-promotionTableFilters${value}`;
  return identifier.replace(/\s+/g, "").replace("/", "");
};
</script>

<template>
  <div>
    <MediaDropdown
        :id="setUniqueIdentifier('-mediaOutlet')"
        name="platform"
        label="Media Outlet"
        :value="form.values.platform"
        multiple
        class="mb-3"
    />

    <KendoMultiSelectTreeComponent
      name="promotionType"
      :id="setUniqueIdentifier('-promotionType')"
      label="Type"
      :data-items="promotionTypeOptions"
      :value="form.values.type"
      value-primitive
      class="mb-3"
    />

    <template v-if="type === 'pending'">
      <KendoMultiSelectTreeComponent
        name="approvalType"
        :id="setUniqueIdentifier('-approvalType')"
        label="Approval Type"
        :data-items="approvalTypeOptions"
        :value="form.values.approvalType"
        class="mb-3"
      />

      <KendoMultiSelectTreeComponent
        name="contentStatus"
        :id="setUniqueIdentifier('-contentStatus')"
        label="Content Status"
        :data-items="contentStatusOptions"
        :value="form.values.contentStatus"
        class="mb-3"
      />

      <KendoDatePickerInputComponent
        name="approvalDateRequired"
        :id="setUniqueIdentifier('-approvalDateRequired')"
        label="Approval Date Required"
        :value="form.values.approvalDateRequired"
        class="mb-3"
      />
    </template>

    <template v-if="type === 'approved'">
      <KendoMultiSelectTreeComponent
        name="contentStatusApproved"
        :id="setUniqueIdentifier('-contentStatusApproved')"
        label="Content Status"
        :data-items="contentStatusOptions"
        :value="form.values.contentStatus"
        class="mb-3"
      />

      <KendoDatePickerInputComponent
        name="dateApproved"
        :id="setUniqueIdentifier('-dateApproved')"
        label="Date Approved"
        :value="form.values.dateApproved"
        class="mb-3"
      />

      <KendoDatePickerInputComponent
        name="dateLive"
        :id="setUniqueIdentifier('-dateLive')"
        label="Date Live"
        :value="form.values.dateLive"
        class="mb-3"
      />
    </template>

    <template v-if="type === 'rejected'">
      <KendoMultiSelectTreeComponent
        name="contentStatusRejected"
        :id="setUniqueIdentifier('-contentStatusRejected')"
        label="Content Status"
        :data-items="contentStatusOptions"
        :value="form.values.contentStatus"
        class="mb-3"
      />

      <KendoDatePickerInputComponent
        name="dateRejected"
        :id="setUniqueIdentifier('-dateRejected')"
        label="Date Rejected"
        :value="form.values.dateRejected"
        class="mb-3"
      />

      <KendoDatePickerInputComponent
        name="dateLive"
        :id="setUniqueIdentifier('-dateLive')"
        label="Date Live"
        :value="form.values.dateLive"
        class="mb-3"
      />
    </template>

    <template v-if="type === 'all'">
      <KendoMultiSelectTreeComponent
        name="contentStatusAll"
        :id="setUniqueIdentifier('-contentStatusAll')"
        label="Content Status"
        :data-items="contentStatusOptions"
        :value="form.values.contentStatus"
        class="mb-3"
      />

      <KendoDatePickerInputComponent
        name="approvalRequiredRejected"
        :id="setUniqueIdentifier('-approvalRequiredRejected')"
        label="Approval Required Rejected"
        :value="form.values.approvalRequiredRejected"
        class="mb-3"
      />

      <KendoDatePickerInputComponent
        name="dateApproved"
        :id="setUniqueIdentifier('-dateApproved')"
        label="Date Approved"
        :value="form.values.dateApproved"
        class="mb-3"
      />

      <KendoDatePickerInputComponent
        name="dateRejected"
        :id="setUniqueIdentifier('-dateRejected')"
        label="Date Rejected"
        :value="form.values.dateRejected"
        class="mb-3"
      />

      <KendoDatePickerInputComponent
        name="dateLive"
        :id="setUniqueIdentifier('-dateLive')"
        label="Date Live"
        :value="form.values.dateLive"
        class="mb-3"
      />
    </template>
  </div>
</template>
