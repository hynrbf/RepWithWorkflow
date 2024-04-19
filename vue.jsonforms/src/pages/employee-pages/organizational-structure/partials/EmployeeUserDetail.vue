<script lang="ts">
import { defineComponent } from "vue";
import DynamicAvatar from "@/components/DynamicAvatarComponent.vue";
import { Employee } from "@/entities/firm-details/Employee";
import EmployeeRoleDetails from "@/pages/employee-pages/organizational-structure/partials/EmployeeRoleDetails.vue";

export default defineComponent({
  name: "EmployeeUserDetail",
  components: {
    EmployeeRoleDetails,
    DynamicAvatar,
  },
  props: {
    employee: {
      type: Object as () => Employee,
      default: new Employee(),
    },
    dialogWidth: {
      type: Number,
      default: 330,
    },
  },

  methods: {
    getStatusPillColor(employmentStatus: string | undefined): string {
      if (!employmentStatus) {
        return "";
      }

      if (employmentStatus === "Active") {
        return "good-status-pill";
      }

      if (employmentStatus === "Onboarding") {
        return "warning-status-pill";
      }

      // If 'Suspended' or 'Terminated'
      return "bad-status-pill";
    },
  },
});
</script>

<template>
  <StackLayout
    orientation="vertical"
    style="margin-bottom: 5px"
    :gap="10"
    :align="{ horizontal: 'center', vertical: 'top' }"
  >
    <div>
      <DynamicAvatar
        size="large"
        type="image"
        rounded="full"
        :customStyle="{
          flexBasis: '60px',
          height: '60px',
        }"
        :imageSrc="employee?.img_id"
        :imageAlt="employee?.img_id"
      />
    </div>

    <div class="d-flex align-items-center">
      <Label class="detail-dialog-employee-name">
        {{ employee.firstName }} {{ employee.lastName }}
      </Label>

      <KendoButton
        style="height: 25px; width: 25px; margin-left: 10px; margin-top: 1px"
        type="button"
        size="small"
        rounded="full"
        shape="square"
        theme-color="light"
        title="View"
        class="ActionButton"
      >
        <IconComponent
          symbol="edit-pen"
          size="20"
          color="var(--brand-color-brand-primary)"
        />
      </KendoButton>
    </div>

    <PillComponent :class="getStatusPillColor(employee.employmentStatus)">
      {{ employee.employmentStatus }}
    </PillComponent>
  </StackLayout>

  <DividerComponent
    class="my-2"
    :height="1"
    :width="dialogWidth - 60"
    :spacing="0"
  />

  <!-- Full name -->
  <div class="d-flex my-2">
    <div class="col d-flex flex-column gap-2">
      <Label class="detail-dialog-section-text">Title</Label>

      <Label class="detail-dialog-text-primary">{{ employee.title }}</Label>
    </div>

    <div class="col d-flex flex-column gap-2">
      <Label class="detail-dialog-section-text">Forename(s)</Label>

      <Label class="detail-dialog-text-primary">{{ employee.firstName }}</Label>
    </div>

    <div class="col d-flex flex-column gap-2">
      <Label class="detail-dialog-section-text">Surname</Label>

      <Label class="detail-dialog-text-primary">{{ employee.lastName }}</Label>
    </div>
  </div>

  <DividerComponent
    v-if="employee.lineManager"
    class="my-2"
    :height="1"
    :width="dialogWidth - 60"
    :spacing="0"
  />

  <!-- Line manager -->
  <div v-if="employee.lineManager" class="d-flex flex-column my-2">
    <text class="detail-dialog-section-text">Line Manager</text>

    <div class="d-flex" style="margin-top: 5px">
      <DynamicAvatar
        type="image"
        rounded="full"
        size="medium"
        :imageSrc="employee.lineManager.img_id"
        :imageAlt="employee.lineManager.img_id"
      />

      <text class="detail-dialog-text-primary" style="margin-top: 8px">
        {{ employee.lineManager.firstName }}
        {{ employee.lineManager.lastName }}
      </text>
    </div>
  </div>

  <DividerComponent
    class="my-2"
    :height="1"
    :width="dialogWidth - 60"
    :spacing="0"
  />

  <EmployeeRoleDetails :employee="employee" :dialogWidth="dialogWidth" />

  <!-- Product Type -->
  <div class="vstack my-3" style="gap: 5px">
    <Label class="detail-dialog-section-text">Product Type</Label>

    <div v-for="product of employee.productType" :key="product.value">
      <Label class="detail-dialog-text-primary">{{ product.value }}</Label>
    </div>
  </div>

  <DividerComponent
    class="my-2"
    :height="1"
    :width="dialogWidth - 60"
    :spacing="0"
  />

  <!-- Email Address, Contact Number -->
  <div class="d-flex mt-2">
    <div class="col vstack">
      <Label class="detail-dialog-section-text">Email Address</Label>

      <Label class="detail-dialog-text-primary">{{
          employee.email
        }}</Label>
    </div>

    <div class="col vstack">
      <Label class="detail-dialog-section-text">Contact Number</Label>

      <Label class="detail-dialog-text-primary">
        {{ employee.contactNumber.dialCode }}
        {{ employee.contactNumber.number }}
      </Label>
    </div>
  </div>
</template>

<style scoped></style>