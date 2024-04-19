import { container, singleton } from "tsyringe";
import { Router, RouteLocationNormalizedLoaded } from "vue-router";
import { INavigationService } from "./INavigationService";
import router from "../../../infra/router";
import { AppConstants } from "@/infra/AppConstants";
import { OnboardingType } from "@/infra/base";
import { IPagesRouteService } from "@/infra/dependency-services/navigation/IPagesRouteService";
import {
  IPageRouteServiceFactory,
  IPageRouteServiceFactoryInfo
} from "@/infra/dependency-services/pages-route-service-factory/IPageRouteServiceFactory";

@singleton()
export default class NavigationService implements INavigationService {
  private readonly _router: Router = router;
  private readonly _pageRouteServiceFactory: IPageRouteServiceFactory;
  private readonly _pagesRouteService: IPagesRouteService;

  //This nav stack is cleared when browser is refreshed so just cached it
  private _navigationStack: string[] = [];

  constructor() {
    const onboardingType =
      localStorage.getItem(AppConstants.onboardingTypeKey) ??
      OnboardingType.Firm.toString();

    this._pageRouteServiceFactory = container.resolve<IPageRouteServiceFactory>(
      IPageRouteServiceFactoryInfo.name,
    );

    //this route service can be for firm, ar, employee, etc depends on the onboarding type
    this._pagesRouteService =
      this._pageRouteServiceFactory.createPageRouteService(onboardingType);
  }

  public async navigateAsync(route: string): Promise<boolean> {
    const result = await this._router.push(route);

    if (result) {
      return false;
    }

    const currentNavigationStack = localStorage.getItem(
      AppConstants.navigationStackKey,
    );

    if (!currentNavigationStack) {
      this.resetNavigationStack(route);
      return false;
    }

    this._navigationStack = JSON.parse(currentNavigationStack) as string[];
    this._navigationStack.push(route);
    localStorage.setItem(
      AppConstants.navigationStackKey,
      JSON.stringify(this._navigationStack),
    );
    return true;
  }

  public async navigateRootAsync(route: string): Promise<boolean> {
    //vue router don't provide to delete router history
    //more like a forward only
    const result = await this._router.push(route);

    if (result) {
      return false;
    }

    this.resetNavigationStack(route);
    return true;
  }

  //ToDo. this dont work good yet
  //ref https://router.vuejs.org/guide/essentials/navigation.html
  public navigateBack(): boolean {
    this._router.go(-1);
    this._navigationStack.pop();
    return true;
  }

  public isCurrentPageFirmProfile(): boolean {
    const firmProfilePages = this._pagesRouteService.getCurrentRoutePaths();
    const currentPage = this.getCurrentPage();
    return firmProfilePages.includes(currentPage);
  }

  public getCurrentPage(): string {
    const currentNavigationStack = localStorage.getItem(
      AppConstants.navigationStackKey,
    );

    if (currentNavigationStack) {
      this._navigationStack = JSON.parse(currentNavigationStack) as string[];
    }

    if (this._navigationStack.length === 0) {
      return "";
    }

    return this._navigationStack[this._navigationStack.length - 1];
  }

  public getActiveRouteServiceRouteSequenceNo(routeName: string): number {
    return this._pagesRouteService.getSequenceNo(routeName);
  }

  public isActivePage(path: string): boolean {
    const currentRoute = this._router
      .currentRoute as unknown as RouteLocationNormalizedLoaded;
    return currentRoute.path === path;
  }

  private resetNavigationStack(route: string): void {
    this._navigationStack = [];
    this._navigationStack.push(route);
    localStorage.setItem(
      AppConstants.navigationStackKey,
      JSON.stringify(this._navigationStack),
    );
  }
}
