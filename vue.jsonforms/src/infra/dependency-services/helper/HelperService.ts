import { IHelperService } from "./IHelperService";
import { AppConstants } from "../../AppConstants";
import { singleton } from "tsyringe";
import { ContactNumber } from "@/entities/ContactNumber";
import axios from "axios";
import { nextTick } from "vue";
import { FcaAddressDetail } from "@/entities/FcaAddressDetail";
import { FCA_URL } from "@/config";
import { CountryCode } from "@/entities/CountryCode";
import { CountryDialCode } from "@/entities/CountryDialCode";
import { isFinite, isNaN } from "lodash";

@singleton()
export default class HelperService implements IHelperService {
  //ToDo. to clean up base by what it does soon, read the IHelperService interface
  dateStringToEpoch(dateString: string): number {
    if (dateString === new Date(0).toDateString()){
      return 0;
    }

    const date = new Date(dateString);
    return date.getTime() / 1000;
  }

  dateToEpoch(date: Date): number {
    return date.getTime() / 1000;
  }

  isStringNumber(value: string): boolean {
    const parsedValue = parseFloat(value);

    if (isNaN(parsedValue)) {
      return false;
    }

    return isFinite(parsedValue);
  }

  convertToSafeNumber(input: string): number {
    const value = parseFloat(input);

    if (isNaN(value)) {
      return 0;
    }

    return value;
  }

  getCurrentDateTimeInEpoch(): number {
    const currentTime = new Date();
    return Math.floor(currentTime.getTime() / 1000);
  }

  convertEpochToDateTime(epoch: number): Date | undefined {
    if (epoch <= 0) {
      return undefined;
    }

    return new Date(epoch * 1000);
  }

  delayAsync(ms: number): Promise<void> {
    return new Promise((resolve) => {
      setTimeout(() => {
        resolve();
      }, ms);
    });
  }

  formatUkNumberValuesBeforeSaving(input: string | undefined): string {
    if (!input) {
      return "";
    }

    if (input.startsWith("0")) {
      // Remove starting '0' in input
      input = input.substring(1);
    }

    return `${AppConstants.UkCountryCode}${input}`.replaceAll(" ", "");
  }

  cleanContactNumber(input: string | undefined): string {
    if (!input) {
      return "";
    }

    return input.replaceAll(AppConstants.UkCountryCode, "").replaceAll(" ", "");
  }

  async convertToContactNoAsync(
    value: string,
    country: string,
  ): Promise<ContactNumber> {
    if (!(value && country)) {
      return new ContactNumber();
    }

    //e.g. +4402077818019
    const found = await this.getCountryDialCodeAsync(country);

    if (!found) {
      return new ContactNumber();
    }

    let restOfNumber = value;

    if (value.startsWith(found.dial_code)) {
      restOfNumber = value.replace(found.dial_code, "");
    }

    return {
      countryCode: found.code.toLowerCase(),
      number: restOfNumber,
      dialCode: found.dial_code,
      country: found.name,
    };
  }

  removePostCodeString(input: string): string {
    if (!input) {
      return "";
    }

    if (!input.includes(")")) {
      return input;
    }

    const fIndex = input.indexOf("(Postcode:");
    return input.substring(0, fIndex - 1).trim();
  }

  capitalizeFirstLetterOfWord(input: string): string {
    input = input?.trim() ?? "";

    if (input.length === 0) {
      return input;
    }

    input = input.toLowerCase();
    return input.charAt(0).toUpperCase() + input.slice(1);
  }

  resizeExpander() {
    //nextTick or  setTimeout(() => {  /*content here*/  }, 10);
    nextTick(() => {
      const collapseContainers = document.querySelectorAll(
        ".k-child-animation-container",
      );

      collapseContainers.forEach((container) => {
        const div = container as HTMLElement;
        div.style.maxHeight = "fit-content";
      });
    });
  }

  getDateForGivenYearsAgo(yearElapsed: number): Date {
    const currentDate = new Date();
    return new Date(
      currentDate.getFullYear() - yearElapsed,
      currentDate.getMonth(),
      currentDate.getDate(),
    );
  }

