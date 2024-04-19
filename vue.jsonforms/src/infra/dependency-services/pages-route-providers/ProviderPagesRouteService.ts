import { IProviderPagesRouteService } from "@/infra/dependency-services/pages-route-providers/IProviderPagesRouteService";
import PagesRouteServiceBase from "@/infra/dependency-services/PagesRouteServiceBase";

export default class ProviderPagesRouteService
  extends PagesRouteServiceBase
  implements IProviderPagesRouteService {}
