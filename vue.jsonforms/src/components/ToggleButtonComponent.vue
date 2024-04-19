<script lang="ts">
import { defineComponent, inject } from "vue";
import { Emitter, EventType } from "mitt";
import { AppConstants } from "@/infra/AppConstants";

export default defineComponent({
  props: {
    modelValue: {
      type: [Boolean, String, Number],
      default: undefined,
    },
    onLabel: {
      type: String,
      default: "Yes",
    },
    offLabel: {
      type: String,
      default: "No",
    },
    onValue: {
      type: [Boolean, String, Number],
      default: true,
    },
    offValue: {
      type: [Boolean, String, Number],
      default: false,
    },
    text: String,
    reverse: Boolean,
    wide: Boolean,
    disabled: Boolean,
    hasDefaultModelValue: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      AppConstants,
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
    };
  },
  watch: {
    disabled: {
      handler(value) {
        if (this.hasDefaultModelValue) {
          this.$emit("update:modelValue", this.modelValue);
          return;
        }

        if (value) {
          this.$emit("update:modelValue", null);
        }
      },
      immediate: true,
    },
  },
});
</script>

<template>
  <div
    :class="[
      'ToggleButton',
      reverse && 'ToggleButton--reverse',
      disabled && 'ToggleButton--disabled',
      wide && 'w-100',
    ]"
  >
    <KendoButtonGroup>
      <KendoButton
        type="button"
        theme-color="primary"
        :fill-mode="modelValue === onValue ? 'solid' : 'outline'"
        :disabled="disabled"
        @click="
          () => {
            $emit('update:modelValue', onValue);
            eventBus.emit(AppConstants.formFieldChangedEvent);
            eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
          }
        "
      >
        {{ onLabel }}
      </KendoButton>
      <KendoButton
        type="button"
        theme-color="primary"
        :fill-mode="modelValue === offValue ? 'solid' : 'outline'"
        :disabled="disabled"
        @click="
          () => {
            $emit('update:modelValue', offValue);
            eventBus.emit(AppConstants.formFieldChangedEvent);
            eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
          }
        "
      >
        {{ offLabel }}
      </KendoButton>
    </KendoButtonGroup>
    <span class="ToggleButton-text" v-if="text || $slots.text">
      <slot name="text">{{ text }}</slot>
    </span>
  </div>
</template>

<style scoped lang="scss">
.ToggleButton {
  display: inline-flex;
  align-items: center;
  gap: 10px;

  &--reverse {
    flex-direction: row-reverse;
  }

  &-text {
    flex: 1;
  }

  :deep(.k-button:first-child) {
    border-top-left-radius: 8px;
    border-bottom-left-radius: 8px;
  }

  :deep(.k-button:last-child) {
    border-top-right-radius: 8px;
    border-bottom-right-radius: 8px;
  }
}
</style>