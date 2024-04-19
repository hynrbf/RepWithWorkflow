<script lang="ts">
import { defineComponent } from "vue";
import { useStaffStore } from "@/stores/useStaffStore";
import { mapState, mapActions } from "pinia";

export default defineComponent({
  props: {
    id: String,
  },
  data() {
    return {
      isLoading: false,
    };
  },
  computed: {
    ...mapState(useStaffStore, ["staffs"]),
    staff() {
      return this.staffs.find((staff) => staff.id === this.id);
    },
  },
  methods: {
    ...mapActions(useStaffStore, ["fetchStaffsAsync"]),
  },
  async mounted() {
    this.isLoading = true;
    await this.fetchStaffsAsync();
    this.isLoading = false;
  },
});
</script>

<template>
  <span>{{ staff?.name || "Undefined User" }}</span>
</template>