<script setup lang="ts">
import { computed, toRefs, ref } from "vue";
import StaticList from "@/infra/StaticListService";
import {AppConstants} from "@/infra/AppConstants";

interface TotalProjectAnnualIncome {
  currency: string;
  amount: number;
  symbol: string;
}

const props = withDefaults(
  defineProps<{
    modelValue: {
      productOrService: string;
      totalProjectAnnualIncome: TotalProjectAnnualIncome;
    }[];
  }>(),
  {
    modelValue: () => [],
  }
);

const emits = defineEmits<{
  (
    event: "update:modelValue",
    value: {
      productOrService: string;
      totalProjectAnnualIncome: TotalProjectAnnualIncome;
    }[]
  ): void;
}>();

const { modelValue } = toRefs(props);

const ProductAndServicesOptions = computed(() =>
  StaticList.getProductsAndServices().map((item) => ({ label: item, value: item }))
);

const deleteItem = (index: number) => {
  emits(
    "update:modelValue",
    modelValue.value.filter((_v, i) => index !== i)
  );
};

const newItem = ref({
  productOrService: "",
  totalProjectAnnualIncome: {
    currency: "GBP",
    amount: 0,
    symbol: "£"
  },
});
const addNewItem = () => {
  modelValue.value.push(newItem.value);
  newItem.value = { 
    productOrService: "", 
    totalProjectAnnualIncome: {
      currency: "GBP",
      amount: 0,
      symbol: "£"
    }
  };
};

const setUniqueIdentifier = (value: string): string => {
  const identifier = `${AppConstants.arFirmDetailsRoute}-productAndServices${value}`;
  return identifier.replace(/\s+/g, "").replace("/", "");
};

const unableToAdd = computed(() => {
  return !newItem.value.productOrService || !newItem.value.totalProjectAnnualIncome.amount;
});

const totalAmount = computed(() =>
  modelValue.value
    .reduce((acc, item) => acc + item.totalProjectAnnualIncome.amount, 0)
    .toFixed(2)
    .replace(/\d(?=(\d{3})+\.)/g, "$&,")
);
</script>

<template>
  <div class="ProductAndServicesTable">
    <div class="ProductAndServicesTable-head">
      <div class="ProductAndServicesTable-cell">Product/Service</div>

      <div class="ProductAndServicesTable-cell">Total Projected Annual Income</div>

      <div class="ProductAndServicesTable-cell">&nbsp;</div>
    </div>

    <div
      v-for="(item, index) in modelValue"
      :key="`fee-item-${index}`"
      class="ProductAndServicesTable-row"
    >
      <div class="ProductAndServicesTable-cell">
        <KendoDropdownListComponent
          :name="`fee-type-${index}`"
          :id="setUniqueIdentifier(`-fee-type-${index}`)"
          :data-items="ProductAndServicesOptions"
          value-primitive
          placeholder="Please Select"
          :is-required="false"
          :model-value="item.productOrService"
          @update:model-value="item.productOrService = $event"
        />
      </div>

      <div class="ProductAndServicesTable-cell">
        <KendoCurrencyInputComponent
            name="highestAmount1"
            :id="setUniqueIdentifier('-highestAmount1')"
            :hasLabel="false"
            :value="item.totalProjectAnnualIncome"
            @onValueChange="item.totalProjectAnnualIncome = $event"
          />
      </div>

      <div class="ProductAndServicesTable-cell py-0 mb-3">
        <button
          type="button"
          class="ActionButton"
          @click.prevent="deleteItem(index)"
        >
          <IconComponent symbol="trash" class="text-error" size="20" />
        </button>
      </div>
    </div>

    <div class="ProductAndServicesTable-row">
      <div class="ProductAndServicesTable-cell">
        <KendoDropdownListComponent
          :name="`fee-type-new`"
          :id="setUniqueIdentifier(`-fee-type-new`)"
          :data-items="ProductAndServicesOptions"
          value-primitive
          :is-required="false"
          placeholder="Please Select"
          v-model="newItem.productOrService"
        />
      </div>

      <div class="ProductAndServicesTable-cell">
        <KendoCurrencyInputComponent
            name="highestAmount2"
            :id="setUniqueIdentifier('-highestAmount2')"
            :hasLabel="false"
            @onValueChange="newItem.totalProjectAnnualIncome = $event"
          />
      </div>

      <div class="ProductAndServicesTable-cell py-0 mb-2">
        <button
          type="button"
          class="ActionButton"
          :disabled="unableToAdd"
          @click.prevent="addNewItem"
        >
          <IconComponent
            symbol="add-circle-27"
            :class="[!unableToAdd && 'text-primary']"
            size="20"
          />
        </button>
      </div>
    </div>

    <div class="ProductAndServicesTable-foot">
      <div class="ProductAndServicesTable-cell text-right">
        <strong>Total</strong>
      </div>

      <div class="ProductAndServicesTable-cell">
        <strong class="ProductAndServicesTable-amount">
          £ {{ totalAmount }}
        </strong>
      </div>

      <div class="ProductAndServicesTable-cell">&nbsp;</div>
    </div>
  </div>
</template>

<style scoped lang="scss">
.ProductAndServicesTable {
  width: 100%;
  border: 1px solid var(--infotable-accent-color);
  border-radius: 8px;
  overflow: hidden;

  &-head,
  &-row,
  &-foot {
    display: flex;
  }

  &-head {
    background-color: var(--infotable-accent-color);
    font-size: var(--font-size-xs);
    font-weight: var(--font-weight-bold);
  }

  &-row {
    border-bottom: 1px solid var(--infotable-accent-color);
    align-items: center;
  }

  &-foot {
    font-size: var(--font-size-sm);
    align-items: center;
  }

  &-cell {
    flex: 1;
    padding: 10px;

    &:last-child {
      flex: 0;
    }
  }

  &-amount {
    font-size: var(--font-size-lg);
    font-weight: var(--font-weight-bold);
    color: rgba(151, 161, 175, 1);
  }
}

.CurrencyInput {
  :deep(.k-input-prefix) {
    background-color: var(--content-content-10);
    padding: 5px 12px;
    color: rgba(151, 161, 175, 1);
    border-right: 1px solid var(--content-content-07);
  }
}

.ActionButton {
  background: none;
  border: none;
  padding: 0;
  margin: 0;
}
</style>
