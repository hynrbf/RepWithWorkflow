<script setup lang="ts">
import { computed, toRefs, ref } from "vue";
import StaticList from "@/infra/StaticListService";
import { Money } from "@/entities/Money";
import {AppConstants} from "@/infra/AppConstants";

const props = withDefaults(
  defineProps<{
    modelValue: {
      type: string;
      amount: Money;
    }[];
  }>(),
  {
    modelValue: () => [],
  },
);

const emits = defineEmits<{
  (
    event: "update:modelValue",
    value: {
      type: string;
      amount: Money;
    }[],
  ): void;
}>();

const { modelValue } = toRefs(props);

const feeTypeOptions = computed(() =>
  StaticList.getFeesCharges().map((item) => ({ label: item, value: item })),
);

const deleteItem = (index: number) => {
  emits(
    "update:modelValue",
    modelValue.value.filter((_v, i) => index !== i),
  );
};

const newItem = ref({
  type: "",
  amount: {
    amount: undefined,
    currency: "GBP",
    symbol: "£",
  } as Money,
});

const addNewItem = () => {
  modelValue.value.push(newItem.value);
  newItem.value = {
    type: "",
    amount: {
      amount: undefined,
      currency: "GBP",
      symbol: "£",
    },
  };
};
const unableToAdd = computed(() => {
  return !newItem.value.type || !newItem.value.amount?.amount;
});

const totalAmount = computed(() =>
  modelValue.value
    .reduce((accumulator, value) => {
      const amount = value.amount?.amount ?? 0;
      return accumulator + amount;
    }, 0)
    .toFixed(2)
    .replace(/\d(?=(\d{3})+\.)/g, "$&,"),
);

const setUniqueIdentifier = (value: string): string => {
  const identifier = `${AppConstants.appointedRepresentativesRoute}${value}`;
  return identifier.replace(/\s+/g, "").replace("/", "");
};
</script>

<template>
  <div class="FeesChargeTable">
    <div class="FeesChargeTable-head">
      <div class="FeesChargeTable-cell">Fee Type</div>
      <div class="FeesChargeTable-cell">Amount to be Charged</div>
      <div class="FeesChargeTable-cell">&nbsp;</div>
    </div>
    <div
      v-for="(item, index) in modelValue"
      :key="`fee-item-${index}`"
      class="FeesChargeTable-row"
    >
      <div class="FeesChargeTable-cell">
        <KendoDropdownListComponent
          :name="`fee-type-${index}`"
          :id="setUniqueIdentifier(`-fee-type-${index}`)"
          :data-items="feeTypeOptions"
          value-primitive
          placeholder="Please Select"
          :is-required="false"
          :model-value="item.type"
          @update:model-value="item.type = $event"
        />
      </div>
      <div class="FeesChargeTable-cell">
        <KendoCurrencyInputComponent
          :name="`fee-amount-${index}`"
          :id="setUniqueIdentifier(`-fee-amount-${index}`)"
          :placeholder="'Enter Amount'"
          :hasLabel="false"
          :value="item.amount"
          :minimumValue="new Money(0, 'EUR')"
          :isValueReactive="true"
          @onValueChange="(value: Money) => (item.amount = value)"
        />
      </div>
      <div class="FeesChargeTable-cell ps-0">
        <button
          type="button"
          class="ActionButton"
          @click.prevent="deleteItem(index)"
        >
          <IconComponent symbol="trash" class="text-error" size="20" />
        </button>
      </div>
    </div>
    <div class="FeesChargeTable-row">
      <div class="FeesChargeTable-cell">
        <KendoDropdownListComponent
          :name="`fee-type-new`"
          :id="setUniqueIdentifier(`-fee-type-new`)"
          :data-items="feeTypeOptions"
          value-primitive
          :is-required="false"
          placeholder="Please Select"
          v-model="newItem.type"
        />
      </div>
      <div class="FeesChargeTable-cell">
        <KendoCurrencyInputComponent
          :name="`fee-amount`"
          :id="setUniqueIdentifier(`-fee-amount`)"
          :placeholder="'Enter Amount'"
          :hasLabel="false"
          :value="newItem.amount"
          :minimumValue="new Money(0, 'EUR')"
          :isValueReactive="true"
          @onValueChange="(value: Money) => (newItem.amount = value)"
        />
      </div>
      <div class="FeesChargeTable-cell ps-0">
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
    <div class="FeesChargeTable-foot">
      <div class="FeesChargeTable-cell text-right">
        <strong>Total</strong>
      </div>
      <div class="FeesChargeTable-cell">
        <strong class="FeesChargeTable-amount"> £ {{ totalAmount }} </strong>
      </div>
      <div class="FeesChargeTable-cell">&nbsp;</div>
    </div>
  </div>
</template>

<style scoped lang="scss">
.FeesChargeTable {
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