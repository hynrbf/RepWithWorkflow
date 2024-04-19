<script setup lang="ts">
import { toRefs, ref, computed } from "vue";
import { onClickOutside } from "@vueuse/core";
import {
  GridSortChangeEvent,
  GridPageChangeEvent,
} from "@progress/kendo-vue-grid";
import { SortDescriptor } from "@progress/kendo-data-query";
import { saveExcel } from "@progress/kendo-vue-excel-export";

export interface GridCardTableColumn {
  [key: string]: any;
}

export interface GridCardTableItem {
  [key: string]: any;

  // Built-in properties
  _inactive?: boolean;
  _pending?: boolean;
  _rejected?: boolean;
  _approved?: boolean;
}

const props = withDefaults(
  defineProps<{
    columns: GridCardTableColumn[];
    items: GridCardTableItem[];
    groups?: { title: string; value: string }[];
    activeGroup?: string;
    noToolbar?: boolean;
    noSwitcher?: boolean;
    noActionbar?: boolean;
    addButtonText?: string;
    searchPlaceholder?: string;
    centerAlignedColumnHeaders?: string[];
    isSortable?: boolean;
    sort?: SortDescriptor[];
    handleSortChange?: (event: GridSortChangeEvent) => void;
    handlePageChange?: (event: GridPageChangeEvent) => void;
    pageable?: boolean;
    total?: number;
    skip?: number;
    take?: number;
    loading?: boolean;
    exportHandler?: (
      items: GridCardTableItem[],
      columns: GridCardTableColumn[],
    ) => void;
  }>(),
  {
    columns: () => [],
    items: () => [],
    groups: () => [],
    noToolbar: false,
    noActionbar: true,
    addButtonText: "Add Item",
    searchPlaceholder: "",
    centerAlignedColumnHeaders: () => [],
    isSortable: false,
    sort: undefined,
    handleSortChange: undefined,
  },
);
const emits = defineEmits<{
  (event: "add"): void;
  (event: "update:activeGroup", value: string): void;
  (event: "change-group", value: string): void;
  (event: "filter", value: any): void;
  (event: "search", value: string): void;
  (event: "clear"): void;
}>();
const { columns, items, exportHandler } = toRefs(props);

const getColumn = (field: string) =>
  columns.value.find((column) => column.field === field);

const tableType = ref("grid");
const isGrid = computed(() => !tableType.value || tableType.value === "grid");
const isCard = computed(() => tableType.value === "card");

const renderRow = (h: any, _trElement: any, defaultSlots: any, props: any) => {
  const { dataItem } = props;

  const additionalClasses = [
    " ", // extra space
    dataItem?.inactive && "is-inactive",
    dataItem?.pending && "is-pending",
    dataItem?.rejected && "is-rejected",
    dataItem?.approved && "is-approved",
  ].join(" ");

  props.class += additionalClasses;

  return h("tr", props, defaultSlots);
};

const isFilterOpen = ref(false);
const filterButton = ref();
const filterBody = ref();

const isCenterAlignedHeader = (columnName: string): boolean => {
  if (!columnName) {
    return false;
  }

  if (!props.centerAlignedColumnHeaders.length) {
    // all columns left aligned by default
    return false;
  }

  return props.centerAlignedColumnHeaders.includes(columnName);
};

onClickOutside(
  filterBody,
  (event) => {
    if ((event.target as HTMLElement).closest(".k-popup")) {
      return;
    }
    isFilterOpen.value = false;
  },
  {
    ignore: [filterButton],
  },
);

const exportData = () => {
  if (typeof exportHandler?.value === "function") {
    exportHandler?.value(items.value, columns.value);
    return;
  }

  saveExcel({
    data: items.value,
    columns: columns.value,
    fileName: `export-${Date.now()}`,
  });
};
</script>

