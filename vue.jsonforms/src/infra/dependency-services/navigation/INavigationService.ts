export declare interface INavigationService {
  navigateAsync(route: string): Promise<boolean>;

  navigateRootAsync(route: string): Promise<boolean>;

  navigateBack(): boolean;

  getCurrentPage(): string;

  isCurrentPageFirmProfile(): boolean;

  getActiveRouteServiceRouteSequenceNo(routeName: string): number;

  isActivePage(path: string): boolean;
}

export const INavigationServiceInfo = {
  name: "INavigationService",
};