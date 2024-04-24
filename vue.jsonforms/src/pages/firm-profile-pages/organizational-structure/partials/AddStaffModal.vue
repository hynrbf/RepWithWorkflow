<script lang="ts">
import { Employee } from "@/entities/firm-details/Employee";
import { defineComponent, ref } from "vue";
import _ from "lodash";
import { v4 as uuidv4 } from "uuid";
import Role from "@/entities/org-structure/Role";
import { useAlert } from "@/composables/useAlert";
import { AppConstants } from "@/infra/AppConstants";
import { usePageLifeCycleStore } from "@/stores/progress-bar/usePageLifeCycleStore";
import { useOrganizationalStructureStore } from "@/stores/organizational-structure/useOrganizationalStructureStore";
import StaticList from "@/infra/StaticListService";
import FcaRoleHeaderTemplate from "@/pages/firm-profile-pages/organizational-structure/partials/FcaRoleHeaderTemplate.vue";
import FcaRoleItemTemplate from "@/pages/firm-profile-pages/organizational-structure/partials/FcaRoleItemTemplate.vue";
import DynamicAvatar from "@/components/DynamicAvatarComponent.vue";
import DropdownListItemModel from "@/components/models/DropdownListItemModel";
import { ProductType } from "@/entities/org-structure/ProductType";
import KendoDropdownListWithTypeComponent from "@/components/form-fields/KendoDropdownListWithTypeComponent.vue";
import { IEmployeeTask } from "@/entities/org-structure/IEmployeeTask";
import { container } from "tsyringe";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import { ContactNumber } from "@/entities/ContactNumber";
import {
  NotificationType,
  useNotification,
} from "@/composables/useNotification";

