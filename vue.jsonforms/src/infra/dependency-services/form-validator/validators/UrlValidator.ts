import {injectable, registry} from "tsyringe";
import {IValidator, ValidatorToken} from "./IValidator";
import isEmpty from "lodash/isEmpty";
import { AppConstants } from "@/infra/AppConstants";

@injectable()
@registry([{token: ValidatorToken, useClass: UrlValidator}])
export class UrlValidator implements IValidator {
    execute<T>(value: T) {
        if (isEmpty(value)) {
            return true;
        }

        try {
            new URL(value as string);
            return true;
        } catch {
            return false;
        }
    }

    getMessage(name: string) {
        return `${name} is not a valid URL.`;
    }

    getKey() {
        return AppConstants.UrlKey;
    }
}
