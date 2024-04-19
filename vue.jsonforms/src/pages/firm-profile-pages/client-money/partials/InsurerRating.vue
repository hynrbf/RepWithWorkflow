<script setup lang="ts">
import { ref, onMounted, toRefs, computed, watchEffect } from "vue";
import { useColorGenerator } from "@/composables/useColorGenerator";

const props = withDefaults(
  defineProps<{
    modelValue: string;
    id?: string;
    label?: string;
    name?: string;
    hint?: string;
    disabled?: boolean;
    isRequired?: boolean;
    maxWidth?: number;
    unrated?: boolean;
  }>(),
  {
    modelValue: "",
    disabled: false,
    isRequired: true,
  }
);

const emits = defineEmits<{
  (event: "update:modelValue", value: string): void;
}>();

const { modelValue, unrated } = toRefs(props);

const fitchRating: string[] = [
  "C",
  "CC-",
  "CC",
  "CC+",
  "CCC",
  "B-",
  "B",
  "B+",
  "BB-",
  "BB",
  "BB+",
  "BBB-",
  "BBB",
  "BBB+",
  "A-",
  "A",
  "A+",
  "AA-",
  "AA",
  "AA+",
  "AAA",
];

const { generateColor } = useColorGenerator();

const ratingElement = ref<HTMLElement>();
const labelElement = ref();
const numberValue = ref(1);
const textValue = computed(() => numberToText(numberValue.value, fitchRating));

const textToNumber = (value: string, list: string[]) => {
  const interval = 100 / list.length;
  return list.indexOf(value) * interval;
};

const numberToText = (value: number, list: string[]) => {
  const interval = 100 / list.length;
  const numbers = Object.keys(list) as unknown as number[];
  const closestNumber = numbers.find((number) => number * interval >= value);
  const index = (closestNumber !== undefined ? closestNumber : list.length) - 1;
  return list[index];
};

const handleChange = (event: any, onChange?: (event: any) => void) => {
  numberValue.value = event.value;
  emits("update:modelValue", textValue.value);

  // update kendo form values
  event.value = textValue.value;
  onChange?.(event);
};

const initElement = () => {
  if (!ratingElement.value || !labelElement.value) {
    return;
  }

  const dragHandle = ratingElement.value.querySelector(".k-draghandle");

  if (!dragHandle) {
    return;
  }

  dragHandle.appendChild(labelElement.value);
};

const isUnrated = ref(unrated.value);

watchEffect(() => {
  if (modelValue.value) {
    numberValue.value = textToNumber(modelValue.value, fitchRating);
  }

  isUnrated.value = unrated.value
});

onMounted(() => {
  initElement();
});
</script>

<template>
  <KendoField :id="id" :name="name" component="template">
    <template v-slot:template="{ props }">
      <KendoFieldWrapper :class="props.class">
        <KendoLabel
          v-if="label"
          :editor-id="id"
          :disabled="disabled"
          :valid="props.valid"
          :editor-valid="props.valid"
          class="control-label mb-2"
        >
          {{ label }}
          <span v-if="!isRequired" class="fineprint ms-1">(Optional)</span>

          <span class="ms-auto">
            <KendoCheckbox v-model="isUnrated" label="Unrated" />
          </span>
        </KendoLabel>

        <div
          ref="ratingElement"
          :class="['InsurerRating', isUnrated && 'InsurerRating--disabled']"
          :style="{
            '--current-color': generateColor(numberValue),
            maxWidth: maxWidth ? `${maxWidth}px` : undefined,
          }"
        >
          <span>Poor</span>
          <KendoSlider
            :value="numberValue"
            :min="1"
            :max="100"
            class="w-100"
            :disabled="isUnrated"
            @change="handleChange($event, props.onChange)"
          ></KendoSlider>
          <label ref="labelElement" class="InsurerRating-label">
            {{ textValue }}
          </label>
          <span>Good</span>
        </div>

        <KendoError v-if="props.touched && props.validationMessage">
          {{ props.validationMessage }}
        </KendoError>

        <KendoHint v-else>
          {{ hint }}
        </KendoHint>
      </KendoFieldWrapper>
    </template>
  </KendoField>
</template>

<style scoped lang="scss">
.InsurerRating {
  display: flex;
  align-items: center;
  margin-top: 15px;
  margin-left: auto;
  margin-right: auto;

  :deep(.k-slider) {
    .k-slider-track {
      background: linear-gradient(to right, #ff0000, #00ff00);
    }
    .k-slider-selection {
      background-color: transparent;
    }
    .k-draghandle {
      width: 20px;
      height: 20px;
      background-color: var(--color-white);
      border-width: 2px;
      border-color: var(--current-color);
    }
  }

  &-label {
    background-color: var(--current-color);
    position: absolute;
    left: 50%;
    transform: translateX(-50%);
    top: 18px;
    font-size: var(--font-size-xs);
    padding: 5px;
    border-radius: 8px;
    min-width: 30px;
    text-align: center;
    pointer-events: none;
    color: var(--color-primary);
  }

  &--disabled {
    .InsurerRating-label {
      display: none;
    }
    :deep(.k-draghandle) {
      border-color: var(--color-neutral) !important;
      opacity: 0;
    }
    :deep(.k-slider-track) {
      background: var(--color-neutral) !important;
    }
  }
}
</style>
