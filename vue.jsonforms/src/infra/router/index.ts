import { RouteRecordRaw, createRouter, createWebHistory } from "vue-router";
import Signup from "@/pages/sign-up-pages/signup/Signup.vue";
import DocumentEdit from "../../pages/admin-pages/documents/DocumentEdit.vue";
import Landing from "../../pages/common-pages/authenticate/Landing.vue";
import AdminPortal from "../../pages/admin-pages/admin-portal/AdminPortal.vue";
import BusinessRules from "../../pages/admin-pages/business-rules/BusinessRules.vue";
import EditPermission from "../../pages/admin-pages/business-rules/edit-permission/EditPermission.vue";
import EditStatus from "../../pages/admin-pages/business-rules/edit-status/EditStatus.vue";
import Portal from "../../pages/common-pages/portal/Portal.vue";
import DocumentTemplating from "../../pages/admin-pages/document-templates/DocumentTemplating.vue";
import Settings from "../../pages/admin-pages/settings/Settings.vue";
import PrivacyNotice from "../../pages/common-pages/privacy-notice/PrivacyNotice.vue";
import Privacy from "../../pages/common-pages/privacy/Privacy.vue";
import DocSignView from "../../pages/sign-up-pages/doc-sign-view/DocSignView.vue";
import DirectDebitSignView from "../../pages/sign-up-pages/direct-debit-sign-view/DirectDebitSignView.vue";
import ScheduleMeetingConfirmation from "../../pages/sign-up-pages/schedule-meeting/ScheduleMeetingConfirmation.vue";
import FirmDetails from "../../pages/firm-profile-pages/firm-details/FirmDetails.vue";
import OwnersAndControllers from "../../pages/firm-profile-pages/owners-and-controllers/OwnersAndControllers.vue";
import CloseLinks from "@/pages/firm-profile-pages/close-links/CloseLinks.vue";
import ForceLogin from "../../pages/common-pages/authenticate/ForceLogin.vue";
import ProfessionalIndemnityInsurance from "../../pages/firm-profile-pages/professional-indemnity-insurance/ProfessionalIndemnityInsurance.vue";
import EmployersLiabilityInsurance from "../../pages/firm-profile-pages/employers-liability-insurance/EmployersLiabilityInsurance.vue";
import OrganizationalStructure from "../../pages/firm-profile-pages/organizational-structure/OrganizationalStructure.vue";
import Providers from "../../pages/firm-profile-pages/providers/Providers.vue";
import Introducers from "../../pages/firm-profile-pages/introducers/Introducers.vue";
import { AppConstants } from "../AppConstants";
import ChangePassword from "@/pages/common-pages/change-password/ChangePassword.vue";
import ARVerify from "@/pages/ar-pages/ar-verify/ARVerify.vue";
import Affiliates from "@/pages/firm-profile-pages/affiliates/Affiliates.vue";
import SignupAr from "@/pages/sign-up-pages/signup-ar/SignupAr.vue";
import AROwnersAndControllers from "@/pages/ar-pages/owners-and-controllers/AROwnersAndControllers.vue";
import AROrganizationalStructure from "@/pages/ar-pages/organizational-structure/AROrganizationalStructure.vue";
import EmployeeCloseLinks from "@/pages/employee-pages/close-links/EmployeeCloseLinks.vue";
import EmployeeOrganizationalStructure from "@/pages/employee-pages/organizational-structure/EmployeeOrganizationalStructure.vue";
import ARProfessionalIndemnityInsurance from "@/pages/ar-pages/professional-indemnity-insurance/ARProfessionalIndemnityInsurance.vue";
import AREmployersLiabilityInsurance from "@/pages/ar-pages/employers-liability-insurance/AREmployersLiabilityInsurance.vue";
import EmployeeOwnersAndControllers from "@/pages/employee-pages/owners-and-controllers/EmployeeOwnersAndControllers.vue";
import ARCloseLinks from "@/pages/ar-pages/close-links/ARCloseLinks.vue";
import TestEmployeeDetailsPage from "@/pages/employee-pages/TestEmployeeDetailsPage.vue";
import EmployeeVerify from "@/pages/employee-pages/employee-verify/EmployeeVerify.vue";
import ProviderVerify from "@/pages/provider-profile-pages/provider-verify/ProviderVerify.vue";
import ProviderTestPage from "@/pages/provider-profile-pages/TestProviderDetailsPage.vue";
import IntroducerVerify from "@/pages/introducer-profile-pages/introducer-verify/IntroducerVerify.vue";
import IntroducerTestPage from "@/pages/introducer-profile-pages/TestIntroducerDetailsPage.vue";

