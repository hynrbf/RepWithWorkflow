<script lang="ts">
import { defineComponent } from "vue";
import { storeToRefs } from "pinia";
import { useStaffStore } from "@/stores/useStaffStore";

const staffStore = useStaffStore();
const { staffs } = storeToRefs(staffStore);
const { fetchStaffsAsync } = staffStore;

export default defineComponent({
  props: {
    id: String,
  },
  data() {
    return {
      isLoading: false,
      staffs: [] as typeof staffs.value,
    };
  },
  computed: {
    options() {
      return this.staffs.map(({ id, name }) => ({
        label: name,
        value: id,
      }));
    },
  },
  async mounted() {
    this.isLoading = true;
    await fetchStaffsAsync();
    this.staffs = staffs.value;
    this.isLoading = false;
  },
});
</script>

<template>
  <KendoDropdownListComponent
      :id="id"
      :data-items="options"
      value-primitive
      :loading="isLoading"
  />
</template>