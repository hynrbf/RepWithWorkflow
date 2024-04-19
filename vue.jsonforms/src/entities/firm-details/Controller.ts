import { Address } from "@/entities/firm-details/Address";
import { Identification } from "@/entities/firm-details/Identification";
import { NameElement } from "@/entities/firm-details/NameElement";
import { CompanyDetailsBase } from "@/entities/firm-details/CompanyDetailsBase";
import { Director } from "@/entities/owners-and-controllers/Director";
import { CompanyOfficerAppointmentDetails } from "@/entities/owners-and-controllers/CompanyOfficerAppointmentDetails";
import { v4 as uuid } from "uuid";
import { IShareholder } from "@/entities/firm-details/IShareholder";

export class Controller extends CompanyDetailsBase implements IShareholder {
  public id: string = uuid();
  public address: Address | undefined;
  public fullAddress: string | undefined;
  public description: string | undefined;
  public identification: Identification | undefined;
  public kind: string | undefined;
  public name: string | undefined;
  public nationality: string | undefined;
  public dateOfBirth: number = 0;
  public ceased: boolean = false;
  public dateCeased: number | undefined;
  public country_of_residence: string | undefined;
  public etag: string | undefined;
  public is_sanctioned: boolean = false;
  public name_elements: NameElement | undefined;
  public natures_of_control: string[] = [];
  public dateNotified: number = 0;
  public principal_office_address: Address | undefined;
  public percentageOfCapital: number | null = null;
  public percentageOfVotingRights: number | null = null;
  public individualControllers: Controller[] = [];
  public corporateControllers: Controller[] = [];
  public directors: Director[] = [] as Director[];
  public directorships: CompanyOfficerAppointmentDetails =
    new CompanyOfficerAppointmentDetails();
  public controllingInterests: Controller[] = [];
}