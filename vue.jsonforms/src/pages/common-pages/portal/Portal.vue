<script lang="ts">
import { defineComponent, defineAsyncComponent, inject } from "vue";
import { container } from "tsyringe";
import { LANDING_URL } from "@/config";
import { AppConstants } from "@/infra/AppConstants";
import {
  IAppService,
  IAppServiceInfo,
} from "@/infra/dependency-services/app/IAppService";
import {
  ICustomerService,
  ICustomerServiceInfo,
} from "@/infra/dependency-services/rest/forms-compliance/ICustomerService";
import {
  IAuth0Service,
  IAuth0ServiceInfo,
} from "@/infra/dependency-services/rest/auth0/IAuth0Service";
import {
  IEncodingService,
  IEncodingServiceInfo,
} from "@/infra/dependency-services/encoding/IEncodingService";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import {
  IProfanityService,
  IProfanityServiceInfo,
} from "@/infra/dependency-services/rest/profanity/IProfanityService";
import {
  INavigationService,
  INavigationServiceInfo,
} from "@/infra/dependency-services/navigation/INavigationService";
import {
  IFirmPagesRouteService,
  IFirmPagesRouteServiceInfo,
} from "@/infra/dependency-services/pages-route-firm/IFirmPagesRouteService";
import {
  IIntroducerPagesRouteService,
  IIntroducerPagesRouteServiceInfo,
} from "@/infra/dependency-services/pages-route-introducers/IIntroducerPagesRouteService";
import {
  ISequenceNoKeeperService,
  ISequenceNoKeeperServiceInfo,
} from "@/infra/dependency-services/sequence-no/ISequenceNoKeeperService";
import { RouteProps } from "@/entities/owners-and-controllers/RouteProps";
import { Emitter, EventType } from "mitt";
import { useStorage } from "@vueuse/core";
import { CustomerEntity } from "@/entities/CustomerEntity";
import { useColorGenerator } from "@/composables/useColorGenerator";
import { PermissionStateEnum } from "@/entities/enums/PermissionStateEnum";
import {
  IAppointedRepresentativePagesRouteService,
  IAppointedRepresentativePagesRouteServiceInfo,
} from "@/infra/dependency-services/pages-route-ar/IAppointedRepresentativePagesRouteService";
import { OnboardingType } from "@/infra/base";
import { CustomerBase } from "@/entities/CustomerBase";
import {
  IEmployeePagesRouteService,
  IEmployeePagesRouteServiceInfo,
} from "@/infra/dependency-services/pages-route-employee/IEmployeePagesRouteService";
import {
  IProviderPagesRouteService,
  IProviderPagesRouteServiceInfo,
} from "@/infra/dependency-services/pages-route-providers/IProviderPagesRouteService";

