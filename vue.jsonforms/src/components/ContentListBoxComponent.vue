<script setup lang="ts">
import { ref, toRefs, watch, useSlots, inject } from "vue";
import { ListBox } from "@progress/kendo-vue-listbox";
import cloneDeep from "lodash/cloneDeep";
import isEmpty from "lodash/isEmpty";
import { AlertType, useAlert } from "@/composables/useAlert";
import { Emitter, EventType } from "mitt";
import { AppConstants } from "@/infra/AppConstants";

type ContentListBoxItem = Record<string, unknown>;

export interface ContentListBoxBlock {
  id: string;
  title: string;
  initialItems: ContentListBoxItem[];
  isFeatured?: boolean;
  noAdd?: boolean;
  emptyText?: string;
  isEditable?: boolean;
  keepEditting?: boolean;
}

const props = withDefaults(
  defineProps<{
    modelValue: Record<string, ContentListBoxItem[]>;
    blocks: ContentListBoxBlock[];
    addTitle?: string;
    deleteableKey?: string;
  }>(),
  {
    modelValue: () => ({}),
    blocks: () => [],
    addTitle: "Add New Text",
    deleteableKey: "deleteable",
  },
);

const emits = defineEmits<{
  (
    event: "update:modelValue",
    value: Record<string, ContentListBoxItem[]>,
  ): void;
  (event: "updated", value: Record<string, ContentListBoxItem[]>): void;
}>();

const slots = useSlots();
const eventBus = inject("$eventBusService") as Emitter<
  Record<EventType, unknown>
>;

const { modelValue, blocks, deleteableKey } = toRefs(props);

const items = ref<Record<string, ContentListBoxItem[]>>({});

const moveTo = (
  currentIndex: number,
  source: string,
  target: string,
  autoEdit = false,
) => {
  if (!items.value[source] || !items.value[target]) {
    return;
  }

  let currentValue: ContentListBoxItem | undefined;
  items.value[source] = items.value[source].filter((value, index) => {
    if (currentIndex === index) {
      currentValue = value;
      return false;
    }
    return true;
  });
  if (currentValue) {
    items.value[target] = [...items.value[target], currentValue];
    setIsEdittingItem(source, currentIndex, false);
    if (autoEdit) {
      setIsEdittingItem(target, items.value[target].length - 1, true);
    }

    emits("update:modelValue", items.value);
    emits("updated", items.value);
    eventBus.emit(AppConstants.formFieldChangedEvent);
    eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
  }
};

// Controls for referencing the state of "Add New Text" box
const isAddingIds = ref<string[]>([]);
const isAdding = (id: string) => isAddingIds.value.includes(id);
const setIsAdding = (id: string, state: boolean) => {
  if (state && !isAddingIds.value.includes(id)) {
    isAddingIds.value = [...isAddingIds.value, id];
  } else {
    isAddingIds.value = isAddingIds.value.filter((item) => item !== id);
  }
};

// Controls for adding new items
const newValues = ref<Record<string, ContentListBoxItem>>({});
const getNewValueSetter = (id: string) => {
  return (key: string, defaultValue: unknown) => {
    return newValues.value?.[id]?.[key] || defaultValue;
  };
};
const setNewValueSetter = (id: string) => {
  return (key: string, value: unknown) => {
    if (!newValues.value[id]) {
      newValues.value[id] = {};
    }
    newValues.value[id][key] = value;
  };
};
const clearNewValues = (id: string) => {
  newValues.value[id] = {};
};
const hasNewValues = (id: string) => {
  return !!Object.values(newValues.value[id] || {}).filter((item) => !!item)
    .length;
};
const addItems = (id: string) => {
  if (!newValues.value[id]) {
    return;
  }
  items.value = {
    ...items.value,
    [id]: [
      ...items.value[id],
      { ...cloneDeep(newValues.value[id]), [deleteableKey.value]: true },
    ],
  };
  clearNewValues(id);
};

