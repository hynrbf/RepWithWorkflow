export class Address {
  public address_line_1: string | undefined;
  public address_line_2: string | undefined;
  public locality: string | undefined;
  public premises: string | undefined;
  public postal_code: string | undefined;
  public care_of_name: string | undefined;
  public care_of: string | undefined;
  public country: string | undefined;
  public region?: string;
  public po_box: string | undefined;
}
