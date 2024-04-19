import { MediaMarketingOutlet } from "@/entities/media-marketing-outlet/MediaMarketingOutlet";

export declare interface IMediaMarketingOutletService {
  getMediaMarketingOutletsAsync(): Promise<MediaMarketingOutlet[]>;
  getMediaMarketingOutletAsync(id: string): Promise<MediaMarketingOutlet | undefined>;
  addMediaMarketingOutletAsync(
    payload: Record<string, unknown>
  ): Promise<MediaMarketingOutlet>;
  updateMediaMarketingOutletAsync(
    id: string,
    payload: Record<string, unknown>
  ): Promise<MediaMarketingOutlet>;
}

export const IMediaMarketingOutletServiceInfo = {
  name: "IMediaMarketingOutletService",
};
