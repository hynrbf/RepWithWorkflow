<script lang="ts">
import { defineAsyncComponent, defineComponent, inject, PropType } from "vue";
import { Employee } from "@/entities/firm-details/Employee";
import ARGridActionButtons from "@/pages/ar-pages/organizational-structure/partials/ARGridActionButtons.vue";
import GridCardTable from "@/components/GridCardTable.vue";
import Role from "@/entities/org-structure/Role";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import { container } from "tsyringe";
import EmployeeModel from "@/pages/models/organizational-structure/EmployeeModel";
import { ContactNumber } from "@/entities/ContactNumber";
import DynamicAvatar from "@/components/DynamicAvatarComponent.vue";
import { orderBy, SortDescriptor } from "@progress/kendo-data-query";
import {
  GridPageChangeEvent,
  GridSortChangeEvent,
} from "@progress/kendo-vue-grid";
import { Emitter, EventType } from "mitt";
import { AppConstants } from "@/infra/AppConstants";

export default defineComponent({
  name: "OrganizationalStructureList",
  components: {
    DynamicAvatar,
    ARUserDetailAsync: defineAsyncComponent(
      () =>
        import(
          "@/pages/ar-pages/organizational-structure/partials/ARUserDetail.vue"
        ),
    ),
    ARTaskDetailsAsync: defineAsyncComponent(
      () =>
        import(
          "@/pages/ar-pages/organizational-structure/partials/ARTaskDetails.vue"
        ),
    ),
    ARRoleDetailsAsync: defineAsyncComponent(
      () =>
        import(
          "@/pages/ar-pages/organizational-structure/partials/ARRoleDetails.vue"
        ),
    ),
    ARGridActionButtons,
    GridCardTable,
  },
  props: {
    employeesValues: {
      type: Array as PropType<Employee[]>,
      default: () => [],
    },
  },
  data() {
    return {
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      defaultStaffAvatar: "default-staff-avatar.png",
      defaultLineManagerAvatar: "default-line-manager-avatar.png",
      gridData: [] as EmployeeModel[],
      isShowUserDetail: false,
      isShowDetailDialog: false,
      detailDialogTitle: "",
      detailDialogWidth: 380,
      employeeAddEditDialogWidth: 750,
      employee: new Employee(),
      sortParams: [] as SortDescriptor[],
      skip: 0,
      take: 7,
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
    };
  },
  created() {
    this.gridData = this.formatEmployeeData();
    const itemsToPDF = orderBy(this.gridData, this.sortParams);
    this.$emit("exportingPDF", this.columnsToPDF, itemsToPDF);
    this.eventBus.on(AppConstants.filterGridEvent, () => {
      if (this.skip == 0) {
        return;
      }

      this.skip = 0;
    });
  },
  unmounted() {
    this.eventBus.off(AppConstants.filterGridEvent);
  },
  watch: {
    employeesValues: {
      handler() {
        this.gridData = this.formatEmployeeData();
      },
      deep: true,
    },
  },
  computed: {
    ContactNumber() {
      return ContactNumber;
    },
    Employee() {
      return Employee;
    },
    EmployeeModel() {
      return EmployeeModel;
    },
    columns() {
      return [
        {
          field: "staffName",
          title: "Staff Name",
          width: 200,
        },
        {
          field: "primaryRoleStr",
          title: "Role(s)",
          width: 280,
        },
        {
          field: "emailAddress",
          title: "Email Address",
          width: 180,
        },
        {
          field: "contactNumberStr",
          title: "Contact Number",
          width: 160,
          sortable: false,
        },
        {
          field: "lineManagerName",
          title: "Line Manager",
          width: 200,
        },
        {
          field: "status",
          title: "Status",
          width: 130,
        },
        {
          field: "task",
          title: "Task(s)",
          width: 80,
          sortable: false,
        },
        {
          field: "actions",
          title: " ",
          width: 108,
          sortable: false,
        },
      ];
    },
    // ToDo. some of the titles are cutting off in the pdf even I remove the width of the columns
    columnsToPDF() {
      return this.columns.filter((column) => column.field !== "actions");
    },
  },
  methods: {
    orderBy,
    formatEmployeeData(): EmployeeModel[] {
      return this.employeesValues.map((employee) => {
        return {
          ...employee,
          staffName: `${employee.firstName} ${employee.lastName}`,
          primaryRoleStr: employee.primaryRole?.name,
          lineManagerName: `${employee.lineManager?.firstName ?? ""} ${
            employee.lineManager?.lastName ?? ""
          }`,
          contactNumberStr: employee.contactNumber?.number,
          status: employee.employmentStatus,
          task: employee.tasks?.length ?? 0,
        } as EmployeeModel;
      });
    },

    emitEditEvent(employee: Employee) {
      this.employee = employee;
      this.$emit("editUser", this.employee);
    },

    viewUser(employee: Employee) {
      this.isShowUserDetail = true;
      this.employee = employee;
      this.$emit("viewUser", this.employee);
    },

    findEmployeeById(id: string): Employee {
      return (
        this.employeesValues.find((employee) => employee.id === id) ??
        new Employee()
      );
    },

    closeViewUserModal() {
      this.isShowUserDetail = false;
      this.employee = new Employee();
    },

    getCount(list: any[] | undefined): number {
      return list?.length ?? 0;
    },

    displayOtherRoles(employee: Employee) {
      this.employee = employee;
      this.detailDialogTitle = "Roles";
      this.isShowDetailDialog = true;
    },

    onTaskOpen(employee: Employee) {
      if (!employee) {
        return;
      }

      this.employee = employee;

      if (!this.employee.tasks?.length) {
        return;
      }

      this.detailDialogTitle = "Tasks";
      this.isShowDetailDialog = true;
    },

    getPrimaryRoleStyleClass(primaryRole: Role | undefined): string {
      if (!primaryRole) {
        return "";
      }

      if (primaryRole.isModified) {
        return "modified";
      }

      return primaryRole.isFcaAuthorised ? "authorised" : "";
    },

    getStatusPillColor(employmentStatus: string): string {
      if (employmentStatus === "Active") {
        return "good-status-pill";
      }

      if (employmentStatus === "Onboarding") {
        return "warning-status-pill";
      }

      // If 'Suspended' or 'Terminated'
      return "bad-status-pill";
    },

    handleSort(e: GridSortChangeEvent) {
      this.sortParams = e.sort;
    },

    handlePage(e: GridPageChangeEvent) {
      this.skip = e.page.skip;
      this.take = e.page.take;
    },
  },
});
</script>

