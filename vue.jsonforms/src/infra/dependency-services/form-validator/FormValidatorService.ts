import {injectAll, injectable} from "tsyringe";
import {IFormValidatorService} from "./IFormValidatorService";
import {IValidator, ValidatorToken} from "./validators/IValidator";
import "./validators";

@injectable()
export default class FormValidatorService implements IFormValidatorService {
    private _fieldName: string = "";

    constructor(@injectAll(ValidatorToken) private validators: IValidator[]) {
    }

    public validate<T>(value: T, rules: Record<string, any> = {}) {
        const validatorsObject: Record<string, IValidator> = this.validators.reduce(
            (accumulator, validator) => ({
                ...accumulator,
                [validator.getKey()]: validator,
            }),
            {}
        );

        for (const rule in rules) {
            if (
                rules[rule] &&
                validatorsObject[rule] &&
                !validatorsObject[rule].execute(value)
            ) {
                return validatorsObject[rule].getMessage(this._fieldName);
            }
        }

        return "";
    }

    public setFieldName(name: string): void {
        this._fieldName = name;
    }
}
