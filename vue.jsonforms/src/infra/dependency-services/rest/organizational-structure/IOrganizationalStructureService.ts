import { Employee } from "@/entities/firm-details/Employee";

export declare interface IOrganizationalStructureService {
  getEmployeesAsync(companyNumber: string): Promise<Employee[]>;
  getEmployeeByEmailAsync(email: string): Promise<Employee | undefined>;
  // TODO. to complete CRUD
  saveOrUpdateEmployeeAsync(employee: Employee): Promise<Employee>;
}

export const IOrganizationalStructureServiceInfo = {
  name: "IOrganizationalStructureService",
};
