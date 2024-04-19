<script lang="ts">
import { Employee } from "@/entities/firm-details/Employee";
import { defineAsyncComponent, defineComponent } from "vue";
import KendoDetailPopupComponent from "@/components/KendoDetailPopupComponent.vue";
import DynamicAvatar from "@/components/DynamicAvatarComponent.vue";
import { IEmployeeTask } from "@/entities/org-structure/IEmployeeTask";
import { container } from "tsyringe";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";

export default defineComponent({
  name: "AROrganizationalStructureCard",
  components: {
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
    DynamicAvatar,
    KendoDetailPopupComponent,
  },
  props: {
    employeesValues: {
      type: Object as () => Employee[],
      default: () => [] as Employee[],
    },
  },
  data() {
    return {
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      isShowViewUserModal: false,
      isShowTasksDetailDialog: false,
      employee: new Employee(),
      detailDialogWidth: 380,
      employeeDetailDialogWidth: 750,
      customStyle: {
        flexBasis: "90px",
        height: "90px",
        margin: "0",
      },
    };
  },
  methods: {
    viewUser(id: string | undefined) {
      if (!id) {
        return;
      }

      this.isShowViewUserModal = true;
      this.employee = this.findEmployeeById(id);
    },

    emitEditEvent(id: string | undefined) {
      if (!id) {
        return;
      }

      this.employee = this.findEmployeeById(id);
      this.$emit("editUser", this.employee);
    },

    findEmployeeById(id: string) {
      return (
        this.employeesValues.find((employee) => employee.id === id) ??
        new Employee()
      );
    },

    onViewTasks(employee: Employee) {
      if (!employee) {
        return;
      }

      this.employee = employee;

      if (!this.employee.tasks?.length) {
        return;
      }

      this.isShowTasksDetailDialog = true;
    },

    isTaskOverdue(task: IEmployeeTask): boolean {
      const currentDate = this.helperService.getCurrentDateTimeInEpoch();
      return currentDate > task.dueDate;
    },

    formatDate(dateInEpoch: number): string {
      return (
        this.helperService
          .convertEpochToDateTime(dateInEpoch)
          ?.toLocaleDateString("en-GB") ?? ""
      );
    },

    getOverdueDays(dateInEpoch: number): number {
      const date = new Date();
      // set to midnight
      date.setMilliseconds(0);
      date.setSeconds(0);
      date.setMinutes(0);
      date.setHours(0);

      const currentDate = date.getTime();
      const taskDueDate =
        this.helperService.convertEpochToDateTime(dateInEpoch)?.getTime() ?? 0;
      // Convert the difference to days
      return (currentDate - taskDueDate) / (1000 * 60 * 60 * 24);
    },
  },
});
</script>

