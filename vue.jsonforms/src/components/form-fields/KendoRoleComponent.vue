<script lang="ts">
import { defineComponent } from "vue";
import StaticList from "@/infra/StaticListService";

export default defineComponent({
  props: {
    id: String,
  },
  data() {
    return {
      options: StaticList.getRoles().map((role) => ({
        label: role,
        value: role,
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