// Controls for removing item
const removeItem = (id: string, index: number) => {
  if (!items.value[id]) {
    return;
  }

  useAlert({
    type: AlertType.CONFIRM,
    content: "Are you sure you want to delete this item?",
    onConfirm() {
      items.value[id].splice(index, 1);
      setIsEdittingItem(id, index, false);
    },
  });
};

// Controls for referencing the edit state an item
const isEdittingItemIds = ref<Record<string, number[]>>({});
const isEdittingItem = (id: string, index: number) => {
  if (blocks.value.some((block) => block.id === id && block.keepEditting)) {
    return true;
  }
  return (isEdittingItemIds.value[id] || []).includes(index);
};
const setIsEdittingItem = (id: string, index: number, state: boolean) => {
  if (!isEdittingItemIds.value[id]) {
    isEdittingItemIds.value[id] = [];
  }

  if (state && !isEdittingItemIds.value[id].includes(index)) {
    isEdittingItemIds.value[id] = [...isEdittingItemIds.value[id], index];
    return;
  }

  isEdittingItemIds.value[id] = isEdittingItemIds.value[id].filter(
    (item) => item !== index,
  );
};

const getValueSetter = (id: string, index: number) => {
  return (key: string, defaultValue?: unknown) => {
    if (!items.value[id] && !items.value[id][index]) {
      return defaultValue;
    }

    return items.value[id][index][key] || defaultValue;
  };
};
const setValueSetter = (id: string, index: number) => {
  return (key: string, value: unknown) => {
    if (!items.value[id] && !items.value[id][index]) {
      return;
    }

    items.value[id][index] = {
      ...items.value[id][index],
      [key]: value,
    };
  };
};

watch(
  modelValue,
  (value) => {
    const initialItems = blocks.value.reduce(
      (accumulator, block) => {
        if (modelValue.value[block.id] || isEmpty(block.initialItems)) {
          return accumulator;
        }

        return { ...accumulator, [block.id]: block.initialItems };
      },
      {} as typeof modelValue.value,
    );

    items.value = {
      ...cloneDeep(value),
      ...initialItems,
    };
  },
  { immediate: true },
);
</script>

