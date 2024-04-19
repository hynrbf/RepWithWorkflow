import { IProfanityService } from "./IProfanityService";
import RestBase from "../RestBase";
import { AppConstants } from "@/infra/AppConstants";
import { singleton } from "tsyringe";
import {Profanity} from "@/entities/Profanity";

@singleton()
export default class ProfanityService
  extends RestBase
  implements IProfanityService
{
  async getProfanityWordsAndSaveLocallyAsync(): Promise<Profanity[]
  > {
    //This is the original json and is get from here https://github.com/dsojevic/profanity-list/blob/main/en.json
    const resultLocal = await this.getRemoteAsync<Profanity[]>("/api/profanity.json");
    localStorage.setItem(
      AppConstants.profanityKey,
      JSON.stringify(resultLocal),
    );
    return resultLocal;
  }

  hasOffensiveWords(_word: string): boolean {
    // TODO. to turn on later.
    // let profanityListLocal = localStorage.getItem(AppConstants.profanityKey);
    //
    // if (!profanityListLocal) {
    //     throw new Error("ProfanityService.hasOffensiveWords\t: Profanity list should be saved in local cache already.");
    // }
    //
    // let profanityList = JSON.parse(profanityListLocal) as {
    //     id: string,
    //     match: string,
    //     tags: string[],
    //     severity: number,
    //     exceptions: string[]
    // }[];
    //
    // if (!word) {
    //     return false;
    // }
    //
    // console.log(`ProfanityService.hasOffensiveWords\t: checking if word '${word}' is offensive ...`);
    //
    // for (const profanity of profanityList) {
    //     let matches = profanity.match.split('|');
    //
    //     for (const match of matches) {
    //         const regex: RegExp = new RegExp(`${match}`);
    //         let wordToMatchLowered = word.toLowerCase();
    //
    //         if (regex.test(wordToMatchLowered)) {
    //             console.log("Profanity found match, check if really is");
    //
    //             if (!profanity.exceptions) {
    //                 console.log(`ProfanityService.hasOffensiveWords\t: '${match}' and the value '${word} matched!. Word '${word}' is OFFENSIVE.'`);
    //                 return true;
    //             }
    //
    //             for (const excIdx in profanity.exceptions) {
    //                 console.log(profanity.exceptions[excIdx]);
    //                 // Replaced the '*' with '.*' to match any number of characters in regex
    //                 // example in json file m*cript -> m.*cript
    //                 let wordPattern = `${profanity.exceptions[excIdx]}`.replace("*", ".*");
    //                 const regExc: RegExp = new RegExp(`^${wordPattern}$`);
    //
    //                 if (regExc.test(wordToMatchLowered)) {
    //                     console.log(`ProfanityService.hasOffensiveWords\t: word '${word}' is NOT offensive.`);
    //                     return false;
    //                 }
    //             }
    //
    //             console.log(`ProfanityService.hasOffensiveWords\t: '${match}' and the value '${word} matched!. Word '${word}' is OFFENSIVE.'`);
    //             return true;
    //         }
    //     }
    // }
    //
    // console.log(`ProfanityService.hasOffensiveWords\t: word '${word}' is NOT offensive.`);
    return false;
  }
}
