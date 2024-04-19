<script setup lang="ts">
import { toRefs, computed, ref, watchEffect } from "vue";
import { PermissionGroup } from "@/entities/PermissionGroup";
import { AppConstants } from "@/infra/AppConstants";
import { PermissionResult } from "@/entities/PermissionResult";
import { CustomerPermission } from "@/entities/CustomerPermission";
import { PermissionStateEnum } from "@/entities/enums/PermissionStateEnum";

const props = withDefaults(
  defineProps<{
    permissionGroups: PermissionGroup[];
    permissionsFromFca: PermissionResult;
    customerPermissions: CustomerPermission[];
    authorised?: boolean;
  }>(),
  {
    permissionGroups: () => [],
    customerPermissions: () => [],
  },
);

const emits = defineEmits<{
  (event: "change", action: string, checked: boolean, value: string): void;
}>();

const {
  permissionGroups,
  permissionsFromFca,
  customerPermissions,
  authorised,
} = toRefs(props);

const permissionNamesFromFca = computed(
  () => permissionsFromFca.value?.permissionNames || [],
);

const permissionNamesFromFcaRaw = computed<Record<string, unknown> | null>(
  () => {
    try {
      return JSON.parse(`${permissionsFromFca.value.raw}`);
    } catch {
      return null;
    }
  },
);

const columns = ref([
  {
    field: "name",
    title: " ",
  },
  {
    field: "fcaAuthorised",
    title: "FCA Authorised",
  },
  {
    field: "add",
    title: "Add",
  },
  {
    field: "pending",
    title: "Pending",
  },
]);

watchEffect(() => {
  if (authorised.value) {
    columns.value.push({
      field: "remove",
      title: "Remove",
    });
  } else {
    columns.value = columns.value.filter(({ field }) => field !== "remove");
  }
});

const data = computed(() =>
  permissionGroups.value.map((item) => ({
    title: item.permissionGroupName,
    items: item.subPermissions.map(({ name, displayText, categoryName }) => ({
      name: displayText || name || "",
      permissionName: name,
      categoryName,
    })),
  })),
);

const isAuthorised = (
  permissionName: string,
  categoryName: string,
): boolean => {
  if (
    !permissionNamesFromFca.value.length ||
    !permissionName ||
    !categoryName
  ) {
    return false;
  }

  //for 'Insurance Broker' category only
  if (categoryName === AppConstants.PermissionCategoryInsuranceBroker) {
    // check if it has parenthesis
    const regex = /\[([^\]]+)\]$/; // Matches the text within the last square bracket
    const matches = regex.exec(permissionName);

    if (matches && matches.length > 0) {
      if (!permissionNamesFromFcaRaw.value) {
        return false;
      }

      let trimmedName = permissionName.replace(matches[0], "").trim();
      let searchText = matches[1]; // Get text Inside square bracket
      let isAuthorised = false;

      Object.keys(permissionNamesFromFcaRaw.value).forEach((key) => {
        if (key.toLowerCase() === trimmedName?.toLowerCase()) {
          const value = permissionNamesFromFcaRaw?.value?.[key];
          isAuthorised = recursivelyCheckValue(value, searchText);
        }
      });

      return isAuthorised;
    }
  }

  //logic for NON 'Insurance Broker'
  return checkIfPermissionListed(permissionName);
};

const recursivelyCheckValue = (obj: any, targetValue: any): boolean => {
  if (!obj) {
    return false;
  }

  if (typeof obj !== "object") {
    return obj === targetValue;
  }

  //handling object that is an array
  if (Array.isArray(obj)) {
    for (const element of obj) {
      if (recursivelyCheckValue(element, targetValue)) {
        return true;
      }
    }
  }

  //handling object that has children with array of keys and values
  if (typeof obj === "object") {
    for (const key in obj) {
      if (recursivelyCheckValue(obj[key], targetValue)) {
        return true;
      }
    }
  }

  return false;
};

const checkIfPermissionListed = (permissionName: string) => {
  //logic for NON 'Insurance Broker'
  let isListed = permissionNamesFromFca.value.find(
    (name) => name.toLowerCase() === permissionName.toLowerCase(),
  );

  if (isListed && isListed.trim().length > 0) {
    return true;
  }

  return false;
};

