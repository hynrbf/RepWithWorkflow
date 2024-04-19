export declare interface IProfanityService {
    getProfanityWordsAndSaveLocallyAsync() : Promise<{
        id: string,
        match: string,
        tags: string[],
        severity: number
    }[]>;

    hasOffensiveWords(word: string): boolean;
}

export const IProfanityServiceInfo = {
    name: "IProfanityService"
};