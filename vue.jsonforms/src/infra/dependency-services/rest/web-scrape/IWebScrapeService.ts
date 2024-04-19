export declare interface IWebScrapeService {
  webScrapeAndRegisterAsync(email: string, fpid: string): void;

  registerMediaAsync(id: string, url: string | undefined, customerId: string): void;
}

export const IWebScrapeServiceInfo = {
  name: "IWebScapeService",
};
