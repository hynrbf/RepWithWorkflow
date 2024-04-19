<script lang="ts">
import { defineComponent } from "vue";
import { Button } from "@progress/kendo-vue-buttons";
import { Label } from "@progress/kendo-vue-labels";
import { PermissionGroup } from "../entities/PermissionGroup";
import { CustomerPermission } from "../entities/CustomerPermission";
import { PermissionStateEnum } from "../entities/enums/PermissionStateEnum";
import { AppConstants } from "../infra/AppConstants";
import { SubPermission } from "../entities/SubPermission";
import { PermissionResult } from "../entities/PermissionResult";

export default defineComponent({
  name: "PermissionGroupItemTemplate",
  props: {
    permissionGroup: {
      type: PermissionGroup,
      default: undefined,
    },
    subPermissionsFromFca: {
      type: Array,
      default: [] as string[],
    },
    customerSubPermissions: {
      type: Array,
      default: [] as CustomerPermission[],
    },
    fcaPermissions: {
      type: PermissionResult,
      default: undefined,
    },
  },
  components: { Button, Label },
  data() {
    return {
      permissionGroupInternal: null as PermissionGroup | null,
      subPermissionNamesFromFcaInternal: [] as string[],
      customerSubPermissionsInternal: [] as CustomerPermission[],
      rawFcaPermissionsInternal: undefined,
      enabledButtonClass: "enabled-btn",
      selectedButtonClass: "selected-btn",
      notAuthorised: AppConstants.notAuthorised,
    };
  },
  computed: {
    hasNoSubPermissions(): boolean {
      if (!this.permissionGroupInternal) {
        return true;
      }

      return this.permissionGroupInternal.subPermissions.length < 1;
    },
  },
  created() {
    this.permissionGroupInternal = this.permissionGroup as PermissionGroup;

    if (this.fcaPermissions) {
      this.subPermissionNamesFromFcaInternal =
        this.fcaPermissions.permissionNames;
      this.rawFcaPermissionsInternal = JSON.parse(
        this.fcaPermissions.raw as string,
      );
    }

    this.customerSubPermissionsInternal = this
      .customerSubPermissions as CustomerPermission[];
  },
  methods: {
    updateButtonSelection(
      event: PointerEvent,
      action: string,
      subPermissionName: string | undefined,
    ) {
      let button = event.currentTarget as HTMLButtonElement;

      if (button.classList?.contains(this.selectedButtonClass)) {
        button.classList.remove(this.selectedButtonClass);
        button.classList.add(this.enabledButtonClass);
      } else {
        button.classList.remove(this.enabledButtonClass);
        button.classList.add(this.selectedButtonClass);
      }

      if (subPermissionName === undefined) {
        subPermissionName = "";
      }

      this.$emit(action, subPermissionName);
    },

    getAddButtonState(subPermission: string | undefined): string {
      if (subPermission === undefined) {
        subPermission = "";
      }

      let found = this.customerSubPermissionsInternal.find(
        (p) => p.subPermissionName === subPermission,
      );

      if (!found) {
        found = this.customerSubPermissionsInternal.find(
          (p) => p.permissionGroupName === subPermission,
        );
      }

      return found &&
        found.isModified &&
        found.state === PermissionStateEnum.Added
        ? this.selectedButtonClass
        : this.enabledButtonClass;
    },

    getRemoveButtonState(subPermission: string | undefined): string {
      if (subPermission === undefined) {
        subPermission = "";
      }

      let found = this.customerSubPermissionsInternal.find(
        (p) => p.subPermissionName === subPermission,
      );

      if (!found) {
        found = this.customerSubPermissionsInternal.find(
          (p) => p.permissionGroupName === subPermission,
        );
      }

      return found &&
        found.isModified &&
        found.state === PermissionStateEnum.Removed
        ? this.selectedButtonClass
        : this.enabledButtonClass;
    },

    getApplicationPendingButtonState(
      subPermission: string | undefined,
      categoryName: string | undefined,
    ): string {
      if (this.isAuthorised(subPermission, categoryName)) {
        return this.enabledButtonClass;
      }

      if (subPermission === undefined) {
        subPermission = "";
      }

      return this.hasApplicationPending(subPermission);
    },

    isAuthorised(
      subPermissionName: string | undefined,
      categoryName: string | undefined,
    ): boolean {
      if (
        !this.subPermissionNamesFromFcaInternal ||
        this.subPermissionNamesFromFcaInternal.length < 1 ||
        !subPermissionName ||
        !categoryName
      ) {
        return false;
      }

      //for 'Insurance Broker' category only
      if (categoryName === AppConstants.PermissionCategoryInsuranceBroker) {
        if (
          subPermissionName.includes(
            "Making Arrangements with a view to Transactions in Investments",
          )
        ) {
        }

        // check if it has parenthesis
        const regex = /\[([^\]]+)\]$/; // Matches the text within the last square bracket
        const matches = regex.exec(subPermissionName);

        if (matches && matches.length > 0) {
          if (!this.rawFcaPermissionsInternal) {
            return false;
          }

          let rawFcaPermissions = this.rawFcaPermissionsInternal;
          let subPermissionNameTrimmed = subPermissionName
            .replace(matches[0], "")
            .trim();
          let searchText = matches[1]; // Get text Inside square bracket
          let isAuthorised = false;

          Object.keys(rawFcaPermissions).forEach((key) => {
            if (key.toLowerCase() === subPermissionNameTrimmed?.toLowerCase()) {
              const value = rawFcaPermissions[key];
              isAuthorised = this.recursivelyCheckValue(value, searchText);
            }
          });

          return isAuthorised;
        }
      }

      //logic for NON 'Insurance Broker'
      return this.checkIfSubPermissionListed(subPermissionName);
    },

    checkIfSubPermissionListed(subPermission: string) {
      //logic for NON 'Insurance Broker'
      let isListed = this.subPermissionNamesFromFcaInternal.find(
        (subP) => subP.toLowerCase() === subPermission.toLowerCase(),
      );

      if (isListed && isListed.trim().length > 0) {
        return true;
      }

      return false;
    },

    hasApplicationPending(subPermissionName: string | undefined): string {
      if (!subPermissionName) {
        return this.enabledButtonClass;
      }

      let found = this.customerSubPermissionsInternal?.find(
        (c) =>
          c.subPermissionName?.toLowerCase() ===
          subPermissionName.toLowerCase(),
      );

      if (!found) {
        found = this.customerSubPermissionsInternal?.find(
          (c) =>
            c.permissionGroupName?.toLowerCase() ===
            subPermissionName.toLowerCase(),
        );

        if (!found) {
          return this.enabledButtonClass;
        }
      }

      return found.hasPendingApplication
        ? this.selectedButtonClass
        : this.enabledButtonClass;
    },

    isApplicationPendingDisabled(
      subPermissionName: string | undefined,
      categoryName: string | undefined,
    ): boolean {
      if (this.isAuthorised(subPermissionName, categoryName)) {
        // if subPermission is 'Authorised', application pending button is disabled.
        return true;
      }

      if (subPermissionName === undefined) {
        subPermissionName = "";
      }

      let selectedSubPermission = this.customerSubPermissionsInternal.find(
        (c) => c.subPermissionName === subPermissionName,
      );

      if (!selectedSubPermission) {
        return false;
      }

      return (
        selectedSubPermission.isModified &&
        selectedSubPermission.state === PermissionStateEnum.Added
      );
    },

    isAddingPermissionDisabled(
      subPermissionName: string | undefined,
      categoryName: string | undefined,
    ): boolean {
      if (this.isAuthorised(subPermissionName, categoryName)) {
        // if subPermission is 'Authorised', add button is disabled.
        return true;
      }

      if (subPermissionName === undefined) {
        subPermissionName = "";
      }

      let selectedSubPermission = this.customerSubPermissionsInternal.find(
        (c) => c.subPermissionName === subPermissionName,
      );

      if (!selectedSubPermission) {
        return false;
      }

      return selectedSubPermission.hasPendingApplication;
    },

    computeSubPermissionDisplayName(subPermission: SubPermission): string {
      if (!subPermission) {
        return "";
      }

      if (!subPermission.displayText) {
        if (!subPermission.name) {
          return "";
        }

        return subPermission.name;
      }

      return subPermission.displayText;
    },

    recursivelyCheckValue(obj: any, targetValue: any): boolean {
      if (!obj) {
        return false;
      }

      if (typeof obj !== "object") {
        return obj === targetValue;
      }

      //handling object that is an array
      if (Array.isArray(obj)) {
        for (const element of obj) {
          if (this.recursivelyCheckValue(element, targetValue)) {
            return true;
          }
        }
      }

      //handling object that has children with array of keys and values
      if (typeof obj === "object") {
        for (const key in obj) {
          if (this.recursivelyCheckValue(obj[key], targetValue)) {
            return true;
          }
        }
      }

      return false;
    },
  },
});
</script>

