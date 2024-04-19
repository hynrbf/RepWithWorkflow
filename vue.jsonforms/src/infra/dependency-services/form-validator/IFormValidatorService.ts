export declare interface IFormValidatorService {
  validate<T>(value: T, rules: Record<string, any>): string;
  setFieldName(name: string): void;
}

export const IFormValidatorServiceInfo = {
  name: "IFormValidatorService",
};
