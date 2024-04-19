import { IIntroducerPagesRouteService } from "@/infra/dependency-services/pages-route-introducers/IIntroducerPagesRouteService";
import PagesRouteServiceBase from "@/infra/dependency-services/PagesRouteServiceBase";

export default class IntroducerPagesRouteService
  extends PagesRouteServiceBase
  implements IIntroducerPagesRouteService {}