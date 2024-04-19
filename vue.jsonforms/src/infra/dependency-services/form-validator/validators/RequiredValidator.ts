import {injectable, registry} from "tsyringe";
import {IValidator, ValidatorToken} from "./IValidator";
import isEmpty from "lodash/isEmpty";
import isNil from "lodash/isNil";
import isNumber from "lodash/isNumber";
import { AppConstants } from "@/infra/AppConstants";

@injectable()
@registry([{token: ValidatorToken, useClass: RequiredValidator}])
export class RequiredValidator implements IValidator {
    execute<T>(value: T) {
        return !(isNil(value) ||
            isEmpty(isNumber(value) ? [value] : value) ||
            value === false);
    }

    getMessage(name: string) {
        return `${name} is required.`;
    }

    getKey() {
        return AppConstants.RequiredKey;
    }
}
