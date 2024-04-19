<script lang="ts">
import { defineComponent } from "vue";
import "reflect-metadata";
import { container, Lifecycle } from "tsyringe";
import DocumentService from "./infra/dependency-services/rest/document-html/DocumentService";
import SigningService from "./infra/dependency-services/rest/sign-now/SigningService";
import Auth0Service from "./infra/dependency-services/rest/auth0/Auth0Service";
import CustomerService from "./infra/dependency-services/rest/forms-compliance/CustomerService";
import CompaniesHouseService from "./infra/dependency-services/rest/company-house/CompaniesHouseService";
import FcaService from "./infra/dependency-services/rest/fca/FcaService";
import AppService from "./infra/dependency-services/app/AppService";
import { ISigningServiceInfo } from "./infra/dependency-services/rest/sign-now/ISigningService";
import { IFcaServiceInfo } from "./infra/dependency-services/rest/fca/IFcaService";
import { ICustomerServiceInfo } from "./infra/dependency-services/rest/forms-compliance/ICustomerService";
import { IDocumentServiceInfo } from "./infra/dependency-services/rest/document-html/IDocumentService";
import { ICompaniesHouseServiceInfo } from "./infra/dependency-services/rest/company-house/ICompaniesHouseService";
import { IAuth0ServiceInfo } from "./infra/dependency-services/rest/auth0/IAuth0Service";
import { IAppServiceInfo } from "./infra/dependency-services/app/IAppService";
import { ISettingServiceInfo } from "./infra/dependency-services/rest/settings/ISettingService";
import SettingService from "./infra/dependency-services/rest/settings/SettingService";
import { IHelperServiceInfo } from "./infra/dependency-services/helper/IHelperService";
import HelperService from "./infra/dependency-services/helper/HelperService";
import { IProfanityServiceInfo } from "./infra/dependency-services/rest/profanity/IProfanityService";
import ProfanityService from "./infra/dependency-services/rest/profanity/ProfanityService";
import { ICalendlyServiceInfo } from "./infra/dependency-services/rest/calendly/ICalendlyService";
import CalendlyService from "./infra/dependency-services/rest/calendly/CalendlyService";
import { IEncodingServiceInfo } from "./infra/dependency-services/encoding/IEncodingService";
import Base64EncodingService from "./infra/dependency-services/encoding/Base64EncodingService";
import { IGetAddressServiceInfo } from "./infra/dependency-services/rest/get-address/IGetAddressService";
import GetAddressService from "./infra/dependency-services/rest/get-address/GetAddressService";
import { INavigationServiceInfo } from "./infra/dependency-services/navigation/INavigationService";
import NavigationService from "./infra/dependency-services/navigation/NavigationService";
import FirmPagesRouteService from "@/infra/dependency-services/pages-route-firm/FirmPagesRouteService";
import { IFirmPagesRouteServiceInfo } from "@/infra/dependency-services/pages-route-firm/IFirmPagesRouteService";
import MediaPlatformService from "./infra/dependency-services/media-platform/MediaPlatformService";
import { IMediaPlatformServiceInfo } from "./infra/dependency-services/media-platform/IMediaPlatformService";
import MediaMarketingOutletMockService from "./infra/dependency-services/rest/media-marketing-outlet/MediaMarketingOutletMockService";
import { IMediaMarketingOutletServiceInfo } from "./infra/dependency-services/rest/media-marketing-outlet/IMediaMarketingOutletService";
import FinancialPromotionService from "@/infra/dependency-services/rest/financial-promotion/FinancialPromotionService";
import { IFinancialPromotionServiceInfo } from "./infra/dependency-services/rest/financial-promotion/IFinancialPromotionService";
import CommentMockService from "@/infra/dependency-services/rest/comment/CommentMockService";
import { ICommentServiceInfo } from "./infra/dependency-services/rest/comment/ICommentService";
import FormValidatorService from "./infra/dependency-services/form-validator/FormValidatorService";
import { IFormValidatorServiceInfo } from "./infra/dependency-services/form-validator/IFormValidatorService";
import MediaFileService from "./infra/dependency-services/rest/media-file/MediaFileService";
import { IMediaFileServiceInfo } from "./infra/dependency-services/rest/media-file/IMediaFileService";
import { IProviderPagesRouteServiceInfo } from "@/infra/dependency-services/pages-route-providers/IProviderPagesRouteService";
import ProviderPagesRouteService from "@/infra/dependency-services/pages-route-providers/ProviderPagesRouteService";
import { IIntroducerPagesRouteServiceInfo } from "@/infra/dependency-services/pages-route-introducers/IIntroducerPagesRouteService";
import IntroducerPagesRouteService from "@/infra/dependency-services/pages-route-introducers/IntroducerPagesRouteService";
import StationeryMockService from "./infra/dependency-services/rest/stationery/StationeryMockService";
import { IStationeryServiceInfo } from "./infra/dependency-services/rest/stationery/IStationeryService";
import DocumentFormattingMockService from "./infra/dependency-services/rest/stationery/DocumentFormattingMockService";
import { IDocumentFormattingServiceInfo } from "./infra/dependency-services/rest/stationery/IDocumentFormattingService";
import ClientMoneyService from "./infra/dependency-services/rest/client-money/ClientMoneyService";
import { IClientMoneyServiceInfo } from "./infra/dependency-services/rest/client-money/IClientMoneyService";
import WebScrapeService from "./infra/dependency-services/rest/web-scrape/WebScrapeService";
import { IWebScrapeServiceInfo } from "./infra/dependency-services/rest/web-scrape/IWebScrapeService";
import { IOrganizationalStructureServiceInfo } from "@/infra/dependency-services/rest/organizational-structure/IOrganizationalStructureService";
import { ISequenceNoKeeperServiceInfo } from "@/infra/dependency-services/sequence-no/ISequenceNoKeeperService";
import SequenceNoKeeperService from "@/infra/dependency-services/sequence-no/SequenceNoKeeperService";
import { IUserSubmittedChangesServiceInfo } from "@/infra/dependency-services/user-submission/IUserSubmittedChangesService";
import UserSubmittedChangesService from "@/infra/dependency-services/user-submission/UserSubmittedChangesService";
import AppointedRepresentativeService from "@/infra/dependency-services/rest/appointed-representative/AppointedRepresentativeService";
import OrganizationalStructureService from "@/infra/dependency-services/rest/organizational-structure/OrganizationalStructureService";
import { MapperService } from "@/infra/dependency-services/mapper/MapperService";
import { IMapperServiceInfo } from "@/infra/dependency-services/mapper/IMapperService";
import { IEmployeePagesRouteServiceInfo } from "@/infra/dependency-services/pages-route-employee/IEmployeePagesRouteService";
import EmployeePagesRouteService from "@/infra/dependency-services/pages-route-employee/EmployeePagesRouteService";
import { IAppointedRepresentativeServiceInfo } from "@/infra/dependency-services/rest/appointed-representative/IAppointedRepresentativeService";
import AppointedRepresentativePagesRouteService from "@/infra/dependency-services/pages-route-ar/AppointedRepresentativePagesRouteService";
import { IAppointedRepresentativePagesRouteServiceInfo } from "@/infra/dependency-services/pages-route-ar/IAppointedRepresentativePagesRouteService";
import {
  IPageRouteServiceFactoryInfo
} from "@/infra/dependency-services/pages-route-service-factory/IPageRouteServiceFactory";
import {PageRouteServiceFactory} from "@/infra/dependency-services/pages-route-service-factory/PageRouteServiceFactory";
import ProvidersService from "@/infra/dependency-services/rest/providers/ProvidersService";
import {IProvidersServiceInfo} from "@/infra/dependency-services/rest/providers/IProvidersService";
import IntroducersService from "@/infra/dependency-services/rest/introducers/IntroducersService";
import {IIntroducersServiceInfo} from "@/infra/dependency-services/rest/introducers/IIntroducersService";