<template>
  <div class="cardview-grid">
    <div v-for="item in employeesValues" :key="`card-${item.id}`">
      <KendoCard class="cardview-style">
        <KendoCardBody>
          <!-- Avatar Section -->
          <DynamicAvatarComponent
            rounded="full"
            type="image"
            :size="'large'"
            :imageSrc="item.img_id"
            :imageAlt="item.img_id"
            :text="`${item?.firstName} ${item?.lastName}`"
            :subText="item?.primaryRole?.name"
            :customStyle="customStyle"
          >
            <template #text="{ text }">
              <Label class="employee-name-text">{{ text }}</Label>
            </template>

            <template #subtext="{ subText }">
              <Label
                class="employee-role-text"
                :class="
                  item.primaryRole?.isFcaAuthorised
                    ? 'authorised'
                    : item.primaryRole?.isModified
                    ? 'modified'
                    : ''
                "
              >
                {{ subText }}
              </Label>
            </template>
          </DynamicAvatarComponent>

          <!-- Email Address and Contact Number Section -->
          <div class="email-contact-section">
            <!-- Email Address -->
            <div class="email-section">
              <div class="font-styling">Email Address</div>

              <div class="item-font-styling" id="cardview-email">
                {{ item?.email }}
              </div>
            </div>

            <!-- Divider -->
            <div class="divider" />

            <!-- Contact Number -->
            <div class="contact-section">
              <div class="font-styling">Contact Number</div>

              <div
                class="item-font-styling"
                style="text-align: center"
                id="cardview-contact"
              >
                {{ item.contactNumber?.dialCode }}
                {{ item.contactNumber?.number }}
              </div>
            </div>
          </div>

          <!-- View Task and Edit Section -->
          <div class="view-task-section">
            <KendoButton
              class="button-styling"
              :class="
                item?.employmentStatus === 'Active'
                  ? 'good-status-btn'
                  : item?.employmentStatus === 'Onboarding'
                  ? 'warning-status-btn'
                  : 'bad-status-btn'
              "
              type="button"
              fill-mode="outline"
              theme-color="primary"
              @click="onViewTasks(item)"
            >
              View Tasks
            </KendoButton>

            <KendoButton
              type="button"
              size="small"
              rounded="full"
              shape="square"
              theme-color="light"
              title="View"
              class="ActionButton"
              @click="viewUser(item.id)"
            >
              <IconComponent symbol="eye" size="20" />
            </KendoButton>

            <KendoButton
              type="button"
              size="small"
              rounded="full"
              shape="square"
              theme-color="light"
              title="Pen"
              class="ActionButton"
              @click="emitEditEvent(item.id)"
            >
              <IconComponent symbol="edit-pen" size="20" />
            </KendoButton>
          </div>
        </KendoCardBody>
      </KendoCard>
    </div>
  </div>

  <KendoDetailPopupComponent
    v-if="isShowViewUserModal"
    :width="employeeDetailDialogWidth"
    @close="isShowViewUserModal = false"
  >
    <template #content>
      <ARUserDetailAsync
        :employee="employee"
        :dialogWidth="employeeDetailDialogWidth"
      />
    </template>
  </KendoDetailPopupComponent>

  <KendoDetailPopupComponent
    v-if="isShowTasksDetailDialog"
    :width="detailDialogWidth"
    title="Tasks"
    @close="isShowTasksDetailDialog = false"
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

      <ARTaskDetailsAsync :employee="employee" :dialogWidth="detailDialogWidth" />
    </template>
  </KendoDetailPopupComponent>
</template>

<style scoped lang="scss">
.cardview-grid {
  display: flex;
  flex-wrap: wrap;
  gap: 15px;
  width: 1070px;
  margin: 0 auto;
}

.cardview-style {
  height: 220px;
  width: 346.5px;
  border-radius: 8px;
  border: 0.5px solid var(--content-content-07);
  box-shadow: 0 1px 0 0 #0073e6 inset;
}

.employee-name-text {
  margin-bottom: -6px;
  width: 195px;
  margin-left: 8px;
  font-size: 20px;
  line-height: 28px;
  font-weight: 700;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.employee-role-text {
  width: 195px;
  margin-left: 8px;
  font-weight: 400;
  font-size: var(--font-size-sm);
  line-height: 17.5px;

  overflow: hidden;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  text-overflow: ellipsis;
}

.email-contact-section {
  display: flex;
  width: 316.5px;
  height: 40px;
  margin-top: 15px;
}

.email-section,
.contact-section {
  display: flex;
  flex-direction: column;
  height: 40px;
  width: 143.25px;
  padding: 0 7.63px;
}

.font-styling {
  font-size: var(--font-size-xs);
  color: var(--color-content-50);
  font-weight: 500;
  line-height: 16px;
  text-align: center;
}

.item-font-styling {
  font-size: var(--font-size-sm);
  font-weight: 600;
  line-height: 17.5px;
  text-decoration-line: underline;
  color: var(--brand-color-brand-primary);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;

  &#cardview-contact {
    text-decoration-line: none !important;
    font-weight: 400 !important;
    color: var(--color-black) !important;
  }
}

.divider {
  background: var(--text-text-disabled);
  width: 0.5px;
  height: 30px;
  margin: 2px 14px 0 14px;
}

.view-task-section {
  display: flex;
  width: 316.5px;
  height: 30px;
  margin-top: 6px;
  gap: 10px;
}

.button-styling {
  height: 30px;
  padding: 4px 12px;
  border-radius: 100px;
  background-color: transparent;
  width: 236.5px;
  font-size: var(--font-size-xs);
  font-weight: 600;
  line-height: 16px;
}

.employee-active {
  border: 1px solid var(--success-700) !important;
}

.employee-email-text {
  font-size: var(--font-size-sm);
  font-weight: 600;
  line-height: 17.5px;
}

.ActionButton * {
  color: var(--brand-color-brand-primary);
}

.authorised {
  color: var(--color-success-700);
}

.modified {
  color: var(--color-warning-700);
}

.k-card-body {
  padding-block: 20px;
  padding-inline: 15px;
}
</style>