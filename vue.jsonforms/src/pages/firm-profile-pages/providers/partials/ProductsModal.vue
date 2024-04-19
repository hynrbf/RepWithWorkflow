<script setup lang="ts">
import { ProvidersProductDetails } from "@/entities/providers-and-introducers/ProvidersProductDetails";
import { toRefs, ref } from "vue";

const props = defineProps<{
  products: ProvidersProductDetails[];
  providerName: string;
}>();

const { products, providerName } = toRefs(props);
const modalElement = ref();
</script>

<template>
  <ModalComponent ref="modalElement" :title="`Products`">
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
        <div v-for="product of products">
          <div class="hstack mt-3">
            <div>{{ product.name }}</div>
          </div>
          <div class="mt-2">
            {{ product.status }}
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