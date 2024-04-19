import { CompanyOfficerAppointment } from "./CompanyOfficerAppointment";

export class CompanyOfficerAppointmentDetails {
  public dateOfBirth: number = 0;
  public fullName: string | undefined;
  public nationalities: string[] = [];
  public items: CompanyOfficerAppointment[] = [] as CompanyOfficerAppointment[];
}