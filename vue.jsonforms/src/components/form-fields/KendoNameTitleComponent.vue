<script lang="ts">
import { defineComponent } from "vue";
import StaticList from "@/infra/StaticListService";

export default defineComponent({
  props: {
    id: String
  },
  data() {
    return {
      options: StaticList.getTitles().map((title) => ({
        label: title,
        value: title,
      })) as Record<string, string>[],
    };
  },
  methods: {
    addCustom(value: string, attachValue: () => void) {
      this.options = [
        ...this.options,
        {
          label: value,
          value,
          class: "is-hidden",
        },
      ];
      this.$nextTick(() => {
        attachValue();
      });
    },
  },
  mounted() {
    //just make sure that the icon of Kendo is removed because we replaced it with our own image icon
    let pickerButtonIcon = document.querySelector(
      "span.k-svg-icon.k-svg-i-caret-alt-down",
    ) as HTMLSpanElement;

    if (!pickerButtonIcon) {
      return;
    }

    if (pickerButtonIcon.classList?.length < 1 ?? true) {
      return;
    }

    pickerButtonIcon.classList.remove("k-svg-icon");
    pickerButtonIcon.classList.remove("k-svg-i-caret-alt-down");

    pickerButtonIcon.classList.add("k-icon");
    pickerButtonIcon.classList.add("k-i-chevron-down");
    pickerButtonIcon.style.scale = "1.25"; // width and height = 20px
    pickerButtonIcon.style.color = "var(--content-content-05)";
  },
});
</script>

<template>
  <KendoDropdownListComponent
      :id="id"
      :data-items="options"
      value-primitive
      :addable="true"
      @add-custom="addCustom"
  />
</template>

<style>
/* ToDo. Move to global states */
.is-hidden {
  display: none !important;
}
</style>
