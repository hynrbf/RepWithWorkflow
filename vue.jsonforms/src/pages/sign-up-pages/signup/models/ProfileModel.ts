import { CompanyEntity } from "@/entities/CompanyEntity";
import { ContactNumber } from "@/entities/ContactNumber";

export class ProfileModel {
  public foreName: string = "";
  public lastName: string = "";
  public email: string = "";
  public contactNumber: ContactNumber = new ContactNumber();
  public homeAddress: string = "";
  public dateOfBirth: string = "";
  public selectedCompany: CompanyEntity;

  constructor() {
    this.selectedCompany = this.getNewSelectedCompany();
  }

  public initializeSelectedCompany() {
    this.selectedCompany = this.getNewSelectedCompany();
  }

  private getNewSelectedCompany(): CompanyEntity {
    return {
      address: "",
      tradingAddress: "",
      appointedRepresentatives: [],
      companyNumber: "",
      firmReferenceNo: "",
      isAuthorized: false,
      isConfirmedFirmDetails: false,
      isSelected: false,
      isSoleTrader: false,
      isVariedFirmPermissions: false,
      postcode: "",
      region: "",
      status: "",
      companyHouseStatus: "",
      type: "",
      companyName: "",
      website: "",
      contactNumber: "",
      sicCode: "",
      countryCode: "",
    };
  }
}
