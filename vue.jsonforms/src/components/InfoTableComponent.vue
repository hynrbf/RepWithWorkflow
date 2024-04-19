<script setup lang="ts">
export interface InfoTableColumn {
  field: string;
  title: string;
}

export interface InfoTableData {
  title: string;
  items: Record<string, any>[];
  footer?: boolean;
}

withDefaults(
  defineProps<{
    id?: string;
    columns?: InfoTableColumn[];
    data?: InfoTableData[];
  }>(),
  {
    columns: () => [],
    data: () => [],
  },
);
</script>

<template>
  <div :id="id" class="InfoTable">
    <div class="InfoTable-header">
      <div class="InfoTable-row text-center">
        <div
          v-for="column in columns"
          :key="`column-${column.field}`"
          class="InfoTable-cell"
        >
          <slot :name="`column-${column.field}`" :column="column" :id="id">
            {{ column.title || column.field }}
          </slot>
        </div>
      </div>
    </div>

    <div
      v-for="({ title, items, footer }, index) in data"
      :key="`data-${index}`"
      class="InfoTable-content"
    >
      <div v-if="title" class="InfoTable-heading">
        <div class="InfoTable-cell">
          {{ title }}
        </div>
      </div>

      <div class="container-fluid">
        <div
          v-for="(item, itemIndex) in items"
          :key="`item-${itemIndex}`"
          class="InfoTable-row row"
        >
          <div
            v-for="column in columns"
            :key="`cell-${column.field}`"
            class="InfoTable-cell"
          >
            <slot
              :name="`cell-${column.field}`"
              :item="item"
              :column="column"
              :id="id"
            >
              {{ item[column.field] }}
            </slot>
          </div>

          <slot name="expanded-content" :item="item" :id="id">
            <div class="container w-100" v-if="false">
              <div class="row">
                <div class="expanded-cell">
                  {{ item }}
                </div>
              </div>
            </div>
          </slot>
        </div>

        <div v-if="footer" class="InfoTable-row">
          <div
            v-for="column in columns"
            :key="`cell-${column.field}`"
            class="InfoTable-cell"
          >
            <slot
              :name="`footer-cell-${column.field}`"
              :column="column"
              :id="id"
            ></slot>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>