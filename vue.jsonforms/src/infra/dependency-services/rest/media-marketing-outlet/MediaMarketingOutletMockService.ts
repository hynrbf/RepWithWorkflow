import { singleton } from "tsyringe";
import { IMediaMarketingOutletService } from "./IMediaMarketingOutletService";
import { MediaMarketingOutlet } from "@/entities/media-marketing-outlet/MediaMarketingOutlet";
import { v4 as uuid } from "uuid";

@singleton()
export default class MediaMarketingOutletService
  implements IMediaMarketingOutletService
{
  private _mediaMarketingOutlets: MediaMarketingOutlet[] = [];

  public getMediaMarketingOutletsAsync(): Promise<MediaMarketingOutlet[]> {
    return Promise.resolve(this._mediaMarketingOutlets);
  }

  public getMediaMarketingOutletAsync(
    id: string,
  ): Promise<MediaMarketingOutlet | undefined> {
    return Promise.resolve(
      this._mediaMarketingOutlets.find((item) => item.id === id),
    );
  }

  public addMediaMarketingOutletAsync(
    payload: Partial<MediaMarketingOutlet>,
  ): Promise<MediaMarketingOutlet> {
    const newItem: MediaMarketingOutlet = {
      id: uuid(),
      url: payload.url,
      name: payload.name,
      owner: payload.owner,
      archived: false,
      platform: payload.platform,
      createdAt: Date.now(),
    };

    this._mediaMarketingOutlets = [...this._mediaMarketingOutlets, newItem];
    return Promise.resolve(newItem);
  }

  public updateMediaMarketingOutletAsync(
    id: string,
    payload: Partial<MediaMarketingOutlet>,
  ): Promise<MediaMarketingOutlet> {
    let updatedItem: MediaMarketingOutlet = {
      archived: false,
      createdAt: undefined,
      id: "",
      name: undefined,
      owner: undefined,
      platform: undefined,
      url: undefined
    };

    this._mediaMarketingOutlets.map((item) => {
      if (item.id === id) {
        return (updatedItem = {
          ...item,
          ...payload,
        });
      }

      return item;
    });
    return Promise.resolve(updatedItem);
  }
}
