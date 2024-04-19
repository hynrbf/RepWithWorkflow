import { Money } from "@/entities/Money";

export interface FeeEntity {
  type: string;
  amount: Money;
}
