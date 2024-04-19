<script lang="ts">
import {
  computed,
  defineComponent,
  inject,
  onBeforeMount,
  onMounted,
  ref,
} from "vue";
import { container } from "tsyringe";
import { Emitter, EventType } from "mitt";
import { useArCustomerStore } from "@/stores/useArCustomerStore";
import { usePageLifeCycleStore } from "@/stores/progress-bar/usePageLifeCycleStore";
import {
  IAppService,
  IAppServiceInfo,
} from "@/infra/dependency-services/app/IAppService";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import {
  AppointedRepresentativeProduct,
  AppointedRepresentativeProductType,
} from "@/entities/appointed-representatives/AppointedRepresentativeProduct";
import { AppointedRepresentativeActivity } from "@/entities/appointed-representatives/AppointedRepresentativeActivity";
import { AppointedRepresentative } from "@/entities/appointed-representatives/AppointedRepresentative";
import { Money } from "@/entities/Money";
import { AppConstants } from "@/infra/AppConstants";
import {
  IAppointedRepresentativeService,
  IAppointedRepresentativeServiceInfo,
} from "@/infra/dependency-services/rest/appointed-representative/IAppointedRepresentativeService";
import StaticList from "@/infra/StaticListService";
import { AlertType, useAlert } from "@/composables/useAlert";
import {
  NotificationType,
  useNotification,
} from "@/composables/useNotification";
import { NavPillItem } from "@/components/NavPillComponent.vue";

