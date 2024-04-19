<script setup lang="ts">
import moment from "moment";
import { MediaMarketingOutlet } from "@/entities/media-marketing-outlet/MediaMarketingOutlet";

defineProps<{ outlet?: MediaMarketingOutlet }>();
</script>

<template>
  <ModalComponent width="380">
    <div class="text-center mb-4">
      <MediaLabelComponent
        :id="outlet?.platform"
        :icon-size="60"
        icon-only
        class="d-inline-flex mb-2"
      />

      <h4 class="font-size-xl">{{ outlet?.name }}</h4>
    </div>
    <div class="row mb-3">
      <div class="col text-center">
        <StatusLabelComponent status="approved" />
        <h6 class="font-weight-medium mt-2">0</h6>
      </div>
      <div class="col text-center">
        <StatusLabelComponent status="pending" />
        <h6 class="font-weight-medium mt-2">0</h6>
      </div>
      <div class="col text-center">
        <StatusLabelComponent status="rejected" />
        <h6 class="font-weight-medium mt-2">0</h6>
      </div>
    </div>
    <div class="border-top pt-4">
      <div class="mb-4 text-break">
        <div class="control-label mb-1 font-size-xs">
          <strong>Account URL</strong>
        </div>
        <a :href="outlet?.url" class="font-size-sm">
          {{ outlet?.url }}
        </a>
      </div>
      <div class="mb-4">
        <div class="control-label mb-1 font-size-xs">
          <strong>Owner</strong>
        </div>
        <StaffLabelComponent class="font-size-sm" :id="outlet?.owner" />
      </div>
      <div class="mb-4">
        <div class="control-label mb-1 font-size-xs">
          <strong>Most Recent Post</strong>
        </div>
        <DateTimeComponent v-if="outlet?.createdAt" :value="moment(outlet.createdAt * 1000).toDate()" class="is-text-nowrap" />
      </div>
      <div>
        <div class="control-label mb-1 font-size-xs">
          <strong>Most Recent Approval</strong>
        </div>
        <DateTimeComponent v-if="outlet?.updatedAt" :value="moment(outlet.updatedAt * 1000).toDate()" class="is-text-nowrap" />
      </div>
    </div>
  </ModalComponent>
</template>
