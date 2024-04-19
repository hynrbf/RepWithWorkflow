<script setup lang="ts">
import { ref, inject, watch, toRefs } from "vue";
import setWith from "lodash/setWith";
import { onMounted } from "vue";

const props = defineProps<{ allowObjectValues?: boolean }>();

const { allowObjectValues } = toRefs(props);

const formElement = ref();
const kendoForm = inject<any>("kendoForm", { default: {} });

const submit = () => {
  formElement?.value?.$el?.requestSubmit();
};

const reset = () => {
  kendoForm.onFormReset();
};

const setValues = (values: Record<string, unknown>) => {
  Object.keys(values).forEach((key) => {
    kendoForm.onChange(key, { value: values[key] });
  });
};

const updateValue = (name: string, value: any) =>
  kendoForm.onChange(name, { value });

// create extra values for objects when used dotted names
const makeObjectValues = (values: Record<string, unknown>) => {
  Object.keys(values).forEach((key) => {
    if (!key.includes(".")) {
      return;
    }

    const names = key.split(".");
    const currentName = names.shift();
    const currentValue = values[`${currentName}`];
    kendoForm.onChange(currentName, {
      value: {
        ...setWith(currentValue || {}, names, values[key], Object),
      },
    });
  });
};

watch(
  () => kendoForm.values,
  (values) => {
    if (allowObjectValues.value) {
      makeObjectValues(values);
    }
  },
);

onMounted(() => {
  //do nothing
});

defineExpose({
  submit,
  reset,
  setValues,
});
</script>

<template>
  <KendoFormElement ref="formElement">
    <slot
      :form="kendoForm"
      :submit="submit"
      :reset="reset"
      :update-value="updateValue"
    ></slot>
  </KendoFormElement>
</template>
