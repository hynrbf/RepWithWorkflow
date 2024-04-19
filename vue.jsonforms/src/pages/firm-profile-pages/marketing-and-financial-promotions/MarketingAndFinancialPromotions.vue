<script lang="ts">
import { defineComponent, defineAsyncComponent, inject } from "vue";
import { container } from "tsyringe";
import { Emitter, EventType } from "mitt";
import { GridPageChangeEvent, GridSortChangeEvent } from "@progress/kendo-vue-grid";
import { SortDescriptor, orderBy, groupBy } from "@progress/kendo-data-query";
import isEmpty from "lodash/isEmpty";
import { mapActions, mapState } from "pinia";
import moment from "moment";
import { v4 as uuid } from "uuid";
import { FinancialPromotion } from "@/entities/financial-promotion/FinancialPromotion";
import { FinancialPromotionStatus } from "@/entities/financial-promotion/FinancialPromotionStatus";
import { FinancialPromotionType } from "@/entities/financial-promotion/FinancialPromotionType";
import { FinancialPromotionModel } from "@/pages/firm-profile-pages/marketing-and-financial-promotions/model/FinancialPromotionModel";
import { MediaMarketingOutlet } from "@/entities/media-marketing-outlet/MediaMarketingOutlet";
import { MediaMarketingOutletModel } from "@/pages/firm-profile-pages/marketing-and-financial-promotions/model/MediaMarketingOutletModel";
import { useCustomerStore } from "@/stores/useCustomerStore";
import { useMediaMarketingOutletStore } from "@/stores/media-marketing-outlet/useMediaMarketingOutletStore";
import { useFinancialPromotionStore } from "@/stores/financial-promotion/useFinancialPromotionStore";
import { AppConstants } from "@/infra/AppConstants";
import StaticList from "@/infra/StaticListService";
import {
  IWebScrapeService,
  IWebScrapeServiceInfo,
} from "@/infra/dependency-services/rest/web-scrape/IWebScrapeService";
import { usePageLifeCycleStore } from "@/stores/progress-bar/usePageLifeCycleStore";
import {
  NotificationType,
  useNotification,
} from "@/composables/useNotification";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import {
  IUserSubmittedChangesService,
  IUserSubmittedChangesServiceInfo,
} from "@/infra/dependency-services/user-submission/IUserSubmittedChangesService";
import { useAlert, AlertType } from "@/composables/useAlert";
 
