<script lang="ts">
import { defineComponent } from "vue";
import { Label } from "@progress/kendo-vue-labels";
import { StackLayout } from "@progress/kendo-vue-layout";

export default defineComponent({
  name: "KendoCopyRightComponent",
  components: { Label, StackLayout },
});
</script>

<template>
  <StackLayout
    orientation="horizontal"
    class="mt-4 p-4"
    :align="{ horizontal: 'center', vertical: 'middle' }"
  >
    <Label class="copyrights">2022 Copyrights © </Label>

    <Label class="comply-dex">ComplyDex</Label>
  </StackLayout>
</template>

<style scoped>
.copyrights {
  color: #898989;
  text-align: center;
  font-size: var(--font-size-sm);
  font-style: italic;
  font-weight: 300;
  line-height: 50px; /* 357.143% */
  letter-spacing: 1px;
}

.comply-dex {
  color: #160e83;
  font-size: var(--font-size-sm);
  font-style: italic;
  font-weight: 300;
  line-height: 50px;
  letter-spacing: 1px;
}
</style>