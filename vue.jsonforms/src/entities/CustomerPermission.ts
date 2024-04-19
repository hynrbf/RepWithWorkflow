import {PermissionEdit} from "./PermissionEdit";
import {PermissionStateEnum} from "./enums/PermissionStateEnum";

export class CustomerPermission extends PermissionEdit {
    public state: PermissionStateEnum = PermissionStateEnum.Added;
    public hasPendingApplication: boolean = false;
    public isModified: boolean = false;
}