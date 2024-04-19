import "@/entities/owners-and-controllers/RouteProps";
import { RouteProps } from "@/entities/owners-and-controllers/RouteProps";
import { FCA_URL, CH_URL } from "@/config";

export class AppConstants {
  public static AppName = "Richdale";

  //cache keys
  public static jsonFormsSchemaKey = "schema";
  public static jsonFormsUiSchemaKey = "ui-schema";
  public static jsonFormsAnswersKey = "schema-answers";
  public static jsonFormsSchemaKeyGuide = "schema-guide";
  public static jsonFormsCustomersKey = "customers";
  public static authTokenCacheKey = "auth-token";
  public static fcaSoleTraderFirmsCacheKey = "fca-firms-sole-trader";
  public static fcaFirmsCacheKey = "fca-firms";
  public static authUserKey = "auth-user";
  public static addressManuallyAddedKey = "address-manually-added";
  public static profanityKey = "profanity";
  public static navigateBackToThankYouPageKey = "navigate-thank-you-page";
  public static navigateBackCustomerKey = "navigate-thank-you-page-customer";
  public static navigateBackFirmKey = "navigate-thank-you-page-firm";
  public static GetAddressIoSelectedKey = "home-address-selected";
  public static corporateShareholdersCount = "corporate-shareholders-count";
  public static companyDirectorshipsKey = "directorships";
  public static companyControllingInterestsKey = "controlling-interests";
  public static companyIndividualControllersKey = "individual-controllers";
  public static companyCorporateControllersKey = "corporate-controllers";
  public static emailKey = "email";
  public static contextMenuSelectionProviderKey =
    "provider-context-menu-selection";
  public static contextMenuSelectionIntroducerKey =
    "introducer-context-menu-selection";
  public static signingTypeKey = "signing-type";
  public static profileKey = "profile";
  public static currentComplianceNameKey = "compliance-name";
  public static hasLogOnAlreadyKey = "is-first-time-log-on";
  public static navigationStackKey = "navigation-stack";
  public static saveOrNextComponentSequenceKey = "saveOrNextComponentSequence";
  public static fcaDefinedStatusesKey = "fca-defined-statuses";
  public static companyHouseDefinedStatusesKey =
    "companyhouse-defined-statuses";
  public static customerProductsKey = "customer-products";
  public static onboardingCompletedKey = "onboarding-completed";
  public static customerAppointedRepresentativeKey =
    "customer-appointed-representative";
  public static customerEmployeesKey = "customer-employees";
  public static customerProvidersKey = "customer-providers";
  public static customerIntroducersKey = "customer-introducers";
  public static onboardingTypeKey = "onboarding-type";

  //store with cache
  public static pageComponentValidationValueStore =
    "pageComponentValidationValueStore";
  public static onboardingCompletionPercentageStore =
    "onboardingCompletionPercentageStore";
  public static commentStore = "commentStore";
  public static organizationalStructureStore = "organizationalStructureStore";
  public static pageFieldsInvalidHandlerStore = "pageFieldsInvalidHandlerStore";
  public static safeCustomerStore = "safeCustomerStore";
  public static customerProviderStore = "customerProviderStore";
  public static customerIntroducerStore = "customerIntroducerStore";

  //misc
  public static seekAuthAdd = "Add";
  public static seekAuthRemove = "Remove";
  public static seekAuthPending = "Pending";
  public static autoCompleteCompanyControlDone =
    "autoCompleteCompanyControlDone";
  public static autoCompleteSelectedCompany = "selectedCompanyFromHouse";
  public static notAuthorised = "Not Currently Authorised";
  public static authorised = "Authorised";
  public static PermissionCategoryMortgateBroker = "Mortgage Broker";
  public static PermissionCategoryInsuranceBroker = "Insurance Broker";
  public static PermissionCategoryAdditional = "Additional Authorisation";
  public static SelectedFirm = "selectedFirm";
  public static DateOfBirth = "date-of-birth";
  public static singleIndemnityLimit = 1300380;
  public static aggregateIndemnityLimit = 1924560;
  public static acceptingReferralStatus = "Accepting";
  public static notAcceptingReferralStatus = "Not Accepting";
  public static mortgageBroking = "Mortgage Broking";
  public static protectionBroking = "Protection Broking";
  public static insuranceBroking = "Insurance Broking";
  public static consumerCredit = "Consumer Credit";
  public static generalCommercial =
    "General Commercial Bridging LoansInsurance Broking";
  public static signInTypeCompany = "company";
  public static signInTypeSoleTrader = "sole trader";
  public static signInStep1 = "Basic";
  public static signInStep2 = "Activity";
  public static signInStep3 = "Details";
  public static signInStep4 = "Schedule";
  public static root = "root";
  public static notificationPopupTimeOut = 3e3;
  public static addedState = "Added";
  public static removedState = "Removed";
  public static viewAction = "view";
  public static downloadAction = "download";
  public static portalRouteName = "Portal Route";

