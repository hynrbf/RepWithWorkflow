<script lang="ts">
import { defineComponent } from "vue";
import { usePageLifeCycleStore } from "@/stores/progress-bar/usePageLifeCycleStore";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";
import { AppConstants } from "@/infra/AppConstants";
import { container } from "tsyringe";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";

export default defineComponent({
  name: "KendoProgressBarComponent",
  props: {
    progressBarValue: {
      type: Number,
      default: 0,
    },
    routeName: {
      type: String,
      default: "",
    },
  },
  data() {
    return {
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
    };
  },
  computed: {
    //The value is computed as follows: let say you have 10 components total in a page
    //2 of them got validation error like empty, or incorrect email format, etc
    //so the value of progressBarValueComputed is 80%
    //we count the components that are RENDERED in the DOM only minus how many got validation error
    progressBarValueComputed(): number {
      if (!this.routeName) {
        throw new Error("routeName is required for ProgressBarComponent");
      }

      const pageComponentErrorStore = usePageComponentValidationValueStore();
      const pageLifeCycleStore = usePageLifeCycleStore();
      const currentPageLifeCycle = pageLifeCycleStore.currentLifeCycleName;
      const componentPlusValidationErrorValue =
        pageComponentErrorStore.componentThenValidationValue;

      if (currentPageLifeCycle !== AppConstants.pageLifeCycleNameMounted) {
        return 0;
      }

      let count = 0;
      let errorCount = 0;

      componentPlusValidationErrorValue.forEach((entry) => {
        Object.keys(entry).forEach((key) => {
          if (!key.toLowerCase().includes(this.routeName.toLowerCase())) {
            return;
          }

          const value = entry[key];
          count++;

          //if the value of component validation is not empty, then there is an error
          //so, we count it
          if (!value) {
            return;
          }

          errorCount++;
        });
      });

      return ((count - errorCount) / count) * 100;
    },
  },
});
</script>

<template>
  <div class="FormProgress">
    <span class="FormProgress-label"
      >{{
        helperService.roundOffToNearestWholeNumber(progressBarValueComputed)
      }}%</span
    >

    <ProgressBar
      class="FormProgress-progressBar"
      :value="progressBarValueComputed"
      label=" "
    />
  </div>
</template>

<style scoped lang="scss">
.FormProgress {
  max-width: 360px;
  text-align: right;
  margin-right: 7px;

  &-progressBar {
    height: 4px;
  }

  &-label {
    font-weight: var(--font-weight-semi-bold);
  }
}
</style>