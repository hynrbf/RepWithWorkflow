import { ContactNumber } from "../ContactNumber";
import { ProductType } from "@/entities/org-structure/ProductType";
import Role from "@/entities/org-structure/Role";
import { IEmployeeTask } from "@/entities/org-structure/IEmployeeTask";
import {CustomerBase} from "@/entities/CustomerBase";

export class Employee extends CustomerBase {
  public companyNo: string = "";
  public contactNumber: ContactNumber = new ContactNumber();
  public isRoot: boolean = false;

  public status?: string = "";
  public customerId?: string;
  public title?: string;
  public lineManager?: Employee;
  public primaryRole?: Role;
  public otherRoles?: Role[] = [];
  public productType?: ProductType[] = [];
  public img_id?: string = "";
  public employmentStatus?: string = "Onboarding";
  public tasks?: IEmployeeTask[] = [];
}
