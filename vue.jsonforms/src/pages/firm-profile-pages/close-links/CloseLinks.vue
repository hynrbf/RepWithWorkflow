<script lang="ts">
import { defineAsyncComponent, defineComponent, inject } from "vue";
import { container } from "tsyringe";
import { AppConstants } from "@/infra/AppConstants";
import { Emitter, EventType } from "mitt";
import {
  INavigationService,
  INavigationServiceInfo,
} from "@/infra/dependency-services/navigation/INavigationService";
import { useCustomerStore } from "@/stores/useCustomerStore";
import { mapState, mapActions } from "pinia";
import { FirmBasicInfo } from "@/entities/FirmBasicInfo";
import { AlertType, useAlert } from "@/composables/useAlert";
import {
  ISequenceNoKeeperService,
  ISequenceNoKeeperServiceInfo,
} from "@/infra/dependency-services/sequence-no/ISequenceNoKeeperService";
import { usePageLifeCycleStore } from "@/stores/progress-bar/usePageLifeCycleStore";
import BusinessIncorporation from "@/pages/models/owners-and-controllers/BusinessIncorporation";
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
import StaticList from "@/infra/StaticListService";
import { usePageFieldsInvalidHandlerStore } from "@/stores/usePageFieldsInvalidHandlerStore";
import { useAutoSaveStore } from "@/stores/useAutoSaveStore";
import { CloseLinkModel } from "@/pages/models/close-links/CloseLinkModel";
import {
  IMapperService,
  IMapperServiceInfo,
} from "@/infra/dependency-services/mapper/IMapperService";
import ScrollableTabComponent from "@/components/ScrollableTabComponent.vue";
import { ScrollableTabItemModel } from "@/components/models/ScrollableTabItemModel";
import {
  IFcaService,
  IFcaServiceInfo,
} from "@/infra/dependency-services/rest/fca/IFcaService";

