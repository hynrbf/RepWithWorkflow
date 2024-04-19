<script lang="ts">
import { defineAsyncComponent, defineComponent, inject } from "vue";
import { CustomerEntity } from "@/entities/CustomerEntity";
import { AppConstants } from "@/infra/AppConstants";
import { FirmDetail } from "@/entities/firm-details/FirmDetail";
import { FirmRepresentative } from "@/entities/FirmRepresentative";
import { LocumDetails } from "@/entities/LocumDetails";
import { AccountDepartmentDetail } from "@/entities/AccountDepartmentDetail";
import { container } from "tsyringe";
import {
  ICustomerService,
  ICustomerServiceInfo,
} from "@/infra/dependency-services/rest/forms-compliance/ICustomerService";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import { FirmProfileStatus } from "@/entities/enums/FirmProfileStatus";
import {
  IFcaService,
  IFcaServiceInfo,
} from "@/infra/dependency-services/rest/fca/IFcaService";
import { Emitter, EventType } from "mitt";
import StaticList from "../../../infra/StaticListService";
import KendoDialog, {
  KendoDialogComponent,
} from "../../../components/KendoDialog.vue";
import {
  ICompaniesHouseService,
  ICompaniesHouseServiceInfo,
} from "@/infra/dependency-services/rest/company-house/ICompaniesHouseService";
import { usePageSectionWithAnchor } from "@/composables/usePageSectionWithAnchor";
import { KendoActionDialogComponent } from "@/components/KendoActionDialog.vue";
import { FirmBasicInfo } from "@/entities/FirmBasicInfo";
import KendoPromptDialog, {
  KendoPromptDialogComponent,
} from "@/components/KendoPromptDialog.vue";
import { ContactNumber } from "@/entities/ContactNumber";
import {
  IAppService,
  IAppServiceInfo,
} from "@/infra/dependency-services/app/IAppService";
import {
  ICalendlyService,
  ICalendlyServiceInfo,
} from "@/infra/dependency-services/rest/calendly/ICalendlyService";
import { SignUpEventTypeModel } from "@/pages/sign-up-pages/signup/models/SignUpEventTypeModel";
import { KendoFlexibleDialogComponent } from "@/components/KendoFlexibleDialog.vue";
import { CompanySearchMode } from "@/entities/enums/CompanySearchMode";
import { AlertType, useAlert } from "@/composables/useAlert";
import { usePageLifeCycleStore } from "@/stores/progress-bar/usePageLifeCycleStore";
import {
  NotificationType,
  useNotification,
} from "@/composables/useNotification";
import {
  IUserSubmittedChangesService,
  IUserSubmittedChangesServiceInfo,
} from "@/infra/dependency-services/user-submission/IUserSubmittedChangesService";
import KendoConfirmDialog, {
  KendoConfirmDialogComponent,
} from "@/components/KendoConfirmDialog.vue";
import { usePageFieldsInvalidHandlerStore } from "@/stores/usePageFieldsInvalidHandlerStore";
import { useAutoSaveStore } from "@/stores/useAutoSaveStore";
import { LocumSoleTraderDetails } from "@/entities/firm-details/LocumSoleTraderDetails";
import { LocumCompanyDetails } from "@/entities/firm-details/LocumCompanyDetails";

