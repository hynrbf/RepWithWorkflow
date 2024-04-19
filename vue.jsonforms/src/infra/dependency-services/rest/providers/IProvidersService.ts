import {ProvidersEntity} from "@/entities/providers-and-introducers/ProvidersEntity";

export declare interface IProvidersService {
  getProviderByEmailAsync(
    email: string,
  ): Promise<ProvidersEntity>;

  getProvidersByCustomerIdAsync(
      customerId: string,
  ): Promise<ProvidersEntity[]>;

  saveOrUpdateProvidersAsync(
    financialPromotion: Partial<ProvidersEntity>,
  ): Promise<ProvidersEntity>;

  deleteProviderAsync(id: string): Promise<boolean>;
}

export const IProvidersServiceInfo = {
  name: "IProvidersService",
};