<template>
  <div class="GridCardTable">
    <div v-if="!noToolbar" class="GridCardTable-toolbar">
      <div v-if="!noSwitcher" class="GridCardTable-buttons">
        <KendoButton
          type="button"
          size="small"
          fill-mode="flat"
          rounded="full"
          shape="square"
          theme-color="primary"
          class="Button"
          :selected="isGrid"
          @click="tableType = 'grid'"
        >
          <IconComponent symbol="bullet-list-17" size="20" />
        </KendoButton>

        <KendoButton
          type="button"
          size="small"
          fill-mode="flat"
          rounded="full"
          shape="square"
          theme-color="primary"
          class="Button"
          :selected="isCard"
          @click="tableType = 'card'"
        >
          <IconComponent symbol="dashboard-square-4" size="20" />
        </KendoButton>
      </div>

      <div v-if="groups.length" class="GridCardTable-groups">
        <div class="k-tabstrip TabStrip TabStrip--lined">
          <div class="k-tabstrip-items-wrapper mb-0">
            <ul class="k-tabstrip-items k-reset" role="tablist" tabindex="0">
              <li
                v-for="group in groups"
                :key="`group-${group.value}`"
                aria-selected="true"
                role="tab"
                :class="[
                  'k-first k-item',
                  group.value === activeGroup && 'k-active',
                ]"
                @click.prevent="
                  () => {
                    emits('update:activeGroup', group.value);
                    emits('change-group', group.value);
                  }
                "
              >
                <span class="k-link">{{ group.title }}</span>
              </li>
            </ul>
          </div>
        </div>
      </div>

      <KendoButton
        type="button"
        theme-color="primary"
        icon="plus"
        @click="emits('add')"
      >
        {{ addButtonText }}
      </KendoButton>
    </div>

    <div v-if="!noActionbar" class="GridCardTable-actionbar">
      <div class="GridCardTable-search">
        <KendoInput
          input-prefix="prefix"
          rounded="full"
          :placeholder="searchPlaceholder"
          @keyup.enter="emits('search', $event?.target?.value)"
        >
          <template #prefix>
            <IconComponent symbol="search" width="15" class="ms-3" />
          </template>
        </KendoInput>

        <KendoButton
          ref="filterButton"
          fill-mode="outline"
          theme-color="primary"
          :class="[
            'GridCardTable-filterButton',
            isFilterOpen && 'is-filter-open',
          ]"
          @click.prevent="isFilterOpen = true"
        >
          <IconComponent symbol="filter-2-58" width="15" class="me-1" />
          Filter
          <KendoButton
            v-if="isFilterOpen"
            type="button"
            size="small"
            rounded="full"
            shape="square"
            theme-color="light"
            title="View"
            class="GridCardTable-filterClose"
            @click.stop="isFilterOpen = false"
          >
            <IconComponent symbol="clear" size="10" />
          </KendoButton>
        </KendoButton>

        <KendoPopup
          anchor="filterButton"
          :show="isFilterOpen"
          popup-class="FilterPopup"
        >
          <div ref="filterBody" class="FilterPopup-body">
            <KendoForm>
              <KendoFormElementComponent
                ref="filterFormElement"
                v-slot="{ form, reset }"
              >
                <slot name="filter-fields" :form="form"></slot>
                <div class="FilterPopup-buttons">
                  <KendoButton
                    theme-color="light"
                    @click.prevent="
                      reset();
                      emits('clear');
                    "
                  >
                    Clear
                  </KendoButton>
                  <KendoButton
                    theme-color="primary"
                    @click.prevent="emits('filter', form)"
                  >
                    Apply Filters
                  </KendoButton>
                </div>
              </KendoFormElementComponent>
            </KendoForm>
          </div>
        </KendoPopup>
      </div>

      <KendoButton
        fill-mode="outline"
        theme-color="primary"
        @click.prevent="exportData"
      >
        <IconComponent symbol="arrow-up-square-87" width="15" class="me-1" />
        Export
      </KendoButton>
    </div>

    <KendoGrid
      v-if="isGrid"
      class="GridTable"
      cell-render="cell"
      header-cell-render="header"
      :row-render="renderRow"
      :columns="columns"
      :data-items="items"
      :sortable="isSortable"
      :sort="sort"
      :pageable="pageable"
      :total="total"
      :skip="skip"
      :take="take"
      :loader="loading"
      @sortchange="handleSortChange"
      @pagechange="handlePageChange"
    >
      <template v-slot:header="{ props }">
        <StackLayout
          orientation="horizontal"
          :align="{
            vertical: 'middle',
            horizontal: isCenterAlignedHeader(props.field) ? 'center' : 'start',
          }"
          :gap="10"
          :class="getColumn(props.field)?.className"
        >
          <Label>{{ props.title || props.field }}</Label>

          <IconComponent
            v-if="props.sortable"
            symbol="sort-default"
            style="color: var(--content-content-06)"
            size="12"
          />
        </StackLayout>
      </template>
      <template v-slot:cell="{ props }">
        <td :class="props.class">
          <slot name="cell">
            <slot
              :name="`cell-${props.field}`"
              :item="props.dataItem"
              :props="props"
            >
              {{ props.dataItem[props.field] }}
            </slot>
          </slot>
        </td>
      </template>
    </KendoGrid>

    <!--ToDo. to polish and make this look good-->
    <div v-if="isCard">
      <div class="row">
        <div
          v-for="(item, index) in items"
          :key="`card-${index}-${item.id}`"
          class="col-lg-4"
        >
          <div class="GridCardTable-card">
            <div class="GridCardTable-cardBody">
              <slot name="card-content" :item="item"></slot>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