<template>
  <div
    style="width: 100%; overflow-x: hidden"
    class="p-2 my-2 pb-4 card shadow d-flex gap-2"
  >
    <div
      v-if="permissionGroupInternal?.permissionGroupName"
      class="d-flex gap-2 font-weight-500"
      style="color: #160e83"
    >
      <!-- PERMISSION GROUP NAME -->
      <div class="col-6 d-flex gap-2 align-items-center">
        <img src="/permission.svg" style="width: 20px" alt="permission.svg" />

        <Label>{{ permissionGroupInternal?.permissionGroupName }}</Label>
      </div>

      <div v-if="hasNoSubPermissions" class="d-flex pt-1">
        <div class="col-3 d-flex gap-1 align-items-center">
          <img
            :src="
              isAuthorised(
                permissionGroupInternal?.permissionGroupName,
                permissionGroupInternal.permissionGroupName,
              )
                ? '/authorization.svg'
                : '/notauthorized.svg'
            "
            :alt="
              isAuthorised(
                permissionGroupInternal?.permissionGroupName,
                permissionGroupInternal.permissionGroupName,
              )
                ? 'authorization.svg'
                : 'notauthorized.svg'
            "
            style="width: 15px"
          />

          <Label style="font-size: 12px; color: #160e83">
            {{
              isAuthorised(
                permissionGroupInternal?.permissionGroupName,
                permissionGroupInternal.permissionGroupName,
              )
                ? "Authorised"
                : notAuthorised
            }}
          </Label>
        </div>

        <div class="col-9 d-flex gap-2 align-items-center">
          <Button
            type="button"
            class="px-3 shadow"
            :theme-color="'primary'"
            :class="
              getAddButtonState(permissionGroupInternal?.permissionGroupName)
            "
            :disabled="
              isAddingPermissionDisabled(
                permissionGroupInternal?.permissionGroupName,
                permissionGroupInternal.permissionGroupName,
              )
            "
            rounded="full"
            @click="
              (e) =>
                updateButtonSelection(
                  e,
                  'addPermission',
                  permissionGroupInternal?.permissionGroupName,
                )
            "
          >
            Add
          </Button>

          <Button
            type="button"
            class="px-3 shadow"
            rounded="full"
            :theme-color="'primary'"
            @click="
              (e) =>
                updateButtonSelection(
                  e,
                  'removePermission',
                  permissionGroupInternal?.permissionGroupName,
                )
            "
            :disabled="
              !isAuthorised(
                permissionGroupInternal?.permissionGroupName,
                permissionGroupInternal.permissionGroupName,
              )
            "
            :class="
              getRemoveButtonState(permissionGroupInternal?.permissionGroupName)
            "
          >
            Remove
          </Button>

          <Button
            type="button"
            class="px-3 shadow"
            :theme-color="'primary'"
            rounded="full"
            @click="
              (e) =>
                updateButtonSelection(
                  e,
                  'pendingApplication',
                  permissionGroupInternal?.permissionGroupName,
                )
            "
            :disabled="
              isApplicationPendingDisabled(
                permissionGroupInternal?.permissionGroupName,
                permissionGroupInternal.permissionGroupName,
              )
            "
            :class="
              getApplicationPendingButtonState(
                permissionGroupInternal?.permissionGroupName,
                permissionGroupInternal.permissionGroupName,
              )
            "
          >
            Application Pending
          </Button>
        </div>
      </div>
    </div>

    <!-- SUB PERMISSIONS -->
    <div
      class="px-2 py-1 d-flex flex-column"
      v-for="subPermission in permissionGroupInternal?.subPermissions"
    >
      <div class="d-flex">
        <!-- SUB PERMISSION NAME -->
        <div class="col d-flex align-items-center gap-2">
          <img src="/activity.svg" alt="activity.svg" style="width: 15px" />

          <Label style="font-size: 12px; text-wrap: normal; color: #6e767d">{{
            computeSubPermissionDisplayName(subPermission)
          }}</Label>
        </div>

        <!-- ACTIONS -->
        <div class="col d-flex align-items-center gap-5">
          <div class="col-3 d-flex gap-1">
            <img
              style="width: 15px"
              :src="
                isAuthorised(subPermission?.name, subPermission?.categoryName)
                  ? '/authorization.svg'
                  : '/notauthorized.svg'
              "
              :alt="
                isAuthorised(subPermission?.name, subPermission?.categoryName)
                  ? 'authorization.svg'
                  : 'notauthorized.svg'
              "
            />

            <Label style="font-size: 12px; color: #160e83">
              {{
                isAuthorised(subPermission?.name, subPermission?.categoryName)
                  ? "Authorised"
                  : notAuthorised
              }}
            </Label>
          </div>

          <div class="col-9 d-flex gap-2">
            <Button
              type="button"
              class="px-3 shadow"
              :theme-color="'primary'"
              :class="getAddButtonState(subPermission?.name)"
              :disabled="
                isAddingPermissionDisabled(
                  subPermission?.name,
                  subPermission?.categoryName,
                )
              "
              @click="
                (e) =>
                  updateButtonSelection(e, 'addPermission', subPermission?.name)
              "
              rounded="full"
              >Add
            </Button>

            <Button
              type="button"
              class="px-3 shadow"
              :theme-color="'primary'"
              rounded="full"
              @click="
                (e) =>
                  updateButtonSelection(
                    e,
                    'removePermission',
                    subPermission?.name,
                  )
              "
              :disabled="
                !isAuthorised(subPermission?.name, subPermission?.categoryName)
              "
              :class="getRemoveButtonState(subPermission?.name)"
            >
              Remove
            </Button>

            <Button
              type="button"
              class="px-3 shadow"
              :theme-color="'primary'"
              rounded="full"
              @click="
                (e) =>
                  updateButtonSelection(
                    e,
                    'pendingApplication',
                    subPermission?.name,
                  )
              "
              :disabled="
                isApplicationPendingDisabled(
                  subPermission?.name,
                  subPermission?.categoryName,
                )
              "
              :class="
                getApplicationPendingButtonState(
                  subPermission?.name,
                  subPermission?.categoryName,
                )
              "
            >
              Application Pending
            </Button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.card {
  border-radius: 16px;
}

.font-weight-500 {
  font-weight: 500;
}

.shadow {
  box-shadow: 0 4px 4px rgba(0, 0, 0, 0.25);
}

.selected-btn {
  background-color: #160e83;
  color: white;
}

.enabled-btn {
  background-color: #ececee;
  color: #6e767d;
}
</style>