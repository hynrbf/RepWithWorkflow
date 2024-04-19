export class FirmBasicInfo {
  firmName?: string;
  companyNumber?: string;
  firmReferenceNumber?: string;
  address?: string = ""; // CH address
  tradingAddress?: string = ""; // FCA address
  headOfficeAddress?: string = "";
  countryCode?: string;
  contactNumber?: string;
  website?: string;
  fcaStatus?: string;
  companyHouseStatus?: string;
  sicCode?: string;
  tradingNames?: string[] = [];
}