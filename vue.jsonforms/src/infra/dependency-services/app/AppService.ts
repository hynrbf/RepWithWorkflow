import { container, singleton } from "tsyringe";
import { AuthUser } from "@/entities/AuthUser";
import { AppConstants } from "../../AppConstants";
import {
  ICustomerService,
  ICustomerServiceInfo,
} from "../rest/forms-compliance/ICustomerService";
import { IAppService } from "./IAppService";
import { CustomerEntity } from "@/entities/CustomerEntity";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import {
  ISettingService,
  ISettingServiceInfo,
} from "@/infra/dependency-services/rest/settings/ISettingService";
import {
  IAppointedRepresentativeService,
  IAppointedRepresentativeServiceInfo,
} from "@/infra/dependency-services/rest/appointed-representative/IAppointedRepresentativeService";
import { AppointedRepresentative } from "@/entities/appointed-representatives/AppointedRepresentative";
import {
  IOrganizationalStructureService,
  IOrganizationalStructureServiceInfo,
} from "@/infra/dependency-services/rest/organizational-structure/IOrganizationalStructureService";
import { Employee } from "@/entities/firm-details/Employee";
import { ProvidersEntity } from "@/entities/providers-and-introducers/ProvidersEntity";
import {
  IProvidersService,
  IProvidersServiceInfo,
} from "@/infra/dependency-services/rest/providers/IProvidersService";
import { IntroducersEntity } from "@/entities/providers-and-introducers/IntroducersEntity";
import {
  IIntroducersService,
  IIntroducersServiceInfo,
} from "@/infra/dependency-services/rest/introducers/IIntroducersService";

@singleton()
export default class AppService implements IAppService {
  private readonly _customerService: ICustomerService;
  private readonly _organisationalStructureService: IOrganizationalStructureService;
  private readonly _providerService: IProvidersService;
  private readonly _introducerService: IIntroducersService;
  private readonly _customerAppointedRepresentativeService: IAppointedRepresentativeService;
  private readonly _helperService: IHelperService;
  private readonly _settingsService: ISettingService;

  constructor() {
    this._customerService = container.resolve<ICustomerService>(
      ICustomerServiceInfo.name,
    );

    this._customerAppointedRepresentativeService =
      container.resolve<IAppointedRepresentativeService>(
        IAppointedRepresentativeServiceInfo.name,
      );

    this._organisationalStructureService =
      container.resolve<IOrganizationalStructureService>(
        IOrganizationalStructureServiceInfo.name,
      );

    this._providerService = container.resolve<IProvidersService>(
      IProvidersServiceInfo.name,
    );

    this._introducerService = container.resolve<IIntroducersService>(
      IIntroducersServiceInfo.name,
    );

    this._helperService = container.resolve<IHelperService>(
      IHelperServiceInfo.name,
    );
    this._settingsService = container.resolve<ISettingService>(
      ISettingServiceInfo.name,
    );
  }

  public async checkCustomerSignedUpAlreadyAsync(): Promise<boolean> {
    const authUser = this.getAuthUserFromLocal();

    if (!authUser) {
      return false;
    }

    const registeredCustomer =
      await this._customerService.getCustomerByEmailAsync(authUser.email);

    if (registeredCustomer) {
      const customers: CustomerEntity[] = [];
      customers.push(registeredCustomer);

      localStorage.setItem(
        AppConstants.jsonFormsCustomersKey,
        JSON.stringify(customers),
      );
    }

    return !!registeredCustomer;
  }

  async getComplianceFirmNameAsync(): Promise<string> {
    const cachedName = localStorage.getItem(
      AppConstants.currentComplianceNameKey,
    );

    if (cachedName) {
      return cachedName;
    }

    const settings = await this._settingsService.getAllSettingsAsync();
    const settingConsultant = settings.find(
      (s) => s.key == "$(CONSULTANCY_NAME)",
    );

    if (!settingConsultant?.value) {
      throw new Error("Please set in the settings CONSULTANCY_NAME");
    }

    localStorage.setItem(
      AppConstants.currentComplianceNameKey,
      settingConsultant.value,
    );
    return settingConsultant.value;
  }

  public saveAuthUserToLocal(authUser: AuthUser): void {
    localStorage.setItem(AppConstants.authUserKey, JSON.stringify(authUser));
  }

