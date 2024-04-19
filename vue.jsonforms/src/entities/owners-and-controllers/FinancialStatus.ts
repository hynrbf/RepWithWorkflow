import {Money} from "@/entities/Money";

export class FinancialStatus {
    public annualIncome: string | undefined;
    public sourceOfIncome: string = "";
    public totalAssets: string | undefined;
    public totalLiabilities: string | undefined;
    public totalAmountToActAsGuarantor: Money = new Money();
    public additionalInformation: string = "";
}