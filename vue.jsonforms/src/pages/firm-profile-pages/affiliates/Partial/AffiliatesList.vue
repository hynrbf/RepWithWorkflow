<script lang="ts">
import { defineComponent, PropType } from "vue";
import { Affiliate } from "@/entities/Affiliate/Affiliate";
import StaticList from "@/infra/StaticListService";
import { AffiliateDetails } from "@/entities/Affiliate/AffiliateDetails";
import { ProviderRepresentative } from "@/entities/providers-and-introducers/ProviderRepresentative";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import { container } from "tsyringe";
import { orderBy, SortDescriptor } from "@progress/kendo-data-query";
import {
  GridSortChangeEvent,
  GridPageChangeEvent,
} from "@progress/kendo-vue-grid";
import {AppConstants} from "@/infra/AppConstants";

export default defineComponent({
  name: "AffiliatesList",
  props: {
    affiliatesValues: {
      type: Array as PropType<Affiliate[]>,
      default: () => [],
    }
  },
  data() {
    return {
      affiliatesInternal: [] as Affiliate[],
      selectedProducts: [] as string[],
      selectedStatus: { label: "", value: "" },
      activeAffiliateTableGroup: "current",
      affiliateTableGroups: [
        { title: "Current", value: "current" },
        { title: "All", value: "all" },
      ],
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      sortParams: [] as SortDescriptor[],
      skip: 0,
      take: 7,
      itemsToPDF: [] as object[],
      isGridVisible: false,
    };
  },
  mounted() {
    this.affiliatesInternal = this.affiliatesValues;
  },
  computed: {
    AffiliateDetails() {
      return AffiliateDetails;
    },
    ProviderRepresentative() {
      return ProviderRepresentative;
    },
    affiliateColumns() {
      return [
        {
          field: "details.name",
          title: "Affiliate Name",
          width: 200,
        },
        {
          field: "details.firmReferenceNumber",
          title: "FRN",
          width: 200,
        },
        {
          field: "details.isPRAAuthorised",
          title: "PRA Authorised",
          width: 200,
        },
        {
          field: "products",
          title: "Products",
          width: 160,
        },
        {
          field: "affiliate",
          title: "Affiliate Representative",
          width: 200,
        },
        {
          field: "status",
          title: "Status",
          width: 140,
        },
        {
          field: "tasks",
          title: "Task(s)",
          width: 160,
          sortable: false,
        },
        {
          field: "actions",
          title: " ",
          width: 160,
          sortable: false,
        },
      ];
    },
  },
  methods: {
    orderBy,
    emitAddNewAffiliate() {
      this.$emit("add-new-affiliate");
    },
    emitViewAffiliateFormList(id: string) {
      this.$emit("view-affiliate-list", id);
    },
    emitEditAffiliateList(id: string) {
      this.$emit("edit-affiliate-list", id);
    },
    emitViewAffiliateFromCard(id: string) {
      this.$emit("view-affiliate-card", id);
    },
    emitAffiliateFromCard(id: string) {
      this.$emit("edit-affiliate-card", id);
    },
    emitFilterAffiliatesByProductsStatus() {
      this.affiliatesInternal = this.affiliatesValues.filter((affiliate) => {
        const products = affiliate.products.map((product) => product.name);

        const hasSelectedProducts = this.selectedProducts.every(
          (selectedProduct) => products.includes(selectedProduct),
        );

        const hasSelectedStatus =
          affiliate.status === this.selectedStatus.value;
        return hasSelectedProducts && hasSelectedStatus;
      });
    },
    emitSearchAffiliatesByNameFRN(value: string) {
      if (!value) {
        this.affiliatesInternal = this.affiliatesValues;
        return;
      }

      this.affiliatesInternal = this.affiliatesValues.filter((affiliate) => {
        if (!affiliate.details.name || !affiliate.details.firmReferenceNumber) {
          return false;
        }

        const hasName = affiliate.details.name
          .toLowerCase()
          .includes(value.toLowerCase());
        const hasFRN = affiliate.details.firmReferenceNumber
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
      this.affiliatesInternal = this.affiliatesValues;
      this.emitClearFilters();
    },
    exportToPdf(values: Affiliate[]) {
      const affiliates = this.formatAffiliatesPdfData(values);
      this.itemsToPDF = orderBy(affiliates, this.sortParams);
      (this.$refs.gridPdfExport as any).save(this.itemsToPDF);
    },
    formatAffiliatesPdfData(values: Affiliate[]): object[] {
      return values.map((affiliate) => {        
        return {
          ...affiliate,
          "details.name": `${affiliate.details?.name} `,
          "details.firmReferenceNumber": affiliate.details.firmReferenceNumber,
          "details.isPRAAuthorised": affiliate.details.isPRAAuthorised,
          products: affiliate.products,
          affiliate: `${affiliate.representative.forename} ${affiliate.representative.surname}`,
          status: affiliate.status,
          tasks: affiliate.tasks,
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
      const identifier = `${AppConstants.affiliatesRoute}${value}`;
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
      :columns="affiliateColumns"
    >
    </grid>
  </GridPdfExport>
  <GridCardTableWithSlot
    isSortable
    :columns="affiliateColumns"
    :items="orderBy(affiliatesInternal, sortParams).slice(skip, skip + take)"
    :groups="affiliateTableGroups"
    :no-actionbar="false"
    search-placeholder="Search Name, FRN"
    :add-button-text="'Add New Affiliate'"
    v-model:activeGroup="activeAffiliateTableGroup"
    :sort="sortParams"
    :handleSortChange="handleSort"
    :handlePageChange="handlePage"
    :pageable="true"
    :skip="skip"
    :take="take"
    :total="affiliatesInternal?.length ?? 0"
    @add="emitAddNewAffiliate"
    @filter="emitFilterAffiliatesByProductsStatus"
    @search="emitSearchAffiliatesByNameFRN"
    @clear="emitClearFilters"
    @close="emitCloseFilter"
    @addExport="exportToPdf"
  >
    <template v-slot:cell-details.name="{ item: { details } }">
      <DynamicAvatarComponent
        rounded="full"
        type="text"
        :text="(details as AffiliateDetails)?.name ?? ''"
      >
        <template #text>
          {{ (details as AffiliateDetails)?.name }}
        </template>
      </DynamicAvatarComponent>
    </template>

    <template v-slot:cell-details.firmReferenceNumber="{ item: { details } }">
      <div
        style="
          color: #309161;
          font-size: 14px;
          font-weight: 400;
          text-decoration: underline;
        "
      >
        <a
          v-if="details?.firmReferenceNumber"
          :href="
            helperService.generateFCASearchUrl(
              (details as AffiliateDetails)?.firmReferenceNumber ?? '',
            )
          "
          target="_blank"
          rel="noopener noreferrer"
          style="
            color: #309161;
            font-size: 14px;
            font-weight: 400;
            text-decoration: underline;
          "
        >
          {{ (details as AffiliateDetails)?.firmReferenceNumber }}
        </a>
      </div>
    </template>

    <template v-slot:cell-details.isPRAAuthorised="{ item: { details } }">
      <img
        v-if="(details as AffiliateDetails)?.isPRAAuthorised"
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

    <template v-slot:cell-affiliate="{ item: { representative } }">
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
        :theme-color="status === 'Complete' ? 'success-tint' : 'warning-tint'"
      >
        {{ "Onboarding" }}
      </PillComponent>
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
          @click="emitViewAffiliateFormList(id)"
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
          @click="emitEditAffiliateList(id)"
        >
          <IconComponent symbol="edit-pen" size="20" />
        </KendoButton>
      </div>
    </template>

    <template v-slot:filter-fields="{}">
      <!-- ToDo. products multi select have checkbox -->
      <KendoMultiSelectInputComponent
        :id="setUniqueIdentifier(`-affiliate-products`)"
        :name="setUniqueIdentifier(`-affiliate-products`)"
        label="Products"
        :data-items="getProducts()"
        class="mb-3"
        :value="selectedProducts"
        @onValueChange="
          (value: string[]) => {
            if (value.length > 0) {
              selectedProducts = value;
            }
          }
        "
      />

      <KendoDropdownListComponent
        :id="setUniqueIdentifier(`-affiliate-status`)"
        :name="setUniqueIdentifier(`-affiliate-status`)"
        label="Status"
        :data-items="getStatuses()"
        class="mb-3"
        :value="selectedStatus"
        @onValueChange="handleStatusChange"
      />
    </template>

    <template v-slot:card>
      <div class="row mt-2">
        <div
          class="col-lg-4"
          v-for="(affiliate, index) in affiliatesValues"
          :key="affiliate.id"
        >
          <CardComponent
            :cardId="affiliate.id"
            :cardName="affiliate.details?.name"
            :cardfcaFirmRefNo="affiliate.details?.firmReferenceNumber"
            :cardTasks="affiliate.tasks"
            :cardIndex="index"
            @view-provider="emitViewAffiliateFromCard"
            @edit-provider="emitAffiliateFromCard"
          />
        </div>
      </div>
    </template>
  </GridCardTableWithSlot>
</template>
