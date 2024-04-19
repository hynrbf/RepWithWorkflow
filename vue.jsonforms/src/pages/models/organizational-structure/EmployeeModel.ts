import { Employee } from "@/entities/firm-details/Employee";
import Role from "@/entities/org-structure/Role";
import { ProductType } from "@/entities/org-structure/ProductType";
import { ContactNumber } from "@/entities/ContactNumber";
import { IEmployeeTask } from "@/entities/org-structure/IEmployeeTask";

export default class EmployeeModel {
  //copied from entities
  public id: string = "";
  public companyNo: string = "";
  public title: string = "";
  public firstName: string = "";
  public lastName: string = "";
  public lineManager: Employee | null = null;
  public primaryRole: Role | null = null;
  public otherRoles?: Role[] | null;
  public productType: ProductType[] = [];
  public email: string = "";
  public contactNumber: ContactNumber = new ContactNumber();
  public status: string = "";
  public isRoot: boolean = false;
  public img_id: string = "";
  public employmentStatus?: string = "Active";
  public tasks?: IEmployeeTask[] = [];

  //used in UI
  public staffName: string | undefined;
  public lineManagerName: string | undefined;
  public primaryRoleStr?: string | undefined;
  public contactNumberStr: string | undefined;
}
