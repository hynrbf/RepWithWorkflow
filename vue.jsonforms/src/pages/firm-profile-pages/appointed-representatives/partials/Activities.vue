<script setup lang="ts">
import { computed, ref, toRefs } from "vue";
import {
  AppointedRepresentativeProduct,
  AppointedRepresentativeProductType,
} from "@/entities/appointed-representatives/AppointedRepresentativeProduct";
import { AppointedRepresentativeActivity } from "@/entities/appointed-representatives/AppointedRepresentativeActivity";
import { AppConstants } from "@/infra/AppConstants";
import { CustomerProduct } from "@/entities/CustomerProduct";
import { Money } from "@/entities/Money";
import { NavPillItem } from "@/components/NavPillComponent.vue";
import StaticList from "@/infra/StaticListService";

const props = withDefaults(
  defineProps<{
    modelValue: AppointedRepresentativeActivity[];
    viewMode?: boolean;
  }>(),
  {
    modelValue: () => [],
  },
);

const emits = defineEmits<{
  (event: "update:modelValue", values: AppointedRepresentativeActivity[]): void;
}>();

const { modelValue, viewMode } = toRefs(props);

const values = computed({
  get() {
    return modelValue.value.reduce(
      (accumulator, value) => {
        return {
          ...accumulator,
          [value.productId]: value,
        };
      },
      {} as Record<string, AppointedRepresentativeActivity>,
    );
  },
  set(values: Record<string, AppointedRepresentativeActivity>) {
    emits("update:modelValue", Object.values(values));
  },
});

const minimumMoney = ref(new Money(0, "EUR"));

const getProducts = (): CustomerProduct => {
  let customerProducts: CustomerProduct[] = [];

  const productsLocalJson = localStorage.getItem(
    AppConstants.customerProductsKey,
  );
  if (productsLocalJson) {
    customerProducts = JSON.parse(productsLocalJson) as CustomerProduct[];
  }
  let customerPageProducts =
    customerProducts.find((cp) => cp.pageName == "Appointed Representatives") ??
    new CustomerProduct();
  return customerPageProducts;
};

const getARProductTypes = () => {
  let productTypes = getProducts().categories.map((category) => {
    const id = category.name?.toLowerCase().replace(/ /g, "-");
    return { id, title: category.displayText };
  });
  return productTypes;
};

const getARProducts = () => {
  let arProducts: AppointedRepresentativeProduct[] = [];
  getProducts().categories.forEach((category) => {
    let typeId = category.name?.toLowerCase().replace(/ /g, "-");
    category.products.forEach((product) => {
      arProducts.push({
        id: product.name?.toLowerCase().replace(/ /g, "-") ?? "",
        typeId: typeId ?? "",
        title: product.displayText ?? "",
      });
    });
  });

  if (arProducts.length == 0) {
    arProducts = StaticList.getARProducts();
  }
  return arProducts;
};

const types = ref<AppointedRepresentativeProductType[]>(
  getARProductTypes() as AppointedRepresentativeProductType[],
);

const staticTypes = ref<AppointedRepresentativeProductType[]>(
  StaticList.getARActivityProducts() as AppointedRepresentativeProductType[],
);

const products = ref<AppointedRepresentativeProduct[]>(
  getARProducts() as AppointedRepresentativeProduct[],
);

const columns = computed(() => [
  {
    field: "product",
    title: "Product",
  },
  {
    field: "appointment",
    title: "Appointed",
  },
  {
    field: "annualFeeIncome",
    title: "Projected Annual Fee Income",
  },
  {
    field: "annualCommissionIncome",
    title: "Projected Annual Commission Income",
  },
  {
    field: "limitations",
    title: "Limitations",
  },
  {
    field: "action",
    title: " ",
  },
]);

const activitiesNavPill = computed(() => {
  const sourceTypes = types.value.length > 0 ? types.value : staticTypes.value;

  return sourceTypes.map(({ id, title, icon }) => ({
    id: `activity-nav-${id}`,
    anchorTo: `panel-${id}`,
    label: title,
    icon,
    active: id === "mortgage-broking",
  }));
});