export default defineComponent({
  components: {
    ViewOutletModal: defineAsyncComponent(
      () => import("./partials/ViewOutletModal.vue"),
    ),
    AddOutletModal: defineAsyncComponent(
      () => import("./partials/AddOutletModal.vue"),
    ),
    AddPromotionModal: defineAsyncComponent(
      () => import("./partials/AddPromotionModal.vue"),
    ),
    OutletActions: defineAsyncComponent(
      () => import("./partials/OutletActions.vue"),
    ),
    PromotionActions: defineAsyncComponent(
      () => import("./partials/PromotionActions.vue"),
    ),
    ViewPromotionModal: defineAsyncComponent(
      () => import("./partials/ViewPromotionModal.vue"),
    ),
    MediaDropdown: defineAsyncComponent(
      () => import("./partials/MediaDropdown.vue"),
    ),
    PromotionTableFilters: defineAsyncComponent(
      () => import("./partials/PromotionTableFilters.vue"),
    ),
  },
  data() {
    return {
      isLoading: false,
      isShowSavingText: false,
      moment,
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      eventBusFormSaved: inject("$eventBusService") as Emitter<
        Record<EventType, boolean>
      >,
      currentTab: 0,
      tabs: [
        {
          title: this.$t("marketingFinancialPage-mediaMarketing"),
          content: "mediaMarketing",
        },
        {
          title: this.$t("marketingFinancialPage-financialPromotions"),
          content: "financialPromotions",
        },
      ],
      activeOutletTableGroup: "active",
      outletTableGroups: [
        { title: "Active", value: "active" },
        { title: "All", value: "all" },
      ],
      isViewOutletModalVisible: false,
      isAddOutletModalVisible: false,
      isFetchingOutlet: false,
      isAddingOutlet: false,
      isArchivingOutlet: false,
      archiveItem: "" as string,
      activePromotionTableGroup: "pending",
      promotionTableGroups: [
        { title: "Pending", value: "pending" },
        { title: "Approved", value: "approved" },
        { title: "Rejected", value: "rejected" },
        { title: "All", value: "all" },
      ],
      isViewPromotionModalVisible: false,
      isAddPromotionModalVisible: false,
      isFetchingPromotion: false,
      isSavingPromotion: false,
      currentPromotionValues: undefined as FinancialPromotion | undefined,
      webScrapeService: container.resolve<IWebScrapeService>(
        IWebScrapeServiceInfo.name,
      ),
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      updatingPromotionIds: [] as string[],
      promotionTableLoading: false,
      marketingOutletTableLoading: false,
      promotionTableSettings: {
        pageSize: 10,
        pageNumber: 1,
        skip: 0,
        take: 10,
        filters: {} as Record<string, unknown>,
        sorts: [] as SortDescriptor[],
      },
      mediaMarketingTableSettings: {
        pageSize: 10,
        pageNumber: 1,
        skip: 0,
        take: 10,
        filters: {} as Record<string, unknown>,
        sorts: [] as SortDescriptor[],
      },
      isSavingAlertOpened: false,
      userSubmittedChangesService:
        container.resolve<IUserSubmittedChangesService>(
          IUserSubmittedChangesServiceInfo.name,
        ),
    };
  },
  computed: {
    AppConstants() {
      return AppConstants;
    },
    ...mapState(useCustomerStore, ["currentFirmName", "currentCustomer"]),
    ...mapState(useMediaMarketingOutletStore, [
      "mediaMarketingOutlet",
      "mediaMarketingOutlets",
      "mediaMarketingOutletsArchived",
      "mediaMarketingOutletsUnarchived",
    ]),
    ...mapState(useFinancialPromotionStore, [
      "financialPromotion",
      "financialPromotions",
      "financialPromotionTotal",
    ]),

    activeOutlet() {
      return this.mediaMarketingOutlet;
    },

    outlets() {
      return this.orderBy(
        this.mediaMarketingOutlets,
        this.mediaMarketingTableSettings.sorts
      );
    },

    outletColumns() {
      return [
        {
          field: "name",
          title: "Account Name",
        },
        {
          field: "approved",
          title: "Approved",
          className: "text-center",
          sortable: false,
        },
        {
          field: "pending",
          title: "Pending",
          className: "text-center",
          sortable: false,
        },
        {
          field: "rejected",
          title: "Rejected",
          className: "text-center",
          sortable: false,
        },
        {
          field: "createdAt",
          title: "Most Recent Post",
          className: "text-center",
        },
        {
          field: "updatedAt",
          title: "Most Recent Approval",
          className: "text-center",
        },
        {
          field: "actions",
          title: " ",
        },
      ];
    },

    outletItems(): MediaMarketingOutletModel[] {
      let outletItems = this.outlets.filter(
        (item: MediaMarketingOutletModel) => !item.archived
      ) as MediaMarketingOutletModel[];

      if (this.activeOutletTableGroup === "all") {
        this.orderBy(
          this.mediaMarketingOutlets.filter(
            (item: MediaMarketingOutletModel) => item.archived
          ),
          this.mediaMarketingTableSettings.sorts
        ).map((item: MediaMarketingOutletModel) => {
          outletItems.push({
            ...item,
            inactive: item.archived,
          });
        });
      }

      return outletItems;
    },

    activePromotion() {
      return this.financialPromotion;
    },

    promotions() {
      return this.financialPromotions;
    },

    promotionColumns() {
      const defaults = [
        {
          field: "media",
          title: "Media",
        },
        {
          field: "type",
          title: "Type",
        },
        {
          field: "contentOwner",
          title: "Content Owner",
        },
        {
          field: "moderator",
          title: "Moderator",
        },
      ];

      switch (this.activePromotionTableGroup) {
        case "pending":
          return [
            ...defaults,
            {
              field: "approvalType",
              title: "Approval Type",
            },
            {
              field: "contentStatus",
              title: "Content Status",
            },
            {
              field: "approvalRequiredDate",
              title: "Approval Required Date",
            },
            {
              field: "actions",
              title: " ",
            },
          ];

        case "approved":
          return [
            ...defaults,
            {
              field: "contentStatus",
              title: "Content Status",
            },
            {
              field: "dateApproved",
              title: "Date Approved",
            },
            {
              field: "dateLive",
              title: "Date Live",
            },
            {
              field: "actions",
              title: " ",
            },
          ];

        case "rejected":
          return [
            ...defaults,
            {
              field: "contentStatus",
              title: "Content Status",
            },
            {
              field: "dateRejected",
              title: "Date Rejected",
            },
            {
              field: "dateLive",
              title: "Date Live",
            },
            {
              field: "actions",
              title: " ",
            },
          ];

        default:
          return [
            ...defaults,
            {
              field: "contentStatus",
              title: "Content Status",
            },
            {
              field: "status",
              title: "Status",
            },
            {
              field: "date",
              title: "Date",
            },
            {
              field: "dateLive",
              title: "Date Live",
            },
            {
              field: "actions",
              title: " ",
            },
          ];
      }
    },

    promotionItems() {
      switch (this.activePromotionTableGroup) {
        case "pending":
          return this.promotions.filter(
            (promotion) =>
              promotion.approvalStatus === FinancialPromotionStatus.Pending,
          );
        case "approved":
          return this.promotions.filter(
            (promotion) =>
              promotion.approvalStatus === FinancialPromotionStatus.Approved,
          );
        case "rejected":
          return this.promotions.filter(
            (promotion) =>
              promotion.approvalStatus === FinancialPromotionStatus.Rejected,
          );
        default:
          return this.promotions.map((promotion: FinancialPromotionModel) => {
            if (
              promotion.approvalStatus === FinancialPromotionStatus.Approved
            ) {
              promotion.approved = true;
            }
            if (
              promotion.approvalStatus === FinancialPromotionStatus.Rejected
            ) {
              promotion.rejected = true;
            }
            if (promotion.approvalStatus === FinancialPromotionStatus.Pending) {
              promotion.pending = true;
            }
            return promotion;
          });
      }
    },

    promotionTypeOptions(): Array<{
      label: string;
      value: FinancialPromotionType;
    }> {
      return [
        {
          label: `${this.currentFirmName} Products/Services`,
          value: "self",
        },
        {
          label: "Authorised Third Party Products/Services",
          value: "authorised-3rd-party",
        },
        {
          label: "Non-Authorised Third Party Products/Services",
          value: "non-authorised-3rd-party",
        },
      ];
    },

    approvalTypeOptions() {
      return StaticList.getPostApprovalTypes().map((type) => ({
        label: type,
        value: type,
      }));
    },

    contentStatusOptions() {
      return [
        {
          label: "Live",
          value: "Live",
        },
        {
          label: "Not Live",
          value: "Not Live",
        },
      ];
    },
  },
  created() {
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameCreated);
  },
  async mounted() {
    this.eventBus.emit(AppConstants.bottomBarEnableEvent, true);
    this.eventBus.on(AppConstants.formFieldPageLevelChangedEvent, () => {
      this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi =
        false;
    });
    this.eventBusFormSaved.on(AppConstants.formSavedEvent, (isAutoNext) => {
      this.handleSaveAsync(isAutoNext);
    });
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameMounted);
    this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi = false;
    this.fetchDataAsync();
  },
  unmounted() {
    this.isLoading = false;
    this.eventBusFormSaved.off(AppConstants.formSavedEvent);
  },
  setup() {
    const pageLifeCycleStore = usePageLifeCycleStore();
    const { changeLifeCycleName } = pageLifeCycleStore;

    return {
      changeLifeCycleName,
    };
  },
  methods: {
    orderBy,
    groupBy,
    ...mapActions(useCustomerStore, ["updateCustomerByEmailAsync"]),
    ...mapActions(useMediaMarketingOutletStore, [
      "createMediaMarketingOutletAsync",
      "updateMediaMarketingOutletAsync",
      "fetchMediaMarketingOutletAsync",
      "fetchMediaMarketingOutletsAsync",
      "deleteMediaMarketingOutletAsync",
      "appendMediaMarketingOutlet",
      "amendMediaMarketingOutlet",
      "getMediaMarketingOutlet",
      "removeMediaMarketingOutlet",
    ]),
    ...mapActions(useFinancialPromotionStore, [
      "fetchFinancialPromotionAsync",
      "fetchFinancialPromotionsAsync",
      "deleteFinancialPromotionAsync",
      "appendFinancialPromotion",
      "amendFinancialPromotion",
      "getFinancialPromotion",
      "removeFinancialPromotion",
      "setFinancialPromotion",
      "saveOrUpdateFinancialPromotionAsync",
    ]),

    async fetchDataAsync() {
      this.isLoading = true;
      await Promise.all([
        this.fetchMediaMarketingOutletsAsync(),
        this.fetchPromotionsDataAsync(),
      ]);
      this.isLoading = false;
    },

    async fetchPromotionsDataAsync() {
      const { pageSize, pageNumber, filters, sorts } =
        this.promotionTableSettings;
      this.promotionTableLoading = true;
      try {
        await this.fetchFinancialPromotionsAsync({
          pageSize,
          pageNumber,
          ...filters,
          // Note. API currently only accepts 1 sorting
          sortPropertyName: !isEmpty(sorts) ? sorts[0].field : undefined,
          sortDirection: !isEmpty(sorts) ? sorts[0].dir : undefined,
        });
      } catch {
        throw new Error("Something went wrong.");
      } finally {
        this.promotionTableLoading = false;
      }
    },

    selectTab(event: any) {
      this.currentTab = event.selected;
    },

    async addOutletAsync(payload: Partial<MediaMarketingOutlet>) {
      const mediaMarketingOutlet = new MediaMarketingOutlet();
      mediaMarketingOutlet.id = uuid();
      mediaMarketingOutlet.url = payload.url;
      mediaMarketingOutlet.name = payload.name;
      mediaMarketingOutlet.owner = payload.owner;
      mediaMarketingOutlet.archived = false;
      mediaMarketingOutlet.platform = payload.platform;
      mediaMarketingOutlet.createdAt = moment().unix();
      mediaMarketingOutlet.updatedAt = moment().unix();
      this.isAddingOutlet = true;
      try {
        await this.createMediaMarketingOutletAsync(mediaMarketingOutlet);
        this.isAddOutletModalVisible = false;
        this.isAddingOutlet = false;
        this.webScrapeService.registerMediaAsync(
          mediaMarketingOutlet.id,
          mediaMarketingOutlet.url,
          `${this.currentCustomer.id}`,
        );
      } catch (error) {
        // ToDo. Add error handler
      }
    },

    viewOutlet(id: string) {
      this.isViewOutletModalVisible = true;
      this.isFetchingOutlet = true;
      this.getMediaMarketingOutlet(id, true);
      this.isFetchingOutlet = false;
    },

    async archiveOutlet(id: string, isArchived: boolean) {
      useAlert({
        content: this.$t(
          isArchived
            ? "marketingFinancialPage-confirmArchiveOutlet"
            : "marketingFinancialPage-confirmUnarchiveOutlet",
        ),
        type: AlertType.CONFIRM,
        onConfirm: () => {
          this.isArchivingOutlet = true;
          this.archiveItem = id;
          this.updateMediaMarketingOutletAsync(id, {
            archived: isArchived,
            updatedAt: moment().unix()
          }).then(() => {
            this.isArchivingOutlet = false;
          });
        },
      });
    },

    getOutlet(id: string) {
      return this.getMediaMarketingOutlet(id, false);
    },

    removeOutlet(id: string) {
      this.removeMediaMarketingOutlet(id);
      this.deleteMediaMarketingOutletAsync(id, false);
    },

    async savePromotionAsync(payload: Partial<FinancialPromotion>) {
      let promotion = new FinancialPromotion();

      if (!payload.id) {
        promotion.id = uuid();
        promotion.mediaOutlet = payload.mediaOutlet;
        promotion.contentUrl = payload.contentUrl;
        promotion.editorContent = {
          content: payload.editorContent?.content,
          rawContent: payload.editorContent?.rawContent,
          suggestions: payload.editorContent?.suggestions,
          commentThreads: payload.editorContent?.commentThreads,
        };
        promotion.owner = payload.owner;
        promotion.moderator = payload.moderator;
        promotion.approvalStatus = FinancialPromotionStatus.Pending;
        promotion.approvalDays = payload.approvalDays;
        promotion.isLive = payload.isLive;
        promotion.media = payload.media || [];
        promotion.type = payload.type;
        // FixMe. Check if product type should include category name. Use value instead of label
        promotion.productType = (payload.productType || []).map(
          ({ label }: any) => label,
        );
        promotion.remunerationMethod = payload.remunerationMethod;
        promotion.createdAt = moment().unix();
        promotion.updatedAt = undefined;
      } else {
        promotion = {
          ...promotion,
          ...payload,
        };
      }

      this.isSavingPromotion = true;
      try {
        await this.saveOrUpdateFinancialPromotionAsync(promotion);
        this.webScrapeService.webScrapeAndRegisterAsync(
          `${this.currentCustomer.email}`,
          promotion.id,
        );
      } catch (error) {
        // ToDo. Add error handler
      } finally {
        this.isSavingPromotion = false;
        this.isAddPromotionModalVisible = false;
        this.currentPromotionValues = undefined;
      }
    },

    async viewPromotionAsync(id: string) {
      this.setFinancialPromotion();
      this.isViewPromotionModalVisible = true;
      this.isFetchingPromotion = true;
      await this.fetchFinancialPromotionAsync(id);
      this.isFetchingPromotion = false;
    },

    removePromotion(id: string) {
      this.removeFinancialPromotion(id);
      this.deleteFinancialPromotionAsync(id, false);
    },

    getFinancialTypeLabel(value: FinancialPromotionType) {
      return (
        this.promotionTypeOptions.find((type) => type.value === value)?.label ||
        ""
      );
    },

    setUpdatingPromotionId(id: string) {
      this.updatingPromotionIds.push(id);
    },

    unsetUpdatingPromotionId(id: string) {
      this.updatingPromotionIds = this.updatingPromotionIds.filter(
        (item) => item !== id,
      );
    },

    approveRejectPost(id: string, isApprove: boolean) {
      useAlert({
        title: "Confirm",
        content: isApprove
          ? "Do you wish to approve this post?"
          : "Do you wish to reject this post?",
        confirmButtonText: isApprove ? "Confirm & Approve" : "Confirm & Reject",
        confirmButtonThemeColor: isApprove ? "primary" : "error",
        onConfirm: async () => {
          const promotion = this.getFinancialPromotion(id);

          if (!promotion) {
            useNotification({
              type: NotificationType.ERROR,
              content: "Something went wrong. Please try again later.",
            });
          }

          try {
            this.setUpdatingPromotionId(id);
            await this.saveOrUpdateFinancialPromotionAsync({
              ...promotion,
              approvalStatus: isApprove
                ? FinancialPromotionStatus.Approved
                : FinancialPromotionStatus.Rejected,
            });
            useNotification({
              type: NotificationType.SUCCESS,
              content: isApprove ? "Post Approved." : "Post Rejected.",
            });
            this.unsetUpdatingPromotionId(id);
          } catch (error) {
            // ToDo. Create error handler
          }
        },
      });
    },

    handleEdit(values: FinancialPromotion) {
      this.currentPromotionValues = values;
      this.isAddPromotionModalVisible = true;
      this.isViewPromotionModalVisible = false;
    },

    async handleSaveAsync(isAutoNext: boolean) {
      if (this.isSavingAlertOpened) {
        return;
      }

      if (isAutoNext) {
        useAlert({
          title: "Confirm",
          content:
            "Marketing and Financial Promotions is incomplete. Are you sure you wish to proceed?",
          confirmButtonText: "Proceed",
          saveDetailsText: "Complete Now",
          type: AlertType.SAVEDETAILS,
          width: 390,
          onSaveDetails: () => {
            this.isSavingAlertOpened = false;
          },
          onConfirm: async () => {
            await this.saveInfoAsync(true, isAutoNext);
          },
          onCancel: () => {
            this.isSavingAlertOpened = false;
          },
          onClose: () => {
            this.isSavingAlertOpened = false;
          },
        });
      } else {
        useAlert({
          title: "Confirm",
          content: "Please make sure the details are correct",
          confirmButtonText: "Confirm & Save",
          onConfirm: async () => {
            await this.saveInfoAsync(true);
          },
          onCancel: () => {
            this.isSavingAlertOpened = false;
          },
          onClose: () => {
            this.isSavingAlertOpened = false;
          },
        });
      }

      this.isSavingAlertOpened = true;
    },

    async saveInfoAsync(
      isShowLoader: boolean = false,
      isAutoNext: boolean = false,
    ): Promise<void> {
      const { email: customerEmail = "" } = this.currentCustomer;
      this.isLoading = isShowLoader;
      this.isShowSavingText = isShowLoader;
      this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi =
        true;
      this.isSavingAlertOpened = false;

      await this.updateCustomerByEmailAsync(customerEmail, {
        financialPromotions: this.financialPromotions,
      })
        .then(() => {
          if (isShowLoader) {
            useNotification({
              type: NotificationType.SUCCESS,
              content: this.$t("common-notification-saved"),
              interval: AppConstants.notificationPopupTimeOut,
            });
          }

          this.isLoading = false;
          this.isShowSavingText = false;
          // made sure notification popup is dismissed first before auto next
          if (isAutoNext) {
            setTimeout(
              () => this.eventBus.emit(AppConstants.autoNextEvent),
              AppConstants.notificationPopupTimeOut,
            );
          }
        })
        .catch();
    },

    async promotionTableFilterAsync(form: any) {
      const { values } = form;
      if (isEmpty(values)) {
        return;
      }
      const filters: Record<string, unknown> = {};

      if (!isEmpty(values.platform)) {
        filters.mediaOutlets = values.platform.map(({ value }: any) => value);
      }

      if (!isEmpty(values.type)) {
        filters.types = values.type.map(({ value }: any) => value);
      }

      if (!isEmpty(values.contentStatus)) {
        filters.contentStatus = values.contentStatus.map(
          ({ value }: any) => value,
        );
      }

      // @TODO Add more filters here...

      this.promotionTableSettings = {
        ...this.promotionTableSettings,
        filters,
        skip: 0,
        pageNumber: 1,
      };
      await this.fetchPromotionsDataAsync();
    },

    async promotionTablePageChangedAsync(event: GridPageChangeEvent) {
      const { skip, take } = event.page;
      const pageNumber = Math.ceil(skip / take + 1);
      this.promotionTableSettings = {
        ...this.promotionTableSettings,
        skip,
        pageNumber,
      };
      await this.fetchPromotionsDataAsync();
    },

    async promotionTableSearchAsync(keyword: string) {
      this.promotionTableSettings = {
        ...this.promotionTableSettings,
        filters: {
          ...this.promotionTableSettings.filters,
          search: keyword,
        },
        skip: 0,
        pageNumber: 1,
      };
      await this.fetchPromotionsDataAsync();
    },

    async promotionTableSortAsync(event: GridSortChangeEvent) {
      this.promotionTableSettings = {
        ...this.promotionTableSettings,
        sorts: event.sort,
      };
      await this.fetchPromotionsDataAsync();
    },

    marketingOutletSortAsync(event: GridSortChangeEvent) {
      this.marketingOutletTableLoading = true;
      setTimeout(() => {
        this.marketingOutletTableLoading = false;
        this.mediaMarketingTableSettings.sorts = event.sort;
      }, 300);
    },

    marketingOutletPageAsync(event: GridPageChangeEvent) {
      this.marketingOutletTableLoading = true;
      setTimeout(() => {
        this.marketingOutletTableLoading = false;
        this.mediaMarketingTableSettings.skip = event.page.skip;
        this.mediaMarketingTableSettings.skip = event.page.skip;
      }, 300);
    },
  },
});
</script>

<template src="./marketing-and-financial-promotions.html" />

<style scoped src="./marketing-and-financial-promotions.scss" />