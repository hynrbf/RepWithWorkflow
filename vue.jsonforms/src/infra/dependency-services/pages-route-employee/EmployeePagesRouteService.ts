import { IEmployeePagesRouteService } from "@/infra/dependency-services/pages-route-employee/IEmployeePagesRouteService";
import PagesRouteServiceBase from "@/infra/dependency-services/PagesRouteServiceBase";

export default class EmployeePagesRouteService
  extends PagesRouteServiceBase
  implements IEmployeePagesRouteService {}
