<script lang="ts">
import { defineComponent } from "vue";
import { Employee } from "@/entities/firm-details/Employee";
import Role from "@/entities/org-structure/Role";

export default defineComponent({
  name: "RoleDetails",
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
    getPrimaryRoleStyleClass(primaryRole: Role): string {
      if (!primaryRole) {
        return "";
      }

      if (primaryRole.isModified) {
        return "modified";
      }

      return primaryRole.isFcaAuthorised ? "authorised" : "";
    },
  },
});
</script>

<template>
  <StackLayout>
    <StackLayout orientation="vertical" :gap="10">
      <Label class="detail-dialog-section-text mt-2">Primary Role</Label>

      <Label
        v-if="employee.primaryRole"
        class="detail-dialog-text"
        :class="getPrimaryRoleStyleClass(employee.primaryRole)"
      >
        {{ employee.primaryRole.name }}
      </Label>

      <div v-if="employee.otherRoles?.length">
        <!--
          :width="detailDialogWidth - 60" since modal has 30px left and right padding by default
         -->
        <DividerComponent
          class="my-1"
          :height="1"
          :width="dialogWidth - 60"
          :spacing="0"
        />

        <Label class="detail-dialog-section-text">Other Role(s)</Label>

        <div v-for="role of employee.otherRoles" class="my-2" :key="role.name">
          <Label
            v-if="role"
            class="detail-dialog-text"
            :class="getPrimaryRoleStyleClass(role)"
          >
            {{ role.name }}
          </Label>
        </div>
      </div>
    </StackLayout>
  </StackLayout>
</template>

<style scoped>
.detail-dialog-section-text {
  color: var(--color-black);
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-semi-bold);
  line-height: 16px;
}

.detail-dialog-text {
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-normal);
  line-height: 17.5px;
}
</style>