export default defineComponent({
  name: "ARActivities",
  data() {
    return {
      AppConstants,
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      isInitializing: true,
    };
  },
  setup() {
    const customerArService =
      container.resolve<IAppointedRepresentativeService>(
        IAppointedRepresentativeServiceInfo.name,
      );

    const eventBus = inject("$eventBusService") as Emitter<
      Record<EventType, unknown>
    >;

    const appService = container.resolve<IAppService>(IAppServiceInfo.name);

    const customerAr = ref(new AppointedRepresentative());

    const { currentArFirmName, currentArCustomer } = useArCustomerStore();
    const pageLifeCycleStore = usePageLifeCycleStore();
    const { changeLifeCycleName } = pageLifeCycleStore;
    const customerCompanyName = ref<string>("Richdale");
    const isLoading = ref<boolean>(false);
    const isShowSavingText = ref<boolean>(false);

    const newItems = ref<Record<string, Record<string, string>>>({});

    customerAr.value =
      appService.getCachedCustomerAppointedRepresentative() ??
      new AppointedRepresentative();

    const activities = ref<AppointedRepresentativeActivity[]>([]);
    const activitiesOriginal = ref<AppointedRepresentativeActivity[]>([]);

    const activityValues = computed({
      get() {
        return activities.value.reduce(
          (accumulator, value) => {
            return {
              ...accumulator,
              [value.productId]: value,
            };
          },
          {} as Record<string, AppointedRepresentativeActivity>,
        );
      },
      set(values: Record<string, AppointedRepresentativeActivity>) {
        activities.value = Object.values(values);
      },
    });

    const types = ref<AppointedRepresentativeProductType[]>(
      StaticList.getARProductTypes(),
    );

    const products = ref<AppointedRepresentativeProduct[]>(
      StaticList.getARProducts(),
    );

    const columns = computed(() => [
      {
        field: "product",
        title: "Product",
      },
      {
        field: "appointment",
        title: "Status",
      },
      {
        field: "requestStatus",
        title: "Add/Remove",
      },
      {
        field: "annualFeeIncome",
        title: "Projected Annual Fee Income",
      },
      {
        field: "annualCommissionIncome",
        title: "Projected Annual Commission Income",
      },
      {
        field: "hasLimitation",
        title: "Limitations",
      },
      {
        field: "action",
        title: " ",
      },
    ]);

    const arProductTypes = computed(() => StaticList.getARActivityProducts());

    const activitiesNavPill = computed(() =>
      arProductTypes.value.map(({ id, title, icon }) => ({
        id: `activity-nav-${id}`,
        anchorTo: `panel-${id}`,
        label: title,
        icon,
      })),
    );

    const activitiesPanels = computed(() =>
      arProductTypes.value.map(({ id, title }) => ({
        id,
        title,
        content: id,
        contentClass: "p-0",
        active: id === "mortgage-broking",
      })),
    );

    const getItems = (typeId: string) => {
      const output = products.value
        .filter((product) => product.typeId === typeId)
        .map((product) => ({
          product: product.id,
          annualFeeIncome: {
            amount: 0,
            currency: "GBP",
            symbol: "£",
          },
          annualCommissionIncome: {
            amount: 0,
            currency: "GBP",
            symbol: "£",
          },
          limitations: "",
          isAppointed: false,
          hasLimitation: false,
          hasPendingApplication: false,
          isModified: false,
          isNewProduct: false,
          isExpanded: false,
        }));

      output.push({
        product: "add-more",
        annualFeeIncome: {
          amount: 0,
          currency: "GBP",
          symbol: "£",
        },
        annualCommissionIncome: {
          amount: 0,
          currency: "GBP",
          symbol: "£",
        },
        limitations: "",
        isAppointed: false,
        hasLimitation: false,
        hasPendingApplication: false,
        isModified: false,
        isNewProduct: false,
        isExpanded: false,
      });

      return output;
    };

    const updateValue = (
      productId: string,
      payload: Partial<AppointedRepresentativeActivity>,
    ) => {
      const activity = activityValues.value[productId];
      const key: string = Object.keys(payload)[0].toString();
      let value: boolean | undefined;

      if (
        key === "hasLimitation" &&
        typeof payload.hasLimitation === "boolean"
      ) {
        value = payload.hasLimitation || false;
        payload = {
          hasLimitation: value,
          limitations: value ? activityValues.value[productId].limitations : "",
        };
      }
      if (key === "annualFeeIncome" || key === "annualCommissionIncome") {
        payload = {
          annualFeeIncome: payload.annualFeeIncome,
          isModified: true,
        };
      }

      if (activity) {
        activityValues.value = {
          ...activityValues.value,
          [productId]: {
            ...activity,
            ...payload,
          } as AppointedRepresentativeActivity,
        };
      } else {
        activityValues.value = {
          ...activityValues.value,
          [productId]: {
            productId,
            ...payload,
          } as AppointedRepresentativeActivity,
        };
      }
    };

    const getProduct = (id: string) =>
      products.value.find((product) => product.id === id);

    const addProduct = (payload: Partial<(typeof products.value)[number]>) => {
      products.value = [
        ...products.value,
        {
          ...payload,
        } as unknown as AppointedRepresentativeProduct,
      ];
    };

    const removeProduct = (typeId: string) => {
      products.value = products.value.filter(
        (product) => product.id !== typeId,
      );
    };

    const formatMoney = (value: number) => {
      return "£ " + value.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,");
    };

    const getTotal = (
      typeId: string,
      key: "annualFeeIncome" | "annualCommissionIncome",
    ) => {
      return Object.values(activityValues.value).reduce((accumulator, item) => {
        const product = getProduct(item.productId);
        if (!product || typeId !== product.typeId) return accumulator;

        return accumulator + (item[key]?.amount || 0);
      }, 0);
    };

    const activityCollapsiblePanel = ref();

    const openActivityPanel = (items: NavPillItem[]) => {
      const item: NavPillItem[] = items.filter((item) => item.active);
      if (item.length > 0) {
        const activePanel: string | undefined = item[0].anchorTo;

        if (activePanel !== undefined && activePanel !== "") {
          activityCollapsiblePanel.value.expand(
            activePanel.replace(/^panel-/, "").trim(),
          );
        }
      }
    };

    const attachNewItem = (typeId: string, values: Record<string, string>) => {
      newItems.value = {
        ...newItems.value,
        [typeId]: {
          ...(newItems.value[typeId] ?? {}),
          ...values,
        },
      };
    };

    const addNewItem = (typeId: string) => {
      const newItem = newItems.value[typeId];

      if (!newItem || !newItem.title || newItem.title === "") {
        return;
      }

      const productId = newItem.title.replace(/ +/g, "-").toLowerCase();

      activityValues.value = {
        ...activityValues.value,
        [productId]: {
          productId,
          annualFeeIncome: newItem.annualFeeIncome,
          annualCommissionIncome: newItem.annualCommissionIncome,
          isAppointed: newItem.isAppointed,
          hasLimitation: newItem.hasLimitation,
          limitations: newItem.limitations,
          hasPendingApplication: true,
          isModified: newItem.isModified,
          isNewProduct: true,
        } as unknown as AppointedRepresentativeActivity,
      };

      addProduct({
        id: productId,
        typeId,
        title: newItem.title,
      });

      delete newItems.value[typeId];

      eventBus.emit(AppConstants.formFieldChangedEvent);
    };

    const deleteItem = (productId: string) => {
      const product = activityValues.value;
      delete product[productId];
      activityValues.value = product;
      removeProduct(productId);
    };

    const saveInfoAsync = async (): Promise<void> => {
      isLoading.value = true;
      isShowSavingText.value = true;

      const updatedCustomerAr =
        await customerArService.getAppointedRepresentativesByEmailAsync(
          customerAr.value?.email ?? "",
        );

      updatedCustomerAr.activities =
        activities.value as AppointedRepresentativeActivity[];

      await customerArService
        .saveOrUpdateAppointedRepresentativeAsync(updatedCustomerAr)
        .then(() => {
          isLoading.value = false;
          isShowSavingText.value = false;
        })
        .catch(() => {
          useNotification({
            type: NotificationType.ERROR,
            content: "Something went wrong.",
            interval: 5e3,
          });
        })
        .finally(() => {
          isLoading.value = false;
          isShowSavingText.value = false;
        });
    };

    const initializedData = () => {
      activities.value = customerAr.value.activities ?? [];
      activitiesOriginal.value = customerAr.value.activities ?? [];
    };

    onBeforeMount(() => {
      changeLifeCycleName(AppConstants.pageLifeCycleNameCreated);
    });

    onMounted(() => {
      changeLifeCycleName(AppConstants.pageLifeCycleNameMounted);
      eventBus.emit(AppConstants.bottomBarEnableEvent, true);
      initializedData();
    });

    return {
      isLoading,
      isShowSavingText,
      currentArCustomer,
      currentArFirmName,
      customerCompanyName,
      activities,
      activitiesOriginal,
      activityValues,
      newItems,
      types,
      columns,
      products,
      activitiesNavPill,
      activitiesPanels,
      getItems,
      updateValue,
      getProduct,
      formatMoney,
      getTotal,
      openActivityPanel,
      attachNewItem,
      addNewItem,
      deleteItem,
      saveInfoAsync,
      activityCollapsiblePanel,
      Money,
    };
  },
  mounted() {
    this.isInitializing = false;
  },
  methods: {
    updateRequestsValue(
      productId: string,
      payload: Partial<AppointedRepresentativeActivity>,
      action: string,
    ) {
      const activity = this.activityValues[productId];
      const key: string = Object.keys(payload)[0].toString();
      const oldPayload = { [key]: false };
      let value: boolean | undefined;

      if (
        typeof payload.isModified === "boolean" ||
        typeof payload.hasPendingApplication === "boolean"
      ) {
        value = payload.isModified || payload.hasPendingApplication;
      } else {
        value = false;
      }

      switch (action) {
        case AppConstants.seekAuthAdd:
          useAlert({
            type: AlertType.ALERT,
            title: "Confirm",
            content: this.$t("common-alert-permissionConfirmAddText"),
            confirmButtonText: value
              ? this.$t("common-alert-confirmAndCancelRequest")
              : this.$t("common-alert-confirmAndAdd"),
            confirmButtonThemeColor: value ? "error" : "primary",
            onConfirm: async () => {
              useNotification({
                type: NotificationType.SUCCESS,
                content: value
                  ? this.$t("common-notification-CancelrequestSubmitted")
                  : this.$t("common-notification-requestSubmitted"),
                interval: 2e3,
              });

              if (activity) {
                this.activityValues = {
                  ...this.activityValues,
                  [productId]: {
                    ...activity,
                    ...payload,
                  } as AppointedRepresentativeActivity,
                };
              } else {
                this.activityValues = {
                  ...this.activityValues,
                  [productId]: {
                    productId,
                    ...payload,
                  } as AppointedRepresentativeActivity,
                };
              }
              await this.saveInfoAsync();
            },
            onCancel: () => {
              this.activityValues = {
                ...this.activityValues,
                [productId]: {
                  ...activity,
                  ...oldPayload,
                } as AppointedRepresentativeActivity,
              };
            },
            onClose: () => {
              const oldPayload = { [key]: false };
              this.activityValues = {
                ...this.activityValues,
                [productId]: {
                  ...activity,
                  ...oldPayload,
                } as AppointedRepresentativeActivity,
              };
            },
          });
          break;
        case AppConstants.seekAuthRemove:
          useAlert({
            type: AlertType.ALERT,
            title: "Confirm",
            content: this.$t("common-alert-permissionConfirmAddText"),
            confirmButtonText: value
              ? this.$t("common-alert-confirmAndCancelRequest")
              : this.$t("common-alert-confirmAndAdd"),
            confirmButtonThemeColor: value ? "error" : "primary",
            onConfirm: async () => {
              useNotification({
                type: NotificationType.SUCCESS,
                content: value
                  ? this.$t("common-notification-CancelrequestSubmitted")
                  : this.$t("common-notification-requestSubmitted"),
                interval: 2e3,
              });

              if (activity) {
                this.activityValues = {
                  ...this.activityValues,
                  [productId]: {
                    ...activity,
                    ...payload,
                  } as AppointedRepresentativeActivity,
                };
              } else {
                this.activityValues = {
                  ...this.activityValues,
                  [productId]: {
                    productId,
                    ...payload,
                  } as AppointedRepresentativeActivity,
                };
              }
              await this.saveInfoAsync();
            },
            onCancel: () => {
              this.activityValues = {
                ...this.activityValues,
                [productId]: {
                  ...activity,
                  ...oldPayload,
                } as AppointedRepresentativeActivity,
              };
            },
            onClose: () => {
              this.activityValues = {
                ...this.activityValues,
                [productId]: {
                  ...activity,
                  ...oldPayload,
                } as AppointedRepresentativeActivity,
              };
            },
          });
          break;
        default:
          throw new Error("We don't support this action in updatePermissions");
      }
    },

    setUniqueIdentifier(value: string): string {
      const identifier = `${AppConstants.arActivitiesRoute}${value}`;
      return this.helperService.removeStringSpacesThenSlash(identifier);
    }
  },
});
</script>

<template src="./ar-activities.html" />

<style scoped src="./ar-activities.scss" />