export default defineComponent({
  components: {
    KendoDropdownListWithTypeComponent,
    DynamicAvatar,
    FcaRoleItemTemplate,
    FcaRoleHeaderTemplate,
  },
  props: {
    employee: {
      type: Object as () => Employee,
      default: new Employee(),
    },
    employeeValues: {
      type: Array as () => Employee[],
      default: () => [],
    },
    saving: {
      type: Boolean,
      default: false,
    },
    isEdit: {
      type: Boolean,
      default: false,
    },
  },
  setup() {
    const modalElement = ref();

    const pageLifeCycleStore = usePageLifeCycleStore();
    const { changeLifeCycleName } = pageLifeCycleStore;

    const organizationalStructureStore = useOrganizationalStructureStore();
    const {
      persistPrimaryRoles,
      persistOtherRoles,
      clearAllRoles,
      getPrimaryRoles,
      getOtherRoles,
    } = organizationalStructureStore;

    return {
      changeLifeCycleName,
      modalElement,
      persistPrimaryRoles,
      persistOtherRoles,
      clearAllRoles,
      getPrimaryRoles,
      getOtherRoles,
    };
  },
  data() {
    return {
      isInitializing: true,
      additionalRole: "",
      primaryRolesFromStore: [] as DropdownListItemModel[],
      otherRolesObjFromStore: [] as DropdownListItemModel[],
      originalPrimaryRoles: [] as Role[],
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      employeeInternal: this.employee,
    };
  },
  computed: {
    Role() {
      return Role;
    },
    Employee() {
      return Employee;
    },
    ProductType() {
      return ProductType;
    },
    ContactNumber() {
      return ContactNumber;
    },
    DropdownListItemModel() {
      return DropdownListItemModel;
    },
    AppConstants() {
      return AppConstants;
    },
    lineManagers(): DropdownListItemModel[] {
      const lineManagerDropdownList = this.employeeValues.map((item) => {
        if (item.id === this.employee.id) {
          return {
            label: "",
            value: "",
          };
        }

        const listItem = this.mapEmployeeToDropdownListItem(item);

        if (!listItem) {
          return {
            label: "",
            value: "",
          };
        }

        return listItem;
      });

      return lineManagerDropdownList.filter((item) => item.label);
    },
    isMinimumFieldsSupplied(): boolean {
      if (
        !this.employeeInternal?.firstName ||
        !this.employeeInternal?.lastName ||
        !this.employeeInternal?.email
      ) {
        return false;
      }

      return (
        this.employeeInternal.firstName.length > 0 &&
        this.employeeInternal.lastName.length > 0 &&
        this.helperService.checkIfEmailFormatIsValid(
          this.employeeInternal.email,
        )
      );
    },
    isAllRequiredFieldsSupplied(): boolean {
      if (
          !this.employeeInternal?.firstName ||
          !this.employeeInternal?.lastName ||
          !this.employeeInternal?.lineManager ||
          !this.employeeInternal?.primaryRole ||
          !this.employeeInternal?.productType ||
          !this.employeeInternal?.email ||
          !this.employeeInternal?.contactNumber
      ) {
        return false;
      }

      return (
          this.employeeInternal.firstName.length > 0 &&
          this.employeeInternal.lastName.length > 0 &&
          this.employeeInternal.lineManager &&
          !!this.employeeInternal.primaryRole.name &&
          this.employeeInternal.primaryRole.name.length>0 &&
          this.employeeInternal.productType.length > 0 &&
          this.helperService.checkIfEmailFormatIsValid(
              this.employeeInternal.email,
          ) &&
          this.employeeInternal.contactNumber.number !== undefined
      );
    },
  },
  watch: {
    employeeInternal(newValue, _oldValue): Employee {
      if (!this.isEdit) {
        this.clearAllRoles();
      }

      this.setupRoles();
      return newValue;
    },
  },
  beforeMount() {
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameCreated);
  },
  mounted() {
    this.clearAllRoles();
    this.setupRoles();
    this.isInitializing = false;
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameMounted);
  },
  unmounted() {
    this.clearAllRoles();
  },
  methods: {
    // Init
    setupRoles() {
      // TODO. to get from fca later
      const primaryRolesTemp = StaticList.getOrgStructureRoles();
      let rolesObj = [] as DropdownListItemModel[];

      if (primaryRolesTemp && primaryRolesTemp.length) {
        primaryRolesTemp.forEach((role) => {
          let newRoleModelItem: Role;

          if (role === this.employeeInternal?.primaryRole?.name) {
            newRoleModelItem = {
              name: this.employeeInternal.primaryRole?.name,
              state: this.employeeInternal.primaryRole?.state,
              isModified: this.employeeInternal.primaryRole?.isModified,
              isFcaAuthorised:
                this.employeeInternal.primaryRole?.isFcaAuthorised,
              isPending: this.employeeInternal.primaryRole?.isPending,
            };
          } else {
            newRoleModelItem = {
              name: role,
              isFcaAuthorised: false,
              state: AppConstants.removedState,
              isModified: false,
              isPending: false,
            };
          }

          const clonedRole = _.cloneDeep(newRoleModelItem);
          this.originalPrimaryRoles.push(clonedRole);

          rolesObj.push({
            label: role,
            value: role,
            raw: newRoleModelItem,
          } as DropdownListItemModel);
        });

        this.persistPrimaryRoles(rolesObj);
      }

      this.persistOtherRoles(rolesObj);
      this.primaryRolesFromStore = this.getPrimaryRoles();
      this.otherRolesObjFromStore = this.getOtherRoles();
    },

    // Line Managers
    onLineManagerChange(
      employee: Employee,
      lineManagerListItem: DropdownListItemModel,
    ) {
      if (!lineManagerListItem) {
        return;
      }

      employee.lineManager = <Employee>lineManagerListItem.raw;
    },

    // Employees
    mapEmployeeToDropdownListItem(
      employee: Employee,
    ): DropdownListItemModel | null {
      if (!employee) {
        return null;
      }

      const fullName = `${employee.firstName} ${employee.lastName}`;

      return {
        label: fullName,
        value: fullName,
        raw: employee,
      };
    },

    // Roles
    mapRoleToDropdownListItem(role: Role): DropdownListItemModel {
      return {
        label: role.name,
        value: role.name,
        raw: role,
      };
    },

    mapRolesToDropdownListItems(roles: Role[]): DropdownListItemModel[] {
      if (!roles?.length) {
        return [] as DropdownListItemModel[];
      }

      return roles.map((item) => {
        return {
          label: item.name,
          value: item.name,
          raw: item,
        };
      });
    },

    onPrimaryRoleChange(
      employee: Employee,
      roleListItem: DropdownListItemModel,
    ) {
      if (!roleListItem) {
        return;
      }

      employee.primaryRole = <Role>roleListItem.raw;
    },

    onOtherRoleChange(
      employee: Employee,
      roleListItems: DropdownListItemModel[],
    ) {
      if (!roleListItems) {
        return;
      }

      employee.otherRoles = roleListItems.map((item) => {
        return <Role>item.raw;
      });
    },

    handleRoleAction(
      role: Role,
      isPrimary: boolean,
      alertContent: string,
      confirmButtonText: string,
      confirmButtonThemeColor?: string,
    ) {
      //once we select role, we hide dropdown at once
      let dropDownListSelector = document.querySelector(
        ".k-animation-container.k-animation-container-relative.k-animation-container-shown",
      ) as HTMLElement;

      if (!dropDownListSelector) {
        return;
      }

      dropDownListSelector.style.display = "none";

      // TODO. to get back. below is not working so hardcoded for now.
      // getCurrentInstance()?.appContext.config.globalProperties.$t("common-alert-title")
      useAlert({
        title: "Confirm",
        content: alertContent,
        confirmButtonText,
        confirmButtonThemeColor,
        onConfirm: () => {
          useNotification({
            type: NotificationType.SUCCESS,
            content: "Request Submitted.",
            interval: 2e3,
          });

          if (isPrimary) {
            this.employeeInternal.primaryRole = role;
          } else {
            this.employeeInternal.otherRoles?.push(role);
          }

          this.updateRoleState(role, isPrimary);
        },
        onClose: () => {},
      });
    },

    addRoleToFca(role: Role, isPrimary: boolean) {
      if (role.state === AppConstants.addedState) {
        // just update without prompt
        this.updateRoleState(role, isPrimary);
        return;
      }

      const alertContent =
        "This action requires a Variation of Permission Application to the FCA. Are you sure you wish to begin performing this activity?";
      this.handleRoleAction(role, isPrimary, alertContent, "Confirm & Add");
    },

    removeRoleFromFca(role: Role, isPrimary: boolean) {
      if (role.state === AppConstants.removedState) {
        // just update without prompt
        this.updateRoleState(role, isPrimary);
        return;
      }

      const alertContent =
        "This action requires a Variation of Permission Application to the FCA. Are you sure you no longer wish to perform this activity?";
      this.handleRoleAction(
        role,
        isPrimary,
        alertContent,
        "Confirm & Remove",
        "error",
      );
    },

    toggleRoleIsPending(role: Role, _isPrimary: boolean) {
      role.isPending = !role.isPending;
      role.isModified = this.checkIfModified(role);
    },

    checkIfModified(role: Role): boolean {
      const foundOriginalRole = this.originalPrimaryRoles.find(
        (r) => r.name === role.name,
      );

      if (foundOriginalRole?.isModified) {
        return true;
      }

      return (
        role.isPending !== foundOriginalRole?.isPending ||
        role.state !== foundOriginalRole.state
      );
    },

    updateRoleState(role: Role, _isPrimary: boolean) {
      role.state =
        role.state === AppConstants.addedState
          ? AppConstants.removedState
          : AppConstants.addedState;
      role.isModified = this.checkIfModified(role);
    },

    getRoleStatusStyle(roleName: string): string {
      if (!roleName) {
        return "";
      }

      const found = this.primaryRolesFromStore?.find(
        (r) => r?.value === roleName,
      ) as DropdownListItemModel;

      if (!found) {
        return "";
      }

      const roleItem = this.getRoleObj(found);

      if (!roleItem) {
        return "";
      }

      if (roleItem?.isModified) {
        return "modified-role";
      }

      if (roleItem?.isFcaAuthorised) {
        return "authorised-role";
      }

      return "";
    },

    getRoleObj(item: DropdownListItemModel): Role | undefined {
      if (!item) {
        return undefined;
      }

      if (!item) {
        return undefined;
      }

      return item.raw as Role;
    },

    addCustom(value: string, _attachValue: () => void) {
      this.persistPrimaryRoles([]);
      this.primaryRolesFromStore.push({
        label: value,
        value: value,
        raw: {
          name: value,
          state: AppConstants.removedState,
          isFcaAuthorised: false,
          isPending: false,
          isModified: false,
        },
      } as DropdownListItemModel);

      // Add also to the otherRoles
      this.otherRolesObjFromStore.push({
        label: value,
        value: value,
        raw: {
          name: value,
          state: AppConstants.removedState,
          isFcaAuthorised: false,
          isPending: false,
          isModified: false,
        },
      } as DropdownListItemModel);
    },

    // Product Type
    onProductTypesChange(employee: Employee, productTypeItems: ProductType[]) {
      if (!productTypeItems) {
        return;
      }

      employee.productType = productTypeItems;
    },

    // Form Operations
    handleRequestStaffToCompleteDetails(values: Partial<Employee>) {
      this.$emit("requestStaffToCompleteDetails", {
        ...(this.employeeInternal || {}),
        ...values,
      });
    },

    handleSubmit() {
      if (!this.employeeInternal.id) {
        this.employeeInternal.id = uuidv4();
      }

      // TEMP. for now because we still don't have means of adding tasks
      this.employeeInternal.tasks = [
        {
          name: "Implement compliance tools",
          description:
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
          dueDate: this.helperService.dateToEpoch(new Date("01/31/2024")),
        },
      ] as IEmployeeTask[];

      this.$emit("submit", this.employeeInternal);
      (this.$refs?.formElement as HTMLFormElement)?.$el?.requestSubmit?.();
    },

    setUniqueIdentifier(value: string): string {
      const identifier = `${AppConstants.organizationalStructureRoute}${value}`;
      return identifier.replace(/\s+/g, "").replace("/", "");
    },
  },
});
</script>

