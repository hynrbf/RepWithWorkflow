import {IEncodingService} from "./IEncodingService";
import {singleton} from "tsyringe";

@singleton()
export default class Base64EncodingService implements IEncodingService {
    public encode(input: string): string {
        return btoa(input);
    }

    decode(input: string): string {
        return atob(input);
    }
}