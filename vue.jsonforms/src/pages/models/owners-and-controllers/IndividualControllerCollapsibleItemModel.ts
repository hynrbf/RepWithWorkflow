import { IndividualControllerModel } from "@/pages/models/owners-and-controllers/IndividualControllerModel";

export class IndividualControllerCollapsibleItemModel {
  public individualController: IndividualControllerModel = new IndividualControllerModel();
  public isCollapsed: boolean = false;
  public fullName: string = "";
}
