export interface IValidator {
    execute<T>(value: T, name?: string): string | boolean

    getMessage(name?: string): string

    getKey(): string
}

export const ValidatorToken = Symbol("validator-token");
