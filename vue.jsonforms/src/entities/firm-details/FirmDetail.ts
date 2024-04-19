import {FirmDetailsBase} from "@/entities/FirmDetailsBase";

export class FirmDetail extends FirmDetailsBase {
  public headOfficeAddress: string | undefined;
  public isHeadOfficeSameAsTradingAddress: boolean = false;
  public isHeadOfficeAddressChanged: boolean = false;
}