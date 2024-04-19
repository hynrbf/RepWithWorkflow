<script lang="ts">
import { defineComponent } from "vue";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";
import { usePageLifeCycleStore } from "@/stores/progress-bar/usePageLifeCycleStore";
import { useOnboardingCompletionPercentageStore } from "@/stores/progress-bar/useOnboardingCompletionPercentageStore";
import { AppConstants } from "@/infra/AppConstants";
import { container } from "tsyringe";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";

export default defineComponent({
  name: "SideMenuProgressTextComponent",
  props: {
    routeName: {
      type: String,
      default: "",
    },
    isActive: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
    };
  },
  computed: {
    //rules is the same with KendoProgressBarComponent
    progressBarValueComputed(): number {
      if (!this.routeName) {
        throw new Error(
          "routeName is required for SideMenuProgressTextComponent",
        );
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

      const computedValue = ((count - errorCount) / count) * 100;
      this.updateValue(
        this.routeName,
        this.helperService.roundOffToNearestWholeNumber(computedValue),
      );
      return computedValue;
    },
  },
  setup() {
    const onboardingCompletionPercentageStore =
      useOnboardingCompletionPercentageStore();
    const { getValue, updateValue } = onboardingCompletionPercentageStore;
    return {
      getValue,
      updateValue,
    };
  },
  methods: {
    getMenuStatusColor(percent: number) {
      const color1 = "#FFB700";
      const color2 = "#39AC73";

      if (percent < 1) {
        return "transparent";
      }

      return percent < 100 ? color1 : color2;

      // ToDo. Remove if not necessary
      // const { hexToRgb, rgbToHex } = useColorGenerator();
      // const color1Rgb = hexToRgb(color1);
      // const color2Rgb = hexToRgb(color2);
      // return rgbToHex(
      //   Math.round(color1Rgb.r + (color2Rgb.r - color1Rgb.r) * (percent / 100)),
      //   Math.round(color1Rgb.g + (color2Rgb.g - color1Rgb.g) * (percent / 100)),
      //   Math.round(color1Rgb.b + (color2Rgb.b - color1Rgb.b) * (percent / 100))
      // );
    },
  },
});
</script>

<template>
  <span
    v-if="
      isActive &&
      helperService.roundOffToNearestWholeNumber(progressBarValueComputed) > 0
    "
    class="MainDrawer-menuStatus"
    :style="{
      backgroundColor: getMenuStatusColor(
        helperService.roundOffToNearestWholeNumber(progressBarValueComputed),
      ),
    }"
    >{{
      helperService.roundOffToNearestWholeNumber(progressBarValueComputed)
    }}%</span
  >

  <span
    v-else
    class="MainDrawer-menuStatus"
    :style="{
      backgroundColor: getMenuStatusColor(getValue(routeName)),
    }"
    >{{ getValue(routeName) > 0 ? `${getValue(routeName)}%` : "" }}</span
  >
</template>

<style scoped lang="scss">
.MainDrawer {
  $root: &;

  &.k-drawer-expanded {
    #{$root}-menuStatus {
      display: block;
    }
  }
}
</style>