import { EmployersLiability } from "@/entities/employers-liability/EmployersLiability";
import { v4 as uuid } from "uuid";

export class EmployersLiabilityModel {
  public id: string = uuid();
  public employerLiability: EmployersLiability = new EmployersLiability();
  public isCollapsed: boolean = false;

  constructor(employerLiability?: EmployersLiability) {
    this.id = uuid();
    this.employerLiability = employerLiability ?? new EmployersLiability();
  }
}