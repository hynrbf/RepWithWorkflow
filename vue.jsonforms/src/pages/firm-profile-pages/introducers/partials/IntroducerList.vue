<script lang="ts">
import { defineComponent, PropType } from "vue";
import { IntroducersEntity } from "@/entities/providers-and-introducers/IntroducersEntity";
import { GridPdfExport } from "@progress/kendo-vue-pdf";
import StaticList from "@/infra/StaticListService";
import { ProviderIntroducerDetails } from "@/entities/providers-and-introducers/ProviderIntroducerDetails";
import { ProviderRepresentative } from "@/entities/providers-and-introducers/ProviderRepresentative";
import { IHelperService, IHelperServiceInfo, } from "@/infra/dependency-services/helper/IHelperService";
import { container } from "tsyringe";
import { orderBy, SortDescriptor } from "@progress/kendo-data-query";
import { GridSortChangeEvent, GridPageChangeEvent } from "@progress/kendo-vue-grid";
import {AppConstants} from "@/infra/AppConstants";

export default defineComponent({
  name: "IntroducerList",
  components: {
    GridPdfExport,
  },
  props: {
    introducers: {
      type: Array as PropType<IntroducersEntity[]>,
      default: () => [],
    },
  },
  data() {
    return {
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      hasIntroducers: false,
      columnMenu: true,
      gridData: [] as IntroducersEntity[],
      skip: 0,
      take: 7,
      group: [],
      filter: null,
      isShowTasksModal: false,
      fcaFirmRefNo: "",
      isGridVisible: false,
      introducersInternal: [] as IntroducersEntity[],
      selectedProducts: [] as string[],
      selectedStatus: { label: "", value: "" },
      activeIntroducerTableGroup: "current",
      introducerTableGroups: [
        { title: "Current", value: "current" },
        { title: "All", value: "all" },
      ],  
      sortParams: [] as SortDescriptor[],  
      itemsToPDF: [] as object[],       
    };
  },
  mounted() {
    this.introducersInternal = this.introducers;
  },  
  computed: {
    ProviderRepresentative() {
      return ProviderRepresentative;
    },
    ProviderIntroducerDetails() {
      return ProviderIntroducerDetails;
    },
    introducerColumns() {
      return [
        {
          field: "details.name",
          title: "Introducer Name",
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
          field: "products",
          title: "Products",
          resizable: true,
        },
        {
          field: "referrals",
          title: "Referrals",
          resizable: true,
        },        
        {
          field: "introducer",
          title: "Introducer Representative",
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
          sortable: false,
        },
        {
          field: "actions",
          title: " ",
          className: "text-center",
        },
      ];
    },
  },
  methods: {
    orderBy,
    closeForm() {
      this.isShowTasksModal = false;
    },
    emitViewIntroducer(id: string) {
      this.$emit("viewIntroducerList", id);
    },
    emitEditIntroducer(id: string) {
      this.$emit("editIntroducerList", id);
    },
    emitAddIntroducer() {
      this.$emit("addIntroducer");
    },
    emitViewIntroducerFromCard(id: string) {
      this.$emit("viewIntroducerCard", id);
    },
    emitIntroducerFromCard(id: string) {
      this.$emit("editIntroducerCard", id);
    },
    emitFilterIntroducersByProductsStatus() {
      this.introducersInternal = this.introducers.filter((introducer) => {
        const products = introducer.products.map((product) => product.name);

        const hasSelectedProducts = this.selectedProducts.every(
          (selectedProduct) => products.includes(selectedProduct),
        );

        const hasSelectedStatus = introducer.status === this.selectedStatus.value;
        return hasSelectedProducts && hasSelectedStatus;
      });
    },
    emitSearchIntroducersByNameFRN(value: string) {
      if (!value) {
        this.introducersInternal = this.introducers;
        return;
      }

      this.introducersInternal = this.introducers.filter((introducer) => {
        if (!introducer.details.name || !introducer.details.fcaFirmRefNo) {
          return false;
        }

        const hasName = introducer.details.name
          .toLowerCase()
          .includes(value.toLowerCase());
        const hasFRN = introducer.details.fcaFirmRefNo
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
      this.introducersInternal = this.introducers;
      this.emitClearFilters();
    },      
    exportToPdf(values: IntroducersEntity[]) {
      const introducers = this.formatIntroducersPdfData(values);
      this.itemsToPDF = orderBy(introducers, this.sortParams);
      (this.$refs.gridPdfExport as any).save(this.itemsToPDF);
    },
    formatIntroducersPdfData(values: IntroducersEntity[]): object[] {
      return values.map((introducer) => {        
        return {
          ...introducer,
          "details.name": `${introducer.details?.name} `,
          "details.fcaFirmRefNo": introducer.details.fcaFirmRefNo,
          praAuthorised: introducer.details.praAuthorised,
          products: introducer.products,
          introducer: `${introducer.representative.forename} ${introducer.representative.surname}`,
          status: introducer.status,
          tasks: introducer.tasks,
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
      const identifier = `${AppConstants.introducersRoute}-introducerList${value}`;
      return identifier.replace(/\s+/g, "").replace("/", "");
    },
  },
});
</script>

<template>
  <GridPdfExport ref="gridPdfExport">
    <grid
      v-show="isGridVisible"
      :data-items="itemsToPDF"
      :columns = "introducerColumns">
    </grid>  
  </GridPdfExport>
  <GridCardTableWithSlot
    isSortable
    :columns="introducerColumns"
    :items="orderBy(introducersInternal, sortParams).slice(skip, skip + take)"
    :groups="introducerTableGroups"
    :no-actionbar="false"
    search-placeholder="Search Name, FRN"
    :add-button-text="'Add New Introducer'"
    v-model:activeGroup="activeIntroducerTableGroup"
    :sort="sortParams"
    :handleSortChange="handleSort"  
    :handlePageChange="handlePage"
    :pageable="true"
    :skip="skip"
    :take="take"
    :total="introducersInternal?.length ?? 0"          
    @add="emitAddIntroducer"
    @filter="emitFilterIntroducersByProductsStatus"
    @search="emitSearchIntroducersByNameFRN"
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
      <div
        style="
          color: #309161;
          font-size: 14px;
          font-weight: 400;
          text-decoration: underline;
        "
      >
        <a
          v-if="(details as ProviderIntroducerDetails)?.fcaFirmRefNo"
          :href="helperService.generateFCASearchUrl((details as ProviderIntroducerDetails)?.fcaFirmRefNo ?? '',)"
          target="_blank"
          rel="noopener noreferrer"
          style="
            color: #309161;
            font-size: 14px;
            font-weight: 400;
            text-decoration: underline;
          "
        >
          {{ (details as ProviderIntroducerDetails)?.fcaFirmRefNo }}
        </a>
      </div>
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
        :text="`${(representative as ProviderRepresentative)?.forename} ${(representative as ProviderRepresentative)?.surname}`"
      >
        <template #text>
          {{ (representative as ProviderRepresentative)?.forename }}
          {{ (representative as ProviderRepresentative)?.surname }}
        </template>
      </DynamicAvatarComponent>      
    </template>
    <template v-slot:cell-status="{ item: { status } }">
      <PillComponent
        :theme-color="status === 'Complete' ? 'success-tint' : 'warning-tint'"
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
          @click="emitViewIntroducer(id)"
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
          @click="emitEditIntroducer(id)"
        >
          <IconComponent symbol="edit-pen" size="20" />
        </KendoButton>
      </div>
    </template>

    <template v-slot:filter-fields="{}">
      <!-- ToDo. products multi select have checkbox -->
      <KendoMultiSelectInputComponent
        name="products"
        :id="setUniqueIdentifier('-products')"
        label="Products"
        :data-items="getProducts()"
        class="mb-3"
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
        :data-items="getStatuses()"
        class="mb-3"
        :value="selectedStatus"
        @onValueChange="handleStatusChange"
        />
      </template>

      <template v-slot:card>
        <div class="row mt-2">
          <div class="col-lg-4" v-for="(introducer, index) in introducers" :key="introducer.id">
            <CardComponent
              :cardId="introducer.id"
              :cardName="introducer.details?.name"
              :cardfcaFirmRefNo="introducer.details?.fcaFirmRefNo"
              :cardTasks="introducer.tasks"
              :cardIndex="index"
              @viewProvider="emitViewIntroducerFromCard"
              @editProvider="emitIntroducerFromCard"
            />
          </div>
        </div>
      </template>
  </GridCardTableWithSlot>
</template>
