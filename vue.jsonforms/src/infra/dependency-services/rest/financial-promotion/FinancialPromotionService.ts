import { singleton } from "tsyringe";
import { IFinancialPromotionService } from "./IFinancialPromotionService";
import { FinancialPromotion } from "@/entities/financial-promotion/FinancialPromotion";
import RestBase from "@/infra/dependency-services/rest/RestBase";
import { REMOTE_API } from "@/config";
import { FinancialPromotionCollection } from "@/entities/financial-promotion/FinancialPromotionCollection";
import isEmpty from "lodash/isEmpty";

@singleton()
export default class FinancialPromotionService
  extends RestBase
  implements IFinancialPromotionService
{
  public async getFinancialPromotionAsync(
    id: string,
  ): Promise<FinancialPromotion | undefined> {
    try {
      return await this.getRemoteAsync<
        FinancialPromotion | undefined
      >(`${REMOTE_API}/GetFinancialPromotionAsync/${id}`);
    } catch (error) {
      return Promise.reject(error);
    }
  }

  public async saveOrUpdateFinancialPromotionAsync(
    financialPromotion: Partial<FinancialPromotion>,
  ): Promise<FinancialPromotion> {
    try {
      return await this.postRemoteAsync<FinancialPromotion>(
        `${REMOTE_API}/SaveOrUpdateFinancialPromotionAsync`,
        JSON.stringify(financialPromotion),
      );
    } catch (error) {
      return Promise.reject(error);
    }
  }

  public async getFinancialPromotionsAsync(
    customerId?: string,
    params?: Record<string, unknown>,
  ): Promise<FinancialPromotionCollection> {
    try {
      let response: FinancialPromotionCollection | null = null;
      if (isEmpty(params)) {
        response = await this.getRemoteAsync<FinancialPromotionCollection>(
          `${REMOTE_API}/GetFinancialPromotionsAsync/${customerId}`,
        );
      } else {
        response = await this.postRemoteAsync<FinancialPromotionCollection>(
          `${REMOTE_API}/GetFinancialPromotionsAsync/${customerId}`,
          params,
        );
      }
      if (!response) {
        throw new Error("Something went wrong.");
      }
      return response;
    } catch (error) {
      return Promise.reject(error);
    }
  }

  public async deleteFinancialPromotionAsync(id: string): Promise<boolean> {
    try {
      return await this.deleteRemoteAsync<boolean>(
        `${REMOTE_API}/DeleteFinancialPromotionAsync/${id}`,
      );
    } catch (error) {
      return Promise.reject(error);
    }
  }
}
