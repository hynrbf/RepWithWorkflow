import { Error } from "@progress/kendo-vue-labels";
import { RouteProps } from "@/entities/owners-and-controllers/RouteProps";

export default abstract class PagesRouteServiceBase {
  protected _pagesRoutePaths: string[] = [];
  protected _pagesRoutes: RouteProps[] = [];
  protected _enabledPagesRoutes: RouteProps[] = [];

  public getCurrentRoutePaths(): string[] {
    if (!this._pagesRoutePaths) {
      throw new Error("Please register the firm pages routes first");
    }

    if (this._pagesRoutePaths.length <= 0) {
      throw new Error(
        "Please register the firm pages routes first with no empty",
      );
    }

    return this._pagesRoutePaths;
  }

  public registerRoutes(data: RouteProps[]): void {
    if (!(data && data.length > 0)) {
      throw new Error("Please register with data that has item");
    }

    this._pagesRoutePaths = [];
    this._pagesRoutes = data;
    this.setupEnabledRoutes();

    data.forEach((item) => {
      this._pagesRoutePaths.push(item.route);
    });
  }

  public getCurrentRoutes(): RouteProps[] {
    if (!this._pagesRoutes) {
      throw new Error("Please register routes first");
    }

    return this._pagesRoutes;
  }

  public getSequenceNo(routeName: string): number {
    const route = this._enabledPagesRoutes.find((r) => r.route === routeName);
    return route ? route.sequenceNo : 0;
  }

  public setupEnabledRoutes(): void {
    let sequence = 0;

    this._enabledPagesRoutes.splice(0);

    for (const firmPagesRoute of this._pagesRoutes) {
      if (firmPagesRoute.isDisabled) {
        continue;
      }

      firmPagesRoute.sequenceNo = sequence;
      this._enabledPagesRoutes.push(firmPagesRoute);
      sequence++;
    }
  }
}