  public clearAllLocalCache(): void {
    localStorage.removeItem(AppConstants.jsonFormsSchemaKey);
    localStorage.removeItem(AppConstants.jsonFormsUiSchemaKey);
    localStorage.removeItem(AppConstants.jsonFormsAnswersKey);
    localStorage.removeItem(AppConstants.fcaFirmsCacheKey);
    localStorage.removeItem(AppConstants.fcaSoleTraderFirmsCacheKey);
    localStorage.removeItem(AppConstants.jsonFormsCustomersKey);
    localStorage.removeItem(AppConstants.addressManuallyAddedKey);
    localStorage.removeItem(AppConstants.DateOfBirth);
    localStorage.removeItem(AppConstants.GetAddressIoSelectedKey);
    localStorage.removeItem(AppConstants.corporateShareholdersCount);
    localStorage.removeItem(AppConstants.companyDirectorshipsKey);
    localStorage.removeItem(AppConstants.companyControllingInterestsKey);
    localStorage.removeItem(AppConstants.companyIndividualControllersKey);
    localStorage.removeItem(AppConstants.companyCorporateControllersKey);
    localStorage.removeItem(AppConstants.contextMenuSelectionProviderKey);
    localStorage.removeItem(AppConstants.contextMenuSelectionIntroducerKey);
    localStorage.removeItem(AppConstants.SelectedFirm);
    localStorage.removeItem(AppConstants.signingTypeKey);
    localStorage.removeItem(AppConstants.profanityKey);
    localStorage.removeItem(AppConstants.profileKey);
    localStorage.removeItem(AppConstants.emailKey);
    localStorage.removeItem(AppConstants.currentComplianceNameKey);
    localStorage.removeItem(AppConstants.hasLogOnAlreadyKey);
    localStorage.removeItem(AppConstants.navigationStackKey);
    localStorage.removeItem(AppConstants.saveOrNextComponentSequenceKey);
    localStorage.removeItem(AppConstants.pageComponentValidationValueStore);
    localStorage.removeItem(AppConstants.onboardingCompletionPercentageStore);
    localStorage.removeItem(AppConstants.commentStore);
    localStorage.removeItem(AppConstants.fcaDefinedStatusesKey);
    localStorage.removeItem(AppConstants.pageFieldsInvalidHandlerStore);
    localStorage.removeItem(AppConstants.organizationalStructureStore);
    localStorage.removeItem(AppConstants.safeCustomerStore);
    localStorage.removeItem(AppConstants.companyHouseDefinedStatusesKey);
    localStorage.removeItem(AppConstants.customerProductsKey);
    localStorage.removeItem(AppConstants.onboardingCompletedKey);
    localStorage.removeItem(AppConstants.onboardingTypeKey);
    localStorage.removeItem(AppConstants.customerAppointedRepresentativeKey);
    localStorage.removeItem(AppConstants.customerProviderStore);
    localStorage.removeItem(AppConstants.customerIntroducerStore);
  }

  public clearThankYouPageCache(): void {
    localStorage.removeItem(AppConstants.navigateBackCustomerKey);
    localStorage.removeItem(AppConstants.navigateBackFirmKey);
    localStorage.removeItem(AppConstants.navigateBackToThankYouPageKey);
  }

  public clearAllLocalCacheWithToken(): void {
    this.clearAllLocalCache();
    this.clearThankYouPageCache();
    localStorage.removeItem(AppConstants.authUserKey);
    localStorage.removeItem(AppConstants.authTokenCacheKey);
  }

  public getAuthUserFromLocal(): AuthUser | undefined {
    const result = localStorage.getItem(AppConstants.authUserKey);

    if (!result) {
      return undefined;
    }

    return JSON.parse(result) as AuthUser;
  }

  public getCachedCustomer(): CustomerEntity | undefined {
    const customerString = localStorage.getItem(
      AppConstants.jsonFormsCustomersKey,
    );

    if (!customerString) {
      throw new Error("The customer should be in cache already");
    }

    const customers = JSON.parse(customerString) as CustomerEntity[];

    if (customers?.length > 0) {
      return customers[0];
    }

    return undefined;
  }

  public getCustomerFirmName(): string {
    const customer = this.getCachedCustomer();

    if (!customer) {
      throw new Error(
        "The customer cached should be existing already in the app service",
      );
    }

    if (customer.isCompanyNotApplicable) {
      const firstName = customer.firstName;
      const lastName = customer.lastName;
      return `${firstName} ${lastName}`;
    }

    const companyName = customer?.companyName ?? "";

    if (!companyName) {
      return companyName;
    }

    return this._helperService.removePostCodeString(companyName);
  }

