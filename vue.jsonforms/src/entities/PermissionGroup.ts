import {SubPermission} from "./SubPermission";

export class PermissionGroup {
    public id: string | undefined;
    public permissionGroupName: string | undefined;
    public categoryName: string | undefined;
    public subPermissions: SubPermission[] = [];
}