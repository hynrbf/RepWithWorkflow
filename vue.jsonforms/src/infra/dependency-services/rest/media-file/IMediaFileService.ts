export declare interface IMediaFileService {
  uploadImageAsync(
    file: Blob,
    fileExtension: string,
    documentName: string,
    uploadFor?: string,
    formData?: FormData,
    // ToDo. part of 18 IMPT errors to fix
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    onProgress?: (progressEvent: any) => void
  ): Promise<string>;

  uploadDocumentAsync(
    file: Blob,
    fileExtension: string,
    documentName: string,
    uploadFor?: string,
    formData?: FormData,
    // ToDo. part of 18 IMPT errors to fix
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    onProgress?: (progressEvent: any) => void
  ): Promise<Record<string, unknown>>;
}

export const IMediaFileServiceInfo = {
  name: "IMediaFileService",
};
