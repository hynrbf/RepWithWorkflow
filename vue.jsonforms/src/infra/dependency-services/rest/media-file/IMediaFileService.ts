export declare interface IMediaFileService {
  uploadImageAsync(
    file: Blob,
    fileExtension: string,
    documentName: string,
    uploadFor?: string,
    formData?: FormData,
    onProgress?: (progressEvent: any) => void
  ): Promise<string>;

  uploadDocumentAsync(
    file: Blob,
    fileExtension: string,
    documentName: string,
    uploadFor?: string,
    formData?: FormData,
    onProgress?: (progressEvent: any) => void
  ): Promise<Record<string, unknown>>;
}

export const IMediaFileServiceInfo = {
  name: "IMediaFileService",
};