export default defineComponent({
  name: "FirmDetails",
  components: {
    KendoConfirmDialog,
    KendoPromptDialog,
    KendoDialog,
    KendoFirmFinderComponentAsync: defineAsyncComponent(
      () => import("@/components/form-fields/KendoFirmFinderComponent.vue"),
    ),
  },
  data() {
    return {
      customerService: container.resolve<ICustomerService>(
        ICustomerServiceInfo.name,
      ),
      companyHouseService: container.resolve<ICompaniesHouseService>(
        ICompaniesHouseServiceInfo.name,
      ),
      fcaService: container.resolve<IFcaService>(IFcaServiceInfo.name),
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      appService: container.resolve<IAppService>(IAppServiceInfo.name),
      calendlyService: container.resolve<ICalendlyService>(
        ICalendlyServiceInfo.name,
      ),
      userSubmittedChangesService:
        container.resolve<IUserSubmittedChangesService>(
          IUserSubmittedChangesServiceInfo.name,
        ),
      kendoDialogInstance: null as KendoDialogComponent | null,
      kendoActionDialogInstance: null as KendoActionDialogComponent | null,
      kendoActionDialogForLocumInstance:
        null as KendoActionDialogComponent | null,
      kendoPromptInstance: null as KendoPromptDialogComponent | null,
      kendoPromptError: null as KendoPromptDialogComponent | null,
      kendoPromptDifferentPermissions:
        null as KendoPromptDialogComponent | null,
      kendoPromptFirmNameNotAuthorised:
        null as KendoPromptDialogComponent | null,
      kendoConfirmDialogInstance: null as KendoConfirmDialogComponent | null,
      kendoCalendarFlexibleDialogInstance:
        null as KendoFlexibleDialogComponent | null,
      isLoading: false,
      isShowSavingText: false,
      isInitializing: true,
      registeredCustomer: null as CustomerEntity | null,
      companyNumber: "",
      complianceFirmName: "",
      firmName: "",
      isFirmDependentOnSingleKeyIndividual: undefined as boolean | undefined,
      hasAppointedLocum: undefined as boolean | undefined,
      isLocumCompany: undefined as boolean | undefined,
      hasNoAppointedLocumInfo: "",
      isHeadOfficeAddressSameAsTradingAddress: false,
      isAccountsDepartmentSameAsFirmRepresentative: false,
      tempTradingName: "",
      firmTypes: [] as string[],
      titles: [] as string[],
      roles: [] as string[],
      isSoleTrader: false,
      firmDetails: new FirmDetail(),
      firmRepresentativeDetail: new FirmRepresentative(),
      locumDetail: new LocumDetails(),
      locumCompanyDetail: undefined as LocumCompanyDetails | undefined,
      locumSoleTraderDetail: undefined as LocumSoleTraderDetails | undefined,
      accountDepartmentDetail: new AccountDepartmentDetail(),
      isProfileCompleted: false,
      pageDescription: "",
      isShowLocumTradingAddress: true,
      firmDetailsKey: "firmDetails",
      locumDetailKey: "locumDetail",
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      eventBusFormSaved: inject("$eventBusService") as Emitter<
        Record<EventType, boolean>
      >,
      eventBusCompanyOrFrnNo: inject("$eventBusService") as Emitter<
        Record<EventType, { payload: string }>
      >,
      eventBusError: inject("$eventBusService") as Emitter<
        Record<EventType, { payload: string }>
      >,
      eventBusDifferentPermissions: inject("$eventBusService") as Emitter<
        Record<EventType, { payload: string }>
      >,
      eventBusFirmNameNotAuthorised: inject("$eventBusService") as Emitter<
        Record<EventType, { payload: string }>
      >,
      eventBusCompanyDetailChanged: inject("$eventBusService") as Emitter<
        Record<EventType, { payload: string | undefined }>
      >,
      eventBusControlElement: inject("$eventBusService") as Emitter<
        Record<EventType, string>
      >,
      // TODO.remove popup when company not found. to confirm if still needed.
      // eventBusNotFoundCompany: inject("$eventBusService") as Emitter<
      //   Record<
      //     EventType,
      //     {
      //       msg: string;
      //       company: string;
      //     }
      //   >
      // >,
      aboutTheFirm: false,
      firmRepresentativeDetailsSection: false,
      locumSection: false,
      accountDepartmentDetailsSection: false,
      isShowLocumRepresentativeDetails: false,
      companyToProceedIfNotFound: "",
      // companyNoToProceed: "", Used in old implementation. Keeping this for now
      inputFirm: new FirmBasicInfo(),
      locumInputFirm: new FirmBasicInfo(),
      activeEventTypes: [] as SignUpEventTypeModel[],
      selectedEventType: null as SignUpEventTypeModel | null,
      isSavingAlertOpened: false,
      isTradingAddressChangedAlertOpened: false,
      isTradingNamesAlreadyChangedFromDB: false,
      debouncedSaveInput: () => {},
    };
  },
  setup() {
    const pageSections = [
      {
        id: "about-firm-section",
        top: 0,
      },
      {
        id: "firm-representative-section",
        top: 0,
      },
      {
        id: "locum-section",
        top: 0,
      },
      {
        id: "account-department-details-section",
        top: 0,
      },
    ];
    const {
      activePageSectionId,
      sectionElementCurrentPosition,
      scrollToPageSection,
      updatePageSectionTop,
      stickyHeaderHeight,
    } = usePageSectionWithAnchor(pageSections, ".page-tab-anchors");

    const pageLifeCycleStore = usePageLifeCycleStore();
    const { changeLifeCycleName } = pageLifeCycleStore;

    const pageFieldsInvalidHandlerStore = usePageFieldsInvalidHandlerStore();
    const { processInvalidFields } = pageFieldsInvalidHandlerStore;

    const { debounceFunction, setAutoSaveFunctionNotCompletedYet } =
      useAutoSaveStore();

    return {
      activePageSectionId,
      sectionElementCurrentPosition,
      scrollToPageSection,
      updatePageSectionTop,
      stickyHeaderHeight,
      changeLifeCycleName,
      processInvalidFields,
      debounceFunction,
      setAutoSaveFunctionNotCompletedYet,
    };
  },
  computed: {
    AppConstants() {
      return AppConstants;
    },
    CompanySearchMode() {
      return CompanySearchMode;
    },
    getAccountsDepartmentDetailTitle(): string {
      return "Accounts Department Details";
    },
  },
  async mounted() {
    this.changeModalBackdropColor();

    if (this.$refs.kendoDialog) {
      this.kendoDialogInstance = this.$refs.kendoDialog as KendoDialogComponent;
    }

    if (this.$refs.kendoActionDialog) {
      this.kendoActionDialogInstance = this.$refs
        .kendoActionDialog as KendoActionDialogComponent;
    }

    if (this.$refs.kendoActionDialogForLocum) {
      this.kendoActionDialogForLocumInstance = this.$refs
        .kendoActionDialogForLocum as KendoActionDialogComponent;
    }

    if (this.$refs.kendoActionDialogCompanyNumber) {
      this.kendoPromptInstance = this.$refs
        .kendoActionDialogCompanyNumber as KendoPromptDialogComponent;
    }

    if (this.$refs.kendoActionDialogError) {
      this.kendoPromptError = this.$refs
        .kendoActionDialogError as KendoPromptDialogComponent;
    }

    if (this.$refs.kendoActionDialogDifferentPermissions) {
      this.kendoPromptDifferentPermissions = this.$refs
        .kendoActionDialogDifferentPermissions as KendoPromptDialogComponent;
    }

    if (this.$refs.kendoActionDialogNotAuthorised) {
      this.kendoPromptFirmNameNotAuthorised = this.$refs
        .kendoActionDialogNotAuthorised as KendoPromptDialogComponent;
    }

    if (this.$refs.kendoConfirmDialog) {
      this.kendoConfirmDialogInstance = this.$refs
        .kendoConfirmDialog as KendoConfirmDialogComponent;
    }

    if (this.$refs.kendoCalendarFlexibleDialog) {
      this.kendoCalendarFlexibleDialogInstance = this.$refs
        .kendoCalendarFlexibleDialog as KendoFlexibleDialogComponent;
    }

    this.eventBus.emit(AppConstants.bottomBarEnableEvent, true);
    this.eventBusFormSaved.on(AppConstants.formSavedEvent, (isAutoNext) => {
      this.handleSubmit(isAutoNext);
    });

    this.eventBusCompanyOrFrnNo.on(
      AppConstants.requestCompanyDetailsUpdateEvent,
      (_data: { payload: string }) => {
        this.kendoPromptInstance?.setActionDialogMessage("Confirm");
      },
    );

    this.eventBusError.on(
      AppConstants.firmSelectionErrorEvent,
      (_data: { payload: string }) => {
        this.kendoPromptError?.setActionDialogMessage("Error");
      },
    );

    this.eventBusDifferentPermissions.on(
      AppConstants.companyNotSamePermissionEvent,
      (_data: { payload: string }) => {
        this.kendoPromptDifferentPermissions?.setActionDialogMessage("Confirm");
      },
    );

    this.eventBusFirmNameNotAuthorised.on(
      AppConstants.locumFirmNameNotAuthorisedEvent,
      (_data: { payload: string }) => {
        this.kendoPromptFirmNameNotAuthorised?.setActionDialogMessage(
          "Confirm",
        );
      },
    );

    this.eventBus.on(AppConstants.formFieldPageLevelChangedEvent, () => {
      this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi =
        false;

      this.setAutoSaveFunctionNotCompletedYet();
      this.debouncedSaveInput();
    });

    //put here the fire and forget pre caching to save the slow
    //firm profile operations
    this.registeredCustomer =
      this.appService.getCachedCustomer() ?? new CustomerEntity();
    const promise1 = this.getCompanyDetailsAsync();
    const promise2 = this.appService.getComplianceFirmNameAsync();
    const promise3 = this.fcaService.getFcaDefinedStatusesAsync();
    const promise4 =
      this.companyHouseService.getCompanyHouseDefinedStatusesAsync();
    const promise5 = this.customerService.getCustomerProducts(
      this.registeredCustomer.email,
    );

    const [
      // eslint-disable-next-line @typescript-eslint/no-unused-vars
      _,
      result2,
      fcaDefinedStatuses,
      companyHouseDefinedStatuses,
      customerProducts,
    ] = await Promise.all([promise1, promise2, promise3, promise4, promise5]);

    if (result2) {
      this.complianceFirmName = result2;
    }

    if (fcaDefinedStatuses && fcaDefinedStatuses.length > 0) {
      localStorage.setItem(
        AppConstants.fcaDefinedStatusesKey,
        JSON.stringify(fcaDefinedStatuses),
      );
    }

    if (companyHouseDefinedStatuses && companyHouseDefinedStatuses.length > 0) {
      localStorage.setItem(
        AppConstants.companyHouseDefinedStatusesKey,
        JSON.stringify(companyHouseDefinedStatuses),
      );
    }

    if (customerProducts && customerProducts.length > 0) {
      localStorage.setItem(
        AppConstants.customerProductsKey,
        JSON.stringify(customerProducts),
      );
    }

    await this.getDirectorshipsControllingInterestsAndControllersThenCacheAsync();

    this.pageDescription = `The information provided by you within Firm Profile, will form the basis of the services ${this.complianceFirmName} will provide you with. This will include but is not limited to the drafting of the Policies and Procedures and Compliance Monitoring Program of ${this.firmName}.`;
    this.isProfileCompleted =
      this.registeredCustomer?.firmProfileEditStatus ===
      FirmProfileStatus.Complete.toString();
    this.isInitializing = false;
    this.helperService.resizeExpander();
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameMounted);
  },
  unmounted() {
    this.eventBus.emit(AppConstants.bottomBarEnableEvent, false);
    this.eventBusFormSaved.off(AppConstants.formSavedEvent);
    this.eventBus.off(AppConstants.formFieldPageLevelChangedEvent);
    // TODO.remove popup when company not found. to confirm if still needed.
    //this.eventBusNotFoundCompany.off(AppConstants.companyNotFoundEvent);
    //this.eventBusNotFoundCompany.off(AppConstants.firmNotFoundEvent);
    this.eventBusCompanyOrFrnNo.off(AppConstants.companyNumberChangeEvent);
    this.eventBusError.off(AppConstants.firmSelectionErrorEvent);
    this.eventBusDifferentPermissions.off(
      AppConstants.companyNotSamePermissionEvent,
    );
    this.eventBusFirmNameNotAuthorised.off(
      AppConstants.locumFirmNameNotAuthorisedEvent,
    );
    this.isLoading = false;
  },
  async created() {
    this.firmTypes = StaticList.getFirmTypes();
    this.titles = StaticList.getTitles();
    this.roles = StaticList.getRoles();
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameCreated);
    this.debouncedSaveInput = this.debounceFunction(this.saveInfoAsync);
    this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi = false;
  },
  methods: {
    handleSubmit(isAutoNext: boolean) {
      if (this.isSavingAlertOpened) {
        return;
      }

      if (isAutoNext) {
        useAlert({
          title: "Confirm",
          content:
            "Firm Details is incomplete. Are you sure you wish to proceed?",
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
    ) {
      if (!this.registeredCustomer) {
        return;
      }

      this.isShowSavingText = isShowLoader;
      this.isLoading = isShowLoader;
      this.processInvalidFields(AppConstants.firmDetailsRoute, this.eventBus);
      const updatedCustomer =
        await this.customerService.getCustomerByEmailAsync(
          this.registeredCustomer.email ?? "",
        );

      if (this.isValidationErrorPopupShown()) {
        return;
      }

      this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi =
        true;
      this.isSavingAlertOpened = false;

      if (this.firmDetails.tradingNames.length < 1 && this.tempTradingName) {
        // You can also save a single trading name without clicking the 'Add trading name' button
        this.firmDetails.tradingNames.push(this.tempTradingName);
      }

      this.firmDetails.isHeadOfficeSameAsTradingAddress =
        this.isHeadOfficeAddressSameAsTradingAddress;

      if (updatedCustomer.firmDetail) {
        if (!updatedCustomer.firmDetail.isRegisteredAddressChanged) {
          this.firmDetails.isRegisteredAddressChanged =
            updatedCustomer.firmDetail?.registeredAddress !==
            this.firmDetails.registeredAddress;
        }

        if (!updatedCustomer.firmDetail.isTradingAddressChanged) {
          this.firmDetails.isTradingAddressChanged =
            updatedCustomer.firmDetail?.tradingAddress !==
            this.firmDetails.tradingAddress;
        }

        if (!updatedCustomer.firmDetail.isHeadOfficeAddressChanged) {
          this.firmDetails.isHeadOfficeAddressChanged =
            updatedCustomer.firmDetail?.headOfficeAddress !==
            this.firmDetails.headOfficeAddress;
        }
      }

      updatedCustomer.firmDetail = this.firmDetails;
      updatedCustomer.firmRepresentativeDetail = this.firmRepresentativeDetail;

      this.locumDetail.isDependentOnSingleKeyIndividual =
        this.isFirmDependentOnSingleKeyIndividual;
      this.locumDetail.isLocumCompany = this.isLocumCompany;
      this.locumDetail.hasAppointedALocum = this.hasAppointedLocum;
      this.locumDetail.hasNoAppointedLocumInfo = this.hasNoAppointedLocumInfo;

      this.locumDetail.locumCompanyDetail = this.locumCompanyDetail;
      this.locumDetail.locumSoleTraderDetail = this.locumSoleTraderDetail;

      if (updatedCustomer.locumDetail) {
        if (
          this.locumDetail.isLocumCompany &&
          this.locumDetail.locumCompanyDetail &&
          updatedCustomer.locumDetail.locumCompanyDetail
        ) {
          if (
            !updatedCustomer.locumDetail.locumCompanyDetail
              .isRegisteredAddressChanged
          ) {
            this.locumDetail.locumCompanyDetail.isRegisteredAddressChanged =
              updatedCustomer.locumDetail?.locumCompanyDetail
                ?.registeredAddress !==
              this.locumDetail.locumCompanyDetail?.registeredAddress;
          }

          if (
            !updatedCustomer.locumDetail.locumCompanyDetail
              .isTradingAddressChanged
          ) {
            this.locumDetail.locumCompanyDetail.isTradingAddressChanged =
              updatedCustomer.locumDetail?.locumCompanyDetail.tradingAddress !==
              this.locumDetail.locumCompanyDetail.tradingAddress;
          }
        }

        if (
          this.locumDetail.isLocumCompany === false &&
          this.locumDetail.locumSoleTraderDetail &&
          updatedCustomer.locumDetail.locumSoleTraderDetail
        ) {
          if (
            !updatedCustomer.locumDetail.locumSoleTraderDetail
              .isTradingAddressChanged
          ) {
            this.locumDetail.locumSoleTraderDetail.isTradingAddressChanged =
              updatedCustomer.locumDetail?.locumSoleTraderDetail
                .tradingAddress !==
              this.locumDetail.locumSoleTraderDetail.tradingAddress;
          }
        }
      }

      updatedCustomer.locumDetail = this.locumDetail;
      updatedCustomer.accountDepartmentDetail = this.accountDepartmentDetail;

      await this.customerService
        .saveCustomerAsync(JSON.stringify(updatedCustomer))
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

          if (isAutoNext) {
            // made sure notification popup is dismissed first before auto next
            setTimeout(
              () => this.eventBus.emit(AppConstants.autoNextEvent),
              AppConstants.notificationPopupTimeOut,
            );
          }
        });
    },

    toggleLocumYesOrNo(selected: boolean) {
      this.isFirmDependentOnSingleKeyIndividual = selected;

      if (!selected) {
        this.hasAppointedLocum = false;
        this.hasNoAppointedLocumInfo = "";
      }

      this.isShowLocumTradingAddress = !this.isSoleTrader && selected;
    },

    toggleHasAppointedLocumYesOrNo(selected: boolean) {
      this.hasAppointedLocum = selected;

      if (!this.hasAppointedLocum) {
        this.isLocumCompany = undefined;
        this.locumCompanyDetail = undefined;
        this.locumSoleTraderDetail = undefined;
      }
    },

    toggleIsLocumCompanyOrSoleTrader(selected: boolean) {
      this.isLocumCompany = selected;
      this.locumInputFirm = new FirmBasicInfo();

      if (this.isLocumCompany) {
        this.locumCompanyDetail = new LocumCompanyDetails();
        this.locumSoleTraderDetail = undefined;
      } else {
        this.locumSoleTraderDetail = new LocumSoleTraderDetails();
        this.locumCompanyDetail = undefined;
      }
    },

    async getCompanyDetailsAsync() {
      if (!this.registeredCustomer) {
        return;
      }

      this.companyNumber = this.registeredCustomer.companyNumber ?? "";
      this.isSoleTrader = this.registeredCustomer.isCompanyNotApplicable;
      this.firmName = this.appService.getCustomerFirmName();
      this.firmDetails.registeredAddress =
        this.registeredCustomer.companyAddress ?? "";

      if (this.registeredCustomer.firmDetail) {
        this.firmDetails = this.registeredCustomer.firmDetail;
        this.isHeadOfficeAddressSameAsTradingAddress =
          this.firmDetails.isHeadOfficeSameAsTradingAddress;
      } else {
        this.firmDetails.firmName = this.firmName;
        this.firmDetails.emailAddress = this.registeredCustomer.email;
        this.firmDetails.companyNumber =
          this.registeredCustomer?.companyNumber ?? "Not Applicable";
        this.firmDetails.firmReferenceNumber =
          this.registeredCustomer.firmReferenceNumber ?? "Not Applicable";
        this.firmDetails.tradingNames = await this.getTradingNamesAsync(
          this.firmDetails.firmReferenceNumber,
        );

        if (this.registeredCustomer.isAuthorised) {
          const addressFromFca =
            await this.fcaService.getFirmAddressesDetailsAsync(
              this.firmDetails.firmReferenceNumber,
              "PPOB",
            );

          if (addressFromFca?.length > 0) {
            const selectedAddress = addressFromFca[0];
            const obj = JSON.parse(JSON.stringify(selectedAddress));

            if (obj["Phone Number"] && obj["country"]) {
              //this is format comes from fca +4402077818019"
              this.firmDetails.contactNumber =
                await this.helperService.convertToContactNoAsync(
                  obj["Phone Number"],
                  obj["country"],
                );
            }

            this.firmDetails.website = obj["Website Address"];
            // build address
            let address = "";
            address += `${obj["Address Line 1"] ? obj["Address Line 1"] : ""}`;
            address += `${
              obj["Address Line 2"] ? `, ${obj["Address Line 2"]}` : ""
            }`;
            address += `${
              selectedAddress.town ? `, ${selectedAddress.town}` : ""
            }`;
            address += `${
              selectedAddress.country ? `, ${selectedAddress.country}` : ""
            }`;
            address += `${
              selectedAddress.postcode ? `, ${selectedAddress.postcode}` : ""
            }`;

            this.firmDetails.registeredAddress = address;
          }
        }
      }

      const firmBasicInfo = new FirmBasicInfo();
      firmBasicInfo.firmName = this.firmDetails.firmName;
      firmBasicInfo.companyNumber = this.firmDetails.companyNumber;
      firmBasicInfo.firmReferenceNumber = this.firmDetails.firmReferenceNumber;
      firmBasicInfo.tradingNames = [...this.firmDetails.tradingNames]; // Create shallow copy
      this.isTradingNamesAlreadyChangedFromDB =
        this.firmDetails.isTradingNamesChanged ?? false;

      if (!this.firmDetails.isRegisteredAddressChanged) {
        firmBasicInfo.address = this.firmDetails.registeredAddress;
      }

      firmBasicInfo.tradingAddress = this.firmDetails.tradingAddress;
      this.inputFirm = firmBasicInfo;

      if (this.registeredCustomer.firmRepresentativeDetail) {
        this.firmRepresentativeDetail =
          this.registeredCustomer.firmRepresentativeDetail;

        if (this.registeredCustomer.accountDepartmentDetail) {
          this.accountDepartmentDetail =
            this.registeredCustomer.accountDepartmentDetail;
          this.isAccountsDepartmentSameAsFirmRepresentative =
            this.accountDepartmentDetail.isSameAsFirmRepresentative;
        } else {
          // if no account department details, set same as firm representative
          this.isAccountsDepartmentSameAsFirmRepresentative = true;
          this.accountDepartmentDetail.isSameAsFirmRepresentative =
            this.isAccountsDepartmentSameAsFirmRepresentative;
          this.setAccountDepartmentDetailsSameAsFirmRepresentative();
        }
      }

      if (this.registeredCustomer.locumDetail) {
        this.locumDetail = this.registeredCustomer.locumDetail;
        this.hasAppointedLocum = this.locumDetail?.hasAppointedALocum;
        this.hasNoAppointedLocumInfo =
          this.locumDetail?.hasNoAppointedLocumInfo ?? "";
        this.isFirmDependentOnSingleKeyIndividual =
          this.locumDetail.isDependentOnSingleKeyIndividual;
        this.isLocumCompany = this.locumDetail.isLocumCompany;

        if (this.locumDetail.isLocumCompany) {
          this.locumCompanyDetail =
            this.locumDetail.locumCompanyDetail ?? new LocumCompanyDetails();
          this.locumInputFirm.firmName =
            this.locumDetail?.locumCompanyDetail?.firmName;
          this.locumInputFirm.companyNumber =
            this.locumDetail?.locumCompanyDetail?.companyNumber;
          this.locumInputFirm.firmReferenceNumber =
            this.locumDetail?.locumCompanyDetail?.firmReferenceNumber;
          this.locumInputFirm.address =
            this.locumDetail?.locumCompanyDetail?.registeredAddress;
          this.locumInputFirm.tradingAddress =
            this.locumDetail?.locumCompanyDetail?.tradingAddress;
        } else if (this.locumDetail.isLocumCompany === false) {
          this.locumSoleTraderDetail =
            this.locumDetail.locumSoleTraderDetail ??
            new LocumSoleTraderDetails();
          this.locumInputFirm.tradingAddress =
            this.locumDetail?.locumSoleTraderDetail?.tradingAddress;
        }
      }
    },

    async getDirectorshipsControllingInterestsAndControllersThenCacheAsync() {
      if (!(this.registeredCustomer && this.companyNumber)) {
        return;
      }

      if (
        this.registeredCustomer.noOfIndividualShareholders < 1 &&
        this.registeredCustomer.noOfCorporateShareholders < 1
      ) {
        // TODO. To get back to confirm if 'getIndividualControllersAsync' is needed.
        const individualControllersPromise =
          this.companyHouseService.getIndividualControllersAsync(
            this.companyNumber,
          );

        const corporateControllersPromise =
          this.companyHouseService.getCorporateControllersAsync(
            this.companyNumber,
          );

        const [individualControllers, corporateControllers] = await Promise.all(
          [individualControllersPromise, corporateControllersPromise],
        );

        if (individualControllers?.length > 0) {
          localStorage.setItem(
            AppConstants.companyIndividualControllersKey,
            JSON.stringify(individualControllers),
          );
        }

        if (corporateControllers?.length > 0) {
          localStorage.setItem(
            AppConstants.companyCorporateControllersKey,
            JSON.stringify(corporateControllers),
          );
        }
      }
    },

    async getTradingNamesAsync(firmReferenceNumber: string): Promise<string[]> {
      if (!firmReferenceNumber || firmReferenceNumber === "Not Applicable") {
        return [] as string[];
      }

      return this.fcaService.getFirmTradingNamesAsync(firmReferenceNumber);
    },

    onTrandingNamesChanged(value: string[]) {
      this.firmDetails.tradingNames = value;
      this.checkIfTradingNamesChanged();
    },

    //ToDo. just make sure values still saved in db
    onToggleTradingAddress(key: string) {
      const currentDetails =
        key === this.firmDetailsKey
          ? this.firmDetails
          : this.locumCompanyDetail;

      if (currentDetails) {
        currentDetails.isTradingSameAsRegisteredAddress =
          !currentDetails.isTradingSameAsRegisteredAddress;

        if (currentDetails.isTradingSameAsRegisteredAddress) {
          currentDetails.tradingAddress =
            currentDetails.registeredAddress ?? "";
        } else {
          currentDetails.tradingAddress = "";
        }
      }
    },

    onToggleHeadOfficeAddress() {
      this.isHeadOfficeAddressSameAsTradingAddress =
        !this.isHeadOfficeAddressSameAsTradingAddress;

      if (this.isHeadOfficeAddressSameAsTradingAddress) {
        this.firmDetails.headOfficeAddress =
          this.firmDetails.tradingAddress ?? "";
      } else {
        this.firmDetails.headOfficeAddress = "";
      }

      this.firmDetails.isHeadOfficeSameAsTradingAddress =
        this.isHeadOfficeAddressSameAsTradingAddress;
    },

    onToggleIsSameFirmRepresentativeDetails() {
      this.isAccountsDepartmentSameAsFirmRepresentative =
        !this.isAccountsDepartmentSameAsFirmRepresentative;
      this.accountDepartmentDetail.isSameAsFirmRepresentative =
        this.isAccountsDepartmentSameAsFirmRepresentative;

      if (this.accountDepartmentDetail.isSameAsFirmRepresentative) {
        this.setAccountDepartmentDetailsSameAsFirmRepresentative();
      } else {
        this.accountDepartmentDetail.title = "";
        this.accountDepartmentDetail.forename = "";
        this.accountDepartmentDetail.surname = "";
        this.accountDepartmentDetail.emailAddress = "";
        this.accountDepartmentDetail.contactNumber = new ContactNumber();
        this.accountDepartmentDetail.jobTitle = "";
      }
    },

    setAccountDepartmentDetailsSameAsFirmRepresentative() {
      this.accountDepartmentDetail.title = this.firmRepresentativeDetail.title;
      this.accountDepartmentDetail.forename =
        this.firmRepresentativeDetail.forename;
      this.accountDepartmentDetail.surname =
        this.firmRepresentativeDetail.surname;
      this.accountDepartmentDetail.emailAddress =
        this.firmRepresentativeDetail.emailAddress;
      this.accountDepartmentDetail.contactNumber =
        this.firmRepresentativeDetail.contactNumber;
      this.accountDepartmentDetail.jobTitle =
        this.firmRepresentativeDetail.jobTitle;
    },

    isValidationErrorPopupShown(): boolean {
      let validationErrorSummary = "";

      const invalidFieldsValidationError =
        this.checkIfHasInvalidFieldValidationErrors();

      if (invalidFieldsValidationError) {
        validationErrorSummary += invalidFieldsValidationError;
      }

      if (validationErrorSummary.length > 0) {
        this.kendoDialogInstance?.setDialogMessage(validationErrorSummary);
        return true;
      }

      return false;
    },

    checkIfHasInvalidFieldValidationErrors(): string {
      let invalidFieldErrors = "";
      const divs = document.querySelectorAll("div.errorMessageDiv");
      const divArray: HTMLDivElement[] = Array.from(divs) as HTMLDivElement[];
      const filteredDivs = divArray.filter(
        (div) => div.innerText.trim().length > 0,
      );

      for (const filteredDiv of filteredDivs) {
        invalidFieldErrors += `${filteredDiv.innerText}\n`;
      }

      return invalidFieldErrors;
    },

    getSectionIndicatorStyle(id: string): string {
      return this.activePageSectionId === id ? "anchor-selected" : "anchor";
    },

    async onToggleAccordionAsync(_: boolean) {
      await this.helperService.delayAsync(50);
      this.updatePageSectionTop();
    },

    onCompanyDetailUpdated(company: FirmBasicInfo) {
      if (!this.firmDetails) {
        return;
      }

      this.firmDetails.firmName = company.firmName;
      this.firmDetails.companyNumber = company.companyNumber;
      this.firmDetails.firmReferenceNumber = company.firmReferenceNumber;
      this.firmDetails.registeredAddress = company.address;
      this.firmDetails.tradingAddress = company.tradingAddress;
      // reset since new search
      this.firmDetails.isTradingNamesChanged = false;
      this.firmDetails.isRegisteredAddressChanged = false;
      this.firmDetails.isTradingAddressChanged = false;
      this.firmDetails.isHeadOfficeAddressChanged = false;
      this.firmDetails.headOfficeAddress = company.headOfficeAddress;
      this.firmDetails.website = company.website;

      //ToDo. TEMP. To get back
      this.firmDetails.contactNumber = {
        country: "GB",
        countryCode: "",
        dialCode: "+44",
        number: company.contactNumber?.replace("+44", ""),
      };
      this.inputFirm = company;
    },

    onFirmRegisteredAddressChanged(value: string) {
      this.firmDetails.registeredAddress = value;

      if (!this.inputFirm?.address) {
        this.firmDetails.isRegisteredAddressChanged = false;
        return;
      }

      this.firmDetails.isRegisteredAddressChanged =
        this.firmDetails.registeredAddress !== this.inputFirm?.address;
    },

    onTradingAddressDoneTyping(elementId: string, hasChanged: boolean) {
      if (!hasChanged || this.isTradingAddressChangedAlertOpened) {
        return;
      }

      this.isTradingAddressChangedAlertOpened = true;

      useAlert({
        title: this.$t("common-alert-title"),
        content: "Are you sure you wish to update your Trading Address?",
        additionalContent: `Current Trading Address with the FCA is ${this.inputFirm.tradingAddress}.`,
        confirmButtonText: "Confirm & Save",
        type: AlertType.ALERT,
        onConfirm: async () => {
          useNotification({
            type: NotificationType.SUCCESS,
            content: "Trading Address updated.",
            interval: AppConstants.notificationPopupTimeOut,
          });

          this.firmDetails.isTradingAddressChanged =
            this.firmDetails.tradingAddress !== this.inputFirm.tradingAddress;
          this.isTradingAddressChangedAlertOpened = false;
          this.eventBusControlElement.emit(
            AppConstants.updateTradingAddressControlStateEvent,
            elementId,
          );
        },
        onClose: () => {
          this.firmDetails.tradingAddress =
            this.inputFirm?.tradingAddress ?? "";

          if (!this.firmDetails.isTradingAddressChanged) {
            this.firmDetails.isTradingAddressChanged = false;
          }

          this.isTradingAddressChangedAlertOpened = false;
        },
      });
    },

    onFirmTradingAddressChanged(value: string) {
      this.firmDetails.tradingAddress = value;

      if (!this.inputFirm?.tradingAddress) {
        this.firmDetails.isTradingAddressChanged = false;
        return;
      }
    },

    onLocumDetailUpdated(locum: FirmBasicInfo) {
      if (!this.locumCompanyDetail) {
        return;
      }

      this.locumCompanyDetail.firmName = locum.firmName;
      this.locumCompanyDetail.companyNumber = locum.companyNumber;
      this.locumCompanyDetail.firmReferenceNumber = locum.firmReferenceNumber;
      this.locumCompanyDetail.fcaStatus = locum.fcaStatus;
      this.locumCompanyDetail.companyHouseStatus = locum.companyHouseStatus;

      if (locum.address) {
        this.locumCompanyDetail.registeredAddress = locum.address;
        this.locumCompanyDetail.isRegisteredAddressChanged = false;
      }

      if (locum.tradingAddress) {
        this.locumCompanyDetail.tradingAddress = locum.tradingAddress;
        this.locumCompanyDetail.isTradingAddressChanged = false;
      }

      this.locumCompanyDetail.website = locum.website;

      //ToDo. TEMP. To get back
      this.locumCompanyDetail.contactNumber = {
        country: "GB",
        countryCode: "",
        dialCode: "+44",
        number: locum.contactNumber?.replace("+44", ""),
      };
      this.locumInputFirm = locum;
    },

    onLocumTradingAddressDoneTyping(elementId: string, hasChanged: boolean) {
      if (
        !hasChanged ||
        this.isTradingAddressChangedAlertOpened ||
        !this.locumInputFirm?.tradingAddress
      ) {
        return;
      }

      this.isTradingAddressChangedAlertOpened = true;

      useAlert({
        title: this.$t("common-alert-title"),
        content: "Are you sure you wish to update your Trading Address?",
        additionalContent: `Current Trading Address with the FCA is ${this.locumInputFirm.tradingAddress}.`,
        confirmButtonText: "Confirm & Save",
        type: AlertType.ALERT,
        onConfirm: async () => {
          useNotification({
            type: NotificationType.SUCCESS,
            content: "Trading Address updated.",
            interval: AppConstants.notificationPopupTimeOut,
          });

          if (this.locumCompanyDetail) {
            this.locumCompanyDetail.isTradingAddressChanged =
              this.locumCompanyDetail.tradingAddress !==
              this.locumInputFirm.tradingAddress;
          }

          this.isTradingAddressChangedAlertOpened = false;
          this.eventBusControlElement.emit(
            AppConstants.updateTradingAddressControlStateEvent,
            elementId,
          );
        },
        onClose: () => {
          if (this.locumCompanyDetail) {
            this.locumCompanyDetail.tradingAddress =
              this.locumInputFirm?.tradingAddress ?? "";

            if (!this.locumCompanyDetail.isTradingAddressChanged) {
              this.locumCompanyDetail.isTradingAddressChanged = false;
            }
          }

          this.isTradingAddressChangedAlertOpened = false;
        },
      });
    },

    onLocumRegisteredAddressChanged(value: string) {
      if (!this.locumCompanyDetail) {
        return;
      }

      this.locumCompanyDetail.registeredAddress = value;

      if (!this.locumInputFirm?.address) {
        this.locumCompanyDetail.isRegisteredAddressChanged = false;
        return;
      }

      this.locumCompanyDetail.isRegisteredAddressChanged =
        this.locumCompanyDetail.registeredAddress !==
        this.locumInputFirm.address;
    },

    onLocumTradingAddressChanged(value: string) {
      if (!this.locumCompanyDetail) {
        return;
      }

      this.locumCompanyDetail.tradingAddress = value;

      if (!this.locumInputFirm?.tradingAddress) {
        this.locumCompanyDetail.isTradingAddressChanged = false;
      }
    },

    onLocumSoleTradingAddressChanged(value: string) {
      if (!this.locumSoleTraderDetail) {
        return;
      }

      this.locumSoleTraderDetail.tradingAddress = value;

      if (!this.locumInputFirm?.tradingAddress) {
        this.locumSoleTraderDetail.isTradingAddressChanged = false;
      }
    },

    onLocumSoleTraderTradingAddressDoneTyping(
      elementId: string,
      hasChanged: boolean,
    ) {
      if (
        !hasChanged ||
        this.isTradingAddressChangedAlertOpened ||
        !this.locumInputFirm?.tradingAddress
      ) {
        return;
      }

      this.isTradingAddressChangedAlertOpened = true;

      useAlert({
        title: this.$t("common-alert-title"),
        content: "Are you sure you wish to update your Trading Address?",
        additionalContent: `Current Trading Address with the FCA is ${this.locumInputFirm.tradingAddress}.`,
        confirmButtonText: "Confirm & Save",
        type: AlertType.ALERT,
        onConfirm: async () => {
          useNotification({
            type: NotificationType.SUCCESS,
            content: "Trading Address updated.",
            interval: AppConstants.notificationPopupTimeOut,
          });

          if (this.locumSoleTraderDetail) {
            this.locumSoleTraderDetail.isTradingAddressChanged =
              this.locumSoleTraderDetail.tradingAddress !==
              this.locumInputFirm.tradingAddress;
          }

          this.isTradingAddressChangedAlertOpened = false;
          this.eventBusControlElement.emit(
            AppConstants.updateTradingAddressControlStateEvent,
            elementId,
          );
        },
        onClose: () => {
          if (this.locumSoleTraderDetail) {
            this.locumSoleTraderDetail.tradingAddress =
              this.locumInputFirm?.tradingAddress ?? "";

            if (!this.locumSoleTraderDetail.isTradingAddressChanged) {
              this.locumSoleTraderDetail.isTradingAddressChanged = false;
            }
          }

          this.isTradingAddressChangedAlertOpened = false;
        },
      });
    },

    showNotAuthorisedFirmMessage(_firm: FirmBasicInfo) {
      this.eventBus.emit(AppConstants.locumFirmNameNotAuthorisedEvent, {
        payload: "",
      });
    },

    onPermissionNotSameAsCustomerFirm(missingPermissions: string[]) {
      this.eventBus.emit(AppConstants.companyNotSamePermissionEvent, {
        payload: "",
      });
      return missingPermissions;
    },

    proceedAnyway() {
      this.firmDetails.firmName = this.companyToProceedIfNotFound;
      this.firmDetails.companyNumber = "";
      this.firmDetails.firmReferenceNumber = "";

      this.inputFirm.firmName = this.firmDetails.firmName;
      this.inputFirm.companyNumber = this.firmDetails.companyNumber;
      this.inputFirm.firmReferenceNumber = this.firmDetails.firmReferenceNumber;
      this.kendoActionDialogInstance?.closeActionDialog();
    },

    proceedLocumAnyway() {
      if (!this.locumDetail.locumCompanyDetail) {
        return;
      }

      this.locumDetail.locumCompanyDetail.firmName =
        this.companyToProceedIfNotFound;
      this.locumDetail.locumCompanyDetail.companyNumber = "";
      this.locumDetail.locumCompanyDetail.firmReferenceNumber = "";

      this.locumInputFirm.firmName =
        this.locumDetail.locumCompanyDetail.firmName;
      this.locumInputFirm.companyNumber =
        this.locumDetail.locumCompanyDetail.companyNumber;
      this.locumInputFirm.firmReferenceNumber =
        this.locumDetail.locumCompanyDetail.firmReferenceNumber;
      this.kendoActionDialogForLocumInstance?.closeActionDialog();
    },

    // NOTE: keeping this. This is the old implementation of changing company number
    // proceedWithUpdateCompanyNumber() {
    //   this.firmDetails.companyNumber = this.companyNoToProceed;
    //   this.kendoPromptInstance?.closeActionDialog();
    //   this.kendoAlertDialogInstance?.showAlertDialogMessage(
    //     "Company Number updated.",
    //   );
    // },
    async confirmPermission() {
      this.kendoPromptDifferentPermissions?.closeActionDialog();
    },

    async confirmNotAuthorised() {
      this.kendoPromptFirmNameNotAuthorised?.closeActionDialog();
    },

    async scheduleAMeetingAsync() {
      this.kendoPromptInstance?.closeActionDialog();

      if (!this.registeredCustomer?.email) {
        return;
      }

      localStorage.setItem(
        AppConstants.emailKey,
        this.registeredCustomer.email,
      );
      let eventTypes = await this.calendlyService.getSchedulingLinkAsync();
      let activeEventTypes = eventTypes.filter((e) => e.active);

      if (!activeEventTypes) {
        this.kendoDialogInstance?.setDialogMessage(
          "There is no active event.",
          "No event found",
        );
        return;
      }

      if (activeEventTypes.length > 1) {
        this.activeEventTypes = activeEventTypes;
        this.kendoCalendarFlexibleDialogInstance?.showMessageAndContent(
          "",
          "Select Event Type",
        );
        return;
      }

      let firstEventType = activeEventTypes[0];
      window.open(firstEventType.scheduling_url);
    },

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    onSelectEventType(event: any) {
      this.selectedEventType = event.target.value;
    },

    onNavigateToScheduling() {
      if (!this.selectedEventType) {
        return;
      }

      window.open(this.selectedEventType.scheduling_url);
      this.kendoCalendarFlexibleDialogInstance?.closeActionDialog();
      this.activeEventTypes = [];
    },

    changeModalBackdropColor() {
      const intervalId = setInterval(() => {
        const modalBackdrop = document.getElementById(
          "kompassious-player-frame",
        );

        if (modalBackdrop) {
          (modalBackdrop as HTMLElement).style.backgroundColor =
            "rgba(13, 13, 13, 0.80)";

          clearInterval(intervalId);
        }
      }, 1000);
    },

    setUniqueIdentifier(value: string): string {
      const identifier = `${AppConstants.firmDetailsRoute}${value}`;
      return identifier.replace(/\s+/g, "").replace("/", "");
    },

    attemptEditEmailAddress() {
      if (this.$refs.kendoActionDialogCompanyNumber) {
        this.kendoPromptInstance = this.$refs
          .kendoActionDialogCompanyNumber as KendoPromptDialogComponent;
      }
      this.kendoPromptInstance?.setActionDialogMessage("Confirm");
    },

    onRemoveItem(selectedItemToRemove: string) {
      this.kendoConfirmDialogInstance?.showConfirmDialogMessage(
        "Confirm",
        `Are you sure you want to delete this ${selectedItemToRemove}`,
        selectedItemToRemove,
      );
    },

    onRemoveItemConfirmed(selectedItemToRemove: string) {
      const indexToRemove =
        this.firmDetails.tradingNames.indexOf(selectedItemToRemove);
      this.firmDetails.tradingNames.splice(indexToRemove, 1);
      this.checkIfTradingNamesChanged();
    },

    checkIfTradingNamesChanged() {
      if (this.isTradingNamesAlreadyChangedFromDB) {
        return;
      }

      const originalTradingNames = this.inputFirm?.tradingNames;

      if (!originalTradingNames) {
        this.firmDetails.isTradingNamesChanged = false;
        return;
      }

      const hasSameLength =
        this.firmDetails.tradingNames.length === originalTradingNames.length;

      if (!hasSameLength) {
        this.firmDetails.isTradingNamesChanged = true;
        return;
      }

      const isComplete = this.firmDetails.tradingNames.every((t) =>
        originalTradingNames.includes(t),
      );
      this.firmDetails.isTradingNamesChanged = !isComplete;
    },
  },
  watch: {
    "firmDetails.contactNumber.number": {
      immediate: true,
      handler(newValue: string) {
        if (
          newValue &&
          newValue.startsWith("+44") &&
          this.firmDetails.contactNumber
        ) {
          this.firmDetails.contactNumber.number = newValue.substring(3);
        }
      },
    },
  },
});
</script>

<template src="./firm-details.html" />

<style scoped src="./firm-details.css" />
