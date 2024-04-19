import {HtmlSource} from "@/entities/HtmlSource";

export declare interface IDocumentService {
    getDocumentsAsync(): Promise<HtmlSource[]>;
    getDocumentsByNameAsync(name: string): Promise<HtmlSource>;
    updateDocumentAsync(documentJson: string): Promise<HtmlSource>;
    uploadWordToConvertToHtmlAsync(file: Blob, fileExtension: string, documentName: string): Promise<HtmlSource>;
    convertHtmlToWordAsync(): Promise<string>;
}

export const IDocumentServiceInfo = {
    name: "IDocumentService"
};