  private async getCountryDialCodeAsync(value: string): Promise<{
    name: string;
    dial_code: string;
    code: string;
  }> {
    const response = await axios.get("/api/list.countryDialCodes.json");
    const countryObj = JSON.parse(
      JSON.stringify(response.data),
    ) as CountryDialCode[];

    return (
      countryObj?.find((c) => c.name.toLowerCase() === value.toLowerCase()) ??
      new CountryDialCode()
    );
  }

  stripHTMLTags(content: string) {
    const tempElem = document.createElement("div");
    tempElem.innerHTML = content;
    return tempElem.textContent || tempElem.innerText || "";
  }

  isWordExist(content: string, word: string) {
    return content.toLowerCase().split(/\s+/g).includes(word.toLowerCase());
  }

  roundOffToNearestWholeNumber(value: number): number {
    if (value) {
      return this.roundToNearestTen(value);
    }

    return 0;
  }

  roundToNearestTen(num: number): number {
    const remainder = num % 10;
    let base = num - remainder;

    if (remainder >= 5) {
      base += 10;
    }

    return base;
  }

  removeStringSpacesThenSlash(value: string): string {
    return value.replace(/\s+/g, "").replace("/", "");
  }

  async getCountryCodeAsync(countryName: string): Promise<string> {
    if (countryName.toLowerCase() === "england") {
      return AppConstants.DefaultCountryCode.toUpperCase();
    }

    const response = await axios.get("/api/countries.json");
    const countries = response.data as CountryCode[];
    const foundCountry = countries.find(
      (c) => c.name.toLowerCase() === countryName.toLowerCase(),
    );
    return foundCountry?.code ?? AppConstants.DefaultCountryCode.toUpperCase();
  }

  formatFcaAddress(details: FcaAddressDetail): string {
    return ["Address Line 1", "Address Line 2", "town", "country", "postcode"]
      .map((i) => details[i])
      .filter((i) => i)
      .join(", ")
      .trim();
  }

  getRandomHexColor() {
    const red = Math.floor(Math.random() * 256);
    const green = Math.floor(Math.random() * 256);
    const blue = Math.floor(Math.random() * 256);
    // Convert the values to hex and pad with zeros if necessary
    const hexRed = red.toString(16).padStart(2, "0");
    const hexGreen = green.toString(16).padStart(2, "0");
    const hexBlue = blue.toString(16).padStart(2, "0");
    const hexColor = `#${hexRed}${hexGreen}${hexBlue}`;
    return hexColor;
  }

  generateFCASearchUrl(fcaFirmRefNo: string) {
    return `${FCA_URL}/search?q=${fcaFirmRefNo}&type=Companies`;
  }

  /**
   * A function that takes a string input and converts it to a capitalized words.
   *
   * @param {string} input - any input string to be converted such as kebab case, snake case, pascal case, camel case
   * @return {string} the capitalized words
   */
  convertToCapitalizedWords(input: string) {
    const kebabCase = input.replace(/-+/g, " "); // kebab case to words with spaces
    const pascalCase = input.replace(/([a-z])([A-Z])/g, "$1 $2"); // pascal case to words with spaces
    const snakeCase = input.replace(/_/g, " "); // snake case to words with spaces
    const camelCase = input
      .replace(/([a-z])([A-Z])/g, "$1 $2")
      .replace(/^./, (str) => str.toUpperCase()); // camel case to words with space

    const capitalizeSentence = (str: string): string => {
      // Function to capitalize the first letter of each word in a sentence
      return str.replace(
        /\w\S*/g,
        (word) => word.charAt(0).toUpperCase() + word.substr(1).toLowerCase(),
      );
    };

    const result = capitalizeSentence(
      kebabCase || pascalCase || snakeCase || camelCase,
    ); // capitalize the words based on the converted formats

    return result; // Return the capitalized words as a string
  }

  checkIfEmailFormatIsValid(email: string): boolean {
    const emailRegex: RegExp = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }
}