//registrations
container.register(
  IDocumentServiceInfo.name,
  {
    useClass: DocumentService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
  ISigningServiceInfo.name,
  {
    useClass: SigningService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
  IAuth0ServiceInfo.name,
  {
    useClass: Auth0Service,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
  ICustomerServiceInfo.name,
  {
    useClass: CustomerService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
  ICompaniesHouseServiceInfo.name,
  {
    useClass: CompaniesHouseService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
  IFcaServiceInfo.name,
  {
    useClass: FcaService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
  IAppServiceInfo.name,
  {
    useClass: AppService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
  ISettingServiceInfo.name,
  {
    useClass: SettingService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
  IHelperServiceInfo.name,
  {
    useClass: HelperService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);
container.register(
  IMapperServiceInfo.name,
  {
    useClass: MapperService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
  IProfanityServiceInfo.name,
  {
    useClass: ProfanityService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
  ICalendlyServiceInfo.name,
  {
    useClass: CalendlyService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
  IEncodingServiceInfo.name,
  {
    useClass: Base64EncodingService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
  IGetAddressServiceInfo.name,
  {
    useClass: GetAddressService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

//ToDo. to think if this is singleton?
container.register(
  INavigationServiceInfo.name,
  {
    useClass: NavigationService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

//ToDo. to think if this is singleton?
container.register(
  IFirmPagesRouteServiceInfo.name,
  {
    useClass: FirmPagesRouteService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
  IProviderPagesRouteServiceInfo.name,
  {
    useClass: ProviderPagesRouteService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
    IIntroducerPagesRouteServiceInfo.name,
    {
      useClass: IntroducerPagesRouteService,
    },
    {
      lifecycle: Lifecycle.Singleton,
    },
);

container.register(
  IAppointedRepresentativePagesRouteServiceInfo.name,
  {
    useClass: AppointedRepresentativePagesRouteService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
  IEmployeePagesRouteServiceInfo.name,
  {
    useClass: EmployeePagesRouteService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
  IMediaPlatformServiceInfo.name,
  {
    useClass: MediaPlatformService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
  ISequenceNoKeeperServiceInfo.name,
  {
    useClass: SequenceNoKeeperService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
  IMediaMarketingOutletServiceInfo.name,
  {
    useClass: MediaMarketingOutletMockService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
  IFinancialPromotionServiceInfo.name,
  {
    useClass: FinancialPromotionService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
  ICommentServiceInfo.name,
  {
    useClass: CommentMockService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(IFormValidatorServiceInfo.name, {
  useClass: FormValidatorService,
});

container.register(
  IMediaFileServiceInfo.name,
  {
    useClass: MediaFileService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
  IOrganizationalStructureServiceInfo.name,
  {
    useClass: OrganizationalStructureService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
    IProvidersServiceInfo.name,
    {
      useClass: ProvidersService,
    },
    {
      lifecycle: Lifecycle.Singleton,
    },
);

container.register(
    IIntroducersServiceInfo.name,
    {
      useClass: IntroducersService,
    },
    {
      lifecycle: Lifecycle.Singleton,
    },
);

container.register(
  IStationeryServiceInfo.name,
  {
    useClass: StationeryMockService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
  IDocumentFormattingServiceInfo.name,
  {
    useClass: DocumentFormattingMockService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
  IClientMoneyServiceInfo.name,
  {
    useClass: ClientMoneyService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
  IWebScrapeServiceInfo.name,
  {
    useClass: WebScrapeService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

//To think if this is singleton? this might affect other user state, when other user is also doing
//but, I might be wrong since this is static web app
container.register(
  IUserSubmittedChangesServiceInfo.name,
  {
    useClass: UserSubmittedChangesService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
  IAppointedRepresentativeServiceInfo.name,
  {
    useClass: AppointedRepresentativeService,
  },
  {
    lifecycle: Lifecycle.Singleton,
  },
);

container.register(
    IPageRouteServiceFactoryInfo.name,
    {
      useClass: PageRouteServiceFactory,
    },
    {
      lifecycle: Lifecycle.Singleton,
    },
);

//main app
export default defineComponent({
  name: "App",
});
</script>

<template src="./app.html" />

<style lang="scss" src="./App.scss" />