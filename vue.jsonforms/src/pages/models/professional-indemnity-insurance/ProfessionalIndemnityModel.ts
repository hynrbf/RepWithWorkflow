import { ProfessionalIndemnity } from "@/entities/professional-indemnity/ProfessionalIndemnity";
import { FirmBasicInfo } from "@/entities/FirmBasicInfo";
import { v4 as uuid } from "uuid";

export class ProfessionalIndemnityModel {
  public id: string = "";
  public pii: ProfessionalIndemnity = new ProfessionalIndemnity();
  public isCollapsed: boolean = false;
  public piiInsurerInputFirm: FirmBasicInfo = new FirmBasicInfo();
  public piiBrokerInputFirm: FirmBasicInfo = new FirmBasicInfo();

  constructor(pii?: ProfessionalIndemnity) {
    this.id = uuid();
    this.pii = pii ?? new ProfessionalIndemnity();
  }
}