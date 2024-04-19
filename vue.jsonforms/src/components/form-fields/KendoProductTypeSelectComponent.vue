<script lang="ts">
import { defineComponent } from "vue";
import DataItemModel from "@/components/models/DataItemModel";
import { AppConstants } from "@/infra/AppConstants";
import { CustomerProduct } from "@/entities/CustomerProduct";

export default defineComponent({
  name: "KendoProductTypeSelectComponent",
  props: {
    id: String,
    pageName: {
      type: String,
      default: "",
    },

    //these both should be used for reactive data,
    //otherwise the validation of Kendo behaves incorrectly
    isValueReactive: {
      type: Boolean,
      default: false,
    },
    isDataLoadedCompletely: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      categories: [
        "Mortgage Broking",
        "Mortgage Lending",
        "Protection Broking",
        "General Insurance Broking",
        "Protection Broking",
        "General Insurance Broking",
        "General Insurance Underwriting",
        "Consumer Credit",
        "Investments",
        "Banking",
      ],
    };
  },
  computed: {
    dataItems(): DataItemModel[] {
      let customerProducts: CustomerProduct[] = [];

      const productsLocalJson = localStorage.getItem(
        AppConstants.customerProductsKey,
      );
      if (productsLocalJson) {
        customerProducts = JSON.parse(productsLocalJson) as CustomerProduct[];
      }
      let customerPageProduct =
        customerProducts.find((cp) => cp.pageName == this.pageName) ??
        new CustomerProduct();

      return customerPageProduct.categories.map((category) => {
        const items = category.products.map((item) => {
          return {
            label: item.displayText,
            value: `${category.name} - ${item.name}`,
          } as DataItemModel;
        });
        return {
          label: category.displayText,
          value: category.name,
          items: items,
        };
      });
    },
  },
});
</script>

<template>
  <KendoMultiSelectTreeComponent
    :id="id"
    :data-items="dataItems"
    :is-data-loaded-completely="isDataLoadedCompletely"
    :isValueReactive="isValueReactive"
    value-primitive
  />
</template>