import {IntroducersEntity} from "@/entities/providers-and-introducers/IntroducersEntity";

export declare interface IIntroducersService {
  getIntroducerByEmailAsync(
    email: string,
  ): Promise<IntroducersEntity>;

  getIntroducersByCustomerIdAsync(
      customerId: string,
  ): Promise<IntroducersEntity[]>;

  saveOrUpdateIntroducersAsync(
    financialPromotion: Partial<IntroducersEntity>,
  ): Promise<IntroducersEntity>;

  deleteIntroducerAsync(id: string): Promise<boolean>;
}

export const IIntroducersServiceInfo = {
  name: "IIntroducersService",
};