  //pub sub
  public static companyNotFoundEvent = "companyNotFoundEvent";
  public static firmNotFoundEvent = "firmNotFoundEvent";
  public static formFieldChangedEvent = "formFieldChangedEvent";
  public static formFieldPageLevelChangedEvent =
    "formFieldChangedEventForSaving";
  public static formExpandOrCollapseEvent = "formExpandOrCollapseEvent";
  public static routesChangedEvent = "routesChangedEvent";
  public static formSavedEvent = "formSavedEvent";
  public static bottomBarEnableEvent = "bottomBarEnableEvent";
  public static nextFormDisableEvent = "nextFormDisableEvent";
  public static companyNumberChangeEvent = "companyNumberChangeEvent";
  public static sideMenuToggledEvent = "sideMenuToggledEvent";
  public static requestCompanyDetailsUpdateEvent =
    "requestCompanyDetailsUpdateEvent";
  public static firmSelectionErrorEvent = "firmSelectionErrorEvent";
  public static userClickSideMenuRouteEvent = "userClickSideMenuRouteEvent";
  public static companyNotSamePermissionEvent = "companyNotSamePermissionEvent";
  public static locumFirmNameNotAuthorisedEvent =
    "locumFirmNameNotAuthorisedEvent";
  public static autoNextEvent = "autoNext";
  public static closeSnipItemsPopupEvent = "closeSnipItemsPopupEvent";
  public static pageFieldInvalidEvent = "pageFieldInvalidEvent";
  public static updateTradingAddressControlStateEvent =
    "updateTradingAddressControlStateEvent";
  public static portalLoadedEvent = "portalLoadedEvent";
  public static filterGridEvent = "onFilterGridEvent";

  //initial data
  public static jsonFormsTypes = ["string", "boolean", "number", "integer"];
  public static cssLists = ["default", "xxx.css"];
  public static UkCountryCode = "+44";
  public static DefaultCountryCode = "gb";

  //route names, firm profile
  public static firmDetailsRoute = "/firm-details";
  public static ownersAndControllersRoute = "/owners-and-controllers";
  public static closeLinksRoute = "/close-links";
  public static permissionProfileRoute = "/permission-profile";
  public static professionalIndemnityRoute =
    "/professional-indemnity-insurance";
  public static employersLiabilityRoute = "/employers-liability-insurance";
  public static stationeryRoute = "/stationery";
  public static providersRoute = "/providers";
  public static privacyNoticeRoute = "/privacy-notice";
  public static dataProtectionRoute = "/data-protection-licence";
  public static organizationalStructureRoute = "/organizational-structure";
  public static marketingAndFinancialPromotionsRoute =
    "/marketing-and-financial-promotions";
  public static introducersRoute = "/introducers";
  public static appointedRepresentativesRoute = "/appointed-representatives";
  public static changePasswordRoute = "/change-password";
  public static arVerifyRoute = "/ar-verify/:email?";
  public static employeeVerifyRoute = "/employee-verify/:email?";
  public static providerVerifyRoute = "/provider-verify/:email?";
  public static introducerVerifyRoute = "/introducer-verify/:email?";
  public static forceLoginRoute = "/force-login";
  public static clientMoneyRoute = "/client-money";
  public static affiliatesRoute = "/affiliates";
  public static signUpRoute = "/signup";
  public static signUpArRoute = "/signup-appointed-representative";

  //route names, ar pages
  public static arFirmDetailsRoute = "/ar-firm-detail";
  public static arCloseLinkRoute = "/ar-close-link";
  public static arOrganizationalStructureRoute = "/ar-organizational-structure";
  public static arOwnersAndControllersRoute = "/ar-owners-and-controllers";
  public static arProfessionalIndemnityRoute =
    "/ar-professional-indemnity-insurance";
  public static arEmployersLiabilityRoute = "/ar-employers-liability-insurance";
  public static arProvidersRoute = "/ar-providers";
  public static arAffiliatesRoute = "/ar-affiliates";
  public static arIntroducersRoute = "/ar-introducers";
  public static arProfileRoute = "/ar-profile";
  public static arDetailsRoute = "/appointed-representative-details";
  public static arDataProtectionRoute = "/ar-data-protection-licence";
  public static arStationeryRoute = "/ar-stationery";
  public static arActivitiesRoute = "/ar-activities";
  public static arMarketingAndFinancialPromotionsRoute =
    "/ar-marketing-and-financial-promotions";

