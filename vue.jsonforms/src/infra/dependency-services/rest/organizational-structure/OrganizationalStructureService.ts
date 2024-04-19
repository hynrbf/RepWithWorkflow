import { REMOTE_API } from "@/config";
import RestBase from "@/infra/dependency-services/rest/RestBase";
import { singleton } from "tsyringe";
import { IOrganizationalStructureService } from "./IOrganizationalStructureService";
import { Employee } from "@/entities/firm-details/Employee";

@singleton()
export default class OrganizationalStructureService
  extends RestBase
  implements IOrganizationalStructureService
{
  constructor() {
    super();
  }

  public async getEmployeesAsync(companyNumber: string): Promise<Employee[]> {
    return await this.getRemoteAsync<Employee[]>(
      `${REMOTE_API}/GetEmployeesAsync/${companyNumber}`,
    );
  }

  public async getEmployeeByEmailAsync(email: string): Promise<Employee | undefined> {
    return await this.getRemoteAsync<Employee>(
        `${REMOTE_API}/GetEmployeeByEmailAsync/${email}`,
    );
  }

  public async saveOrUpdateEmployeeAsync(
    employee: Employee,
  ): Promise<Employee> {
    return await this.postRemoteAsync<Employee>(
      `${REMOTE_API}/SaveOrUpdateEmployeeAsync`,
      JSON.stringify(employee),
    );
  }
}