  public async checkCustomerAppointedRepresentativeSignedUpAlreadyAsync(): Promise<boolean> {
    const authUser = this.getAuthUserFromLocal();

    if (!authUser) {
      return false;
    }

    const registeredCustomerAr =
      await this._customerAppointedRepresentativeService.getAppointedRepresentativesByEmailAsync(
        authUser.email,
      );

    if (!registeredCustomerAr) {
      return false;
    }

    //ToDo. remove this soon to get to api
    registeredCustomerAr.mediaMarketingOutlets = [
      {
        id: "d3655790-0c3f-492d-bc49-80d6518914e7",
        url: "https://www.axa-art.co.uk",
        name: "Default Website",
        owner: undefined,
        archived: false,
        platform: "website",
        createdAt: 1710995926,
      },
    ];

    const registeredCustomerArs = [
      registeredCustomerAr,
    ] as AppointedRepresentative[];

    localStorage.setItem(
      AppConstants.customerAppointedRepresentativeKey,
      JSON.stringify(registeredCustomerArs),
    );

    return true;
  }

  public async checkEmployeeSignedUpAlreadyAsync(): Promise<boolean> {
    const authUser = this.getAuthUserFromLocal();

    if (!authUser) {
      return false;
    }

    const registeredEmployee =
      await this._organisationalStructureService.getEmployeeByEmailAsync(
        authUser.email,
      );

    if (!registeredEmployee) {
      return false;
    }

    const registeredCustomerEmployees = [registeredEmployee] as Employee[];

    localStorage.setItem(
      AppConstants.customerEmployeesKey,
      JSON.stringify(registeredCustomerEmployees),
    );

    return true;
  }

  public async checkProviderSignedUpAlreadyAsync(): Promise<boolean> {
    const authUser = this.getAuthUserFromLocal();

    if (!authUser) {
      return false;
    }

    const registeredProvider =
      await this._providerService.getProviderByEmailAsync(authUser.email);

    if (!registeredProvider) {
      return false;
    }

    const registeredCustomerProvider: ProvidersEntity[] = [registeredProvider];

    localStorage.setItem(
      AppConstants.customerProvidersKey,
      JSON.stringify(registeredCustomerProvider),
    );

    return true;
  }

  public async checkIntroducerSignedUpAlreadyAsync(): Promise<boolean> {
    const authUser = this.getAuthUserFromLocal();

    if (!authUser) {
      return false;
    }

    const registeredIntroducer =
      await this._introducerService.getIntroducerByEmailAsync(authUser.email);

    if (!registeredIntroducer) {
      return false;
    }

    const registeredCustomerIntroducer: IntroducersEntity[] = [
      registeredIntroducer,
    ];

    localStorage.setItem(
      AppConstants.customerIntroducersKey,
      JSON.stringify(registeredCustomerIntroducer),
    );

    return true;
  }

  public getCachedCustomerAppointedRepresentative():
    | AppointedRepresentative
    | undefined {
    const customerArString = localStorage.getItem(
      AppConstants.customerAppointedRepresentativeKey,
    );

    if (!customerArString) {
      throw new Error(
        "The customer appointed Representative should be in cache already",
      );
    }

    const customersArs = JSON.parse(
      customerArString,
    ) as AppointedRepresentative[];

    if (customersArs?.length) {
      return customersArs[0];
    }

    return undefined;
  }

  public getCachedEmployee(): Employee | undefined {
    const customerString = localStorage.getItem(
      AppConstants.customerEmployeesKey,
    );

    if (!customerString) {
      throw new Error("The employee should be in cache already");
    }

    const customers = JSON.parse(customerString) as Employee[];

    if (customers?.length > 0) {
      return customers[0];
    }

    return undefined;
  }

  getCustomerArFirmName(): string {
    const customerAr = this.getCachedCustomerAppointedRepresentative();

    if (!customerAr) {
      throw new Error(
        "The customer appointed representative cached should be existing already in the app service",
      );
    }

    if (customerAr.isCompanyNotApplicable) {
      const firstName = customerAr.firstName;
      const lastName = customerAr.lastName;
      return `${firstName} ${lastName}`;
    }

    const companyName = customerAr?.companyName ?? "";

    if (!companyName) {
      return companyName;
    }

    return this._helperService.removePostCodeString(companyName);
  }

  public getEmployeeFirmName(): string {
    const employee = this.getCachedEmployee();

    if (!employee) {
      throw new Error(
        "The customer cached should be existing already in the app service",
      );
    }

    if (employee.isCompanyNotApplicable) {
      const firstName = employee.firstName;
      const lastName = employee.lastName;
      return `${firstName} ${lastName}`;
    }

    const companyName = employee?.companyName ?? "";

    if (!companyName) {
      return companyName;
    }

    return this._helperService.removePostCodeString(companyName);
  }
}
