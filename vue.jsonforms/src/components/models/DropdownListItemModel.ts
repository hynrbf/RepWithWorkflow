export default class DropdownListItemModel {
  public label: string | undefined;
  public value: string | undefined;
  public class?: string;
  // ToDo. part of 18 IMPT errors to fix
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  public raw?: any;
}