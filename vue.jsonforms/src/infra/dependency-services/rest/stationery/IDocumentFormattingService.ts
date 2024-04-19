import { DocumentFormatting } from "@/entities/stationery/DocumentFormatting";

export declare interface IDocumentFormattingService {
  getDocumentFormattingsAsync(): Promise<DocumentFormatting[]>;
  updateDocumentFormattingAsync(
    id: string,
    payload: Record<string, unknown>
  ): Promise<DocumentFormatting>;
}

export const IDocumentFormattingServiceInfo = {
  name: "IDocumentFormattingService",
};
