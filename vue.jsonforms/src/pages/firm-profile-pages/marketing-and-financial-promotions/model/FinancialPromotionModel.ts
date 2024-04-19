import { FinancialPromotion } from "@/entities/financial-promotion/FinancialPromotion";

export class FinancialPromotionModel extends FinancialPromotion {
    public approved?: boolean = false;
    public rejected?: boolean = false;
    public pending?: boolean = false;
}