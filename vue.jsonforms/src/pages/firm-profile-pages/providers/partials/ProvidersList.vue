<script lang="ts">
import { defineComponent, inject, PropType } from "vue";
import { ProvidersEntity } from "@/entities/providers-and-introducers/ProvidersEntity";
import { ProvidersTaskDetails } from "@/entities/providers-and-introducers/ProvidersTaskDetails";
import StaticList from "@/infra/StaticListService";
import { Emitter, EventType } from "mitt";
import { ProviderIntroducerDetails } from "@/entities/providers-and-introducers/ProviderIntroducerDetails";
import { ProviderRepresentative } from "@/entities/providers-and-introducers/ProviderRepresentative";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import {
  GridPageChangeEvent,
  GridSortChangeEvent,
} from "@progress/kendo-vue-grid";
import { orderBy, SortDescriptor } from "@progress/kendo-data-query";
import { container } from "tsyringe";
import { AppConstants } from "@/infra/AppConstants";

// ToDo. This component will be transfer soon to the Providers.vue
// since we combine list and card view in one component
export default defineComponent({
  name: "ProvidersList",
  props: {
    providers: {
      type: Array as PropType<ProvidersEntity[]>,
      default: () => [],
    },
  },
  data() {
    return {
      hasProviders: false,
      columnMenu: true,
      selectedField: "selected",
      expandField: "expanded",
      gridPageable: {
        buttonCount: 5,
        info: true,
        type: "numeric",
        pageSizes: true,
        previousNext: true,
      },
      gridData: [] as ProvidersEntity[],
      tasks: [] as ProvidersTaskDetails[],
      skip: 0,
      take: 7,
      group: [],
      sortParams: [] as SortDescriptor[],
      filter: null,
      isShowTasksModal: false,
      fcaFirmRefNo: "",
      providerTableGroups: [
        { title: "Current", value: "current" },
        { title: "All", value: "all" },
      ],
      activeProviderTableGroup: "current",
      providersInternal: [] as ProvidersEntity[],
      selectedProducts: [] as string[],
      selectedStatus: { label: "", value: "" },
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      itemsToPDF: [] as object[],
    };
  },
  mounted() {
    this.providersInternal = this.providers;
  },
  unmounted() {
    this.eventBus.off(AppConstants.filterGridEvent);
  },
  computed: {
    ProviderRepresentative() {
      return ProviderRepresentative;
    },
    ProviderIntroducerDetails() {
      return ProviderIntroducerDetails;
    },
    providersColumns() {
      return [
        {
          field: "details.name",
          title: "Provider Name",
          width: "200",
          resizable: true,
        },
        {
          field: "details.fcaFirmRefNo",
          title: "FRN",
          width: 70,
          resizable: true,
        },
        {
          field: "praAuthorised",
          title: "PRA Authorised",
          resizable: true,
        },
        {
          field: "products",
          title: "Products",
          resizable: true,
        },
        {
          field: "introducer",
          title: "Provider Representative",
          resizable: true,
        },
        {
          field: "status",
          title: "Status",
        },
        {
          field: "tasks",
          title: "Tasks",
          width: 70,
        },
        {
          field: "actions",
          title: " ",
          className: "text-center",
        },
      ];
    },
    columnsToPDF() {
      return this.providersColumns.filter(
        (column) => column.field !== "actions",
      );
    },
  },
  created() {
    //this.gridData = this.formatEmployeeData();
    const itemsToPDF = orderBy(this.gridData, this.sortParams);
    this.$emit("exportingPDF", this.columnsToPDF, itemsToPDF);
    this.eventBus.on(AppConstants.filterGridEvent, () => {
      if (this.skip == 0) {
        return;
      }

      this.skip = 0;
    });
  },
  methods: {
    orderBy,
    closeForm() {
      this.isShowTasksModal = false;
    },

    emitAddNewProvider() {
      this.$emit("addNewProvider");
    },

    emitViewProviderFromList(id: string) {
      this.$emit("viewProviderList", id);
    },

    emitEditProviderList(id: string) {
      this.$emit("editProviderList", id);
    },

    emitViewTasksFromCard(providerName: string, tasks: ProvidersTaskDetails[]) {
      this.$emit("viewTasksCard", providerName, tasks);
    },

    emitViewProviderFromCard(id: string) {
      this.$emit("viewProviderCard", id);
    },

    emitProviderFromCard(id: string) {
      this.$emit("editProviderCard", id);
    },

    emitFilterProvidersByProductsStatus() {
      this.providersInternal = this.providers.filter((provider) => {
        const products = provider.products.map((product) => product.name);

        const hasSelectedProducts = this.selectedProducts.every(
          (selectedProduct) => products.includes(selectedProduct),
        );

        const hasSelectedStatus = provider.status === this.selectedStatus.value;
        return hasSelectedProducts && hasSelectedStatus;
      });
    },

    emitSearchProvidersByNameFRN(value: string) {
      if (!value) {
        this.providersInternal = this.providers;
        return;
      }

      this.providersInternal = this.providers.filter((provider) => {
        if (!provider.details.name || !provider.details.fcaFirmRefNo) {
          return false;
        }

        const hasName = provider.details.name
          .toLowerCase()
          .includes(value.toLowerCase());
        const hasFRN = provider.details.fcaFirmRefNo
          .toLowerCase()
          .includes(value.toLowerCase());

        return hasName || hasFRN;
      });
    },

    emitClearFilters() {
      this.selectedProducts = [];
      this.selectedStatus = { label: "", value: "" };
    },

    emitCloseFilter() {
      this.providersInternal = this.providers;
      this.emitClearFilters();
    },

    exportToPdf(values: ProvidersEntity[]) {
      const providers = this.formatProvidersPdfData(values);
      this.itemsToPDF = orderBy(providers, this.sortParams);
      (this.$refs.gridPdfExport as any).save(this.itemsToPDF);
    },

    formatProvidersPdfData(values: ProvidersEntity[]): object[] {
      return values.map((provider) => {
        return {
          ...provider,
          "details.name": `${provider.details?.name} `,
          "details.fcaFirmRefNo": provider.details.fcaFirmRefNo,
          praAuthorised: provider.details.praAuthorised,
          products: provider.products,
          introducer:  `${provider.representative.forename} ${provider.representative.surname}`,
          status: provider.status,
          tasks: provider.tasks,
        };
      });
    },

    getProducts() {
      return StaticList.getProducts();
    },

    getStatuses() {
      return StaticList.getStatuses().map((type) => ({
        label: type,
        value: type,
      }));
    },

    handleStatusChange(selectedStatusItem: { label: string; value: string }) {
      this.selectedStatus = selectedStatusItem;
    },

    handleSort(e: GridSortChangeEvent) {
      this.sortParams = e.sort;
    },

    handlePage(e: GridPageChangeEvent) {
      this.skip = e.page.skip;
      this.take = e.page.take;
    },

    setUniqueIdentifier(value: string): string {
      const identifier = `${AppConstants.providersRoute}-list${value}`;
      return identifier.replace(/\s+/g, "").replace("/", "");
    },
  },
});
</script>

