export declare interface IFormValidatorService {
  // ToDo. part of 18 IMPT errors to fix
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  validate<T>(value: T, rules: Record<string, any>): string;
  setFieldName(name: string): void;
}

export const IFormValidatorServiceInfo = {
  name: "IFormValidatorService",
};
