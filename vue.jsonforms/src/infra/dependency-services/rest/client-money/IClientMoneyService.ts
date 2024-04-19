import { InsurerRating } from "@/entities/client-money/InsurerRating";

export declare interface IClientMoneyService {
  getInsurerRatingsAsync(company: string): Promise<InsurerRating>;
}

export const IClientMoneyServiceInfo = {
  name: "IClientMoneyService",
};
