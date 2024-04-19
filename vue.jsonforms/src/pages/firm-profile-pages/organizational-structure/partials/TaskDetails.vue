<script lang="ts">
import { defineComponent } from "vue";
import { Employee } from "@/entities/firm-details/Employee";
import { IEmployeeTask } from "@/entities/org-structure/IEmployeeTask";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import { container } from "tsyringe";

export default defineComponent({
  name: "TaskDetails",
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
  data() {
    return {
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
    };
  },
  methods: {
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
  <div class="d-flex flex-column gap-2 mt-2">
    <StackLayout
      v-for="(task, index) of employee.tasks"
      orientation="vertical"
      :gap="10"
      :key="task.name"
    >
      <DividerComponent
        v-if="index > 0"
        class="my-1"
        :height="1"
        :width="dialogWidth - 60"
        :spacing="0"
      />

      <StackLayout class="task-detail-dialog">
        <Label class="col task-name">{{ task.name }}</Label>

        <div class="col d-flex flex-grow-1 justify-content-center">
          <PillComponent
            class="task-due-text"
            :class="
              isTaskOverdue(task) ? 'bad-status-pill' : 'good-status-pill'
            "
          >
            <Label v-if="isTaskOverdue(task)"
              >Overdue by {{ getOverdueDays(task.dueDate) }} days</Label
            >

            <Label v-else>Due by {{ formatDate(task.dueDate) }}</Label>
          </PillComponent>
        </div>
      </StackLayout>

      <Label class="detail-dialog-text" style="margin-bottom: 10px">{{
        task.description
      }}</Label>
    </StackLayout>
  </div>
</template>

<style scoped>
.task-detail-dialog {
  margin-top: 10px;
  display: flex;
  flex-direction: row;
  align-items: start;
}

.task-detail-dialog .task-name {
  font-size: var(--font-size-sm);
  font-weight: 600;
  line-height: 17.5px;
}

.task-due-text {
  flex-direction: row;
  padding: 2px 10px;
  border-radius: 150px;
  font-size: var(--font-size-sm);
  font-weight: 500;
  line-height: 18px;
  letter-spacing: 0;
  text-align: center;
}
</style>
