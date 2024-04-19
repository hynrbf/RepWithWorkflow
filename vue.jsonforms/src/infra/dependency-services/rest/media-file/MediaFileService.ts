import { singleton } from "tsyringe";
import { IMediaFileService } from "./IMediaFileService";
import { REMOTE_API } from "@/config";
import RestBase from "../RestBase";

@singleton()
export default class MediaFileService
  extends RestBase
  implements IMediaFileService
{
  public async uploadImageAsync(
    file: Blob,
    fileExtension: string,
    documentName: string,
    additionalData?: string,
    formData?: FormData,
    onProgress?: (progressEvent: any) => void,
  ): Promise<string> {
    if (!formData) {
      formData = new FormData();
    }

    formData.append("file", file);
    formData.append("name", documentName);
    formData.append("fileExtension", fileExtension);

    let response;

    if (additionalData) {
      const segment = additionalData.split("-");
      formData.append("purpose", segment[0]);
      formData.append("companyNo", segment[1]);

      response = await this.postRemoteWithConfigAsync<string>(
        `${REMOTE_API}/UploadCustomerFileAsync`,
        formData,
        true,
        {
          onUploadProgress: onProgress,
        },
      );
      return response;
    }

    response = await this.postRemoteWithConfigAsync<string>(
      `${REMOTE_API}/UploadImageFileAsync`,
      formData,
      true,
      {
        onUploadProgress: onProgress,
      },
    );
    return response;
  }

  public async uploadDocumentAsync(
    file: Blob,
    fileExtension: string,
    documentName: string,
    uploadFor?: string,
    formData?: FormData,
    onProgress?: (progressEvent: any) => void,
  ): Promise<Record<string, unknown>> {
    if (!formData) {
      formData = new FormData();
    }

    formData.append("file", file);
    formData.append("name", documentName);
    formData.append("fileExtension", fileExtension);

    let response;

    if (uploadFor) {
      const segment = uploadFor.split("-");
      formData.append("purpose", segment[0]);
      formData.append("companyNo", segment[1]);

      if (segment[2] !== null && segment[2] !== undefined) {
        formData.append("corporateCompanyNo", segment[2]);
      }

      if (segment[3] !== null && segment[3] !== undefined) {
        formData.append("foreNameAndSurname", segment[3]);
      }

      response = await this.postRemoteWithConfigAsync<Record<string, unknown>>(
        `${REMOTE_API}/UploadCustomerFileAsync`,
        formData,
        true,
        {
          onUploadProgress: onProgress,
        },
      );
    } else {
      response = await this.postRemoteWithConfigAsync<Record<string, unknown>>(
        `${REMOTE_API}/UploadDocumentAsync`,
        formData,
        true,
        {
          onUploadProgress: onProgress,
        },
      );
    }

    return response;
  }
}
