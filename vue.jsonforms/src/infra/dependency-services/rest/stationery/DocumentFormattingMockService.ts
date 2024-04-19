import { singleton } from "tsyringe";
import { IDocumentFormattingService } from "./IDocumentFormattingService";
import { DocumentFormatting } from "@/entities/stationery/DocumentFormatting";
import { v4 as uuid } from "uuid";
import moment from "moment";

@singleton()
export default class DocumentFormattingMockService
  implements IDocumentFormattingService
{
  private documentFormattings: DocumentFormatting[] = [
    {
      id: uuid(),
      name: "Title 1",
      font: "Inter, sans-serif",
      size: 16,
      isBold: false,
      isItalic: false,
      isUnderline: false,
      alignment: "left",
      textCase: "",
      updatedAt: moment().unix(),
    },
    {
      id: uuid(),
      name: "Title 2",
      font: "Inter, sans-serif",
      size: 14,
      isBold: false,
      isItalic: false,
      isUnderline: false,
      alignment: "left",
      textCase: "",
      updatedAt: moment().unix(),
    },
    {
      id: uuid(),
      name: "Header 1",
      font: "Figtree, sans-serif",
      size: 16,
      isBold: false,
      isItalic: false,
      isUnderline: false,
      alignment: "left",
      textCase: "",
      updatedAt: moment().unix(),
    },
    {
      id: uuid(),
      name: "Header 2",
      font: "Figtree, sans-serif",
      size: 14,
      isBold: false,
      isItalic: false,
      isUnderline: false,
      alignment: "left",
      textCase: "",
      updatedAt: moment().unix(),
    },
    {
      id: uuid(),
      name: "Paragraph",
      font: "Figtree, sans-serif",
      size: 12,
      isBold: false,
      isItalic: false,
      isUnderline: false,
      alignment: "left",
      textCase: "",
      updatedAt: moment().unix(),
    },
  ];

  public getDocumentFormattingsAsync(): Promise<DocumentFormatting[]> {
    return Promise.resolve(this.documentFormattings);
  }

  public updateDocumentFormattingAsync(
    id: string,
    payload: Partial<DocumentFormatting>,
  ): Promise<DocumentFormatting> {
    let updatedItem: DocumentFormatting = {
      alignment: undefined,
      font: undefined,
      id: undefined,
      isBold: undefined,
      isItalic: undefined,
      isUnderline: undefined,
      name: undefined,
      size: undefined,
      textCase: undefined,
      updatedAt: undefined
    };

    this.documentFormattings = this.documentFormattings.map((item) => {
      if (item.id === id) {
        return (updatedItem = {
          ...item,
          ...payload,
          updatedAt: moment().unix(),
        });
      }

      return item;
    });
    return Promise.resolve(updatedItem);
  }
}