export default defineComponent({
  name: "CloseLinks",
  components: {
    ScrollableTabComponent,
    KendoFirmFinderComponentAsync: defineAsyncComponent(
      () => import("@/components/form-fields/KendoFirmFinderComponent.vue"),
    ),
  },
  data() {
    return {
      hasOtherCloseLinks: undefined as boolean | undefined,
      natureOfCloseLink: [] as string[],
      isLoading: true,
      isShowSavingText: false,
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      navigationService: container.resolve<INavigationService>(
        INavigationServiceInfo.name,
      ),
      isInitializing: true,
      sequenceNoKeeperService: container.resolve<ISequenceNoKeeperService>(
        ISequenceNoKeeperServiceInfo.name,
      ),
      eventBusSideMenuRoute: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      eventBusFormSaved: inject("$eventBusService") as Emitter<
        Record<EventType, boolean>
      >,
      eventBusControlElement: inject("$eventBusService") as Emitter<
        Record<EventType, string>
      >,
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      mapperService: container.resolve<IMapperService>(IMapperServiceInfo.name),
      fcaService: container.resolve<IFcaService>(IFcaServiceInfo.name),
      userSubmittedChangesService:
        container.resolve<IUserSubmittedChangesService>(
          IUserSubmittedChangesServiceInfo.name,
        ),
      isSavingAlertOpened: false,
      collapsibleCloseLinkItems: [] as {
        closeLink: CloseLinkModel;
        isCollapsed: boolean;
      }[],
      debouncedAutoSaveFunction: () => {},
      isTradingAddressChangedAlertOpened: false,
      activeCloseLinkTabIndex: -1,
    };
  },
  setup() {
    const pageLifeCycleStore = usePageLifeCycleStore();
    const { changeLifeCycleName } = pageLifeCycleStore;

    const pageFieldsInvalidHandlerStore = usePageFieldsInvalidHandlerStore();
    const { processInvalidFields } = pageFieldsInvalidHandlerStore;

    const { debounceFunction, setAutoSaveFunctionNotCompletedYet } =
      useAutoSaveStore();

    return {
      changeLifeCycleName,
      processInvalidFields,
      debounceFunction,
      setAutoSaveFunctionNotCompletedYet,
    };
  },
  computed: {
    BusinessIncorporation() {
      return BusinessIncorporation;
    },
    AppConstants() {
      return AppConstants;
    },
    ...mapState(useCustomerStore, ["currentCustomer", "currentFirmName"]),
    closeLinkTabs: {
      get() {
        return this.collapsibleCloseLinkItems.map(({ closeLink }, index) => {
          return {
            id: `close-link-${closeLink.id}`,
            title: closeLink?.companyName ?? "New Close Link",
            content: "",
            active: this.activeCloseLinkTabIndex === index,
          } as ScrollableTabItemModel;
        });
      },
      set(tabs: ScrollableTabItemModel[]) {
        for (const index in tabs) {
          if (tabs[index].active) {
            this.activeCloseLinkTabIndex = +index;
            break;
          }
        }
      },
    },
  },
  created() {
    if (this.currentCustomer?.closeLinks) {
      const closeLinks = this.mapperService.mapCloseLinkEntitiesToModels(
        this.currentCustomer.closeLinks,
      );
      this.collapsibleCloseLinkItems = closeLinks.map((value) => {
        return {
          closeLink: value,
          isCollapsed: false,
        };
      });

      if (this.collapsibleCloseLinkItems.length) {
        this.hasOtherCloseLinks = true;
      }

      if (this.hasOtherCloseLinks) {
        this.activeCloseLinkTabIndex = 0;
      }
    }

    this.natureOfCloseLink = [
      `Parent Undertaking of ${this.currentFirmName}`,
      `Subsidiary Undertaking of ${this.currentFirmName}`,
      `Parent Undertaking of a Subsidiary Undertaking of ${this.currentFirmName}`,
      `Subsidiary Undertaking of a Parent Undertaking of ${this.currentFirmName}`,
      `Undertaking of which ${this.currentFirmName} owns or controls 20% or more of the capital or voting rights`,
    ];

    this.changeLifeCycleName(AppConstants.pageLifeCycleNameCreated);
    this.debouncedAutoSaveFunction = this.debounceFunction(this.saveInfoAsync);
  },
  mounted() {
    this.isLoading = false;
    this.eventBus.emit(AppConstants.bottomBarEnableEvent, true);
    this.eventBusFormSaved.on(AppConstants.formSavedEvent, (isAutoNext) => {
      this.submit(isAutoNext);
    });
    this.eventBus.on(AppConstants.formFieldPageLevelChangedEvent, () => {
      this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi =
        false;

      this.setAutoSaveFunctionNotCompletedYet();
      this.debouncedAutoSaveFunction();
    });
    this.isInitializing = false;
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameMounted);
    this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi = false;
  },
  unmounted() {
    this.eventBus.emit(AppConstants.bottomBarEnableEvent, false);
    this.eventBusFormSaved.off(AppConstants.formSavedEvent);
    this.eventBus.off(AppConstants.formFieldPageLevelChangedEvent);
    this.isLoading = false;
  },
  methods: {
    ...mapActions(useCustomerStore, ["updateCustomerByEmailAsync"]),

    submit(isAutoNext: boolean) {
      if (this.isSavingAlertOpened) {
        return;
      }

      if (isAutoNext) {
        useAlert({
          title: "Confirm",
          content:
            "Close Links is incomplete. Are you sure you wish to proceed?",
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
      (this.$refs?.formElement as HTMLFormElement)?.$el?.requestSubmit?.();
    },

    async saveInfoAsync(
      isShowLoader: boolean = false,
      isAutoNext: boolean = false,
    ): Promise<void> {
      if (!this.currentCustomer) {
        return;
      }

      const { email: customerEmail = "" } = this.currentCustomer;
      this.processInvalidFields(AppConstants.closeLinksRoute, this.eventBus);
      this.isLoading = isShowLoader;
      this.isShowSavingText = isShowLoader;
      this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi =
        true;
      this.isSavingAlertOpened = false;

      const closeLinks = this.collapsibleCloseLinkItems.map(
        (item) => item.closeLink,
      );

      await this.updateCustomerByEmailAsync(customerEmail, {
        closeLinks: closeLinks,
      }).then(() => {
        if (isShowLoader) {
          useNotification({
            type: NotificationType.SUCCESS,
            content: this.$t("common-notification-saved"),
            interval: AppConstants.notificationPopupTimeOut,
          });
        }

        this.isLoading = false;
        this.isShowSavingText = false;

        if (isAutoNext) {
          // made sure notification popup is dismissed first before auto next
          setTimeout(
            () => this.eventBus.emit(AppConstants.autoNextEvent),
            AppConstants.notificationPopupTimeOut,
          );
        }
      });
    },

    toggleHasOtherCloseLink(value: boolean) {
      if (value) {
        this.addCloseLink();
      } else {
        this.collapsibleCloseLinkItems = [];
      }
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    },

    onPercentageCapitalValueChange(value: number | null, item: CloseLinkModel) {
      item.percentageOfCapital = value;
      item.percentageOfVotingRights = value;
    },

    addCloseLink() {
      this.collapsibleCloseLinkItems.push({
        closeLink: new CloseLinkModel(),
        isCollapsed: false,
      });

      this.activeCloseLinkTabIndex = this.collapsibleCloseLinkItems.length - 1;
    },

    removeCloseLink(indexToRemove: number) {
      this.collapsibleCloseLinkItems.splice(indexToRemove, 1);
      if (
        indexToRemove <= this.activeCloseLinkTabIndex &&
        this.activeCloseLinkTabIndex > 0
      ) {
        this.activeCloseLinkTabIndex -= 1;
      }
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    },

    async onCloseLinkUpdated(updatedFirm: FirmBasicInfo, index: number) {
      if (!this.collapsibleCloseLinkItems[index]) {
        return;
      }

      this.collapsibleCloseLinkItems[index].closeLink.companyName =
        updatedFirm?.firmName ?? "";

      if (!updatedFirm.firmName) {
        this.collapsibleCloseLinkItems[index].closeLink.companyNumber = "";
        this.collapsibleCloseLinkItems[index].closeLink.firmReferenceNumber =
          "";
        this.collapsibleCloseLinkItems[index].closeLink.firm =
          new FirmBasicInfo();
      } else {
        this.collapsibleCloseLinkItems[index].closeLink.companyNumber =
          updatedFirm?.companyNumber ?? "";
        this.collapsibleCloseLinkItems[index].closeLink.firmReferenceNumber =
          updatedFirm?.firmReferenceNumber ?? "";
        this.collapsibleCloseLinkItems[index].closeLink.firm = updatedFirm;
        // other details
        this.collapsibleCloseLinkItems[index].closeLink.registeredAddress =
          updatedFirm.address;
        this.collapsibleCloseLinkItems[index].closeLink.tradingAddress =
          updatedFirm.tradingAddress;
        this.collapsibleCloseLinkItems[index].closeLink.website =
          updatedFirm.website;
        this.collapsibleCloseLinkItems[index].closeLink.natureOfBusiness =
          StaticList.getNatureOfBusinessBySicCode(updatedFirm.sicCode);

        await this.populateContactNumberAsync(
          index,
          updatedFirm.firmReferenceNumber,
        );

        this.collapsibleCloseLinkItems[index].closeLink.countryOfIncorporation =
          updatedFirm.countryCode;
      }
    },

    async populateContactNumberAsync(
      index: number,
      firmRefNo: string | undefined,
    ) {
      if (!firmRefNo) {
        return;
      }

      const addressDetails = await this.fcaService.getFirmAddressesDetailsAsync(
        firmRefNo,
        "PPOB",
      );

      if (addressDetails[0]["Phone Number"] && addressDetails[0]["country"]) {
        this.collapsibleCloseLinkItems[index].closeLink.contactNumber =
          await this.helperService.convertToContactNoAsync(
            addressDetails[0]["Phone Number"],
            addressDetails[0]["country"],
          );
      }
    },

    //the back and next will be affected, but will get back to this
    async goToOwnersAndControllersAsync() {
      const activeRouteServiceRouteSequenceNo =
        this.navigationService.getActiveRouteServiceRouteSequenceNo(
          AppConstants.ownersAndControllersRoute,
        );
      this.sequenceNoKeeperService.setSequenceNo(
        activeRouteServiceRouteSequenceNo,
      );
      this.eventBusSideMenuRoute.emit(AppConstants.userClickSideMenuRouteEvent);
      await this.navigationService.navigateRootAsync(
        AppConstants.ownersAndControllersRoute,
      );
    },

    onToggleTradingAddress(index: number): void {
      this.collapsibleCloseLinkItems[
        index
      ].closeLink.isTradingSameAsRegisteredAddress =
        !this.collapsibleCloseLinkItems[index].closeLink
          .isTradingSameAsRegisteredAddress;

      if (
        !this.collapsibleCloseLinkItems[index].closeLink
          .isTradingSameAsRegisteredAddress
      ) {
        this.collapsibleCloseLinkItems[index].closeLink.tradingAddress = "";
        return;
      }

      this.collapsibleCloseLinkItems[index].closeLink.tradingAddress =
        this.collapsibleCloseLinkItems[index].closeLink.registeredAddress;
    },

    onTradingAddressDoneTyping(
      elementId: string,
      hasChanged: boolean,
      closeLinkModel: CloseLinkModel,
    ) {
      if (!hasChanged || this.isTradingAddressChangedAlertOpened) {
        return;
      }

      this.isTradingAddressChangedAlertOpened = true;

      useAlert({
        title: this.$t("common-alert-title"),
        content: "Are you sure you wish to update your Trading Address?",
        additionalContent: `Current Trading Address with the FCA is ${closeLinkModel.firm?.tradingAddress}.`,
        confirmButtonText: "Confirm & Save",
        type: AlertType.ALERT,
        onConfirm: async () => {
          useNotification({
            type: NotificationType.SUCCESS,
            content: "Trading Address updated.",
            interval: AppConstants.notificationPopupTimeOut,
          });

          closeLinkModel.isTradingAddressChanged =
            closeLinkModel.tradingAddress !==
            closeLinkModel.firm.tradingAddress;
          this.isTradingAddressChangedAlertOpened = false;
          this.eventBusControlElement.emit(
            AppConstants.updateTradingAddressControlStateEvent,
            elementId,
          );
        },
        onClose: () => {
          closeLinkModel.tradingAddress =
            closeLinkModel.firm?.tradingAddress ?? "";

          if (!closeLinkModel.isTradingAddressChanged) {
            closeLinkModel.isTradingAddressChanged = false;
          }

          this.isTradingAddressChangedAlertOpened = false;
        },
      });
    },

    onTradingAddressChanged(value: string, closeLinkModel: CloseLinkModel) {
      closeLinkModel.tradingAddress = value;

      if (!closeLinkModel.tradingAddress) {
        closeLinkModel.isTradingAddressChanged = false;
      }
    },

    onBusinessIncorporationChange(
      businessIncorporation: { country: string; businessNature: string },
      item: CloseLinkModel,
    ) {
      item.countryOfIncorporation = businessIncorporation.country;
      item.natureOfBusiness = businessIncorporation.businessNature;
    },

    mapCountryAndNatureOfBusinessToBusinessIncorporation(
      item: CloseLinkModel,
    ): BusinessIncorporation {
      return {
        country: item.countryOfIncorporation ?? "",
        businessNature: item.natureOfBusiness ?? "",
      };
    },

    setUniqueIdentifier(value: string): string {
      const identifier = `${AppConstants.closeLinksRoute}${value}`;
      return identifier.replace(/\s+/g, "").replace("/", "");
    },
  },
});
</script>

<template src="./close-links.html" />

<style scoped src="./close-links.css" />