const isAddDisabled = (
  permissionName: string,
  categoryName: string,
): boolean => {
  if (isAuthorised(permissionName, categoryName)) {
    // if subPermission is 'Authorised', add button is disabled.
    return true;
  }

  const permission = customerPermissions.value.find(
    ({ subPermissionName }) => subPermissionName === permissionName,
  );

  if (!permission) {
    return false;
  }

  return permission.hasPendingApplication;
};

const isRemoveDisabled = (
  permissionName: string,
  categoryName: string,
): boolean => {
  return !isAuthorised(permissionName, categoryName);
};

const isPendingDisabled = (
  permissionName: string,
  categoryName: string,
): boolean => {
  if (isAuthorised(permissionName, categoryName)) {
    // if subPermission is 'Authorised', application pending button is disabled.
    return true;
  }

  const permission = customerPermissions.value.find(
    ({ subPermissionName }) => subPermissionName === permissionName,
  );

  if (!permission) {
    return false;
  }

  return (
    permission.isModified && permission.state === PermissionStateEnum.Added
  );
};

const isPermissionModified = (
  permissionName: string,
  _categoryName: string,
): boolean => {
  const permission = customerPermissions.value.find(
    ({ subPermissionName }) => subPermissionName === permissionName,
  );

  if (!permission) {
    return false;
  }

  return permission.isModified;
};

const isChecked = (state: PermissionStateEnum, permissionName: string) => {
  const permission = customerPermissions.value.find(
    ({ subPermissionName }) => subPermissionName === permissionName,
  );

  if (!permission) {
    return false;
  }

  if (permission.state === state && permission.isModified) {
    return true;
  }

  return false;
};
</script>

<template>
  <InfoTableComponent class="PermissionsTable" :columns="columns" :data="data">
    <template #cell-name="{ item: { name, permissionName, categoryName } }">
      <span
        :class="[
          isPermissionModified(permissionName, categoryName) && 'is-edited',
        ]"
        >{{ name }}</span
      >
    </template>
    <template #cell-fcaAuthorised="{ item: { permissionName, categoryName } }">
      <img
        v-if="isAuthorised(permissionName, categoryName)"
        src="/approved-icon.svg"
        alt="Authorised"
        title="Authorised"
        width="16"
        height="16"
      />
      <img
        v-else
        src="/rejected-grey-icon.svg"
        alt="Non-Authorised"
        title="Non-Authorised"
        width="16"
        height="16"
      />
    </template>
    <template #cell-add="{ item: { permissionName, categoryName } }">
      <KendoCheckbox
        class="Checkbox Checkbox--plus"
        :checked="isChecked(PermissionStateEnum.Added, permissionName)"
        :disabled="isAddDisabled(permissionName, categoryName)"
        @change="
          emits(
            'change',
            AppConstants.seekAuthAdd,
            $event.value,
            permissionName,
          )
        "
      />
    </template>
    <template #cell-pending="{ item: { permissionName, categoryName } }">
      <KendoCheckbox
        class="Checkbox"
        :checked="isChecked(PermissionStateEnum.Pending, permissionName)"
        :disabled="isPendingDisabled(permissionName, categoryName)"
        @change="
          emits(
            'change',
            AppConstants.seekAuthPending,
            $event.value,
            permissionName,
          )
        "
      />
    </template>
    <template #cell-remove="{ item: { permissionName, categoryName } }">
      <KendoCheckbox
        class="Checkbox Checkbox--minus"
        :checked="isChecked(PermissionStateEnum.Removed, permissionName)"
        :disabled="isRemoveDisabled(permissionName, categoryName)"
        @change="
          emits(
            'change',
            AppConstants.seekAuthRemove,
            $event.value,
            permissionName,
          )
        "
      />
    </template>
  </InfoTableComponent>
</template>

<style scoped lang="scss">
.PermissionsTable {
  :deep(.InfoTable-row) {
    .InfoTable-cell:not(:nth-child(1)) {
      max-width: 150px;
      flex: 0 0 150px;
      text-align: center;
    }
  }

  .is-edited {
    // ToDo. Get from scss
    color: #cc8800;
  }
}
</style>