const activitiesPanels = computed(() => {
  const panels = types.value.length > 0 ? types.value : staticTypes.value;

  return panels.map(({ id, title }) => ({
    id,
    title,
    content: id,
    contentClass: "p-0",
  }));
});

const getItems = (typeId: string) => {
  const output = products.value
    .filter((product) => product.typeId === typeId)
    .map((product) => ({
      product: product.id,
      annualFeeIncome: {
        amount: 0,
        currency: "GBP",
        symbol: "£",
      },
      annualCommissionIncome: {
        amount: 0,
        currency: "GBP",
        symbol: "£",
      },
      limitations: "",
      hasLimitation: false,
      appointment: false,
    }));

  output.push({
    product: "add-more",
    annualFeeIncome: {
      amount: 0,
      currency: "GBP",
      symbol: "£",
    },
    annualCommissionIncome: {
      amount: 0,
      currency: "GBP",
      symbol: "£",
    },
    limitations: "",
    hasLimitation: false,
    appointment: false,
  });

  return output;
};

const updateValue = (
  productId: string,
  payload: Partial<AppointedRepresentativeActivity>,
) => {
  const activity = values.value[productId];

  if (activity) {
    values.value = {
      ...values.value,
      [productId]: {
        ...activity,
        ...payload,
      },
    };
  } else {
    values.value = {
      ...values.value,
      [productId]: {
        productId,
        ...payload,
      } as AppointedRepresentativeActivity,
    };
  }
};
const removeValue = (productId: string) => {
  const products = Object.assign({}, values.value);
  delete products[productId];
  values.value = products;
};

const getProduct = (id: string) =>
  products.value.find((product) => product.id === id);

const addProduct = (payload: Partial<(typeof products.value)[number]>) => {
  products.value = [
    ...products.value,
    {
      ...payload,
    } as unknown as AppointedRepresentativeProduct,
  ];
};

const formatMoney = (value: number) => {
  return "£ " + value.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,");
};

const getTotal = (
  typeId: string,
  key: "annualFeeIncome" | "annualCommissionIncome",
) => {
  return Object.values(values.value).reduce((accumulator, item) => {
    const product = getProduct(item.productId);
    if (!product || typeId !== product.typeId) return accumulator;

    return accumulator + (item[key]?.amount || 0);
  }, 0);
};

const ActivityCollapsiblePanel = ref();

const selectedAll = ref<string[]>([]);
const selectAll = (typeId: string, isAppointed: boolean) => {
  if (isAppointed) {
    selectedAll.value = [...selectedAll.value, typeId];
  } else {
    selectedAll.value = selectedAll.value.filter((id) => id !== typeId);
  }

  products.value.forEach((product) => {
    if (product.typeId !== typeId) {
      return;
    }
    setTimeout(() => {
      if (isAppointed) {
        updateValue(product.id, { isAppointed });
      } else {
        removeValue(product.id);
      }
    }, 200);
  });
};

const openActivityPanel = (items: NavPillItem[]) => {
  const item: NavPillItem[] = items.filter((item) => item.active);
  if (item.length > 0) {
    const activePanel: string | undefined = item[0].anchorTo;

    if (activePanel !== undefined && activePanel !== "") {
      ActivityCollapsiblePanel.value.expand(
        activePanel.replace(/^panel-/, "").trim(),
      );
    }
  }
};

const newItems = ref<Record<string, Record<string, string>>>({});
const attachNewItem = (typeId: string, values: Record<string, string>) => {
  newItems.value = {
    ...newItems.value,
    [typeId]: {
      ...(newItems.value[typeId] ?? {}),
      ...values,
    },
  };
};
const addNewItem = (typeId: string) => {
  const newItem = newItems.value[typeId];
  if (!newItem) {
    return;
  }

  const productId = newItem.title.replace(/ +/g, "-").toLowerCase();

  values.value = {
    ...values.value,
    [productId]: {
      productId,
      annualFeeIncome: newItem.annualFeeIncome,
      annualCommissionIncome: newItem.annualCommissionIncome,
      isAppointed: true,
      hasLimitation: newItem.hasLimitation,
      isNewProduct: true,
    } as unknown as AppointedRepresentativeActivity,
  };

  addProduct({
    id: productId,
    typeId,
    title: newItem.title,
  });

  delete newItems.value[typeId];
};

