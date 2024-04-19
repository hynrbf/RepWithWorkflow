export default class Role {
    public name: string | undefined;
    public state: string | undefined;
    public isModified: boolean = false;
    public isFcaAuthorised: boolean = false;
    public isPending: boolean = false;
}