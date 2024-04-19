export interface RequestConfigOptions {
  maxBodyLength: number;
  headers: {
    [key: string]: string | null;
  };
}