const setUniqueIdentifier = (value: string): string => {
  const identifier = `${AppConstants.appointedRepresentativesRoute}${value}`;
  return identifier.replace(/\s+/g, "").replace("/", "");
};
</script>

<template>
  <div v-if="viewMode">
    <dl v-for="group in types" :key="`dl-${group.id}`" class="DescList">
      <dt>
        {{ group.title }} |
        {{
          formatMoney(
            getTotal(group.id, "annualFeeIncome") +
              getTotal(group.id, "annualCommissionIncome")
          )
        }}
      </dt>
      <dd>
        <table class="DescTable">
          <tr>
            <th width="30%">Product Name</th>
            <th class="text-center">Projected Annual Fee Income</th>
            <th class="text-center">Projected Annual Commission Income</th>
            <th>Limitations</th>
          </tr>
          <tr v-for="item in getItems(group.id)" :key="`dt-${item.product}`">
            <td>{{ getItems(group.id).length === 1 ? "-" : (item.product === "add-more" ? "" :  getProduct(item.product)?.title) }}</td>
            <td class="text-center">
              {{
                getItems(group.id).length === 1 ? "-" : (item.product === "add-more" ? "" : formatMoney(values[item.product]?.annualFeeIncome?.amount ?? 0))
              }}
            </td>
            <td class="text-center">
              {{
                getItems(group.id).length === 1 ? "-" :  (item.product === "add-more" ? "" :  formatMoney(
                  values[item.product]?.annualCommissionIncome?.amount ?? 0
                ))
              }}
            </td>
            <td>{{ getItems(group.id).length === 1 ? "-" : (item.product === "add-more" ? "" : values[item.product]?.limitations) }}</td>
          </tr>
        </table>
      </dd>
    </dl>
  </div>

  <div v-else>
    <NavPillComponent
      :items="activitiesNavPill"
      class="ActivitiesNavPill is-sticky-top is-bg-white"
      anchorable
      @update:items="openActivityPanel"
    ></NavPillComponent>

    <div class="mb-3"></div>

    <CollapsiblePanelComponent
      ref="ActivityCollapsiblePanel"
      class="ActivityCollapsiblePanel activity-modal"
      :items="activitiesPanels"
      :accordion="false"
    >
      <template #title="{ item }">
        <div class="ActivityCollapsiblePanel-title">
          <span>{{ item.title }}</span>
          <KendoSwitchToggleComponent
            :name="`toggle-${item.id}`"
            :id="setUniqueIdentifier(`-toggle-${item.id}`)"
            :is-required="false"
            no-text
            :value="selectedAll.includes(item.content)"
            @click.stop
            @onValueChange="
              (event: boolean) => {
                selectAll(item.content, event);
                openActivityPanel(item.id);
              }
            "
          />
        </div>
      </template>
      <template #content="{ item }">
        <InfoTableComponent
          class="ActivityInfoTable"
          :id="item.content"
          :columns="columns"
          :data="[{ items: getItems(item.content), footer: true }]"
        >
          <template #cell-product="{ item, id }">
            <KendoInput
              v-if="item.product === 'add-more'"
              name="add-more"
              placeholder="Add Product"
              :model-value="newItems[id]?.title ?? ''"
              @update:model-value="attachNewItem(id ?? '', { title: $event })"
            />
            <span v-else>{{ getProduct(item.product)?.title }}</span>
          </template>
          <template #cell-appointment="{ item, id }">
            <KendoSwitchToggleComponent
              v-if="item.product === 'add-more'"
              :name="`appointed-${item.product}`"
              :id="setUniqueIdentifier(`-appointed-${item.product}`)"
              :is-required="false"
              no-text
              :value="newItems[id]?.isAppointed ?? false"
              @onValueChange="attachNewItem(id ?? '', { isAppointed: $event })"
            />
            <KendoSwitchToggleComponent
              v-else
              :name="`appointed-${item.product}`"
              :id="setUniqueIdentifier(`-appointed-${item.product}`)"
              :is-required="false"
              no-text
              :value="values[item.product]?.isAppointed ?? false"
              @onValueChange="
                (event: boolean) => {
                  if (event) {
                    updateValue(item.product, { isAppointed: event });
                  } else {
                    removeValue(item.product);
                  }
                }
              "
            />
          </template>
          <template #cell-annualFeeIncome="{ item, id }">
            <KendoCurrencyInputComponent
              v-if="item.product === 'add-more'"
              :name="`annualFeeIncome-${item.product}`"
              :id="setUniqueIdentifier(`-annualFeeIncome-${item.product}`)"
              :placeholder="'Enter Amount'"
              :hasLabel="false"
              :value="newItems[id]?.annualFeeIncome"
              :minimumValue="minimumMoney"
              :isValueReactive="true"
              @onValueChange="
                attachNewItem(id, {
                  annualFeeIncome: $event ?? minimumMoney,
                })
              "
            />
            <KendoCurrencyInputComponent
              v-else
              :name="`annualFeeIncome-${item.product}`"
              :id="setUniqueIdentifier(`-annualFeeIncome-${item.product}`)"
              :placeholder="'Enter Amount'"
              :hasLabel="false"
              :disabled="!values[item.product]?.isAppointed"
              :value="values[item.product]?.annualFeeIncome"
              :minimumValue="minimumMoney"
              :isValueReactive="true"
              @onValueChange="
                updateValue(item.product, {
                  annualFeeIncome: $event ?? minimumMoney,
                })
              "
            />
          </template>
          <template #cell-annualCommissionIncome="{ item, id }">
            <KendoCurrencyInputComponent
              v-if="item.product === 'add-more'"
              :name="`annualCommissionIncome-${item.product}`"
              :id="setUniqueIdentifier(`-annualCommissionIncome-${item.product}`)"
              :placeholder="'Enter Amount'"
              :hasLabel="false"
              :value="newItems[id]?.annualCommissionIncome"
              :minimumValue="minimumMoney"
              :isValueReactive="true"
              @onValueChange="
                attachNewItem(id, {
                  annualCommissionIncome: $event ?? minimumMoney,
                })
              "
            />
            <KendoCurrencyInputComponent
              v-else
              :name="`annualCommissionIncome-${item.product}`"
              :id="setUniqueIdentifier(`-annualCommissionIncome-${item.product}`)"
              :placeholder="'Enter Amount'"
              :hasLabel="false"
              :disabled="!values[item.product]?.isAppointed"
              :value="values[item.product]?.annualCommissionIncome"
              :minimumValue="minimumMoney"
              :isValueReactive="true"
              @onValueChange="
                updateValue(item.product, {
                  annualCommissionIncome: $event ?? minimumMoney,
                })
              "
            />
          </template>
          <template #cell-limitations="{ item, id }">
            <KendoSwitchToggleComponent
              v-if="item.product === 'add-more'"
              :name="`limitation-${item.product}`"
              :id="setUniqueIdentifier(`-limitation-${item.product}`)"
              :is-required="false"
              no-text
              :value="newItems[id]?.hasLimitation ?? false"
              @onValueChange="attachNewItem(id, { hasLimitation: $event })"
            />
            <KendoSwitchToggleComponent
              v-else
              :name="`limitation-${item.product}`"
              :id="setUniqueIdentifier(`-limitation-${item.product}`)"
              :is-required="false"
              no-text
              :value="values[item.product]?.hasLimitation ?? false"
              @onValueChange="
                updateValue(item.product, { hasLimitation: $event })
              "
            />
          </template>
          <template #expanded-content="{ item, item: { product }, id }">
            <div class="mt-0">
              <KendoGenericInputComponent
                v-if="product === 'add-more' && newItems[id]?.hasLimitation"
                :id="setUniqueIdentifier(`-limitations-${product}`)"
                :name="`limitations-${product}`"
                class="col-custom-activities"
                :placeholder="'Please Type Limitations'"
                :value="newItems[id]?.limitations"
                :isRequired="false"
                :model-value="newItems[id]?.limitations ?? ''"
                @update:model-value="attachNewItem(id, { limitations: $event })"
              />

              <KendoGenericInputComponent
                v-if="values[item.product]?.hasLimitation"
                :id="setUniqueIdentifier(`-limitations-${item.product}`)"
                :name="`limitations-${item.product}`"
                class="col-custom-activities"
                :placeholder="'Please Type Limitations'"
                :value="values[item.product]?.limitations"
                :isRequired="false"
                @update:model-value="
                  updateValue(item.product, { limitations: $event })
                "
              />
            </div>
          </template>
          <template #cell-action="{ item, id }">
            <div v-if="item.product === 'add-more'">
              <button class="AddButton" @click.prevent="addNewItem(id ?? '')">
                <IconComponent symbol="add-circle-27" size="20" />
              </button>
            </div>
            <div v-else>
              <div style="width: 20px"></div>
            </div>
          </template>
          <template #footer-cell-product>
            <div class="py-1">
              <strong>Total</strong>
            </div>
          </template>
          <template #footer-cell-annualFeeIncome="{ id }">
            <div class="py-1">
              <span class="TotalText">
                {{ formatMoney(getTotal(id, "annualFeeIncome")) }}
              </span>
            </div>
          </template>
          <template #footer-cell-annualCommissionIncome="{ id }">
            <div class="py-1">
              <span class="TotalText">
                {{ formatMoney(getTotal(id, "annualCommissionIncome")) }}
              </span>
            </div>
          </template>
        </InfoTableComponent>
      </template>
    </CollapsiblePanelComponent>
  </div>
