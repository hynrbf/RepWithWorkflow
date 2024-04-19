import { DisclosureText } from "./IDisclosureText";
export class DisclosureEntity {
  timePeriodDisclosureText: DisclosureText[] = [];
  affiliateDisclosureText: DisclosureText[] = [];
  taxDisclosureText: DisclosureText[] = [];
  mortgageDisclosureText: DisclosureText[] = [];
  investmentDisclosureText: DisclosureText[] = [];
  cryptoDisclosureText: DisclosureText[] = [];
  timePeriodDisclosureConfirmedText?: DisclosureText[];
  affiliateDisclosureConfirmedText?: DisclosureText[];
  taxDisclosureConfirmedText?: DisclosureText[];
  mortgageDisclosureConfirmedText?: DisclosureText[];
  investmentDisclosureConfirmedText?: DisclosureText[];
  cryptoDisclosureConfirmedText?: DisclosureText[];
}
