<script lang="ts">
import { defineComponent } from "vue";
import { FirmPermissionCategorized } from "@/entities/FirmPermissionCategorized";

export default defineComponent({ 
  name: "ViewAffiliatePermissions",
  props: {
    items: {
      type: Array as () => FirmPermissionCategorized[],
      default: () => [], 
    }
  },
});
</script>

<template>
  <ModalComponentFlexible title="Permissions" :modalClass="{'ap-modal': true}" ref="modalElement">
		<div v-for="(item, index) in items" :key="index">
			<AccordionNested :title="item.categoryName" :items="item.permissions">
				<template #itemTitle="{item}">{{ item.permissionName }}</template>

				<template #item="{item}">
					<div class="types-content-wrapper k-d-flex k-flex-col">
						<div>
							<Label class="k-font-semibold font-size-xs k-text-black k-mb-1">Customer Type</Label>
							<template v-if="item?.customerTypes?.length">
								<div v-for="(customerType, j) in item.customerTypes" :key="j">
									<ul class="k-list-none k-m-0 k-p-0">
										<li class="k-mb-1">{{ customerType }}</li>
									</ul>
								</div>
							</template>
							<template v-else>
								<div>--</div>	
							</template>
						</div>

						<div>
							<Label class="k-font-semibold font-size-xs k-text-black">Investment Type</Label>
							<template v-if="item.investmentTypes?.length">
								<div v-for="(investmentType, h) in item.investmentTypes" :key="h">
									<ul class="k-list-none k-m-0 k-p-0">
										<li>{{ investmentType }}</li>
									</ul>
								</div>
							</template>
							<template v-else>
								<div>--</div>
							</template>
						</div>                            
					</div>
				</template>
			</AccordionNested>
		</div>
  </ModalComponentFlexible>
</template>

<style scoped>
:global(.ap-modal .k-dialog-titlebar) {
    padding: 15px;
}

:global(.ap-modal .k-dialog-titlebar .k-dialog-title) {
	display: flex;
	align-items: center;
	margin-block: -15px;	
}

:global(.ap-modal .k-dialog-content) {
    padding: 0 15px !important;
    gap: 20px;
    display: flex;
    flex-direction: column;
}

:global(.ap-modal .k-actions) {
	padding: 0 0 15px 0;
	margin: 0;
}
</style>