</template>

<style scoped lang="scss">
.ActivitiesNavPill {
  margin-left: -10px;
  margin-right: -10px;
}

.ActivityCollapsiblePanel {
  &-title {
    display: flex;

    span {
      min-width: 340px;
    }
  }

  :deep(.CollapsiblePanel-header) {
    padding: 15px;
  }

  :deep(.CollapsiblePanel-item) {
    border-color: var(--infotable-accent-color);
  }

  :deep(.CollapsiblePanel-item.is-active) {
    border-color: var(--color-primary);
  }
}

.ActivityInfoTable {
  :deep(.InfoTable-header) {
    font-size: var(--font-size-sm);
    margin-bottom: 0;
  }

  :deep(.InfoTable-content) {
    border-top-left-radius: 0;
    border-top-right-radius: 0;
    margin-bottom: 0;
    border: 0;

    .InfoTable-row:not(:last-child) {
      border-bottom: 1px solid var(--infotable-accent-color);
    }
  }

  :deep(.InfoTable-cell) {
    display: flex;
    align-items: center;
    justify-content: center;

    &:nth-child(1) {
      justify-content: left;
      flex: 0 0 330px;
      max-width: 330px;
    }

    &:nth-child(2),
    &:nth-child(5),
    &:nth-child(6) {
      flex: 0;
    }

    &:nth-child(2),
    &:nth-child(5) {
      padding-left: 25px;
      padding-right: 25px;
    }
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

.AddButton {
  background: none;
  border: none;
  padding: 0;
  margin: 0;
  color: var(--color-primary);
}

.TotalText {
  font-weight: var(--font-weight-bold);
  font-size: var(--font-size-lg);
  color: rgba(151, 161, 175, 1);
}

.DescList {
  dt {
    font-size: var(--font-size-default);
    margin-bottom: 10px;
  }

  dd {
    font-size: var(--font-size-sm);
  }
}

.DescTable {
  width: 100%;

  th {
    font-size: var(--font-size-xs);
    padding: 5px 0;
  }
}

.activity-modal {
  .col-custom-activities {
    padding: 0px !important;
    margin-left: 34.7%;
    width: 53%;
    margin-top: 0;
    margin-bottom: 17px;
  }
}
</style>