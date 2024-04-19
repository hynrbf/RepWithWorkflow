import { ContactNumber } from "@/entities/ContactNumber";
import { FcaAddressDetail } from "@/entities/FcaAddressDetail";

export declare interface IHelperService {
  //ToDo. to clean up base by what it does soon
  //helper-dates
  dateStringToEpoch(dateString: string): number;

  dateToEpoch(date: Date): number;

  getCurrentDateTimeInEpoch(): number;

  convertEpochToDateTime(epoch: number): Date | undefined;

  getDateForGivenYearsAgo(yearElapsed: number): Date;

  //helper-formatting
  isStringNumber(value: string): boolean;

  convertToSafeNumber(input: string): number;

  formatUkNumberValuesBeforeSaving(input: string | undefined): string;

  cleanContactNumber(input: string | undefined): string;

  convertToContactNoAsync(
    value: string,
    country: string,
  ): Promise<ContactNumber>;

  removePostCodeString(input: string): string;

  capitalizeFirstLetterOfWord(input: string): string;

  checkIfEmailFormatIsValid(email: string): boolean;

  //helper-string
  isWordExist(content: string, word: string): boolean;

  removeStringSpacesThenSlash(value: string): string;

  convertToCapitalizedWords(input: string): string;

  stripHTMLTags(content: string): string;

  //helper-fca
  getCountryCodeAsync(countryName: string): Promise<string>;

  formatFcaAddress(details: FcaAddressDetail): string;

  generateFCASearchUrl(fcaFirmRefNo: string): string;

  //helper-numbers
  roundOffToNearestWholeNumber(value: number): number;

  //this should stay here
  resizeExpander(): void;

  getRandomHexColor(): string;

  delayAsync(ms: number): Promise<void>;
}

export const IHelperServiceInfo = {
  name: "IHelperService",
};
