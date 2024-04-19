import { IPagesRouteService } from "@/infra/dependency-services/navigation/IPagesRouteService";

export interface IPageRouteServiceFactory {
  createPageRouteService(onboardingType: string): IPagesRouteService;
}

export const IPageRouteServiceFactoryInfo = {
  name: "IPageRouteServiceFactory",
};
