export class FirmPermission {
  public categoryName: string = "";
  public permissionName: string | undefined;
  public PermissionDisplayText: string | undefined;
  public isForOthers: boolean = false;
  public customerTypes: string[] = [];
  public investmentTypes: string[] = [];
  public limitations: string[] = [];
}