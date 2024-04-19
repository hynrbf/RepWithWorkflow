<script lang="ts">
import { defineComponent } from "vue";
import { container } from "tsyringe";
import {
  ICustomerService,
  ICustomerServiceInfo,
} from "@/infra/dependency-services/rest/forms-compliance/ICustomerService";
import { AppConstants } from "@/infra/AppConstants";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";

export default defineComponent({
  name: "ScheduleMeetingConfirmation",
  data() {
    return {
      customerService: container.resolve<ICustomerService>(
        ICustomerServiceInfo.name,
      ),
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      calendlyEmail: "",
      eventUuid: "",
      startTime: "",
      endTime: "",
      eventName: "",
    };
  },
  async created() {
    this.calendlyEmail = this.$route.query.invitee_email as string;
    this.eventUuid = this.$route.query.event_type_uuid as string;
    this.startTime = this.$route.query.event_start_time as string;
    this.endTime = this.$route.query.event_end_time as string;
    this.eventName = this.$route.query.event_type_name as string;

    await this.updateScheduleMeetingWithTrueEmailAsync();
  },
  methods: {
    async updateScheduleMeetingWithTrueEmailAsync(): Promise<boolean> {
      let emailFromLocal = localStorage.getItem(
        AppConstants.emailKey,
      ) as string;
      let epoch = this.helperService.dateStringToEpoch(this.startTime);
      return await this.customerService.checkAndUpdateScheduleMeetingWithCustomerEmail(
        this.calendlyEmail,
        emailFromLocal,
        this.eventUuid,
        epoch,
      );
    },
  },
});
</script>

<template>
  <div class="k-card shadow mx-4 my-4 card p-4">
    <p class="font-weight-700" style="font-size: 20px">
      Meeting schedule confirmed!
    </p>

    <div class="d-flex flex-column gap-2">
      <div class="d-flex">
        <Label class="col-2">Event Name</Label>
        <Label class="col"
          ><span class="font-weight-700">{{ eventName }}</span></Label
        >
      </div>

      <div class="d-flex">
        <Label class="col-2">Start Time</Label>
        <Label class="col"
          ><span class="font-weight-700">{{ startTime }}</span></Label
        >
      </div>

      <div class="d-flex">
        <Label class="col-2">End Time</Label>
        <Label class="col"
          ><span class="font-weight-700">{{ endTime }}</span></Label
        >
      </div>
    </div>
  </div>
</template>

<style scoped />