  // route names, employee pages
  public static employeeFirmDetailsRoute = "/employee-firm-detail";
  public static employeeOwnersAndControllersRoute =
    "/employee-owners-and-controllers";
  public static employeeCloseLinkRoute = "/employee-close-link";
  public static employeeOrganizationalStructureRoute =
    "/employee-organizational-structure";

  // route names, provider pages
  public static providerProfileDetailsRoute = "/provider-profile-details";
  public static providerProfileProductsRoute =
      "/provider-profile-products";
  public static providerProfileDirectorsAndControllersRoute =
      "/provider-profile-directors-and-controllers";

  // route names, introducer pages
  public static introducerProfileFirmDetailsRoute = "/introducer-profile-firm-details";
  public static introducerProfileScopeReferralsFeesRoute =
      "/introducer-profile-scope-referrals-fees";
  public static introducerProfileDirectorsAndControllersRoute =
      "/introducer-profile-directors-and-controllers";
  public static introducerProfileDueDiligenceRoute = "/introducer-profile-due-diligence";
  public static introducerProfileDeclarationsRoute =
      "/introducer-profile-declarations";

  // external routes
  public static fcaURLFirmSearch = (firmRefererenceNo: number | string) =>
    `${FCA_URL}/search?q=${firmRefererenceNo}&type=Companies`;
  public static chURLCompanyNumberSearch = (companyNumber: number | string) =>
    `${CH_URL}/company/${companyNumber}`;

  //ToDo. when pages created completed, the order steps are
  //firm details, owners controllers, close links, permissions, org structure chart, professional indemnity,
  // employers liability, data protection, client money, stationary,
  // marketing financial promotions, appointed representative, providers, affilieates, introducers
  public static firmProfilePages: RouteProps[] = [
    {
      route: AppConstants.firmDetailsRoute,
      title: "sidebarNav-firmDetails",
      icon: "clipboard-text-22",
      sequenceNo: 0,
      isDisabled: false,
    },
    {
      route: AppConstants.ownersAndControllersRoute,
      title: "sidebarNav-ownersControllers",
      icon: "user-multiple-group-27",
      sequenceNo: 1,
      isDisabled: false,
    },
    {
      route: AppConstants.closeLinksRoute,
      title: "sidebarNav-closeLinks",
      icon: "no-poverty",
      sequenceNo: 2,
      isDisabled: false,
    },
    {
      route: AppConstants.permissionProfileRoute,
      title: "sidebarNav-permissions",
      icon: "account-setting-40",
      sequenceNo: 3,
      isDisabled: false,
    },
    {
      route: AppConstants.organizationalStructureRoute,
      title: "sidebarNav-organisationalStructureChart",
      icon: "work-organization-34",
      sequenceNo: 4,
      isDisabled: false,
    },
    {
      route: AppConstants.professionalIndemnityRoute,
      title: "sidebarNav-professionalIndemnityInsurance",
      icon: "information-circle",
      sequenceNo: 5,
      isDisabled: false,
    },
    {
      route: AppConstants.employersLiabilityRoute,
      title: "sidebarNav-employersLiabilityInsurance",
      icon: "office-stamp-document",
      sequenceNo: 6,
      isDisabled: false,
    },
    {
      route: AppConstants.dataProtectionRoute,
      title: "sidebarNav-dataProtectionLicence",
      icon: "data-protection",
      sequenceNo: 7,
      isDisabled: false,
    },
    {
      route: AppConstants.clientMoneyRoute,
      title: "sidebarNav-clientMoney",
      icon: "money",
      sequenceNo: 8,
      isDisabled: false,
    },
    {
      route: AppConstants.stationeryRoute,
      title: "sidebarNav-stationery",
      icon: "envelope-letter",
      sequenceNo: 9,
      isDisabled: false,
    },
    {
      route: AppConstants.marketingAndFinancialPromotionsRoute,
      title: "sidebarNav-marketingAndFinancialPromotion",
      icon: "social-profile-click",
      sequenceNo: 10,
      isDisabled: false,
    },
    {
      route: AppConstants.appointedRepresentativesRoute,
      title: "sidebarNav-appointedRepresentatives",
      icon: "user-protection-check",
      sequenceNo: 11,
      isDisabled: false,
    },
    {
      route: AppConstants.providersRoute,
      title: "sidebarNav-providers",
      icon: "user-edit-pencil-45",
      sequenceNo: 12,
      isDisabled: false,
    },
    {
      route: AppConstants.affiliatesRoute,
      title: "sidebarNav-affiliates",
      icon: "user-work-laptop-32",
      sequenceNo: 13,
      isDisabled: false,
    },
    {
      route: AppConstants.introducersRoute,
      title: "sidebarNav-introducers",
      icon: "user-collaborate-group-29",
      sequenceNo: 14,
      isDisabled: false,
    },
  ];

