import { CorporateControllerModel } from "@/pages/models/owners-and-controllers/CorporateControllerModel";

export class CorporateControllerCollapsibleItem {
  public corporateController: CorporateControllerModel = new CorporateControllerModel();
  public isCollapsed: boolean = false;
  public fullName: string | undefined;
  public items:CorporateControllerCollapsibleItem[] = [] as CorporateControllerCollapsibleItem[];
}
