<script lang="ts">
import { defineAsyncComponent, defineComponent, inject } from "vue";
import { AppConstants } from "@/infra/AppConstants";
import { container } from "tsyringe";
import {
  IFirmPagesRouteService,
  IFirmPagesRouteServiceInfo,
} from "@/infra/dependency-services/pages-route-firm/IFirmPagesRouteService";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import { Controller } from "@/entities/firm-details/Controller";
import { TabStripSelectEventArguments } from "@progress/kendo-vue-layout";
import { IndividualControllerModel } from "@/pages/models/owners-and-controllers/IndividualControllerModel";
import StaticList from "@/infra/StaticListService";
import { Address } from "@/entities/firm-details/Address";
import { IndividualController } from "@/entities/owners-and-controllers/IndividualController";
import { CorporateControllerModel } from "@/pages/models/owners-and-controllers/CorporateControllerModel";
import { CompanyOfficerAppointment } from "@/entities/owners-and-controllers/CompanyOfficerAppointment";
import { CompanyOfficerAppointmentDetails } from "@/entities/owners-and-controllers/CompanyOfficerAppointmentDetails";
import { Emitter, EventType } from "mitt";
import { KendoDialogComponent } from "@/components/KendoDialog.vue";
import { CorporateControllerCollapsibleItem } from "@/pages/models/owners-and-controllers/CorporateControllerCollapsibleItem";
import { CorporateController } from "@/entities/owners-and-controllers/CorporateController";
import { Director } from "@/entities/owners-and-controllers/Director";
import { IndividualControllerDetails } from "@/entities/owners-and-controllers/IndividualControllerDetails";
import { FinancialStatus } from "@/entities/owners-and-controllers/FinancialStatus";
import {
  IAppService,
  IAppServiceInfo,
} from "@/infra/dependency-services/app/IAppService";
import { AlertType, useAlert } from "@/composables/useAlert";
import {
  ISequenceNoKeeperService,
  ISequenceNoKeeperServiceInfo,
} from "@/infra/dependency-services/sequence-no/ISequenceNoKeeperService";
import {
  INavigationService,
  INavigationServiceInfo,
} from "@/infra/dependency-services/navigation/INavigationService";
import { usePageLifeCycleStore } from "@/stores/progress-bar/usePageLifeCycleStore";
import {
  NotificationType,
  useNotification,
} from "@/composables/useNotification";
import {
  IUserSubmittedChangesService,
  IUserSubmittedChangesServiceInfo,
} from "@/infra/dependency-services/user-submission/IUserSubmittedChangesService";
import { FirmBasicInfo } from "@/entities/FirmBasicInfo";
import { usePageFieldsInvalidHandlerStore } from "@/stores/usePageFieldsInvalidHandlerStore";
import { useAutoSaveStore } from "@/stores/useAutoSaveStore";
import { v4 as uuid } from "uuid";
import { ScrollableTabItemModel } from "@/components/models/ScrollableTabItemModel";
import { IndividualControllerCollapsibleItemModel } from "@/pages/models/owners-and-controllers/IndividualControllerCollapsibleItemModel";
import { AppointedRepresentative } from "@/entities/appointed-representatives/AppointedRepresentative";
import {
  IAppointedRepresentativeService,
  IAppointedRepresentativeServiceInfo,
} from "@/infra/dependency-services/rest/appointed-representative/IAppointedRepresentativeService";

