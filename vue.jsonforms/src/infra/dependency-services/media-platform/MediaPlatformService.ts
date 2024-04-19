import { singleton } from "tsyringe";
import { IMediaPlatformService } from "./IMediaPlatformService";
import { MediaPlatform } from "@/entities/MediaPlatform";
import StaticList from "@/infra/StaticListService";

@singleton()
export default class MediaPlatformService implements IMediaPlatformService {
  private mediaPlatforms: MediaPlatform[] = StaticList.getMediaPlatforms();

  public getMediaPlatforms(): MediaPlatform[] {
    return this.mediaPlatforms;
  }

  public getMediaPlatform(id: string): MediaPlatform | undefined {
    return this.getMediaPlatforms().find((platform) => platform.id === id);
  }

  public validateUrl(url: string, id?: string): boolean {
    let regex;

    switch (id) {
      case "facebook":
      case "facebook-ads":
        regex = /^(https?:\/\/)?((w{3}\.)?)facebook.com\/.*/gi;
        break;

      case "twitter":
      case "twitter-ads":
        regex = /^(https?:\/\/)?((w{3}\.)?)twitter\.com\/(#!\/)?[a-z0-9_]+$/gi;
        break;

      case "instagram":
      case "instagram-ads":
        regex = /^\s*(http:\/\/)?instagram\.com\/[a-z\d-_]{1,255}\s*$/gi;
        break;

      case "linkedin":
      case "linkedin-ads":
        regex = /^https:\/\/[a-z]{2,3}\.linkedin\.com\/.*$/gim;
        break;

      case "google-ads":
        regex = /^https:\/\/([a-z]{2,3}\.)?google\.com\/.*$/gim;
        break;

      default:
        regex =
          /(https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9]+\.[^\s]{2,}|www\.[a-zA-Z0-9]+\.[^\s]{2,})/gi;
    }

    return new RegExp(regex).test(url);
  }
}