export default defineComponent({
  name: "Portal",
  components: {
    TopBar: defineAsyncComponent(() => import("./partials/TopBar.vue")),
    OnBoarding: defineAsyncComponent(() => import("./partials/OnBoarding.vue")),
  },
  computed: {
    OnboardingType() {
      return OnboardingType;
    },
    AppConstants() {
      return AppConstants;
    },
  },
  data() {
    return {
      sideMenuRouteItems: [] as RouteProps[],
      authService: container.resolve<IAuth0Service>(IAuth0ServiceInfo.name),
      base64EncodingService: container.resolve<IEncodingService>(
        IEncodingServiceInfo.name,
      ),
      profanityService: container.resolve<IProfanityService>(
        IProfanityServiceInfo.name,
      ),
      firmPagesRouteService: container.resolve<IFirmPagesRouteService>(
        IFirmPagesRouteServiceInfo.name,
      ),
      appointedRepresentativePagesRouteService:
        container.resolve<IAppointedRepresentativePagesRouteService>(
          IAppointedRepresentativePagesRouteServiceInfo.name,
        ),
      employeePagesRouteService: container.resolve<IEmployeePagesRouteService>(
        IEmployeePagesRouteServiceInfo.name,
      ),
      providerPagesRouteService: container.resolve<IProviderPagesRouteService>(
        IProviderPagesRouteServiceInfo.name,
      ),
      introducerPagesRouteService:
        container.resolve<IIntroducerPagesRouteService>(
          IIntroducerPagesRouteServiceInfo.name,
        ),
      navigationService: container.resolve<INavigationService>(
        INavigationServiceInfo.name,
      ),
      sequenceNoKeeperService: container.resolve<ISequenceNoKeeperService>(
        ISequenceNoKeeperServiceInfo.name,
      ),
      proposalUrl: "",
      isSidebarOpen: false,
      isDialogOpen: false,
      appService: container.resolve<IAppService>(IAppServiceInfo.name),
      customerService: container.resolve<ICustomerService>(
        ICustomerServiceInfo.name,
      ),
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      currentEmailFromQueryString: "",
      pleaseWaitText:
        "Please do not close your browser! We are diligently preparing your document for the signing process.",
      noDocumentYetText: "",
      activeGroup: null,
      adminGroupName: "admin",
      firmProfileGroupName: "firmProfile",
      providersGroupName: "provider",
      isLoading: true,
      isBusyFrameRefresh: false,
      intervalId: null as unknown as NodeJS.Timeout, //ToDo. correct this  declaration
      isBackDisabled: true,
      introducerGroupName: "provider",
      isBottomBarEnabled: false,
      isNextButtonDisabled: false,
      eventBusRoutes: inject("$eventBusService") as Emitter<
        Record<EventType, RouteProps[]>
      >,
      eventBusSideMenuRoute: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      onboardingType: OnboardingType.Firm.toString(),
    };
  },
  watch: {
    proposalUrl(newVal, _oldVal) {
      if (newVal && newVal.toLowerCase().includes("signnow")) {
        this.noDocumentYetText = "";
        clearInterval(this.intervalId);
        this.isBackDisabled = false;
      }
    },
    isSideMenuOpen(value) {
      this.eventBusRoutes.emit(AppConstants.sideMenuToggledEvent, value);
    },
  },
  async created() {
    this.currentEmailFromQueryString = this.extractEmailFromEncodedUrl();
    let savedToken = localStorage.getItem(AppConstants.authTokenCacheKey);

    if (this.currentEmailFromQueryString && !savedToken) {
      await this.authService.getTokenForSignupAndSaveLocallyAsync();
    }

    if (!savedToken) {
      await this.$auth0.loginWithRedirect();
      return;
    }

    this.eventBusRoutes.on(AppConstants.routesChangedEvent, (value) => {
      if (!value) {
        return;
      }

      this.sideMenuRouteItems = value;
    });

    this.eventBusRoutes.on(AppConstants.bottomBarEnableEvent, (value: any) => {
      this.isBottomBarEnabled = value;
    });

    this.eventBusRoutes.on(AppConstants.nextFormDisableEvent, (value: any) => {
      this.isNextButtonDisabled = value;
    });

    if (this.currentEmailFromQueryString) {
      this.noDocumentYetText = this.pleaseWaitText;
      this.isDialogOpen = true;
      this.isLoading = false;
      this.intervalId = setInterval(this.refreshIframeAsync, 3000);
      return;
    }

    this.appService.clearThankYouPageCache();
    this.toggleSidebar();

    this.onboardingType =
      localStorage.getItem(AppConstants.onboardingTypeKey) ??
      OnboardingType.Firm.toString();
    this.setSideMenuRoutes();

    if (this.sideMenuRouteItems) {
      let customer: CustomerBase | undefined = undefined;

      switch (this.onboardingType) {
        case OnboardingType.Ar:
          customer = this.appService.getCachedCustomerAppointedRepresentative();

          //when you disable item in sideMenuRouteItems, the AppConstants.firmProfilePages will be the same because they are same memory
          this.sideMenuRouteItems.forEach((item) => {
            if (!customer) {
              throw new Error(
                "customer should be cached already before we can go to portal",
              );
            }

            // Rule 1: If current customer is sole trader, disable close link and owners & controllers
            if (
              customer.isCompanyNotApplicable &&
              (item.route === AppConstants.arCloseLinkRoute ||
                item.route === AppConstants.arOwnersAndControllersRoute)
            ) {
              item.isDisabled = true;
            }
          });
          break;
        case OnboardingType.Employee:
          //do nothing for now
          break;
        case OnboardingType.Provider:
          //do nothing for now
          break;
        case OnboardingType.Introducer:
          //do nothing for now
          break;
        case OnboardingType.Firm:
        default:
          customer = this.appService.getCachedCustomer();

          //when you disable item in sideMenuRouteItems, the AppConstants.firmProfilePages will be the same because they are same memory
          this.sideMenuRouteItems.forEach((item) => {
            if (!customer) {
              throw new Error(
                "customer should be cached already before we can go to portal",
              );
            }

            // Rule 1: If current customer is sole trader, disable close link and owners & controllers
            if (
              customer.isCompanyNotApplicable &&
              (item.route === AppConstants.closeLinksRoute ||
                item.route === AppConstants.ownersAndControllersRoute)
            ) {
              item.isDisabled = true;
            }

            // Rule 2: If "Hold and Control Client Money" is not authorized, disable client money
            const currentCustomer = customer as CustomerEntity;
            const permissionName = "Hold and Control Client Money";
            const permissions = currentCustomer.customerPermissions || [];
            if (
              !permissions.find(
                ({ subPermissionName, state }) =>
                  permissionName.toLowerCase() ===
                    subPermissionName?.toLowerCase() &&
                  state === PermissionStateEnum.Added,
              ) &&
              item.route === AppConstants.clientMoneyRoute
            ) {
              item.isDisabled = true;
            }
          });
          break;
      }
    }

    this.registerRoutes();
    await this.profanityService.getProfanityWordsAndSaveLocallyAsync();
    this.isDialogOpen = false;
    this.isLoading = false;

    //ToDo. OnBoarding. To see the timings as we have onmounted below navigation as well, will polish soon...
    this.eventBusSideMenuRoute.emit(AppConstants.portalLoadedEvent);
  },
  async mounted() {
    //do not route the app if the 'currentEmailFromQueryString' has value or meaning the proposal signup is showing
    if (this.currentEmailFromQueryString) {
      return;
    }

    await this.redirectToFirstPageAsync();
  },
  methods: {
    toggleSidebar() {
      this.isSidebarOpen = !this.isSidebarOpen;
      this.toggleGroup(this.adminGroupName);
    },

    toggleGroup(groupName: any) {
      if (this.isSidebarOpen && this.activeGroup != groupName) {
        this.activeGroup = groupName;
      } else {
        this.activeGroup = null;
      }
    },

    async refreshIframeAsync() {
      if (this.isBusyFrameRefresh) {
        return;
      }

      this.isBusyFrameRefresh = true;
      let customer = await this.customerService.getCustomerByEmailAsync(
        this.currentEmailFromQueryString,
      );
      let urlResult = customer.embeddedSigning?.link as string;

      if (urlResult) {
        this.proposalUrl = customer.embeddedSigning?.link as string;
      }

      await this.helperService.delayAsync(6000);
      this.isBusyFrameRefresh = false;
    },

    async closeSignNowDialog() {
      await this.logoutAsync();
      this.isDialogOpen = !this.isDialogOpen;
    },

    async logoutAsync() {
      await this.$auth0.logout({
        logoutParams: {
          returnTo: LANDING_URL,
        },
      });
      this.appService.clearAllLocalCache();
      this.appService.clearThankYouPageCache();
      localStorage.removeItem(AppConstants.authTokenCacheKey);
      localStorage.removeItem(AppConstants.authUserKey);
      localStorage.removeItem(AppConstants.SelectedFirm);
    },

    extractEmailFromEncodedUrl(): string {
      let indexOfQuestionMark = this.$route.fullPath.indexOf("?");

      if (indexOfQuestionMark < 1) {
        return "";
      }

      let queryPart = this.$route.fullPath.substring(indexOfQuestionMark + 1);
      let queryString = this.base64EncodingService.decode(queryPart);
      return queryString.replace("email=", "");
    },

    async redirectToAsync(routeName: string) {
      if (this.navigationService.isActivePage(routeName)) {
        return;
      }

      const activeSequenceNo =
        this.navigationService.getActiveRouteServiceRouteSequenceNo(routeName);
      this.sequenceNoKeeperService.setSequenceNo(activeSequenceNo);

      await this.navigationService.navigateRootAsync(routeName);
      this.eventBusSideMenuRoute.emit(AppConstants.userClickSideMenuRouteEvent);
    },

    isMenuActive(route: string) {
      return this.navigationService.isActivePage(route);
    },

    handleMenuItemHover(event: MouseEvent, out = false) {
      const stampClass = "MainDrawer-menuStamp";
      const activeClass = "MainDrawer-menuActive";

      if (out) {
        document
          .querySelectorAll(`.${stampClass}`)
          .forEach((el) => el.remove());
        return;
      }

      if (!event.target || !this.$refs["mainDrawer"]) {
        return;
      }

      const mainDrawer = (this.$refs["mainDrawer"] as any).$el as HTMLElement;

      if (mainDrawer.classList.contains("k-drawer-expanded")) {
        return;
      }

      const menuItem = event.target as HTMLElement;
      const menuStamp = menuItem.cloneNode(true) as HTMLElement;
      const wrapper = menuItem.closest(".k-drawer-wrapper") as HTMLElement;
      const offsetTop = menuItem.offsetTop;
      const paddingTop = parseInt(getComputedStyle(wrapper).paddingTop);
      const scrollTop = wrapper.scrollTop;
      menuStamp.style.setProperty("left", `${menuItem.offsetLeft}px`);
      menuStamp.style.setProperty(
        "top",
        `${offsetTop + paddingTop - scrollTop}px`,
      );
      menuStamp.classList.add(stampClass);
      mainDrawer.appendChild(menuStamp);
      setTimeout(() => menuStamp.classList.add(activeClass), 100);
    },

    getMenuStatusColor(percent: number) {
      const { hexToRgb, rgbToHex } = useColorGenerator();

      const color1Rgb = hexToRgb("#FFB700");
      const color2Rgb = hexToRgb("#39AC73");

      return rgbToHex(
        Math.round(color1Rgb.r + (color2Rgb.r - color1Rgb.r) * (percent / 100)),
        Math.round(color1Rgb.g + (color2Rgb.g - color1Rgb.g) * (percent / 100)),
        Math.round(color1Rgb.b + (color2Rgb.b - color1Rgb.b) * (percent / 100)),
      );
    },

    setSideMenuRoutes() {
      switch (this.onboardingType) {
        case OnboardingType.Ar: {
          this.sideMenuRouteItems = AppConstants.arProfilePages;
          break;
        }
        case OnboardingType.Employee: {
          this.sideMenuRouteItems = AppConstants.employeeProfilePages;
          break;
        }
        case OnboardingType.Provider: {
          this.sideMenuRouteItems = AppConstants.providerProfilePages;
          break;
        }
        case OnboardingType.Introducer: {
          this.sideMenuRouteItems = AppConstants.introducerProfilePages;
          break;
        }
        default: {
          this.sideMenuRouteItems = AppConstants.firmProfilePages;
          break;
        }
      }
    },

    registerRoutes() {
      switch (this.onboardingType) {
        case OnboardingType.Ar: {
          this.appointedRepresentativePagesRouteService.registerRoutes(
            AppConstants.arProfilePages,
          );
          break;
        }
        case OnboardingType.Employee: {
          this.employeePagesRouteService.registerRoutes(
            AppConstants.employeeProfilePages,
          );
          break;
        }
        case OnboardingType.Provider: {
          this.providerPagesRouteService.registerRoutes(
            AppConstants.providerProfilePages,
          );
          break;
        }
        case OnboardingType.Introducer: {
          this.introducerPagesRouteService.registerRoutes(
            AppConstants.introducerProfilePages,
          );
          break;
        }
        default: {
          this.firmPagesRouteService.registerRoutes(
            AppConstants.firmProfilePages,
          );
          break;
        }
      }
    },

    async redirectToFirstPageAsync() {
      switch (this.onboardingType) {
        case OnboardingType.Ar: {
          const customerArPagesRoutes =
            this.appointedRepresentativePagesRouteService.getCurrentRoutes();
          const foundCustomerArPage = customerArPagesRoutes.find(
            (r) => r.route === this.navigationService.getCurrentPage(),
          );
          //if not coming from logout and login page, or user just refresh browser, do not redirect
          if (foundCustomerArPage) {
            return;
          }

          await this.redirectToAsync(AppConstants.arFirmDetailsRoute);
          break;
        }
        case OnboardingType.Employee: {
          const employeePagesRoutes =
            this.employeePagesRouteService.getCurrentRoutes();
          const foundEmployeePage = employeePagesRoutes.find(
            (r) => r.route === this.navigationService.getCurrentPage(),
          );
          //if not coming from logout and login page, or user just refresh browser, do not redirect
          if (foundEmployeePage) {
            return;
          }

          await this.redirectToAsync(AppConstants.employeeFirmDetailsRoute);
          break;
        }
        case OnboardingType.Provider: {
          const providerPagesRoutes =
            this.providerPagesRouteService.getCurrentRoutes();
          const foundProviderPage = providerPagesRoutes.find(
            (r) => r.route === this.navigationService.getCurrentPage(),
          );
          //if not coming from logout and login page, or user just refresh browser, do not redirect
          if (foundProviderPage) {
            return;
          }

          await this.redirectToAsync(AppConstants.providerProfileDetailsRoute);
          break;
        }
        case OnboardingType.Introducer: {
          const introducerPagesRoutes =
            this.introducerPagesRouteService.getCurrentRoutes();
          const foundIntroducerPage = introducerPagesRoutes.find(
            (r) => r.route === this.navigationService.getCurrentPage(),
          );
          //if not coming from logout and login page, or user just refresh browser, do not redirect
          if (foundIntroducerPage) {
            return;
          }

          await this.redirectToAsync(
            AppConstants.introducerProfileFirmDetailsRoute,
          );
          break;
        }
        default: {
          const firmPagesRoutes = this.firmPagesRouteService.getCurrentRoutes();
          const foundFirmPage = firmPagesRoutes.find(
            (r) => r.route === this.navigationService.getCurrentPage(),
          );

          //if not coming from logout and login page, or user just refresh browser, do not redirect
          if (foundFirmPage) {
            return;
          }

          await this.redirectToAsync(AppConstants.firmDetailsRoute);
          break;
        }
      }
    },
  },
  setup() {
    const isSideMenuOpen = useStorage("sidebar-open", false);
    return {
      isSideMenuOpen,
    };
  },
});
</script>

<template src="./portal.html" />

<style src="./portal.scss" scoped />
