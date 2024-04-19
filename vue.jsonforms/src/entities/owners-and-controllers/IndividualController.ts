import { IndividualControllerDetails } from "./IndividualControllerDetails";
import { FinancialStatus } from "./FinancialStatus";
import { CompanyOfficerAppointmentDetails } from "./CompanyOfficerAppointmentDetails";
import { Controller } from "@/entities/firm-details/Controller";
import { v4 as uuid } from "uuid";

export class IndividualController {
  public id: string = uuid();
  public detail: IndividualControllerDetails =
    new IndividualControllerDetails();
  public directorsAndDirectorship: CompanyOfficerAppointmentDetails[] =
    [] as CompanyOfficerAppointmentDetails[];
  public controllingInterests: Controller[] = [] as Controller[];
  public financialStatus: FinancialStatus = new FinancialStatus();
  public curriculumVitaeUrls: string[] = [];
}