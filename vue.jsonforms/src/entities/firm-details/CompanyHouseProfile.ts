export type CompanyHouseProfileAccounts = {
  accounting_reference_date?: {
    day: string;
    month: string;
  };
  last_accounts?: {
    made_up_to: string | null;
    period_start_on: string | null;
    period_end_on: string | null;
  };
};

export class CompanyHouseProfile {
  public jurisdiction: string | undefined;
  public company_name: string | undefined;
  public company_number: string | undefined;
  public company_status: string | undefined;
  public date_of_creation: string | undefined;
  public type: string | undefined;
  public registered_office_address: RegisteredOfficeAddress =
    new RegisteredOfficeAddress();
  public sic_codes: string[] = [];
  public accounts: CompanyHouseProfileAccounts = {};
}

export class RegisteredOfficeAddress {
  public address_line_1: string | undefined;
  public address_line_2: string | undefined;
  public address_line_3: string | undefined;
  public address_line_4: string | undefined;
  public locality: string | undefined;
  public premises: string | undefined;
  public postal_code: string | undefined;
  public care_of_name: string | undefined;
  public care_of: string | undefined;
  public country: string | undefined;
  public region: string | undefined;
  public po_box: string | undefined;
}
