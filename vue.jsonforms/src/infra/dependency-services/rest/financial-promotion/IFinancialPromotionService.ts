import { FinancialPromotionCollection } from "@/entities/financial-promotion/FinancialPromotionCollection";
import { FinancialPromotion } from "@/entities/financial-promotion/FinancialPromotion";

export declare interface IFinancialPromotionService {
  getFinancialPromotionAsync(
    id: string
  ): Promise<FinancialPromotion | undefined>;

  getFinancialPromotionsAsync(
    customerId?: string,
    params?: Record<string, unknown>
  ): Promise<FinancialPromotionCollection>;

  saveOrUpdateFinancialPromotionAsync(
    financialPromotion: Partial<FinancialPromotion>
  ): Promise<FinancialPromotion>;

  deleteFinancialPromotionAsync(id: string): Promise<boolean>;
}

export const IFinancialPromotionServiceInfo = {
  name: "IFinancialPromotionService",
};
