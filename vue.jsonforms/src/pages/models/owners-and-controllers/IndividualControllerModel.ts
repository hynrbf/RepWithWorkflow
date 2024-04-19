import { IndividualControllerDetails } from "@/entities/owners-and-controllers/IndividualControllerDetails";
import { CompanyOfficerAppointmentDetails } from "@/entities/owners-and-controllers/CompanyOfficerAppointmentDetails";
import { Controller } from "@/entities/firm-details/Controller";
import { FinancialStatus } from "@/entities/owners-and-controllers/FinancialStatus";
import { v4 as uuid } from "uuid";

export class IndividualControllerModel {
  public id: string = uuid();
  public detail: IndividualControllerDetails =
    new IndividualControllerDetails();
  public directorsAndDirectorship: CompanyOfficerAppointmentDetails =
    new CompanyOfficerAppointmentDetails();
  public controllingInterests: Controller[] = [] as Controller[];
  public financialStatus: FinancialStatus = new FinancialStatus();
  public curriculumVitaeUrls: string[] = [];

  getFullNameOrSetDefault(defaultValue: string): string {
    let fullName = defaultValue;

    if (this.detail.forename) {
      fullName = this.detail.forename;
    }

    if (this.detail.surname) {
      fullName += ` ${this.detail.surname}`;
    }

    return fullName;
  }
}