// Appointed Representative Pages
const ARDetails = () =>
  import(
    /* webpackChunkName: "ARProfilePages */
    "@/pages/ar-pages/ar-details/ARDetails.vue"
  );

const ARActivities = () =>
  import(
    /* webpackChunkName: "ARActivities */
    "@/pages/ar-pages/activities/ARActivities.vue"
  );

const ARDataProtection = () =>
  import(
    /* webpackChunkName: "ARDataProtection */
    "@/pages/ar-pages/data-protection-licence/ARDataProtectionLicence.vue"
  );

const ARStationery = () =>
  import(
    /* webpackChunkName: "ARStationery */
    "@/pages/ar-pages/stationery/ARStationery.vue"
  );

const ARIntroducers = () =>
  import(
    /* webpackChunkName: "ARIntroducers */
    "@/pages/ar-pages/introducers/ARIntroducers.vue"
  );

const ARAffiliates = () =>
  import(
    /* webpackChunkName: "ARAffiliates */
    "@/pages/ar-pages/affiliates/ARAffiliates.vue"
  );

const ARMarketingAndFinancial = () =>
  import(
    /* webpackChunkName: "ARMarketingAndFinancial */
    "@/pages/ar-pages/marketing-and-financial-promotions/ARMarketingAndFinancialPromotions.vue"
  );

const ARProviders = () =>
  import(
    /* webpackChunkName: "ARProviders */
    "@/pages/ar-pages/providers/ARProviders.vue"
  );

