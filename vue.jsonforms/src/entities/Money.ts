export class Money {
  public currency: string;
  public symbol: string;
  public amount?: number;

  constructor(
    amount: number | undefined = undefined,
    currency: string = "GBP",
    symbol: string = "£",
  ) {
    this.currency = currency;
    this.symbol = symbol;
    this.amount = amount;
  }
}
