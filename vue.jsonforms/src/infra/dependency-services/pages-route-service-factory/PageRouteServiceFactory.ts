import { IPageRouteServiceFactory } from "@/infra/dependency-services/pages-route-service-factory/IPageRouteServiceFactory";
import { IPagesRouteService } from "../navigation/IPagesRouteService";
import { OnboardingType } from "@/infra/base";
import {
  IFirmPagesRouteService,
  IFirmPagesRouteServiceInfo,
} from "@/infra/dependency-services/pages-route-firm/IFirmPagesRouteService";
import { container } from "tsyringe";
import {
  IAppointedRepresentativePagesRouteService,
  IAppointedRepresentativePagesRouteServiceInfo,
} from "@/infra/dependency-services/pages-route-ar/IAppointedRepresentativePagesRouteService";
import {
  IEmployeePagesRouteService,
  IEmployeePagesRouteServiceInfo,
} from "@/infra/dependency-services/pages-route-employee/IEmployeePagesRouteService";
import {
  IProviderPagesRouteService,
  IProviderPagesRouteServiceInfo,
} from "@/infra/dependency-services/pages-route-providers/IProviderPagesRouteService";
import {
  IIntroducerPagesRouteService,
  IIntroducerPagesRouteServiceInfo,
} from "@/infra/dependency-services/pages-route-introducers/IIntroducerPagesRouteService";

export class PageRouteServiceFactory implements IPageRouteServiceFactory {
  createPageRouteService(onboardingType: string): IPagesRouteService {
    if (onboardingType === OnboardingType.Ar.toString()) {
      return container.resolve<IAppointedRepresentativePagesRouteService>(
        IAppointedRepresentativePagesRouteServiceInfo.name,
      );
    }

    if (onboardingType === OnboardingType.Employee.toString()) {
      return container.resolve<IEmployeePagesRouteService>(
        IEmployeePagesRouteServiceInfo.name,
      );
    }

    if (onboardingType === OnboardingType.Provider.toString()) {
      return container.resolve<IProviderPagesRouteService>(
        IProviderPagesRouteServiceInfo.name,
      );
    }

    if (onboardingType === OnboardingType.Introducer.toString()) {
      return container.resolve<IIntroducerPagesRouteService>(
        IIntroducerPagesRouteServiceInfo.name,
      );
    }

    return container.resolve<IFirmPagesRouteService>(
      IFirmPagesRouteServiceInfo.name,
    );
  }
}