const routes: Array<RouteRecordRaw> = [
  {
    path: "/",
    name: "Landing",
    component: Landing,
  },
  {
    path: AppConstants.forceLoginRoute,
    name: "ForceLogin",
    component: ForceLogin,
  },
  {
    path: AppConstants.signUpRoute,
    name: "Sign Up New",
    component: Signup, // SignUp
    meta: {
      bodyClass: "signup-page",
    },
  },
  {
    path: AppConstants.signUpArRoute,
    name: "Sign Up Appointed Representative",
    component: SignupAr,
    meta: {
      bodyClass: "signup-page",
    },
  },
  {
    path: AppConstants.privacyNoticeRoute,
    name: "Privacy Notice",
    component: PrivacyNotice,
  },
  {
    path: "/admin/documents",
    name: "Admin Documents",
    component: DocumentEdit,
  },
  {
    path: "/admin/portal",
    name: "Admin Portal",
    component: AdminPortal,
  },
  {
    path: "/admin/business-rules",
    name: "Business Rules",
    component: BusinessRules,
  },
  {
    path: "/admin/business-rules/edit-permission",
    name: "Edit Permission",
    component: EditPermission,
  },
  {
    path: "/admin/business-rules/edit-status",
    name: "Edit Status",
    component: EditStatus,
  },
  {
    path: "/admin/document-templates",
    name: "Document Templating",
    component: DocumentTemplating,
  },
  {
    path: "/admin/settings",
    name: "Settings",
    component: Settings,
  },
  {
    path: "/doc-sign-view",
    name: "Doc Sign View",
    component: DocSignView,
  },
  {
    path: "/direct-debit-sign-view",
    name: "Direct Debit Mandate Sign View",
    component: DirectDebitSignView,
  },
  //ToDo. issue in router/index.ts because we can't do like this, see Privacy.vue
  {
    path: "/privacy",
    name: "Privacy",
    component: Privacy,
  },
  {
    path: "/portal",
    name: AppConstants.portalRouteName,
    component: Portal,
    meta: {
      removeFormatting: true,
    },
    children: [
      {
        path: "/admin/portal",
        name: "Admin Portal",
        component: AdminPortal,
      },
      {
        path: "/admin/business-rules/edit-permission",
        name: "Edit Permission",
        component: EditPermission,
      },
      {
        path: "/admin/business-rules/edit-status",
        name: "Edit Status",
        component: EditStatus,
      },
      {
        path: "/admin/document-templates",
        name: "Document Templating",
        component: DocumentTemplating,
      },
      {
        path: AppConstants.permissionProfileRoute,
        name: "Permissions",
        component: () =>
          import(
            "@/pages/firm-profile-pages/permission-profile/PermissionProfile.vue"
          ),
      },
      {
        path: "/admin/settings",
        name: "Settings",
        component: Settings,
      },
      {
        path: AppConstants.privacyNoticeRoute,
        name: "Privacy Notice",
        component: PrivacyNotice,
      },
      {
        path: AppConstants.firmDetailsRoute,
        name: "Firm Details",
        component: FirmDetails,
      },
      {
        path: AppConstants.ownersAndControllersRoute,
        name: "Owners & Controllers",
        component: OwnersAndControllers,
      },
      {
        path: AppConstants.closeLinksRoute,
        name: "Close Links",
        component: CloseLinks,
      },
      {
        path: AppConstants.professionalIndemnityRoute,
        name: "Professional Indemnity Insurance",
        component: ProfessionalIndemnityInsurance,
      },
      {
        path: AppConstants.employersLiabilityRoute,
        name: "Employers Liability Insurance",
        component: EmployersLiabilityInsurance,
      },
      {
        path: AppConstants.stationeryRoute,
        name: "Stationery",
        component: () =>
          import("@/pages/firm-profile-pages/stationery/Stationery.vue"),
      },
      {
        path: AppConstants.dataProtectionRoute,
        name: "Data Protection Licence",
        component: () =>
          import(
            "@/pages/firm-profile-pages/data-protection-licence/DataProtectionLicence.vue"
          ),
      },
      {
        path: AppConstants.clientMoneyRoute,
        name: "client-money",
        component: () =>
          import("@/pages/firm-profile-pages/client-money/ClientMoney.vue"),
      },
      {
        path: AppConstants.organizationalStructureRoute,
        name: "Organisational Structure Chart",
        component: OrganizationalStructure,
      },
      {
        path: AppConstants.providersRoute,
        name: "Providers",
        component: Providers,
      },
      {
        path: AppConstants.marketingAndFinancialPromotionsRoute,
        name: "marketing-and-financial-promotions",
        component: () =>
          import(
            "@/pages/firm-profile-pages/marketing-and-financial-promotions/MarketingAndFinancialPromotions.vue"
          ),
      },
      {
        path: AppConstants.introducersRoute,
        name: "Introducers",
        component: Introducers,
      },
      {
        path: AppConstants.affiliatesRoute,
        name: "affiliates",
        component: Affiliates,
      },
      {
        path: AppConstants.appointedRepresentativesRoute,
        name: "appointed-representatives",
        component: () =>
          import(
            "@/pages/firm-profile-pages/appointed-representatives/AppointedRepresentatives.vue"
          ),
      },
      // AR routes
      {
        path: AppConstants.arFirmDetailsRoute,
        name: "Appointed Representatives (AR) Details",
        component: ARDetails,
        meta: {
          breadcrumb: "Details",
          removeFormatting: true,
        },
      },
      {
        path: AppConstants.arOwnersAndControllersRoute,
        name: "Appointed Representative Owners & Controllers",
        component: AROwnersAndControllers,
        meta: {
          breadcrumb: "Owners & Controllers",
        },
      },
      {
        path: AppConstants.arCloseLinkRoute,
        name: "Appointed Representative Close Links",
        component: ARCloseLinks,
        meta: {
          breadcrumb: "Close Links",
        },
      },
      {
        path: AppConstants.arActivitiesRoute,
        name: "AR Activities",
        component: ARActivities,
        meta: {
          breadcrumb: "Activities",
        },
      },
      {
        path: AppConstants.arMarketingAndFinancialPromotionsRoute,
        name: "AR MarketingAndFinancial",
        component: ARMarketingAndFinancial,
        meta: {
          breadcrumb: "Marketing and Financial",
        },
      },
      {
        path: AppConstants.arProvidersRoute,
        name: "Appointed Representative Providers",
        component: ARProviders,
        meta: {
          breadcrumb: "Providers",
        },
      },
      {
        path: AppConstants.arAffiliatesRoute,
        name: "Appointed Representative Affiliates",
        component: ARAffiliates,
        meta: {
          breadcrumb: "Affiliates",
        },
      },
      {
        path: AppConstants.arOrganizationalStructureRoute,
        name: "Appointed Representative Organisational Structure Chart",
        component: AROrganizationalStructure,
        meta: {
          breadcrumb: "Organisational Structure Chart",
        },
      },
      {
        path: AppConstants.arProfessionalIndemnityRoute,
        name: "Appointed Representative Professional Indemnity Insurance",
        component: ARProfessionalIndemnityInsurance,
        meta: {
          breadcrumb: "Professional Indemnity Insurance",
        },
      },
      {
        path: AppConstants.arEmployersLiabilityRoute,
        name: "Appointed Representative Employers Liability Insurance",
        component: AREmployersLiabilityInsurance,
        meta: {
          breadcrumb: "Employers Liability Insurance",
        },
      },
      {
        path: AppConstants.arDataProtectionRoute,
        name: "AR Data Protection Licence",
        component: ARDataProtection,
        meta: {
          breadcrumb: "Data Protection Licence",
        },
      },
      {
        path: AppConstants.arStationeryRoute,
        name: "AR stationery",
        component: ARStationery,
        meta: {
          breadcrumb: "Stationery",
        },
      },
      {
        path: AppConstants.arIntroducersRoute,
        name: "Appointed Representative Introducers",
        component: ARIntroducers,
        meta: {
          breadcrumb: "Introducers",
        },
      },
      // Employee routes
      {
        path: AppConstants.employeeFirmDetailsRoute,
        name: "Employee Details",
        component: TestEmployeeDetailsPage,
        meta: {
          removeFormatting: true,
        },
      },
      {
        path: AppConstants.employeeOwnersAndControllersRoute,
        name: "Employee Owners & Controllers",
        component: EmployeeOwnersAndControllers,
      },
      {
        path: AppConstants.employeeCloseLinkRoute,
        name: "Employee Close Links",
        component: EmployeeCloseLinks,
      },
      {
        path: AppConstants.employeeOrganizationalStructureRoute,
        name: "Employee Organisational Structure Chart",
        component: EmployeeOrganizationalStructure,
        meta: {
          breadcrumb: "Organisational Structure Chart",
        },
      },
      // Provider routes
      {
        path: AppConstants.providerProfileDetailsRoute,
        name: "Provider Details",
        component: ProviderTestPage,
      },
      {
        path: AppConstants.providerProfileProductsRoute,
        name: "Provider Products",
        component: ProviderTestPage,
      },
      {
        path: AppConstants.providerProfileDirectorsAndControllersRoute,
        name: "Provider Directors & Controllers",
        component: ProviderTestPage,
      },
      // Introducers routes
      {
        path: AppConstants.introducerProfileFirmDetailsRoute,
        name: "Introducer Firm Details",
        component: IntroducerTestPage,
      },
      {
        path: AppConstants.introducerProfileScopeReferralsFeesRoute,
        name: "Introducer Scope and Referrals (& Fees)",
        component: IntroducerTestPage,
      },
      {
        path: AppConstants.introducerProfileDirectorsAndControllersRoute,
        name: "Introducer Directors & Controllers",
        component: IntroducerTestPage,
      },
      {
        path: AppConstants.introducerProfileDueDiligenceRoute,
        name: "Introducer Due Diligence",
        component: IntroducerTestPage,
      },
      {
        path: AppConstants.introducerProfileDeclarationsRoute,
        name: "Introducer Declarations",
        component: IntroducerTestPage,
      },
    ],
  },
  {
    path: "/schedule-meeting-confirmation",
    name: "Schedule Meeting Confirmation",
    component: ScheduleMeetingConfirmation,
  },
  {
    path: AppConstants.changePasswordRoute,
    name: "Change Password",
    component: ChangePassword,
  },
  {
    path: AppConstants.arVerifyRoute,
    name: "AR Verify",
    component: ARVerify,
  },
  {
    path: AppConstants.employeeVerifyRoute,
    name: "Customer Employee Verify",
    component: EmployeeVerify,
  },
  {
    path: AppConstants.providerVerifyRoute,
    name: "Customer Provider Verify",
    component: ProviderVerify,
  },
  {
    path: AppConstants.introducerVerifyRoute,
    name: "Customer Introducer Verify",
    component: IntroducerVerify,
  },
];

