import { singleton } from "tsyringe";
import { HtmlSource } from "@/entities/HtmlSource";
import { APP_CACHE_ENABLE, REMOTE_API, USE_REMOTE_DB } from "@/config";
import RestBase from "../RestBase";
import { IDocumentService } from "./IDocumentService";

@singleton()
export default class DocumentService
  extends RestBase
  implements IDocumentService
{
  readonly _jsonHtmlSourceKey: string = "html-source-document";

  public async getDocumentsAsync(): Promise<HtmlSource[]> {
    const htmlSourceDocumentsLocalStore = localStorage.getItem(
      this._jsonHtmlSourceKey,
    );

    if (htmlSourceDocumentsLocalStore && APP_CACHE_ENABLE) {
      return JSON.parse(htmlSourceDocumentsLocalStore) as HtmlSource[];
    }

    if (!USE_REMOTE_DB) {
      const resultLocal = await this.getRemoteAsync<HtmlSource[]>(
        "/api/htmlSource.json",
      );
      localStorage.setItem(
        this._jsonHtmlSourceKey,
        JSON.stringify(resultLocal),
      );
      return resultLocal;
    }

    const documents = await this.getRemoteAsync<HtmlSource[]>(
      `${REMOTE_API}/GetAllHtmlSourceDocumentsAsync`,
    );

    if (APP_CACHE_ENABLE && documents.length > 0) {
      localStorage.setItem(this._jsonHtmlSourceKey, JSON.stringify(documents));
    }

    return documents;
  }

  public async getDocumentsByNameAsync(name: string): Promise<HtmlSource> {
    return await this.getRemoteAsync<HtmlSource>(
      `${REMOTE_API}/GetDocumentHtmlContentAsync/${name}`,
    );
  }

  public async updateDocumentAsync(documentJson: string): Promise<HtmlSource> {
    if (!USE_REMOTE_DB) {
      const htmlDocumentsFromLocalStoreString = localStorage.getItem(
        this._jsonHtmlSourceKey,
      );

      if (htmlDocumentsFromLocalStoreString) {
        const data = JSON.parse(documentJson) as HtmlSource;
        const htmlDocFromLocalStoreObj = JSON.parse(
          htmlDocumentsFromLocalStoreString,
        ) as HtmlSource[];
        const index = htmlDocFromLocalStoreObj.findIndex(
          (s) => s.id === data.id,
        );
        htmlDocFromLocalStoreObj[index].content = data.content;
        localStorage.setItem(
          this._jsonHtmlSourceKey,
          JSON.stringify(htmlDocFromLocalStoreObj),
        );
        return htmlDocFromLocalStoreObj[index];
      }

      throw `${this._jsonHtmlSourceKey} should be in the local storage already`;
    }

    const updatedHtmlSourceDocumentModel =
      await this.putRemoteAsync<HtmlSource>(
        `${REMOTE_API}/SaveHtmlSourceDocumentAsync`,
        JSON.parse(documentJson),
      );

    // Remove Tiny MCE documents list from local storage
    localStorage.removeItem(this._jsonHtmlSourceKey);
    return updatedHtmlSourceDocumentModel;
  }

  public async uploadWordToConvertToHtmlAsync(
    file: Blob,
    fileExtension: string,
    documentName: string,
  ): Promise<HtmlSource> {
    const formData = new FormData();
    formData.append("file", file);
    formData.append("name", documentName);
    formData.append("fileExtension", fileExtension);
    return await this.postRemoteAsync<HtmlSource>(
      `${REMOTE_API}/UploadDocumentAsync`,
      formData,
      true,
    );
    //ToDo. this is only testing for uploading files or images
    // return await this.postRemoteAsync(
    //   `${REMOTE_API}/UploadImageFileAsync`,
    //   formData,
    //   true,
    // );
  }

  public async convertHtmlToWordAsync(): Promise<string> {
    return await this.postRemoteAsync<string>(
      `${REMOTE_API}/ConvertHtmlToDocAsync`,
      "",
    );
  }
}