export default defineComponent({
  name: "AROwnersAndControllers",
  components: {
    ARCorporateControllerItemComponent: defineAsyncComponent(
      () =>
        import(
          "@/pages/ar-pages/owners-and-controllers/partials/ARCorporateControllerItemComponent.vue"
        ),
    ),
    ARIndividualControllerItemComponent: defineAsyncComponent(
      () =>
        import(
          "@/pages/ar-pages/owners-and-controllers/partials/ARIndividualControllerItemComponent.vue"
        ),
    ),
    ARIndividualControllerChartModal: defineAsyncComponent(
      () => import("./partials/ARIndividualControllerChartModal.vue"),
    ),
    ARCorporateControllerChartModal: defineAsyncComponent(
      () => import("./partials/ARCorporateControllerChartModal.vue"),
    ),
  },
  data() {
    return {
      firmPagesRouteService: container.resolve<IFirmPagesRouteService>(
        IFirmPagesRouteServiceInfo.name,
      ),
      customerArService: container.resolve<IAppointedRepresentativeService>(
        IAppointedRepresentativeServiceInfo.name,
      ),
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      appService: container.resolve<IAppService>(IAppServiceInfo.name),
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      navigationService: container.resolve<INavigationService>(
        INavigationServiceInfo.name,
      ),
      kendoDialogInstance: null as KendoDialogComponent | null,
      registeredCustomer: new AppointedRepresentative(),
      firmName: "",
      tabs: [] as { title: string; content: string; titleRender: string }[],
      selectedControllerTabIndex: 0,
      titles: [] as string[],
      nationalities: [] as string[],
      isLoading: false,
      isInitializing: true,
      isShowSavingText: false,
      firmTypes: [] as string[],
      financialSolvencies: [] as string[],
      sequenceNoKeeperService: container.resolve<ISequenceNoKeeperService>(
        ISequenceNoKeeperServiceInfo.name,
      ),
      eventBusSideMenuRoute: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      eventBusFormSaved: inject("$eventBusService") as Emitter<
        Record<EventType, boolean>
      >,
      userSubmittedChangesService:
        container.resolve<IUserSubmittedChangesService>(
          IUserSubmittedChangesServiceInfo.name,
        ),
      isSavingAlertOpened: false,
      debouncedAutoSaveFunction: () => {},

      // Individual Controllers
      individualControllerItems:
        [] as IndividualControllerCollapsibleItemModel[],
      selectedIndividualControllerTabIndex: 0,
      individualControllersCount: 0,
      hasIndividualControllers: undefined as boolean | undefined,
      isShowIndividualControllerChartModal: false,

      // Corporate Controllers
      corporateControllerItems: [] as CorporateControllerCollapsibleItem[],
      selectedCorporateControllerTabIndex: 0,
      corporateControllersCount: 0,
      hasCorporateControllers: undefined as boolean | undefined,
      isShowCorporateControllerChartModal: false,
    };
  },
  computed: {
    AppConstants() {
      return AppConstants;
    },
    isNextButtonDisabled(): boolean {
      return (
        this.registeredCustomer.noOfIndividualShareholders < 1 &&
        this.registeredCustomer.noOfCorporateShareholders < 1
      );
    },
    individualControllerTabs: {
      get() {
        return this.individualControllerItems.map(
          ({ individualController }, index) => {
            return {
              id: `individual-controller-${individualController.id}`,
              title: individualController.getFullNameOrSetDefault(
                this.$t("individualControllerPage-newItemTitle"),
              ),
              content: "",
              active: this.selectedIndividualControllerTabIndex === index,
            };
          },
        );
      },
      set(tabs: ScrollableTabItemModel[]) {
        for (const index in tabs) {
          if (tabs[index].active) {
            this.selectedIndividualControllerTabIndex = +index;
            break;
          }
        }
      },
    },
    corporateControllerTabs: {
      get() {
        return this.corporateControllerItems.map(
          ({ corporateController, fullName }, index) => {
            let corporateControllerName = corporateController.companyName;

            return {
              id: `corporate-controller-${corporateController.id}`,
              title: fullName ?? corporateControllerName,
              content: "",
              active: this.selectedCorporateControllerTabIndex === index,
            };
          },
        );
      },
      set(tabs: ScrollableTabItemModel[]) {
        for (const index in tabs) {
          if (tabs[index].active) {
            this.selectedCorporateControllerTabIndex = +index;
            break;
          }
        }
      },
    },
  },
  watch: {
    registeredCustomer: {
      handler() {
        //ToDo. check if this make sense!
        //this.eventBus.emit(AppConstants.nextFormDisableEvent, this.isNextButtonDisabled);
      },
      immediate: true,
    },
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
  created() {
    this.registeredCustomer =
      this.appService.getCachedCustomerAppointedRepresentative() ??
      new AppointedRepresentative();

    if (!this.registeredCustomer.email) {
      throw new Error(
        "Customer Appointed Representative object must be already been cached before going to owners and controllers screen",
      );
    }

    this.firmName = this.appService.getCustomerArFirmName();

    this.individualControllersCount =
      this.registeredCustomer.noOfIndividualShareholders ?? 0;
    this.setupIndividualControllerItems();

    this.corporateControllersCount =
      this.registeredCustomer.noOfCorporateShareholders ?? 0;
    this.setupCorporateControllerItems();

    this.titles = StaticList.getTitles();
    this.nationalities = StaticList.getNationalities();
    this.firmTypes = StaticList.getFirmTypes();
    this.financialSolvencies = StaticList.getFinancialSolvency();
    this.tabs = [
      {
        title: `Individual Controllers (${this.individualControllersCount})`,
        content: "IndividualTemplate",
        titleRender: "",
      },
      {
        title: `Corporate Controllers (${this.corporateControllersCount})`,
        content: "CorporateTemplate",
        titleRender: "",
      },
      //the title shown as the iconcomponent, and content is blank
      {
        title: "",
        content: "",
        titleRender: "OrgChart",
      },
    ];
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameCreated);
    this.debouncedAutoSaveFunction = this.debounceFunction(this.saveInfoAsync);
  },
  async mounted() {
    if (this.$refs.kendoDialog) {
      this.kendoDialogInstance = this.$refs.kendoDialog as KendoDialogComponent;
    }

    this.helperService.resizeExpander();

    this.eventBus.emit(AppConstants.bottomBarEnableEvent, true);
    this.eventBusFormSaved.on(AppConstants.formSavedEvent, (isAutoNext) =>
      this.save(isAutoNext),
    );
    this.eventBus.on(AppConstants.formFieldPageLevelChangedEvent, () => {
      this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi =
        false;

      this.setAutoSaveFunctionNotCompletedYet();
      this.debouncedAutoSaveFunction();
    });
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameMounted);
    this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi = false;
    this.isInitializing = false;
  },
  unmounted() {
    this.eventBus.emit(AppConstants.bottomBarEnableEvent, false);
    this.eventBusFormSaved.off(AppConstants.formSavedEvent);
    this.eventBus.off(AppConstants.formFieldPageLevelChangedEvent);
    this.isLoading = false;
  },
  methods: {
    onTabItemSelect(e: TabStripSelectEventArguments) {
      if (this.selectedControllerTabIndex === 0 && e.selected > 1) {
        if (
          !(
            this.registeredCustomer.individualControllers &&
            this.registeredCustomer.individualControllers.length > 0
          )
        ) {
          return;
        }

        this.isShowIndividualControllerChartModal = true;
        return;
      }

      if (this.selectedControllerTabIndex === 1 && e.selected > 1) {
        if (
          !(
            this.registeredCustomer.corporateControllers &&
            this.registeredCustomer.corporateControllers.length > 0
          )
        ) {
          return;
        }

        this.isShowCorporateControllerChartModal = true;
        return;
      }

      this.selectedControllerTabIndex = e.selected;
    },

    // Individual Controller Methods
    setupIndividualControllerItems() {
      //when they are just coming from signup, get the number of individual and corporate controllers
      //from local else we just default both to 1
      let individualControllers: IndividualControllerModel[] = [];

      if (this.individualControllersCount < 1) {
        const individualControllersFromLocal = localStorage.getItem(
          AppConstants.companyIndividualControllersKey,
        );

        if (individualControllersFromLocal) {
          const individualControllerItemsFromLocal = JSON.parse(
            individualControllersFromLocal,
          ) as Controller[];

          for (let item of individualControllerItemsFromLocal) {
            let itemToAdd = new IndividualControllerModel();
            itemToAdd.detail.forename = item.name_elements?.forename ?? "";
            itemToAdd.detail.surname = item.name_elements?.surname ?? "";
            itemToAdd.detail.title = item.name_elements?.title ?? "";
            itemToAdd.detail.dateOfBirth = item.dateOfBirth;
            itemToAdd.detail.countryOfBirth = item.country_of_residence;

            if (item.nationality) {
              itemToAdd.detail.nationalities.push(item.nationality);
            }

            itemToAdd.detail.homeAddress = this.buildAddress(item.address);

            if (item.controllingInterests) {
              itemToAdd.controllingInterests = item.controllingInterests;
            } else {
              // no controlling interest? do not display
              continue;
            }

            itemToAdd.directorsAndDirectorship = item.directorships;
            individualControllers.push(itemToAdd);
          }

          this.registeredCustomer.noOfIndividualShareholders =
            individualControllers.length;
          this.individualControllersCount =
            this.registeredCustomer.noOfIndividualShareholders;
        }
      } else {
        if (
          this.registeredCustomer.individualControllers &&
          this.registeredCustomer.individualControllers.length > 0
        ) {
          individualControllers = this.mapIndividualControllerToModel(
            this.registeredCustomer.individualControllers,
          );
        }
      }

      this.generateIndividualControllersToCollapsibleItems(
        individualControllers,
      );
    },

    generateIndividualControllersToCollapsibleItems(
      individualControllers: IndividualControllerModel[],
    ) {
      const countToCreate =
        this.individualControllersCount < 1
          ? 0
          : this.individualControllersCount - individualControllers.length;

      if (countToCreate < 0) {
        const countToRemove = Math.abs(countToCreate);

        for (let i = 0; i < countToRemove; i++) {
          individualControllers.pop();
        }

        return;
      }

      // create additional items aside from existing
      for (let i = 0; i < countToCreate; i++) {
        let individualController = new IndividualControllerModel();
        individualController.directorsAndDirectorship = {
          nationalities: [],
          dateOfBirth: 0,
          fullName: "",
          items: [],
        };
        individualController.controllingInterests = [];
        individualControllers.push(individualController);
      }

      // populate collapsible items
      for (const individualController of individualControllers) {
        let newItem = {
          individualController: individualController,
          isCollapsed: false,
          fullName: "",
        };

        if (
          individualController.detail.forename &&
          individualController.detail.surname
        ) {
          newItem.fullName = `${individualController.detail.forename} ${individualController.detail.surname}`;
        }

        this.individualControllerItems.push(newItem);
      }

      if (this.individualControllerItems.length) {
        this.hasIndividualControllers = true;
      }
    },

    toggleHasIndividualControllers(value: boolean) {
      this.hasIndividualControllers = value;

      if (this.hasIndividualControllers) {
        if (!this.individualControllersCount) {
          this.addNewIndividualControllerItem();
        }
      } else {
        this.individualControllerItems = [];
        this.individualControllersCount = 0;
      }

      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    },

    removeIndividualControllerItem(indexToRemove: number) {
      useAlert({
        title: "Confirm",
        content: "Are you sure you wish to remove this Individual Controller?",
        confirmButtonText: "Confirm & Delete",
        confirmButtonThemeColor: "error",
        onConfirm: () => {
          this.individualControllerItems.splice(indexToRemove, 1);
          this.individualControllersCount =
            this.individualControllerItems.length;

          if (
            indexToRemove <= this.selectedIndividualControllerTabIndex &&
            this.selectedIndividualControllerTabIndex > 0
          ) {
            this.selectedIndividualControllerTabIndex -= 1;
          }

          if (this.individualControllersCount < 1) {
            this.hasIndividualControllers = false;
          }
        },
        onClose: () => {
          // Do nothing for now
        },
      });
    },

    addNewIndividualControllerItem() {
      const newIndividualController = new IndividualControllerModel();
      this.individualControllerItems.push({
        individualController: newIndividualController,
        isCollapsed: false,
        fullName: this.$t("individualControllerPage-newItemTitle"),
      });

      this.individualControllersCount = this.individualControllerItems.length;
      this.selectedIndividualControllerTabIndex =
        this.individualControllersCount - 1;
    },

    buildAddress(address: Address | undefined): string {
      if (!address) {
        return "";
      }

      let fullAddress = address.premises ?? "";

      if (address.address_line_1) {
        fullAddress += `, ${address.address_line_1}`;
      }

      if (address.address_line_2) {
        fullAddress += `, ${address.address_line_2}`;
      }

      if (address.locality) {
        fullAddress += `, ${address.locality}`;
      }

      if (address.country) {
        fullAddress += `, ${address.country}`;
      }

      if (address.postal_code) {
        fullAddress += `, ${address.postal_code}`;
      }

      return fullAddress;
    },

    mapIndividualControllerToModel(
      individualControllers: IndividualController[],
    ): IndividualControllerModel[] {
      const models: IndividualControllerModel[] = [];

      for (const individualController of individualControllers) {
        if (
          individualController.detail &&
          !individualController.detail.isHomeAddressChanged
        ) {
          individualController.detail.originalHomeAddress =
            individualController.detail.homeAddress;
        }

        const newItem = new IndividualControllerModel();
        newItem.detail = individualController.detail;
        newItem.controllingInterests =
          individualController.controllingInterests;
        newItem.directorsAndDirectorship =
          individualController.directorsAndDirectorship[0];
        newItem.financialStatus =
          individualController.financialStatus ?? new FinancialStatus();
        newItem.curriculumVitaeUrls = [];
        models.push(newItem);
      }

      return models;
    },

    onDateOfNameChange(
      value: Date,
      individualController: IndividualControllerModel,
    ) {
      individualController.detail.dateOfNameChange =
        this.helperService.dateStringToEpoch(value.toDateString());
    },

    onDateOfBirthChange(
      value: Date,
      individualController: IndividualControllerModel,
    ) {
      individualController.detail.dateOfBirth =
        this.helperService.dateStringToEpoch(value.toDateString());
    },

    onHomeAddressResidenceDateChange(
      value: Date,
      individualController: IndividualControllerModel,
    ) {
      individualController.detail.homeAddressResidenceDate =
        this.helperService.dateStringToEpoch(value.toDateString());
    },

    convertEpochValueToDate(input: number | undefined): Date | undefined {
      if (!input) {
        return new Date();
      }

      return this.helperService.convertEpochToDateTime(input);
    },

    showDetails(item: {
      individualController: IndividualControllerModel;
      isCollapsed: boolean;
    }) {
      item.isCollapsed = !item.isCollapsed;
    },

    isPassportNumberRequired(item: IndividualControllerModel): boolean {
      const notAvailableStr = "Not Available";
      const nAStr = "N/A";
      const nationalInsuranceNumber = item.detail?.nationalInsuranceNumber;

      return (
        nationalInsuranceNumber?.localeCompare(notAvailableStr, undefined, {
          sensitivity: "base",
        }) === 0 ||
        nationalInsuranceNumber?.localeCompare(nAStr, undefined, {
          sensitivity: "base",
        }) === 0
      );
    },

    mapIndividualControllerModelToEntity(
      models: IndividualControllerModel[],
    ): IndividualController[] {
      const entities: IndividualController[] = [];

      for (const individualController of models) {
        entities.push({
          id: individualController.id,
          detail: individualController.detail,
          controllingInterests: individualController.controllingInterests,
          directorsAndDirectorship: [
            individualController.directorsAndDirectorship,
          ],
          financialStatus: individualController.financialStatus,
          curriculumVitaeUrls: individualController.curriculumVitaeUrls,
        });
      }

      return entities;
    },

    mapCorporateControllerModelToEntity(
      models: CorporateControllerModel[],
    ): CorporateController[] {
      const entities: CorporateController[] = [];

      for (const model of models) {
        const firm = new FirmBasicInfo();
        firm.firmName = model.companyName;
        firm.companyNumber = model.companyNumber;
        firm.firmReferenceNumber = model.firmReferenceNumber;

        entities.push({
          companyName: model.companyName,
          companyNumber: model.companyNumber,
          firmReferenceNumber: model.firmReferenceNumber,
          firm: firm,
          countryOfIncorporation: model.countryOfIncorporation,
          natureOfBusiness: model.natureOfBusiness,
          firmType: model.firmType,
          financialSolvency: model.financialSolvency,
          registeredAddress: model.registeredAddress,
          isRegisteredAddressChanged: model.isRegisteredAddressChanged,
          isTradingAddressSameAsRegisteredAddress:
            model.isTradingAddressSameAsRegisteredAddress,
          tradingAddress: model.tradingAddress,
          isTradingAddressChanged: model.isTradingAddressChanged,
          isHeadOfficeAddressSameAsTradingAddress:
            model.isHeadOfficeAddressSameAsTradingAddress,
          headOfficeAddress: model.headOfficeAddress,
          emailAddress: model.emailAddress,
          contactNumber: model.contactNumber,
          percentageOfCapital: model.percentageOfCapital,
          percentageOfVotingRights: model.percentageOfVotingRights,
          individualOwners: model.individualOwners,
          corporateOwners: model.corporateOwners,
          isSubjectToRegulationByAnotherRegulator:
            model.isSubjectToRegulationByAnotherRegulator,
          isThirdCountryFirm: model.isThirdCountryFirm,
          thirdCountryFirmInfo: model.thirdCountryFirmInfo,
          isMemberOfFinancialConglomerate:
            model.isMemberOfFinancialConglomerate,
          memberOfFinancialConglomerateInfo:
            model.memberOfFinancialConglomerateInfo,
          isMemberOfThirdCountryFinancialConglomerate:
            model.isMemberOfThirdCountryFinancialConglomerate,
          memberOfThirdCountryFinancialConglomerateInfo:
            model.memberOfThirdCountryFinancialConglomerateInfo,
          isMemberOfThirdCountryBanking: model.isMemberOfThirdCountryBanking,
          memberOfThirdCountryBankingInfo:
            model.memberOfThirdCountryBankingInfo,
          hasBeenSubjectToAnyMaterialComplaints:
            model.hasBeenSubjectToAnyMaterialComplaints,
          beenSubjectToAnyMaterialComplaintsInfo:
            model.beenSubjectToAnyMaterialComplaintsInfo,
          directors: model.directors,
          individualControllers: this.mapIndividualControllerModelToEntity(
            model.individualControllers,
          ),
          corporateControllers: this.mapCorporateControllerModelToEntity(
            model.corporateControllers,
          ),
          supportingDocumentsUrls: model.supportingDocumentsUrls,
        });
      }

      return entities;
    },

    // Corporate Controller Methods
    toggleHasCorporateControllers(value: boolean) {
      this.hasCorporateControllers = value;

      if (this.hasCorporateControllers) {
        if (!this.corporateControllersCount) {
          this.addNewCorporateControllerItem();
        }
      } else {
        this.corporateControllerItems = [];
        this.corporateControllersCount = 0;
        this.selectedCorporateControllerTabIndex = -1;
      }

      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    },

    addNewCorporateControllerItem() {
      const corporateController = new CorporateControllerModel();
      corporateController.directors = [new Director()];
      corporateController.individualControllers =
        [] as IndividualControllerModel[];

      this.corporateControllerItems.push({
        corporateController: corporateController,
        isCollapsed: false,
        fullName: this.$t("corporateControllerPage-newItemTitle"),
        items: [],
      });

      this.corporateControllersCount = this.corporateControllerItems.length;
      this.selectedCorporateControllerTabIndex =
        this.corporateControllersCount - 1;
    },

    removeCorporateControllerItem(index: number) {
      useAlert({
        title: "Confirm",
        content: "Are you sure you wish to remove this Corporate Controller?",
        confirmButtonText: "Confirm & Delete",
        confirmButtonThemeColor: "error",
        onConfirm: () => {
          this.corporateControllerItems.splice(index, 1);
          this.corporateControllersCount = this.corporateControllerItems.length;

          if (
            index <= this.selectedCorporateControllerTabIndex &&
            this.selectedCorporateControllerTabIndex > 0
          ) {
            this.selectedCorporateControllerTabIndex -= 1;
          }

          if (this.corporateControllersCount < 1) {
            this.hasCorporateControllers = false;
          }
        },
        onClose: () => {
          // Do nothing for now
        },
      });
    },

    buildCorporateControllerCollapsibleItem(
      corporateController: CorporateControllerModel,
    ): CorporateControllerCollapsibleItem[] {
      let collapsibleItems = [] as CorporateControllerCollapsibleItem[];

      for (const item of corporateController.corporateControllers) {
        collapsibleItems.push({
          fullName: item.companyName ?? "",
          corporateController: item,
          isCollapsed: false,
          items: [] as CorporateControllerCollapsibleItem[],
        });
      }

      return collapsibleItems;
    },

    setupCorporateControllerItems() {
      let corporateControllers: CorporateControllerModel[] = [];

      if (this.corporateControllersCount < 1) {
        const corporateControllersFromLocal = localStorage.getItem(
          AppConstants.companyCorporateControllersKey,
        );

        if (corporateControllersFromLocal) {
          let corporateControllerItemsFromLocal = JSON.parse(
            corporateControllersFromLocal,
          ) as Controller[];

          let existingCorpController = [] as CorporateController[];

          for (let item of corporateControllerItemsFromLocal) {
            let newCorporateController =
              this.mapControllerToCorporateController(item);

            existingCorpController.push(newCorporateController);
          }

          corporateControllers = this.mapCorporateControllerToModel(
            existingCorpController,
          );

          this.registeredCustomer.noOfCorporateShareholders =
            corporateControllers.length;
          this.corporateControllersCount =
            this.registeredCustomer.noOfCorporateShareholders;
        }
      } else {
        if (
          this.registeredCustomer.corporateControllers &&
          this.registeredCustomer.corporateControllers.length > 0
        ) {
          corporateControllers = this.mapCorporateControllerToModel(
            this.registeredCustomer.corporateControllers,
          );
        }
      }

      for (let corporateController of corporateControllers) {
        if (!corporateController) {
          corporateController = new CorporateControllerModel();
        }

        corporateController.individualOwners =
          corporateController.individualControllers?.length ?? 0;

        for (const item of corporateController.individualControllers) {
          if (!corporateController.individualControllerItems) {
            corporateController.individualControllerItems = [] as {
              individualController: IndividualControllerModel;
              isCollapsed: boolean;
              fullName: string;
            }[];
          }

          if (!item.directorsAndDirectorship) {
            item.directorsAndDirectorship = {
              nationalities: [],
              dateOfBirth: 0,
              fullName: `${item.detail.forename} ${item.detail.surname}`,
              items: [new CompanyOfficerAppointment()],
            };
          }

          corporateController.individualControllerItems.push({
            individualController: item,
            isCollapsed: false,
            fullName: item?.directorsAndDirectorship.fullName ?? "",
          });
        }

        corporateController.corporateControllerItems =
          this.buildCorporateControllerCollapsibleItem(corporateController);
      }

      this.populateCorporateControllerItemsToDisplay(corporateControllers);
    },

    mapControllerToCorporateController(item: Controller): CorporateController {
      const firm = new FirmBasicInfo();
      firm.firmName = item.companyName;
      firm.companyNumber = item.companyNumber;
      firm.firmReferenceNumber = item.firmReferenceNumber;

      let newItem: CorporateController = {
        companyName: item.name,
        companyNumber: item.companyNumber,
        firmReferenceNumber: item.firmReferenceNumber,
        firm: firm,
        percentageOfCapital: item.percentageOfCapital,
        percentageOfVotingRights: item.percentageOfVotingRights,
        registeredAddress: item.fullAddress,
        isRegisteredAddressChanged: false,
        corporateOwners: item.corporateControllers?.length ?? 0,
        individualOwners: item.individualControllers?.length ?? 0,
        natureOfBusiness: item.natureOfBusiness,
        countryOfIncorporation: item.countryOfIncorporation,
        directors: item.directors,
        hasBeenSubjectToAnyMaterialComplaints: false,
        isHeadOfficeAddressSameAsTradingAddress: false,
        isMemberOfFinancialConglomerate: false,
        isMemberOfThirdCountryBanking: false,
        isMemberOfThirdCountryFinancialConglomerate: false,
        isSubjectToRegulationByAnotherRegulator: false,
        isThirdCountryFirm: false,
        isTradingAddressSameAsRegisteredAddress: false,
        financialSolvency: "",
        firmType: "",
        contactNumber: {
          number: "",
          dialCode: "+44",
          country: "United Kingdom",
          countryCode: "gb",
        },
        emailAddress: "",
        beenSubjectToAnyMaterialComplaintsInfo: "",
        headOfficeAddress: "",
        memberOfFinancialConglomerateInfo: "",
        memberOfThirdCountryBankingInfo: "",
        memberOfThirdCountryFinancialConglomerateInfo: "",
        thirdCountryFirmInfo: "",
        tradingAddress: "",
        isTradingAddressChanged: false,
        corporateControllers: [],
        individualControllers: [],
        supportingDocumentsUrls: [],
      };

      item.corporateControllers?.forEach((current, _i, _) => {
        newItem.corporateControllers.push(
          this.mapControllerToCorporateController(current),
        );
      });

      item.individualControllers?.forEach((current, _i, _) => {
        newItem.individualControllers.push(
          this.mapControllerToIndividualController(current),
        );
      });

      return newItem;
    },

    mapControllerToIndividualController(
      item: Controller,
    ): IndividualController {
      return {
        curriculumVitaeUrls: [],
        id: "",
        detail: {
          title: item.name_elements?.title ?? "",
          forename: item.name_elements?.forename ?? "",
          surname: item.name_elements?.surname ?? "",
          previousFullName: "",
          reasonForChangeName: "",
          dateOfNameChange: 0,
          commonlyUsedName: "",
          contactNumber: {
            number: "",
            dialCode: "+44",
            country: "United Kingdom",
            countryCode: "gb",
          },
          emailAddress: "",
          homeAddress: item.fullAddress,
          originalHomeAddress: item.fullAddress,
          isHomeAddressChanged: false,
          homeAddressResidenceDate: 0,
          previousAddresses: [],
          dateOfBirth: item.dateOfBirth,
          countryOfBirth: "",
          nationalities: item.nationality ?? [],
          previousNationalities: [],
          nationalInsuranceNumber: "",
          passportNumber: "",
          percentageOfCapital: item.percentageOfCapital,
          percentageOfVotingRights: item.percentageOfVotingRights,
          hasBeenSubjectToAnyMaterialComplaints: false,
          additionalInformation: "",
          supportingDocumentsUrls: [],
        } as IndividualControllerDetails,
        directorsAndDirectorship: [item.directorships],
        controllingInterests: item.controllingInterests,
        financialStatus: new FinancialStatus(),
      } as IndividualController;
    },

    mapCorporateControllerToModel(
      corporateControllers: CorporateController[],
    ): CorporateControllerModel[] {
      const models: CorporateControllerModel[] = [];

      if (!corporateControllers) {
        return models;
      }

      for (const corporateController of corporateControllers) {
        const firm = new FirmBasicInfo();
        firm.firmName = corporateController.companyName;
        firm.companyNumber = corporateController.companyNumber;
        firm.firmReferenceNumber = corporateController.firmReferenceNumber;
        firm.address = corporateController.registeredAddress;
        firm.tradingAddress = corporateController.tradingAddress;

        let newCorporateControllerModel: CorporateControllerModel = {
          id: uuid(),
          corporateControllerTabItems: [],
          companyName: corporateController.companyName,
          companyNumber: corporateController.companyNumber,
          firmReferenceNumber: corporateController.firmReferenceNumber,
          firm: firm,
          countryOfIncorporation: corporateController.countryOfIncorporation,
          natureOfBusiness: corporateController.natureOfBusiness,
          firmType: corporateController.firmType,
          financialSolvency: corporateController.financialSolvency,
          registeredAddress: corporateController.registeredAddress ?? "",
          isRegisteredAddressChanged:
            corporateController.isRegisteredAddressChanged,
          isTradingAddressSameAsRegisteredAddress:
            corporateController.isTradingAddressSameAsRegisteredAddress,
          tradingAddress: corporateController.tradingAddress,
          isTradingAddressChanged: corporateController.isTradingAddressChanged,
          isHeadOfficeAddressSameAsTradingAddress:
            corporateController.isHeadOfficeAddressSameAsTradingAddress,
          headOfficeAddress: corporateController.headOfficeAddress,
          emailAddress: corporateController.emailAddress,
          contactNumber: corporateController.contactNumber,
          percentageOfCapital: corporateController.percentageOfCapital,
          percentageOfVotingRights:
            corporateController.percentageOfVotingRights,
          individualOwners: corporateController.individualOwners,
          individualOwnersActual: corporateController.individualOwners,
          corporateOwners: corporateController.corporateOwners,
          corporateOwnersActual: corporateController.corporateOwners,
          isSubjectToRegulationByAnotherRegulator:
            corporateController.isSubjectToRegulationByAnotherRegulator,
          isThirdCountryFirm: corporateController.isThirdCountryFirm,
          thirdCountryFirmInfo: corporateController.thirdCountryFirmInfo,
          isMemberOfFinancialConglomerate:
            corporateController.isMemberOfFinancialConglomerate,
          memberOfFinancialConglomerateInfo:
            corporateController.memberOfFinancialConglomerateInfo,
          isMemberOfThirdCountryFinancialConglomerate:
            corporateController.isMemberOfThirdCountryFinancialConglomerate,
          memberOfThirdCountryFinancialConglomerateInfo:
            corporateController.memberOfThirdCountryFinancialConglomerateInfo,
          isMemberOfThirdCountryBanking:
            corporateController.isMemberOfThirdCountryBanking,
          memberOfThirdCountryBankingInfo:
            corporateController.memberOfThirdCountryBankingInfo,
          hasBeenSubjectToAnyMaterialComplaints:
            corporateController.hasBeenSubjectToAnyMaterialComplaints,
          beenSubjectToAnyMaterialComplaintsInfo:
            corporateController.beenSubjectToAnyMaterialComplaintsInfo,
          directors: corporateController.directors,
          individualControllers: this.mapIndividualControllerToModel(
            corporateController.individualControllers,
          ),
          corporateControllers: this.mapCorporateControllerToModel(
            corporateController.corporateControllers,
          ),
          corporateControllerItems: [] as CorporateControllerCollapsibleItem[],
          individualControllerItems: [] as {
            individualController: IndividualControllerModel;
            isCollapsed: boolean;
            fullName: string;
          }[],
          supportingDocumentsUrls: corporateController.supportingDocumentsUrls,
        };

        for (const item of newCorporateControllerModel.corporateControllers) {
          newCorporateControllerModel.corporateControllerItems =
            this.buildCorporateControllerCollapsibleItem(item);
        }

        models.push(newCorporateControllerModel);
      }

      return models;
    },

    populateCorporateControllerItemsToDisplay(
      corporateControllers: CorporateControllerModel[],
    ) {
      for (const corporateController of corporateControllers) {
        let fullName = corporateController.companyName ?? "<Company Name>";

        this.corporateControllerItems.push({
          corporateController: corporateController,
          isCollapsed: false,
          fullName:
            fullName.trim().length < 1
              ? this.$t("corporateControllerPage-newItemTitle")
              : fullName,
          items: [],
        });
      }

      if (this.corporateControllerItems.length) {
        this.hasCorporateControllers = true;
      }
    },

    save(isAutoNext: boolean) {
      if (this.isSavingAlertOpened) {
        return;
      }

      if (isAutoNext) {
        useAlert({
          title: "Confirm",
          content:
            "Owners & Controllers is incomplete. Are you sure you wish to proceed?",
          confirmButtonText: "Proceed",
          saveDetailsText: "Complete Now",
          type: AlertType.SAVEDETAILS,
          width: 390,
          onSaveDetails: () => {
            this.isSavingAlertOpened = false;
            (
              this.$refs?.formElement as HTMLFormElement
            )?.$el?.requestSubmit?.();
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
          title: this.$t("common-alert-title"),
          content: this.$t("common-alert-content"),
          confirmButtonText: this.$t("common-alert-buttonText"),
          onConfirm: async () => {
            await this.saveInfoAsync();
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
    ) {
      if (!this.registeredCustomer) {
        return;
      }

      this.processInvalidFields(
        AppConstants.ownersAndControllersRoute,
        this.eventBus,
      );
      this.isShowSavingText = isShowLoader;
      this.isLoading = isShowLoader;
      this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi =
        true;
      this.isSavingAlertOpened = false;
      const updatedCustomerAr =
        await this.customerArService.getAppointedRepresentativesByEmailAsync(
          this.registeredCustomer?.email ?? "",
        );

      // Individual Controllers
      const individualControllers = this.individualControllerItems.map(
        (i) => i.individualController,
      );

      for (const individualController of individualControllers) {
        if (!individualController.directorsAndDirectorship) {
          individualController.directorsAndDirectorship =
            new CompanyOfficerAppointmentDetails();
        }

        this.updateDirectorshipEndDateOriginalValue(
          individualController.directorsAndDirectorship,
        );

        if (!individualController.directorsAndDirectorship.fullName) {
          const forename = individualController.detail.forename;
          const surname = individualController.detail.surname;
          individualController.directorsAndDirectorship.fullName = `${forename} ${surname}`;
        }
      }

      updatedCustomerAr.individualControllers =
        this.mapIndividualControllerModelToEntity(individualControllers);

      // Corporate Controllers
      const corporateControllers = this.corporateControllerItems.map(
        (i) => i.corporateController,
      );

      corporateControllers?.forEach((current, _i, _) => {
        let childIndividualControllerItems =
          current.individualControllerItems.map((i) => i.individualController);
        childIndividualControllerItems?.forEach((currentChild, _i, _) => {
          if (!currentChild.directorsAndDirectorship) {
            currentChild.directorsAndDirectorship =
              new CompanyOfficerAppointmentDetails();
          }

          if (!currentChild.directorsAndDirectorship.fullName) {
            const forename = currentChild.detail.forename;
            const surname = currentChild.detail.surname;
            currentChild.directorsAndDirectorship.fullName = `${forename} ${surname}`;
          }
        });
      });

      updatedCustomerAr.corporateControllers =
        this.mapCorporateControllerModelToEntity(corporateControllers);

      await this.customerArService
        .saveOrUpdateAppointedRepresentativeAsync(updatedCustomerAr)
        .then((customerAr) => {
          if (isShowLoader) {
            useNotification({
              type: NotificationType.SUCCESS,
              content: this.$t("common-notification-saved"),
              interval: AppConstants.notificationPopupTimeOut,
            });
          }

          this.isLoading = false;
          this.isShowSavingText = false;
          this.individualControllersCount =
            customerAr.noOfIndividualShareholders;
          this.corporateControllersCount = customerAr.noOfCorporateShareholders;

          if (isAutoNext) {
            // made sure notification popup is dismissed first before auto next
            setTimeout(
              () => this.eventBus.emit(AppConstants.autoNextEvent),
              AppConstants.notificationPopupTimeOut,
            );
          }
        });
    },

    updateDirectorshipEndDateOriginalValue(
      directorships: CompanyOfficerAppointmentDetails,
    ) {
      if (!directorships?.items) {
        return;
      }

      for (const directorshipItem of directorships.items) {
        if (!directorshipItem.directorshipEndDate) {
          continue;
        }

        const endDate = this.helperService.convertEpochToDateTime(
          directorshipItem.directorshipEndDate,
        );

        if (!endDate) {
          continue;
        }

        const year: number = endDate.getUTCFullYear();
        const month: number = endDate.getUTCMonth() + 1; // Months are zero-based
        const day: number = endDate.getUTCDate();
        directorshipItem.originalResignedOn = `${year}-${month
          .toString()
          .padStart(2, "0")}-${day.toString().padStart(2, "0")}`;
      }
    },

    closeIndividualControllerChartModal() {
      this.isShowIndividualControllerChartModal = false;
    },

    closeCorporateControllerChartModal() {
      this.isShowCorporateControllerChartModal = false;
    },

    setUniqueIdentifier(value: string): string {
      const identifier = `${AppConstants.arOwnersAndControllersRoute}${value}`;
      return this.helperService.removeStringSpacesThenSlash(identifier);
    },
  },
});
</script>

<template src="./ar-owners-and-controllers.html" />

<style src="./ar-owners-and-controllers.css" />