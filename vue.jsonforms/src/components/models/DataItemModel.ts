export default class DataItemModel {
    public label: string | undefined;
    public value: string | undefined;
    public items?: DataItemModel[];
    public parent?: string | undefined;
    public expanded?: boolean | undefined;
    public checkField?: boolean | undefined;
    public checkIndeterminateField?: boolean | undefined;
}