<template>
  <ModalComponent
    ref="modalElement"
    :title="`${isEdit ? '' : 'Add New Staff'}`"
    width="750"
  >
    <Form>
      <FormElement ref="formElement">
        <OverlayLoader :loading="saving">
          <!-- Avatar, Edit Button, Divider -->
          <div class="d-flex flex-column" v-if="isEdit">
            <div class="d-flex align-self-center" style="margin-left: 14px">
              <DynamicAvatar
                size="large"
                type="image"
                rounded="full"
                :avatarCustomStyle="{
                  flexBasis: '60px',
                  height: '60px',
                }"
                :imageSrc="employee?.img_id"
                :imageAlt="employee?.img_id"
              />

              <KendoButton
                class="edit-icon-button"
                type="button"
                size="small"
                shape="square"
                theme-color="light"
                title="Edit"
              >
                <IconComponent
                  symbol="edit-pen"
                  size="20"
                  style="color: var(--brand-color-brand-primary)"
                />
              </KendoButton>
            </div>

            <text class="edit-text"> Edit</text>

            <DividerComponent
              :height="1"
              :width="690"
              :spacing="0"
              style="margin-top: 20px; margin-bottom: 20px"
            />
          </div>

          <div class="d-flex flex-column" style="gap: 20px">
            <!-- Title, Forename(s), Surname -->
            <div class="d-flex" style="gap: 15px">
              <KendoNameTitleComponent
                class="col"
                :name="setUniqueIdentifier('-title')"
                :id="'title'"
                :isRequired="false"
                label="Title"
                :value="employeeInternal?.title"
                @onValueChange="
                  (value: string) => (employeeInternal.title = value)
                "
                :isDataLoadedCompletely="!isInitializing"
                :isValueReactive="true"
              />

              <KendoGenericInputComponent
                :id="setUniqueIdentifier('-foreName')"
                class="col"
                name="foreName"
                label="Forename(s)"
                placeholder="John"
                :value="employeeInternal?.firstName"
                @onValueChange="
                  (value: string) => (employeeInternal.firstName = value)
                "
                :isDataLoadedCompletely="!isInitializing"
                :isValueReactive="true"
              />

              <KendoGenericInputComponent
                :id="setUniqueIdentifier('-surName')"
                class="col"
                name="surName"
                label="Surname"
                placeholder="Doe"
                :value="employeeInternal?.lastName"
                @onValueChange="
                  (value: string) => (employeeInternal.lastName = value)
                "
                :isDataLoadedCompletely="!isInitializing"
                :isValueReactive="true"
              />
            </div>

            <!-- Line Manager, Primary Role, Other Roles -->
            <KendoDropdownListWithTypeComponent
              :id="setUniqueIdentifier('-lineManager')"
              name="lineManager"
              label="Line Manager"
              :disabled="employeeInternal?.isRoot"
              :dataItems="lineManagers"
              :value="
                employeeInternal?.lineManager
                  ? mapEmployeeToDropdownListItem(employeeInternal.lineManager)
                  : null
              "
              @onValueChange="
                (lineManager: DropdownListItemModel) =>
                  onLineManagerChange(employee, lineManager)
              "
              :isDataLoadedCompletely="!isInitializing"
              :isValueReactive="true"
            >
              <template #display="{ value }">
                <div v-if="employeeInternal?.isRoot" class="hstack gap-2">
                  {{ AppConstants.root }}
                </div>

                <div v-else class="hstack gap-2">{{ value?.label }}</div>
              </template>
            </KendoDropdownListWithTypeComponent>

            <KendoDropdownListWithTypeComponent
              :id="setUniqueIdentifier('-primaryRole')"
              :name="'primaryRole'"
              label="Primary Role"
              :value="
                employeeInternal?.primaryRole
                  ? mapRoleToDropdownListItem(employeeInternal.primaryRole)
                  : null
              "
              placeholder="Select Primary Role"
              :dataItems="primaryRolesFromStore"
              :addable="true"
              bypassDefaultOnSelectEvent
              @addCustom="addCustom"
              @onValueChange="
                (value: DropdownListItemModel) =>
                  onPrimaryRoleChange(employee, value)
              "
              :isDataLoadedCompletely="!isInitializing"
              :isValueReactive="true"
            >
              <template #header>
                <FcaRoleHeaderTemplate />
              </template>

              <template v-slot:display="{ value, clickEvent }">
                <FcaRoleItemTemplate
                  :role="getRoleObj(value as DropdownListItemModel)"
                  :clickEvent="clickEvent"
                  @addRoleToFca="(role: Role) => addRoleToFca(role, true)"
                  @removeRoleFromFca="
                    (role: Role) => removeRoleFromFca(role, true)
                  "
                  @toggleRoleIsPending="
                    (role: Role) => toggleRoleIsPending(role, true)
                  "
                />
              </template>

              <template v-slot:valueDisplayTemplate="{ value }">
                <Label
                  :style="
                    getRoleObj(value as DropdownListItemModel)?.isModified
                      ? 'color: var(--color-warning-700)'
                      : getRoleObj(value as DropdownListItemModel)
                          ?.isFcaAuthorised
                      ? 'color: var(--color-success-600)'
                      : ''
                  "
                >
                  {{ getRoleObj(value as DropdownListItemModel)?.name }}
                </Label>
              </template>

              <template v-slot:footerTemplate="{ onAdd }">
                <div class="col-6">
                  <InputWithAddComponent
                    v-model="additionalRole"
                    placeholder="Others"
                    @add="onAdd(additionalRole)"
                  />
                </div>
              </template>
            </KendoDropdownListWithTypeComponent>

            <KendoMultiSelectTreeComponent
              :id="setUniqueIdentifier('-otherRole')"
              :name="'otherRole'"
              label="Other Roles"
              :value="
                employee?.otherRoles
                  ? mapRolesToDropdownListItems(employee.otherRoles)
                  : []
              "
              @onValueChange="
                (roles: DropdownListItemModel[]) =>
                  onOtherRoleChange(employee, roles)
              "
              placeholder="Select Other Applicable Roles"
              :dataItems="otherRolesObjFromStore"
              :isRequired="false"
              @addCustom="addCustom"
              :isDataLoadedCompletely="!isInitializing"
              :isValueReactive="true"
              bypassDefaultOnSelectEvent
              :addable="true"
            >
              <template #header>
                <FcaRoleHeaderTemplate is-multi-select />
              </template>

              <template #display="{ item, clickEvent }">
                <FcaRoleItemTemplate
                  :role="getRoleObj(item as DropdownListItemModel)"
                  :clickEvent="clickEvent"
                  @addRoleToFca="(role: Role) => addRoleToFca(role, false)"
                  @removeRoleFromFca="
                    (role: Role) => removeRoleFromFca(role, false)
                  "
                  @toggleRoleIsPending="
                    (role: Role) => toggleRoleIsPending(role, false)
                  "
                  isMultiSelect
                />
              </template>

              <template #tagTemplate="{ tagProps, item, onClose }">
                <KendoTooltip anchor-element="target" position="top">
                  <PillComponent
                    themeColor="white"
                    size="lg"
                    class="font-weight-semi-bold"
                    :class="getRoleStatusStyle(tagProps?.tagData.text)"
                    textClass="is-truncated"
                    closeable
                    @close="onClose(item)"
                  >
                    <span class="is-truncated" :title="tagProps?.tagData.text">
                      {{ tagProps?.tagData.text }}
                    </span>
                  </PillComponent>
                </KendoTooltip>
                <a
                  href="#"
                  v-if="
                    tagProps?.dataItems.length > 4 &&
                    tagProps?.dataItems.length - 1 === tagProps?.index
                  "
                >
                  <b>+{{ tagProps?.dataItems.length - 4 }}</b>
                </a>
              </template>

              <template #footerTemplate="{ onAdd }">
                <div class="col-6">
                  <InputWithAddComponent
                    v-model="additionalRole"
                    placeholder="Others"
                    @add="onAdd(additionalRole)"
                  />
                </div>
              </template>
            </KendoMultiSelectTreeComponent>

            <KendoProductTypeSelectComponent
              :id="setUniqueIdentifier('-productType')"
              name="productType"
              label="Product Type"
              pageName="Organisational Structure Chart"
              :value="employee?.productType"
              @onValueChange="
                (productTypes: ProductType[]) =>
                  onProductTypesChange(employee, productTypes)
              "
              :isDataLoadedCompletely="!isInitializing"
              :isValueReactive="true"
            />

            <!-- Email Address, Contact Number -->
            <div class="d-flex" style="gap: 15px">
              <KendoEmailAddressInputComponent
                :id="setUniqueIdentifier('-emailAddress')"
                class="col"
                name="emailAddress"
                label="Email Address"
                placeholder="name@domain.com"
                :value="employeeInternal?.email"
                @onValueChange="
                  (value: string) => (employeeInternal.email = value)
                "
                :isDataLoadedCompletely="!isInitializing"
                :isValueReactive="true"
              />

              <KendoTelInputComponent
                :id="setUniqueIdentifier('-contactNo')"
                class="col"
                name="contactNo"
                label="Contact Number"
                :value="employeeInternal?.contactNumber"
                @onValueChange="
                  (value: ContactNumber) =>
                    (employeeInternal.contactNumber = value)
                "
                :isValueReactive="true"
                :isDataLoadedCompletely="!isInitializing"
              />
            </div>
          </div>

          <!-- Action Buttons -->
          <div class="text-right" style="margin-top: 20px">
            <KendoButton
              style="margin-right: 10px; font-weight: 600"
              type="button"
              fill-mode="outline"
              theme-color="primary"
              :disabled="!isAllRequiredFieldsSupplied"
              @click="handleRequestStaffToCompleteDetails"
            >
              Request Staff to Complete Details
            </KendoButton>

            <KendoButton
              type="button"
              theme-color="primary"
              :disabled="saving || !isMinimumFieldsSupplied"
              @click="handleSubmit"
            >
              {{ isEdit ? "Save Changes" : "Save & Add New Staff" }}
            </KendoButton>
          </div>
        </OverlayLoader>
      </FormElement>
    </Form>
  </ModalComponent>
</template>

<style scoped>
.edit-icon-button {
  position: absolute;
  margin-left: 50px;
  margin-top: 4px;
  width: 25px;
  height: 25px;
}

.edit-text {
  margin-top: 10px;
  color: var(--color-black);
  text-align: center;
  font-size: var(--font-size-xl);
  font-style: normal;
  font-weight: var(--font-weight-bold);
  line-height: 28px;
}

.authorised-role {
  border-color: var(--color-success-700);
}

.modified-role {
  border-color: var(--color-warning-700);
}
</style>