<script setup lang="ts">
import { ref } from "vue";
import type { CollapsibleModel } from "@/components/models/CollapsibleModel";

defineProps<{ 
  title: string, 
  items: CollapsibleModel[], 
  expandIcon?: string, 
  collapseIcon?: string, 
  hideIndicator?: boolean
}>();

const isCollapsed = ref(true);

const handleToggleChange = (e: boolean) => {
  isCollapsed.value = e;
};
</script>

<template>
	<KendoAccordionItemComponent @onToggle="handleToggleChange" :title="title" :class="{'accordion-nested': true, 'is-collapsed': isCollapsed}">
		<CollapsiblePanel
			class="item-collapsible"
			v-for="(item, i) in items"
			:key="i"
			:collapse="true"
			expandIcon="/accordion-arrow-down.svg"
			collapseIcon="/accordion-arrow-up.svg"
			:hideIndicator="true"
		>
		
			<template #title>
				<slot name="itemTitle" :item="item"></slot>
				{{ item.title }}
			</template>
			<slot name="content">{{ item.content }}</slot>
			<slot name="item" :item="item"></slot>
		</CollapsiblePanel>                        
	</KendoAccordionItemComponent>
</template>

<style scoped>
.accordion-nested {
	border: 0;
	padding: 0;
	gap: 0 !important;
}

.accordion-nested .k-stack-layout {
    padding: 0;
	gap: 0 !important;
	border: 0.3px solid #97A1AF;
}

:global(.accordion-nested .k-stack-layout .section-title) {
	font-size: var(--font-size-default);
}

:global(.accordion-nested .k-stack-layout.k-hstack) {
    padding: 4px 15px;
	background: #F9FAFB;
	border-radius: 8px;
	border: 1px solid rgba(0, 0, 0, 0.08);
}

:global(.accordion-nested.is-collapsed .k-stack-layout.k-hstack) {
	border-bottom-left-radius: 0;
	border-bottom-right-radius: 0;
}

.accordion-nested .k-expander {
	border-radius: 0;
}

.accordion-nested .k-expander.k-card {
	margin-top: 0;
	border-top: none;
}

:global(.accordion-nested .k-expander .k-expander-header) {
	font-size: var(--font-size-sm);	
	padding-block: 4px;
}

.accordion-nested .k-expander.k-focus {
	box-shadow: none;
}

:global(.accordion-nested .k-expander .k-expander-indicator .k-svg-icon) {
	width: 32px;
	height: 32px;
}

:global(.accordion-nested .k-expander .k-expander-content) {
	padding-block: 10px;
	padding-inline: 15px;
}

.accordion-nested .k-expander:last-child {
	border-bottom-left-radius: 8px;
	border-bottom-right-radius: 8px;
}

:global(.accordion-nested .k-expander .k-right-5) {
	right: 22px;
}

.types-content-wrapper {
	gap: 20px;
}
</style>