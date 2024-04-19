<script setup lang="ts">
import { ProvidersTaskDetails } from "@/entities/providers-and-introducers/ProvidersTaskDetails";
import { toRefs, ref } from "vue";

const props = defineProps<{
  tasks: ProvidersTaskDetails[];
  providerName: string;
}>();

const { tasks, providerName } = toRefs(props);
const modalElement = ref();
</script>

<template>
  <ModalComponent ref="modalElement" :title="`Tasks`">
    <div class="d-flex modal-content">
      <div class="vstack">
        <DynamicAvatarComponent
          class="vstack"
          type="image"
          rounded="full"
          size="large"
        />
        <div class="d-flex flex-row vstack align-center">
          <text class="provider-name">{{ providerName }}</text>
        </div>
        <div v-for="task of tasks">
          <div class="hstack mt-3">
            <div>[{{ task.name }}]</div>
            <PillComponent
              :themeColor="'success-tint'"
              style="margin-left: 20px"
            >
              Due by {{ task.dueDate }}
            </PillComponent>
          </div>
          <div class="mt-2">
            {{ task.description }}
          </div>
          <DividerComponent
            :height="1"
            :width="550"
            :spacing="0"
            style="margin-top: 20px"
          />
        </div>
      </div>
    </div>
  </ModalComponent>
</template>

<style scoped>
.modal-content {
  flex-direction: column;
  align-items: flex-start;
  justify-content: flex-start;
}

.provider-name {
  color: var(--color-black);
  text-align: center;
  font-size: var(--font-size-xl);
  font-weight: var(--font-weight-bold);
  font-style: normal;
  line-height: 28px;
}

.align-center {
  align-items: center;
  justify-content: center;
}
</style>