  public static arProfilePages: RouteProps[] = [
    {
      route: AppConstants.arFirmDetailsRoute,
      title: "sidebarNav-ar-firmDetails",
      icon: "clipboard-text-22",
      sequenceNo: 0,
      isDisabled: false,
    },
    {
      route: AppConstants.arOwnersAndControllersRoute,
      title: "sidebarNav-ar-ownersControllers",
      icon: "user-multiple-group-27",
      sequenceNo: 1,
      isDisabled: false,
    },
    {
      route: AppConstants.arCloseLinkRoute,
      title: "sidebarNav-ar-closeLinks",
      icon: "no-poverty",
      sequenceNo: 2,
      isDisabled: false,
    },
    {
      route: AppConstants.arActivitiesRoute,
      title: "sidebarNav-ar-activities",
      icon: "account-setting-40",
      sequenceNo: 3,
      isDisabled: false,
    },
    {
      route: AppConstants.arOrganizationalStructureRoute,
      title: "sidebarNav-ar-organisationalStructureChart",
      icon: "work-organization-34",
      sequenceNo: 4,
      isDisabled: false,
    },
    {
      route: AppConstants.arProfessionalIndemnityRoute,
      title: "sidebarNav-ar-professionalIndemnityInsurance",
      icon: "information-circle",
      sequenceNo: 5,
      isDisabled: false,
    },
    {
      route: AppConstants.arEmployersLiabilityRoute,
      title: "sidebarNav-ar-employersLiabilityInsurance",
      icon: "office-stamp-document",
      sequenceNo: 6,
      isDisabled: false,
    },
    {
      route: AppConstants.arDataProtectionRoute,
      title: "sidebarNav-ar-dataProtectionLicence",
      icon: "data-protection",
      sequenceNo: 7,
      isDisabled: false,
    },
    {
      route: AppConstants.arStationeryRoute,
      title: "sidebarNav-ar-stationery",
      icon: "envelope-letter",
      sequenceNo: 8,
      isDisabled: false,
    },
    {
      route: AppConstants.arMarketingAndFinancialPromotionsRoute,
      title: "sidebarNav-marketingAndFinancialPromotion",
      icon: "social-profile-click",
      sequenceNo: 9,
      isDisabled: false,
    },
    {
      route: AppConstants.arProvidersRoute,
      title: "sidebarNav-ar-providers",
      icon: "user-edit-pencil-45",
      sequenceNo: 10,
      isDisabled: false,
    },
    {
      route: AppConstants.arAffiliatesRoute,
      title: "sidebarNav-ar-affiliates",
      icon: "user-work-laptop-32",
      sequenceNo: 11,
      isDisabled: false,
    },
    {
      route: AppConstants.arIntroducersRoute,
      title: "sidebarNav-ar-introducers",
      icon: "user-collaborate-group-29",
      sequenceNo: 12,
      isDisabled: false,
    },
  ];

  public static employeeProfilePages: RouteProps[] = [
    {
      route: AppConstants.employeeFirmDetailsRoute,
      title: "sidebarNav-employee-firmDetails",
      icon: "clipboard-text-22",
      sequenceNo: 0,
      isDisabled: false,
    },
    {
      route: AppConstants.employeeOwnersAndControllersRoute,
      title: "sidebarNav-employee-ownersControllers",
      icon: "user-multiple-group-27",
      sequenceNo: 1,
      isDisabled: false,
    },
    {
      route: AppConstants.employeeCloseLinkRoute,
      title: "sidebarNav-employee-closeLinks",
      icon: "no-poverty",
      sequenceNo: 2,
      isDisabled: false,
    },
    {
      route: AppConstants.employeeOrganizationalStructureRoute,
      title: "sidebarNav-employee-organisationalStructureChart",
      icon: "work-organization-34",
      sequenceNo: 3,
      isDisabled: false,
    },
  ];