//add additional routes but these are only in dev mode
if (import.meta.env.DEV) {
  routes.push({
    path: "/testonly",
    name: "TestOnly",
    component: () => import("@/pages/tests/TestOnly.vue"),
  });

  routes.push({
    path: "/test-icons",
    name: "TestIcons",
    component: () => import("@/pages/tests/TestIcons.vue"),
  });

  routes.push({
    path: "/test-alert",
    name: "TestAlert",
    component: () => import("@/pages/tests/TestAlert.vue"),
  });

  routes.push({
    path: "/test-anchor",
    name: "TestAnchorWithScroll",
    component: () => import("@/pages/tests/TestAnchorWithScroll.vue"),
  });

  routes.push({
    path: "/playground",
    name: "Playground",
    component: () => import("@/pages/tests/Playground.vue"),
  });

  routes.push({
    path: "/testprogressbar",
    name: "TestProgressBar",
    component: () => import("@/pages/tests/TestProgressBar.vue"),
  });
}

const router = createRouter({
  history: createWebHistory(),
  routes,
});

// set/unset body class for each pages
router.afterEach((to, from) => {
  if (from.meta.bodyClass) {
    document.body.classList.remove(`${from.meta.bodyClass}`);
  }
  if (to.meta.bodyClass) {
    document.body.classList.add(`${to.meta.bodyClass}`);
  }
});

export default router;
