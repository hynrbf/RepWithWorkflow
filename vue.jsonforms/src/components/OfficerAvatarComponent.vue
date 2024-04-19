<script lang="ts">
import { defineComponent } from "vue";
import { useStaffStore } from "@/stores/useStaffStore";
import { mapActions, mapState } from "pinia";

export default defineComponent({
  name: "OfficerAvatarComponent",
  props: {
    id: {
      type: String,
      default: "",
    },
  },
  data() {
    return {
      isFetching: false,
    };
  },
  computed: {
    ...mapState(useStaffStore, ["isFetchingStaffs"]),
    text() {
      const staff = this.getStaff(this.id);

      if (staff) {
        return staff.name;
      }

      return "Undefined User";
    },
  },
  methods: {
    ...mapActions(useStaffStore, ["getStaff", "fetchStaffsAsync"]),
    async fetchAsync() {
      this.isFetching = true;
      await this.fetchStaffsAsync();
      this.isFetching = false;
    }
  },
  async mounted() {
    if (!this.id || this.getStaff(this.id) || this.isFetchingStaffs) {
      return;
    }
    this.fetchAsync();
  },
});
</script>

<template>
  <DynamicAvatarComponent :text="text" />
</template>
