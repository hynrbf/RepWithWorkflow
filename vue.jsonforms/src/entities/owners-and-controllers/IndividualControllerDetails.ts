import AddressInfo from "@/pages/models/owners-and-controllers/AddressInfo";
import { ContactNumber } from "../ContactNumber";
import {IShareholder} from "@/entities/firm-details/IShareholder";

export class IndividualControllerDetails implements IShareholder {
  public title: string | undefined;
  public forename: string | undefined;
  public surname: string | undefined;
  public previousFullName: string | undefined;
  public reasonForChangeName: string | undefined;
  public dateOfNameChange: number = 0;
  public commonlyUsedName: string | undefined;
  public contactNumber: ContactNumber = new ContactNumber();
  public emailAddress: string | undefined;
  public homeAddress: string | undefined;
  public isHomeAddressChanged: boolean = false;
  public homeAddressResidenceDate: number | undefined;
  public previousAddresses: AddressInfo[] = [];
  public dateOfBirth: number = 0;
  public countryOfBirth: string | undefined;
  public nationalities: string[] = [];
  public previousNationalities: string[] = [];
  public nationalInsuranceNumber: string | undefined;
  public passportNumber: string | undefined;
  public percentageOfCapital: number | null = null;
  public percentageOfVotingRights: number | null = null;
  public hasBeenSubjectToAnyMaterialComplaints?: boolean = undefined;
  public additionalInformation: string | undefined;
  public supportingDocumentsUrls: string[] = [];
  public originalHomeAddress: string | undefined;
}
