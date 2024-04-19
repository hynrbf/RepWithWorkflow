<script setup lang="ts">
import { toRefs, onMounted } from "vue";
import {
  PageSection,
  usePageSectionWithAnchor,
} from "@/composables/usePageSectionWithAnchor";
import { watch } from "vue";

export interface NavPillItem {
  id: string;
  label: string;
  icon?: string;
  active?: boolean;
  anchorTo?: string;
  onClick?: () => void;
}

const props = withDefaults(
  defineProps<{
    items: NavPillItem[];
    anchorable?: boolean;
    id?: string;
    hiddenItems?: string[];
  }>(),
  {
    items: () => [],
    id: () => `NavPill-${(Math.random() + 1).toString(36).substring(7)}`,
    hiddenItems: () => [],
  },
);

const emits = defineEmits<{
  (event: "update:items", items: NavPillItem[]): void;
}>();

const { items, anchorable, id, hiddenItems } = toRefs(props);

const { activePageSectionId, setPageSections, scrollToPageSection } =
  usePageSectionWithAnchor([], `#${id.value}`, 2);

onMounted(() => {
  if (anchorable.value) {
    setPageSections(
      items.value.map(({ anchorTo }) => ({
        id: anchorTo || "",
        top: 0,
      })) as PageSection[],
    );
  }
});

const handleClick = (index: number) => {
  const { anchorTo, onClick } = items.value[index];

  if (anchorable.value && anchorTo) {
    scrollToPageSection(anchorTo || "");
    return;
  }

  if (typeof onClick === "function") {
    onClick();
  }
};

const isActiveItem = (index: number) => {
  const { anchorTo, active } = items.value[index];

  if (anchorable.value) {
    return anchorTo === activePageSectionId.value;
  }

  return active;
};

watch(activePageSectionId, (value) => {
  if (!anchorable.value) {
    return;
  }

  emits(
    "update:items",
    items.value.map((item) => {
      item.active = item.anchorTo === value;
      return item;
    }),
  );

  items.value.forEach((item) => {
    document
      .querySelector(`#${item.anchorTo}`)
      ?.classList.remove("is-anchored");
    if (item.anchorTo === value) {
      document.querySelector(`#${item.anchorTo}`)?.classList.add("is-anchored");
    }
  });
});
</script>

<template>
  <ul :id="id" class="NavPill">
    <li
      v-for="(item, index) in items"
      :key="`nav-pill-${index}-${item.id}`"
      :class="['NavPill-item', isActiveItem(index) && 'is-active']"
      v-show="!hiddenItems.includes(item.id)"
    >
      <a
        href="javascript:;"
        class="NavPill-link"
        @click.prevent="handleClick(index)"
      >
        <slot name="icon" v-if="item.icon">
          <IconComponent :symbol="item.icon" size="18" />
        </slot>
        <span class="is-truncated">
          <slot name="label">{{ item.label }}</slot>
        </span>
      </a>
    </li>
  </ul>
</template>