// ToDo. Move to global scss
.GridCardTable {
  &-toolbar,
  &-actionbar {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 25px;
    margin-bottom: 18px;
  }

  &-buttons {
    display: flex;
    align-items: center;
    gap: 15px;
  }

  &-groups {
    margin-right: auto;
  }

  &-search {
    display: flex;
    align-items: center;
    min-width: 300px;
    gap: 10px;
  }

  &-filterButton {
    &.is-filter-open {
      padding-left: 12px;
      padding-right: 12px;
    }
  }

  &-filterClose {
    margin-left: 5px;
    width: 24px;
    height: 24px;
    padding: 2px;
  }

  :deep(.k-table-row) {
    // ToDo. Move colors to variables
    &.is-inactive {
      background-color: #f2f4f7;
      color: #637083;
    }

    &.is-approved {
      background-color: rgba(236, 249, 242, 0.5);
    }

    &.is-pending {
      background-color: rgba(255, 249, 235, 0.5);
    }

    &.is-rejected {
      background-color: rgba(254, 231, 231, 0.5);
    }
  }

  :deep(.k-pager) {
    margin-top: 20px;

    .k-button {
      padding: 0;
      min-width: 30px;
      height: 30px;
      border-radius: 50%;
    }

    .k-pager-numbers .k-button {
      background: var(--color-white) !important;
      border: 1px solid var(--color-neutral-tint) !important;

      &.k-selected {
        background-color: var(--color-primary) !important;
        border-color: var(--color-primary) !important;
        color: var(--color-white) !important;
      }
    }

    .k-button-text {
      font-size: var(--font-size-sm) !important;
    }

    .k-pager-numbers,
    .k-pager-numbers-wrap {
      gap: 5px;
    }
  }

  &-card {
    border-bottom-left-radius: 8px;
    border-bottom-right-radius: 8px;
    border-top-left-radius: 8px;
    border-top-right-radius: 8px;
    border-color: rgba(0, 0, 0, 0.08);
    border-width: 1px;
    border-style: solid;
    color: #202b37;
    background-color: #ffffff;
  }

  &-cardBody {
    padding: 16px;
  }
}

.FilterClose {
  width: 24px;
  height: 24px;
  padding: 2px;
}

:global(.FilterPopup) {
  margin-top: 5px !important;
  border: 1px solid var(--color-primary) !important;
  border-radius: 8px;
  width: 400px;
  max-width: 100%;
}

:global(.FilterPopup-buttons) {
  display: flex;
  gap: 10px;
}

:global(.FilterPopup-buttons .k-button) {
  flex: 1;
}

:global(.FilterPopup-body) {
  padding: 20px;
}
</style>