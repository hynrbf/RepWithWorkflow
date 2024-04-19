import {v4} from "uuid";
import {Money} from "@/entities/Money";
export class AppointedRepresentativeActivity {
  public id: string = v4();
  public productId: string = "";
  public annualFeeIncome?: Money;
  public annualCommissionIncome?: Money;
  public limitations?: string;
  public isAppointed: boolean = false;
  public hasLimitation: boolean = false;
  // additional
  public name?: string;
  public projectedAnnualFee?: Money;
  public projectedAnnualCommissionIncome?: Money;
  public yearsOfExperience?: number;
  public hasPendingApplication?: boolean = false;
  public isModified?: boolean = false;
  public isNewProduct?: boolean = false;
}