<template>
  <div class="ContentListBox">
    <template
      v-for="(block, blockIndex) in blocks"
      :key="`ContentListBox-${block.id}`"
    >
      <div
        :class="[
          'ContentListBox-pane',
          block.isFeatured && 'ContentListBox-featured',
        ]"
      >
        <div class="ContentListBox-head">
          <span>{{ block.title }}</span>
        </div>
        <div class="ContentListBox-body">
          <div v-if="!items[block.id]?.length" class="ContentListBox-empty">
            <slot
              name="empty"
              :block-id="block.id"
              :empty-text="block.emptyText"
            >
              {{ block.emptyText || "Empty" }}
            </slot>
          </div>
          <ListBox
            v-else
            :data-items="items[block.id]"
            text-field="name"
            item="item"
          >
            <template #item="{ props }">
              <div class="ContentListBox-item">
                <slot name="item" :props="props">
                  <button
                    v-if="blockIndex > 0"
                    class="ContentListBox-action"
                    @click.prevent="
                      moveTo(
                        props.index,
                        block.id,
                        blocks[blockIndex - 1]?.id,
                        blocks[blockIndex - 1]?.isEditable,
                      )
                    "
                  >
                    <IconComponent size="22" symbol="move-left-74" />
                  </button>
                  <button
                    v-if="
                      blockIndex === blocks.length - 1 &&
                      props.dataItem.deleteable
                    "
                    class="ContentListBox-action"
                    @click.prevent="removeItem(block.id, props.index)"
                  >
                    <IconComponent
                      size="22"
                      symbol="trash"
                      class="text-error"
                    />
                  </button>
                  <div class="ContentListBox-content">
                    <slot
                      v-if="!isEdittingItem(block.id, props.index)"
                      name="item-content"
                      :props="props"
                    >
                      {{ props }}
                    </slot>
                    <div v-else class="ContentListBox-editContent">
                      <div class="ContentListBox-editContentActions">
                        <button class="ContentListBox-action">
                          <!-- ToDo. Create more dynamic refresh function -->
                          <IconComponent
                            size="16"
                            symbol="button-refresh-arrow"
                            @click.prevent="
                              moveTo(
                                props.index,
                                block.id,
                                blocks[blockIndex - 1]?.id,
                                blocks[blockIndex - 1]?.isEditable,
                              )
                            "
                          />
                        </button>
                        <button
                          class="ContentListBox-action"
                          @click.prevent="
                            () => {
                              setIsEdittingItem(block.id, props.index, false);
                              emits('update:modelValue', items);
                              emits('updated', items);
                              eventBus.emit(AppConstants.formFieldChangedEvent);
                              eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
                            }
                          "
                        >
                          <IconComponent size="16" symbol="save" />
                        </button>
                      </div>
                      <div v-if="slots['item-edit-content']">
                        <slot
                          name="item-edit-content"
                          :props="props"
                          :block-id="block.id"
                          :index="props.index"
                          :get-value="getValueSetter(block.id, props.index)"
                          :set-value="setValueSetter(block.id, props.index)"
                        ></slot>
                      </div>
                    </div>
                  </div>
                  <button
                    v-if="
                      blockIndex < blocks.length - 1 &&
                      props.dataItem.deleteable
                    "
                    class="ContentListBox-action"
                    @click.prevent="removeItem(block.id, props.index)"
                  >
                    <IconComponent
                      size="22"
                      symbol="trash"
                      class="text-error"
                    />
                  </button>
                  <button
                    v-if="blockIndex < blocks.length - 1"
                    class="ContentListBox-action"
                    @click.prevent="
                      moveTo(
                        props.index,
                        block.id,
                        blocks[blockIndex + 1]?.id ??
                          blocks[blockIndex - 1]?.id,
                        blocks[blockIndex + 1]?.isEditable ??
                          blocks[blockIndex - 1]?.isEditable,
                      )
                    "
                  >
                    <IconComponent size="22" symbol="move-right-4" />
                  </button>
                  <div
                    v-if="
                      slots['item-footer'] &&
                      !isEdittingItem(block.id, props.index)
                    "
                    class="ContentListBox-foot"
                  >
                    <slot name="item-footer" :props="props"></slot>
                  </div>
                  <div
                    v-if="
                      slots['item-edit-footer'] &&
                      isEdittingItem(block.id, props.index)
                    "
                    class="ContentListBox-foot"
                  >
                    <slot
                      name="item-edit-footer"
                      :props="props"
                      :block-id="block.id"
                      :index="props.index"
                      :get-value="getValueSetter(block.id, props.index)"
                      :set-value="setValueSetter(block.id, props.index)"
                    ></slot>
                  </div>
                </slot>
              </div>
            </template>
          </ListBox>

          <!-- Add Item -->
          <div v-if="!block.noAdd" class="ContentListBox-add">
            <div class="ContentListBox-addHead">
              <span class="ContentListBox-addTitle">
                {{ addTitle }}
              </span>
              <div class="ContentListBox-addActions">
                <button
                  v-if="!isAdding(block.id)"
                  class="text-primary"
                  @click.prevent="setIsAdding(block.id, true)"
                >
                  <IconComponent symbol="add-circle-27" size="20" />
                </button>
                <template v-else>
                  <button
                    class="text-error"
                    @click.prevent="
                      () => {
                        clearNewValues(block.id);
                        setIsAdding(block.id, false);
                      }
                    "
                  >
                    <IconComponent symbol="clear" size="14" />
                  </button>
                  <button
                    :class="[
                      !hasNewValues(block.id) ? 'text-neutral' : 'text-success',
                    ]"
                    :disabled="!hasNewValues(block.id)"
                    @click.prevent="
                      () => {
                        addItems(block.id);
                        setIsAdding(block.id, false);
                        emits('update:modelValue', items);
                        emits('updated', items);
                        eventBus.emit(AppConstants.formFieldChangedEvent);
                        eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
                      }
                    "
                  >
                    <IconComponent symbol="check-big" size="16" />
                  </button>
                </template>
              </div>
            </div>
            <div v-if="isAdding(block.id)" class="ContentListBox-addBody">
              <slot
                name="create-form"
                :block-id="block.id"
                :get-value="getNewValueSetter(block.id)"
                :set-value="setNewValueSetter(block.id)"
              ></slot>
            </div>
          </div>
        </div>
      </div>
      <div v-if="blockIndex < blocks.length - 1" class="ContentListBox-toolbar">
        <a href="javascript:;">
          <IconComponent
            symbol="arrow-transfer-horizontal-large-2-75"
            size="16"
          />
        </a>
      </div>
    </template>
  </div>
