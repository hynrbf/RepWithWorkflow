<script lang="ts">
import { defineComponent } from "vue";
import KendoCompaniesAutoCompleteComponent from "@/components/form-fields/KendoCompaniesAutoCompleteComponent.vue";
import { StackLayout } from "@progress/kendo-vue-layout";

export default defineComponent({
  name: "TestOnly",
  components: { StackLayout, KendoCompaniesAutoCompleteComponent },
  data() {
    return {
      urlVal: "",
      inputVal: "",
    };
  },
  mounted() {
    // this.companyInternal.firmName = "Richde";
  },
  methods: {
    async handleSubmitAsync() {},
  },
});
</script>

<template>
  <div style="padding: 50px">
    <Form @submit="handleSubmitAsync">
      <FormElement>
        <StackLayout orientation="vertical">
          <p>Url Type</p>
          <KendoGenericInputComponent
            class="col-md-4"
            type="url"
            :name="`url`"
            label="Website"
            :isRequired="true"
            :value="urlVal"
            @onValueChange="(value: string) => (urlVal = value)"
          />

          <p>Non URL</p>
          <KendoGenericInputComponent
            class="col-md-4"
            :name="`nonUrl`"
            label="Any input"
            :isRequired="false"
            :value="inputVal"
            @onValueChange="(value: string) => (inputVal = value)"
          />
        </StackLayout>
      </FormElement>
    </Form>
  </div>
</template>

<style scoped />