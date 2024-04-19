<script lang="ts">
import {defineComponent} from "vue";
import { useAlert } from "@/composables/useAlert";

export default defineComponent({
  inject: ["alertService"],
  methods: {
    alertWithProvideInject() {

      //ToDo. not working
      // const alertService = inject<useAlert>("alertService");
      //
      // if (!alertService) {
      //   return;
      // }

      this.alertService({
        title: "Confirm",
        content:
          "This action requires a Variation of Permission Application to the FCA. Are you sure you no longer wish to perform this activity?",
        confirmButtonText: "Confirm & Remove",
        confirmButtonThemeColor: "error",
        onConfirm: () => {
          alert("confirmed!!!");
        },
      });
    },
    alertWithComposable() {
      useAlert({
        title: "Confirm",
        content:
          "This action requires a Variation of Permission Application to the FCA. Are you sure you no longer wish to perform this activity?",
        confirmButtonText: "Confirm & Add",
        onConfirm: () => {
          alert("confirmed!!!");
        },
      });
    },
  },
});
</script>

<template>
  <div class="p-5">
    <KendoButton theme-color="error" @click.prevent="alertWithProvideInject">
      Alert with provide & inject
    </KendoButton>
    &nbsp;
    <KendoButton theme-color="primary" @click.prevent="alertWithComposable">
      Alert with composable
    </KendoButton>
  </div>
</template>