  public static providerProfilePages: RouteProps[] = [
    {
      route: AppConstants.providerProfileDetailsRoute,
      title: "sidebarNav-providers-profile-details",
      icon: "clipboard-text-22",
      sequenceNo: 0,
      isDisabled: false,
    },
    {
      route: AppConstants.providerProfileProductsRoute,
      title: "sidebarNav-providers-profile-products",
      icon: "account-setting-40",
      sequenceNo: 1,
      isDisabled: false,
    },
    {
      route: AppConstants.providerProfileDirectorsAndControllersRoute,
      title: "sidebarNav-providers-profile-directorsControllers",
      icon: "user-multiple-group-27",
      sequenceNo: 2,
      isDisabled: false,
    },
  ];

  public static introducerProfilePages: RouteProps[] = [
    {
      route: AppConstants.introducerProfileFirmDetailsRoute,
      title: "sidebarNav-introducers-profile-firmDetails",
      icon: "clipboard-text-22",
      sequenceNo: 0,
      isDisabled: false,
    },
    {
      route: AppConstants.introducerProfileScopeReferralsFeesRoute,
      title: "sidebarNav-introducers-profile-scopeReferralsFees",
      icon: "no-poverty",
      sequenceNo: 1,
      isDisabled: false,
    },
    {
      route: AppConstants.introducerProfileDirectorsAndControllersRoute,
      title: "sidebarNav-introducers-profile-directorsControllers",
      icon: "user-multiple-group-27",
      sequenceNo: 2,
      isDisabled: false,
    },
    {
      route: AppConstants.introducerProfileDueDiligenceRoute,
      title: "sidebarNav-introducers-profile-dueDiligence",
      icon: "account-setting-40",
      sequenceNo: 3,
      isDisabled: false,
    },
    {
      route: AppConstants.introducerProfileDeclarationsRoute,
      title: "sidebarNav-introducers-profile-declarations",
      icon: "account-setting-40",
      sequenceNo: 4,
      isDisabled: false,
    },
  ];

  //validator keys
  public static RequiredKey = "required";
  public static UrlKey = "url";

  // File extensions
  public static ImageExtensions = ["jpg", "jpeg", "png", "gif", "svg"];
  public static DocumentExtensions = [
    "txt",
    "doc",
    "docx",
    "pdf",
    "xls",
    "xlsx",
    "ppt",
    "pptx",
  ];

  // Disclosures
  public static disclosureTypes = [
    {
      id: "timePeriodDisclosure",
      title: "Time Period Disclosure",
      description:
        "A Time Period Disclosure is required <u>at the beginning</u> of any content which may no-longer be accurate after a specific time period has elapsed.",
      icon: "timer-nine-15",
    },
    {
      id: "affiliateDisclosure",
      title: "Affiliate Disclosure",
      description:
        "An Affiliate Disclosure is required <u>at the beginning</u> of any content which feature a product or service provided by any third party.",
      icon: "business-card",
    },
    {
      id: "taxDisclosure",
      title: "Tax Disclosure",
      description:
        "A Tax Disclosure is required <u>at the beginning</u> of any content which makes reference to any tax considerations.",
      icon: "money-tax",
    },
    {
      id: "mortgageDisclosure",
      title: "Mortgage Disclosure",
      description:
        "A Mortgage Disclosure is required within any content which makes reference to obtaining an Owner Occupier or CBTL Mortgage.",
      icon: "share-money-dollar-22",
    },
    {
      id: "investmentDisclosure",
      title: "Investment Disclosure",
      description:
        "An Investment Disclosure is required <u>at the beginning</u> of any content which makes reference to investing in Readily Realisable Securities (RRS). (Shares or bonds traded on a recognised Stock Exchange).",
      icon: "investment-selection-41",
    },
    {
      id: "cryptoDisclosure",
      title: "Crypto Disclosure",
      description:
        "A Crypto Disclosure is required <u>at the beginning</u> of any content which makes reference to Cryptoassets.",
      icon: "bitcoin-circle-1-80",
    },
  ];

  //page lifecycle name
  public static pageLifeCycleNameMounted = "mounted";
  public static pageLifeCycleNameCreated = "created";

  // Comment content types
  public static CommentFinancialPromotion = "financial-promotion";
}