</template>

<style lang="scss">
.ContentListBox {
  $root: &;
  display: flex;
  gap: 20px;

  &-pane {
    flex: 1;
    border: 1px solid #97a1af;
    background: var(--color-white);
    overflow: hidden;
    border-radius: 8px;
    display: flex;
    flex-direction: column;

    &#{$root}-featured {
      #{$root}-head {
        background-color: #e5f4ff;
        color: var(--color-primary);
      }
    }
  }

  &-head {
    padding: 10px;
    background-color: #e4e7ec;
    font-weight: var(--font-weight-semi-bold);
    font-size: var(--font-size-sm);
    text-align: center;
  }

  &-body {
    display: flex;
    flex-direction: column;
    padding: 15px;
    flex: 1;
  }

  &-item {
    width: 100%;
    border: #97a1af 1px solid;
    border-radius: 8px;
    padding: 10px;
    display: flex;
    align-items: start;
    gap: 8px;
    margin-bottom: 10px;
    flex-wrap: wrap;
  }

  &-content {
    flex: 1;
  }

  &-foot {
    flex: 0 0 100%;
    max-width: 100%;
  }

  &-action {
    background: none;
    border: none;
    padding: 0;
    color: var(--color-primary);
  }

  &-empty {
    display: flex;
    align-items: center;
    justify-content: center;
    text-align: center;
    padding: 10px;
    color: #414e62;
    font-weight: var(--font-weight-semi-bold);
    flex: 1;
  }

  &-add {
    border-radius: 8px;
    padding: 10px;
    border: var(--color-primary) 1px solid;

    &Head {
      display: flex;
      align-items: center;
    }

    &Title {
      flex: 1;
      font-size: var(--font-size-sm);
      font-weight: var(--font-weight-semi-bold);
      color: var(--color-primary);
    }

    &Actions {
      display: flex;
      align-items: center;
      gap: 10px;

      button {
        background: none;
        border: none;
        margin: 0;
        padding: 0;
      }
    }

    &Body {
      margin-top: 10px;
    }
  }

  &-toolbar {
    display: flex;
    flex-direction: column;
    justify-content: center;
    gap: 10px;

    a {
      display: inline-block;
      width: 34px;
      height: 34px;
      line-height: 34px;
      background-color: #3fc6eb;
      text-align: center;
      border-radius: 50%;
    }
  }

  &-editContent {
    display: flex;
    flex-direction: column;
    gap: 8px;

    &Actions {
      display: flex;
      justify-content: flex-end;
      gap: 10px;
    }
  }

  .k-listbox {
    width: 100%;
    height: auto;
    flex: 1;

    .k-list-scroller {
      border: none;
    }

    .k-list-item {
      padding: 0;
      background: transparent !important;
      cursor: default;
    }

    .k-list-ul {
      padding: 0;
    }
  }
}
</style>