<template>
  <GridPdfExport ref="gridPdfExport">
    <grid v-show="false" :data-items="itemsToPDF" :columns="providersColumns">
    </grid>
  </GridPdfExport>
  <GridCardTableWithSlot
    isSortable
    :columns="providersColumns"
    :items="orderBy(providersInternal, sortParams).slice(skip, skip + take)"
    :groups="providerTableGroups"
    :no-actionbar="false"
    search-placeholder="Search Name, FRN"
    :add-button-text="'Add New Provider'"
    v-model:activeGroup="activeProviderTableGroup"
    :sort="sortParams"
    :handleSortChange="handleSort"
    :handlePageChange="handlePage"
    :pageable="true"
    :skip="skip"
    :take="take"
    :total="providersInternal?.length ?? 0"
    @add="emitAddNewProvider"
    @filter="emitFilterProvidersByProductsStatus"
    @search="emitSearchProvidersByNameFRN"
    @clear="emitClearFilters"
    @close="emitCloseFilter"
    @addExport="exportToPdf"
  >
    <template v-slot:cell-details.name="{ item: { details } }">
      <DynamicAvatarComponent
        rounded="full"
        type="text"
        :text="(details as ProviderIntroducerDetails)?.name ?? ''"
      >
        <template #text>
          {{ (details as ProviderIntroducerDetails)?.name }}
        </template>
      </DynamicAvatarComponent>
    </template>

    <template v-slot:cell-details.fcaFirmRefNo="{ item: { details } }">
      <div class="fca-firm-ref-no-cell">
        <a
          v-if="(details as ProviderIntroducerDetails)?.fcaFirmRefNo"
          :href="
            helperService.generateFCASearchUrl(
              (details as ProviderIntroducerDetails)?.fcaFirmRefNo ?? '',
            )
          "
          target="_blank"
          rel="noopener noreferrer"
        >
          {{ (details as ProviderIntroducerDetails)?.fcaFirmRefNo }}
        </a>
      </div>
    </template>

    <template v-slot:cell-praAuthorised="{ item: { details } }">
      <img
        v-if="(details as ProviderIntroducerDetails)?.praAuthorised"
        src="/approved-icon.svg"
        alt="Authorised"
        title="Authorised"
        width="16"
        height="16"
      />

      <img
        v-else
        src="/rejected-grey-icon.svg"
        alt="Non-Authorised"
        title="Non-Authorised"
        width="16"
        height="16"
      />
    </template>

    <template v-slot:cell-products="{ item: { products } }">
      {{ ""
      }}<!-- //products?.at(0).name -->
      <span style="text-decoration: underline">{{
        products?.length > 1 ? "+2" : ""
      }}</span>
    </template>

    <template v-slot:cell-introducer="{ item: { representative } }">
      <DynamicAvatarComponent
        rounded="full"
        type="text"
        :text="`${(representative as ProviderRepresentative)?.forename} ${
          (representative as ProviderRepresentative)?.surname
        }`"
      >
        <template #text>
          {{ (representative as ProviderRepresentative)?.forename }}
          {{ (representative as ProviderRepresentative)?.surname }}
        </template>
      </DynamicAvatarComponent>
    </template>

    <template v-slot:cell-status="{ item: { status } }">
      <PillComponent
        :themeColor="status === 'Complete' ? 'success-tint' : 'warning-tint'"
      >
        {{ status }}
      </PillComponent>
    </template>

    <template v-slot:cell-tasks="{ item: { tasks } }">
      <div class="text-center">
        <span class="row-name">{{ tasks?.length ?? 0 }}</span>
      </div>
    </template>

    <template v-slot:cell-actions="{ item: { id } }">
      <div class="d-inline-flex gap-2">
        <KendoButton
          type="button"
          size="small"
          rounded="full"
          shape="square"
          theme-color="light"
          title="View"
          class="ActionButton"
          @click="emitViewProviderFromList(id)"
        >
          <IconComponent symbol="eye" size="20" />
        </KendoButton>

        <KendoButton
          type="button"
          size="small"
          rounded="full"
          shape="square"
          theme-color="light"
          title="Edit"
          class="ActionButton"
          @click="emitEditProviderList(id)"
        >
          <IconComponent symbol="edit-pen" size="20" />
        </KendoButton>
      </div>
    </template>

    <template v-slot:filter-fields>
      <!-- ToDo. products multi select have checkbox -->
      <KendoMultiSelectInputComponent
        name="products"
        :id="setUniqueIdentifier('-products')"
        label="Products"
        class="mb-3"
        :dataItems="getProducts()"
        :isRequired="false"
        :value="selectedProducts"
        @onValueChange="
          (value: any) => {
            selectedProducts = value;
          }
        "
      />

      <KendoDropdownListComponent
        name="status"
        :id="setUniqueIdentifier('-status')"
        label="Status"
        class="mb-3"
        :dataItems="getStatuses()"
        :value="selectedStatus"
        :isRequired="false"
        @onValueChange="handleStatusChange"
      />
    </template>

    <template v-slot:card>
      <div class="row mt-2">
        <div
          class="col-lg-4"
          v-for="(provider, index) in providers"
          :key="provider.id"
        >
          <CardComponent
            :cardId="provider.id"
            :cardName="provider.details?.name"
            :cardfcaFirmRefNo="provider.details?.fcaFirmRefNo"
            :cardTasks="provider.tasks"
            :cardIndex="index"
            @viewProvider="emitViewProviderFromCard"
            @editProvider="emitProviderFromCard"
            @viewTasks="emitViewTasksFromCard"
          />
        </div>
      </div>
    </template>
  </GridCardTableWithSlot>
</template>

<style scoped lang="scss">
.icon {
  display: inline-block;
  width: 50px;
  height: 50px;
}

.circle {
  width: 100%;
  height: 100%;
  border-radius: 50%;
  overflow: hidden;
  background-color: #cccccc; /* Default background color */
  display: flex;
  justify-content: center;
  align-items: center;
}

.circle img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.fca-firm-ref-no-cell,
.fca-firm-ref-no-cell > a {
  color: var(--color-success-700);
  font-size: var(--font-size-sm);
  font-weight: 400;
  text-decoration: underline;
}
</style>
