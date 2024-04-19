<script setup lang="ts">
import { toRefs, computed, ref, inject, onMounted } from "vue";
import { cloneDeep } from "lodash";
import { Emitter, EventType } from "mitt";
import { AppConstants } from "@/infra/AppConstants";

const props = withDefaults(
  defineProps<{
    items: Array<string | Record<string, unknown>>;
    limit?: number;
    popupTitle?: string;
    isShowAll?: boolean;
  }>(),
  { items: () => [], limit: 3, isShowAll: true },
);

const { items, limit, isShowAll } = toRefs(props);

const visibleItems = computed(() =>
  cloneDeep(items.value).splice(0, limit.value),
);
const popupItems = computed(() =>
  isShowAll.value
    ? cloneDeep(items.value)
    : cloneDeep(items.value).splice(limit.value),
);

const remainingItemsCount =
  cloneDeep(items.value).splice(limit.value)?.length ?? 0;

const isPopup = ref(false);

const eventBus = inject("$eventBusService") as Emitter<
  Record<EventType, unknown>
>;

onMounted(() => {
  eventBus.on(AppConstants.closeSnipItemsPopupEvent, () => {
    isPopup.value = false;
  });
});
</script>

<template>
  <div class="SnipItems">
    <slot name="visible-items" :items="visibleItems">
      <div v-for="(item, index) in visibleItems" :key="`snip-item-${index}`">
        <slot name="item" :item="item" :index="index">
          {{ item }}
        </slot>
      </div>
    </slot>

    <a
      v-show="remainingItemsCount"
      href="javascript:;"
      ref="snipButton"
      @click.prevent="isPopup = !isPopup"
    >
      <strong>+{{ remainingItemsCount }}</strong>
    </a>

    <KendoPopup
      anchor="snipButton"
      popup-class="SnipPopup"
      :show="isPopup"
      :anchor-align="{ horizontal: 'right', vertical: 'bottom' }"
      :popup-align="{ horizontal: 'right', vertical: 'top' }"
    >
      <div class="SnipPopup-head">
        <span v-if="popupTitle" class="SnipPopup-title">
          {{ popupTitle }}
        </span>

        <KendoButton
          class="SnipPopup-button"
          size="small"
          @click.prevent="isPopup = false"
        >
          <IconComponent symbol="clear" size="15" />
        </KendoButton>
      </div>

      <div class="SnipPopup-body">
        <slot name="popup-body">
          <div
            v-for="(item, index) in popupItems"
            :key="`snip-body-item-${index}`"
          >
            <slot name="popup-item" :item="item" :index="index">
              {{ item }}
            </slot>
          </div>
        </slot>
      </div>
    </KendoPopup>
  </div>
</template>

<style scoped lang="scss">
.SnipItems {
  display: flex;
  align-items: center;
  flex-wrap: nowrap;
  gap: 5px;
}

:global(.SnipPopup) {
  border-radius: 8px;
}

:global(.SnipPopup-body) {
  padding: 10px 15px 15px;
  width: 300px;
  max-width: 100%;
}

:global(.SnipPopup-head) {
  display: flex;
  padding: 15px 15px 0;
}

:global(.SnipPopup-button) {
  padding: 0 !important;
  border: 0 !important;
  margin: 0 0 0 auto !important;
  background: transparent !important;
}

:global(.SnipPopup-title) {
  font-size: var(--font-size-lg);
  font-weight: var(--font-weight-semi-bold);
}
</style>
