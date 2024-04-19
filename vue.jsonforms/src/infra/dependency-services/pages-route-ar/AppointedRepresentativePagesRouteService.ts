import { IAppointedRepresentativePagesRouteService } from "@/infra/dependency-services/pages-route-ar/IAppointedRepresentativePagesRouteService";
import PagesRouteServiceBase from "@/infra/dependency-services/PagesRouteServiceBase";

export default class AppointedRepresentativePagesRouteService
  extends PagesRouteServiceBase
  implements IAppointedRepresentativePagesRouteService {}