<template>
  <GridCardTable
    class="mt-4"
    id="organizational-structure-list"
    isSortable
    :columns="columns"
    :items="orderBy(gridData, sortParams).slice(skip, skip + take)"
    :noToolbar="true"
    :centerAlignedColumnHeaders="['status', 'task']"
    :sort="sortParams"
    :handleSortChange="handleSort"
    :handlePageChange="handlePage"
    :pageable="true"
    :skip="skip"
    :take="take"
    :total="gridData?.length ?? 0"
  >
    <template v-slot:cell-staffName="{ item: { staffName, img_id } }">
      <DynamicAvatarComponent
        rounded="full"
        type="image"
        class="staff-name-cell"
        :imageSrc="img_id"
        :imageAlt="img_id"
        :text="staffName"
      >
        <template #text="{ text }">
          <span>{{ text }}</span>
        </template>
      </DynamicAvatarComponent>
    </template>

    <template v-slot:cell-primaryRoleStr="{ item }">
      <div
        v-if="(item as EmployeeModel)?.primaryRoleStr"
        class="roles-cell"
        @click="displayOtherRoles(item as Employee)"
      >
        <Label
          class="tail-truncated-text"
          :class="
            getPrimaryRoleStyleClass(
              (item as EmployeeModel)?.primaryRole ?? undefined,
            )
          "
        >
          {{ (item as EmployeeModel)?.primaryRoleStr }}
        </Label>

        <Label
          class="remaining-count"
          v-if="getCount((item as EmployeeModel)?.otherRoles ?? undefined)"
        >
          +{{ getCount((item as EmployeeModel)?.otherRoles ?? undefined) }}
        </Label>
      </div>
    </template>

    <template v-slot:cell-emailAddress="{ item: { emailAddress } }">
      <span class="email-cell-text">{{ emailAddress }}</span>
    </template>

    <template v-slot:cell-contactNumber.number="{ item: { contactNumber } }">
      <span class="contact-number-cell-text"
        >{{ (contactNumber as ContactNumber)?.dialCode }}
        {{ (contactNumber as ContactNumber)?.number }}</span
      >
    </template>

    <template v-slot:cell-lineManager="{ item: employee }">
      <div v-if="!(employee as Employee)?.lineManager">
        <span>&nbsp;</span>
      </div>

      <DynamicAvatarComponent
        v-else
        rounded="full"
        type="image"
        class="staff-name-cell"
        :imageSrc="(employee as Employee)?.lineManager?.img_id"
        :imageAlt="(employee as Employee)?.lineManager?.img_id"
        :text="`${(employee as Employee)?.lineManager?.firstName} ${
          (employee as Employee)?.lineManager?.lastName
        }`"
      >
        <template #text="{ text }">
          <span>{{ text }}</span>
        </template>
      </DynamicAvatarComponent>
    </template>

    <template v-slot:cell-status="{ item: { employmentStatus } }">
      <div v-if="employmentStatus" class="d-flex justify-content-center">
        <PillComponent
          class="employment-status"
          :class="getStatusPillColor(employmentStatus)"
        >
          {{ employmentStatus }}
        </PillComponent>
      </div>
    </template>

    <template v-slot:cell-task="{ item: employee }">
      <div
        v-if="employee"
        class="tasks-cell"
        @click="onTaskOpen(employee as Employee)"
      >
        <span
          :style="{
            textDecoration: (employee as Employee)?.tasks?.length
              ? 'underline'
              : 'none',
            cursor: (employee as Employee)?.tasks?.length
              ? 'pointer'
              : 'default',
          }"
          >{{ (employee as Employee)?.tasks?.length ?? 0 }}</span
        >
      </div>
    </template>

    <template v-slot:cell-actions="{ item: employee }">
      <ARGridActionButtons
        @view="viewUser(employee as Employee)"
        @edit="emitEditEvent(employee as Employee)"
      />
    </template>
  </GridCardTable>

  <!-- Employee Details -->
  <KendoDetailPopupComponent
    v-if="isShowUserDetail"
    :width="employeeAddEditDialogWidth"
    @close="closeViewUserModal"
  >
    <template #content>
      <ARUserDetailAsync
        :employee="employee"
        :dialogWidth="employeeAddEditDialogWidth"
      />
    </template>
  </KendoDetailPopupComponent>

  <!-- Employee's Roles/Tasks Details -->
  <KendoDetailPopupComponent
    v-if="isShowDetailDialog"
    :width="detailDialogWidth"
    :title="detailDialogTitle"
    @close="isShowDetailDialog = false"
  >
    <template #content>
      <StackLayout
        orientation="vertical"
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

        <Label class="detail-dialog-employee-name">
          {{ employee.firstName }} {{ employee.lastName }}
        </Label>
      </StackLayout>

      <ARRoleDetailsAsync
        v-if="detailDialogTitle === 'Roles'"
        :employee="employee"
        :dialogWidth="detailDialogWidth"
      />

      <ARTaskDetailsAsync
        v-else
        :employee="employee"
        :dialogWidth="detailDialogWidth"
      />
    </template>
  </KendoDetailPopupComponent>
</template>

<style src="../ar-organizational-structure.css" />
