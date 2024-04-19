import { singleton } from "tsyringe";
import { IFinancialPromotionService } from "./IFinancialPromotionService";
import { FinancialPromotion } from "@/entities/financial-promotion/FinancialPromotion";
import { FinancialPromotionCollection } from "@/entities/financial-promotion/FinancialPromotionCollection";

@singleton()
export default class FinancialPromotionMockService
  implements IFinancialPromotionService
{
  private _financialPromotions: FinancialPromotion[] = [];

  public getFinancialPromotionAsync(
    id: string
  ): Promise<FinancialPromotion | undefined> {
    return Promise.resolve(
      this._financialPromotions.find((item) => item.id === id)
    );
  }

  getFinancialPromotionsAsync(
    _customerId?: string | undefined,
    _params?: Record<string, unknown>
  ): Promise<FinancialPromotionCollection> {
    return Promise.resolve({
      data: this._financialPromotions,
      total: this._financialPromotions.length,
    });
  }

  saveOrUpdateFinancialPromotionAsync(
    _financialPromotion: Partial<FinancialPromotion>
  ): Promise<FinancialPromotion> {
    throw new Error("Method not implemented.");
  }

  deleteFinancialPromotionAsync(_id: string): Promise<boolean> {
    throw new Error("Method not implemented.");
  }
}
